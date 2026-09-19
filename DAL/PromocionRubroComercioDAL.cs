using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace DAL
{
    public class PromocionRubroComercioDAL
    {
        public List<int> ObtenerRubroComercioIdsPorPromocion(int promocionId, AccesoDatosSqlServer accesoDatos = null)
        {
            const string query = @"
                SELECT RubroComercioId
                FROM PromocionRubroComercio
                WHERE PromocionId = @PromocionId
                ORDER BY RubroComercioId";

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
                    .Select(row => (int)row["RubroComercioId"])
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

        public bool ReemplazarRubrosComercioDePromocion(int promocionId, List<int> rubroComercioIds, AccesoDatosSqlServer accesoDatos = null)
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
                    DELETE FROM PromocionRubroComercio
                    WHERE PromocionId = @PromocionId;";

                for (int i = 0; i < rubroComercioIds.Count; i++)
                {
                    query += string.Format(@"
                        INSERT INTO PromocionRubroComercio (PromocionId, RubroComercioId)
                        VALUES (@PromocionId, @RubroComercioId{0});", i);

                    parametros.Add(accesoDatos.CreateParameter("@RubroComercioId" + i, SqlDbType.Int, rubroComercioIds[i]));
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
