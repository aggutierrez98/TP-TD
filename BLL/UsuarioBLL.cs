using System;
using System.Collections.Generic;
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
                throw new Exception("El nombre de usuario ya existe. Por favor elegí otro.");
            }

            string hash = passwordHasher.HashPassword(passwordRaw);

            Usuario nuevoUser = new Usuario(username)
            {
                PasswordHash = hash
            };

            return usuarioDAL.Insertar(nuevoUser);
        }

        public List<Usuario> ObtenerTodos()
        {
            return usuarioDAL.ObtenerTodos();
        }

        public bool Actualizar(int id, string nuevoUsername)
        {
            if (string.IsNullOrWhiteSpace(nuevoUsername))
            {
                throw new Exception("El nombre de usuario no puede estar vacío.");
            }

            // Verificar unicidad: si existe otro usuario con ese username, rechazar
            Usuario existente = usuarioDAL.ObtenerPorUsername(nuevoUsername);
            if (existente != null && existente.Id != id)
            {
                throw new Exception("El nombre de usuario ya existe. Por favor elegí otro.");
            }

            Usuario usuarioActualizado = new Usuario
            {
                Id = id,
                Username = nuevoUsername
            };

            return usuarioDAL.Actualizar(usuarioActualizado);
        }

        public bool Eliminar(int id)
        {
            return usuarioDAL.Eliminar(id);
        }
    }
}
