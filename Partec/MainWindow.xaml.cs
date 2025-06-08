namespace Partec
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using Google.Protobuf.Collections;
    using MahApps.Metro.Controls;
    using MaterialDesignThemes.Wpf;
    using Partec.Backend.Modelo;
    using Partec.Backend.Servicios;
    using Partec.Frontend.ControlUsuario;
    using Partec.Frontend.Dialogos;
    using Partec.MVVM;

    public partial class MainWindow : MetroWindow
    {
        private GestionincidenciasContext context;
        private MVIncidencia mvIncidencia;
        private MVTiposHardware mvTiposHardware;
        private MVDepartamento mvDepartamento;
        private MVRol MVRol;
        public MVLogin mvLogin { get; set; }

        // Guarda la tarea
        private Task inicializacionRolTask;
        private Task inicializacionIncidenciaTask;
        private Task inicializacionHardwareTask;
        private Task inicializacionProfesorTask;
        private Task inicializacionDepartamentoTask;
        private List<String> permisos;
        EmailHelper emailHelper;

        public MainWindow(GestionincidenciasContext context, MVLogin mvLogin)
        {
            InitializeComponent();
            this.context = context;
            this.mvLogin = mvLogin;
            this.DataContext = this.mvLogin;
            Initialize();
            this.emailHelper = new EmailHelper(new GestionincidenciasContext());
            Eventos.RolModificado += Initialize;
        }

        public async void Initialize()
        {
            tbcMain.Items.Clear();

            permisos = mvLogin
                .usuario.IdRolNavigation.IdPermisos.Select(p => p.NombreControl)
                .ToList();

            if (permisos.Contains("incidencia_admin"))
            {
                //Control sobre incidencias
                mvIncidencia = new MVIncidencia(
                    new GestionincidenciasContext(),
                    mvLogin.usuario,
                    admin: true
                );
                this.inicializacionIncidenciaTask = mvIncidencia.inicializa();
                TabItem tbiIncidenciaManejo = CrearTab(
                    "tbiIncidencia",
                    "Manejo de incidencias",
                    PackIconKind.Inbox,
                    "#FFDDA853",
                    "#183B4E",
                    new UCIncidencia(mvIncidencia, inicializacionIncidenciaTask)
                );
                tbcMain.Items.Add(tbiIncidenciaManejo);
            }
            else if (permisos.Contains("incidencia_responsable"))
            {
                //TODO Agregar el true para las cosas
                mvIncidencia = new MVIncidencia(
                    new GestionincidenciasContext(),
                    mvLogin.usuario,
                    admin: false,
                    responsable: true
                );
                this.inicializacionIncidenciaTask = mvIncidencia.inicializa();
                TabItem tbiIncidenciaManejo = CrearTab(
                    "tbiIncidencia",
                    "Manejo de incidencias",
                    PackIconKind.Inbox,
                    "#FFDDA853",
                    "#183B4E",
                    new UCIncidencia(mvIncidencia, inicializacionIncidenciaTask)
                );
                tbcMain.Items.Add(tbiIncidenciaManejo);
            }
            else if (permisos.Contains("incidencia_edit_own"))
            {
                mvIncidencia = new MVIncidencia(
                    new GestionincidenciasContext(),
                    mvLogin.usuario,
                    admin: false,
                    responsable: false
                );
                this.inicializacionIncidenciaTask = mvIncidencia.inicializa();
                TabItem tbiIncidenciaManejo = CrearTab(
                    "tbiIncidencia",
                    "Manejo de incidencias",
                    PackIconKind.Inbox,
                    "#FFDDA853",
                    "#183B4E",
                    new UCIncidencia(mvIncidencia, inicializacionIncidenciaTask)
                );
                tbcMain.Items.Add(tbiIncidenciaManejo);
            }
            if (permisos.Contains("usuario_crud"))
            {
                this.inicializacionProfesorTask = mvLogin.Inicializa();
                TabItem tbProfesor = CrearTab(
                    "tbProfesor",
                    "Manejo de Profesor",
                    PackIconKind.Account,
                    "#FFDDA853",
                    "#183B4E",
                    new UCProfesor(mvLogin, inicializacionProfesorTask)
                );
                tbcMain.Items.Add(tbProfesor);
            }
            if (permisos.Contains("departamento_admin"))
            {
                mvDepartamento = new MVDepartamento(new GestionincidenciasContext());
                this.inicializacionDepartamentoTask = mvDepartamento.Inicializa();
                TabItem tbDepartamento = CrearTab(
                    "tbDepartamento",
                    "Gestión de Departamentos",
                    PackIconKind.BriefcaseAccount,
                    "#FFDDA853",
                    "#183B4E",
                    new UCDepartamento(mvDepartamento, inicializacionDepartamentoTask)
                );
                tbcMain.Items.Add(tbDepartamento);
            }
            if (permisos.Contains("tipohw_manage"))
            {
                mvTiposHardware = new MVTiposHardware(new GestionincidenciasContext());
                this.inicializacionHardwareTask = mvTiposHardware.inicializa();
                TabItem tbTipoHardware = CrearTab(
                    "tbTipoHardware",
                    "Tipo Hardware",
                    PackIconKind.Harddisk,
                    "#FFDDA853",
                    "#183B4E",
                    new UCHardware(mvTiposHardware, inicializacionHardwareTask)
                );
                tbcMain.Items.Add(tbTipoHardware);
            }
            if (permisos.Contains("roles_permisos_manage"))
            {
                MVRol = new MVRol(new GestionincidenciasContext());
                this.inicializacionRolTask = MVRol.inicializa();
                TabItem tbRol = CrearTab(
                    "tbRol",
                    "Gestión de Roles",
                    PackIconKind.Security,
                    "#FFDDA853",
                    "#183B4E",
                    new UCRol(MVRol, inicializacionRolTask)
                );
                tbcMain.Items.Add(tbRol);
            }
            if (permisos.Contains("informes_view"))
            {
                var vmGraficas = new MVGraficas(new GestionincidenciasContext());
                var ucGraficas = new UCGraficas(vmGraficas);
                TabItem tbGraficas = CrearTab(
                    "tbGraficas",
                    "Gráficos",
                    PackIconKind.ChartBar,
                    "#FFDDA853",
                    "#183B4E",
                    ucGraficas
                );
                tbcMain.Items.Add(tbGraficas);
            }
        }

        private TabItem CrearTab(
            string nombreTab,
            string tituloHeader,
            PackIconKind iconoHeader,
            string fondoTabHex,
            string colorTextoHex,
            UserControl vista,
            Object content = null
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
                HorizontalAlignment = HorizontalAlignment.Left,
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
            if (content == null)
            {
                tabItem.Content = vista;
            }
            else
            {
                tabItem.Content = content;
            }
            return tabItem;
        }

        //Zona de Clicks, aquí se pondrán todos



        private void BotonHardware_Click(object sender, RoutedEventArgs e)
        {
            DgTipoHardware dginci = new DgTipoHardware(mvTiposHardware);
            dginci.ShowDialog();
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

        private void cambiarContrasenya_Click(object sender, RoutedEventArgs e)
        {
            mvLogin = null;
            (new Login()).Show();

            this.Close();
        }
    }
}
