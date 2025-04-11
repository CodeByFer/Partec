using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Partec.Backend.Servicios.Base
{
    public interface IServicioGenerico<T> where T : class
    {

        //Devuelve un objeto de la clase segun el id enviado
        Task<T> GetByIDAsync(int id);

        //Devuelve todos los elementos de una tabla --> Select * from
        Task<IEnumerable<T>> GetAllAsync();

        //Agrega un objeto a la tabla
        Task<bool> AddAsync(T entity);

        //Actualiza un objeto en la tabla
        Task<bool> UpdateAsync(T entity);

        //Elimina un objeto d ela tabla
        Task<bool> DeleteAsync(int id);

        //Devuelve una lista filtrada. Select * . . . where
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);

    }
}
