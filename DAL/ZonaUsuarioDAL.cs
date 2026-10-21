using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using BE;

namespace DAL
{
    public class ZonaUsuarioDAL
    {
        private ZonaUsuario Map(DataRow row)
        {
            return new ZonaUsuario
            {
                Id = (int)row["Id"],
                Usuario = new Usuario { Id = (int)row["UsuarioId"] },
                Nombre = row["Nombre"].ToString(),
                Latitud = (decimal)row["Latitud"],
                Longitud = (decimal)row["Longitud"],
                RadioMetros = (int)row["RadioMetros"]
            };
        }

        public ZonaUsuario ObtenerPorId(int id, AccesoDatosSqlServer accesoDatos = null)
        {
            const string query = @"
                SELECT Id, UsuarioId, Nombre, Latitud, Longitud, RadioMetros
                FROM ZonaUsuario
                WHERE Id = @Id";

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

        public List<ZonaUsuario> ObtenerPorUsuario(Usuario usuario, AccesoDatosSqlServer accesoDatos = null)
        {
            const string query = @"
                SELECT Id, UsuarioId, Nombre, Latitud, Longitud, RadioMetros
                FROM ZonaUsuario
                WHERE UsuarioId = @UsuarioId
                ORDER BY Nombre";

            bool dispose = accesoDatos == null;
            accesoDatos = accesoDatos ?? new AccesoDatosSqlServer();

            try
            {
                List<SqlParameter> parametros = new List<SqlParameter>
                {
                    accesoDatos.CreateParameter("@UsuarioId", SqlDbType.Int, usuario.Id)
                };

                DataTable dataTable = accesoDatos.Read(query, parametros);
                List<ZonaUsuario> zonas = new List<ZonaUsuario>();
                foreach (DataRow row in dataTable.Rows)
                {
                    zonas.Add(Map(row));
                }

                return zonas;
            }
            finally
            {
                if (dispose)
                {
                    accesoDatos.Dispose();
                }
            }
        }

        public int InsertarYObtenerId(ZonaUsuario zona, AccesoDatosSqlServer accesoDatos = null)
        {
            const string query = @"
                INSERT INTO ZonaUsuario (UsuarioId, Nombre, Latitud, Longitud, RadioMetros)
                OUTPUT INSERTED.Id
                VALUES (@UsuarioId, @Nombre, @Latitud, @Longitud, @RadioMetros)";

            bool dispose = accesoDatos == null;
            accesoDatos = accesoDatos ?? new AccesoDatosSqlServer();

            try
            {
                List<SqlParameter> parametros = new List<SqlParameter>
                {
                    accesoDatos.CreateParameter("@UsuarioId", SqlDbType.Int, zona.Usuario.Id),
                    accesoDatos.CreateParameter("@Nombre", SqlDbType.VarChar, zona.Nombre),
                    accesoDatos.CreateParameter("@Latitud", SqlDbType.Decimal, zona.Latitud),
                    accesoDatos.CreateParameter("@Longitud", SqlDbType.Decimal, zona.Longitud),
                    accesoDatos.CreateParameter("@RadioMetros", SqlDbType.Int, zona.RadioMetros)
                };

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

        public bool Eliminar(ZonaUsuario zona, AccesoDatosSqlServer accesoDatos = null)
        {
            const string query = @"
                DELETE FROM ZonaUsuario
                WHERE Id = @Id";

            bool dispose = accesoDatos == null;
            accesoDatos = accesoDatos ?? new AccesoDatosSqlServer();

            try
            {
                List<SqlParameter> parametros = new List<SqlParameter>
                {
                    accesoDatos.CreateParameter("@Id", SqlDbType.Int, zona.Id)
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
