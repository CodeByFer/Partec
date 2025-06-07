using System.Collections.ObjectModel;
using System.Windows.Data;
using System.Windows.Input;
using Microsoft.EntityFrameworkCore;
using Partec.Backend.Modelo;
using Partec.Backend.Servicios;
using Partec.Frontend.Dialogos;
using Partec.MVVM.Base;

namespace Partec.MVVM
{
    public class MVDepartamento : MVBaseCRUD<Departamento>
    {
        private DepartamentoServicio departamentoServicio;
        public ListCollectionView ListaDepartamentos { get; set; }

        private Departamento _selectedDepartamento;
        public Departamento SelectedDepartamento
        {
            get => _selectedDepartamento;
            set
            {
                _selectedDepartamento = value;
                OnPropertyChanged(nameof(SelectedDepartamento));
            }
        }

        public Departamento Departamento { get; set; } = new Departamento();

        public MVDepartamento(GestionincidenciasContext context)
        {
            try
            {
                context.Database.OpenConnection();
            }
            catch (Exception ex) { }
            departamentoServicio = new DepartamentoServicio(context);
            servicio = departamentoServicio;
        }

        public async Task Inicializa()
        {
            ListaDepartamentos = new ListCollectionView(
                (await departamentoServicio.GetAllAsync()).ToList()
            );
            OnPropertyChanged(nameof(ListaDepartamentos));
        }

        public void cancelar() { }

        public Boolean guarda
        {
            get
            {
                if (SelectedDepartamento.IdDepartamento == 0)
                {
                    var result = Task.Run(() => Add(SelectedDepartamento)).Result;
                    if (result)
                    {
                        Eventos.OnDepartamentoAgregado();
                        ListaDepartamentos.AddNewItem(SelectedDepartamento);
                        ListaDepartamentos.CommitNew();
                        ListaDepartamentos.Refresh();
                    }
                    return result;
                }
                else
                {
                    var result = Task.Run(() => Update(SelectedDepartamento)).Result;
                    if (result)
                    {
                        Eventos.OnDepartamentoAgregado();
                        ListaDepartamentos.Refresh();
                    }
                    return result;
                }
            }
        }
        public Boolean borra
        {
            get
            {
                if (SelectedDepartamento.IdDepartamento != 0)
                {
                    var result = Task.Run(() => Delete(SelectedDepartamento.IdDepartamento)).Result;
                    if (result)
                    {
                        Eventos.OnDepartamentoAgregado();
                        ListaDepartamentos.Remove(SelectedDepartamento);
                        ListaDepartamentos.Refresh();
                    }
                    return result;
                }
                return false;
            }
        }
    }
}
