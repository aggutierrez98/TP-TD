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
                Comercio = new Comercio { Id = (int)row["ComercioId"] },
                CategoriaProducto = new CategoriaProducto { Id = (int)row["CategoriaProductoId"] },
                Moneda = new Moneda { Id = (int)row["MonedaId"] },
                Nombre = row["Nombre"].ToString(),
                Precio = (decimal)row["Precio"]
            };
        }

        public Producto ObtenerPorId(int id, AccesoDatosSqlServer accesoDatos = null)
        {
            const string query = @"
                SELECT Id, ComercioId, CategoriaProductoId, MonedaId, Nombre, Precio
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

        public List<Producto> ObtenerPorComercio(int comercioId, AccesoDatosSqlServer accesoDatos = null)
        {
            const string query = @"
                SELECT Id, ComercioId, CategoriaProductoId, MonedaId, Nombre, Precio
                FROM Producto
                WHERE ComercioId = @ComercioId
                ORDER BY Nombre";

            bool dispose = accesoDatos == null;
            accesoDatos = accesoDatos ?? new AccesoDatosSqlServer();

            try
            {
                List<SqlParameter> parametros = new List<SqlParameter>
                {
                    accesoDatos.CreateParameter("@ComercioId", SqlDbType.Int, comercioId)
                };

                DataTable dataTable = accesoDatos.Read(query, parametros);
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
                INSERT INTO Producto (ComercioId, CategoriaProductoId, MonedaId, Nombre, Precio)
                OUTPUT INSERTED.Id
                VALUES (@ComercioId, @CategoriaProductoId, @MonedaId, @Nombre, @Precio)";

            bool dispose = accesoDatos == null;
            accesoDatos = accesoDatos ?? new AccesoDatosSqlServer();

            try
            {
                List<SqlParameter> parametros = new List<SqlParameter>
                {
                    accesoDatos.CreateParameter("@ComercioId", SqlDbType.Int, producto.Comercio.Id),
                    accesoDatos.CreateParameter("@CategoriaProductoId", SqlDbType.Int, producto.CategoriaProducto.Id),
                    accesoDatos.CreateParameter("@MonedaId", SqlDbType.Int, producto.Moneda.Id),
                    accesoDatos.CreateParameter("@Nombre", SqlDbType.VarChar, producto.Nombre),
                    accesoDatos.CreateParameter("@Precio", SqlDbType.Decimal, producto.Precio)
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
                    MonedaId = @MonedaId,
                    Nombre = @Nombre,
                    Precio = @Precio
                WHERE Id = @Id";

            bool dispose = accesoDatos == null;
            accesoDatos = accesoDatos ?? new AccesoDatosSqlServer();

            try
            {
                List<SqlParameter> parametros = new List<SqlParameter>
                {
                    accesoDatos.CreateParameter("@CategoriaProductoId", SqlDbType.Int, producto.CategoriaProducto.Id),
                    accesoDatos.CreateParameter("@MonedaId", SqlDbType.Int, producto.Moneda.Id),
                    accesoDatos.CreateParameter("@Nombre", SqlDbType.VarChar, producto.Nombre),
                    accesoDatos.CreateParameter("@Precio", SqlDbType.Decimal, producto.Precio),
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

        public bool Eliminar(int id, AccesoDatosSqlServer accesoDatos = null)
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
