namespace Partec.MVVM
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.IO;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Data;
    using Microsoft.EntityFrameworkCore;
    using Partec.Backend.Modelo;
    using Partec.Backend.Servicios;
    using Partec.Backend.Utiles;
    using Partec.MVVM.Base;

    public class MVIncidencia : MVBaseCRUD<Incidencia>
    {
        private ProfesorServicio profesorServicio;

        private ArchivoServicio archivoServicio;

        public Profesor? profesorAntiguo = null;

        private TipoHardwareServicio tipoHardwareServicio;

        private IncidenciaServicio incidenciaServicio;

        private DepartamentoServicio departamentoServicio;

        private Task actualizacion;

        private EstadoServicio estadoServicio;

        private Profesor usuario;

        private Incidencia _selectedIncidencia = new Incidencia();

        private ListCollectionView _ListaIncidencias = new ListCollectionView(
            new List<Incidencia>()
        );

        public ListCollectionView ListaIncidencias => _ListaIncidencias;

        public ObservableCollection<Estado> ListaEstados { get; set; }

        public IEnumerable<Profesor> ListaResponsables { get; set; }

        private GestionincidenciasContext context;

        private List<Predicate<Incidencia>> criterios;

        private Predicate<Incidencia> criterioDepartamento;

        private Predicate<Incidencia> criterioResponsable;

        private Predicate<Incidencia> criterioEstado;

        private Predicate<Incidencia> criterioTipoHardware;

        private Predicate<Incidencia> criterioFechaInicio;

        private Predicate<Incidencia> criterioFechaFin;

        private Predicate<Incidencia> criterioTipo;

        private Predicate<object> predicadoFiltro;

        private int? _filtroDepartamento;

        public int? FiltroDepartamento
        {
            get => _filtroDepartamento;
            set
            {
                _filtroDepartamento = value;
                OnPropertyChanged(nameof(FiltroDepartamento));
                Filtrar();
            }
        }

        private bool _filtrarSinResponsable;

        public bool FiltrarSinResponsable
        {
            get => _filtrarSinResponsable;
            set
            {
                if (_filtrarSinResponsable != value)
                {
                    _filtrarSinResponsable = value;
                    OnPropertyChanged(nameof(FiltrarSinResponsable));
                    Filtrar();
                }
            }
        }

        private int? _filtroEstado;

        public int? FiltroEstado
        {
            get => _filtroEstado;
            set
            {
                _filtroEstado = value;
                OnPropertyChanged(nameof(FiltroEstado));
                Filtrar();
            }
        }

        private int? _filtroTipoHardware;

        public int? FiltroTipoHardware
        {
            get => _filtroTipoHardware;
            set
            {
                _filtroTipoHardware = value;
                OnPropertyChanged(nameof(FiltroTipoHardware));
                Filtrar();
            }
        }

        private DateTime? _filtroFechaInicio;

        public DateTime? FiltroFechaInicio
        {
            get => _filtroFechaInicio;
            set
            {
                _filtroFechaInicio = value;
                OnPropertyChanged(nameof(FiltroFechaInicio));
                Filtrar();
            }
        }

        private DateTime? _filtroFechaFin;

        public DateTime? FiltroFechaFin
        {
            get => _filtroFechaFin;
            set
            {
                _filtroFechaFin = value;
                OnPropertyChanged(nameof(FiltroFechaFin));
                Filtrar();
            }
        }

        private string _filtroTipo;

        public string FiltroTipo
        {
            get => _filtroTipo;
            set
            {
                _filtroTipo = value;
                OnPropertyChanged(nameof(FiltroTipo));
                Filtrar();
            }
        }

        private ICollectionView _ListaIncidenciasFiltrada;

        public ICollectionView ListaIncidenciasFiltrada
        {
            get { return _ListaIncidenciasFiltrada; }
            set
            {
                ListaIncidenciasFiltrada = CollectionViewSource.GetDefaultView(value);
                OnPropertyChanged(nameof(ListaIncidenciasFiltrada));
            }
        }

        public IEnumerable<String> TiposIncidencia { get; set; } = ["HW", "SW"];

        public ObservableCollection<Departamento> ListaDepartamentos { get; set; }

        public IEnumerable<TipoHardware> ListaTiposHardware { get; set; }

        private Visibility _hardwareFieldsVisibility = Visibility.Collapsed;

        public Incidencia Clonar
        {
            get { return (Incidencia)_selectedIncidencia.Clone(); }
        }

        public Incidencia SelectedIncidencia
        {
            get => _selectedIncidencia;
            set
            {
                _selectedIncidencia = value;
                if (value != null)
                {
                    TipoSeleccionado = value.TipoIncidencia;
                }

                OnPropertyChanged(nameof(SelectedIncidencia));
            }
        }

        private string _tipoSeleccionado;

        private Boolean admin = false;

        private Boolean responsable = false;

        public string TipoSeleccionado
        {
            get => _tipoSeleccionado;
            set
            {
                if (_tipoSeleccionado != value)
                {
                    _tipoSeleccionado = value;
                    SelectedIncidencia.TipoIncidencia = value;
                    ActualizarVisibilidadCamposHardware();
                    OnPropertyChanged(nameof(TipoSeleccionado));
                }
            }
        }

        public MVIncidencia(
            GestionincidenciasContext contexto,
            Profesor usuario,
            Incidencia incidenciaEdit = null,
            Boolean admin = false,
            Boolean responsable = false
        )
        {
            context = contexto;
            try
            {
                context.Database.OpenConnection();
            }
            catch (Exception ex) { }

            this.admin = admin;
            this.responsable = responsable;

            ListaResponsables = new ObservableCollection<Profesor>();
            ListaEstados = new ObservableCollection<Estado>();
            incidenciaServicio = new IncidenciaServicio(context);
            profesorServicio = new ProfesorServicio(context);
            departamentoServicio = new DepartamentoServicio(context);
            estadoServicio = new EstadoServicio(context);
            tipoHardwareServicio = new TipoHardwareServicio(context);
            archivoServicio = new ArchivoServicio(context);
            this.usuario = usuario;
            if (incidenciaEdit != null)
            {
                _selectedIncidencia = incidenciaEdit;
                TipoSeleccionado = incidenciaEdit.TipoIncidencia;
            }
            servicio = incidenciaServicio;
        }

        public Visibility HardwareFieldsVisibility
        {
            get { return _hardwareFieldsVisibility; }
            set
            {
                _hardwareFieldsVisibility = value;
                OnPropertyChanged(nameof(HardwareFieldsVisibility));
            }
        }

        public async Task inicializa()
        {
            //Todos
            servicio = incidenciaServicio;
            Eventos.TipoHardwareAgregado += CargarTiposHardware;
            Eventos.DepartamentoAgregado += CargarDepartamentos;
            ListaTiposHardware = await tipoHardwareServicio.GetAllAsync();
            OnPropertyChanged(nameof(ListaTiposHardware));
            ListaDepartamentos = new ObservableCollection<Departamento>(
                (await departamentoServicio.GetAllAsync())
            );
            ListaResponsables = await profesorServicio.GetAllAsync();
            OnPropertyChanged(nameof(ListaResponsables));
            OnPropertyChanged(nameof(ListaDepartamentos));
            Eventos.ResponsableAgregado += CargarResponsables;
            Eventos.EstadoAgregado += CargarEstados;
            ListaEstados = new ObservableCollection<Estado>(await estadoServicio.GetAllAsync());
            OnPropertyChanged(nameof(ListaEstados));
            if (admin)
            {
                _ListaIncidencias = new ListCollectionView(
                    (await incidenciaServicio.GetAllAsync()).ToList()
                );
                if (_ListaIncidencias == null || _ListaIncidencias.Count == 0)
                {
                    _ListaIncidencias = new ListCollectionView(new List<Incidencia>());
                }
            }
            else if (responsable && !admin)
            {
                _ListaIncidencias = new ListCollectionView(
                    (
                        await incidenciaServicio.FindAsync(i =>
                            (i.IdResponsable == usuario.IdProfesor)
                            || (i.IdProfesor == usuario.IdProfesor)
                        )
                    ).ToList()
                );
                if (_ListaIncidencias == null || _ListaIncidencias.Count == 0)
                {
                    _ListaIncidencias = new ListCollectionView(new List<Incidencia>());
                }
            }
            else
            {
                if (_ListaIncidencias == null || _ListaIncidencias.Count == 0)
                {
                    _ListaIncidencias = new ListCollectionView(
                        (
                            await incidenciaServicio.FindAsync(i =>
                                i.IdProfesor == usuario.IdProfesor
                            )
                        ).ToList()
                    );
                    if (_ListaIncidencias == null || _ListaIncidencias.Count == 0)
                    {
                        _ListaIncidencias = new ListCollectionView(new List<Incidencia>());
                    }
                }
            }

            _ListaIncidenciasFiltrada = _ListaIncidencias;
            OnPropertyChanged(nameof(ListaIncidencias));
            OnPropertyChanged(nameof(ListaIncidenciasFiltrada));

            ActualizarVisibilidadCamposHardware();

            ListaIncidenciasFiltrada.Filter = FiltroIncidencias;
            criterios = new List<Predicate<Incidencia>>();
            predicadoFiltro = new Predicate<object>(FiltroCriterios);
            InicializaCriterios();
            ListaIncidencias.Filter = predicadoFiltro;
        }

        private async void CargarTiposHardware()
        {
            if (actualizacion != null && !actualizacion.IsCompleted)
                await actualizacion;

            actualizacion = Task.Run(async () =>
            {
                ListaTiposHardware = await tipoHardwareServicio.GetAllAsync();
                OnPropertyChanged(nameof(ListaTiposHardware));
            });

            await actualizacion;
        }

        private async void CargarEstados()
        {
            if (actualizacion != null && !actualizacion.IsCompleted)
                await actualizacion;

            actualizacion = Task.Run(async () =>
            {
                ListaEstados = new ObservableCollection<Estado>(await estadoServicio.GetAllAsync());
                OnPropertyChanged(nameof(ListaEstados));
            });

            await actualizacion;
        }

        private async void CargarDepartamentos()
        {
            if (actualizacion != null && !actualizacion.IsCompleted)
                await actualizacion;

            actualizacion = Task.Run(async () =>
            {
                ListaDepartamentos = new ObservableCollection<Departamento>(
                    await departamentoServicio.GetAllAsync()
                );
                OnPropertyChanged(nameof(ListaDepartamentos));
            });

            await actualizacion;
        }

        private async void CargarResponsables()
        {
            if (actualizacion != null && !actualizacion.IsCompleted)
                await actualizacion;

            actualizacion = Task.Run(async () =>
            {
                ListaResponsables = await profesorServicio.GetAllAsync();
                OnPropertyChanged(nameof(ListaResponsables));
            });

            await actualizacion;
        }

        private async void CargarIncidencias()
        {
            if (actualizacion != null && !actualizacion.IsCompleted)
                await actualizacion;

            actualizacion = Task.Run(async () =>
            {
                _ListaIncidencias = new ListCollectionView(
                    (await incidenciaServicio.GetAllAsync()).ToList()
                );
                _ListaIncidenciasFiltrada = _ListaIncidencias;
                OnPropertyChanged(nameof(ListaIncidencias));
                OnPropertyChanged(nameof(ListaIncidenciasFiltrada));
            });

            await actualizacion;
        }

        private bool FiltroCriterios(object item)
        {
            if (item is not Incidencia incidencia)
                return false;

            return criterios.TrueForAll(c => c(incidencia));
        }

        private void InicializaCriterios()
        {
            criterioDepartamento = i => i.IdDepartamento == FiltroDepartamento;
            criterioEstado = i => i.IdEstado == FiltroEstado;
            criterioTipoHardware = i => i.IdTipoHw == FiltroTipoHardware;
            criterioFechaInicio = i => i.FechaIntroduccion >= FiltroFechaInicio;
            criterioFechaFin = i => i.FechaIntroduccion <= FiltroFechaFin;
            criterioTipo = i => i.TipoIncidencia != null && i.TipoIncidencia.Equals(FiltroTipo);
            criterioResponsable = i => !FiltrarSinResponsable || !i.IdResponsable.HasValue;
        }

        public void Filtrar()
        {
            criterios.Clear();

            if (FiltroDepartamento.HasValue)
                criterios.Add(criterioDepartamento);
            if (FiltroEstado.HasValue)
                criterios.Add(criterioEstado);
            if (FiltroTipoHardware.HasValue)
                criterios.Add(criterioTipoHardware);
            if (FiltroFechaInicio.HasValue)
                criterios.Add(criterioFechaInicio);
            if (FiltroFechaFin.HasValue)
                criterios.Add(criterioFechaFin);
            if (!string.IsNullOrWhiteSpace(FiltroTipo))
                criterios.Add(criterioTipo);
            if (FiltrarSinResponsable)
                criterios.Add(criterioResponsable);

            ListaIncidencias.Refresh();
        }

        private bool FiltroIncidencias(object obj)
        {
            if (obj is not Incidencia inc)
                return false;

            if (FiltroDepartamento.HasValue && inc.IdDepartamento != FiltroDepartamento.Value)
                return false;
            if (FiltroEstado.HasValue && inc.IdEstado != FiltroEstado.Value)
                return false;
            if (FiltroTipoHardware.HasValue && inc.IdTipoHw != FiltroTipoHardware.Value)
                return false;
            if (FiltroFechaInicio.HasValue && inc.FechaIntroduccion < FiltroFechaInicio.Value)
                return false;
            if (FiltroFechaFin.HasValue && inc.FechaIntroduccion > FiltroFechaFin.Value)
                return false;
            if (String.IsNullOrEmpty(FiltroTipo) && inc.TipoIncidencia.Equals(FiltroTipo))
                return false;
            if (FiltrarSinResponsable && inc.IdResponsable.HasValue)
                return false;

            return true;
        }

        public List<Archivo> ListaArchivosAuxiliar { get; set; } = new List<Archivo>();

        public void LimpiarFiltros()
        {
            FiltroDepartamento = null;
            FiltroEstado = null;
            FiltroTipoHardware = null;
            FiltroFechaInicio = null;
            FiltroFechaFin = null;
            FiltroTipo = null;
            FiltrarSinResponsable = false;
            OnFiltroChanged();
        }

        private void OnFiltroChanged()
        {
            ListaIncidenciasFiltrada.Refresh();
            OnPropertyChanged(nameof(ListaIncidenciasFiltrada));
        }

        public void ActualizarVisibilidadCamposHardware()
        {
            if (
                !string.IsNullOrEmpty(SelectedIncidencia.TipoIncidencia)
                && SelectedIncidencia.TipoIncidencia.Equals("HW")
            )
            {
                HardwareFieldsVisibility = Visibility.Visible;
            }
            else
            {
                HardwareFieldsVisibility = Visibility.Collapsed;
            }
        }

        private String _ArchivoSeleccionado { get; set; }

        public String ArchivoSeleccionado
        {
            get => _ArchivoSeleccionado;
            set
            {
                _ArchivoSeleccionado = value;
                OnPropertyChanged(nameof(ArchivoSeleccionado));
            }
        }

        public bool guarda
        {
            get
            {
                try
                {
                    var result = Task.Run(() => Update(_selectedIncidencia)).Result;
                    if (result)
                    {
                        Eventos.OnResponsableIncidenciaAgregado(_selectedIncidencia);
                        if (
                            profesorAntiguo != null
                            && profesorAntiguo.IdProfesor != _selectedIncidencia.IdResponsable
                        )
                        {
                            Eventos.OnResponsableIncidenciaCambiado(
                                profesorAntiguo,
                                _selectedIncidencia
                            );
                        }
                        OnPropertyChanged(nameof(SelectedIncidencia));
                    }

                    return result;
                }
                catch (Exception)
                {
                    try
                    {
                        _selectedIncidencia.IdProfesor = usuario.IdProfesor;
                        _selectedIncidencia.IdProfesorNavigation = usuario;
                        var result = Task.Run(() => Add(_selectedIncidencia)).Result;

                        if (result)
                        {
                            ListaIncidencias.AddNewItem(_selectedIncidencia);
                            ListaIncidencias.CommitNew();
                            SelectedIncidencia.Archivos = ListaArchivosAuxiliar;
                            GuardarArchivosPostIncidencia();

                            // Asignar los archivos a la incidencia y subirlos
                            SubirArchivosSiCorresponden();
                            Eventos.OnIncidenciaAgregada(_selectedIncidencia);
                            if (_selectedIncidencia.IdResponsable != null)
                            {
                                Eventos.OnResponsableIncidenciaAgregado(_selectedIncidencia);
                            }
                        }

                        return result;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(
                            "Error al guardar la incidencia. Por favor, revise los datos introducidos."
                                + ex.Message,
                            "Error",
                            MessageBoxButton.OK,
                            MessageBoxImage.Error
                        );
                        return false;
                    }
                }
            }
        }

        public Visibility vsFechaResolucion
        {
            get
            {
                if (admin || responsable)
                {
                    return Visibility.Visible;
                }
                return Visibility.Hidden;
            }
        }

        public Visibility vsAdmin
        {
            get
            {
                if (admin)
                {
                    return Visibility.Visible;
                }
                return Visibility.Hidden;
            }
        }

        private void GuardarArchivosPostIncidencia()
        {
            if (_selectedIncidencia == null || ListaArchivosAuxiliar == null)
                return;

            var carpetaDestino = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                "Partec",
                "Incidencias",
                $"Incidencia{_selectedIncidencia.IdIncidencia}"
            );

            Directory.CreateDirectory(carpetaDestino);

            foreach (var archivo in ListaArchivosAuxiliar)
            {
                var nombreArchivo = Path.GetFileName(archivo.RutaRelativa);
                var destino = Path.Combine(carpetaDestino, nombreArchivo);

                // Solo copiar si aún no está
                if (!File.Exists(destino))
                {
                    File.Copy(archivo.RutaRelativa, destino, overwrite: true);
                }

                // Actualizar ruta relativa
                archivo.RutaRelativa = Path.Combine(
                    "Partec",
                    "Incidencias",
                    $"Incidencia{_selectedIncidencia.IdIncidencia}",
                    nombreArchivo
                );
            }
        }

        private List<String> _lstArchivos;

        public List<String> lstArchivos
        {
            get => _lstArchivos;
            set
            {
                _lstArchivos = value;
                OnPropertyChanged(nameof(lstArchivos));
            }
        }

        private async void SubirArchivosSiCorresponden()
        {
            if (_selectedIncidencia?.Archivos != null)
            {
                foreach (var archivo in _selectedIncidencia.Archivos)
                {
                    archivo.IdIncidencia = _selectedIncidencia.IdIncidencia;
                    await archivoServicio.AddAsync(archivo);
                }
            }
            return;
        }

        public bool borrar
        {
            get
            {
                var resultArchivos = false;
                if (_selectedIncidencia.Archivos.Count != 0)
                {
                    resultArchivos = Task.Run(
                        () => archivoServicio.DeleteAllAsync(_selectedIncidencia.Archivos.ToList())
                    ).Result;
                }

                var result = Task.Run(() => Delete(_selectedIncidencia.IdIncidencia)).Result;

                if (result)
                {
                    ListaIncidencias.Remove(_selectedIncidencia);
                    ListaIncidencias.Refresh();

                    OnPropertyChanged(nameof(SelectedIncidencia));
                }

                return result;
            }
        }

        public void CancelarIncidencia()
        {
            _selectedIncidencia = new Incidencia();
        }
    }
}
