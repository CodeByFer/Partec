using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using MaterialDesignThemes.Wpf;
using Partec.Backend.Modelo;
using Partec.Backend.Utiles;
using Partec.MVVM;
using Button = System.Windows.Controls.Button;
using ComboBox = System.Windows.Controls.ComboBox;
using ListBox = System.Windows.Controls.ListBox;
using Orientation = System.Windows.Controls.Orientation;

namespace Partec.Frontend.Dialogos
{
    /// <summary>
    /// Interaction logic for DgIncidencias.xaml
    /// </summary>
    public partial class DgIncidencias : MetroWindow
    {
        MVIncidencia mv;

        public DgIncidencias(
            MVIncidencia mVIncidencia,
            Incidencia incidencia = null,
            Task inicioMv = null
        )
        {
            InitializeComponent();
            if (inicioMv != null)
            {
                inicioMv.Wait();
            }
            mv = mVIncidencia;

            if (incidencia != null)
            {
                mv.lstArchivos = ArchivoHelper.ObtenerArchivosIncidencia(incidencia);
            }
            else
            {
                mv.lstArchivos = new List<string>();
            }
            mv.SelectedIncidencia = incidencia ?? new Incidencia();
            this.DataContext = mv;
        }

        private void BtnAñadirArchivo_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Title = "Seleccionar archivos",
                Filter =
                    "Imágenes|*.bmp;*.jpg;*.jpeg;*.png;*.gif;*.tiff;*.ico;*.webp;*.heic;*.avif|"
                    + "Documentos|*.pdf;*.doc;*.docx;*.xls;*.xlsx;*.ppt;*.pptx;*.txt;*.rtf|"
                    + "Todos los archivos|*.*",
                Multiselect = true,
            };

            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string[] rutasSeleccionadas = openFileDialog.FileNames;

                // Crear objetos Archivo (sin copiar aún)
                var nuevosArchivos = ArchivoHelper.GuardarArchivosIncidenciaYCrearObjetos(
                    mv.SelectedIncidencia,
                    rutasSeleccionadas.ToList()
                );

                mv.ListaArchivosAuxiliar.AddRange(nuevosArchivos);

                // Mostrar nombres en la lista visual (no hay que mostrar aún los copiados físicamente)
                if (mv.lstArchivos == null)
                {
                    mv.lstArchivos = new List<string>();
                }

                foreach (var ruta in rutasSeleccionadas)
                {
                    mv.lstArchivos.Add(ruta);
                }
            }
        }

        private void MinButton_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void MaxButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.WindowState == WindowState.Normal)
                this.WindowState = WindowState.Maximized;
            else
                this.WindowState = WindowState.Normal;
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e) =>
            BtnCancelar_Click(sender, e);

        private async void BtnGuardar_Click(object sender, RoutedEventArgs e)
        {
            if (mv.IsValid(this))
            {
                if (mv.guarda)
                {
                    await this.ShowMessageAsync(
                        "Gestión Inicdencias",
                        "La incidencia ha sido agregada correctamente y se le comunicará al responsable"
                    );
                    DialogResult = true;
                }
                else
                {
                    await this.ShowMessageAsync(
                        "Gestión Incidencias",
                        "Error interno, contacte con el administrador"
                    );
                }
            }
            else
            {
                await this.ShowMessageAsync(
                    "Gestión Incidencias",
                    "Tienes campos obligatorios sin rellenar correctamente"
                );
            }
        }

        private void BtnCancelar_Click(object sender, RoutedEventArgs e)
        {
            mv.CancelarIncidencia();
            this.Close();
        }

        private void BtnAbrir_Click(object sender, RoutedEventArgs e)
        {
            var boton = sender as Button;

            if (boton != null)
            {
                ArchivoHelper.AbrirArchivo((String)boton.Content);
            }
        }
    }
}
