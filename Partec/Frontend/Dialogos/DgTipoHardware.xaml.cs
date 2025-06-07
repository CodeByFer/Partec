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
using System.Windows.Shapes;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using Partec.MVVM;

namespace Partec.Frontend.Dialogos
{
    /// <summary>
    /// Interaction logic for DgTipoHardware.xaml
    /// </summary>
    public partial class DgTipoHardware : MetroWindow
    {
        private MVTiposHardware mv;

        public DgTipoHardware(MVTiposHardware mvTipoHardware)
        {
            InitializeComponent();
            mv = mvTipoHardware;
            DataContext = mv;
        }

        private async void BtnGuardar_Click(object sender, RoutedEventArgs e)
        {
            if (mv.IsValid(this))
            {
                if (mv.guarda)
                {
                    await this.ShowMessageAsync(
                        "Gestión de tipos de Hardware",
                        "La incidencia a sigo agregada correctamente y se le comunicará al responsable"
                    );
                    DialogResult = true;
                }
                else
                {
                    await this.ShowMessageAsync(
                        "Gestión de tipos de Hardware",
                        "Error interno, contacte con el administrados"
                    );
                }
            }
            else
            {
                await this.ShowMessageAsync(
                    "Gestión de tipos de Hardware",
                    "Tienes campos obligatorios sin rellenar correctamente"
                );
            }
        }

        private void BtnCancelar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
