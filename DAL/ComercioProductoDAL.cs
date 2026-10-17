using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using BE;

namespace DAL
{
    public class ComercioProductoDAL
    {
        private ComercioProducto Map(DataRow row)
        {
            return new ComercioProducto
            {
                Comercio = new Comercio { Id = (int)row["ComercioId"] },
                Producto = new Producto { Id = (int)row["ProductoId"] },
                Moneda = new Moneda { Id = (int)row["MonedaId"] },
                Precio = (decimal)row["Precio"]
            };
        }

        public List<ComercioProducto> ObtenerPorComercio(int comercioId, AccesoDatosSqlServer accesoDatos = null)
        {
            const string query = @"
                SELECT cp.ComercioId, cp.ProductoId, cp.MonedaId, cp.Precio
                FROM ComercioProducto cp
                INNER JOIN Producto p ON p.Id = cp.ProductoId
                WHERE cp.ComercioId = @ComercioId
                ORDER BY p.Nombre";

            bool dispose = accesoDatos == null;
            accesoDatos = accesoDatos ?? new AccesoDatosSqlServer();

            try
            {
                List<SqlParameter> parametros = new List<SqlParameter>
                {
                    accesoDatos.CreateParameter("@ComercioId", SqlDbType.Int, comercioId)
                };

                DataTable dataTable = accesoDatos.Read(query, parametros);
                List<ComercioProducto> asignaciones = new List<ComercioProducto>();
                foreach (DataRow row in dataTable.Rows)
                {
                    asignaciones.Add(Map(row));
                }

                return asignaciones;
            }
            finally
            {
                if (dispose)
                {
                    accesoDatos.Dispose();
                }
            }
        }

        public ComercioProducto ObtenerPorId(int comercioId, int productoId, AccesoDatosSqlServer accesoDatos = null)
        {
            const string query = @"
                SELECT ComercioId, ProductoId, MonedaId, Precio
                FROM ComercioProducto
                WHERE ComercioId = @ComercioId AND ProductoId = @ProductoId";

            bool dispose = accesoDatos == null;
            accesoDatos = accesoDatos ?? new AccesoDatosSqlServer();

            try
            {
                List<SqlParameter> parametros = new List<SqlParameter>
                {
                    accesoDatos.CreateParameter("@ComercioId", SqlDbType.Int, comercioId),
                    accesoDatos.CreateParameter("@ProductoId", SqlDbType.Int, productoId)
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

        public bool Insertar(ComercioProducto comercioProducto, AccesoDatosSqlServer accesoDatos = null)
        {
            const string query = @"
                INSERT INTO ComercioProducto (ComercioId, ProductoId, MonedaId, Precio)
                VALUES (@ComercioId, @ProductoId, @MonedaId, @Precio)";

            bool dispose = accesoDatos == null;
            accesoDatos = accesoDatos ?? new AccesoDatosSqlServer();

            try
            {
                List<SqlParameter> parametros = new List<SqlParameter>
                {
                    accesoDatos.CreateParameter("@ComercioId", SqlDbType.Int, comercioProducto.Comercio.Id),
                    accesoDatos.CreateParameter("@ProductoId", SqlDbType.Int, comercioProducto.Producto.Id),
                    accesoDatos.CreateParameter("@MonedaId", SqlDbType.Int, comercioProducto.Moneda.Id),
                    accesoDatos.CreateParameter("@Precio", SqlDbType.Decimal, comercioProducto.Precio)
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

        public bool ActualizarPrecio(int comercioId, int productoId, decimal precio, AccesoDatosSqlServer accesoDatos = null)
        {
            const string query = @"
                UPDATE ComercioProducto
                SET Precio = @Precio
                WHERE ComercioId = @ComercioId AND ProductoId = @ProductoId";

            bool dispose = accesoDatos == null;
            accesoDatos = accesoDatos ?? new AccesoDatosSqlServer();

            try
            {
                List<SqlParameter> parametros = new List<SqlParameter>
                {
                    accesoDatos.CreateParameter("@Precio", SqlDbType.Decimal, precio),
                    accesoDatos.CreateParameter("@ComercioId", SqlDbType.Int, comercioId),
                    accesoDatos.CreateParameter("@ProductoId", SqlDbType.Int, productoId)
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

        public bool Eliminar(int comercioId, int productoId, AccesoDatosSqlServer accesoDatos = null)
        {
            const string query = @"
                DELETE FROM ComercioProducto
                WHERE ComercioId = @ComercioId AND ProductoId = @ProductoId";

            bool dispose = accesoDatos == null;
            accesoDatos = accesoDatos ?? new AccesoDatosSqlServer();

            try
            {
                List<SqlParameter> parametros = new List<SqlParameter>
                {
                    accesoDatos.CreateParameter("@ComercioId", SqlDbType.Int, comercioId),
                    accesoDatos.CreateParameter("@ProductoId", SqlDbType.Int, productoId)
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

        public int ContarPorProducto(int productoId, AccesoDatosSqlServer accesoDatos = null)
        {
            const string query = @"
                SELECT COUNT(*) AS Total
                FROM ComercioProducto
                WHERE ProductoId = @ProductoId";

            bool dispose = accesoDatos == null;
            accesoDatos = accesoDatos ?? new AccesoDatosSqlServer();

            try
            {
                List<SqlParameter> parametros = new List<SqlParameter>
                {
                    accesoDatos.CreateParameter("@ProductoId", SqlDbType.Int, productoId)
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
