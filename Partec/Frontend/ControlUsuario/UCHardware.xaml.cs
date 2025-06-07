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
    /// Interaction logic for UCHardware.xaml
    /// </summary>
    public partial class UCHardware : UserControl
    {
        private MVTiposHardware mv;
        private Task inicializacionTask;

        public UCHardware(MVTiposHardware mv, Task inicializacion = null)
        {
            this.mv = mv;

            InitializeComponent();
            this.DataContext = this.mv;
            this.inicializacionTask = inicializacion;
        }

        private void BtnContextEditar_Click(object sender, RoutedEventArgs e)
        {
            inicializacionTask?.Wait();
            DgTipoHardware dialogo = new DgTipoHardware(mv);
            dialogo.ShowDialog();
        }

        private async void BtnContextEliminar_Click(object sender, RoutedEventArgs e)
        {
            inicializacionTask?.Wait();
            var metroWindow = (MetroWindow)Window.GetWindow(this);
            if (mv.SelectedTipoHardware.Incidencia.Count != 0)
            {
                await metroWindow.ShowMessageAsync(
                    "Gestión de tipos de Hardware",
                    "No se puede eliminar el tipo de hardware porque tiene incidencias asociadas"
                );
            }
            else
            {
                if (mv.borra)
                {
                    await metroWindow.ShowMessageAsync(
                        "Gestión de tipos de Hardware",
                        "Tipo de Hardware elimnado correctamente"
                    );
                }
            }
        }

        private void BtnNuevoHardware_Click(object sender, RoutedEventArgs e)
        {
            inicializacionTask?.Wait();

            mv.SelectedTipoHardware = new TipoHardware();

            DgTipoHardware dialogo = new DgTipoHardware(mv);
            dialogo.ShowDialog();
        }
    }
}
