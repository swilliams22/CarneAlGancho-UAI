using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abstracciones
{
    public interface IUsuario
    {
        int Id { get; }
        string Email { get; }
        string PasswordHash { get; }
        string Salt { get; }
        int Retries { get; }
        bool Active { get; }
    }
}
