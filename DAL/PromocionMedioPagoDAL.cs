using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using BE;

namespace DAL
{
    public class PromocionMedioPagoDAL
    {
        public List<MedioPago> ObtenerMediosPagoPorPromocion(Promocion promocion, AccesoDatosSqlServer accesoDatos = null)
        {
            const string query = @"
                SELECT MedioPagoId
                FROM PromocionMedioPago
                WHERE PromocionId = @PromocionId
                ORDER BY MedioPagoId";

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
                    .Select(row => new MedioPago { Id = (int)row["MedioPagoId"] })
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

        public bool ReemplazarMediosPagoDePromocion(Promocion promocion, List<MedioPago> mediosPago, AccesoDatosSqlServer accesoDatos = null)
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
                    DELETE FROM PromocionMedioPago
                    WHERE PromocionId = @PromocionId;";

                for (int i = 0; i < mediosPago.Count; i++)
                {
                    query += string.Format(@"
                        INSERT INTO PromocionMedioPago (PromocionId, MedioPagoId)
                        VALUES (@PromocionId, @MedioPagoId{0});", i);

                    parametros.Add(accesoDatos.CreateParameter("@MedioPagoId" + i, SqlDbType.Int, mediosPago[i].Id));
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

        public int ContarPorMedioPago(MedioPago medioPago, AccesoDatosSqlServer accesoDatos = null)
        {
            const string query = @"
                SELECT COUNT(*) AS Total
                FROM PromocionMedioPago
                WHERE MedioPagoId = @Id";

            bool dispose = accesoDatos == null;
            accesoDatos = accesoDatos ?? new AccesoDatosSqlServer();

            try
            {
                List<SqlParameter> parametros = new List<SqlParameter>
                {
                    accesoDatos.CreateParameter("@Id", SqlDbType.Int, medioPago.Id)
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
