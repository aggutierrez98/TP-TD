using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace DAL
{
    public class PromocionEntidadFinancieraDAL
    {
        public List<int> ObtenerEntidadFinancieraIdsPorPromocion(int promocionId, AccesoDatosSqlServer accesoDatos = null)
        {
            const string query = @"
                SELECT EntidadFinancieraId
                FROM PromocionEntidadFinanciera
                WHERE PromocionId = @PromocionId
                ORDER BY EntidadFinancieraId";

            bool dispose = accesoDatos == null;
            accesoDatos = accesoDatos ?? new AccesoDatosSqlServer();

            try
            {
                List<SqlParameter> parametros = new List<SqlParameter>
                {
                    accesoDatos.CreateParameter("@PromocionId", SqlDbType.Int, promocionId)
                };

                DataTable dataTable = accesoDatos.Read(query, parametros);
                return dataTable.Rows.Cast<DataRow>()
                    .Select(row => (int)row["EntidadFinancieraId"])
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

        public bool ReemplazarEntidadesFinancierasDePromocion(int promocionId, List<int> entidadFinancieraIds, AccesoDatosSqlServer accesoDatos = null)
        {
            bool dispose = accesoDatos == null;
            accesoDatos = accesoDatos ?? new AccesoDatosSqlServer();

            try
            {
                List<SqlParameter> parametros = new List<SqlParameter>
                {
                    accesoDatos.CreateParameter("@PromocionId", SqlDbType.Int, promocionId)
                };

                string query = @"
                    DELETE FROM PromocionEntidadFinanciera
                    WHERE PromocionId = @PromocionId;";

                for (int i = 0; i < entidadFinancieraIds.Count; i++)
                {
                    query += string.Format(@"
                        INSERT INTO PromocionEntidadFinanciera (PromocionId, EntidadFinancieraId)
                        VALUES (@PromocionId, @EntidadFinancieraId{0});", i);

                    parametros.Add(accesoDatos.CreateParameter("@EntidadFinancieraId" + i, SqlDbType.Int, entidadFinancieraIds[i]));
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

        public int ContarPorEntidadFinanciera(int entidadFinancieraId, AccesoDatosSqlServer accesoDatos = null)
        {
            const string query = @"
                SELECT COUNT(*) AS Total
                FROM PromocionEntidadFinanciera
                WHERE EntidadFinancieraId = @Id";

            bool dispose = accesoDatos == null;
            accesoDatos = accesoDatos ?? new AccesoDatosSqlServer();

            try
            {
                List<SqlParameter> parametros = new List<SqlParameter>
                {
                    accesoDatos.CreateParameter("@Id", SqlDbType.Int, entidadFinancieraId)
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
