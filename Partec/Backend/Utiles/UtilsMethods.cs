namespace Partec.Backend.Utiles
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using MaterialDesignThemes.Wpf;

    public static class UtilsMethods
    {
        public static TabItem CrearTabItem(
            string nombreInterno = "tbiIncidencia",
            string texto = "Incidencia",
            PackIconKind icono = PackIconKind.Inbox,
            string colorFondo = "#FFDDA853",
            string colorTexto = "#183B4E"
        )
        {
            // Crear el TabItem
            var tabItem = new TabItem
            {
                Name = nombreInterno,
                Margin = new Thickness(10),
                Background = new SolidColorBrush(
                    (Color)ColorConverter.ConvertFromString(colorFondo)
                ),
                Foreground = new SolidColorBrush(
                    (Color)ColorConverter.ConvertFromString(colorTexto)
                ),
            };

            // Aplicar estilo si existe en recursos
            tabItem.SetResourceReference(Control.StyleProperty, "MaterialDesignTabItem");

            // Icono
            var icon = new PackIcon
            {
                Kind = icono,
                Height = 20,
                Width = 20,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(0),
            };

            // Texto
            var textBlock = new TextBlock
            {
                Text = texto,
                FontSize = 20,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(5, 0, 0, 0),
            };

            // WrapPanel
            var wrapPanel = new WrapPanel
            {
                Margin = new Thickness(0),
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
            };

            wrapPanel.Children.Add(icon);
            wrapPanel.Children.Add(textBlock);

            tabItem.Header = wrapPanel;

            return tabItem;
        }

        public static Button CrearBoton(
            string texto,
            PackIconKind icono,
            RoutedEventHandler onClick
        )
        {
            Button button = new Button
            {
                Width = 200,
                Height = 200,
                Margin = new Thickness(40),
                Background = new SolidColorBrush(
                    (Color)ColorConverter.ConvertFromString("#FFDDA853")
                ),
            };
            button.Click += onClick;

            StackPanel stackPanel = new StackPanel
            {
                Orientation = Orientation.Vertical,
                Margin = new Thickness(0),
            };

            stackPanel.Children.Add(
                new PackIcon
                {
                    Kind = icono,
                    Width = 40,
                    Height = 40,
                    HorizontalAlignment = HorizontalAlignment.Center,
                }
            );

            stackPanel.Children.Add(
                new TextBlock
                {
                    Text = texto,
                    FontSize = 25,
                    TextWrapping = TextWrapping.Wrap,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                }
            );

            button.Content = stackPanel;

            return button;
        }
    }
}
