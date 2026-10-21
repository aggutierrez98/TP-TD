using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using BE;

namespace DAL
{
    public class ComercioReglaCategorizacionDAL
    {
        // Devuelve los comercios con solo el Id; REPOSITORIO los hidrata.
        public List<Comercio> ObtenerComerciosPorReglaCategorizacion(ReglaCategorizacion reglaCategorizacion, AccesoDatosSqlServer accesoDatos = null)
        {
            const string query = @"
                SELECT ComercioId
                FROM ComercioReglaCategorizacion
                WHERE ReglaCategorizacionId = @ReglaCategorizacionId
                ORDER BY ComercioId";

            bool dispose = accesoDatos == null;
            accesoDatos = accesoDatos ?? new AccesoDatosSqlServer();

            try
            {
                List<SqlParameter> parametros = new List<SqlParameter>
                {
                    accesoDatos.CreateParameter("@ReglaCategorizacionId", SqlDbType.Int, reglaCategorizacion.Id)
                };

                DataTable dataTable = accesoDatos.Read(query, parametros);
                return dataTable.Rows.Cast<DataRow>()
                    .Select(row => new Comercio { Id = (int)row["ComercioId"] })
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

        public bool ReemplazarComerciosDeReglaCategorizacion(ReglaCategorizacion reglaCategorizacion, IEnumerable<Comercio> comercios, AccesoDatosSqlServer accesoDatos = null)
        {
            List<Comercio> comerciosAAsociar = comercios.ToList();
            bool dispose = accesoDatos == null;
            accesoDatos = accesoDatos ?? new AccesoDatosSqlServer();

            try
            {
                List<SqlParameter> parametros = new List<SqlParameter>
                {
                    accesoDatos.CreateParameter("@ReglaCategorizacionId", SqlDbType.Int, reglaCategorizacion.Id)
                };

                string query = @"
                    DELETE FROM ComercioReglaCategorizacion
                    WHERE ReglaCategorizacionId = @ReglaCategorizacionId;";

                for (int i = 0; i < comerciosAAsociar.Count; i++)
                {
                    query += string.Format(@"
                        INSERT INTO ComercioReglaCategorizacion (ReglaCategorizacionId, ComercioId)
                        VALUES (@ReglaCategorizacionId, @ComercioId{0});", i);

                    parametros.Add(accesoDatos.CreateParameter("@ComercioId" + i, SqlDbType.Int, comerciosAAsociar[i].Id));
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

        public int ContarPorComercio(Comercio comercio, AccesoDatosSqlServer accesoDatos = null)
        {
            const string query = @"
                SELECT COUNT(*) AS Total
                FROM ComercioReglaCategorizacion
                WHERE ComercioId = @Id";

            bool dispose = accesoDatos == null;
            accesoDatos = accesoDatos ?? new AccesoDatosSqlServer();

            try
            {
                List<SqlParameter> parametros = new List<SqlParameter>
                {
                    accesoDatos.CreateParameter("@Id", SqlDbType.Int, comercio.Id)
                };

                DataTable dataTable = accesoDatos.Read(query, parametros);
                return (int)dataTable.Rows[0]["Total"];
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
