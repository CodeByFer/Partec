using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Partec.Backend.Modelo;
using Partec.Backend.Servicios.Base;

namespace Partec.Backend.Servicios
{
    public class TipoHardwareServicio : ServicioGenerico<TipoHardware>
    {
        private GestionincidenciasContext context;

        public TipoHardwareServicio(GestionincidenciasContext context) : base(context)
        {
            this.context = context;
        }
    }
}
