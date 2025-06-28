using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace Servicios
{
    public static class ServicioEncriptador
    {
        public static string Hash(string value)
        {
            var md5 = new MD5CryptoServiceProvider();
            var md5data = md5.ComputeHash(Encoding.ASCII.GetBytes(value));
            return (new ASCIIEncoding()).GetString(md5data);
        }

        // Genera un hash sha 256 y una sal aleatoria a partir de la contraseña.
        public static void CreateHash(string password,
            out string passwordHash,
            out string salt,
            int iterations = 100_000)
        {
            // 1) Generar sal aleatoria (16 bytes)
            byte[] saltBytes = new byte[16];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(saltBytes);
            }
            salt = Convert.ToBase64String(saltBytes);

            // 2) Derivar el hash con PBKDF2 (SHA-256)
            byte[] hashBytes;
            using (var derive = new Rfc2898DeriveBytes(
                password,
                saltBytes,
                iterations,
                HashAlgorithmName.SHA256))
            {
                hashBytes = derive.GetBytes(32); // 32 bytes = 256 bits
            }

            // 3) Convertir a Base64 para almacenar
            passwordHash = Convert.ToBase64String(hashBytes);
        }


        // Calcula el hash a partir de la contraseña en claro y la sal (ambos en Base64).
        public static string ComputeHash(string password, string salt, int iterations = 100_000)
        {
            // 1) Decodificar la sal
            byte[] saltBytes = Convert.FromBase64String(salt);

            // 2) Derivar el hash con PBKDF2 (SHA-256)
            byte[] hashBytes;
            using (var derive = new Rfc2898DeriveBytes(
                password,
                saltBytes,
                iterations,
                HashAlgorithmName.SHA256))
            {
                hashBytes = derive.GetBytes(32);
            }

            // 3) Convertir a Base64 para comparar
            return Convert.ToBase64String(hashBytes);
        }
    }
}

