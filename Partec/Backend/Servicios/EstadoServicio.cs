using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Partec.Backend.Modelo;
using Partec.Backend.Servicios.Base;

namespace Partec.Backend.Servicios
{
    public class EstadoServicio : ServicioGenerico<Estado>
    {
        private GestionincidenciasContext contexto;

        public EstadoServicio(GestionincidenciasContext contexto) : base(contexto)
        {
            this.contexto = contexto;
        }
    }
}
