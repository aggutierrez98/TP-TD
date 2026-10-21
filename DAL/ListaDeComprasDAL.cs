using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using BE;

namespace DAL
{
    public class ListaDeComprasDAL
    {
        private ListaDeCompras Map(DataRow row)
        {
            return new ListaDeCompras
            {
                Id = (int)row["Id"],
                Usuario = new Usuario { Id = (int)row["UsuarioId"] },
                Nombre = row["Nombre"].ToString(),
                FechaCreacion = (System.DateTime)row["FechaCreacion"]
            };
        }

        public ListaDeCompras ObtenerPorId(int id, AccesoDatosSqlServer accesoDatos = null)
        {
            const string query = @"
                SELECT Id, UsuarioId, Nombre, FechaCreacion
                FROM ListaDeCompras
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

        public List<ListaDeCompras> ObtenerPorUsuario(Usuario usuario, AccesoDatosSqlServer accesoDatos = null)
        {
            const string query = @"
                SELECT Id, UsuarioId, Nombre, FechaCreacion
                FROM ListaDeCompras
                WHERE UsuarioId = @UsuarioId
                ORDER BY FechaCreacion DESC";

            bool dispose = accesoDatos == null;
            accesoDatos = accesoDatos ?? new AccesoDatosSqlServer();

            try
            {
                List<SqlParameter> parametros = new List<SqlParameter>
                {
                    accesoDatos.CreateParameter("@UsuarioId", SqlDbType.Int, usuario.Id)
                };

                DataTable dataTable = accesoDatos.Read(query, parametros);
                List<ListaDeCompras> listas = new List<ListaDeCompras>();
                foreach (DataRow row in dataTable.Rows)
                {
                    listas.Add(Map(row));
                }

                return listas;
            }
            finally
            {
                if (dispose)
                {
                    accesoDatos.Dispose();
                }
            }
        }

        public int InsertarYObtenerId(ListaDeCompras listaDeCompras, AccesoDatosSqlServer accesoDatos = null)
        {
            const string query = @"
                INSERT INTO ListaDeCompras (UsuarioId, Nombre, FechaCreacion)
                OUTPUT INSERTED.Id
                VALUES (@UsuarioId, @Nombre, @FechaCreacion)";

            bool dispose = accesoDatos == null;
            accesoDatos = accesoDatos ?? new AccesoDatosSqlServer();

            try
            {
                List<SqlParameter> parametros = new List<SqlParameter>
                {
                    accesoDatos.CreateParameter("@UsuarioId", SqlDbType.Int, listaDeCompras.Usuario.Id),
                    accesoDatos.CreateParameter("@Nombre", SqlDbType.VarChar, listaDeCompras.Nombre),
                    accesoDatos.CreateParameter("@FechaCreacion", SqlDbType.DateTime, listaDeCompras.FechaCreacion)
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

        public bool Actualizar(ListaDeCompras listaDeCompras, AccesoDatosSqlServer accesoDatos = null)
        {
            const string query = @"
                UPDATE ListaDeCompras
                SET Nombre = @Nombre
                WHERE Id = @Id";

            bool dispose = accesoDatos == null;
            accesoDatos = accesoDatos ?? new AccesoDatosSqlServer();

            try
            {
                List<SqlParameter> parametros = new List<SqlParameter>
                {
                    accesoDatos.CreateParameter("@Nombre", SqlDbType.VarChar, listaDeCompras.Nombre),
                    accesoDatos.CreateParameter("@Id", SqlDbType.Int, listaDeCompras.Id)
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

        public bool Eliminar(ListaDeCompras lista, AccesoDatosSqlServer accesoDatos = null)
        {
            const string query = @"
                DELETE FROM ListaDeCompras
                WHERE Id = @Id";

            bool dispose = accesoDatos == null;
            accesoDatos = accesoDatos ?? new AccesoDatosSqlServer();

            try
            {
                List<SqlParameter> parametros = new List<SqlParameter>
                {
                    accesoDatos.CreateParameter("@Id", SqlDbType.Int, lista.Id)
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
