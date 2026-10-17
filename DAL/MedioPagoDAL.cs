using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using BE;

namespace DAL
{
    public class MedioPagoDAL
    {
        private MedioPago Map(DataRow row)
        {
            return new MedioPago
            {
                Id = (int)row["Id"],
                Tipo = (TipoMedioPago)Enum.Parse(typeof(TipoMedioPago), row["Tipo"].ToString()),
                Marca = row.IsNull("Marca") ? null : row["Marca"].ToString()
            };
        }

        public MedioPago ObtenerPorId(int id, AccesoDatosSqlServer accesoDatos = null)
        {
            const string query = @"
                SELECT Id, Tipo, Marca
                FROM MedioPago
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

        public List<MedioPago> ObtenerTodos(AccesoDatosSqlServer accesoDatos = null)
        {
            const string query = @"
                SELECT Id, Tipo, Marca
                FROM MedioPago
                ORDER BY Tipo, Marca";

            bool dispose = accesoDatos == null;
            accesoDatos = accesoDatos ?? new AccesoDatosSqlServer();

            try
            {
                DataTable dataTable = accesoDatos.Read(query);
                List<MedioPago> mediosPago = new List<MedioPago>();
                foreach (DataRow row in dataTable.Rows)
                {
                    mediosPago.Add(Map(row));
                }

                return mediosPago;
            }
            finally
            {
                if (dispose)
                {
                    accesoDatos.Dispose();
                }
            }
        }

        public int InsertarYObtenerId(MedioPago medioPago, AccesoDatosSqlServer accesoDatos = null)
        {
            const string query = @"
                INSERT INTO MedioPago (Tipo, Marca)
                OUTPUT INSERTED.Id
                VALUES (@Tipo, @Marca)";

            bool dispose = accesoDatos == null;
            accesoDatos = accesoDatos ?? new AccesoDatosSqlServer();

            try
            {
                List<SqlParameter> parametros = new List<SqlParameter>
                {
                    accesoDatos.CreateParameter("@Tipo", SqlDbType.VarChar, medioPago.Tipo.ToString()),
                    accesoDatos.CreateParameter("@Marca", SqlDbType.VarChar, medioPago.Marca)
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

        public bool Actualizar(MedioPago medioPago, AccesoDatosSqlServer accesoDatos = null)
        {
            const string query = @"
                UPDATE MedioPago
                SET Tipo = @Tipo,
                    Marca = @Marca
                WHERE Id = @Id";

            bool dispose = accesoDatos == null;
            accesoDatos = accesoDatos ?? new AccesoDatosSqlServer();

            try
            {
                List<SqlParameter> parametros = new List<SqlParameter>
                {
                    accesoDatos.CreateParameter("@Tipo", SqlDbType.VarChar, medioPago.Tipo.ToString()),
                    accesoDatos.CreateParameter("@Marca", SqlDbType.VarChar, medioPago.Marca),
                    accesoDatos.CreateParameter("@Id", SqlDbType.Int, medioPago.Id)
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
                DELETE FROM MedioPago
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
