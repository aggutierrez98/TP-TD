using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace DAL
{
    public class ComercioReglaCategorizacionDAL
    {
        public List<int> ObtenerComercioIdsPorReglaCategorizacion(int reglaCategorizacionId, AccesoDatosSqlServer accesoDatos = null)
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
                    accesoDatos.CreateParameter("@ReglaCategorizacionId", SqlDbType.Int, reglaCategorizacionId)
                };

                DataTable dataTable = accesoDatos.Read(query, parametros);
                return dataTable.Rows.Cast<DataRow>()
                    .Select(row => (int)row["ComercioId"])
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

        public bool ReemplazarComerciosDeReglaCategorizacion(int reglaCategorizacionId, List<int> comercioIds, AccesoDatosSqlServer accesoDatos = null)
        {
            bool dispose = accesoDatos == null;
            accesoDatos = accesoDatos ?? new AccesoDatosSqlServer();

            try
            {
                List<SqlParameter> parametros = new List<SqlParameter>
                {
                    accesoDatos.CreateParameter("@ReglaCategorizacionId", SqlDbType.Int, reglaCategorizacionId)
                };

                string query = @"
                    DELETE FROM ComercioReglaCategorizacion
                    WHERE ReglaCategorizacionId = @ReglaCategorizacionId;";

                for (int i = 0; i < comercioIds.Count; i++)
                {
                    query += string.Format(@"
                        INSERT INTO ComercioReglaCategorizacion (ReglaCategorizacionId, ComercioId)
                        VALUES (@ReglaCategorizacionId, @ComercioId{0});", i);

                    parametros.Add(accesoDatos.CreateParameter("@ComercioId" + i, SqlDbType.Int, comercioIds[i]));
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

        public int ContarPorComercio(int comercioId, AccesoDatosSqlServer accesoDatos = null)
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
                    accesoDatos.CreateParameter("@Id", SqlDbType.Int, comercioId)
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
