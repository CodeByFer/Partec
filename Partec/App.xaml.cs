using System.Configuration;
using System.Data;
using System.Windows;
using MahApps.Metro.Controls;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Partec.Backend.Modelo;
using Partec.Backend.Utiles;

namespace Partec
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        // App.xaml.cs
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Para todas las ventanas normales (Window)
            EventManager.RegisterClassHandler(
                typeof(Window),
                Window.LoadedEvent,
                new RoutedEventHandler(
                    (sender, args) =>
                    {
                        if (sender is Window ventana)
                        {
                            ventana.WindowState = WindowState.Maximized;
                        }
                    }
                )
            );

            // Para todas las ventanas MetroWindow
            EventManager.RegisterClassHandler(
                typeof(MetroWindow),
                Window.LoadedEvent,
                new RoutedEventHandler(
                    (sender, args) =>
                    {
                        if (sender is MetroWindow metroVentana)
                        {
                            metroVentana.WindowState = WindowState.Maximized;
                        }
                    }
                )
            );
        }
    }
}
