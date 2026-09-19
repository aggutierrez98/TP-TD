using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using BE;

namespace DAL
{
    public class MonedaDAL
    {
        private Moneda Map(DataRow row)
        {
            return new Moneda
            {
                Id = (int)row["Id"],
                Codigo = row["Codigo"].ToString(),
                Nombre = row["Nombre"].ToString()
            };
        }

        public Moneda ObtenerPorId(int id, AccesoDatosSqlServer accesoDatos = null)
        {
            const string query = @"
                SELECT Id, Codigo, Nombre
                FROM Moneda
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

        public List<Moneda> ObtenerTodas(AccesoDatosSqlServer accesoDatos = null)
        {
            const string query = @"
                SELECT Id, Codigo, Nombre
                FROM Moneda
                ORDER BY Nombre";

            bool dispose = accesoDatos == null;
            accesoDatos = accesoDatos ?? new AccesoDatosSqlServer();

            try
            {
                DataTable dataTable = accesoDatos.Read(query);
                List<Moneda> monedas = new List<Moneda>();
                foreach (DataRow row in dataTable.Rows)
                {
                    monedas.Add(Map(row));
                }

                return monedas;
            }
            finally
            {
                if (dispose)
                {
                    accesoDatos.Dispose();
                }
            }
        }

        public int InsertarYObtenerId(Moneda moneda, AccesoDatosSqlServer accesoDatos = null)
        {
            const string query = @"
                INSERT INTO Moneda (Codigo, Nombre)
                OUTPUT INSERTED.Id
                VALUES (@Codigo, @Nombre)";

            bool dispose = accesoDatos == null;
            accesoDatos = accesoDatos ?? new AccesoDatosSqlServer();

            try
            {
                List<SqlParameter> parametros = new List<SqlParameter>
                {
                    accesoDatos.CreateParameter("@Codigo", SqlDbType.VarChar, moneda.Codigo),
                    accesoDatos.CreateParameter("@Nombre", SqlDbType.VarChar, moneda.Nombre)
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

        public bool Actualizar(Moneda moneda, AccesoDatosSqlServer accesoDatos = null)
        {
            const string query = @"
                UPDATE Moneda
                SET Codigo = @Codigo,
                    Nombre = @Nombre
                WHERE Id = @Id";

            bool dispose = accesoDatos == null;
            accesoDatos = accesoDatos ?? new AccesoDatosSqlServer();

            try
            {
                List<SqlParameter> parametros = new List<SqlParameter>
                {
                    accesoDatos.CreateParameter("@Codigo", SqlDbType.VarChar, moneda.Codigo),
                    accesoDatos.CreateParameter("@Nombre", SqlDbType.VarChar, moneda.Nombre),
                    accesoDatos.CreateParameter("@Id", SqlDbType.Int, moneda.Id)
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
                DELETE FROM Moneda
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
