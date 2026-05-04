using ABSTRACCIONES;
using System;
using System.Security.Cryptography;

namespace SERVICIOS
{
    public class PasswordHasher : IPasswordHasher
    {
        private const int SaltSize = 16; // Tamaño del salt
        private const int HashSize = 32; // Tamaño para el hash
        private const int Iterations = 20000; // Numero de iteraciones del algoritmo PBKDF2

        public string HashPassword(string password)
        {
            byte[] salt;
            new RNGCryptoServiceProvider().GetBytes(salt = new byte[SaltSize]);

            // Creamos el hash con el password y el salt
            Rfc2898DeriveBytes pbkdf2 = new Rfc2898DeriveBytes(password, salt, Iterations);
            byte[] hash = pbkdf2.GetBytes(HashSize);

            // Combinamos el salt y el hash en un solo array de bytes (queda como [salt,hash])
            byte[] hashBytes = new byte[SaltSize + HashSize];

            // Copiamos el salt al inicio del array de bytes
            Array.Copy(salt, 0, hashBytes, 0, SaltSize);

            // Copiamos el hash a continuación del salt en el array de bytes
            Array.Copy(hash, 0, hashBytes, SaltSize, HashSize);

            return Convert.ToBase64String(hashBytes);
        }

        public bool VerifyPassword(string password, string storedHash)
        {
            // Obtenemos los bytes del hash que esta en la BD
            byte[] storedHashBytes = Convert.FromBase64String(storedHash);

            // Extraemos el salt del hash en BD
            byte[] storedHashSalt = new byte[SaltSize];
            Array.Copy(storedHashBytes, 0, storedHashSalt, 0, SaltSize);

            // Extraemos el hash original (sin salt)
            byte[] originalStoredHash = new byte[HashSize];
            Array.Copy(storedHashBytes, SaltSize, originalStoredHash, 0, HashSize);

            // Generamos el hash del password ingresado por el usuario con el hash y salt original (guardado en BD)
            Rfc2898DeriveBytes pbkdf2 = new Rfc2898DeriveBytes(password, storedHashSalt, Iterations);
            byte[] testHash = pbkdf2.GetBytes(HashSize);

            // Comparamos byte a byte para evitar falsos negativos.
            if (originalStoredHash.Length != testHash.Length)
            {
                return false;
            }

            for (int i = 0; i < originalStoredHash.Length; i++)
            {
                if (originalStoredHash[i] != testHash[i])
                {
                    return false;
                }
            }

            return true;
        }

    }
}
