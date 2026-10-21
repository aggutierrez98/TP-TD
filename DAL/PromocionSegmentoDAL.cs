using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using BE;

namespace DAL
{
    public class PromocionSegmentoDAL
    {
        public List<string> ObtenerSegmentosPorPromocion(Promocion promocion, AccesoDatosSqlServer accesoDatos = null)
        {
            const string query = @"
                SELECT Segmento
                FROM PromocionSegmento
                WHERE PromocionId = @PromocionId
                ORDER BY Segmento";

            bool dispose = accesoDatos == null;
            accesoDatos = accesoDatos ?? new AccesoDatosSqlServer();

            try
            {
                List<SqlParameter> parametros = new List<SqlParameter>
                {
                    accesoDatos.CreateParameter("@PromocionId", SqlDbType.Int, promocion.Id)
                };

                DataTable dataTable = accesoDatos.Read(query, parametros);
                return dataTable.Rows.Cast<DataRow>()
                    .Select(row => (string)row["Segmento"])
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

        public bool ReemplazarSegmentosDePromocion(Promocion promocion, List<string> segmentos, AccesoDatosSqlServer accesoDatos = null)
        {
            bool dispose = accesoDatos == null;
            accesoDatos = accesoDatos ?? new AccesoDatosSqlServer();

            try
            {
                List<SqlParameter> parametros = new List<SqlParameter>
                {
                    accesoDatos.CreateParameter("@PromocionId", SqlDbType.Int, promocion.Id)
                };

                string query = @"
                    DELETE FROM PromocionSegmento
                    WHERE PromocionId = @PromocionId;";

                for (int i = 0; i < segmentos.Count; i++)
                {
                    query += string.Format(@"
                        INSERT INTO PromocionSegmento (PromocionId, Segmento)
                        VALUES (@PromocionId, @Segmento{0});", i);

                    parametros.Add(accesoDatos.CreateParameter("@Segmento" + i, SqlDbType.VarChar, segmentos[i]));
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
