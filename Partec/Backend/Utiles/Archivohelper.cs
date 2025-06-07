using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using LiveChartsCore;
using LiveChartsCore.Drawing;
using LiveChartsCore.Geo;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Drawing.Geometries;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.SkiaSharpView.SKCharts;
using LiveChartsCore.SkiaSharpView.VisualElements;
using LiveChartsCore.SkiaSharpView.WPF;
using Microsoft.Win32;
using Partec.Backend.Modelo;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using SkiaSharp;

namespace Partec.Backend.Utiles
{
    public static class ArchivoHelper
    {
        public static List<Archivo> GuardarArchivosIncidenciaYCrearObjetos(
            Incidencia incidencia,
            List<string> rutasOriginales
        )
        {
            var archivos = new List<Archivo>();

            string carpetaDestinoRelativa = Path.Combine(
                "Partec",
                "Incidencias",
                $"Incidencia{incidencia.IdIncidencia}"
            );
            string carpetaDestinoAbsoluta = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                carpetaDestinoRelativa
            );

            if (!Directory.Exists(carpetaDestinoAbsoluta))
                Directory.CreateDirectory(carpetaDestinoAbsoluta);

            foreach (var rutaOriginal in rutasOriginales)
            {
                string nombreArchivo = Path.GetFileName(rutaOriginal);
                string extension = Path.GetExtension(nombreArchivo);
                string tipo = DeterminarTipoArchivo(extension); // 'grafico', 'resumen', etc.

                string rutaDestinoCompleta = Path.Combine(carpetaDestinoAbsoluta, nombreArchivo);

                try
                {
                    var archivo = new Archivo
                    {
                        RutaRelativa = rutaOriginal,
                        TipoArchivo = tipo,
                        IdIncidencia = incidencia.IdIncidencia,
                    };

                    archivos.Add(archivo);
                }
                catch (Exception ex)
                {
                    // Puedes registrar o mostrar el error si algún archivo falla
                    Console.WriteLine($"Error copiando archivo: {ex.Message}");
                }
            }

            return archivos;
        }

        private static string DeterminarTipoArchivo(string extension)
        {
            extension = extension.ToLowerInvariant();

            var extensionesGraficas = new[]
            {
                ".bmp",
                ".jpg",
                ".jpeg",
                ".png",
                ".gif",
                ".tiff",
                ".ico",
                ".webp",
                ".heic",
                ".avif",
            };
            var extensionesResumen = new[] { ".txt", ".rtf" };
            var extensionesInforme = new[]
            {
                ".pdf",
                ".doc",
                ".docx",
                ".xls",
                ".xlsx",
                ".ppt",
                ".pptx",
            };

            if (extensionesGraficas.Contains(extension))
                return "grafico";
            if (extensionesResumen.Contains(extension))
                return "resumen";
            if (extensionesInforme.Contains(extension))
                return "informe";
            return "otro";
        }

        public static List<Archivo> CrearObjetosArchivoSinCopiar(
            Incidencia incidencia,
            List<string> rutas
        )
        {
            var archivos = new List<Archivo>();
            string carpetaRelativa = Path.Combine(
                "Incidencias",
                $"Incidencia{incidencia.IdIncidencia}"
            );

            foreach (var rutaOriginal in rutas)
            {
                string nombreArchivo = Path.GetFileName(rutaOriginal);
                string extension = Path.GetExtension(nombreArchivo);
                string tipo = "otro";

                // Guardar solo la ruta relativa en la propiedad
                string rutaRelativa = Path.Combine(carpetaRelativa, nombreArchivo);

                var archivo = new Archivo
                {
                    RutaRelativa = rutaRelativa,
                    TipoArchivo = tipo,
                    IdIncidencia = incidencia.IdIncidencia,
                };

                // Guardar la ruta original temporalmente (por ejemplo, en Tag o una propiedad auxiliar)
                archivo.RutaRelativa = rutaOriginal; // O usa un diccionario externo si prefieres no usar Tag

                archivos.Add(archivo);
            }

            return archivos;
        }

        public static void CopiarArchivoFisicamente(Archivo archivo)
        {
            string? origen = archivo.RutaRelativa as string;
            if (string.IsNullOrEmpty(origen) || !File.Exists(origen))
                return;

            string destinoAbsoluto = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                archivo.RutaRelativa
            );

            string? carpetaDestino = Path.GetDirectoryName(destinoAbsoluto);
            if (!Directory.Exists(carpetaDestino))
                Directory.CreateDirectory(carpetaDestino);

