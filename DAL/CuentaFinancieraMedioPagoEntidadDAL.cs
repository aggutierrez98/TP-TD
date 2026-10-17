using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace DAL
{
    public class CuentaFinancieraMedioPagoEntidadDAL
    {
        public List<int> ObtenerMedioPagoEntidadIdsPorCuenta(int cuentaFinancieraId, AccesoDatosSqlServer accesoDatos = null)
        {
            const string query = @"
                SELECT MedioPagoEntidadId
                FROM CuentaFinancieraMedioPagoEntidad
                WHERE CuentaFinancieraId = @CuentaFinancieraId
                ORDER BY MedioPagoEntidadId";

            bool dispose = accesoDatos == null;
            accesoDatos = accesoDatos ?? new AccesoDatosSqlServer();

            try
            {
                List<SqlParameter> parametros = new List<SqlParameter>
                {
                    accesoDatos.CreateParameter("@CuentaFinancieraId", SqlDbType.Int, cuentaFinancieraId)
                };

                DataTable dataTable = accesoDatos.Read(query, parametros);
                return dataTable.Rows.Cast<DataRow>()
                    .Select(row => (int)row["MedioPagoEntidadId"])
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

        public bool ReemplazarMediosPagoEntidadDeCuenta(int cuentaFinancieraId, List<int> medioPagoEntidadIds, AccesoDatosSqlServer accesoDatos = null)
        {
            bool dispose = accesoDatos == null;
            accesoDatos = accesoDatos ?? new AccesoDatosSqlServer();

            try
            {
                List<SqlParameter> parametros = new List<SqlParameter>
                {
                    accesoDatos.CreateParameter("@CuentaFinancieraId", SqlDbType.Int, cuentaFinancieraId)
                };

                string query = @"
                    DELETE FROM CuentaFinancieraMedioPagoEntidad
                    WHERE CuentaFinancieraId = @CuentaFinancieraId;";

                for (int i = 0; i < medioPagoEntidadIds.Count; i++)
                {
                    query += string.Format(@"
                        INSERT INTO CuentaFinancieraMedioPagoEntidad (CuentaFinancieraId, MedioPagoEntidadId)
                        VALUES (@CuentaFinancieraId, @MedioPagoEntidadId{0});", i);

                    parametros.Add(accesoDatos.CreateParameter("@MedioPagoEntidadId" + i, SqlDbType.Int, medioPagoEntidadIds[i]));
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

        public int ContarPorMedioPagoEntidad(int medioPagoEntidadId, AccesoDatosSqlServer accesoDatos = null)
        {
            const string query = @"
                SELECT COUNT(*) AS Total
                FROM CuentaFinancieraMedioPagoEntidad
                WHERE MedioPagoEntidadId = @Id";

            bool dispose = accesoDatos == null;
            accesoDatos = accesoDatos ?? new AccesoDatosSqlServer();

            try
            {
                List<SqlParameter> parametros = new List<SqlParameter>
                {
                    accesoDatos.CreateParameter("@Id", SqlDbType.Int, medioPagoEntidadId)
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
