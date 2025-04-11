using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Partec.Backend.Modelo;
using Partec.Backend.Servicios.Base;

namespace Partec.Backend.Servicios
{
    public class PermisoServicio : ServicioGenerico<Permiso>
    {
        private GestionincidenciasContext context;

        public PermisoServicio(GestionincidenciasContext context) : base(context)
        {
            this.context = context;
        }
    }
}
