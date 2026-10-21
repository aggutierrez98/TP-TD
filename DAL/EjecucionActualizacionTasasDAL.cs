using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using BE;

namespace DAL
{
    // Historial de la actualizacion automatica de tasas: una fila por ejecucion y una por par
    // evaluado. Los enums se guardan como texto (ToString) y se leen con Enum.Parse. El par
    // del detalle se devuelve solo con los ids de moneda; el repositorio los hidrata.
    public class EjecucionActualizacionTasasDAL
    {
        private const string ColumnasEjecucion = "Id, FechaInicio, Origen, Resultado, ConsumioRequest, Mensaje";

        private EjecucionActualizacionTasas Map(DataRow row)
        {
            return new EjecucionActualizacionTasas
            {
                Id = (int)row["Id"],
                FechaInicio = (DateTime)row["FechaInicio"],
                Origen = (OrigenActualizacionTasas)Enum.Parse(typeof(OrigenActualizacionTasas), (string)row["Origen"]),
                Resultado = (ResultadoActualizacionTasas)Enum.Parse(typeof(ResultadoActualizacionTasas), (string)row["Resultado"]),
                ConsumioRequest = (bool)row["ConsumioRequest"],
                Mensaje = row["Mensaje"] == DBNull.Value ? null : (string)row["Mensaje"]
            };
        }

        private ActualizacionTasaDetalle MapDetalle(DataRow row, EjecucionActualizacionTasas ejecucion)
        {
            return new ActualizacionTasaDetalle
            {
                Id = (int)row["Id"],
                Ejecucion = ejecucion,
                MonedaBase = new Moneda { Id = (int)row["MonedaBaseId"] },
                MonedaConversion = new Moneda { Id = (int)row["MonedaConversionId"] },
                ValorAnterior = (decimal)row["ValorAnterior"],
                ValorNuevo = row["ValorNuevo"] == DBNull.Value ? (decimal?)null : (decimal)row["ValorNuevo"],
                Estado = (EstadoActualizacionTasa)Enum.Parse(typeof(EstadoActualizacionTasa), (string)row["Estado"])
            };
        }

