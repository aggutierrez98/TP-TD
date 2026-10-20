using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using BE;

namespace DAL
{
    // Registro tecnico del proceso automatico de notificaciones: una fila por corrida y una por
    // usuario y tipo evaluado. Los enums se guardan como texto (ToString) y se leen con Enum.Parse.
    public class GeneracionNotificacionesDAL
    {
        private const string ColumnasCorrida = "Id, InicioUtc, FinUtc, Jornada, Entregadas, Omitidas, Fallidas";
        private const string ColumnasDetalle = "Id, CorridaId, UsuarioId, TipoAutomatico, Jornada, Resultado, Motivo, NotificacionId, FechaUtc";

        private static DateTime Utc(object valor)
        {
            return DateTime.SpecifyKind((DateTime)valor, DateTimeKind.Utc);
        }

        private CorridaGeneracionNotificaciones Map(DataRow row)
        {
            return new CorridaGeneracionNotificaciones
            {
                Id = (int)row["Id"],
                InicioUtc = Utc(row["InicioUtc"]),
                FinUtc = row.IsNull("FinUtc") ? (DateTime?)null : Utc(row["FinUtc"]),
                Jornada = (DateTime)row["Jornada"],
                Entregadas = (int)row["Entregadas"],
                Omitidas = (int)row["Omitidas"],
                Fallidas = (int)row["Fallidas"]
            };
        }

        private DetalleGeneracionNotificacion MapDetalle(DataRow row)
        {
            return new DetalleGeneracionNotificacion
            {
                Id = (int)row["Id"],
                CorridaId = (int)row["CorridaId"],
                UsuarioId = (int)row["UsuarioId"],
                Tipo = (TipoNotificacionAutomatica)Enum.Parse(typeof(TipoNotificacionAutomatica), (string)row["TipoAutomatico"]),
                Jornada = (DateTime)row["Jornada"],
                Resultado = (ResultadoGeneracionNotificacion)Enum.Parse(typeof(ResultadoGeneracionNotificacion), (string)row["Resultado"]),
                Motivo = row.IsNull("Motivo") ? null : (string)row["Motivo"],
                NotificacionId = row.IsNull("NotificacionId") ? (int?)null : (int)row["NotificacionId"],
                FechaUtc = Utc(row["FechaUtc"])
            };
        }

        public int InsertarCorridaYObtenerId(CorridaGeneracionNotificaciones corrida, AccesoDatosSqlServer accesoDatos = null)
        {
            const string query = @"
                INSERT INTO CorridaGeneracionNotificaciones (InicioUtc, Jornada)
                OUTPUT INSERTED.Id
                VALUES (@InicioUtc, @Jornada)";

            bool dispose = accesoDatos == null;
            accesoDatos = accesoDatos ?? new AccesoDatosSqlServer();

            try
            {
                return accesoDatos.WriteWithResult(query, new List<SqlParameter>
                {
                    accesoDatos.CreateParameter("@InicioUtc", SqlDbType.DateTime2, corrida.InicioUtc),
                    accesoDatos.CreateParameter("@Jornada", SqlDbType.Date, corrida.Jornada)
                });
            }
            finally
            {
                if (dispose)
                {
                    accesoDatos.Dispose();
                }
            }
        }

        public bool FinalizarCorrida(int corridaId, DateTime finUtc, int entregadas, int omitidas, int fallidas, AccesoDatosSqlServer accesoDatos = null)
        {
            const string query = @"
                UPDATE CorridaGeneracionNotificaciones
                SET FinUtc = @FinUtc, Entregadas = @Entregadas, Omitidas = @Omitidas, Fallidas = @Fallidas
                WHERE Id = @Id";

            bool dispose = accesoDatos == null;
            accesoDatos = accesoDatos ?? new AccesoDatosSqlServer();

            try
            {
                return accesoDatos.Write(query, new List<SqlParameter>
                {
                    accesoDatos.CreateParameter("@FinUtc", SqlDbType.DateTime2, finUtc),
                    accesoDatos.CreateParameter("@Entregadas", SqlDbType.Int, entregadas),
                    accesoDatos.CreateParameter("@Omitidas", SqlDbType.Int, omitidas),
                    accesoDatos.CreateParameter("@Fallidas", SqlDbType.Int, fallidas),
                    accesoDatos.CreateParameter("@Id", SqlDbType.Int, corridaId)
                }) > 0;
            }
            finally
            {
                if (dispose)
                {
                    accesoDatos.Dispose();
                }
            }
        }

