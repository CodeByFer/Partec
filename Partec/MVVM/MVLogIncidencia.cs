using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using Partec.Backend.Modelo;
using Partec.Backend.Servicios;
using Partec.MVVM.Base;

namespace Partec.MVVM
{
    class MVLogIncidencia : MVBaseCRUD<LogIncidencia>
    {
        private GestionincidenciasContext _context;
        private LogIncidenciaServicio _logIncidenciaServicio;
        private ListCollectionView _listaLogs;
        public ListCollectionView ListaLogs
        {
            get => _listaLogs;
            set
            {
                _listaLogs = value;
                OnPropertyChanged(nameof(ListaLogs));
            }
        }

        public MVLogIncidencia(GestionincidenciasContext context)
        {
            _context = context;

            _logIncidenciaServicio = new LogIncidenciaServicio(_context);
        }

        public async Task Inicializar()
        {
            try
            {
                ListaLogs = new ListCollectionView(
                    (await _logIncidenciaServicio.GetAllAsync()).ToList()
                );
            }
            catch (Exception ex)
            {
                // Manejo de errores, por ejemplo, registrar el error o mostrar un mensaje al usuario
                Console.WriteLine($"Error al inicializar MVLogIncidencia: {ex.Message}");
            }
        }
    }
}
