using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using BE;

namespace DAL
{
    public class MedioDePagoDAL
    {
        private MedioDePago Map(DataRow row)
        {
            return new MedioDePago
            {
                Id = (int)row["Id"],
                EntidadFinanciera = new EntidadFinanciera { Id = (int)row["EntidadFinancieraId"] },
                Tipo = (TipoMedioDePago)Enum.Parse(typeof(TipoMedioDePago), row["Tipo"].ToString()),
                Marca = row.IsNull("Marca") ? null : row["Marca"].ToString(),
                Segmento = row.IsNull("Segmento") ? null : row["Segmento"].ToString()
            };
        }

        public MedioDePago ObtenerPorId(int id, AccesoDatosSqlServer accesoDatos = null)
        {
            const string query = @"
                SELECT Id, EntidadFinancieraId, Tipo, Marca, Segmento
                FROM MedioDePago
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

        public List<MedioDePago> ObtenerPorEntidadFinanciera(int entidadFinancieraId, AccesoDatosSqlServer accesoDatos = null)
        {
            const string query = @"
                SELECT Id, EntidadFinancieraId, Tipo, Marca, Segmento
                FROM MedioDePago
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

                DataTable dataTable = accesoDatos.Read(query, parametros);
                List<MedioDePago> mediosDePago = new List<MedioDePago>();
                foreach (DataRow row in dataTable.Rows)
                {
                    mediosDePago.Add(Map(row));
                }

                return mediosDePago;
            }
            finally
            {
                if (dispose)
                {
                    accesoDatos.Dispose();
                }
            }
        }

        public int InsertarYObtenerId(MedioDePago medioDePago, AccesoDatosSqlServer accesoDatos = null)
        {
            const string query = @"
                INSERT INTO MedioDePago (EntidadFinancieraId, Tipo, Marca, Segmento)
                OUTPUT INSERTED.Id
                VALUES (@EntidadFinancieraId, @Tipo, @Marca, @Segmento)";

            bool dispose = accesoDatos == null;
            accesoDatos = accesoDatos ?? new AccesoDatosSqlServer();

            try
            {
                List<SqlParameter> parametros = new List<SqlParameter>
                {
                    accesoDatos.CreateParameter("@EntidadFinancieraId", SqlDbType.Int, medioDePago.EntidadFinanciera.Id),
                    accesoDatos.CreateParameter("@Tipo", SqlDbType.VarChar, medioDePago.Tipo.ToString()),
                    accesoDatos.CreateParameter("@Marca", SqlDbType.VarChar, medioDePago.Marca),
                    accesoDatos.CreateParameter("@Segmento", SqlDbType.VarChar, medioDePago.Segmento)
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

        public bool Actualizar(MedioDePago medioDePago, AccesoDatosSqlServer accesoDatos = null)
        {
            const string query = @"
                UPDATE MedioDePago
                SET Tipo = @Tipo,
                    Marca = @Marca,
                    Segmento = @Segmento
                WHERE Id = @Id";

            bool dispose = accesoDatos == null;
            accesoDatos = accesoDatos ?? new AccesoDatosSqlServer();

            try
            {
                List<SqlParameter> parametros = new List<SqlParameter>
                {
                    accesoDatos.CreateParameter("@Tipo", SqlDbType.VarChar, medioDePago.Tipo.ToString()),
                    accesoDatos.CreateParameter("@Marca", SqlDbType.VarChar, medioDePago.Marca),
                    accesoDatos.CreateParameter("@Segmento", SqlDbType.VarChar, medioDePago.Segmento),
                    accesoDatos.CreateParameter("@Id", SqlDbType.Int, medioDePago.Id)
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
                DELETE FROM MedioDePago
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