        public int InsertarYObtenerId(EjecucionActualizacionTasas ejecucion, AccesoDatosSqlServer accesoDatos = null)
        {
            const string query = @"
                INSERT INTO EjecucionActualizacionTasas (FechaInicio, Origen, Resultado, ConsumioRequest, Mensaje)
                OUTPUT INSERTED.Id
                VALUES (@FechaInicio, @Origen, @Resultado, @ConsumioRequest, @Mensaje)";

            bool dispose = accesoDatos == null;
            accesoDatos = accesoDatos ?? new AccesoDatosSqlServer();

            try
            {
                List<SqlParameter> parametros = new List<SqlParameter>
                {
                    accesoDatos.CreateParameter("@FechaInicio", SqlDbType.DateTime, ejecucion.FechaInicio),
                    accesoDatos.CreateParameter("@Origen", SqlDbType.VarChar, ejecucion.Origen.ToString()),
                    accesoDatos.CreateParameter("@Resultado", SqlDbType.VarChar, ejecucion.Resultado.ToString()),
                    accesoDatos.CreateParameter("@ConsumioRequest", SqlDbType.Bit, ejecucion.ConsumioRequest),
                    accesoDatos.CreateParameter("@Mensaje", SqlDbType.NVarChar, ejecucion.Mensaje)
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

        public int InsertarDetalleYObtenerId(ActualizacionTasaDetalle detalle, AccesoDatosSqlServer accesoDatos = null)
        {
            const string query = @"
                INSERT INTO ActualizacionTasaDetalle (EjecucionId, MonedaBaseId, MonedaConversionId, ValorAnterior, ValorNuevo, Estado)
                OUTPUT INSERTED.Id
                VALUES (@EjecucionId, @MonedaBaseId, @MonedaConversionId, @ValorAnterior, @ValorNuevo, @Estado)";

            bool dispose = accesoDatos == null;
            accesoDatos = accesoDatos ?? new AccesoDatosSqlServer();

            try
            {
                List<SqlParameter> parametros = new List<SqlParameter>
                {
                    accesoDatos.CreateParameter("@EjecucionId", SqlDbType.Int, detalle.Ejecucion.Id),
                    accesoDatos.CreateParameter("@MonedaBaseId", SqlDbType.Int, detalle.MonedaBase.Id),
                    accesoDatos.CreateParameter("@MonedaConversionId", SqlDbType.Int, detalle.MonedaConversion.Id),
                    accesoDatos.CreateParameter("@ValorAnterior", SqlDbType.Decimal, detalle.ValorAnterior),
                    accesoDatos.CreateParameter("@ValorNuevo", SqlDbType.Decimal, detalle.ValorNuevo),
                    accesoDatos.CreateParameter("@Estado", SqlDbType.VarChar, detalle.Estado.ToString())
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

        public List<EjecucionActualizacionTasas> ObtenerRecientes(int cantidad, AccesoDatosSqlServer accesoDatos = null)
        {
            const string query = @"
                SELECT TOP (@Cantidad) " + ColumnasEjecucion + @"
                FROM EjecucionActualizacionTasas
                ORDER BY FechaInicio DESC, Id DESC";

            bool dispose = accesoDatos == null;
            accesoDatos = accesoDatos ?? new AccesoDatosSqlServer();

            try
            {
                List<SqlParameter> parametros = new List<SqlParameter>
                {
                    accesoDatos.CreateParameter("@Cantidad", SqlDbType.Int, cantidad)
                };

                DataTable dataTable = accesoDatos.Read(query, parametros);
                List<EjecucionActualizacionTasas> ejecuciones = new List<EjecucionActualizacionTasas>();
                foreach (DataRow row in dataTable.Rows)
                {
                    ejecuciones.Add(Map(row));
                }

                return ejecuciones;
            }
            finally
            {
                if (dispose)
                {
                    accesoDatos.Dispose();
                }
            }
        }

        public List<ActualizacionTasaDetalle> ObtenerDetallePorEjecucion(EjecucionActualizacionTasas ejecucion, AccesoDatosSqlServer accesoDatos = null)
        {
            const string query = @"
                SELECT Id, EjecucionId, MonedaBaseId, MonedaConversionId, ValorAnterior, ValorNuevo, Estado
                FROM ActualizacionTasaDetalle
                WHERE EjecucionId = @EjecucionId
                ORDER BY Id";

            bool dispose = accesoDatos == null;
            accesoDatos = accesoDatos ?? new AccesoDatosSqlServer();

            try
            {
                List<SqlParameter> parametros = new List<SqlParameter>
                {
                    accesoDatos.CreateParameter("@EjecucionId", SqlDbType.Int, ejecucion.Id)
                };

                DataTable dataTable = accesoDatos.Read(query, parametros);
                List<ActualizacionTasaDetalle> detalles = new List<ActualizacionTasaDetalle>();
                foreach (DataRow row in dataTable.Rows)
                {
                    detalles.Add(MapDetalle(row, ejecucion));
                }

                return detalles;
            }
            finally
            {
                if (dispose)
                {
                    accesoDatos.Dispose();
                }
            }
        }

        // conBloqueo: UPDLOCK + HOLDLOCK retienen el rango leido hasta el fin de la transaccion,
        // de modo que dos procesos (el ejecutable y el boton) no verifiquen el tope a la vez.
        public EjecucionActualizacionTasas ObtenerUltimaExitosa(bool conBloqueo, AccesoDatosSqlServer accesoDatos = null)
        {
            string hint = conBloqueo ? " WITH (UPDLOCK, HOLDLOCK)" : string.Empty;
            string query = @"
                SELECT TOP 1 " + ColumnasEjecucion + @"
                FROM EjecucionActualizacionTasas" + hint + @"
                WHERE Resultado = 'Exitosa'
                ORDER BY FechaInicio DESC, Id DESC";

            bool dispose = accesoDatos == null;
            accesoDatos = accesoDatos ?? new AccesoDatosSqlServer();

            try
            {
                DataTable dataTable = accesoDatos.Read(query);
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

        // Mes calendario de la fecha de referencia: [primer dia, primer dia del mes siguiente).
        public int ContarRequestsDelMes(DateTime referencia, AccesoDatosSqlServer accesoDatos = null)
        {
            const string query = @"
                SELECT COUNT(*) AS Total
                FROM EjecucionActualizacionTasas
                WHERE ConsumioRequest = 1
                  AND FechaInicio >= @Desde
                  AND FechaInicio < @Hasta";

            DateTime desde = new DateTime(referencia.Year, referencia.Month, 1);
            DateTime hasta = desde.AddMonths(1);

            bool dispose = accesoDatos == null;
            accesoDatos = accesoDatos ?? new AccesoDatosSqlServer();

            try
            {
                List<SqlParameter> parametros = new List<SqlParameter>
                {
                    accesoDatos.CreateParameter("@Desde", SqlDbType.DateTime, desde),
                    accesoDatos.CreateParameter("@Hasta", SqlDbType.DateTime, hasta)
                };

                DataTable dataTable = accesoDatos.Read(query, parametros);
                return (int)dataTable.Rows[0]["Total"];
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
