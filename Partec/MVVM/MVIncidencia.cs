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

    /// <summary>
    /// Defines the <see cref="MVIncidencia" />
    /// </summary>
    public class MVIncidencia : MVBaseCRUD<Incidencia>
    {
        /// <summary>
        /// Defines the profesorServicio
        /// </summary>
        private ProfesorServicio profesorServicio;
        private ArchivoServicio archivoServicio;

        public Profesor? profesorAntiguo = null;

        /// <summary>
        /// Defines the tipoHardwareServicio
        /// </summary>
        private TipoHardwareServicio tipoHardwareServicio;

        /// <summary>
        /// Defines the incidenciaServicio
        /// </summary>
        private IncidenciaServicio incidenciaServicio;

        /// <summary>
        /// Defines the departamentoServicio
        /// </summary>
        private DepartamentoServicio departamentoServicio;

        private Task actualizacion;

        /// <summary>
        /// Defines the estadoServicio
        /// </summary>
        private EstadoServicio estadoServicio;

        /// <summary>
        /// Defines the usuario
        /// </summary>
        private Profesor usuario;

        /// <summary>
        /// Defines the _selectedIncidencia
        /// </summary>
        private Incidencia _selectedIncidencia = new Incidencia();

        /// <summary>
        /// Defines the _ListaIncidencias
        /// </summary>
        private ListCollectionView _ListaIncidencias;

        /// <summary>
        /// Gets the ListaIncidencias
        /// </summary>
        public ListCollectionView ListaIncidencias => _ListaIncidencias;

        /// <summary>
        /// Gets or sets the ListaEstados
        /// </summary>
        public ObservableCollection<Estado> ListaEstados { get; set; }

        /// <summary>
        /// Gets or sets the ListaResponsables
        /// </summary>
        public IEnumerable<Profesor> ListaResponsables { get; set; }

        /// <summary>
        /// Defines the context
        /// </summary>
        private GestionincidenciasContext context;

        /// <summary>
        /// Defines the criterios
        /// </summary>
        private List<Predicate<Incidencia>> criterios;

        /// <summary>
        /// Defines the criterioDepartamento
        /// </summary>
        private Predicate<Incidencia> criterioDepartamento;
        private Predicate<Incidencia> criterioResponsable;

        /// <summary>
        /// Defines the criterioEstado
        /// </summary>
        private Predicate<Incidencia> criterioEstado;

        /// <summary>
        /// Defines the criterioTipoHardware
        /// </summary>
        private Predicate<Incidencia> criterioTipoHardware;

        /// <summary>
        /// Defines the criterioFechaInicio
        /// </summary>
        private Predicate<Incidencia> criterioFechaInicio;

        /// <summary>
        /// Defines the criterioFechaFin
        /// </summary>
        private Predicate<Incidencia> criterioFechaFin;

        /// <summary>
        /// Defines the criterioTipo
        /// </summary>
        private Predicate<Incidencia> criterioTipo;

        /// <summary>
        /// Defines the predicadoFiltro
        /// </summary>
        private Predicate<object> predicadoFiltro;

        /// <summary>
        /// Defines the _filtroDepartamento
        /// </summary>
        private int? _filtroDepartamento;

        /// <summary>
        /// Gets or sets the FiltroDepartamento
        /// </summary>
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

        /// <summary>
        /// Defines the _filtrarSinResponsable
        /// </summary>
        private bool _filtrarSinResponsable;

        /// <summary>
        /// Gets or sets a value indicating whether FiltrarSinResponsable
        /// </summary>
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

        /// <summary>
        /// Defines the _filtroEstado
        /// </summary>
        private int? _filtroEstado;

        /// <summary>
        /// Gets or sets the FiltroEstado
        /// </summary>
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

        /// <summary>
        /// Defines the _filtroTipoHardware
        /// </summary>
        private int? _filtroTipoHardware;

        /// <summary>
        /// Gets or sets the FiltroTipoHardware
        /// </summary>
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

        /// <summary>
        /// Defines the _filtroFechaInicio
        /// </summary>
        private DateTime? _filtroFechaInicio;

        /// <summary>
        /// Gets or sets the FiltroFechaInicio
        /// </summary>
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

        /// <summary>
        /// Defines the _filtroFechaFin
        /// </summary>
        private DateTime? _filtroFechaFin;

        /// <summary>
        /// Gets or sets the FiltroFechaFin
        /// </summary>
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

        /// <summary>
        /// Defines the _filtroTipo
        /// </summary>
        private string _filtroTipo;

        /// <summary>
        /// Gets or sets the FiltroTipo
        /// </summary>
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

        /// <summary>
        /// Defines the _ListaIncidenciasFiltrada
        /// </summary>
        private ICollectionView _ListaIncidenciasFiltrada;

        /// <summary>
        /// Gets or sets the ListaIncidenciasFiltrada
        /// </summary>
        public ICollectionView ListaIncidenciasFiltrada
        {
            get { return _ListaIncidenciasFiltrada; }
            set
            {
                ListaIncidenciasFiltrada = CollectionViewSource.GetDefaultView(value);
                OnPropertyChanged(nameof(ListaIncidenciasFiltrada));
            }
        }

        /// <summary>
        /// Gets or sets the TiposIncidencia
        /// </summary>
        public IEnumerable<String> TiposIncidencia { get; set; } = ["HW", "SW"];

        /// <summary>
        /// Gets or sets the ListaDepartamentos
        /// </summary>
        public ObservableCollection<Departamento> ListaDepartamentos { get; set; }

        /// <summary>
        /// Gets or sets the ListaTiposHardware
        /// </summary>
        public IEnumerable<TipoHardware> ListaTiposHardware { get; set; }

        /// <summary>
        /// Defines the _hardwareFieldsVisibility
        /// </summary>
        private Visibility _hardwareFieldsVisibility = Visibility.Collapsed;

        /// <summary>
        /// Gets the Clonar
        /// </summary>
        public Incidencia Clonar
        {
            get { return (Incidencia)_selectedIncidencia.Clone(); }
        }

        /// <summary>
        /// Gets or sets the SelectedIncidencia
        /// </summary>
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

        /// <summary>
        /// Defines the _tipoSeleccionado
        /// </summary>
        private string _tipoSeleccionado;

        /// <summary>
        /// Gets or sets the TipoSeleccionado
        /// </summary>
        ///

        private Boolean admin;
        private Boolean responsable;
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

        /// <summary>
        /// Initializes a new instance of the <see cref="MVIncidencia"/> class.
        /// </summary>
        /// <param name="contexto">The contexto<see cref="GestionincidenciasContext"/></param>
        /// <param name="usuario">The usuario<see cref="Profesor"/></param>
        /// <param name="incidenciaEdit">The incidenciaEdit<see cref="Incidencia"/></param>
        /// <param name="editar">The editar<see cref="Boolean"/></param>
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
        }

        /// <summary>
        /// Gets or sets the HardwareFieldsVisibility
        /// </summary>
        public Visibility HardwareFieldsVisibility
        {
            get { return _hardwareFieldsVisibility; }
            set
            {
                _hardwareFieldsVisibility = value;
                OnPropertyChanged(nameof(HardwareFieldsVisibility));
            }
        }

        /// <summary>
        /// The inicializa
        /// </summary>
        /// <returns>The <see cref="Task"/></returns>
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
            OnPropertyChanged(nameof(ListaDepartamentos));
            if (admin)
            {
                Eventos.ResponsableAgregado += CargarResponsables;
                ListaResponsables = await profesorServicio.GetAllAsync();
                OnPropertyChanged(nameof(ListaResponsables));
                _ListaIncidencias = new ListCollectionView(
                    (await incidenciaServicio.GetAllAsync()).ToList()
                );
            }
            if (admin || responsable)
            {
                Eventos.EstadoAgregado += CargarEstados;
                ListaEstados = new ObservableCollection<Estado>(await estadoServicio.GetAllAsync());
                OnPropertyChanged(nameof(ListaEstados));
            }
            if (responsable && !admin)
            {
                _ListaIncidencias = new ListCollectionView(
                    (
                        await incidenciaServicio.FindAsync(i =>
                            (i.IdResponsable == usuario.IdProfesor)
                            || (i.IdProfesor == usuario.IdProfesor)
                        )
                    ).ToList()
                );
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
                }
            }

            _ListaIncidenciasFiltrada = _ListaIncidencias;
            OnPropertyChanged(nameof(ListaIncidencias));
            OnPropertyChanged(nameof(ListaIncidenciasFiltrada));
            CargarTiposHardware();

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

        /// <summary>
        /// The FiltroCriterios
        /// </summary>
        /// <param name="item">The item<see cref="object"/></param>
        /// <returns>The <see cref="bool"/></returns>
        private bool FiltroCriterios(object item)
        {
            if (item is not Incidencia incidencia)
                return false;

            return criterios.TrueForAll(c => c(incidencia));
        }

        /// <summary>
        /// The InicializaCriterios
        /// </summary>
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

        /// <summary>
        /// The Filtrar
        /// </summary>
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

        /// <summary>
        /// The FiltroIncidencias
        /// </summary>
        /// <param name="obj">The obj<see cref="object"/></param>
        /// <returns>The <see cref="bool"/></returns>
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

        /// <summary>
        /// The LimpiarFiltros
        /// </summary>
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

        /// <summary>
        /// The OnFiltroChanged
        /// </summary>
        private void OnFiltroChanged()
        {
            ListaIncidenciasFiltrada.Refresh();
            OnPropertyChanged(nameof(ListaIncidenciasFiltrada));
        }

        /// <summary>
        /// The ActualizarVisibilidadCamposHardware
        /// </summary>
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

        /// <summary>
        /// Gets a value indicating whether guarda
        /// </summary>
        ///
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
                    catch (Exception)
                    {
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

        /// <summary>
        /// Gets a value indicating whether borrar
        /// </summary>
        public bool borrar
        {
            get
            {
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

        /// <summary>
        /// The CancelarIncidencia
        /// </summary>
        public void CancelarIncidencia()
        {
            _selectedIncidencia = new Incidencia();
        }
    }
}
