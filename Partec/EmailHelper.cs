using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Partec.Backend.Modelo;
using Partec.Backend.Servicios;
using PdfSharp.Drawing;
using PdfSharp.Pdf;

namespace Partec
{
    public class EmailHelper
    {
        private ProfesorServicio _profesorServicio;
        private IncidenciaServicio _incidenciaServicio;
        private GestionincidenciasContext _context;

        private static String fromEmail = "partecincidencias@gmail.com";
        private static readonly SemaphoreSlim _mutex = new SemaphoreSlim(1, 1);

        public EmailHelper(GestionincidenciasContext context)
        {
            try
            {
                context.Database.OpenConnection();
                _context = context;
            }
            catch (Exception ex) { }
            _profesorServicio = new ProfesorServicio(_context);
            _incidenciaServicio = new IncidenciaServicio(_context);
            Eventos.IncidenciaAgregada += enviarEmailAdministrador;
            Eventos.ResponsableIncidenciaAgregado += enviarEmailResponsable;
            Eventos.ResponsableIncidenciaModificado += enviarEmailResponsableAntiguo;
        }

        public async void enviarEmailResponsable(Incidencia incidencia)
        {
            await _mutex.WaitAsync();
            if (incidencia != null || incidencia.IdResponsable != null)
            {
                Attachment adjunto = GenerarResumenIncidenciaAgregadaAtta(incidencia);
                MailMessage mensajeResponsable = new MailMessage();
                mensajeResponsable.From = new MailAddress(fromEmail);
                mensajeResponsable.To.Add(
                    (
                        await _profesorServicio.FindAsync(p =>
                            p.IdProfesor == incidencia.IdResponsable
                        )
                    )
                        .FirstOrDefault()
                        .Email
                );
                mensajeResponsable.Subject = "Nueva incidencia agregada";
                mensajeResponsable.Body =
                    "Usted fue asignado/a a la incidencia de tipo "
                    + incidencia.TipoIncidencia
                    + " consulte el PDF adjunto para más detalles\n\n\n\nEste correo ha sido generado automáticamente por el sistema Partec. Por favor, no responda a este mensaje, ya que no se encuentra monitoreado.\r\nSi necesita asistencia o desea realizar alguna consulta, póngase en contacto con el equipo de soporte a través de los canales oficiales.";

                mensajeResponsable.Attachments.Add(adjunto);
                EnviarCorreoConGmail(mensajeResponsable);
            }
            _mutex.Release();
        }

        public async void enviarEmailResponsableAntiguo(
            Profesor profesorAntiguo,
            Incidencia incidencia
        )
        {
            await _mutex.WaitAsync();
            if (incidencia != null)
            {
                MailMessage mensaje = new MailMessage();
                mensaje.From = new MailAddress(fromEmail);
                mensaje.To.Add(
                    String.IsNullOrEmpty(profesorAntiguo.Email)
                        ? (
                            await _profesorServicio.FindAsync(p =>
                                p.IdProfesor == profesorAntiguo.IdProfesor
                            )
                        )
                            .FirstOrDefault()
                            .Email
                        : profesorAntiguo.Email
                );
                mensaje.Subject =
                    "Retirado de la incidencia "
                    + incidencia.IdIncidencia
                    + " del tipo "
                    + incidencia.TipoIncidencia;
                mensaje.Body =
                    "Usted fue retirado/a de ésta incidencia por un Coordinador de TIC.\n\n\n\nEste correo ha sido generado automáticamente por el sistema Partec. Por favor, no responda a este mensaje, ya que no se encuentra monitoreado.\r\nSi necesita asistencia o desea realizar alguna consulta, póngase en contacto con el equipo de soporte a través de los canales oficiales.";

                EnviarCorreoConGmail(mensaje);
            }
            _mutex.Release();
        }

        public async void enviarEmailAdministrador(Incidencia incidencia)
        {
            await _mutex.WaitAsync();
            if (incidencia != null)
            {
                IEnumerable<Profesor> listaAdministradores = await _profesorServicio.FindAsync(p =>
                    p.IdRolNavigation.NombreRol.ToLower().Equals("Coordinador TIC")
                );
                Attachment adjunto = GenerarResumenIncidenciaAgregadaAtta(incidencia);

                MailMessage mensajeAdministrador = new MailMessage();
                mensajeAdministrador.From = new MailAddress(fromEmail);

                mensajeAdministrador.Subject = "Nueva incidencia agregada";
                mensajeAdministrador.Body =
                    "Este correo ha sido generado automáticamente por el sistema Partec. Por favor, no responda a este mensaje, ya que no se encuentra monitoreado.\r\nSi necesita asistencia o desea realizar alguna consulta, póngase en contacto con el equipo de soporte a través de los canales oficiales.";
                mensajeAdministrador.Attachments.Add(adjunto);

                if (listaAdministradores.Count() > 0)
                {
                    foreach (Profesor profesor in listaAdministradores)
                    {
                        mensajeAdministrador.To.Add(profesor.Email);
                    }
                }
                EnviarCorreoConGmail(mensajeAdministrador);

                MailMessage mensajeProfesor = new MailMessage();
                mensajeProfesor.From = new MailAddress(fromEmail);
                mensajeProfesor.To.Add(incidencia.IdProfesorNavigation.Email);
                mensajeProfesor.Subject = "Nueva incidencia agregada";
                mensajeProfesor.Body =
                    "Este correo ha sido generado automáticamente por el sistema Partec. Por favor, no responda a este mensaje, ya que no se encuentra monitoreado.\r\nSi necesita asistencia o desea realizar alguna consulta, póngase en contacto con el equipo de soporte a través de los canales oficiales.";

                mensajeProfesor.Attachments.Add(adjunto);
                EnviarCorreoConGmail(mensajeProfesor);
            }
            _mutex.Release();
        }

