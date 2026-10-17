using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace DAL
{
    public class PromocionCategoriaProductoDAL
    {
        public List<int> ObtenerCategoriaProductoIdsPorPromocion(int promocionId, bool excluidos, AccesoDatosSqlServer accesoDatos = null)
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
                    accesoDatos.CreateParameter("@PromocionId", SqlDbType.Int, promocionId),
                    accesoDatos.CreateParameter("@Excluido", SqlDbType.Bit, excluidos)
                };

                DataTable dataTable = accesoDatos.Read(query, parametros);
                return dataTable.Rows.Cast<DataRow>()
                    .Select(row => (int)row["CategoriaProductoId"])
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
        public bool ReemplazarCategoriasProductoDePromocion(int promocionId, List<int> incluidos, List<int> excluidos, AccesoDatosSqlServer accesoDatos = null)
        {
            bool dispose = accesoDatos == null;
            accesoDatos = accesoDatos ?? new AccesoDatosSqlServer();

            try
            {
                List<SqlParameter> parametros = new List<SqlParameter>
                {
                    accesoDatos.CreateParameter("@PromocionId", SqlDbType.Int, promocionId)
                };

                string query = @"
                    DELETE FROM PromocionCategoriaProducto
                    WHERE PromocionId = @PromocionId;";

                for (int i = 0; i < incluidos.Count; i++)
                {
                    query += string.Format(@"
                        INSERT INTO PromocionCategoriaProducto (PromocionId, CategoriaProductoId, Excluido)
                        VALUES (@PromocionId, @IncluidoId{0}, 0);", i);

                    parametros.Add(accesoDatos.CreateParameter("@IncluidoId" + i, SqlDbType.Int, incluidos[i]));
                }

                for (int i = 0; i < excluidos.Count; i++)
                {
                    query += string.Format(@"
                        INSERT INTO PromocionCategoriaProducto (PromocionId, CategoriaProductoId, Excluido)
                        VALUES (@PromocionId, @ExcluidoId{0}, 1);", i);

                    parametros.Add(accesoDatos.CreateParameter("@ExcluidoId" + i, SqlDbType.Int, excluidos[i]));
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

        public int ContarPorCategoriaProducto(int categoriaProductoId, AccesoDatosSqlServer accesoDatos = null)
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
                    accesoDatos.CreateParameter("@Id", SqlDbType.Int, categoriaProductoId)
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
