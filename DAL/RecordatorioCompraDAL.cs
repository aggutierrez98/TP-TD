using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using BE;

namespace DAL
{
    public class RecordatorioCompraDAL
    {
        private RecordatorioCompra Map(DataRow row)
        {
            var recordatorio = new RecordatorioCompra
            {
                Id = (int)row["Id"],
                Usuario = new Usuario { Id = (int)row["UsuarioId"] },
                Mensaje = row["Mensaje"].ToString(),
                Fecha = (System.DateTime)row["Fecha"],
                Leida = (bool)row["Leida"],
                // Nulos solo en la recomendacion de compras agregada (automatica o ejemplo).
                ItemLista = row.IsNull("ItemListaId") ? null : new ItemLista { Id = (int)row["ItemListaId"] },
                Promocion = row.IsNull("PromocionId") ? null : new Promocion { Id = (int)row["PromocionId"] },
                FechaVencimiento = row.IsNull("FechaVencimiento") ? (System.DateTime?)null : (System.DateTime)row["FechaVencimiento"]
            };
            MetadataNotificacionDAL.Leer(row, recordatorio);
            return recordatorio;
        }

        public RecordatorioCompra ObtenerPorId(int id, AccesoDatosSqlServer accesoDatos = null)
        {
            const string query = @"
                SELECT Id, UsuarioId, Mensaje, Fecha, Leida, ItemListaId, PromocionId, FechaVencimiento, " + MetadataNotificacionDAL.Columnas + @"
                FROM Notificacion
                WHERE Id = @Id AND TipoNotificacion = 'RecordatorioCompra'";

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

        public List<RecordatorioCompra> ObtenerPorUsuario(Usuario usuario, AccesoDatosSqlServer accesoDatos = null)
        {
            const string query = @"
                SELECT Id, UsuarioId, Mensaje, Fecha, Leida, ItemListaId, PromocionId, FechaVencimiento, " + MetadataNotificacionDAL.Columnas + @"
                FROM Notificacion
                WHERE UsuarioId = @UsuarioId AND TipoNotificacion = 'RecordatorioCompra'
                ORDER BY Fecha DESC";

            bool dispose = accesoDatos == null;
            accesoDatos = accesoDatos ?? new AccesoDatosSqlServer();

            try
            {
                List<SqlParameter> parametros = new List<SqlParameter>
                {
                    accesoDatos.CreateParameter("@UsuarioId", SqlDbType.Int, usuario.Id)
                };

                DataTable dataTable = accesoDatos.Read(query, parametros);
                List<RecordatorioCompra> recordatorios = new List<RecordatorioCompra>();
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

        public List<RecordatorioCompra> ObtenerTodas(AccesoDatosSqlServer accesoDatos = null)
        {
            const string query = @"
                SELECT Id, UsuarioId, Mensaje, Fecha, Leida, ItemListaId, PromocionId, FechaVencimiento, " + MetadataNotificacionDAL.Columnas + @"
                FROM Notificacion
                WHERE TipoNotificacion = 'RecordatorioCompra'
                ORDER BY Fecha DESC";

            bool dispose = accesoDatos == null;
            accesoDatos = accesoDatos ?? new AccesoDatosSqlServer();

            try
            {
                DataTable dataTable = accesoDatos.Read(query);
                List<RecordatorioCompra> recordatorios = new List<RecordatorioCompra>();
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

        public int InsertarYObtenerId(RecordatorioCompra recordatorio, AccesoDatosSqlServer accesoDatos = null)
        {
            const string query = @"
                INSERT INTO Notificacion (UsuarioId, TipoNotificacion, Mensaje, Fecha, Leida, ItemListaId, PromocionId, FechaVencimiento, TipoAutomatico, FechaJornada, ClaveEjemplo)
                OUTPUT INSERTED.Id
                VALUES (@UsuarioId, 'RecordatorioCompra', @Mensaje, @Fecha, @Leida, @ItemListaId, @PromocionId, @FechaVencimiento, @TipoAutomatico, @FechaJornada, @ClaveEjemplo)";

            bool dispose = accesoDatos == null;
            accesoDatos = accesoDatos ?? new AccesoDatosSqlServer();

            try
            {
                List<SqlParameter> parametros = new List<SqlParameter>
                {
                    accesoDatos.CreateParameter("@UsuarioId", SqlDbType.Int, recordatorio.Usuario.Id),
                    accesoDatos.CreateParameter("@Mensaje", SqlDbType.NVarChar, recordatorio.Mensaje),
                    accesoDatos.CreateParameter("@Fecha", SqlDbType.DateTime, recordatorio.Fecha),
                    accesoDatos.CreateParameter("@Leida", SqlDbType.Bit, recordatorio.Leida),
                    accesoDatos.CreateParameter("@ItemListaId", SqlDbType.Int, recordatorio.ItemLista != null ? (object)recordatorio.ItemLista.Id : null),
                    accesoDatos.CreateParameter("@PromocionId", SqlDbType.Int, recordatorio.Promocion != null ? (object)recordatorio.Promocion.Id : null),
                    accesoDatos.CreateParameter("@FechaVencimiento", SqlDbType.Date, recordatorio.FechaVencimiento)
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

        public bool Actualizar(RecordatorioCompra recordatorio, AccesoDatosSqlServer accesoDatos = null)
        {
            const string query = @"
                UPDATE Notificacion
                SET Mensaje = @Mensaje,
                    Fecha = @Fecha,
                    Leida = @Leida,
                    PromocionId = @PromocionId,
                    FechaVencimiento = @FechaVencimiento
                WHERE Id = @Id AND TipoNotificacion = 'RecordatorioCompra'";

            bool dispose = accesoDatos == null;
            accesoDatos = accesoDatos ?? new AccesoDatosSqlServer();

            try
            {
                List<SqlParameter> parametros = new List<SqlParameter>
                {
                    accesoDatos.CreateParameter("@Mensaje", SqlDbType.NVarChar, recordatorio.Mensaje),
                    accesoDatos.CreateParameter("@Fecha", SqlDbType.DateTime, recordatorio.Fecha),
                    accesoDatos.CreateParameter("@Leida", SqlDbType.Bit, recordatorio.Leida),
                    accesoDatos.CreateParameter("@PromocionId", SqlDbType.Int, recordatorio.Promocion != null ? (object)recordatorio.Promocion.Id : null),
                    accesoDatos.CreateParameter("@FechaVencimiento", SqlDbType.Date, recordatorio.FechaVencimiento),
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

        public bool Eliminar(RecordatorioCompra recordatorio, AccesoDatosSqlServer accesoDatos = null)
        {
            const string query = @"
                DELETE FROM Notificacion
                WHERE Id = @Id AND TipoNotificacion = 'RecordatorioCompra'";

            bool dispose = accesoDatos == null;
            accesoDatos = accesoDatos ?? new AccesoDatosSqlServer();

            try
            {
                List<SqlParameter> parametros = new List<SqlParameter>
                {
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
    }
}
