using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using BE;

namespace DAL
{
    public class CategoriaDAL
    {
        private Categoria Map(DataRow row)
        {
            return new Categoria
            {
                Id = (int)row["Id"],
                Nombre = row["Nombre"].ToString(),
                Usuario = row.IsNull("UsuarioId") ? null : new Usuario { Id = (int)row["UsuarioId"] }
            };
        }

        public Categoria ObtenerPorId(int id, AccesoDatosSqlServer accesoDatos = null)
        {
            const string query = @"
                SELECT Id, Nombre, UsuarioId
                FROM Categoria
                WHERE Id = @Id";

            bool dispose = accesoDatos == null;
            accesoDatos = accesoDatos ?? new AccesoDatosSqlServer();

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

        public List<Categoria> ObtenerVisiblesParaUsuario(int usuarioId, AccesoDatosSqlServer accesoDatos = null)
        {
            const string query = @"
                SELECT Id, Nombre, UsuarioId
                FROM Categoria
                WHERE UsuarioId IS NULL OR UsuarioId = @UsuarioId
                ORDER BY UsuarioId, Nombre";

            bool dispose = accesoDatos == null;
            accesoDatos = accesoDatos ?? new AccesoDatosSqlServer();

            try
            {
                List<SqlParameter> parametros = new List<SqlParameter>
                {
                    accesoDatos.CreateParameter("@UsuarioId", SqlDbType.Int, usuarioId)
                };

                DataTable dataTable = accesoDatos.Read(query, parametros);
                List<Categoria> categorias = new List<Categoria>();
                foreach (DataRow row in dataTable.Rows)
                {
                    categorias.Add(Map(row));
                }

                return categorias;
            }
            finally
            {
                if (dispose)
                {
                    accesoDatos.Dispose();
                }
            }
        }

        public int InsertarYObtenerId(Categoria categoria, AccesoDatosSqlServer accesoDatos = null)
        {
            const string query = @"
                INSERT INTO Categoria (Nombre, UsuarioId)
                OUTPUT INSERTED.Id
                VALUES (@Nombre, @UsuarioId)";

            bool dispose = accesoDatos == null;
            accesoDatos = accesoDatos ?? new AccesoDatosSqlServer();

            try
            {
                List<SqlParameter> parametros = new List<SqlParameter>
                {
                    accesoDatos.CreateParameter("@Nombre", SqlDbType.VarChar, categoria.Nombre),
                    accesoDatos.CreateParameter("@UsuarioId", SqlDbType.Int, categoria.Usuario?.Id)
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

        public bool Actualizar(Categoria categoria, AccesoDatosSqlServer accesoDatos = null)
        {
            const string query = @"
                UPDATE Categoria
                SET Nombre = @Nombre
                WHERE Id = @Id";

            bool dispose = accesoDatos == null;
            accesoDatos = accesoDatos ?? new AccesoDatosSqlServer();

            try
            {
                List<SqlParameter> parametros = new List<SqlParameter>
                {
                    accesoDatos.CreateParameter("@Nombre", SqlDbType.VarChar, categoria.Nombre),
                    accesoDatos.CreateParameter("@Id", SqlDbType.Int, categoria.Id)
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

        public bool Eliminar(int id, AccesoDatosSqlServer accesoDatos = null)
        {
            const string query = @"
                DELETE FROM Categoria
                WHERE Id = @Id";

            bool dispose = accesoDatos == null;
            accesoDatos = accesoDatos ?? new AccesoDatosSqlServer();

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

        public bool ExisteNombreEnScope(string nombre, int? usuarioId, int? idExcluido, AccesoDatosSqlServer accesoDatos = null)
        {
            string query = usuarioId.HasValue
                ? @"SELECT COUNT(1) FROM Categoria
                    WHERE UsuarioId = @UsuarioId AND Nombre = @Nombre AND (@IdExcluido IS NULL OR Id <> @IdExcluido)"
                : @"SELECT COUNT(1) FROM Categoria
                    WHERE UsuarioId IS NULL AND Nombre = @Nombre AND (@IdExcluido IS NULL OR Id <> @IdExcluido)";

            bool dispose = accesoDatos == null;
            accesoDatos = accesoDatos ?? new AccesoDatosSqlServer();

            try
            {
                List<SqlParameter> parametros = new List<SqlParameter>
                {
                    accesoDatos.CreateParameter("@Nombre", SqlDbType.VarChar, nombre),
                    accesoDatos.CreateParameter("@IdExcluido", SqlDbType.Int, idExcluido)
                };

                if (usuarioId.HasValue)
                {
                    parametros.Add(accesoDatos.CreateParameter("@UsuarioId", SqlDbType.Int, usuarioId));
                }

                DataTable dataTable = accesoDatos.Read(query, parametros);
                return (int)dataTable.Rows[0][0] > 0;
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