            File.Copy(origen, destinoAbsoluto, overwrite: true);
        }

        public static void GuardarArchivosIncidencia(
            Incidencia incidencia,
            List<string> rutasArchivos
        )
        {
            // Ruta base: Documentos\Incidencias\<ID>
            string carpetaDocumentos = Environment.GetFolderPath(
                Environment.SpecialFolder.MyDocuments
            );
            string carpetaDestino = Path.Combine(
                carpetaDocumentos,
                "Incidencias",
                incidencia.IdIncidencia.ToString()
            );

            // Crear la carpeta si no existe
            if (!Directory.Exists(carpetaDestino))
            {
                Directory.CreateDirectory(carpetaDestino);
            }

            foreach (var ruta in rutasArchivos)
            {
                if (File.Exists(ruta))
                {
                    string nombreArchivo = Path.GetFileName(ruta);
                    string destino = Path.Combine(carpetaDestino, nombreArchivo);

                    File.Copy(ruta, destino, overwrite: true);
                }
                else
                {
                    Console.WriteLine($"Archivo no encontrado: {ruta}");
                }
            }
        }

        public static List<string> ObtenerArchivosIncidencia(Incidencia incidencia)
        {
            string carpetaDocumentos = Environment.GetFolderPath(
                Environment.SpecialFolder.MyDocuments
            );
            string carpetaIncidencia = Path.Combine(
                carpetaDocumentos,
                "Partec", // <-- Añadido aquí
                "Incidencias",
                $"Incidencia{incidencia.IdIncidencia}"
            );

            if (Directory.Exists(carpetaIncidencia))
            {
                return new List<string>(Directory.GetFiles(carpetaIncidencia));
            }

            return new List<string>(); // Vacío si no hay carpeta
        }

        public static void AbrirArchivo(string rutaArchivo)
        {
            if (File.Exists(rutaArchivo))
            {
                Process.Start(
                    new ProcessStartInfo { FileName = rutaArchivo, UseShellExecute = true }
                );
            }
            else
            {
                Console.WriteLine($"Archivo no encontrado: {rutaArchivo}");
            }
        }

        public static void ExportarGraficosAPdf(
            CartesianChart grIncMes,
            CartesianChart grIncDep,
            PieChart grIncEst,
            PieChart grIncTipo
        )
        {
            var saveFileDialog = new SaveFileDialog
            {
                Filter = "PDF files (*.pdf)|*.pdf",
                Title = "Guardar Gráficos como PDF",
                FileName = "GraficosIncidencias.pdf",
            };

            if (saveFileDialog.ShowDialog() != true)
                return;

            var documento = new PdfDocument();
            double width = 900;
            double height = 600;

            // Agregar gráficos uno por uno
            AgregarGraficoCartesianAPdf(documento, grIncMes, "Incidencias por Mes", width, height);
            AgregarGraficoCartesianAPdf(
                documento,
                grIncDep,
                "Incidencias por Departamento",
                width,
                height
            );
            AgregarGraficoPieAPdf(documento, grIncEst, "Incidencias por Estado", width, height);
            AgregarGraficoPieAPdf(documento, grIncTipo, "Incidencias por Tipo", width, height);

            documento.Save(saveFileDialog.FileName);

            MessageBox.Show(
                "PDF generado correctamente en:\n" + saveFileDialog.FileName,
                "Exportación completada",
                MessageBoxButton.OK,
                MessageBoxImage.Information
            );
        }

        private static void AgregarGraficoCartesianAPdf(
            PdfDocument doc,
            CartesianChart chart,
            string titulo,
            double width,
            double height
        )
        {
            var skChart = new SKCartesianChart(chart) { Width = (int)width, Height = (int)height };

            AgregarGraficoSkiaAPdf(doc, skChart.GetImage(), titulo, width, height);
        }

        private static void AgregarGraficoPieAPdf(
            PdfDocument doc,
            PieChart chart,
            string titulo,
            double width,
            double height
        )
        {
            var skChart = new SKPieChart(chart) { Width = (int)width, Height = (int)height };

            AgregarGraficoSkiaAPdf(doc, skChart.GetImage(), titulo, width, height);
        }

        private static void AgregarGraficoSkiaAPdf(
            PdfDocument doc,
            SKImage skImage,
            string titulo,
            double width,
            double height
        )
        {
            double margin = 20;

            var pagina = doc.AddPage();
            pagina.Width = width * 72.0 / 96.0;
            pagina.Height = height * 72.0 / 96.0;

            var gfx = XGraphics.FromPdfPage(pagina);

            using SKData data = skImage.Encode(SKEncodedImageFormat.Png, 100);
            byte[] imageBytes = data.ToArray();
            using var ms = new MemoryStream(imageBytes);
            XImage pdfImage = XImage.FromStream(ms);

            // Título
            gfx.DrawString(
                titulo,
                new XFont("Arial", 16, XFontStyleEx.Bold),
                XBrushes.Black,
                new XRect(margin, margin, pagina.Width - 2 * margin, 25),
                XStringFormats.TopLeft
            );

            // Imagen
            double imageY = margin + 25;
            gfx.DrawImage(
                pdfImage,
                margin,
                imageY,
                pagina.Width - 2 * margin,
                pagina.Height - imageY - margin
            );
        }
    }
}
