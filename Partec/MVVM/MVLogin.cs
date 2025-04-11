using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Partec.Backend.Modelo;
using Partec.Backend.Servicios;
using Partec.MVVM.Base;

namespace Partec.MVVM
{
    public class MVLogin : MVBaseCRUD<Profesor>
    {
        private ProfesorServicio profesorServicio;
        private Profesor _usuario = new Profesor();
        private GestionincidenciasContext context;
        public Profesor usuario => _usuario;


        public String username
        {
            get
            {
                return _usuario.Email;
            }
            set
            {
                _usuario.Email = value; OnPropertyChanged(nameof(username));
            }
        }
        public String password
        {
            get
            {
                return _usuario.Password;
            }
            set
            {
                _usuario.Password = value; OnPropertyChanged(nameof(password));
            }
        }


        public MVLogin(GestionincidenciasContext context)
        {
            this.context = context;
            profesorServicio = new ProfesorServicio(context);

        }

        public async Task<Boolean> login()
        {
            IEnumerable<Profesor> usu = await profesorServicio.FindAsync(p => p.Email == username && p.Password == password);
            if (usu.Count() > 0)
            {
                return true;
            }
            else { return false; }
        }
    }
}
