using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using BE;

namespace DAL
{
    public class TarjetaDashboardDAL
    {
        private TarjetaDashboard Map(DataRow row)
        {
            return new TarjetaDashboard
            {
                Id = (int)row["Id"],
                Dashboard = new Dashboard { Id = (int)row["DashboardId"] },
                Tipo = (TipoTarjetaDashboard)Enum.Parse(typeof(TipoTarjetaDashboard), row["Tipo"].ToString()),
                Orden = (int)row["Orden"]
            };
        }

        public TarjetaDashboard ObtenerPorId(int id, AccesoDatosSqlServer accesoDatos = null)
        {
            const string query = @"
                SELECT Id, DashboardId, Tipo, Orden
                FROM TarjetaDashboard
                WHERE Id = @Id";

            bool dispose = accesoDatos == null;
            accesoDatos = accesoDatos ?? new AccesoDatosSqlServer();

            try
            {
                List<SqlParameter> parametros = new List<SqlParameter>
                {
                    accesoDatos.CreateParameter("@Id", SqlDbType.Int, id)
                };

                DataTable dataTable = accesoDatos.Read(query, parametros);
                return dataTable.Rows.Count == 0 ? null : Map(dataTable.Rows[0]);
            }
            finally
            {
                if (dispose)
                {
                    accesoDatos.Dispose();
                }
            }
        }

        public List<TarjetaDashboard> ObtenerPorDashboard(int dashboardId, AccesoDatosSqlServer accesoDatos = null)
        {
            const string query = @"
                SELECT Id, DashboardId, Tipo, Orden
                FROM TarjetaDashboard
                WHERE DashboardId = @DashboardId
                ORDER BY Orden";

            bool dispose = accesoDatos == null;
            accesoDatos = accesoDatos ?? new AccesoDatosSqlServer();

            try
            {
                List<SqlParameter> parametros = new List<SqlParameter>
                {
                    accesoDatos.CreateParameter("@DashboardId", SqlDbType.Int, dashboardId)
                };

                DataTable dataTable = accesoDatos.Read(query, parametros);
                List<TarjetaDashboard> tarjetas = new List<TarjetaDashboard>();
                foreach (DataRow row in dataTable.Rows)
                {
                    tarjetas.Add(Map(row));
                }

                return tarjetas;
            }
            finally
            {
                if (dispose)
                {
                    accesoDatos.Dispose();
                }
            }
        }

        public int InsertarYObtenerId(TarjetaDashboard tarjeta, AccesoDatosSqlServer accesoDatos = null)
        {
            const string query = @"
                INSERT INTO TarjetaDashboard (DashboardId, Tipo, Orden)
                OUTPUT INSERTED.Id
                VALUES (@DashboardId, @Tipo, @Orden)";

            bool dispose = accesoDatos == null;
            accesoDatos = accesoDatos ?? new AccesoDatosSqlServer();

            try
            {
                List<SqlParameter> parametros = new List<SqlParameter>
                {
                    accesoDatos.CreateParameter("@DashboardId", SqlDbType.Int, tarjeta.Dashboard.Id),
                    accesoDatos.CreateParameter("@Tipo", SqlDbType.VarChar, tarjeta.Tipo.ToString()),
                    accesoDatos.CreateParameter("@Orden", SqlDbType.Int, tarjeta.Orden)
                };

                return accesoDatos.WriteWithResult(query, parametros);
            }
            finally
            {
                if (dispose)
                {
                    accesoDatos.Dispose();
                }
            }
        }

        public bool Actualizar(TarjetaDashboard tarjeta, AccesoDatosSqlServer accesoDatos = null)
        {
            const string query = @"
                UPDATE TarjetaDashboard
                SET Tipo = @Tipo,
                    Orden = @Orden
                WHERE Id = @Id";

            bool dispose = accesoDatos == null;
            accesoDatos = accesoDatos ?? new AccesoDatosSqlServer();

            try
            {
                List<SqlParameter> parametros = new List<SqlParameter>
                {
                    accesoDatos.CreateParameter("@Tipo", SqlDbType.VarChar, tarjeta.Tipo.ToString()),
                    accesoDatos.CreateParameter("@Orden", SqlDbType.Int, tarjeta.Orden),
                    accesoDatos.CreateParameter("@Id", SqlDbType.Int, tarjeta.Id)
                };

                return accesoDatos.Write(query, parametros) > 0;
            }
            finally
            {
                if (dispose)
                {
                    accesoDatos.Dispose();
                }
            }
        }

        public bool Eliminar(int id, AccesoDatosSqlServer accesoDatos = null)
        {
            const string query = @"
                DELETE FROM TarjetaDashboard
                WHERE Id = @Id";

            bool dispose = accesoDatos == null;
            accesoDatos = accesoDatos ?? new AccesoDatosSqlServer();

            try
            {
                List<SqlParameter> parametros = new List<SqlParameter>
                {
                    accesoDatos.CreateParameter("@Id", SqlDbType.Int, id)
                };

                return accesoDatos.Write(query, parametros) > 0;
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
