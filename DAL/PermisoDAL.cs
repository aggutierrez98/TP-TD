using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using SERVICIOS;

namespace DAL
{
    public class PermisoDAL
    {
        public List<object> ObtenerNodos(AccesoDatos accesoDatos = null)
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

        public List<Tuple<int, int>> ObtenerRelaciones(AccesoDatos accesoDatos = null)
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
                    .Select(row => Tuple.Create((int)row["PermisoPadreId"], (int)row["PermisoHijoId"]))
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

        public int Guardar(object permiso, AccesoDatos accesoDatos = null)
        {
            bool dispose = accesoDatos == null;
            accesoDatos = accesoDatos ?? new AccesoDatos();

            try
            {
                int permisoId = ObtenerId(permiso);
                string permisoNombre = ObtenerNombre(permiso);
                bool esGrupo = EsGrupo(permiso);

                if (permisoId <= 0)
                {
                    const string insertQuery = @"
                        INSERT INTO Permiso (Nombre, EsGrupo)
                        OUTPUT INSERTED.Id
                        VALUES (@Nombre, @EsGrupo)";

                    List<SqlParameter> insertParametros = new List<SqlParameter>
                    {
                        accesoDatos.CreateParameter("@Nombre", SqlDbType.VarChar, permisoNombre),
                        accesoDatos.CreateParameter("@EsGrupo", SqlDbType.Bit, esGrupo)
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
                    accesoDatos.CreateParameter("@Id", SqlDbType.Int, permisoId),
                    accesoDatos.CreateParameter("@Nombre", SqlDbType.VarChar, permisoNombre),
                    accesoDatos.CreateParameter("@EsGrupo", SqlDbType.Bit, esGrupo)
                };

                accesoDatos.Write(updateQuery, updateParametros);
                return permisoId;
            }
            finally
            {
                if (dispose)
                {
                    accesoDatos.Dispose();
                }
            }
        }

        public bool ReemplazarRelaciones(List<Tuple<int, int>> relaciones, AccesoDatos accesoDatos = null)
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

                    parametros.Add(accesoDatos.CreateParameter("@GrupoId" + i, SqlDbType.Int, relaciones[i].Item1));
                    parametros.Add(accesoDatos.CreateParameter("@PermisoId" + i, SqlDbType.Int, relaciones[i].Item2));
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

        private static object MapNodo(DataRow row)
        {
            bool esGrupo = row["EsGrupo"] != DBNull.Value && Convert.ToBoolean(row["EsGrupo"]);
            object nodo = esGrupo ? (object) new PermisoGrupo() : new PermisoSimple();

            if (nodo is PermisoGrupo grupo)
            {
                grupo.Id = (int)row["Id"];
                grupo.Nombre = row["Nombre"].ToString();
            }
            else if (nodo is PermisoSimple simple)
            {
                simple.Id = (int)row["Id"];
                simple.Nombre = row["Nombre"].ToString();
            }

            return nodo;
        }

        private static int ObtenerId(object permiso)
        {
            if (permiso is PermisoGrupo grupo)
            {
                return grupo.Id;
            }

            if (permiso is PermisoSimple simple)
            {
                return simple.Id;
            }

            throw new InvalidOperationException("Tipo de permiso no soportado.");
        }

        private static string ObtenerNombre(object permiso)
        {
            if (permiso is PermisoGrupo grupo)
            {
                return grupo.Nombre;
            }

            if (permiso is PermisoSimple simple)
            {
                return simple.Nombre;
            }

            throw new InvalidOperationException("Tipo de permiso no soportado.");
        }

        private static bool EsGrupo(object permiso)
        {
            if (permiso is PermisoGrupo)
            {
                return true;
            }

            if (permiso is PermisoSimple)
            {
                return false;
            }

            throw new InvalidOperationException("Tipo de permiso no soportado.");
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
