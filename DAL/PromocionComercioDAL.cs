using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace DAL
{
    public class PromocionComercioDAL
    {
        public List<int> ObtenerComercioIdsPorPromocion(int promocionId, AccesoDatosSqlServer accesoDatos = null)
        {
            const string query = @"
                SELECT ComercioId
                FROM PromocionComercio
                WHERE PromocionId = @PromocionId
                ORDER BY ComercioId";

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

        public List<int> ObtenerPromocionIdsPorComercio(int comercioId, AccesoDatosSqlServer accesoDatos = null)
        {
            const string query = @"
                SELECT PromocionId
                FROM PromocionComercio
                WHERE ComercioId = @ComercioId
                ORDER BY PromocionId";

            bool dispose = accesoDatos == null;
            accesoDatos = accesoDatos ?? new AccesoDatosSqlServer();

            try
            {
                List<SqlParameter> parametros = new List<SqlParameter>
                {
                    accesoDatos.CreateParameter("@ComercioId", SqlDbType.Int, comercioId)
                };

                DataTable dataTable = accesoDatos.Read(query, parametros);
                return dataTable.Rows.Cast<DataRow>()
                    .Select(row => (int)row["PromocionId"])
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

        public bool ReemplazarComerciosDePromocion(int promocionId, List<int> comercioIds, AccesoDatosSqlServer accesoDatos = null)
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
                    DELETE FROM PromocionComercio
                    WHERE PromocionId = @PromocionId;";

                for (int i = 0; i < comercioIds.Count; i++)
                {
                    query += string.Format(@"
                        INSERT INTO PromocionComercio (PromocionId, ComercioId)
                        VALUES (@PromocionId, @ComercioId{0});", i);

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
    }
}
