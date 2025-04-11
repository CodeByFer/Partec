using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Partec.Backend.Modelo;
using Partec.Backend.Servicios.Base;

namespace Partec.Backend.Servicios
{
    public class IncidenciaServicio : ServicioGenerico<Incidencia>
    {
        private GestionincidenciasContext context;

        public IncidenciaServicio(GestionincidenciasContext context) : base(context)
        {
            this.context = context;
        }
    }
}
