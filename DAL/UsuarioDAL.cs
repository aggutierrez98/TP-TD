using BE;
using System.Collections.Generic;
using System.Data;

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
            using (AccesoDatos accesoDatos = new AccesoDatos())
            {
                List<System.Data.SqlClient.SqlParameter> parametros = new List<System.Data.SqlClient.SqlParameter>
                {
                    accesoDatos.CreateParameter("@Username", SqlDbType.VarChar, username)
                };

                DataTable dt = accesoDatos.Read(query, parametros);

                if (dt.Rows.Count > 0)
                {
                    return Map(dt.Rows[0]);
                }

                return null;
            }
        }

        public bool Insertar(Usuario usuario)
        {
            string query = "INSERT INTO Usuarios (Username, PasswordHash) VALUES (@Username, @PasswordHash)";
            using (AccesoDatos accesoDatos = new AccesoDatos())
            {
                List<System.Data.SqlClient.SqlParameter> parametros = new List<System.Data.SqlClient.SqlParameter>
                {
                    accesoDatos.CreateParameter("@Username", SqlDbType.VarChar, usuario.Username),
                    accesoDatos.CreateParameter("@PasswordHash", SqlDbType.VarChar, usuario.PasswordHash)
                };

                int afectadas = accesoDatos.Write(query, parametros);

                return afectadas > 0;
            }
        }

        public List<BE.Usuario> ObtenerTodos()
        {
            string query = "SELECT Id, Username, PasswordHash FROM Usuarios ORDER BY Username";
            using (AccesoDatos accesoDatos = new AccesoDatos())
            {
                DataTable dt = accesoDatos.Read(query);

                List<BE.Usuario> lista = new List<BE.Usuario>();
                foreach (DataRow row in dt.Rows)
                {
                    lista.Add(Map(row));
                }
                return lista;
            }
        }

        public bool Actualizar(Usuario usuario)
        {
            string query = "UPDATE Usuarios SET Username = @Username WHERE Id = @Id";
            using (AccesoDatos accesoDatos = new AccesoDatos())
            {
                List<System.Data.SqlClient.SqlParameter> parametros = new List<System.Data.SqlClient.SqlParameter>
                {
                    accesoDatos.CreateParameter("@Username", SqlDbType.VarChar, usuario.Username),
                    accesoDatos.CreateParameter("@Id", SqlDbType.Int, usuario.Id)
                };

                int afectadas = accesoDatos.Write(query, parametros);

                return afectadas > 0;
            }
        }

        public bool Eliminar(int id)
        {
            string query = @"
DELETE FROM UsuarioPreferencias WHERE UsuarioId = @Id;
DELETE FROM UsuarioPermiso WHERE UsuarioId = @Id;
DELETE FROM Usuarios WHERE Id = @Id";
            using (AccesoDatos accesoDatos = new AccesoDatos())
            {
                List<System.Data.SqlClient.SqlParameter> parametros = new List<System.Data.SqlClient.SqlParameter>
                {
                    accesoDatos.CreateParameter("@Id", SqlDbType.Int, id)
                };

                int afectadas = accesoDatos.Write(query, parametros);

                return afectadas > 0;
            }
        }
    }
}
