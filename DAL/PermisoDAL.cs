using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using SERVICIOS;

namespace DAL
{
    public class PermisoDAL
    {
        public List<PermisoNodo> ObtenerNodos(AccesoDatos accesoDatos = null)
        {
            const string query = @"
                SELECT p.Id,
                       p.Nombre,
                       p.EsGrupo
                FROM Permiso p
                ORDER BY p.Nombre, p.Id";

            bool dispose = accesoDatos == null;
            accesoDatos = accesoDatos ?? new AccesoDatos();

            try
            {
                DataTable dataTable = accesoDatos.Read(query);
                return dataTable.Rows.Cast<DataRow>()
                    .Select(MapNodo)
                    .ToList();
            }
            finally
            {
                if (dispose)
                {
                    accesoDatos.Dispose();
                }
            }
        }

        public List<PermisoRelacion> ObtenerRelaciones(AccesoDatos accesoDatos = null)
        {
            const string query = @"
                SELECT PermisoPadreId,
                       PermisoHijoId
                FROM PermisoCompuesto
                ORDER BY PermisoPadreId, PermisoHijoId";

            bool dispose = accesoDatos == null;
            accesoDatos = accesoDatos ?? new AccesoDatos();

            try
            {
                DataTable dataTable = accesoDatos.Read(query);
                return dataTable.Rows.Cast<DataRow>()
                    .Select(row => new PermisoRelacion
                    {
                        GrupoId = (int)row["PermisoPadreId"],
                        PermisoId = (int)row["PermisoHijoId"]
                    })
                    .ToList();
            }
            finally
            {
                if (dispose)
                {
                    accesoDatos.Dispose();
                }
            }
        }

        public int Guardar(PermisoNodo permiso, AccesoDatos accesoDatos = null)
        {
            bool dispose = accesoDatos == null;
            accesoDatos = accesoDatos ?? new AccesoDatos();

            try
            {
                if (permiso.Id <= 0)
                {
                    const string insertQuery = @"
                        INSERT INTO Permiso (Nombre, EsGrupo)
                        OUTPUT INSERTED.Id
                        VALUES (@Nombre, @EsGrupo)";

                    List<SqlParameter> insertParametros = new List<SqlParameter>
                    {
                        accesoDatos.CreateParameter("@Nombre", SqlDbType.VarChar, permiso.Nombre),
                        accesoDatos.CreateParameter("@EsGrupo", SqlDbType.Bit, permiso.EsGrupo)
                    };

                    return accesoDatos.WriteWithResult(insertQuery, insertParametros);
                }

                const string updateQuery = @"
                    UPDATE Permiso
                    SET Nombre = @Nombre,
                        EsGrupo = @EsGrupo
                    WHERE Id = @Id";

                List<SqlParameter> updateParametros = new List<SqlParameter>
                {
                    accesoDatos.CreateParameter("@Id", SqlDbType.Int, permiso.Id),
                    accesoDatos.CreateParameter("@Nombre", SqlDbType.VarChar, permiso.Nombre),
                    accesoDatos.CreateParameter("@EsGrupo", SqlDbType.Bit, permiso.EsGrupo)
                };

                accesoDatos.Write(updateQuery, updateParametros);
                return permiso.Id;
            }
            finally
            {
                if (dispose)
                {
                    accesoDatos.Dispose();
                }
            }
        }

        public bool ReemplazarRelaciones(List<PermisoRelacion> relaciones, AccesoDatos accesoDatos = null)
        {
            bool dispose = accesoDatos == null;
            accesoDatos = accesoDatos ?? new AccesoDatos();

            try
            {
                List<SqlParameter> parametros = new List<SqlParameter>();
                string query = "DELETE FROM PermisoCompuesto;";

                for (int i = 0; i < relaciones.Count; i++)
                {
                    query += $@"
                        INSERT INTO PermisoCompuesto (PermisoPadreId, PermisoHijoId)
                        VALUES (@GrupoId{i}, @PermisoId{i});";

                    parametros.Add(accesoDatos.CreateParameter("@GrupoId" + i, SqlDbType.Int, relaciones[i].GrupoId));
                    parametros.Add(accesoDatos.CreateParameter("@PermisoId" + i, SqlDbType.Int, relaciones[i].PermisoId));
                }

                return accesoDatos.Write(query, parametros) >= 0;
            }
            finally
            {
                if (dispose)
                {
                    accesoDatos.Dispose();
                }
            }
        }

        public bool EliminarPermisos(List<int> idsPermiso, AccesoDatos accesoDatos = null)
        {
            if (idsPermiso == null || idsPermiso.Count == 0)
            {
                return true;
            }

            bool dispose = accesoDatos == null;
            accesoDatos = accesoDatos ?? new AccesoDatos();

            try
            {
                List<SqlParameter> parametros = new List<SqlParameter>();
                string whereClause = CrearWhereIn(accesoDatos, "@Id", idsPermiso, parametros);
                string query = $@"
                    DELETE FROM UsuarioPermiso WHERE PermisoId IN ({whereClause});
                    DELETE FROM PermisoCompuesto WHERE PermisoPadreId IN ({whereClause}) OR PermisoHijoId IN ({whereClause});
                    DELETE FROM Permiso WHERE Id IN ({whereClause});";

                return accesoDatos.Write(query, parametros) >= 0;
            }
            finally
            {
                if (dispose)
                {
                    accesoDatos.Dispose();
                }
            }
        }

        public int ObtenerIdPorNombre(string nombre, AccesoDatos accesoDatos = null)
        {
            const string query = @"
                SELECT TOP 1 Id
                FROM Permiso
                WHERE Nombre = @Nombre";

            bool dispose = accesoDatos == null;
            accesoDatos = accesoDatos ?? new AccesoDatos();

            try
            {
                List<SqlParameter> parametros = new List<SqlParameter>
                {
                    accesoDatos.CreateParameter("@Nombre", SqlDbType.VarChar, nombre)
                };

                DataTable dataTable = accesoDatos.Read(query, parametros);
                return dataTable.Rows.Count == 0
                    ? 0
                    : (int)dataTable.Rows[0]["Id"];
            }
            finally
            {
                if (dispose)
                {
                    accesoDatos.Dispose();
                }
            }
        }

        private static PermisoNodo MapNodo(DataRow row)
        {
            bool esGrupo = row["EsGrupo"] != DBNull.Value && Convert.ToBoolean(row["EsGrupo"]);
            PermisoNodo nodo = esGrupo ? (PermisoNodo)new PermisoGrupo() : new PermisoSimple();
            nodo.Id = (int)row["Id"];
            nodo.Nombre = row["Nombre"].ToString();
            return nodo;
        }

        private static string CrearWhereIn(AccesoDatos accesoDatos, string prefijo, IList<int> ids, ICollection<SqlParameter> parametros)
        {
            List<string> nombres = new List<string>();
            for (int i = 0; i < ids.Count; i++)
            {
                string nombre = prefijo + i;
                nombres.Add(nombre);
                parametros.Add(accesoDatos.CreateParameter(nombre, SqlDbType.Int, ids[i]));
            }

            return string.Join(", ", nombres);
        }
    }
}
