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
using Partec.Backend.Utiles;
using Partec.MVVM;

namespace Partec.Frontend.ControlUsuario
{
    /// <summary>
    /// Interaction logic for UCGraficas.xaml
    /// </summary>
    public partial class UCGraficas : UserControl
    {
        private MVGraficas mvGraficas;

        public UCGraficas(MVGraficas vm)
        {
            mvGraficas = vm;

            InitializeComponent();
            DataContext = vm;
            Loaded += async (s, e) => await vm.CargarDatosAsync();
        }

        private async void BtnRefresh_Click(object sender, RoutedEventArgs e)
        {
            _ = mvGraficas.CargarDatosAsync();
        }

        private void BtnImprimir_Click(object sender, RoutedEventArgs e)
        {
            ArchivoHelper.ExportarGraficosAPdf(grIncMes, grIncDep, grIncEst, grIncTipo);
        }
    }
}
