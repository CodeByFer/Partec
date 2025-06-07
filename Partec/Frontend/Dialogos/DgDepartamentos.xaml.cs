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
    /// Interaction logic for DgDepartamentos.xaml
    /// </summary>
    public partial class DgDepartamentos : MetroWindow
    {
        private MVDepartamento mvDepartamento;

        public DgDepartamentos(MVDepartamento mv)
        {
            mvDepartamento = mv;
            DataContext = mvDepartamento;
            InitializeComponent();
        }

        private async void BtnGuardar_Click(object sender, RoutedEventArgs e)
        {
            if (mvDepartamento.guarda)
            {
                await this.ShowMessageAsync(
                    "Gestión de Departamentos",
                    "Departamento guardado correctamente"
                );
                DialogResult = true;
            }
            else
            {
                await this.ShowMessageAsync(
                    "Gestión de Departamentos",
                    "Error al guardar el departamento"
                );
            }
        }

        private void BtnCancelar_Click(object sender, RoutedEventArgs e)
        {
            mvDepartamento.cancelar();
            this.Close();
        }
    }
}
