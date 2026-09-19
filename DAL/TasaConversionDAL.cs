using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using BE;

namespace DAL
{
    public class TasaConversionDAL
    {
        private TasaConversion Map(DataRow row)
        {
            return new TasaConversion
            {
                Id = (int)row["Id"],
                MonedaBase = new Moneda { Id = (int)row["MonedaBaseId"] },
                MonedaConversion = new Moneda { Id = (int)row["MonedaConversionId"] },
                CalculoConversion = (decimal)row["CalculoConversion"],
                FechaActualizacion = (System.DateTime)row["FechaActualizacion"]
            };
        }

        public TasaConversion ObtenerPorId(int id, AccesoDatosSqlServer accesoDatos = null)
        {
            const string query = @"
                SELECT Id, MonedaBaseId, MonedaConversionId, CalculoConversion, FechaActualizacion
                FROM TasaConversion
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

        public List<TasaConversion> ObtenerTodas(AccesoDatosSqlServer accesoDatos = null)
        {
            const string query = @"
                SELECT Id, MonedaBaseId, MonedaConversionId, CalculoConversion, FechaActualizacion
                FROM TasaConversion
                ORDER BY FechaActualizacion DESC";

            bool dispose = accesoDatos == null;
            accesoDatos = accesoDatos ?? new AccesoDatosSqlServer();

            try
            {
                DataTable dataTable = accesoDatos.Read(query);
                List<TasaConversion> tasas = new List<TasaConversion>();
                foreach (DataRow row in dataTable.Rows)
                {
                    tasas.Add(Map(row));
                }

                return tasas;
            }
            finally
            {
                if (dispose)
                {
                    accesoDatos.Dispose();
                }
            }
        }

        public TasaConversion ObtenerPorParDeMonedas(int monedaBaseId, int monedaConversionId, AccesoDatosSqlServer accesoDatos = null)
        {
            const string query = @"
                SELECT Id, MonedaBaseId, MonedaConversionId, CalculoConversion, FechaActualizacion
                FROM TasaConversion
                WHERE MonedaBaseId = @MonedaBaseId AND MonedaConversionId = @MonedaConversionId";

            bool dispose = accesoDatos == null;
            accesoDatos = accesoDatos ?? new AccesoDatosSqlServer();

            try
            {
                List<SqlParameter> parametros = new List<SqlParameter>
                {
                    accesoDatos.CreateParameter("@MonedaBaseId", SqlDbType.Int, monedaBaseId),
                    accesoDatos.CreateParameter("@MonedaConversionId", SqlDbType.Int, monedaConversionId)
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

        public int InsertarYObtenerId(TasaConversion tasa, AccesoDatosSqlServer accesoDatos = null)
        {
            const string query = @"
                INSERT INTO TasaConversion (MonedaBaseId, MonedaConversionId, CalculoConversion)
                OUTPUT INSERTED.Id
                VALUES (@MonedaBaseId, @MonedaConversionId, @CalculoConversion)";

            bool dispose = accesoDatos == null;
            accesoDatos = accesoDatos ?? new AccesoDatosSqlServer();

            try
            {
                List<SqlParameter> parametros = new List<SqlParameter>
                {
                    accesoDatos.CreateParameter("@MonedaBaseId", SqlDbType.Int, tasa.MonedaBase.Id),
                    accesoDatos.CreateParameter("@MonedaConversionId", SqlDbType.Int, tasa.MonedaConversion.Id),
                    accesoDatos.CreateParameter("@CalculoConversion", SqlDbType.Decimal, tasa.CalculoConversion)
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

        public bool Actualizar(TasaConversion tasa, AccesoDatosSqlServer accesoDatos = null)
        {
            const string query = @"
                UPDATE TasaConversion
                SET CalculoConversion = @CalculoConversion,
                    FechaActualizacion = GETDATE()
                WHERE Id = @Id";

            bool dispose = accesoDatos == null;
            accesoDatos = accesoDatos ?? new AccesoDatosSqlServer();

            try
            {
                List<SqlParameter> parametros = new List<SqlParameter>
                {
                    accesoDatos.CreateParameter("@CalculoConversion", SqlDbType.Decimal, tasa.CalculoConversion),
                    accesoDatos.CreateParameter("@Id", SqlDbType.Int, tasa.Id)
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
                DELETE FROM TasaConversion
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
