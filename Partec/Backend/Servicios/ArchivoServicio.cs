using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Partec.Backend.Modelo;
using Partec.Backend.Servicios.Base;

namespace Partec.Backend.Servicios
{
    public class ArchivoServicio : ServicioGenerico<Archivo>
    {
        private GestionincidenciasContext contexto;

        public ArchivoServicio(GestionincidenciasContext context) : base(context) 
        {
            this.contexto = context;
        }
     

        
    }
}
