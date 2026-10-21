using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using BE;

namespace DAL
{
    public class PromocionCategoriaProductoDAL
    {
        public List<CategoriaProducto> ObtenerCategoriasProductoPorPromocion(Promocion promocion, bool excluidos, AccesoDatosSqlServer accesoDatos = null)
        {
            const string query = @"
                SELECT CategoriaProductoId
                FROM PromocionCategoriaProducto
                WHERE PromocionId = @PromocionId AND Excluido = @Excluido
                ORDER BY CategoriaProductoId";

            bool dispose = accesoDatos == null;
            accesoDatos = accesoDatos ?? new AccesoDatosSqlServer();

            try
            {
                List<SqlParameter> parametros = new List<SqlParameter>
                {
                    accesoDatos.CreateParameter("@PromocionId", SqlDbType.Int, promocion.Id),
                    accesoDatos.CreateParameter("@Excluido", SqlDbType.Bit, excluidos)
                };

                DataTable dataTable = accesoDatos.Read(query, parametros);
                return dataTable.Rows.Cast<DataRow>()
                    .Select(row => new CategoriaProducto { Id = (int)row["CategoriaProductoId"] })
                    .ToList();
            }
            finally
            {
                if (dispose)
                {
                    accesoDatos.Dispose();
                }
            }
        }

        // Un mismo elemento no puede ir incluido y excluido a la vez: la PK (PromocionId, CategoriaProductoId)
        // rechaza el segundo INSERT y la transaccion completa se revierte.
        public bool ReemplazarCategoriasProductoDePromocion(Promocion promocion, List<CategoriaProducto> incluidos, List<CategoriaProducto> excluidos, AccesoDatosSqlServer accesoDatos = null)
        {
            bool dispose = accesoDatos == null;
            accesoDatos = accesoDatos ?? new AccesoDatosSqlServer();

            try
            {
                List<SqlParameter> parametros = new List<SqlParameter>
                {
                    accesoDatos.CreateParameter("@PromocionId", SqlDbType.Int, promocion.Id)
                };

                string query = @"
                    DELETE FROM PromocionCategoriaProducto
                    WHERE PromocionId = @PromocionId;";

                for (int i = 0; i < incluidos.Count; i++)
                {
                    query += string.Format(@"
                        INSERT INTO PromocionCategoriaProducto (PromocionId, CategoriaProductoId, Excluido)
                        VALUES (@PromocionId, @IncluidoId{0}, 0);", i);

                    parametros.Add(accesoDatos.CreateParameter("@IncluidoId" + i, SqlDbType.Int, incluidos[i].Id));
                }

                for (int i = 0; i < excluidos.Count; i++)
                {
                    query += string.Format(@"
                        INSERT INTO PromocionCategoriaProducto (PromocionId, CategoriaProductoId, Excluido)
                        VALUES (@PromocionId, @ExcluidoId{0}, 1);", i);

                    parametros.Add(accesoDatos.CreateParameter("@ExcluidoId" + i, SqlDbType.Int, excluidos[i].Id));
                }

                return accesoDatos.Write(query, parametros) >= 0;
            }
            finally
            {
                if (dispose)
                {
                    accesoDatos.Dispose();
                }
            }
        }

        public int ContarPorCategoriaProducto(CategoriaProducto categoriaProducto, AccesoDatosSqlServer accesoDatos = null)
        {
            const string query = @"
                SELECT COUNT(*) AS Total
                FROM PromocionCategoriaProducto
                WHERE CategoriaProductoId = @Id";

            bool dispose = accesoDatos == null;
            accesoDatos = accesoDatos ?? new AccesoDatosSqlServer();

            try
            {
                List<SqlParameter> parametros = new List<SqlParameter>
                {
                    accesoDatos.CreateParameter("@Id", SqlDbType.Int, categoriaProducto.Id)
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
