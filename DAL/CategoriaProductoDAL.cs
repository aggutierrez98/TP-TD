using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using BE;

namespace DAL
{
    public class CategoriaProductoDAL
    {
        private CategoriaProducto Map(DataRow row)
        {
            return new CategoriaProducto
            {
                Id = (int)row["Id"],
                Nombre = row["Nombre"].ToString()
            };
        }

        public CategoriaProducto ObtenerPorId(int id, AccesoDatosSqlServer accesoDatos = null)
        {
            const string query = @"
                SELECT Id, Nombre
                FROM CategoriaProducto
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

        public List<CategoriaProducto> ObtenerTodas(AccesoDatosSqlServer accesoDatos = null)
        {
            const string query = @"
                SELECT Id, Nombre
                FROM CategoriaProducto
                ORDER BY Nombre";

            bool dispose = accesoDatos == null;
            accesoDatos = accesoDatos ?? new AccesoDatosSqlServer();

            try
            {
                DataTable dataTable = accesoDatos.Read(query);
                List<CategoriaProducto> categorias = new List<CategoriaProducto>();
                foreach (DataRow row in dataTable.Rows)
                {
                    categorias.Add(Map(row));
                }

                return categorias;
            }
            finally
            {
                if (dispose)
                {
                    accesoDatos.Dispose();
                }
            }
        }

        public int InsertarYObtenerId(CategoriaProducto categoria, AccesoDatosSqlServer accesoDatos = null)
        {
            const string query = @"
                INSERT INTO CategoriaProducto (Nombre)
                OUTPUT INSERTED.Id
                VALUES (@Nombre)";

            bool dispose = accesoDatos == null;
            accesoDatos = accesoDatos ?? new AccesoDatosSqlServer();

            try
            {
                List<SqlParameter> parametros = new List<SqlParameter>
                {
                    accesoDatos.CreateParameter("@Nombre", SqlDbType.VarChar, categoria.Nombre)
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

        public bool Actualizar(CategoriaProducto categoria, AccesoDatosSqlServer accesoDatos = null)
        {
            const string query = @"
                UPDATE CategoriaProducto
                SET Nombre = @Nombre
                WHERE Id = @Id";

            bool dispose = accesoDatos == null;
            accesoDatos = accesoDatos ?? new AccesoDatosSqlServer();

            try
            {
                List<SqlParameter> parametros = new List<SqlParameter>
                {
                    accesoDatos.CreateParameter("@Nombre", SqlDbType.VarChar, categoria.Nombre),
                    accesoDatos.CreateParameter("@Id", SqlDbType.Int, categoria.Id)
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

        public bool Eliminar(CategoriaProducto categoria, AccesoDatosSqlServer accesoDatos = null)
        {
            const string query = @"
                DELETE FROM CategoriaProducto
                WHERE Id = @Id";

            bool dispose = accesoDatos == null;
            accesoDatos = accesoDatos ?? new AccesoDatosSqlServer();

            try
            {
                List<SqlParameter> parametros = new List<SqlParameter>
                {
                    accesoDatos.CreateParameter("@Id", SqlDbType.Int, categoria.Id)
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
