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
using Microsoft.EntityFrameworkCore;
using Partec.Backend.Modelo;
using Partec.MVVM;

namespace Partec
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : MetroWindow
    {
        GestionincidenciasContext context;
        MVLogin login;
        public Login()
        {
            if (ConectarBD())
            {
                InitializeComponent();
                tbUsername.Focus();
                this.login = new MVLogin(context);
                DataContext = this.login;
            }
        }

        private async void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            if (await login.login())
            {

                MainWindow ventanaPrincipal = new MainWindow(context, login.usuario);

                ventanaPrincipal.Show();
                this.Close();
            }
            else
            {
                _ = this.ShowMessageAsync("INICIO DE SESIÓN", "El usuario y/o contraseña son incorrectos " + login.username + " " + login.password + " a");
            }
        }

        private bool ConectarBD()
        {
            bool correcto = true;
            context = new GestionincidenciasContext();
            try
            {
                context.Database.OpenConnection();
            }
            catch (Exception ex)
            {
                correcto = false;
                this.ShowMessageAsync("CONEXIÓN DE LA BASE DE DATOS",
                    "Upps!!! No podemos conectar con la BD. Contacte con el administrador");
            }
            return correcto;
        }
    }
}
