using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using Partec.Backend.Modelo;
using Partec.Backend.Servicios;
using Partec.MVVM.Base;

namespace Partec.MVVM
{
    public class MVLogin : MVBaseCRUD<Profesor>
    {
        private ProfesorServicio profesorServicio;
        private RolServicio rolServicio;
        private DepartamentoServicio departamentoServicio;
        private Profesor _usuario = new Profesor();
        private GestionincidenciasContext context;
        public Profesor usuario => _usuario;

        private Profesor _selectedProfesor = new Profesor();

        private ListCollectionView _ListaProfesores;
        public ListCollectionView ListaProfesores
        {
            get { return _ListaProfesores; }
            set
            {
                _ListaProfesores = value;
                OnPropertyChanged(nameof(ListaProfesores));
            }
        }
        private ListCollectionView _ListaRoles;
        public ListCollectionView ListaRoles
        {
            get { return _ListaRoles; }
            set
            {
                _ListaProfesores = value;
                OnPropertyChanged(nameof(ListaRoles));
            }
        }
        private ListCollectionView _ListaDepartamentos;
        public ListCollectionView ListaDepartamentos
        {
            get { return _ListaDepartamentos; }
            set
            {
                _ListaProfesores = value;
                OnPropertyChanged(nameof(ListaDepartamentos));
            }
        }

        public Profesor SelectedProfesor
        {
            get => _selectedProfesor;
            set
            {
                _selectedProfesor = value;
                OnPropertyChanged(nameof(SelectedProfesor));
            }
        }

        public async Task Inicializa()
        {
            //   Eventos.RolModificado += cargarPermisos;
            rolServicio = new RolServicio(context);
            departamentoServicio = new DepartamentoServicio(context);
            _ListaProfesores = new ListCollectionView(
                (await profesorServicio.GetAllAsync()).ToList()
            );
            _ListaDepartamentos = new ListCollectionView(
                (await departamentoServicio.GetAllAsync()).ToList()
            );
            _ListaRoles = new ListCollectionView((await rolServicio.GetAllAsync()).ToList());

            OnPropertyChanged(nameof(ListaProfesores));
            OnPropertyChanged(nameof(ListaDepartamentos));
            OnPropertyChanged(nameof(ListaRoles));
        }

        public Profesor Clonar
        {
            get { return (Profesor)_selectedProfesor.Clone(); }
        }
        public bool Guarda
        {
            get
            {
                try
                {
                    bool result;
                    if (_selectedProfesor.IdProfesor == 0)
                    {
                        result = Task.Run(() => Add(_selectedProfesor)).Result;
                        if (result)
                        {
                            Eventos.OnResponsableAgregado();
                            ListaProfesores.AddNewItem(_selectedProfesor);
                            ListaProfesores.CommitNew();
                            OnPropertyChanged(nameof(SelectedProfesor));
                            OnPropertyChanged(nameof(ListaProfesores));
                        }
                    }
                    else
                    {
                        result = Task.Run(() => Update(_selectedProfesor)).Result;
                        if (result)
                        {
                            Eventos.OnResponsableAgregado();
                            ListaProfesores.Refresh();
                            OnPropertyChanged(nameof(SelectedProfesor));
                            OnPropertyChanged(nameof(ListaProfesores));
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
        public bool Borrar
        {
            get
            {
                var result = Task.Run(() => Delete(_selectedProfesor.IdProfesor)).Result;
                if (result)
                {
                    Eventos.OnResponsableAgregado();
                    ListaProfesores.Remove(_selectedProfesor);
                    ListaProfesores.Refresh();

                    OnPropertyChanged(nameof(SelectedProfesor));
                }
                return result;
            }
        }

        public void CancelarProfesor() { }

        public String username
        {
            get { return _usuario.Email; }
            set
            {
                _usuario.Email = value;
                OnPropertyChanged(nameof(username));
            }
        }
        public String password
        {
            get { return _usuario.Password; }
            set
            {
                _usuario.Password = value;
                OnPropertyChanged(nameof(password));
            }
        }

        public MVLogin(GestionincidenciasContext contexto)
        {
            this.context = contexto;

            profesorServicio = new ProfesorServicio(context);

            servicio = profesorServicio;
        }

        public async void cargarPermisos()
        {
            _usuario = (
                await profesorServicio.FindAsync(u => u.IdProfesor == _usuario.IdProfesor)
            ).FirstOrDefault();
        }

        public async Task<Boolean> login()
        {
            IEnumerable<Profesor> usu = await profesorServicio.FindAsync(p =>
                p.Email.ToLower() == username.ToLower() && p.Password == password
            );
            if (usu.Count() > 0)
            {
                _usuario = usu.FirstOrDefault();
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
