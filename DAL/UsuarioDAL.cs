using BE;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace DAL
{
    public class UsuarioDAL
    {
        public UsuarioDAL()
        {
        }

        private Usuario Map(DataRow row)
        {
            return new Usuario
            {
                Id = (int)row["Id"],
                Username = row["Username"].ToString(),
                PasswordHash = row["PasswordHash"].ToString()
            };
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
                return Map(dt.Rows[0]);
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
            int afectadas = access.Write(query, parametros);
            access.Close();

            return afectadas > 0;
        }

        public List<BE.Usuario> ObtenerTodos()
        {
            string query = "SELECT Id, Username, PasswordHash FROM Usuarios ORDER BY Username";

            Access access = new Access();
            access.Open();
            DataTable dt = access.Read(query);
            access.Close();

            List<BE.Usuario> lista = new List<BE.Usuario>();
            foreach (DataRow row in dt.Rows)
            {
                lista.Add(Map(row));
            }
            return lista;
        }

        public bool Actualizar(Usuario usuario)
        {
            string query = "UPDATE Usuarios SET Username = @Username WHERE Id = @Id";
            List<SqlParameter> parametros = new List<SqlParameter>
            {
                new SqlParameter("@Username", SqlDbType.VarChar) { Value = usuario.Username },
                new SqlParameter("@Id", SqlDbType.Int) { Value = usuario.Id }
            };

            Access access = new Access();
            access.Open();
            int afectadas = access.Write(query, parametros);
            access.Close();

            return afectadas > 0;
        }

        public bool Eliminar(int id)
        {
            string query = "DELETE FROM Usuarios WHERE Id = @Id";
            List<SqlParameter> parametros = new List<SqlParameter>
            {
                new SqlParameter("@Id", SqlDbType.Int) { Value = id }
            };

            Access access = new Access();
            access.Open();
            int afectadas = access.Write(query, parametros);
            access.Close();

            return afectadas > 0;
        }
    }
}
