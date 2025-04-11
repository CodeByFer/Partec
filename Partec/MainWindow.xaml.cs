using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MahApps.Metro.Controls;
using Partec.Backend.Modelo;


namespace Partec
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        private GestionincidenciasContext context;
        private Profesor usuario;
        private Dictionary<DateTime, List<string>> incidencias = new Dictionary<DateTime, List<string>>();

        public MainWindow(GestionincidenciasContext context, Profesor usuario)
        {
            InitializeComponent();
            this.context = context;
            this.usuario = usuario;
            GenerarIncidencias(500);
            MostrarCalendario();
        }
        public MainWindow()
        {
            InitializeComponent();
            GenerarIncidencias(500);
            MostrarCalendario();
        }
        private void AgregarIncidencia(DateTime fecha, string descripcion)
        {
            if (!incidencias.ContainsKey(fecha))
                incidencias[fecha] = new List<string>();

            incidencias[fecha].Add(descripcion);
        }
        private void GenerarIncidencias(int cantidad)
        {
            Random rand = new Random();
            DateTime inicio = DateTime.Today;

            for (int i = 0; i < cantidad; i++)
            {
                // Espaciado cada 1–3 días aleatoriamente
                int diasExtra = rand.Next(-100, 100);
                inicio = inicio.AddDays(diasExtra);

                string descripcion = $"Incidencia #{i + 1}: " + GenerarDescripcionAleatoria(rand);
                AgregarIncidencia(inicio, descripcion);
            }
        }

        // Generador de textos aleatorios para las incidencias
        private string GenerarDescripcionAleatoria(Random rand)
        {
            string[] acciones = { "Fallo en", "Revisión de", "Actualización de", "Problema con", "Error en" };
            string[] sistemas = { "servidor", "base de datos", "correo", "firewall", "aplicación", "infraestructura", "usuario", "licencias" };
            string[] criticos = { "crítico", "leve", "pendiente", "urgente", "solucionado" };

            return $"{acciones[rand.Next(acciones.Length)]} {sistemas[rand.Next(sistemas.Length)]} ({criticos[rand.Next(criticos.Length)]})";
        }


        // Función genérica para encontrar elementos hijos en el árbol visual

        private void MostrarCalendario()
        {
            CalendarioGrid.Children.Clear();

            foreach (var item in incidencias)
            {

                var fecha = item.Key;
                var listaIncidencias = item.Value;

                // Contenedor del día
                Border dia = new Border
                {
                    Background = new SolidColorBrush(Color.FromRgb(39, 84, 138)),
                    BorderBrush = new SolidColorBrush(Color.FromRgb(29, 59, 83)),
                    BorderThickness = new Thickness(1),
                    Margin = new Thickness(5),
                    CornerRadius = new CornerRadius(8),
                    Padding = new Thickness(10)
                };

                StackPanel contenido = new StackPanel();

                // Fecha grande
                TextBlock textoFecha = new TextBlock
                {
                    Text = fecha.ToString("dd MMM"),
                    FontWeight = FontWeights.Bold,
                    FontSize = 18,
                    Foreground = new SolidColorBrush(Color.FromRgb(245, 238, 220)),
                    Margin = new Thickness(0, 0, 0, 5)
                };
                contenido.Children.Add(textoFecha);

                // Lista de incidencias
                foreach (var incidencia in listaIncidencias)
                {
                    if (incidencia.Contains("solucionado"))
                    {
                        continue;
                    }
                    TextBlock textoIncidencia = new TextBlock
                    {
                        Text = "• " + incidencia,
                        FontSize = 14,
                        Foreground = new SolidColorBrush(Color.FromRgb(245, 238, 220)),
                        TextWrapping = TextWrapping.Wrap,
                        Margin = new Thickness(0, 2, 0, 2)
                    };
                    contenido.Children.Add(textoIncidencia);
                }

                if (contenido.GetChildObjects().Count() > 1)
                {
                    dia.Child = contenido;

                    CalendarioGrid.Children.Add(dia);
                }

            }
        }

    }
}