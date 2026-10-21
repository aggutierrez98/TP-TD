using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using BE;

namespace DAL
{
    public class PromocionRubroComercioDAL
    {
        public List<RubroComercio> ObtenerRubrosComercioPorPromocion(Promocion promocion, AccesoDatosSqlServer accesoDatos = null)
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
                    accesoDatos.CreateParameter("@PromocionId", SqlDbType.Int, promocion.Id)
                };

                DataTable dataTable = accesoDatos.Read(query, parametros);
                return dataTable.Rows.Cast<DataRow>()
                    .Select(row => new RubroComercio { Id = (int)row["RubroComercioId"] })
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

        public bool ReemplazarRubrosComercioDePromocion(Promocion promocion, List<RubroComercio> rubrosComercio, AccesoDatosSqlServer accesoDatos = null)
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
                    DELETE FROM PromocionRubroComercio
                    WHERE PromocionId = @PromocionId;";

                for (int i = 0; i < rubrosComercio.Count; i++)
                {
                    query += string.Format(@"
                        INSERT INTO PromocionRubroComercio (PromocionId, RubroComercioId)
                        VALUES (@PromocionId, @RubroComercioId{0});", i);

                    parametros.Add(accesoDatos.CreateParameter("@RubroComercioId" + i, SqlDbType.Int, rubrosComercio[i].Id));
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

        public int ContarPorRubroComercio(RubroComercio rubroComercio, AccesoDatosSqlServer accesoDatos = null)
        {
            const string query = @"
                SELECT COUNT(*) AS Total
                FROM PromocionRubroComercio
                WHERE RubroComercioId = @Id";

            bool dispose = accesoDatos == null;
            accesoDatos = accesoDatos ?? new AccesoDatosSqlServer();

            try
            {
                List<SqlParameter> parametros = new List<SqlParameter>
                {
                    accesoDatos.CreateParameter("@Id", SqlDbType.Int, rubroComercio.Id)
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
