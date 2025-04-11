using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Partec.Backend.Modelo;
using Partec.Backend.Servicios.Base;

namespace Partec.Backend.Servicios
{
    public class DepartamentoServicio : ServicioGenerico<Departamento>
    {
        private GestionincidenciasContext contexto;
        public DepartamentoServicio(GestionincidenciasContext context) : base(context)
        {
            this.contexto = context;
        }


    }
}
