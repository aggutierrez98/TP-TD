using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using BE;

namespace DAL
{
    public class ProductoDAL
    {
        private Producto Map(DataRow row)
        {
            return new Producto
            {
                Id = (int)row["Id"],
                CategoriaProducto = new CategoriaProducto { Id = (int)row["CategoriaProductoId"] },
                Nombre = row["Nombre"].ToString()
            };
        }

        public Producto ObtenerPorId(int id, AccesoDatosSqlServer accesoDatos = null)
        {
            const string query = @"
                SELECT Id, CategoriaProductoId, Nombre
                FROM Producto
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

        public List<Producto> ObtenerTodos(AccesoDatosSqlServer accesoDatos = null)
        {
            const string query = @"
                SELECT Id, CategoriaProductoId, Nombre
                FROM Producto
                ORDER BY Nombre";

            bool dispose = accesoDatos == null;
            accesoDatos = accesoDatos ?? new AccesoDatosSqlServer();

            try
            {
                DataTable dataTable = accesoDatos.Read(query, new List<SqlParameter>());
                List<Producto> productos = new List<Producto>();
                foreach (DataRow row in dataTable.Rows)
                {
                    productos.Add(Map(row));
                }

                return productos;
            }
            finally
            {
                if (dispose)
                {
                    accesoDatos.Dispose();
                }
            }
        }

        public int InsertarYObtenerId(Producto producto, AccesoDatosSqlServer accesoDatos = null)
        {
            const string query = @"
                INSERT INTO Producto (CategoriaProductoId, Nombre)
                OUTPUT INSERTED.Id
                VALUES (@CategoriaProductoId, @Nombre)";

            bool dispose = accesoDatos == null;
            accesoDatos = accesoDatos ?? new AccesoDatosSqlServer();

            try
            {
                List<SqlParameter> parametros = new List<SqlParameter>
                {
                    accesoDatos.CreateParameter("@CategoriaProductoId", SqlDbType.Int, producto.CategoriaProducto.Id),
                    accesoDatos.CreateParameter("@Nombre", SqlDbType.VarChar, producto.Nombre)
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

        public bool Actualizar(Producto producto, AccesoDatosSqlServer accesoDatos = null)
        {
            const string query = @"
                UPDATE Producto
                SET CategoriaProductoId = @CategoriaProductoId,
                    Nombre = @Nombre
                WHERE Id = @Id";

            bool dispose = accesoDatos == null;
            accesoDatos = accesoDatos ?? new AccesoDatosSqlServer();

            try
            {
                List<SqlParameter> parametros = new List<SqlParameter>
                {
                    accesoDatos.CreateParameter("@CategoriaProductoId", SqlDbType.Int, producto.CategoriaProducto.Id),
                    accesoDatos.CreateParameter("@Nombre", SqlDbType.VarChar, producto.Nombre),
                    accesoDatos.CreateParameter("@Id", SqlDbType.Int, producto.Id)
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

        public bool Eliminar(Producto producto, AccesoDatosSqlServer accesoDatos = null)
        {
            const string query = @"
                DELETE FROM Producto
                WHERE Id = @Id";

            bool dispose = accesoDatos == null;
            accesoDatos = accesoDatos ?? new AccesoDatosSqlServer();

            try
            {
                List<SqlParameter> parametros = new List<SqlParameter>
                {
                    accesoDatos.CreateParameter("@Id", SqlDbType.Int, producto.Id)
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

        public int ContarPorCategoria(CategoriaProducto categoria, AccesoDatosSqlServer accesoDatos = null)
        {
            const string query = @"
                SELECT COUNT(*) AS Total
                FROM Producto
                WHERE CategoriaProductoId = @Id";

            bool dispose = accesoDatos == null;
            accesoDatos = accesoDatos ?? new AccesoDatosSqlServer();

            try
            {
                List<SqlParameter> parametros = new List<SqlParameter>
                {
                    accesoDatos.CreateParameter("@Id", SqlDbType.Int, categoria.Id)
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
