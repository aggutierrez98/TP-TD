using System;
using BE;
using DAL;
using SERVICIOS;
using ABSTRACCIONES;

namespace BLL
{
    public class UsuarioBLL
    {
        private UsuarioDAL usuarioDAL;
        private IPasswordHasher passwordHasher;

        public UsuarioBLL()
        {
            usuarioDAL = new UsuarioDAL();
            passwordHasher = new PasswordHasher();
        }

        public Usuario Login(string username, string passwordRaw)
        {


            Usuario usuarioHash = usuarioDAL.ObtenerPorUsername(username);
            if (usuarioHash == null)
            {
                return null;
            }

            bool esValido = passwordHasher.VerifyPassword(passwordRaw, usuarioHash.PasswordHash);
            if (esValido)
            {
                return usuarioHash;
            }

            return null;
        }

        public bool Registrar(string username, string passwordRaw)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(passwordRaw))
            {
                throw new Exception("El usuario y la contraseña no pueden estar vacíos.");
            }

            Usuario existente = usuarioDAL.ObtenerPorUsername(username);
            if (existente != null)
            {
                throw new Exception("El usuario ya existe.");
            }

            string hash = passwordHasher.HashPassword(passwordRaw);

            Usuario nuevoUser = new Usuario(username)
            {
                PasswordHash = hash
            };

            return usuarioDAL.Insertar(nuevoUser);
        }
    }
}
