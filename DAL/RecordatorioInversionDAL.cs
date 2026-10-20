using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using BE;

namespace DAL
{
    public class RecordatorioInversionDAL
    {
        private RecordatorioInversion Map(DataRow row)
        {
            var recordatorio = new RecordatorioInversion
            {
                Id = (int)row["Id"],
                Usuario = new Usuario { Id = (int)row["UsuarioId"] },
                Mensaje = row["Mensaje"].ToString(),
                Fecha = (System.DateTime)row["Fecha"],
                Leida = (bool)row["Leida"]
            };
            MetadataNotificacionDAL.Leer(row, recordatorio);
            return recordatorio;
        }

        public RecordatorioInversion ObtenerPorId(int id, AccesoDatosSqlServer accesoDatos = null)
        {
            const string query = @"
                SELECT Id, UsuarioId, Mensaje, Fecha, Leida, " + MetadataNotificacionDAL.Columnas + @"
                FROM Notificacion
                WHERE Id = @Id AND TipoNotificacion = 'RecordatorioInversion'";

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

        public List<RecordatorioInversion> ObtenerPorUsuario(int usuarioId, AccesoDatosSqlServer accesoDatos = null)
        {
            const string query = @"
                SELECT Id, UsuarioId, Mensaje, Fecha, Leida, " + MetadataNotificacionDAL.Columnas + @"
                FROM Notificacion
                WHERE UsuarioId = @UsuarioId AND TipoNotificacion = 'RecordatorioInversion'
                ORDER BY Fecha DESC";

            bool dispose = accesoDatos == null;
            accesoDatos = accesoDatos ?? new AccesoDatosSqlServer();

            try
            {
                List<SqlParameter> parametros = new List<SqlParameter>
                {
                    accesoDatos.CreateParameter("@UsuarioId", SqlDbType.Int, usuarioId)
                };

                DataTable dataTable = accesoDatos.Read(query, parametros);
                List<RecordatorioInversion> recordatorios = new List<RecordatorioInversion>();
                foreach (DataRow row in dataTable.Rows)
                {
                    recordatorios.Add(Map(row));
                }

                return recordatorios;
            }
            finally
            {
                if (dispose)
                {
                    accesoDatos.Dispose();
                }
            }
        }

        public List<RecordatorioInversion> ObtenerTodas(AccesoDatosSqlServer accesoDatos = null)
        {
            const string query = @"
                SELECT Id, UsuarioId, Mensaje, Fecha, Leida, " + MetadataNotificacionDAL.Columnas + @"
                FROM Notificacion
                WHERE TipoNotificacion = 'RecordatorioInversion'
                ORDER BY Fecha DESC";

            bool dispose = accesoDatos == null;
            accesoDatos = accesoDatos ?? new AccesoDatosSqlServer();

            try
            {
                DataTable dataTable = accesoDatos.Read(query);
                List<RecordatorioInversion> recordatorios = new List<RecordatorioInversion>();
                foreach (DataRow row in dataTable.Rows)
                {
                    recordatorios.Add(Map(row));
                }

                return recordatorios;
            }
            finally
            {
                if (dispose)
                {
                    accesoDatos.Dispose();
                }
            }
        }

        public int InsertarYObtenerId(RecordatorioInversion recordatorio, AccesoDatosSqlServer accesoDatos = null)
        {
            const string query = @"
                INSERT INTO Notificacion (UsuarioId, TipoNotificacion, Mensaje, Fecha, Leida, TipoAutomatico, FechaJornada, ClaveEjemplo)
                OUTPUT INSERTED.Id
                VALUES (@UsuarioId, 'RecordatorioInversion', @Mensaje, @Fecha, @Leida, @TipoAutomatico, @FechaJornada, @ClaveEjemplo)";

            bool dispose = accesoDatos == null;
            accesoDatos = accesoDatos ?? new AccesoDatosSqlServer();

            try
            {
                List<SqlParameter> parametros = new List<SqlParameter>
                {
                    accesoDatos.CreateParameter("@UsuarioId", SqlDbType.Int, recordatorio.Usuario.Id),
                    accesoDatos.CreateParameter("@Mensaje", SqlDbType.NVarChar, recordatorio.Mensaje),
                    accesoDatos.CreateParameter("@Fecha", SqlDbType.DateTime, recordatorio.Fecha),
                    accesoDatos.CreateParameter("@Leida", SqlDbType.Bit, recordatorio.Leida)
                };
                MetadataNotificacionDAL.AgregarParametros(accesoDatos, parametros, recordatorio);

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

        public bool Actualizar(RecordatorioInversion recordatorio, AccesoDatosSqlServer accesoDatos = null)
        {
            const string query = @"
                UPDATE Notificacion
                SET Mensaje = @Mensaje,
                    Fecha = @Fecha,
                    Leida = @Leida
                WHERE Id = @Id AND TipoNotificacion = 'RecordatorioInversion'";

            bool dispose = accesoDatos == null;
            accesoDatos = accesoDatos ?? new AccesoDatosSqlServer();

            try
            {
                List<SqlParameter> parametros = new List<SqlParameter>
                {
                    accesoDatos.CreateParameter("@Mensaje", SqlDbType.NVarChar, recordatorio.Mensaje),
                    accesoDatos.CreateParameter("@Fecha", SqlDbType.DateTime, recordatorio.Fecha),
                    accesoDatos.CreateParameter("@Leida", SqlDbType.Bit, recordatorio.Leida),
                    accesoDatos.CreateParameter("@Id", SqlDbType.Int, recordatorio.Id)
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
                DELETE FROM Notificacion
                WHERE Id = @Id AND TipoNotificacion = 'RecordatorioInversion'";

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
