namespace Partec
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using Google.Protobuf.Collections;
    using MahApps.Metro.Controls;
    using MaterialDesignThemes.Wpf;
    using Partec.Backend.Modelo;
    using Partec.Frontend.ControlUsuario;
    using Partec.MVVM;

    public partial class MainWindow : MetroWindow
    {
        private GestionincidenciasContext context;

        public MVLogin mvLogin { get; set; }

        private Dictionary<DateTime, List<string>> incidencias =
            new Dictionary<DateTime, List<string>>();

        public MainWindow(GestionincidenciasContext context, MVLogin mvLogin)
        {
            InitializeComponent();
            this.context = context;
            this.mvLogin = mvLogin;
            this.DataContext = this.mvLogin;
            _ = Initialize();
        }

        public MainWindow()
        {
            InitializeComponent();
        }

        public async Task<Boolean> Initialize()
        {
            //TODO agregar los permisos de la base de datos con un bucle iterable
            TabItem tab = CrearTab(
                "tbiIncidencia",
                "Incidencia",
                PackIconKind.Inbox,
                "#FFDDA853",
                "#183B4E",
                new UCIncidencia()
            );
            TabItem tab2 = CrearTab(
                "tbiIncidencia",
                "Incidencia",
                PackIconKind.Inbox,
                "#FFDDA853",
                "#183B4E",
                new UCIncidencia()
            );
            tbcMain.Items.Add(tab);
            tbcMain.Items.Add(tab2);

            return true;
        }

        private TabItem CrearTab(
            string nombreTab,
            string tituloHeader,
            PackIconKind iconoHeader,
            string fondoTabHex,
            string colorTextoHex,
            UserControl vista
        )
        {
            TabItem tabItem = new TabItem
            {
                Name = nombreTab,
                Margin = new Thickness(10),
                Background = new SolidColorBrush(
                    (Color)ColorConverter.ConvertFromString(fondoTabHex)
                ),
                Foreground = new SolidColorBrush(
                    (Color)ColorConverter.ConvertFromString(colorTextoHex)
                ),
                Style = (Style)Application.Current.FindResource("MaterialDesignTabItem"),
            };

            WrapPanel headerPanel = new WrapPanel
            {
                Margin = new Thickness(0),
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
            };

            headerPanel.Children.Add(
                new PackIcon
                {
                    Kind = iconoHeader,
                    Height = 20,
                    Width = 20,
                    VerticalAlignment = VerticalAlignment.Center,
                }
            );

            headerPanel.Children.Add(
                new TextBlock
                {
                    Text = tituloHeader,
                    FontSize = 20,
                    Margin = new Thickness(5, 0, 0, 0),
                    VerticalAlignment = VerticalAlignment.Center,
                }
            );

            tabItem.Header = headerPanel;
            tabItem.Content = vista;
            return tabItem;
        }

        //Zona de Clicks, aquí se pondrán todos

        private void BotonIncidencia_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Has hecho clic en un botón de incidencia.", "Incidencia");
        }

        private void minButton_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void maxButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.WindowState == WindowState.Normal)
                this.WindowState = WindowState.Maximized;
            else
                this.WindowState = WindowState.Normal;
        }

        private void exitButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void logoutButton_Click(object sender, RoutedEventArgs e)
        {
            mvLogin = null;
            (new Login()).Show();

            this.Close();
        }
    }
}
