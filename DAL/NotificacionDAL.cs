using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using BE;

namespace DAL
{
    public class NotificacionDAL
    {
        public Notificacion ObtenerPersonal(int id, int usuarioId, AccesoDatosSqlServer accesoDatos = null)
        {
            bool dispose = accesoDatos == null;
            accesoDatos = accesoDatos ?? new AccesoDatosSqlServer();
            try
            {
                var tabla = accesoDatos.Read("SELECT * FROM Notificacion WHERE Id = @Id AND UsuarioId = @UsuarioId",
                    new List<SqlParameter> { accesoDatos.CreateParameter("@Id", SqlDbType.Int, id), accesoDatos.CreateParameter("@UsuarioId", SqlDbType.Int, usuarioId) });
                return tabla.Rows.Count == 0 ? null : MapPersonal(tabla.Rows[0]);
            }
            finally { if (dispose) accesoDatos.Dispose(); }
        }

        public bool MarcarPersonalLeida(int id, int usuarioId, AccesoDatosSqlServer accesoDatos = null)
        {
            bool dispose = accesoDatos == null;
            accesoDatos = accesoDatos ?? new AccesoDatosSqlServer();
            try
            {
                return accesoDatos.Write("UPDATE Notificacion SET Leida = 1 WHERE Id = @Id AND UsuarioId = @UsuarioId AND Leida = 0",
                    new List<SqlParameter> { accesoDatos.CreateParameter("@Id", SqlDbType.Int, id), accesoDatos.CreateParameter("@UsuarioId", SqlDbType.Int, usuarioId) }) > 0;
            }
            finally { if (dispose) accesoDatos.Dispose(); }
        }

        public PaginaNotificaciones ObtenerPaginaPersonal(int usuarioId, FiltroNotificacion filtro, int pagina, AccesoDatosSqlServer accesoDatos = null)
        {
            const string query = @"
                DECLARE @Ids TABLE (Id INT PRIMARY KEY, Fecha DATETIME);
                INSERT INTO @Ids SELECT Id, Fecha FROM Notificacion WITH (HOLDLOCK)
                WHERE UsuarioId = @UsuarioId
                  AND (@Desde IS NULL OR Fecha >= @Desde)
                  AND (@Hasta IS NULL OR Fecha < @Hasta);
                DECLARE @Total INT = (SELECT COUNT(*) FROM @Ids);
                DECLARE @Paginas INT = (@Total + 9) / 10;
                DECLARE @Actual INT = CASE WHEN @Paginas = 0 THEN 1 WHEN @Pagina > @Paginas THEN @Paginas ELSE @Pagina END;
                SELECT @Total AS Total, @Actual AS Pagina, n.*
                FROM (SELECT 1 AS Uno) s
                OUTER APPLY (SELECT n.* FROM @Ids i JOIN Notificacion n ON n.Id = i.Id
                    ORDER BY i.Fecha DESC, i.Id DESC OFFSET (@Actual - 1) * 10 ROWS FETCH NEXT 10 ROWS ONLY) n
                ORDER BY n.Fecha DESC, n.Id DESC;";
            bool dispose = accesoDatos == null;
            accesoDatos = accesoDatos ?? new AccesoDatosSqlServer();
            try
            {
                filtro = filtro ?? new FiltroNotificacion();
                System.DateTime? hasta = filtro.FechaHasta.HasValue && filtro.FechaHasta.Value.Date < System.DateTime.MaxValue.Date
                    ? filtro.FechaHasta.Value.Date.AddDays(1) : (System.DateTime?)null;
                var tabla = accesoDatos.Read(query, new List<SqlParameter> {
                    accesoDatos.CreateParameter("@UsuarioId", SqlDbType.Int, usuarioId),
                    accesoDatos.CreateParameter("@Desde", SqlDbType.DateTime, filtro.FechaDesde?.Date),
                    accesoDatos.CreateParameter("@Hasta", SqlDbType.DateTime, hasta),
                    accesoDatos.CreateParameter("@Pagina", SqlDbType.Int, pagina) });
                var resultado = new PaginaNotificaciones { Total = (int)tabla.Rows[0]["Total"], Pagina = (int)tabla.Rows[0]["Pagina"] };
                foreach (DataRow row in tabla.Rows)
                    if (row["Id"] != System.DBNull.Value) resultado.Elementos.Add(MapPersonal(row));
                return resultado;
            }
            finally { if (dispose) accesoDatos.Dispose(); }
        }

        public int ContarNoLeidas(int usuarioId, AccesoDatosSqlServer accesoDatos = null)
        {
            bool dispose = accesoDatos == null;
            accesoDatos = accesoDatos ?? new AccesoDatosSqlServer();
            try
            {
                return (int)accesoDatos.Read("SELECT COUNT(*) AS Total FROM Notificacion WHERE UsuarioId = @UsuarioId AND Leida = 0",
                    new List<SqlParameter> { accesoDatos.CreateParameter("@UsuarioId", SqlDbType.Int, usuarioId) }).Rows[0]["Total"];
            }
            finally { if (dispose) accesoDatos.Dispose(); }
        }

        private static Notificacion MapPersonal(DataRow row)
        {
            Notificacion resultado;
            switch (row["TipoNotificacion"].ToString())
            {
                case "RecordatorioCompra":
                    resultado = new RecordatorioCompra {
                        ItemLista = row["ItemListaId"] == System.DBNull.Value ? null : new ItemLista { Id = (int)row["ItemListaId"] },
                        Promocion = row["PromocionId"] == System.DBNull.Value ? null : new Promocion { Id = (int)row["PromocionId"] },
                        FechaVencimiento = (System.DateTime)row["FechaVencimiento"] };
                    break;
                case "RecordatorioInversion": resultado = new RecordatorioInversion(); break;
                default: resultado = new Notificacion(); break;
            }
            resultado.Id = (int)row["Id"];
            resultado.Usuario = new Usuario { Id = (int)row["UsuarioId"] };
            resultado.Mensaje = row["Mensaje"].ToString();
            resultado.Fecha = (System.DateTime)row["Fecha"];
            resultado.Leida = (bool)row["Leida"];
            return resultado;
        }

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

        // Cuenta cualquier fila de Notificacion (notificacion o recordatorio) que referencia a la promocion.
        public int ContarPorPromocion(int promocionId, AccesoDatosSqlServer accesoDatos = null)
        {
            const string query = @"
                SELECT COUNT(*) AS Total
                FROM Notificacion
                WHERE PromocionId = @PromocionId";

            bool dispose = accesoDatos == null;
            accesoDatos = accesoDatos ?? new AccesoDatosSqlServer();

            try
            {
                List<SqlParameter> parametros = new List<SqlParameter>
                {
                    accesoDatos.CreateParameter("@PromocionId", SqlDbType.Int, promocionId)
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
