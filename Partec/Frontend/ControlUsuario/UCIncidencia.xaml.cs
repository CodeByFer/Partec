using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Partec.Backend.Modelo;
using Partec.Frontend.Dialogos;
using Partec.MVVM;

namespace Partec.Frontend.ControlUsuario
{
    /// <summary>
    /// Interaction logic for UCIncidencia.xaml
    /// </summary>
    public partial class UCIncidencia : UserControl
    {
        private MVIncidencia mvIncidencia;
        private Task inicializacionIncidenciaTask;

        public UCIncidencia(MVIncidencia mv, Task inicializacionIncidenciaTask = null)
        {
            mvIncidencia = mv;
            InitializeComponent();
            this.DataContext = mvIncidencia;
            this.inicializacionIncidenciaTask = inicializacionIncidenciaTask;
            dgIncidencia.Items.Refresh();
        }

        private async void BtnNuevaIncidencia_Click(object sender, RoutedEventArgs e)
        {
            await inicializacionIncidenciaTask;
            mvIncidencia.SelectedIncidencia = new Incidencia();
            DgIncidencias dginci = new DgIncidencias(
                mvIncidencia,
                inicioMv: inicializacionIncidenciaTask
            );
            dginci.ShowDialog();
        }

        private async void BtnContextEditar_Click(object sender, RoutedEventArgs e)
        {
            await inicializacionIncidenciaTask;
            mvIncidencia.SelectedIncidencia = (Backend.Modelo.Incidencia)dgIncidencia.SelectedItem;
            if (mvIncidencia.SelectedIncidencia.IdResponsable != null)
            {
                mvIncidencia.profesorAntiguo = new Profesor()
                {
                    IdProfesor = (int)mvIncidencia.SelectedIncidencia.IdResponsable,
                };
            }

            Incidencia articuloAux = mvIncidencia.Clonar;
            DgIncidencias ac = new DgIncidencias(
                mvIncidencia,
                inicioMv: inicializacionIncidenciaTask,
                incidencia: mvIncidencia.SelectedIncidencia
            );
            ac.ShowDialog();

            if (ac.DialogResult.Equals(true))
            {
                dgIncidencia.Items.Refresh();
                mvIncidencia.SelectedIncidencia = new Incidencia();
            }
            else
            {
                mvIncidencia.SelectedIncidencia = articuloAux;
                dgIncidencia.SelectedItem = articuloAux;
            }
        }

        private async void BtnContextEliminar_Click(object sender, RoutedEventArgs e)
        {
            await inicializacionIncidenciaTask;
            mvIncidencia.SelectedIncidencia = (Backend.Modelo.Incidencia)dgIncidencia.SelectedItem;

            if (mvIncidencia.borrar)
            {
                MessageBox.Show("Incidencia eliminada correctamente", "Gestión de incidencias");
            }
            else
            {
                MessageBox.Show(
                    "Error al intentar eliminar la incidencia",
                    "Gestión de incidencias"
                );
            }

            dgIncidencia.Items.Refresh();
        }

        private void BtnLimpiarFiltro_Click(object sender, RoutedEventArgs e)
        {
            mvIncidencia.LimpiarFiltros();
        }
    }
}
