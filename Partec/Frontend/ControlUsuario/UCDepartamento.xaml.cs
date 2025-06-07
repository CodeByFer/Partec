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
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using Partec.Backend.Modelo;
using Partec.Frontend.Dialogos;
using Partec.MVVM;

namespace Partec.Frontend.ControlUsuario
{
    /// <summary>
    /// Interaction logic for UCDepartamento.xaml
    /// </summary>
    public partial class UCDepartamento : UserControl
    {
        private MVDepartamento mv;
        private Task inicializa;

        public UCDepartamento(MVDepartamento mv, Task inicializa)
        {
            this.inicializa = inicializa;
            this.mv = mv;
            InitializeComponent();
            DataContext = this.mv;
        }

        private void BtnNuevoDepartamento_Click(object sender, RoutedEventArgs e)
        {
            inicializa?.Wait();
            mv.SelectedDepartamento = new Departamento();
            DgDepartamentos dialogo = new DgDepartamentos(mv);
            dialogo.ShowDialog();
        }

        private void BtnContextEditar_Click(object sender, RoutedEventArgs e)
        {
            inicializa?.Wait();
            mv.SelectedDepartamento = (Departamento)dgDepartamentos.SelectedItem;
            DgDepartamentos dialogo = new DgDepartamentos(mv);
            dialogo.ShowDialog();
        }

        private void BtnContextEliminar_Click(object sender, RoutedEventArgs e)
        {
            inicializa?.Wait();
            var metroWindow = (MetroWindow)Window.GetWindow(this);
            if (mv.SelectedDepartamento.Incidencia.Count != 0)
            {
                metroWindow.ShowMessageAsync(
                    "Gestión de departamentos",
                    "No se puede eliminar el departamento porque tiene incidencias asociadas"
                );
            }
            else
            {
                if (mv.borra)
                {
                    metroWindow.ShowMessageAsync(
                        "Gestión de departamentos",
                        "Departamento eliminado correctamente"
                    );
                }
            }
        }
    }
}
