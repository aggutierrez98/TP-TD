using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using BE;

namespace DAL
{
    public class ItemListaDAL
    {
        private ItemLista Map(DataRow row)
        {
            return new ItemLista
            {
                Id = (int)row["Id"],
                ListaDeCompras = new ListaDeCompras { Id = (int)row["ListaDeComprasId"] },
                Descripcion = (string)row["Descripcion"],
                Cantidad = (int)row["Cantidad"],
                Comprado = (bool)row["Comprado"]
            };
        }

        public ItemLista ObtenerPorId(int id, AccesoDatosSqlServer accesoDatos = null)
        {
            const string query = @"
                SELECT Id, ListaDeComprasId, Descripcion, Cantidad, Comprado
                FROM ItemLista
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

        public List<ItemLista> ObtenerPorListaDeCompras(ListaDeCompras lista, AccesoDatosSqlServer accesoDatos = null)
        {
            const string query = @"
                SELECT Id, ListaDeComprasId, Descripcion, Cantidad, Comprado
                FROM ItemLista
                WHERE ListaDeComprasId = @ListaDeComprasId
                ORDER BY Id";

            bool dispose = accesoDatos == null;
            accesoDatos = accesoDatos ?? new AccesoDatosSqlServer();

            try
            {
                List<SqlParameter> parametros = new List<SqlParameter>
                {
                    accesoDatos.CreateParameter("@ListaDeComprasId", SqlDbType.Int, lista.Id)
                };

                DataTable dataTable = accesoDatos.Read(query, parametros);
                List<ItemLista> items = new List<ItemLista>();
                foreach (DataRow row in dataTable.Rows)
                {
                    items.Add(Map(row));
                }

                return items;
            }
            finally
            {
                if (dispose)
                {
                    accesoDatos.Dispose();
                }
            }
        }

        public int InsertarYObtenerId(ItemLista itemLista, AccesoDatosSqlServer accesoDatos = null)
        {
            const string query = @"
                INSERT INTO ItemLista (ListaDeComprasId, Descripcion, Cantidad, Comprado)
                OUTPUT INSERTED.Id
                VALUES (@ListaDeComprasId, @Descripcion, @Cantidad, @Comprado)";

            bool dispose = accesoDatos == null;
            accesoDatos = accesoDatos ?? new AccesoDatosSqlServer();

            try
            {
                List<SqlParameter> parametros = new List<SqlParameter>
                {
                    accesoDatos.CreateParameter("@ListaDeComprasId", SqlDbType.Int, itemLista.ListaDeCompras.Id),
                    accesoDatos.CreateParameter("@Descripcion", SqlDbType.VarChar, itemLista.Descripcion),
                    accesoDatos.CreateParameter("@Cantidad", SqlDbType.Int, itemLista.Cantidad),
                    accesoDatos.CreateParameter("@Comprado", SqlDbType.Bit, itemLista.Comprado)
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

        public bool Actualizar(ItemLista itemLista, AccesoDatosSqlServer accesoDatos = null)
        {
            const string query = @"
                UPDATE ItemLista
                SET Descripcion = @Descripcion,
                    Cantidad = @Cantidad,
                    Comprado = @Comprado
                WHERE Id = @Id";

            bool dispose = accesoDatos == null;
            accesoDatos = accesoDatos ?? new AccesoDatosSqlServer();

            try
            {
                List<SqlParameter> parametros = new List<SqlParameter>
                {
                    accesoDatos.CreateParameter("@Descripcion", SqlDbType.VarChar, itemLista.Descripcion),
                    accesoDatos.CreateParameter("@Cantidad", SqlDbType.Int, itemLista.Cantidad),
                    accesoDatos.CreateParameter("@Comprado", SqlDbType.Bit, itemLista.Comprado),
                    accesoDatos.CreateParameter("@Id", SqlDbType.Int, itemLista.Id)
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

        public bool Eliminar(ItemLista item, AccesoDatosSqlServer accesoDatos = null)
        {
            const string query = @"
                DELETE FROM ItemLista
                WHERE Id = @Id";

            bool dispose = accesoDatos == null;
            accesoDatos = accesoDatos ?? new AccesoDatosSqlServer();

            try
            {
                List<SqlParameter> parametros = new List<SqlParameter>
                {
                    accesoDatos.CreateParameter("@Id", SqlDbType.Int, item.Id)
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
