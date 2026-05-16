using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace DAL
{
    public class UsuarioPermisoDAL
    {
        public List<int> ObtenerPermisoIdsPorUsuario(int usuarioId)
        {
            const string query = @"
SELECT PermisoId
FROM UsuarioPermiso
WHERE UsuarioId = @UsuarioId
ORDER BY PermisoId";

            using (AccesoDatos accesoDatos = new AccesoDatos())
            {
                List<SqlParameter> parametros = new List<SqlParameter>
                {
                    accesoDatos.CreateParameter("@UsuarioId", SqlDbType.Int, usuarioId)
                };

                DataTable dataTable = accesoDatos.Read(query, parametros);
                return dataTable.Rows.Cast<DataRow>()
                    .Select(row => (int)row["PermisoId"])
                    .ToList();
            }
        }

        public bool ReemplazarPermisosUsuario(int usuarioId, List<int> permisosId)
        {
            using (AccesoDatos accesoDatos = new AccesoDatos())
            {
                List<SqlParameter> parametros = new List<SqlParameter>
                {
                    accesoDatos.CreateParameter("@UsuarioId", SqlDbType.Int, usuarioId)
                };

                string query = @"
DELETE FROM UsuarioPermiso
WHERE UsuarioId = @UsuarioId;";

                for (int i = 0; i < permisosId.Count; i++)
                {
                    query += $@"
INSERT INTO UsuarioPermiso (UsuarioId, PermisoId)
VALUES (@UsuarioId, @PermisoId{i});";

                    parametros.Add(accesoDatos.CreateParameter("@PermisoId" + i, SqlDbType.Int, permisosId[i]));
                }

                return accesoDatos.Write(query, parametros) >= 0;
            }
        }
    }
}
