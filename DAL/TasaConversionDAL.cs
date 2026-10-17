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
                FechaActualizacion = (System.DateTime)row["FechaActualizacion"],
                EsManual = (bool)row["EsManual"],
                ProtegidaPorUsuarioId = row["ProtegidaPorUsuarioId"] == System.DBNull.Value ? (int?)null : (int)row["ProtegidaPorUsuarioId"],
                FechaProteccion = row["FechaProteccion"] == System.DBNull.Value ? (System.DateTime?)null : (System.DateTime)row["FechaProteccion"]
            };
        }

        public TasaConversion ObtenerPorId(int id, AccesoDatosSqlServer accesoDatos = null)
        {
            const string query = @"
                SELECT Id, MonedaBaseId, MonedaConversionId, CalculoConversion, FechaActualizacion,
                       EsManual, ProtegidaPorUsuarioId, FechaProteccion
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
                SELECT Id, MonedaBaseId, MonedaConversionId, CalculoConversion, FechaActualizacion,
                       EsManual, ProtegidaPorUsuarioId, FechaProteccion
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
                SELECT Id, MonedaBaseId, MonedaConversionId, CalculoConversion, FechaActualizacion,
                       EsManual, ProtegidaPorUsuarioId, FechaProteccion
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

        // Escritura de la actualizacion automatica: no toca EsManual y, por defensa ante una
        // carrera con la edicion del operador, no modifica una tasa que ya quedo manual.
        public bool ActualizarValorAutomatico(int id, decimal calculoConversion, AccesoDatosSqlServer accesoDatos = null)
        {
            const string query = @"
                UPDATE TasaConversion
                SET CalculoConversion = @CalculoConversion,
                    FechaActualizacion = GETDATE()
                WHERE Id = @Id AND EsManual = 0";

            bool dispose = accesoDatos == null;
            accesoDatos = accesoDatos ?? new AccesoDatosSqlServer();

            try
            {
                List<SqlParameter> parametros = new List<SqlParameter>
                {
                    accesoDatos.CreateParameter("@CalculoConversion", SqlDbType.Decimal, calculoConversion),
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

        public bool MarcarManual(int id, int usuarioId, AccesoDatosSqlServer accesoDatos = null)
        {
            const string query = @"
                UPDATE TasaConversion
                SET EsManual = 1,
                    ProtegidaPorUsuarioId = @UsuarioId,
                    FechaProteccion = GETDATE()
                WHERE Id = @Id";

            bool dispose = accesoDatos == null;
            accesoDatos = accesoDatos ?? new AccesoDatosSqlServer();

            try
            {
                List<SqlParameter> parametros = new List<SqlParameter>
                {
                    accesoDatos.CreateParameter("@UsuarioId", SqlDbType.Int, usuarioId),
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

        public bool Liberar(int id, AccesoDatosSqlServer accesoDatos = null)
        {
            const string query = @"
                UPDATE TasaConversion
                SET EsManual = 0,
                    ProtegidaPorUsuarioId = NULL,
                    FechaProteccion = NULL
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
