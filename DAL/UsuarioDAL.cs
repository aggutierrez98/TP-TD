using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using BE;

namespace DAL
{
    public class UsuarioDAL
    {
        private Usuario Map(DataRow row)
        {
            return new Usuario
            {
                Id = (int)row["Id"],
                Username = row["Username"].ToString(),
                PasswordHash = row["PasswordHash"].ToString(),
                Activo = (bool)row["Activo"]
            };
        }

        public Usuario ObtenerPorId(int id, AccesoDatos accesoDatos = null)
        {
            const string query = @"
SELECT Id, Username, PasswordHash, Activo
FROM Usuarios
WHERE Id = @Id";

            bool dispose = accesoDatos == null;
            accesoDatos = accesoDatos ?? new AccesoDatos();

            try
            {
                List<SqlParameter> parametros = new List<SqlParameter>
                {
                    accesoDatos.CreateParameter("@Id", SqlDbType.Int, id)
                };

                DataTable dataTable = accesoDatos.Read(query, parametros);
                return dataTable.Rows.Count == 0 ? null : Map(dataTable.Rows[0]);
            }
            finally
            {
                if (dispose)
                {
                    accesoDatos.Dispose();
                }
            }
        }

        public Usuario ObtenerPorUsername(string username, AccesoDatos accesoDatos = null)
        {
            const string query = @"
SELECT Id, Username, PasswordHash, Activo
FROM Usuarios
WHERE Username = @Username";

            bool dispose = accesoDatos == null;
            accesoDatos = accesoDatos ?? new AccesoDatos();

            try
            {
                List<SqlParameter> parametros = new List<SqlParameter>
                {
                    accesoDatos.CreateParameter("@Username", SqlDbType.VarChar, username)
                };

                DataTable dataTable = accesoDatos.Read(query, parametros);
                return dataTable.Rows.Count == 0 ? null : Map(dataTable.Rows[0]);
            }
            finally
            {
                if (dispose)
                {
                    accesoDatos.Dispose();
                }
            }
        }

        public int InsertarYObtenerId(Usuario usuario, AccesoDatos accesoDatos = null)
        {
            const string query = @"
INSERT INTO Usuarios (Username, PasswordHash, Activo)
OUTPUT INSERTED.Id
VALUES (@Username, @PasswordHash, @Activo)";

            bool dispose = accesoDatos == null;
            accesoDatos = accesoDatos ?? new AccesoDatos();

            try
            {
                List<SqlParameter> parametros = new List<SqlParameter>
                {
                    accesoDatos.CreateParameter("@Username", SqlDbType.VarChar, usuario.Username),
                    accesoDatos.CreateParameter("@PasswordHash", SqlDbType.VarChar, usuario.PasswordHash),
                    accesoDatos.CreateParameter("@Activo", SqlDbType.Bit, usuario.Activo)
                };

                return accesoDatos.WriteWithResult(query, parametros);
            }
            finally
            {
                if (dispose)
                {
                    accesoDatos.Dispose();
                }
            }
        }

        public bool Insertar(Usuario usuario, AccesoDatos accesoDatos = null)
        {
            return InsertarYObtenerId(usuario, accesoDatos) > 0;
        }

        public List<Usuario> ObtenerTodos()
        {
            const string query = @"
SELECT Id, Username, PasswordHash, Activo
FROM Usuarios
ORDER BY Activo DESC, Username";

            using (AccesoDatos accesoDatos = new AccesoDatos())
            {
                DataTable dataTable = accesoDatos.Read(query);
                List<Usuario> usuarios = new List<Usuario>();

                foreach (DataRow row in dataTable.Rows)
                {
                    usuarios.Add(Map(row));
                }

                return usuarios;
            }
        }

        public bool Actualizar(Usuario usuario, AccesoDatos accesoDatos = null)
        {
            const string query = @"
UPDATE Usuarios
SET Username = @Username
WHERE Id = @Id";

            bool dispose = accesoDatos == null;
            accesoDatos = accesoDatos ?? new AccesoDatos();

            try
            {
                List<SqlParameter> parametros = new List<SqlParameter>
                {
                    accesoDatos.CreateParameter("@Username", SqlDbType.VarChar, usuario.Username),
                    accesoDatos.CreateParameter("@Id", SqlDbType.Int, usuario.Id)
                };

                return accesoDatos.Write(query, parametros) > 0;
            }
            finally
            {
                if (dispose)
                {
                    accesoDatos.Dispose();
                }
            }
        }

        public bool GuardarEstadoCompleto(Usuario usuario, AccesoDatos accesoDatos = null)
        {
            const string query = @"
UPDATE Usuarios
SET Username = @Username,
    PasswordHash = @PasswordHash,
    Activo = @Activo
WHERE Id = @Id";

            bool dispose = accesoDatos == null;
            accesoDatos = accesoDatos ?? new AccesoDatos();

            try
            {
                List<SqlParameter> parametros = new List<SqlParameter>
                {
                    accesoDatos.CreateParameter("@Id", SqlDbType.Int, usuario.Id),
                    accesoDatos.CreateParameter("@Username", SqlDbType.VarChar, usuario.Username),
                    accesoDatos.CreateParameter("@PasswordHash", SqlDbType.VarChar, usuario.PasswordHash),
                    accesoDatos.CreateParameter("@Activo", SqlDbType.Bit, usuario.Activo)
                };

                return accesoDatos.Write(query, parametros) > 0;
            }
            finally
            {
                if (dispose)
                {
                    accesoDatos.Dispose();
                }
            }
        }

        public bool Desactivar(int id, AccesoDatos accesoDatos = null)
        {
            const string query = @"
UPDATE Usuarios
SET Activo = 0
WHERE Id = @Id
  AND Activo = 1";

            bool dispose = accesoDatos == null;
            accesoDatos = accesoDatos ?? new AccesoDatos();

            try
            {
                List<SqlParameter> parametros = new List<SqlParameter>
                {
                    accesoDatos.CreateParameter("@Id", SqlDbType.Int, id)
                };

                return accesoDatos.Write(query, parametros) > 0;
            }
            finally
            {
                if (dispose)
                {
                    accesoDatos.Dispose();
                }
            }
        }
    }
}
