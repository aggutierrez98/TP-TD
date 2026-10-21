using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using BE;

namespace DAL
{
    public class EntidadFinancieraDAL
    {
        private EntidadFinanciera Map(DataRow row)
        {
            return new EntidadFinanciera
            {
                Id = (int)row["Id"],
                Nombre = row["Nombre"].ToString()
            };
        }

        public EntidadFinanciera ObtenerPorId(int id, AccesoDatosSqlServer accesoDatos = null)
        {
            const string query = @"
                SELECT Id, Nombre
                FROM EntidadFinanciera
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

        public List<EntidadFinanciera> ObtenerTodas(AccesoDatosSqlServer accesoDatos = null)
        {
            const string query = @"
                SELECT Id, Nombre
                FROM EntidadFinanciera
                ORDER BY Nombre";

            bool dispose = accesoDatos == null;
            accesoDatos = accesoDatos ?? new AccesoDatosSqlServer();

            try
            {
                DataTable dataTable = accesoDatos.Read(query);
                List<EntidadFinanciera> entidades = new List<EntidadFinanciera>();
                foreach (DataRow row in dataTable.Rows)
                {
                    entidades.Add(Map(row));
                }

                return entidades;
            }
            finally
            {
                if (dispose)
                {
                    accesoDatos.Dispose();
                }
            }
        }

        public int InsertarYObtenerId(EntidadFinanciera entidad, AccesoDatosSqlServer accesoDatos = null)
        {
            const string query = @"
                INSERT INTO EntidadFinanciera (Nombre)
                OUTPUT INSERTED.Id
                VALUES (@Nombre)";

            bool dispose = accesoDatos == null;
            accesoDatos = accesoDatos ?? new AccesoDatosSqlServer();

            try
            {
                List<SqlParameter> parametros = new List<SqlParameter>
                {
                    accesoDatos.CreateParameter("@Nombre", SqlDbType.VarChar, entidad.Nombre)
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

        public bool Actualizar(EntidadFinanciera entidad, AccesoDatosSqlServer accesoDatos = null)
        {
            const string query = @"
                UPDATE EntidadFinanciera
                SET Nombre = @Nombre
                WHERE Id = @Id";

            bool dispose = accesoDatos == null;
            accesoDatos = accesoDatos ?? new AccesoDatosSqlServer();

            try
            {
                List<SqlParameter> parametros = new List<SqlParameter>
                {
                    accesoDatos.CreateParameter("@Nombre", SqlDbType.VarChar, entidad.Nombre),
                    accesoDatos.CreateParameter("@Id", SqlDbType.Int, entidad.Id)
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

        public bool Eliminar(EntidadFinanciera entidad, AccesoDatosSqlServer accesoDatos = null)
        {
            const string query = @"
                DELETE FROM EntidadFinanciera
                WHERE Id = @Id";

            bool dispose = accesoDatos == null;
            accesoDatos = accesoDatos ?? new AccesoDatosSqlServer();

            try
            {
                List<SqlParameter> parametros = new List<SqlParameter>
                {
                    accesoDatos.CreateParameter("@Id", SqlDbType.Int, entidad.Id)
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