        public int InsertarDetalleYObtenerId(DetalleGeneracionNotificacion detalle, AccesoDatosSqlServer accesoDatos = null)
        {
            const string query = @"
                INSERT INTO DetalleGeneracionNotificacion (CorridaId, UsuarioId, TipoAutomatico, Jornada, Resultado, Motivo, NotificacionId, FechaUtc)
                OUTPUT INSERTED.Id
                VALUES (@CorridaId, @UsuarioId, @TipoAutomatico, @Jornada, @Resultado, @Motivo, @NotificacionId, @FechaUtc)";

            bool dispose = accesoDatos == null;
            accesoDatos = accesoDatos ?? new AccesoDatosSqlServer();

            try
            {
                return accesoDatos.WriteWithResult(query, new List<SqlParameter>
                {
                    accesoDatos.CreateParameter("@CorridaId", SqlDbType.Int, detalle.CorridaId),
                    accesoDatos.CreateParameter("@UsuarioId", SqlDbType.Int, detalle.UsuarioId),
                    accesoDatos.CreateParameter("@TipoAutomatico", SqlDbType.VarChar, detalle.Tipo.ToString()),
                    accesoDatos.CreateParameter("@Jornada", SqlDbType.Date, detalle.Jornada),
                    accesoDatos.CreateParameter("@Resultado", SqlDbType.VarChar, detalle.Resultado.ToString()),
                    accesoDatos.CreateParameter("@Motivo", SqlDbType.VarChar, detalle.Motivo),
                    accesoDatos.CreateParameter("@NotificacionId", SqlDbType.Int, detalle.NotificacionId),
                    accesoDatos.CreateParameter("@FechaUtc", SqlDbType.DateTime2, detalle.FechaUtc)
                });
            }
            finally
            {
                if (dispose)
                {
                    accesoDatos.Dispose();
                }
            }
        }

        public CorridaGeneracionNotificaciones ObtenerCorrida(int id, AccesoDatosSqlServer accesoDatos = null)
        {
            const string query = @"
                SELECT " + ColumnasCorrida + @"
                FROM CorridaGeneracionNotificaciones
                WHERE Id = @Id";

            bool dispose = accesoDatos == null;
            accesoDatos = accesoDatos ?? new AccesoDatosSqlServer();

            try
            {
                DataTable dataTable = accesoDatos.Read(query, new List<SqlParameter>
                {
                    accesoDatos.CreateParameter("@Id", SqlDbType.Int, id)
                });
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

        public List<CorridaGeneracionNotificaciones> ObtenerRecientes(int cantidad, AccesoDatosSqlServer accesoDatos = null)
        {
            const string query = @"
                SELECT TOP (@Cantidad) " + ColumnasCorrida + @"
                FROM CorridaGeneracionNotificaciones
                ORDER BY InicioUtc DESC, Id DESC";

            bool dispose = accesoDatos == null;
            accesoDatos = accesoDatos ?? new AccesoDatosSqlServer();

            try
            {
                DataTable dataTable = accesoDatos.Read(query, new List<SqlParameter>
                {
                    accesoDatos.CreateParameter("@Cantidad", SqlDbType.Int, cantidad)
                });
                List<CorridaGeneracionNotificaciones> corridas = new List<CorridaGeneracionNotificaciones>();
                foreach (DataRow row in dataTable.Rows)
                {
                    corridas.Add(Map(row));
                }

                return corridas;
            }
            finally
            {
                if (dispose)
                {
                    accesoDatos.Dispose();
                }
            }
        }

        public List<DetalleGeneracionNotificacion> ObtenerDetallePorCorrida(int corridaId, AccesoDatosSqlServer accesoDatos = null)
        {
            const string query = @"
                SELECT " + ColumnasDetalle + @"
                FROM DetalleGeneracionNotificacion
                WHERE CorridaId = @CorridaId
                ORDER BY Id";

            bool dispose = accesoDatos == null;
            accesoDatos = accesoDatos ?? new AccesoDatosSqlServer();

            try
            {
                DataTable dataTable = accesoDatos.Read(query, new List<SqlParameter>
                {
                    accesoDatos.CreateParameter("@CorridaId", SqlDbType.Int, corridaId)
                });
                List<DetalleGeneracionNotificacion> detalles = new List<DetalleGeneracionNotificacion>();
                foreach (DataRow row in dataTable.Rows)
                {
                    detalles.Add(MapDetalle(row));
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
    }
}
