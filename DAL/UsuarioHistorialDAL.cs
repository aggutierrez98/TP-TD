using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using SERVICIOS;
using ABSTRACCIONES;

namespace DAL
{
    public class UsuarioHistorialDAL
    {
        public int GuardarVersion(UsuarioHistorialSnapshot snapshot, AccesoDatosSqlServer accesoDatos = null)
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
            accesoDatos = accesoDatos ?? new AccesoDatosSqlServer();

            try
            {
                List<SqlParameter> parametros = new List<SqlParameter>
                {
                    accesoDatos.CreateParameter("@UsuarioId", SqlDbType.Int, snapshot.Usuario.Id),
                    accesoDatos.CreateParameter("@FechaHora", SqlDbType.DateTime, snapshot.FechaHora),
                    accesoDatos.CreateParameter("@ActorUsuarioId", SqlDbType.Int, snapshot.ActorUsuario?.Id),
                    accesoDatos.CreateParameter("@ActorUsernameSnapshot", SqlDbType.VarChar, snapshot.ActorUsernameSnapshot),
                    accesoDatos.CreateParameter("@Accion", SqlDbType.VarChar, snapshot.Accion),
                    accesoDatos.CreateParameter("@Observaciones", SqlDbType.VarChar, snapshot.Observaciones),
                    accesoDatos.CreateParameter("@Username", SqlDbType.VarChar, snapshot.Username),
                    accesoDatos.CreateParameter("@PasswordHash", SqlDbType.VarChar, snapshot.PasswordHash),
                    accesoDatos.CreateParameter("@Activo", SqlDbType.Bit, snapshot.Activo),
                    accesoDatos.CreateParameter("@IdiomaId", SqlDbType.Int, snapshot.Idioma?.Id)
                };

                int historialId = accesoDatos.WriteWithResult(insertHistorial, parametros);

                List<int> permisos = snapshot.Permisos == null
                    ? new List<int>()
                    : snapshot.Permisos.Select(p => p.Id).Distinct().OrderBy(item => item).ToList();

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

        public List<UsuarioHistorialSnapshot> ObtenerVersiones(IUsuarioAutenticado usuario)
        {
            const string query = @"
                SELECT HistorialId, UsuarioId, FechaHora, ActorUsuarioId, ActorUsernameSnapshot, Accion, Observaciones,
                       Username, PasswordHash, Activo, IdiomaId
                FROM UsuarioHistorial
                WHERE UsuarioId = @UsuarioId
                ORDER BY FechaHora DESC, HistorialId DESC";

            using (AccesoDatosSqlServer accesoDatos = new AccesoDatosSqlServer())
            {
                List<SqlParameter> parametros = new List<SqlParameter>
                {
                    accesoDatos.CreateParameter("@UsuarioId", SqlDbType.Int, usuario.Id)
                };

                DataTable dataTable = accesoDatos.Read(query, parametros);
                return dataTable.Rows.Cast<DataRow>()
                    .Select(row => Map(row, accesoDatos))
                    .ToList();
            }
        }

        public UsuarioHistorialSnapshot ObtenerPorId(int id)
        {
            const string query = @"
                SELECT HistorialId, UsuarioId, FechaHora, ActorUsuarioId, ActorUsernameSnapshot, Accion, Observaciones,
                       Username, PasswordHash, Activo, IdiomaId
                FROM UsuarioHistorial
                WHERE HistorialId = @HistorialId";

            using (AccesoDatosSqlServer accesoDatos = new AccesoDatosSqlServer())
            {
                List<SqlParameter> parametros = new List<SqlParameter>
                {
                    accesoDatos.CreateParameter("@HistorialId", SqlDbType.Int, id)
                };

                DataTable dataTable = accesoDatos.Read(query, parametros);
                if (dataTable.Rows.Count == 0)
                {
                    return null;
                }

                return Map(dataTable.Rows[0], accesoDatos);
            }
        }

        private UsuarioHistorialSnapshot Map(DataRow row, AccesoDatosSqlServer accesoDatos)
        {
            int historialId = (int)row["HistorialId"];
            return new UsuarioHistorialSnapshot
            {
                HistorialId = historialId,
                Usuario = new UsuarioDAL().ObtenerPorId((int)row["UsuarioId"], accesoDatos),
                FechaHora = (DateTime)row["FechaHora"],
                ActorUsuario = row.IsNull("ActorUsuarioId") ? null : new UsuarioDAL().ObtenerPorId((int)row["ActorUsuarioId"], accesoDatos),
                ActorUsernameSnapshot = row["ActorUsernameSnapshot"].ToString(),
                Accion = row["Accion"].ToString(),
                Observaciones = row["Observaciones"].ToString(),
                Username = row["Username"].ToString(),
                PasswordHash = row["PasswordHash"].ToString(),
                Activo = (bool)row["Activo"],
                Idioma = row.IsNull("IdiomaId") ? null : new IdiomaDAL().ObtenerPorId((int)row["IdiomaId"], accesoDatos)
                    ?? new Idioma { Id = (int)row["IdiomaId"] },
                Permisos = ObtenerPermisos(historialId, accesoDatos)
            };
        }

        private List<IPermiso> ObtenerPermisos(int historialId, AccesoDatosSqlServer accesoDatos)
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
            var catalogo = new PermisoDAL().ObtenerNodos(accesoDatos).ToDictionary(p => p.Id);
            return dataTable.Rows.Cast<DataRow>()
                .Select(row => catalogo.TryGetValue((int)row["PermisoId"], out IPermiso permiso) ? permiso : new PermisoSimple { Id = (int)row["PermisoId"] })
                .ToList();
        }
    }
}
