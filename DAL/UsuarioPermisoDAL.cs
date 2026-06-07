using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace DAL
{
    public class UsuarioPermisoDAL
    {
        public List<int> ObtenerPermisoIdsPorUsuario(int usuarioId, AccesoDatos accesoDatos = null)
        {
            const string query = @"
SELECT PermisoId
FROM UsuarioPermiso
WHERE UsuarioId = @UsuarioId
ORDER BY PermisoId";

            bool dispose = accesoDatos == null;
            accesoDatos = accesoDatos ?? new AccesoDatos();

            try
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
            finally
            {
                if (dispose)
                {
                    accesoDatos.Dispose();
                }
            }
        }

        public bool ReemplazarPermisosUsuario(int usuarioId, List<int> permisosId, AccesoDatos accesoDatos = null)
        {
            bool dispose = accesoDatos == null;
            accesoDatos = accesoDatos ?? new AccesoDatos();

            try
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
                    query += string.Format(@"
                        INSERT INTO UsuarioPermiso (UsuarioId, PermisoId)
                        VALUES (@UsuarioId, @PermisoId{0});", i);

                    parametros.Add(accesoDatos.CreateParameter("@PermisoId" + i, SqlDbType.Int, permisosId[i]));
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
    }
}