        Attachment GenerarResumenIncidenciaAgregadaAtta(Incidencia incidencia)
        {
            string nombreArchivo = "Incidencia_" + incidencia.IdIncidencia + "_agregada.pdf"; // tu nombre personalizado
            string rutaTemp = Path.Combine(Path.GetTempPath(), nombreArchivo);
            GenerarResumenIncidenciaAgregada(incidencia, rutaTemp);
            return new Attachment(rutaTemp);
        }

        private async void EnviarCorreoConGmail(MailMessage mensaje)
        {
            try
            {
                // Configuración del servidor SMTP de Gmail
                SmtpClient clienteSmtp = new SmtpClient("smtp.gmail.com", 587);
                clienteSmtp.Credentials = new NetworkCredential(
                    "partecincidencias@gmail.com",
                    "" //AQUI VA LA CONTRASEÑA DE LA CUENTA DE GMAIL
                );
                clienteSmtp.EnableSsl = true;

                // Enviar el correo
                clienteSmtp.Send(mensaje);
                clienteSmtp.Dispose();
            }
            catch (Exception ex) { }
        }

        public static PdfDocument GenerarResumenIncidenciaAgregada(
            Incidencia incidencia,
            string rutaArchivo
        )
        {
            PdfDocument documento = new PdfDocument();
            documento.Info.Title = $"Resumen Incidencia #{incidencia.IdIncidencia}";

            PdfPage pagina = documento.AddPage();
            XGraphics gfx = XGraphics.FromPdfPage(pagina);

            // Color de fondo (Beige claro #F5EEDC)
            XSolidBrush fondo = new XSolidBrush(XColor.FromArgb(0xF5, 0xEE, 0xDC));
            gfx.DrawRectangle(fondo, 0, 0, pagina.Width, pagina.Height);

            // Colores según paleta
            XBrush colorTitulo = new XSolidBrush(XColor.FromArgb(0x27, 0x54, 0x8A)); // Azul medio
            XBrush colorEtiqueta = new XSolidBrush(XColor.FromArgb(0xDD, 0xA8, 0x53)); // Amarillo mostaza
            XBrush colorTexto = new XSolidBrush(XColor.FromArgb(0x18, 0x3B, 0x4E)); // Azul oscuro

            XFont tituloFont = new XFont("Arial", 16, XFontStyleEx.Bold);
            XFont etiquetaFont = new XFont("Arial", 12, XFontStyleEx.Bold);
            XFont textoFont = new XFont("Arial", 12, XFontStyleEx.Regular);

            double y = 40;

            void AgregarLinea(string etiqueta, string valor)
            {
                // Dibujar etiqueta en color amarillo mostaza
                gfx.DrawString(
                    $"{etiqueta}: ",
                    etiquetaFont,
                    colorEtiqueta,
                    new XRect(40, y, pagina.Width - 80, 20),
                    XStringFormats.TopLeft
                );

                // Medir ancho de la etiqueta para posicionar el texto
                double anchoEtiqueta = gfx.MeasureString($"{etiqueta}: ", etiquetaFont).Width;

                // Dibujar el valor en azul oscuro justo después de la etiqueta
                gfx.DrawString(
                    valor,
                    textoFont,
                    colorTexto,
                    new XRect(40 + anchoEtiqueta, y, pagina.Width - 80 - anchoEtiqueta, 20),
                    XStringFormats.TopLeft
                );

                y += 20;
            }

            gfx.DrawString(
                "Resumen de Incidencia",
                tituloFont,
                colorTitulo,
                new XRect(0, y, pagina.Width, 40),
                XStringFormats.TopCenter
            );
            y += 40;

            AgregarLinea("Tipo", incidencia.TipoIncidencia);
            AgregarLinea("Fecha Incidencia", incidencia.FechaIncidencia.ToString("g"));
            AgregarLinea("Fecha Introducción", incidencia.FechaIntroduccion.ToString("g"));
            AgregarLinea(
                "Profesor que ha insertado la incidencia",
                incidencia.IdProfesorNavigation.Nombre
                    + " "
                    + incidencia.IdProfesorNavigation.Apellidos
            );
            AgregarLinea(
                "Departamento de la incidencia",
                incidencia.IdDepartamentoNavigation.Nombre
            );
            AgregarLinea("Ubicación", incidencia.Ubicacion);
            if (incidencia.TipoIncidencia == "HW")
            {
                AgregarLinea("Tipo HW", incidencia.IdTipoHwNavigation.Descripcion ?? "N/A");
                AgregarLinea("Modelo", incidencia.Modelo ?? "N/A");
                AgregarLinea("Número Serie", incidencia.NumeroSerie ?? "N/A");
            }

            AgregarLinea("Descripción", incidencia.Descripcion);
            AgregarLinea("Observaciones", incidencia.Observaciones ?? "N/A");
            AgregarLinea("Estado", incidencia.IdEstadoNavigation.DescripcionEstado);

            documento.Save(rutaArchivo);

            // (Opcional) Abrir automáticamente
            Process.Start(new ProcessStartInfo(rutaArchivo) { UseShellExecute = true });

            return documento;
        }
    }
}
