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
    /// Interaction logic for UCRol.xaml
    /// </summary>
    public partial class UCRol : UserControl
    {
        private MVRol mv;
        private Task inicializa;

        public UCRol(MVRol mv, Task inicializ = null)
        {
            this.inicializa = inicializ;
            this.mv = mv;
            InitializeComponent();
            DataContext = this.mv;
        }

        private void BtnContextEliminar_Click(object sender, RoutedEventArgs e)
        {
            inicializa?.Wait();
            if (dgRoles.SelectedItem is Rol rol)
            {
                if (
                    MessageBox.Show(
                        "¿Está seguro de que desea eliminar este rol?",
                        "Confirmación",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Warning
                    ) == MessageBoxResult.Yes
                )
                {
                    if (mv.borra)
                    {
                        MessageBox.Show(
                            "Rol eliminado correctamente",
                            "Éxito",
                            MessageBoxButton.OK,
                            MessageBoxImage.Information
                        );
                    }
                    else
                    {
                        MessageBox.Show(
                            "Error al eliminar el rol",
                            "Error",
                            MessageBoxButton.OK,
                            MessageBoxImage.Error
                        );
                    }
                }
            }
        }

        private void BtnContextEditar_Click(object sender, RoutedEventArgs e)
        {
            inicializa?.Wait();
            mv.SelectedRol = (Rol)dgRoles.SelectedItem;
            DgRoles dg = new DgRoles(mv, inicializa);
            dg.ShowDialog();
        }

        private void BtnNuevoRol_Click(object sender, RoutedEventArgs e)
        {
            inicializa?.Wait();
            mv.SelectedRol = new Rol();
            DgRoles dg = new DgRoles(mv, inicializa);
            dg.ShowDialog();
        }
    }
}
