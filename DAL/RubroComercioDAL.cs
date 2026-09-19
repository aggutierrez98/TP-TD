using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using BE;

namespace DAL
{
    public class RubroComercioDAL
    {
        private RubroComercio Map(DataRow row)
        {
            return new RubroComercio
            {
                Id = (int)row["Id"],
                Nombre = row["Nombre"].ToString()
            };
        }

        public RubroComercio ObtenerPorId(int id, AccesoDatosSqlServer accesoDatos = null)
        {
            const string query = @"
                SELECT Id, Nombre
                FROM RubroComercio
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

        public List<RubroComercio> ObtenerTodas(AccesoDatosSqlServer accesoDatos = null)
        {
            const string query = @"
                SELECT Id, Nombre
                FROM RubroComercio
                ORDER BY Nombre";

            bool dispose = accesoDatos == null;
            accesoDatos = accesoDatos ?? new AccesoDatosSqlServer();

            try
            {
                DataTable dataTable = accesoDatos.Read(query);
                List<RubroComercio> rubros = new List<RubroComercio>();
                foreach (DataRow row in dataTable.Rows)
                {
                    rubros.Add(Map(row));
                }

                return rubros;
            }
            finally
            {
                if (dispose)
                {
                    accesoDatos.Dispose();
                }
            }
        }

        public int InsertarYObtenerId(RubroComercio rubro, AccesoDatosSqlServer accesoDatos = null)
        {
            const string query = @"
                INSERT INTO RubroComercio (Nombre)
                OUTPUT INSERTED.Id
                VALUES (@Nombre)";

            bool dispose = accesoDatos == null;
            accesoDatos = accesoDatos ?? new AccesoDatosSqlServer();

            try
            {
                List<SqlParameter> parametros = new List<SqlParameter>
                {
                    accesoDatos.CreateParameter("@Nombre", SqlDbType.VarChar, rubro.Nombre)
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

        public bool Actualizar(RubroComercio rubro, AccesoDatosSqlServer accesoDatos = null)
        {
            const string query = @"
                UPDATE RubroComercio
                SET Nombre = @Nombre
                WHERE Id = @Id";

            bool dispose = accesoDatos == null;
            accesoDatos = accesoDatos ?? new AccesoDatosSqlServer();

            try
            {
                List<SqlParameter> parametros = new List<SqlParameter>
                {
                    accesoDatos.CreateParameter("@Nombre", SqlDbType.VarChar, rubro.Nombre),
                    accesoDatos.CreateParameter("@Id", SqlDbType.Int, rubro.Id)
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
                DELETE FROM RubroComercio
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
