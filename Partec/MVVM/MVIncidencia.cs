namespace Partec.MVVM
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Input;
    using Partec.Backend.Modelo;
    using Partec.Backend.Servicios;
    using Partec.MVVM.Base;

    public class MVIncidencia : MVBaseCRUD<Incidencia>
    {
        private Incidencia _selectedIncidencia;
        public Incidencia IncidenciaSeleccionada { get; set; } = new Incidencia();
        public IEnumerable<Incidencia> ListaIncidencias { get; set; }

        public IEnumerable<Profesor> ListaResponsables { get; set; }

        private GestionincidenciasContext context;

        private IncidenciaServicio incidenciaServicio;
        public IEnumerable<String> TiposIncidencia { get; set; } = ["HW", "SW"];

        public IEnumerable<Departamento> ListaDepartamentos { get; set; }

        public IEnumerable<TipoHardware> ListaTiposHardware { get; set; }

        private Visibility _hardwareFieldsVisibility = Visibility.Collapsed;
        public Incidencia SelectedIncidencia
        {
            get => _selectedIncidencia;
            set
            {
                _selectedIncidencia = value;
                OnPropertyChanged(nameof(SelectedIncidencia));
            }
        }

        public MVIncidencia(GestionincidenciasContext context)
        {
            this.context = context;
            ListaIncidencias = new ObservableCollection<Incidencia>();
            ListaResponsables = new ObservableCollection<Profesor>();
            incidenciaServicio = new IncidenciaServicio(this.context);
        }

        public async Task CargarIncidenciasAsync()
        {
            ListaIncidencias = await incidenciaServicio.GetAllAsync();
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

        public void ActualizarVisibilidadCamposHardware()
        {
            if (IncidenciaSeleccionada.TipoIncidencia == "HW")
                HardwareFieldsVisibility = Visibility.Visible;
            else
                HardwareFieldsVisibility = Visibility.Collapsed;
        }

        public bool guarda
        {
            get { return Task.Run(() => Update(_selectedIncidencia)).Result; }
        }

        private void CancelarIncidencia() { }
    }
}
