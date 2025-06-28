using Abstracciones;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Servicios
{
    public class ServicioSesion
    {
        private IUsuario _user {  get; set; }

        public IUsuario Usuario
        {
            get
            {
                return _user;
            }
        }

        public void Login(IUsuario usuario)
        {
            _user = usuario;
        }

        public void Logout()
        {
            _user = null;
        }

        public bool IsLogged()
        {
            return _user != null;
        }
    }
}
