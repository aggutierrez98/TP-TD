using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using BE;

namespace DAL
{
    public class ComercioDAL
    {
        private Comercio Map(DataRow row)
        {
            return new Comercio
            {
                Id = (int)row["Id"],
                RubroComercio = new RubroComercio { Id = (int)row["RubroComercioId"] },
                Nombre = row["Nombre"].ToString(),
                Ubicacion = row.IsNull("Ubicacion") ? null : row["Ubicacion"].ToString()
            };
        }

        public Comercio ObtenerPorId(int id, AccesoDatosSqlServer accesoDatos = null)
        {
            const string query = @"
                SELECT Id, RubroComercioId, Nombre, Ubicacion
                FROM Comercio
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

        public List<Comercio> ObtenerPorRubro(int rubroComercioId, AccesoDatosSqlServer accesoDatos = null)
        {
            const string query = @"
                SELECT Id, RubroComercioId, Nombre, Ubicacion
                FROM Comercio
                WHERE RubroComercioId = @RubroComercioId
                ORDER BY Nombre";

            bool dispose = accesoDatos == null;
            accesoDatos = accesoDatos ?? new AccesoDatosSqlServer();

            try
            {
                List<SqlParameter> parametros = new List<SqlParameter>
                {
                    accesoDatos.CreateParameter("@RubroComercioId", SqlDbType.Int, rubroComercioId)
                };

                DataTable dataTable = accesoDatos.Read(query, parametros);
                List<Comercio> comercios = new List<Comercio>();
                foreach (DataRow row in dataTable.Rows)
                {
                    comercios.Add(Map(row));
                }

                return comercios;
            }
            finally
            {
                if (dispose)
                {
                    accesoDatos.Dispose();
                }
            }
        }

        public List<Comercio> ObtenerTodos(AccesoDatosSqlServer accesoDatos = null)
        {
            const string query = @"
                SELECT Id, RubroComercioId, Nombre, Ubicacion
                FROM Comercio
                ORDER BY Nombre";

            bool dispose = accesoDatos == null;
            accesoDatos = accesoDatos ?? new AccesoDatosSqlServer();

            try
            {
                DataTable dataTable = accesoDatos.Read(query);
                List<Comercio> comercios = new List<Comercio>();
                foreach (DataRow row in dataTable.Rows)
                {
                    comercios.Add(Map(row));
                }

                return comercios;
            }
            finally
            {
                if (dispose)
                {
                    accesoDatos.Dispose();
                }
            }
        }

        public int InsertarYObtenerId(Comercio comercio, AccesoDatosSqlServer accesoDatos = null)
        {
            const string query = @"
                INSERT INTO Comercio (RubroComercioId, Nombre, Ubicacion)
                OUTPUT INSERTED.Id
                VALUES (@RubroComercioId, @Nombre, @Ubicacion)";

            bool dispose = accesoDatos == null;
            accesoDatos = accesoDatos ?? new AccesoDatosSqlServer();

            try
            {
                List<SqlParameter> parametros = new List<SqlParameter>
                {
                    accesoDatos.CreateParameter("@RubroComercioId", SqlDbType.Int, comercio.RubroComercio.Id),
                    accesoDatos.CreateParameter("@Nombre", SqlDbType.VarChar, comercio.Nombre),
                    accesoDatos.CreateParameter("@Ubicacion", SqlDbType.VarChar, comercio.Ubicacion)
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

        public bool Actualizar(Comercio comercio, AccesoDatosSqlServer accesoDatos = null)
        {
            const string query = @"
                UPDATE Comercio
                SET RubroComercioId = @RubroComercioId,
                    Nombre = @Nombre,
                    Ubicacion = @Ubicacion
                WHERE Id = @Id";

            bool dispose = accesoDatos == null;
            accesoDatos = accesoDatos ?? new AccesoDatosSqlServer();

            try
            {
                List<SqlParameter> parametros = new List<SqlParameter>
                {
                    accesoDatos.CreateParameter("@RubroComercioId", SqlDbType.Int, comercio.RubroComercio.Id),
                    accesoDatos.CreateParameter("@Nombre", SqlDbType.VarChar, comercio.Nombre),
                    accesoDatos.CreateParameter("@Ubicacion", SqlDbType.VarChar, comercio.Ubicacion),
                    accesoDatos.CreateParameter("@Id", SqlDbType.Int, comercio.Id)
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
                DELETE FROM Comercio
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
