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
        public List<PermisoNodo> ObtenerNodos()
        {
            const string query = @"
SELECT p.Id,
       p.Nombre,
       CASE WHEN EXISTS (
           SELECT 1
           FROM Grupo g
           WHERE g.GrupoId = p.Id) THEN 1 ELSE 0 END AS EsGrupo
FROM Permiso p
ORDER BY p.Nombre";

            using (AccesoDatos accesoDatos = new AccesoDatos())
            {
                DataTable dataTable = accesoDatos.Read(query);
                return dataTable.Rows.Cast<DataRow>()
                    .Select(MapNodo)
                    .ToList();
            }
        }

        public List<PermisoRelacion> ObtenerRelaciones()
        {
            const string query = @"
SELECT GrupoId, PermisoId
FROM Grupo
ORDER BY GrupoId, PermisoId";

            using (AccesoDatos accesoDatos = new AccesoDatos())
            {
                DataTable dataTable = accesoDatos.Read(query);
                return dataTable.Rows.Cast<DataRow>()
                    .Select(row => new PermisoRelacion
                    {
                        GrupoId = (int)row["GrupoId"],
                        PermisoId = (int)row["PermisoId"]
                    })
                    .ToList();
            }
        }

        public int InsertarPermiso(string nombre)
        {
            const string query = @"
INSERT INTO Permiso (Nombre)
OUTPUT INSERTED.Id
VALUES (@Nombre)";

            using (AccesoDatos accesoDatos = new AccesoDatos())
            {
                List<SqlParameter> parametros = new List<SqlParameter>
                {
                    accesoDatos.CreateParameter("@Nombre", SqlDbType.VarChar, nombre)
                };

                return accesoDatos.WriteWithResult(query, parametros);
            }
        }

        public bool ActualizarNombre(int id, string nombre)
        {
            const string query = @"
UPDATE Permiso
SET Nombre = @Nombre
WHERE Id = @Id";

            using (AccesoDatos accesoDatos = new AccesoDatos())
            {
                List<SqlParameter> parametros = new List<SqlParameter>
                {
                    accesoDatos.CreateParameter("@Id", SqlDbType.Int, id),
                    accesoDatos.CreateParameter("@Nombre", SqlDbType.VarChar, nombre)
                };

                return accesoDatos.Write(query, parametros) > 0;
            }
        }

        public bool ReemplazarRelaciones(List<PermisoRelacion> relaciones)
        {
            using (AccesoDatos accesoDatos = new AccesoDatos())
            {
                List<SqlParameter> parametros = new List<SqlParameter>();
                string query = "DELETE FROM Grupo;";

                for (int i = 0; i < relaciones.Count; i++)
                {
                    query += $@"
INSERT INTO Grupo (GrupoId, PermisoId)
VALUES (@GrupoId{i}, @PermisoId{i});";

                    parametros.Add(accesoDatos.CreateParameter("@GrupoId" + i, SqlDbType.Int, relaciones[i].GrupoId));
                    parametros.Add(accesoDatos.CreateParameter("@PermisoId" + i, SqlDbType.Int, relaciones[i].PermisoId));
                }

                return accesoDatos.Write(query, parametros) >= 0;
            }
        }

        public bool EliminarPermisos(List<int> idsPermiso)
        {
            if (idsPermiso == null || idsPermiso.Count == 0)
            {
                return true;
            }

            using (AccesoDatos accesoDatos = new AccesoDatos())
            {
                List<SqlParameter> parametros = new List<SqlParameter>();
                string whereClause = CrearWhereIn(accesoDatos, "@Id", idsPermiso, parametros);
                string query = $@"
DELETE FROM UsuarioPermiso WHERE PermisoId IN ({whereClause});
DELETE FROM Grupo WHERE GrupoId IN ({whereClause}) OR PermisoId IN ({whereClause});
DELETE FROM Permiso WHERE Id IN ({whereClause});";

                return accesoDatos.Write(query, parametros) >= 0;
            }
        }

        private static PermisoNodo MapNodo(DataRow row)
        {
            bool esGrupo = Convert.ToInt32(row["EsGrupo"]) == 1;
            PermisoNodo nodo = esGrupo ? (PermisoNodo)new GrupoPermiso() : new PermisoSimple();
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
