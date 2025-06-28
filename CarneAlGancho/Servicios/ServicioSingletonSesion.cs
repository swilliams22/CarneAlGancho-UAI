using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;

namespace Servicios
{
    public class ServicioSingletonSesion
    {
        private static ServicioSesion _instancia;
        private static object _lock = new object();

        public static ServicioSesion Instancia
        {
            get
            {
                lock (_lock)
                {
                    if (_instancia == null) 
                        _instancia = new ServicioSesion();
                }

                return _instancia;
            }
        }
    }
}
