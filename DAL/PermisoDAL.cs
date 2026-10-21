using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using ABSTRACCIONES;
using SERVICIOS;

namespace DAL
{
    public class PermisoDAL
    {
        public List<IPermiso> ObtenerNodos(AccesoDatosSqlServer accesoDatos = null)
        {
            const string query = @"
                SELECT p.Id,
                       p.Nombre,
                       p.EsGrupo
                FROM Permiso p
                ORDER BY p.Nombre, p.Id";

            bool dispose = accesoDatos == null;
            accesoDatos = accesoDatos ?? new AccesoDatosSqlServer();

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

        public List<Tuple<int, int>> ObtenerRelaciones(AccesoDatosSqlServer accesoDatos = null)
        {
            const string query = @"
                SELECT PermisoPadreId,
                       PermisoHijoId
                FROM PermisoCompuesto
                ORDER BY PermisoPadreId, PermisoHijoId";

            bool dispose = accesoDatos == null;
            accesoDatos = accesoDatos ?? new AccesoDatosSqlServer();

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

        public int Guardar(IPermiso permiso, AccesoDatosSqlServer accesoDatos = null)
        {
            bool dispose = accesoDatos == null;
            accesoDatos = accesoDatos ?? new AccesoDatosSqlServer();

            try
            {
                int permisoId = permiso.Id;
                string permisoNombre = permiso.Nombre;
                bool esGrupo = permiso.EsGrupo;

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

        public bool ReemplazarRelaciones(List<Tuple<int, int>> relaciones, AccesoDatosSqlServer accesoDatos = null)
        {
            bool dispose = accesoDatos == null;
            accesoDatos = accesoDatos ?? new AccesoDatosSqlServer();

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

        public bool EliminarPermisos(List<IPermiso> permisos, AccesoDatosSqlServer accesoDatos = null)
        {
            if (permisos == null || permisos.Count == 0)
            {
                return true;
            }

            bool dispose = accesoDatos == null;
            accesoDatos = accesoDatos ?? new AccesoDatosSqlServer();

            try
            {
                List<SqlParameter> parametros = new List<SqlParameter>();
                string whereClause = CrearWhereIn(accesoDatos, "@Id", permisos.Select(p => p.Id).ToList(), parametros);
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

        public IPermiso ObtenerPorNombre(string nombre, AccesoDatosSqlServer accesoDatos = null)
        {
            const string query = @"
                SELECT TOP 1 Id, Nombre, EsGrupo
                FROM Permiso
                WHERE Nombre = @Nombre";

            bool dispose = accesoDatos == null;
            accesoDatos = accesoDatos ?? new AccesoDatosSqlServer();

            try
            {
                List<SqlParameter> parametros = new List<SqlParameter>
                {
                    accesoDatos.CreateParameter("@Nombre", SqlDbType.VarChar, nombre)
                };

                DataTable dataTable = accesoDatos.Read(query, parametros);
                return dataTable.Rows.Count == 0
                    ? null
                    : MapNodo(dataTable.Rows[0]);
            }
            finally
            {
                if (dispose)
                {
                    accesoDatos.Dispose();
                }
            }
        }

        private static IPermiso MapNodo(DataRow row)
        {
            bool esGrupo = row["EsGrupo"] != DBNull.Value && Convert.ToBoolean(row["EsGrupo"]);
            IPermiso nodo = esGrupo ? (IPermiso)new PermisoGrupo() : new PermisoSimple();
            nodo.Id = (int)row["Id"];
            nodo.Nombre = row["Nombre"].ToString();
            return nodo;
        }

        private static string CrearWhereIn(AccesoDatosSqlServer accesoDatos, string prefijo, IList<int> ids, ICollection<SqlParameter> parametros)
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
