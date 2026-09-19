using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace DAL
{
    public class PromocionMedioDePagoDAL
    {
        public List<int> ObtenerMedioDePagoIdsPorPromocion(int promocionId, AccesoDatosSqlServer accesoDatos = null)
        {
            const string query = @"
                SELECT MedioDePagoId
                FROM PromocionMedioDePago
                WHERE PromocionId = @PromocionId
                ORDER BY MedioDePagoId";

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
                    .Select(row => (int)row["MedioDePagoId"])
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

        public bool ReemplazarMediosDePagoDePromocion(int promocionId, List<int> medioDePagoIds, AccesoDatosSqlServer accesoDatos = null)
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
                    DELETE FROM PromocionMedioDePago
                    WHERE PromocionId = @PromocionId;";

                for (int i = 0; i < medioDePagoIds.Count; i++)
                {
                    query += string.Format(@"
                        INSERT INTO PromocionMedioDePago (PromocionId, MedioDePagoId)
                        VALUES (@PromocionId, @MedioDePagoId{0});", i);

                    parametros.Add(accesoDatos.CreateParameter("@MedioDePagoId" + i, SqlDbType.Int, medioDePagoIds[i]));
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
