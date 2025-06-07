using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;
using Microsoft.EntityFrameworkCore;
using Partec.Backend.Modelo;
using Partec.Backend.Servicios;
using Partec.MVVM.Base;

namespace Partec.MVVM
{
    public class MVRol : MVBaseCRUD<Rol>
    {
        private RolServicio rolServicio;
        private PermisoServicio permisoServicio;
        private Task inicializar;
        public ListCollectionView ListaRoles { get; set; }
        public ObservableCollection<Permiso> ListaPermisos { get; set; }
        private Rol _selectedRol = new Rol();
        public Rol SelectedRol
        {
            get => _selectedRol;
            set
            {
                _selectedRol = value;
                OnPropertyChanged(nameof(SelectedRol));
            }
        }

        public bool guarda
        {
            get
            {
                try
                {
                    GuardarCambiosRol();
                    bool result;
                    if (SelectedRol.IdRol == 0)
                    {
                        result = Task.Run(() => Add(SelectedRol)).Result;
                        if (result)
                        {
                            Eventos.OnRolModificado();
                            ListaRoles.AddNewItem(SelectedRol);
                            ListaRoles.CommitNew();
                            OnPropertyChanged(nameof(SelectedRol));
                            OnPropertyChanged(nameof(SelectedRol));
                        }
                    }
                    else
                    {
                        result = Task.Run(() => Update(SelectedRol)).Result;
                        if (result)
                        {
                            Eventos.OnRolModificado();
                            ListaRoles.Refresh();
                            OnPropertyChanged(nameof(SelectedRol));
                            OnPropertyChanged(nameof(ListaRoles));
                        }
                    }

                    return result;
                }
                catch
                {
                    return false;
                }
            }
        }

        public Boolean borra
        {
            get
            {
                var result = Task.Run(() => Delete(SelectedRol.IdRol)).Result;
                if (result)
                {
                    Eventos.OnEstadoAgregado();
                    ListaRoles.Remove(SelectedRol);
                    ListaRoles.Refresh();
                    OnPropertyChanged(nameof(SelectedRol));
                }
                return result;
            }
        }

        private void GuardarCambiosRol()
        {
            // Limpiar permisos actuales
            SelectedRol.IdPermisos.Clear();

            // Agregar los permisos seleccionados
            foreach (var permiso in ListaPermisos.Where(p => p.Asignado))
            {
                SelectedRol.IdPermisos.Add(permiso);
            }
        }

        public MVRol(GestionincidenciasContext context, Task inicializa = null)
        {
            try
            {
                context.Database.OpenConnection();
            }
            catch { }
            this.inicializar = inicializa;
            rolServicio = new RolServicio(context);
            permisoServicio = new PermisoServicio(context);
            servicio = rolServicio;
        }

        public async Task inicializa()
        {
            ListaRoles = new ListCollectionView((await rolServicio.GetAllAsync()).ToList());
            ListaPermisos = new ObservableCollection<Permiso>(
                (await permisoServicio.GetAllAsync()).ToList()
            );
            OnPropertyChanged(nameof(ListaRoles));
            OnPropertyChanged(nameof(ListaPermisos));
        }

        public async Task CargarPermisosParaRol()
        {
            // Cargar todos los permisos
            var todos = (await permisoServicio.GetAllAsync()).ToList();

            // Marcar los asignados
            foreach (var permiso in todos)
            {
                permiso.Asignado = SelectedRol.IdPermisos.Any(p =>
                    p.IdPermiso == permiso.IdPermiso
                );
            }

            ListaPermisos = new ObservableCollection<Permiso>(todos);
            OnPropertyChanged(nameof(ListaPermisos));
        }

        public void CancelarRol() { }
    }
}
