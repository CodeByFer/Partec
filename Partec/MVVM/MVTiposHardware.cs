using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using Microsoft.EntityFrameworkCore;
using MySqlX.XDevAPI.Common;
using Partec.Backend.Modelo;
using Partec.Backend.Servicios;
using Partec.MVVM.Base;

namespace Partec.MVVM
{
    public class MVTiposHardware : MVBaseCRUD<TipoHardware>
    {
        private TipoHardwareServicio tipoHardwareServicio;
        private GestionincidenciasContext context;
        private TipoHardware _selectedTipoHardware;
        public ListCollectionView ListaTiposHardware { get; set; }
        public TipoHardware SelectedTipoHardware
        {
            get => _selectedTipoHardware;
            set
            {
                _selectedTipoHardware = value;
                OnPropertyChanged(nameof(SelectedTipoHardware));
            }
        }

        public MVTiposHardware(GestionincidenciasContext contexto)
        {
            this.context = contexto;
            try
            {
                context.Database.OpenConnection();
            }
            catch (Exception ex) { }
            tipoHardwareServicio = new TipoHardwareServicio(context);
            servicio = tipoHardwareServicio;
        }

        public async Task<bool> inicializa()
        {
            ListaTiposHardware = new ListCollectionView(
                (await tipoHardwareServicio.GetAllAsync()).ToList()
            );
            OnPropertyChanged(nameof(ListaTiposHardware));
            return true;
        }

        public bool guarda
        {
            get
            {
                var result = false;
                if (_selectedTipoHardware.IdTipoHw == 0)
                {
                    result = Task.Run(() => Add(_selectedTipoHardware)).Result;
                    if (result)
                    {
                        ListaTiposHardware.AddNewItem(_selectedTipoHardware);
                        ListaTiposHardware.CommitNew();
                        Eventos.OnTipoHardwareAgregado();
                        OnPropertyChanged(nameof(ListaTiposHardware));
                    }
                }
                else
                    result = Task.Run(() => Update(_selectedTipoHardware)).Result;
                if (result)
                {
                    Eventos.OnTipoHardwareAgregado();
                    ListaTiposHardware.Refresh();
                    OnPropertyChanged(nameof(SelectedTipoHardware));
                    OnPropertyChanged(nameof(ListaTiposHardware));
                }
                return result;
            }
        }

        public bool borra
        {
            get
            {
                var result = false;
                if (_selectedTipoHardware.IdTipoHw != 0)
                {
                    result = Task.Run(() => Delete(_selectedTipoHardware.IdTipoHw)).Result;
                    if (result)
                    {
                        Eventos.OnTipoHardwareAgregado();
                        ListaTiposHardware.Remove(_selectedTipoHardware);
                        ListaTiposHardware.Refresh();
                        OnPropertyChanged(nameof(ListaTiposHardware));
                    }
                    return result;
                }
                else
                    return false;
            }
        }
    }
}
