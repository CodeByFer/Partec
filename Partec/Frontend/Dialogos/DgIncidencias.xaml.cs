using System;
using System.Collections.Generic;
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
using Partec.Backend.Modelo;
using Partec.MVVM;

namespace Partec.Frontend.Dialogos
{
    /// <summary>
    /// Interaction logic for DgIncidencias.xaml
    /// </summary>
    public partial class DgIncidencias : MetroWindow
    {
        private GestionincidenciasContext context;
        MVIncidencia mv;

        public DgIncidencias(GestionincidenciasContext contexto)
        {
            InitializeComponent();
            this.context = contexto;
            mv = new MVIncidencia(this.context);

            this.DataContext = mv;
        }

        private void BtnAñadirArchivo_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "Seleccionar archivos";
            openFileDialog.Filter =
                "Imágenes|*.bmp;*.jpg;*.jpeg;*.png;*.gif;*.tiff;*.ico;*.webp;*.heic;*.avif|"
                + "Documentos|*.pdf;*.doc;*.docx;*.xls;*.xlsx;*.ppt;*.pptx;*.txt;*.rtf|"
                + "Todos los archivos|*.*";
            openFileDialog.Multiselect = true; // 👉 habilita selección múltiple

            // Fix: Use DialogResult.OK to compare with the result of ShowDialog()
            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string[] rutasArchivos = openFileDialog.FileNames;

                // Mostrar todas las rutas en un TextBlock (o en una ListBox si quieres algo más bonito)
                lstArchivos.Items.Clear();
                foreach (string ruta in rutasArchivos)
                {
                    lstArchivos.Items.Add(System.IO.Path.GetFileName(ruta));
                }
            }
        }
    }
}
