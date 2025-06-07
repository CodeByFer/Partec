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
    /// Interaction logic for UCProfesor.xaml
    /// </summary>
    public partial class UCProfesor : UserControl
    {
        private MVLogin mv;
        private Task inicializacionProfesorTask;

        public UCProfesor(MVLogin mv, Task inicializa = null)
        {
            this.mv = mv;
            InitializeComponent();
            this.DataContext = this.mv;
            this.inicializacionProfesorTask = inicializa;
            dgProfesor.Items.Refresh();
        }

        private async void BtnNuevoProfesor_Click(object sender, RoutedEventArgs e)
        {
            if (inicializacionProfesorTask != null)
            {
                await inicializacionProfesorTask;
            }
            mv.SelectedProfesor = new Profesor();
            DgProfesores dgprofe = new DgProfesores(mv, inicializacionProfesorTask);
            dgprofe.ShowDialog();
        }

        private void BtnContextEditar_Click(object sender, RoutedEventArgs e)
        {
            if (dgProfesor.SelectedItem is Profesor profesor)
            {
                mv.SelectedProfesor = profesor;
                DgProfesores dgprofe = new DgProfesores(mv, inicializacionProfesorTask);
                dgprofe.ShowDialog();
            }
            else { }
        }

        private async void BtnContextEliminar_Click(object sender, RoutedEventArgs e)
        {
            await inicializacionProfesorTask;
            mv.SelectedProfesor = (Backend.Modelo.Profesor)dgProfesor.SelectedItem;

            if (mv.Borrar)
            {
                MessageBox.Show("Profesor/a eliminado/a correctamente", "Gestión de Profesores");
            }
            else
            {
                MessageBox.Show(
                    "Error al intentar eliminar el profesor / la profesora",
                    "Gestión de Profesores"
                );
            }

            dgProfesor.Items.Refresh();
        }
    }
}
