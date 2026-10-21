using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using ABSTRACCIONES;
using BE;

namespace DAL
{
    public class UsuarioPermisoDAL
    {
        public List<IPermiso> ObtenerPermisosPorUsuario(Usuario usuario, AccesoDatosSqlServer accesoDatos = null)
        {
            const string query = @"
                SELECT PermisoId
                FROM UsuarioPermiso
                WHERE UsuarioId = @UsuarioId
                ORDER BY PermisoId";

            bool dispose = accesoDatos == null;
            accesoDatos = accesoDatos ?? new AccesoDatosSqlServer();

            try
            {
                List<SqlParameter> parametros = new List<SqlParameter>
                {
                    accesoDatos.CreateParameter("@UsuarioId", SqlDbType.Int, usuario.Id)
                };

                DataTable dataTable = accesoDatos.Read(query, parametros);
                var catalogo = new PermisoDAL().ObtenerNodos(accesoDatos).ToDictionary(p => p.Id);
                return dataTable.Rows.Cast<DataRow>()
                    .Select(row => catalogo[(int)row["PermisoId"]])
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

        public bool ReemplazarPermisosUsuario(Usuario usuario, List<IPermiso> permisos, AccesoDatosSqlServer accesoDatos = null)
        {
            bool dispose = accesoDatos == null;
            accesoDatos = accesoDatos ?? new AccesoDatosSqlServer();

            try
            {
                List<SqlParameter> parametros = new List<SqlParameter>
                {
                    accesoDatos.CreateParameter("@UsuarioId", SqlDbType.Int, usuario.Id)
                };

                string query = @"
                    DELETE FROM UsuarioPermiso
                    WHERE UsuarioId = @UsuarioId;";

                for (int i = 0; i < permisos.Count; i++)
                {
                    query += string.Format(@"
                        INSERT INTO UsuarioPermiso (UsuarioId, PermisoId)
                        VALUES (@UsuarioId, @PermisoId{0});", i);

                    parametros.Add(accesoDatos.CreateParameter("@PermisoId" + i, SqlDbType.Int, permisos[i].Id));
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
