﻿using System;
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
    /// Interaction logic for DgRoles.xaml
    /// </summary>
    public partial class DgRoles : MetroWindow
    {
        private MVRol mv;
        private Task inicializa;

        public DgRoles(MVRol mv, Task inicializa)
        {
            InitializeComponent();
            this.inicializa = inicializa;
            this.mv = mv;
            DataContext = this.mv;
            _ = mv.CargarPermisosParaRol();
        }

        private async void BtnGuardar_Click(object sender, RoutedEventArgs e)
        {
            inicializa?.Wait();
            if (mv.IsValid(this))
            {
                if (mv.guarda)
                {
                    await this.ShowMessageAsync("Gestión de Roles", "Rol guardado correctamente");
                    DialogResult = true;
                }
                else
                {
                    await this.ShowMessageAsync(
                        "Gestión de Roles",
                        "Error interno, contacte con el administrador"
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

        private void BtnCancelar_Click(object sender, RoutedEventArgs e)
        {
            mv.CancelarRol();
            this.Close();
        }
    }
}
