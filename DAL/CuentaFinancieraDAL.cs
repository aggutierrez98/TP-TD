using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using BE;

namespace DAL
{
    public class CuentaFinancieraDAL
    {
        private CuentaFinanciera Map(DataRow row)
        {
            return new CuentaFinanciera
            {
                Id = (int)row["Id"],
                Usuario = new Usuario { Id = (int)row["UsuarioId"] },
                EntidadFinanciera = new EntidadFinanciera { Id = (int)row["EntidadFinancieraId"] },
                Alias = row["Alias"].ToString(),
                Tipo = (TipoCuentaFinanciera)Enum.Parse(typeof(TipoCuentaFinanciera), row["Tipo"].ToString()),
                CredencialesCifradas = row.IsNull("CredencialesCifradas") ? null : row["CredencialesCifradas"].ToString()
            };
        }

        public CuentaFinanciera ObtenerPorId(int id, AccesoDatosSqlServer accesoDatos = null)
        {
            const string query = @"
                SELECT Id, UsuarioId, EntidadFinancieraId, Alias, Tipo, CredencialesCifradas
                FROM CuentaFinanciera
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

        public List<CuentaFinanciera> ObtenerPorUsuario(int usuarioId, AccesoDatosSqlServer accesoDatos = null)
        {
            const string query = @"
                SELECT Id, UsuarioId, EntidadFinancieraId, Alias, Tipo, CredencialesCifradas
                FROM CuentaFinanciera
                WHERE UsuarioId = @UsuarioId
                ORDER BY Alias";

            bool dispose = accesoDatos == null;
            accesoDatos = accesoDatos ?? new AccesoDatosSqlServer();

            try
            {
                List<SqlParameter> parametros = new List<SqlParameter>
                {
                    accesoDatos.CreateParameter("@UsuarioId", SqlDbType.Int, usuarioId)
                };

                DataTable dataTable = accesoDatos.Read(query, parametros);
                List<CuentaFinanciera> cuentas = new List<CuentaFinanciera>();
                foreach (DataRow row in dataTable.Rows)
                {
                    cuentas.Add(Map(row));
                }

                return cuentas;
            }
            finally
            {
                if (dispose)
                {
                    accesoDatos.Dispose();
                }
            }
        }

        public List<CuentaFinanciera> ObtenerPorEntidadFinanciera(int entidadFinancieraId, AccesoDatosSqlServer accesoDatos = null)
        {
            const string query = @"
                SELECT Id, UsuarioId, EntidadFinancieraId, Alias, Tipo, CredencialesCifradas
                FROM CuentaFinanciera
                WHERE EntidadFinancieraId = @EntidadFinancieraId
                ORDER BY Alias";

            bool dispose = accesoDatos == null;
            accesoDatos = accesoDatos ?? new AccesoDatosSqlServer();

            try
            {
                List<SqlParameter> parametros = new List<SqlParameter>
                {
                    accesoDatos.CreateParameter("@EntidadFinancieraId", SqlDbType.Int, entidadFinancieraId)
                };

                DataTable dataTable = accesoDatos.Read(query, parametros);
                List<CuentaFinanciera> cuentas = new List<CuentaFinanciera>();
                foreach (DataRow row in dataTable.Rows)
                {
                    cuentas.Add(Map(row));
                }

                return cuentas;
            }
            finally
            {
                if (dispose)
                {
                    accesoDatos.Dispose();
                }
            }
        }

        public int InsertarYObtenerId(CuentaFinanciera cuenta, AccesoDatosSqlServer accesoDatos = null)
        {
            const string query = @"
                INSERT INTO CuentaFinanciera (UsuarioId, EntidadFinancieraId, Alias, Tipo, CredencialesCifradas)
                OUTPUT INSERTED.Id
                VALUES (@UsuarioId, @EntidadFinancieraId, @Alias, @Tipo, @CredencialesCifradas)";

            bool dispose = accesoDatos == null;
            accesoDatos = accesoDatos ?? new AccesoDatosSqlServer();

            try
            {
                List<SqlParameter> parametros = new List<SqlParameter>
                {
                    accesoDatos.CreateParameter("@UsuarioId", SqlDbType.Int, cuenta.Usuario.Id),
                    accesoDatos.CreateParameter("@EntidadFinancieraId", SqlDbType.Int, cuenta.EntidadFinanciera.Id),
                    accesoDatos.CreateParameter("@Alias", SqlDbType.VarChar, cuenta.Alias),
                    accesoDatos.CreateParameter("@Tipo", SqlDbType.VarChar, cuenta.Tipo.ToString()),
                    accesoDatos.CreateParameter("@CredencialesCifradas", SqlDbType.VarChar, cuenta.CredencialesCifradas)
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

        public bool Actualizar(CuentaFinanciera cuenta, AccesoDatosSqlServer accesoDatos = null)
        {
            const string query = @"
                UPDATE CuentaFinanciera
                SET EntidadFinancieraId = @EntidadFinancieraId,
                    Alias = @Alias,
                    Tipo = @Tipo,
                    CredencialesCifradas = @CredencialesCifradas
                WHERE Id = @Id";

            bool dispose = accesoDatos == null;
            accesoDatos = accesoDatos ?? new AccesoDatosSqlServer();

            try
            {
                List<SqlParameter> parametros = new List<SqlParameter>
                {
                    accesoDatos.CreateParameter("@EntidadFinancieraId", SqlDbType.Int, cuenta.EntidadFinanciera.Id),
                    accesoDatos.CreateParameter("@Alias", SqlDbType.VarChar, cuenta.Alias),
                    accesoDatos.CreateParameter("@Tipo", SqlDbType.VarChar, cuenta.Tipo.ToString()),
                    accesoDatos.CreateParameter("@CredencialesCifradas", SqlDbType.VarChar, cuenta.CredencialesCifradas),
                    accesoDatos.CreateParameter("@Id", SqlDbType.Int, cuenta.Id)
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
                DELETE FROM CuentaFinanciera
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
