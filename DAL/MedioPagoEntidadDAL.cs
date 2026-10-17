using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using BE;

namespace DAL
{
    public class MedioPagoEntidadDAL
    {
        // EntidadFinanciera y MedioPago quedan como referencias livianas (solo Id): se
        // hidratan en REPOSITORIO.
        private MedioPagoEntidad Map(DataRow row)
        {
            return new MedioPagoEntidad
            {
                Id = (int)row["Id"],
                EntidadFinanciera = new EntidadFinanciera { Id = (int)row["EntidadFinancieraId"] },
                MedioPago = new MedioPago { Id = (int)row["MedioPagoId"] },
                Segmento = row.IsNull("Segmento") ? null : row["Segmento"].ToString()
            };
        }

        public MedioPagoEntidad ObtenerPorId(int id, AccesoDatosSqlServer accesoDatos = null)
        {
            const string query = @"
                SELECT Id, EntidadFinancieraId, MedioPagoId, Segmento
                FROM MedioPagoEntidad
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

        public List<MedioPagoEntidad> ObtenerPorEntidadFinanciera(int entidadFinancieraId, AccesoDatosSqlServer accesoDatos = null)
        {
            const string query = @"
                SELECT Id, EntidadFinancieraId, MedioPagoId, Segmento
                FROM MedioPagoEntidad
                WHERE EntidadFinancieraId = @EntidadFinancieraId
                ORDER BY Id";

            bool dispose = accesoDatos == null;
            accesoDatos = accesoDatos ?? new AccesoDatosSqlServer();

            try
            {
                List<SqlParameter> parametros = new List<SqlParameter>
                {
                    accesoDatos.CreateParameter("@EntidadFinancieraId", SqlDbType.Int, entidadFinancieraId)
                };

                return MapearTodos(accesoDatos.Read(query, parametros));
            }
            finally
            {
                if (dispose)
                {
                    accesoDatos.Dispose();
                }
            }
        }

        public List<MedioPagoEntidad> ObtenerPorMedioPago(int medioPagoId, AccesoDatosSqlServer accesoDatos = null)
        {
            const string query = @"
                SELECT Id, EntidadFinancieraId, MedioPagoId, Segmento
                FROM MedioPagoEntidad
                WHERE MedioPagoId = @MedioPagoId
                ORDER BY Id";

            bool dispose = accesoDatos == null;
            accesoDatos = accesoDatos ?? new AccesoDatosSqlServer();

            try
            {
                List<SqlParameter> parametros = new List<SqlParameter>
                {
                    accesoDatos.CreateParameter("@MedioPagoId", SqlDbType.Int, medioPagoId)
                };

                return MapearTodos(accesoDatos.Read(query, parametros));
            }
            finally
            {
                if (dispose)
                {
                    accesoDatos.Dispose();
                }
            }
        }

        public List<string> ObtenerSegmentosDistintos(AccesoDatosSqlServer accesoDatos = null)
        {
            const string query = @"
                SELECT DISTINCT Segmento
                FROM MedioPagoEntidad
                WHERE Segmento IS NOT NULL
                ORDER BY Segmento";

            bool dispose = accesoDatos == null;
            accesoDatos = accesoDatos ?? new AccesoDatosSqlServer();

            try
            {
                DataTable dataTable = accesoDatos.Read(query, new List<SqlParameter>());
                List<string> segmentos = new List<string>();
                foreach (DataRow row in dataTable.Rows)
                {
                    segmentos.Add(row["Segmento"].ToString());
                }

                return segmentos;
            }
            finally
            {
                if (dispose)
                {
                    accesoDatos.Dispose();
                }
            }
        }

        public int InsertarYObtenerId(MedioPagoEntidad medioPagoEntidad, AccesoDatosSqlServer accesoDatos = null)
        {
            const string query = @"
                INSERT INTO MedioPagoEntidad (EntidadFinancieraId, MedioPagoId, Segmento)
                OUTPUT INSERTED.Id
                VALUES (@EntidadFinancieraId, @MedioPagoId, @Segmento)";

            bool dispose = accesoDatos == null;
            accesoDatos = accesoDatos ?? new AccesoDatosSqlServer();

            try
            {
                List<SqlParameter> parametros = new List<SqlParameter>
                {
                    accesoDatos.CreateParameter("@EntidadFinancieraId", SqlDbType.Int, medioPagoEntidad.EntidadFinanciera.Id),
                    accesoDatos.CreateParameter("@MedioPagoId", SqlDbType.Int, medioPagoEntidad.MedioPago.Id),
                    accesoDatos.CreateParameter("@Segmento", SqlDbType.VarChar, medioPagoEntidad.Segmento)
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

        public bool Actualizar(MedioPagoEntidad medioPagoEntidad, AccesoDatosSqlServer accesoDatos = null)
        {
            const string query = @"
                UPDATE MedioPagoEntidad
                SET MedioPagoId = @MedioPagoId,
                    Segmento = @Segmento
                WHERE Id = @Id";

            bool dispose = accesoDatos == null;
            accesoDatos = accesoDatos ?? new AccesoDatosSqlServer();

            try
            {
                List<SqlParameter> parametros = new List<SqlParameter>
                {
                    accesoDatos.CreateParameter("@MedioPagoId", SqlDbType.Int, medioPagoEntidad.MedioPago.Id),
                    accesoDatos.CreateParameter("@Segmento", SqlDbType.VarChar, medioPagoEntidad.Segmento),
                    accesoDatos.CreateParameter("@Id", SqlDbType.Int, medioPagoEntidad.Id)
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
                DELETE FROM MedioPagoEntidad
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

        private List<MedioPagoEntidad> MapearTodos(DataTable dataTable)
        {
            List<MedioPagoEntidad> mediosPagoEntidad = new List<MedioPagoEntidad>();
            foreach (DataRow row in dataTable.Rows)
            {
                mediosPagoEntidad.Add(Map(row));
            }

            return mediosPagoEntidad;
        }
    }
}
