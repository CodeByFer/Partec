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
    /// Interaction logic for DgProfesores.xaml
    /// </summary>
    public partial class DgProfesores : MetroWindow
    {
        private MVLogin mv;

        public DgProfesores(MVLogin mv, Task inicializa = null)
        {
            this.mv = mv;
            this.DataContext = this.mv;
            inicializa?.Wait();
            InitializeComponent();
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

        private void BtnCancelar_Click(object sender, RoutedEventArgs e)
        {
            mv.CancelarProfesor();
            this.Close();
        }

        private async void BtnGuardar_Click(object sender, RoutedEventArgs e)
        {
            if (mv.IsValid(this))
            {
                if (mv.Guarda)
                {
                    await this.ShowMessageAsync(
                        "Gestión de profesores",
                        "El profesor/a ha sido agregado correctamente"
                    );
                    DialogResult = true;
                }
                else
                {
                    await this.ShowMessageAsync(
                        "Gestión de profesores",
                        "El profesor/a ha sido agregado correctamente"
                    );
                }
            }
            else
            {
                await this.ShowMessageAsync(
                    "Gestión de profesores",
                    "Tienes campos obligatorios sin rellenar correctamente"
                );
            }
        }
    }
}
