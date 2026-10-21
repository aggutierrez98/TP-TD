using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using BE;

namespace DAL
{
    public class PromocionComercioDAL
    {
        public List<Comercio> ObtenerComerciosPorPromocion(Promocion promocion, AccesoDatosSqlServer accesoDatos = null)
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
                    accesoDatos.CreateParameter("@PromocionId", SqlDbType.Int, promocion.Id)
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

        public List<Promocion> ObtenerPromocionesPorComercio(Comercio comercio, AccesoDatosSqlServer accesoDatos = null)
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
                    accesoDatos.CreateParameter("@ComercioId", SqlDbType.Int, comercio.Id)
                };

                DataTable dataTable = accesoDatos.Read(query, parametros);
                return dataTable.Rows.Cast<DataRow>()
                    .Select(row => new Promocion { Id = (int)row["PromocionId"] })
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

        public bool ReemplazarComerciosDePromocion(Promocion promocion, List<Comercio> comercios, AccesoDatosSqlServer accesoDatos = null)
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
                    DELETE FROM PromocionComercio
                    WHERE PromocionId = @PromocionId;";

                for (int i = 0; i < comercios.Count; i++)
                {
                    query += string.Format(@"
                        INSERT INTO PromocionComercio (PromocionId, ComercioId)
                        VALUES (@PromocionId, @ComercioId{0});", i);

                    parametros.Add(accesoDatos.CreateParameter("@ComercioId" + i, SqlDbType.Int, comercios[i].Id));
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
