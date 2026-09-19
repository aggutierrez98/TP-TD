using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using BE;

namespace DAL
{
    public class NotificacionDAL
    {
        private Notificacion Map(DataRow row)
        {
            return new Notificacion
            {
                Id = (int)row["Id"],
                Usuario = new Usuario { Id = (int)row["UsuarioId"] },
                Mensaje = row["Mensaje"].ToString(),
                Fecha = (System.DateTime)row["Fecha"],
                Leida = (bool)row["Leida"]
            };
        }

        public Notificacion ObtenerPorId(int id, AccesoDatosSqlServer accesoDatos = null)
        {
            const string query = @"
                SELECT Id, UsuarioId, Mensaje, Fecha, Leida
                FROM Notificacion
                WHERE Id = @Id AND TipoNotificacion = 'Notificacion'";

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

        public List<Notificacion> ObtenerPorUsuario(int usuarioId, AccesoDatosSqlServer accesoDatos = null)
        {
            const string query = @"
                SELECT Id, UsuarioId, Mensaje, Fecha, Leida
                FROM Notificacion
                WHERE UsuarioId = @UsuarioId AND TipoNotificacion = 'Notificacion'
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
                List<Notificacion> notificaciones = new List<Notificacion>();
                foreach (DataRow row in dataTable.Rows)
                {
                    notificaciones.Add(Map(row));
                }

                return notificaciones;
            }
            finally
            {
                if (dispose)
                {
                    accesoDatos.Dispose();
                }
            }
        }

        public List<Notificacion> ObtenerTodas(AccesoDatosSqlServer accesoDatos = null)
        {
            const string query = @"
                SELECT Id, UsuarioId, Mensaje, Fecha, Leida
                FROM Notificacion
                WHERE TipoNotificacion = 'Notificacion'
                ORDER BY Fecha DESC";

            bool dispose = accesoDatos == null;
            accesoDatos = accesoDatos ?? new AccesoDatosSqlServer();

            try
            {
                DataTable dataTable = accesoDatos.Read(query);
                List<Notificacion> notificaciones = new List<Notificacion>();
                foreach (DataRow row in dataTable.Rows)
                {
                    notificaciones.Add(Map(row));
                }

                return notificaciones;
            }
            finally
            {
                if (dispose)
                {
                    accesoDatos.Dispose();
                }
            }
        }

        public int InsertarYObtenerId(Notificacion notificacion, AccesoDatosSqlServer accesoDatos = null)
        {
            const string query = @"
                INSERT INTO Notificacion (UsuarioId, TipoNotificacion, Mensaje, Fecha, Leida)
                OUTPUT INSERTED.Id
                VALUES (@UsuarioId, 'Notificacion', @Mensaje, @Fecha, @Leida)";

            bool dispose = accesoDatos == null;
            accesoDatos = accesoDatos ?? new AccesoDatosSqlServer();

            try
            {
                List<SqlParameter> parametros = new List<SqlParameter>
                {
                    accesoDatos.CreateParameter("@UsuarioId", SqlDbType.Int, notificacion.Usuario.Id),
                    accesoDatos.CreateParameter("@Mensaje", SqlDbType.VarChar, notificacion.Mensaje),
                    accesoDatos.CreateParameter("@Fecha", SqlDbType.DateTime, notificacion.Fecha),
                    accesoDatos.CreateParameter("@Leida", SqlDbType.Bit, notificacion.Leida)
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

        public bool Actualizar(Notificacion notificacion, AccesoDatosSqlServer accesoDatos = null)
        {
            const string query = @"
                UPDATE Notificacion
                SET Mensaje = @Mensaje,
                    Fecha = @Fecha,
                    Leida = @Leida
                WHERE Id = @Id AND TipoNotificacion = 'Notificacion'";

            bool dispose = accesoDatos == null;
            accesoDatos = accesoDatos ?? new AccesoDatosSqlServer();

            try
            {
                List<SqlParameter> parametros = new List<SqlParameter>
                {
                    accesoDatos.CreateParameter("@Mensaje", SqlDbType.VarChar, notificacion.Mensaje),
                    accesoDatos.CreateParameter("@Fecha", SqlDbType.DateTime, notificacion.Fecha),
                    accesoDatos.CreateParameter("@Leida", SqlDbType.Bit, notificacion.Leida),
                    accesoDatos.CreateParameter("@Id", SqlDbType.Int, notificacion.Id)
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
                WHERE Id = @Id AND TipoNotificacion = 'Notificacion'";

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
