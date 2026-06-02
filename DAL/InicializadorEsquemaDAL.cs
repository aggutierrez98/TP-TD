using System;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Text.RegularExpressions;

namespace DAL
{
    public class InicializadorEsquemaDAL
    {
        private static readonly string[] ConnectionStringNames = { "TpIDS", "TpIDS-UAI" };

        public void EjecutarScript(string rutaScript)
        {
            if (string.IsNullOrWhiteSpace(rutaScript))
            {
                throw new ArgumentException("La ruta del script no puede estar vacia.", "rutaScript");
            }

            if (!File.Exists(rutaScript))
            {
                throw new FileNotFoundException("No se encontro el script de base de datos.", rutaScript);
            }

            SqlConnectionStringBuilder builder = ObtenerConnectionString();
            AsegurarBaseExistente(builder);

            string contenido = File.ReadAllText(rutaScript);
            string[] lotes = Regex.Split(contenido, @"^\s*GO\s*($|\-\-.*$)", RegexOptions.Multiline | RegexOptions.IgnoreCase);

            using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
            {
                connection.Open();

                foreach (string lote in lotes)
                {
                    string sql = (lote ?? string.Empty).Trim();
                    if (string.IsNullOrWhiteSpace(sql) ||
                        string.Equals(sql, "USE TpCatedraIDS;", StringComparison.OrdinalIgnoreCase) ||
                        string.Equals(sql, "USE TpCatedraIDS", StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }

                    using (SqlCommand command = connection.CreateCommand())
                    {
                        command.CommandText = sql;
                        command.ExecuteNonQuery();
                    }
                }
            }
        }

        private SqlConnectionStringBuilder ObtenerConnectionString()
        {
            foreach (string connectionStringName in ConnectionStringNames)
            {
                ConnectionStringSettings connectionSettings = ConfigurationManager.ConnectionStrings[connectionStringName];
                if (connectionSettings == null || string.IsNullOrWhiteSpace(connectionSettings.ConnectionString))
                {
                    continue;
                }

                return new SqlConnectionStringBuilder(connectionSettings.ConnectionString);
            }

            throw new InvalidOperationException("No se encontro ninguna connection string valida para inicializar el esquema.");
        }

        private void AsegurarBaseExistente(SqlConnectionStringBuilder builder)
        {
            string nombreBase = builder.InitialCatalog;
            SqlConnectionStringBuilder masterBuilder = new SqlConnectionStringBuilder(builder.ConnectionString)
            {
                InitialCatalog = "master"
            };

            using (SqlConnection connection = new SqlConnection(masterBuilder.ConnectionString))
            {
                connection.Open();

                using (SqlCommand command = connection.CreateCommand())
                {
                    command.CommandText = @"
IF DB_ID(@NombreBase) IS NULL
BEGIN
    DECLARE @Sql NVARCHAR(MAX);
    SET @Sql = N'CREATE DATABASE [' + REPLACE(@NombreBase, ']', ']]') + N']';
    EXEC(@Sql);
END";
                    command.Parameters.AddWithValue("@NombreBase", nombreBase);
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
