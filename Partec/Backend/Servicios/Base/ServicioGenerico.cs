using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NLog;

namespace Partec.Backend.Servicios.Base
{
    public class ServicioGenerico<T> : IServicioGenerico<T> where T : class
    {
        
        private readonly DbContext _context; // Contexto de la base de datos
        private readonly DbSet<T> _dbSet;    // DbSet para la entidad genérica
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        public ServicioGenerico(DbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        public async Task<T> GetByIDAsync(int id)
        {
            try
            {

                return await _dbSet.FindAsync(id);
            }
            catch (Exception ex)
            {
                GuardarExcepcion(ex, $"Error al obtener entidad de tipo {typeof(T).Name}");
                throw new Exception("Error BD Individual");
            }
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            /*"Select * from Table"*/
            try
            {

                return await _dbSet.ToListAsync();
            }
            catch (Exception ex)
            {
                GuardarExcepcion(ex, $"Error al obtener todas las entidades de tipo {typeof(T).Name}");
                throw new Exception("Error BD Tablas");
            }
        }

        public async Task<bool> AddAsync(T entity)
        {
            bool resultado = true;
            try
            {

               _= _dbSet.AddAsync(entity);
                await _context.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                resultado = false;
                GuardarExcepcion(ex, $"Error al agregar entidad de tipo {typeof(T).Name}");

            }


            return resultado;
        }

        public async Task<bool> UpdateAsync(T entity)
        {
            bool lito = true;
            try
            {

                _dbSet.Update(entity);
                // _context.Entry(entity).State = EntityState.Modified;
                await _context.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                lito = false;
                GuardarExcepcion(ex, $"Error al actualizar entidad de tipo {typeof(T).Name}");
                throw new Exception("Error al Actualizar");
            }
            return lito;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            bool lito = true;
            try
            {

                T entity = await _dbSet.FindAsync(id);

                if (entity != null)
                {
                    _dbSet.Remove(entity);
                    await _context.SaveChangesAsync();

                }
                else
                {

                }
            }
            catch (Exception ex)
            {
                lito = false;
                GuardarExcepcion(ex, $"Error al eliminar entidad de tipo {typeof(T).Name} con ID");

            }

            return lito;
        }

        public async Task<IEnumerable<T>> FindAsync(Expression<Func</*Lo que recibe*/T, /*Lo que devuelve*/bool>> predicate)
        {
            try
            {

                return await _dbSet.Where(predicate).ToListAsync();
            }
            catch (Exception ex)
            {
                GuardarExcepcion(ex, $"Error al buscar entidades de tipo {typeof(T).Name}");
                throw new Exception("Error en la SQL");
            }
        }

        private void GuardarExcepcion(Exception ex, string mensaje)
        {
            logger.Error(mensaje + "\n" + ex.InnerException);
            logger.Error(ex.Message);
            logger.Error(ex.StackTrace);
        }
    }
}

