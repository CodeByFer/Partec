using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Partec.Backend.Modelo;
using Partec.Backend.Servicios.Base;
using SkiaSharp;

namespace Partec.Backend.Servicios
{
    public class IncidenciaServicio : ServicioGenerico<Incidencia>
    {
        private GestionincidenciasContext context;

        public IncidenciaServicio(GestionincidenciasContext context)
            : base(context)
        {
            try
            {
                context.Database.OpenConnection();
            }
            catch (Exception ex) { }
            this.context = context;
        }

        public async Task<IEnumerable<Incidencia>> GetAllAllAsync()
        {
            try
            {
                return await context
                    .Incidencias.Include(i => i.IdEstadoNavigation)
                    .Include(i => i.IdDepartamentoNavigation)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                GuardarExcepcion(
                    ex,
                    $"Error al obtener todas las entidades de tipo {typeof(Incidencia).Name}"
                );
                throw new Exception("Error BD Tablas");
            }
        }
    }
}
