using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using BE;

namespace DAL
{
    public class ReglaCategorizacionDAL
    {
        private ReglaCategorizacion Map(DataRow row)
        {
            return new ReglaCategorizacion
            {
                Id = (int)row["Id"],
                Usuario = new Usuario { Id = (int)row["UsuarioId"] },
                Categoria = new Categoria { Id = (int)row["CategoriaId"] },
                Nombre = row["Nombre"].ToString(),
                PalabrasClave = row.IsNull("PalabrasClave") ? null : row["PalabrasClave"].ToString()
            };
        }

        public ReglaCategorizacion ObtenerPorId(int id, AccesoDatosSqlServer accesoDatos = null)
        {
            const string query = @"
                SELECT Id, UsuarioId, CategoriaId, Nombre, PalabrasClave
                FROM ReglaCategorizacion
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

        public List<ReglaCategorizacion> ObtenerPorUsuario(int usuarioId, AccesoDatosSqlServer accesoDatos = null)
        {
            const string query = @"
                SELECT Id, UsuarioId, CategoriaId, Nombre, PalabrasClave
                FROM ReglaCategorizacion
                WHERE UsuarioId = @UsuarioId
                ORDER BY Nombre";

            bool dispose = accesoDatos == null;
            accesoDatos = accesoDatos ?? new AccesoDatosSqlServer();

            try
            {
                List<SqlParameter> parametros = new List<SqlParameter>
                {
                    accesoDatos.CreateParameter("@UsuarioId", SqlDbType.Int, usuarioId)
                };

                DataTable dataTable = accesoDatos.Read(query, parametros);
                List<ReglaCategorizacion> reglas = new List<ReglaCategorizacion>();
                foreach (DataRow row in dataTable.Rows)
                {
                    reglas.Add(Map(row));
                }

                return reglas;
            }
            finally
            {
                if (dispose)
                {
                    accesoDatos.Dispose();
                }
            }
        }

        public int InsertarYObtenerId(ReglaCategorizacion reglaCategorizacion, AccesoDatosSqlServer accesoDatos = null)
        {
            const string query = @"
                INSERT INTO ReglaCategorizacion (UsuarioId, CategoriaId, Nombre, PalabrasClave)
                OUTPUT INSERTED.Id
                VALUES (@UsuarioId, @CategoriaId, @Nombre, @PalabrasClave)";

            bool dispose = accesoDatos == null;
            accesoDatos = accesoDatos ?? new AccesoDatosSqlServer();

            try
            {
                List<SqlParameter> parametros = new List<SqlParameter>
                {
                    accesoDatos.CreateParameter("@UsuarioId", SqlDbType.Int, reglaCategorizacion.Usuario.Id),
                    accesoDatos.CreateParameter("@CategoriaId", SqlDbType.Int, reglaCategorizacion.Categoria.Id),
                    accesoDatos.CreateParameter("@Nombre", SqlDbType.VarChar, reglaCategorizacion.Nombre),
                    accesoDatos.CreateParameter("@PalabrasClave", SqlDbType.VarChar, reglaCategorizacion.PalabrasClave)
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

        public bool Actualizar(ReglaCategorizacion reglaCategorizacion, AccesoDatosSqlServer accesoDatos = null)
        {
            const string query = @"
                UPDATE ReglaCategorizacion
                SET CategoriaId = @CategoriaId,
                    Nombre = @Nombre,
                    PalabrasClave = @PalabrasClave
                WHERE Id = @Id";

            bool dispose = accesoDatos == null;
            accesoDatos = accesoDatos ?? new AccesoDatosSqlServer();

            try
            {
                List<SqlParameter> parametros = new List<SqlParameter>
                {
                    accesoDatos.CreateParameter("@CategoriaId", SqlDbType.Int, reglaCategorizacion.Categoria.Id),
                    accesoDatos.CreateParameter("@Nombre", SqlDbType.VarChar, reglaCategorizacion.Nombre),
                    accesoDatos.CreateParameter("@PalabrasClave", SqlDbType.VarChar, reglaCategorizacion.PalabrasClave),
                    accesoDatos.CreateParameter("@Id", SqlDbType.Int, reglaCategorizacion.Id)
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
                DELETE FROM ReglaCategorizacion
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
    }
}
