using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BE;
using SingeltonSession;
using DAL;
using Servicios;

namespace BLL
{
    public class UsuarioBLL
    {
        public LoginResult Login(string usuario, string password)
        {
            if (ServicioSingletonSesion.Instancia.IsLogged())
                throw new Exception("Ya hay una sesión iniciada");

            var usuarioCon = UsuarioDAL.GetByEmail(usuario);

            // 1- Revisa que haya encontrado el usuario en la BD, si no envia excepcion
            if (usuarioCon == null) throw new LoginException(LoginResult.InvalidUsername);

            // 2- Revisa que no este bloqueado
            if (!usuarioCon.Active) throw new LoginException(LoginResult.BloquedUser);

            // 3- Revisa que cocincida la clave, si no envia excepcion
            if (ServicioEncriptador.ComputeHash(password, usuarioCon.Salt) != usuarioCon.PasswordHash)
            {
                // Aumenta intentos si la clave no coincide
                usuarioCon.Retries++;
                // Actualiza DB
                UsuarioDAL.UpdateRetries(usuarioCon);

                // Bloquea el usuario si llega a 3 intentos fallidos
                if (usuarioCon.Retries >= 3)
                {
                    usuarioCon.Active = false;
                    UsuarioDAL.BlockUser(usuarioCon);
                    throw new LoginException(LoginResult.BloquedUser);
                }

                // Clave incorrecta
                throw new LoginException(LoginResult.InvalidPassword);
            }
            else
            {
                if (usuarioCon.Retries > 0)
                {
                    usuarioCon.Retries = 0;
                    UsuarioDAL.UpdateRetries(usuarioCon);
                }

                ServicioSingletonSesion.Instancia.Login(usuarioCon);
                return LoginResult.ValidUser;
            }
        }

        public bool CheckUsersDVH()
        {
            return UsuarioDAL.CheckUsersDVH();
        }

        public void CalculateDVV()
        {
            UsuarioDAL.CalculateDVV();
        }

        public void Logout()
        {
            if (!ServicioSingletonSesion.Instancia.IsLogged())
                throw new Exception("No hay sesion iniciada");

            ServicioSingletonSesion.Instancia.Logout();
        }

        public int UnblockUser(string usuario)
        {
            return UsuarioDAL.UnblockUser(usuario);
        }
    }
}
