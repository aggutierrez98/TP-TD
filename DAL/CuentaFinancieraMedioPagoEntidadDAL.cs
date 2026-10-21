using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using BE;

namespace DAL
{
    public class CuentaFinancieraMedioPagoEntidadDAL
    {
        public List<MedioPagoEntidad> ObtenerMediosPagoEntidadPorCuenta(CuentaFinanciera cuenta, AccesoDatosSqlServer accesoDatos = null)
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
                    accesoDatos.CreateParameter("@CuentaFinancieraId", SqlDbType.Int, cuenta.Id)
                };

                DataTable dataTable = accesoDatos.Read(query, parametros);
                return dataTable.Rows.Cast<DataRow>()
                    .Select(row => new MedioPagoEntidad { Id = (int)row["MedioPagoEntidadId"] })
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

        public bool ReemplazarMediosPagoEntidadDeCuenta(CuentaFinanciera cuenta, IEnumerable<MedioPagoEntidad> medios, AccesoDatosSqlServer accesoDatos = null)
        {
            List<MedioPagoEntidad> seleccionados = medios.ToList();
            bool dispose = accesoDatos == null;
            accesoDatos = accesoDatos ?? new AccesoDatosSqlServer();

            try
            {
                List<SqlParameter> parametros = new List<SqlParameter>
                {
                    accesoDatos.CreateParameter("@CuentaFinancieraId", SqlDbType.Int, cuenta.Id)
                };

                string query = @"
                    DELETE FROM CuentaFinancieraMedioPagoEntidad
                    WHERE CuentaFinancieraId = @CuentaFinancieraId;";

                for (int i = 0; i < seleccionados.Count; i++)
                {
                    query += string.Format(@"
                        INSERT INTO CuentaFinancieraMedioPagoEntidad (CuentaFinancieraId, MedioPagoEntidadId)
                        VALUES (@CuentaFinancieraId, @MedioPagoEntidadId{0});", i);

                    parametros.Add(accesoDatos.CreateParameter("@MedioPagoEntidadId" + i, SqlDbType.Int, seleccionados[i].Id));
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

        public int ContarPorMedioPagoEntidad(MedioPagoEntidad medio, AccesoDatosSqlServer accesoDatos = null)
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
                    accesoDatos.CreateParameter("@Id", SqlDbType.Int, medio.Id)
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
