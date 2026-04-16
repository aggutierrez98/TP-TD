using BE;
using DAL.Mappers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace DAL
{
    public class UsuarioDAL
    {
        private UsuarioMapper mapper;

        public UsuarioDAL()
        {
            mapper = new UsuarioMapper();
        }

        public BE.Usuario ObtenerPorUsername(string username)
        {
            string query = "SELECT Id, Username, PasswordHash FROM Usuarios WHERE Username = @Username";
            List<SqlParameter> parametros = new List<SqlParameter>
            {
                new SqlParameter("@Username", SqlDbType.VarChar) { Value = username }
            };

            Access access = new Access();

            access.Open();
            DataTable dt = access.Read(query, parametros);
            access.Close();

            if (dt.Rows.Count > 0)
            {
                return mapper.Map(dt.Rows[0]);
            }

            return null;
        }

        public bool Insertar(Usuario usuario)
        {
            string query = "INSERT INTO Usuarios (Username, PasswordHash) VALUES (@Username, @PasswordHash)";
            List<SqlParameter> parametros = new List<SqlParameter>
            {
                new SqlParameter("@Username", SqlDbType.VarChar) { Value = usuario.Username },
                new SqlParameter("@PasswordHash", SqlDbType.VarChar) { Value = usuario.PasswordHash }
            };

            Access access = new Access();

            access.Open();
            int afactadas = access.Write(query, parametros);
            access.Close();

            return afactadas > 0;
        }
    }
}
