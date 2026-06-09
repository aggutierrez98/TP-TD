using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using SERVICIOS;

namespace DAL
{
    public class UsuarioHistorialDAL
    {
        public int GuardarVersion(UsuarioHistorialSnapshot snapshot, AccesoDatos accesoDatos = null)
        {
            const string insertHistorial = @"
                INSERT INTO UsuarioHistorial
                (
                    UsuarioId,
                    FechaHora,
                    ActorUsuarioId,
                    ActorUsernameSnapshot,
                    Accion,
                    Observaciones,
                    Username,
                    PasswordHash,
                    Activo,
                    IdiomaId
                )
                OUTPUT INSERTED.HistorialId
                VALUES
                (
                    @UsuarioId,
                    @FechaHora,
                    @ActorUsuarioId,
                    @ActorUsernameSnapshot,
                    @Accion,
                    @Observaciones,
                    @Username,
                    @PasswordHash,
                    @Activo,
                    @IdiomaId
                )";

            bool dispose = accesoDatos == null;
            accesoDatos = accesoDatos ?? new AccesoDatos();

            try
            {
                List<SqlParameter> parametros = new List<SqlParameter>
                {
                    accesoDatos.CreateParameter("@UsuarioId", SqlDbType.Int, snapshot.UsuarioId),
                    accesoDatos.CreateParameter("@FechaHora", SqlDbType.DateTime, snapshot.FechaHora),
                    accesoDatos.CreateParameter("@ActorUsuarioId", SqlDbType.Int, snapshot.ActorUsuarioId),
                    accesoDatos.CreateParameter("@ActorUsernameSnapshot", SqlDbType.VarChar, snapshot.ActorUsernameSnapshot),
                    accesoDatos.CreateParameter("@Accion", SqlDbType.VarChar, snapshot.Accion),
                    accesoDatos.CreateParameter("@Observaciones", SqlDbType.VarChar, snapshot.Observaciones),
                    accesoDatos.CreateParameter("@Username", SqlDbType.VarChar, snapshot.Username),
                    accesoDatos.CreateParameter("@PasswordHash", SqlDbType.VarChar, snapshot.PasswordHash),
                    accesoDatos.CreateParameter("@Activo", SqlDbType.Bit, snapshot.Activo),
                    accesoDatos.CreateParameter("@IdiomaId", SqlDbType.Int, snapshot.IdiomaId)
                };

                int historialId = accesoDatos.WriteWithResult(insertHistorial, parametros);

                List<int> permisos = snapshot.PermisosIds == null
                    ? new List<int>()
                    : snapshot.PermisosIds.Distinct().OrderBy(item => item).ToList();

                if (permisos.Count > 0)
                {
                    List<SqlParameter> parametrosPermisos = new List<SqlParameter>
                    {
                        accesoDatos.CreateParameter("@HistorialId", SqlDbType.Int, historialId)
                    };

                    string queryPermisos = string.Empty;
                    for (int i = 0; i < permisos.Count; i++)
                    {
                        queryPermisos += string.Format(@"
                            INSERT INTO UsuarioHistorialPermiso (HistorialId, PermisoId)
                            VALUES (@HistorialId, @PermisoId{0});", i);
                        parametrosPermisos.Add(accesoDatos.CreateParameter("@PermisoId" + i, SqlDbType.Int, permisos[i]));
                    }

                    accesoDatos.Write(queryPermisos, parametrosPermisos);
                }

                return historialId;
            }
            finally
            {
                if (dispose)
                {
                    accesoDatos.Dispose();
                }
            }
        }

        public List<UsuarioHistorialSnapshot> ObtenerVersiones(int usuarioId)
        {
            const string query = @"
                SELECT HistorialId, UsuarioId, FechaHora, ActorUsuarioId, ActorUsernameSnapshot, Accion, Observaciones,
                       Username, PasswordHash, Activo, IdiomaId
                FROM UsuarioHistorial
                WHERE UsuarioId = @UsuarioId
                ORDER BY FechaHora DESC, HistorialId DESC";

            using (AccesoDatos accesoDatos = new AccesoDatos())
            {
                List<SqlParameter> parametros = new List<SqlParameter>
                {
                    accesoDatos.CreateParameter("@UsuarioId", SqlDbType.Int, usuarioId)
                };

                DataTable dataTable = accesoDatos.Read(query, parametros);
                return dataTable.Rows.Cast<DataRow>()
                    .Select(row => Map(row, accesoDatos))
                    .ToList();
            }
        }

        public UsuarioHistorialSnapshot ObtenerVersion(int historialId)
        {
            const string query = @"
                SELECT HistorialId, UsuarioId, FechaHora, ActorUsuarioId, ActorUsernameSnapshot, Accion, Observaciones,
                       Username, PasswordHash, Activo, IdiomaId
                FROM UsuarioHistorial
                WHERE HistorialId = @HistorialId";

            using (AccesoDatos accesoDatos = new AccesoDatos())
            {
                List<SqlParameter> parametros = new List<SqlParameter>
                {
                    accesoDatos.CreateParameter("@HistorialId", SqlDbType.Int, historialId)
                };

                DataTable dataTable = accesoDatos.Read(query, parametros);
                if (dataTable.Rows.Count == 0)
                {
                    return null;
                }

                return Map(dataTable.Rows[0], accesoDatos);
            }
        }

        private UsuarioHistorialSnapshot Map(DataRow row, AccesoDatos accesoDatos)
        {
            int historialId = (int)row["HistorialId"];
            return new UsuarioHistorialSnapshot
            {
                HistorialId = historialId,
                UsuarioId = (int)row["UsuarioId"],
                FechaHora = (DateTime)row["FechaHora"],
                ActorUsuarioId = row["ActorUsuarioId"] == DBNull.Value ? (int?)null : (int)row["ActorUsuarioId"],
                ActorUsernameSnapshot = row["ActorUsernameSnapshot"].ToString(),
                Accion = row["Accion"].ToString(),
                Observaciones = row["Observaciones"].ToString(),
                Username = row["Username"].ToString(),
                PasswordHash = row["PasswordHash"].ToString(),
                Activo = (bool)row["Activo"],
                IdiomaId = row["IdiomaId"] == DBNull.Value ? (int?)null : (int)row["IdiomaId"],
                PermisosIds = ObtenerPermisos(historialId, accesoDatos)
            };
        }

        private List<int> ObtenerPermisos(int historialId, AccesoDatos accesoDatos)
        {
            const string query = @"
                SELECT PermisoId
                FROM UsuarioHistorialPermiso
                WHERE HistorialId = @HistorialId
                ORDER BY PermisoId";

            List<SqlParameter> parametros = new List<SqlParameter>
            {
                accesoDatos.CreateParameter("@HistorialId", SqlDbType.Int, historialId)
            };

            DataTable dataTable = accesoDatos.Read(query, parametros);
            return dataTable.Rows.Cast<DataRow>()
                .Select(row => (int)row["PermisoId"])
                .ToList();
        }
    }
}
