using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Partec.Backend.Modelo;

namespace Partec
{
    public static class Eventos
    {
        public static event Action TipoHardwareAgregado;
        public static event Action DepartamentoAgregado;
        public static event Action ResponsableAgregado;
        public static event Action RolModificado;
        public static event Action EstadoAgregado;
        public static event Action<Incidencia> IncidenciaAgregada;
        public static event Action<Incidencia> ResponsableIncidenciaAgregado;
        public static event Action<Profesor, Incidencia> ResponsableIncidenciaModificado;

        public static async void OnIncidenciaAgregada(Incidencia obj)
        {
            IncidenciaAgregada?.Invoke(obj);
        }

        public static void OnTipoHardwareAgregado()
        {
            TipoHardwareAgregado?.Invoke();
        }

        public static void OnResponsableAgregado()
        {
            ResponsableAgregado?.Invoke();
        }

        public static void OnDepartamentoAgregado()
        {
            DepartamentoAgregado?.Invoke();
        }

        public static void OnEstadoAgregado()
        {
            EstadoAgregado?.Invoke();
        }

        public static void OnRolModificado()
        {
            RolModificado?.Invoke();
        }

        public static async void OnResponsableIncidenciaAgregado(Incidencia incidencia)
        {
            ResponsableIncidenciaAgregado?.Invoke(incidencia);
        }

        public static async void OnResponsableIncidenciaCambiado(
            Profesor profesorAntiguo,
            Incidencia incidencia
        )
        {
            ResponsableIncidenciaModificado?.Invoke(profesorAntiguo, incidencia);
        }
    }
}
