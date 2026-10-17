using System.Configuration;
using System.Data.SqlClient;
using SERVICIOS;

namespace DAL
{
    // T08: backup y restauracion de la base de datos via BACKUP DATABASE / RESTORE DATABASE
    // nativos de SQL Server (T-SQL plano, sin copiar el .mdf ni exportar scripts).
    //
    // Decision de arquitectura: este es el primer DAL del proyecto que NO usa
    // AccesoDatosSqlServer. AccesoDatosSqlServer.ExecuteInTransaction siempre abre una
    // SqlTransaction explicita si no hay ninguna activa (ver ExecuteInTransaction), y SQL
    // Server prohibe ejecutar BACKUP DATABASE / RESTORE DATABASE dentro de una transaccion
    // explicita (falla en runtime con un error real de SQL Server). Por eso BackupDAL abre
    // su propia SqlConnection en autocommit, fuera de ese mecanismo.
    public class BackupDAL
    {
        // Los backups/restores pueden tardar mas que cualquier otra operacion del sistema.
        private const int SinLimiteDeTiempo = 0;

        public void CrearBackup(string rutaArchivo)
        {
            string connectionString = ResolverConnectionString();
            string nombreBase = ObtenerNombreBaseDatos(connectionString);

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string sql = string.Format("BACKUP DATABASE {0} TO DISK = @RutaArchivo", EscaparNombreBase(nombreBase));
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.CommandTimeout = SinLimiteDeTiempo;
                        command.Parameters.AddWithValue("@RutaArchivo", rutaArchivo);
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (SqlException ex)
            {
                throw new ExcepcionTecnica("Error.Technical.SqlFailure", ex);
            }
        }

        public void RestaurarBackup(string rutaArchivo)
        {
            string connectionStringOriginal = ResolverConnectionString();
            string nombreBase = ObtenerNombreBaseDatos(connectionStringOriginal);
            string nombreBaseEscapado = EscaparNombreBase(nombreBase);
            string connectionStringMaster = ConstruirConnectionStringMaster(connectionStringOriginal);

            // RESTORE DATABASE exige acceso exclusivo: hay que conectarse a master (no a la
            // base objetivo, que no se puede tocar en SINGLE_USER desde su propia conexion)
            // para poder tirar a todos los demas usuarios antes de restaurar.
            SqlConnection connection = new SqlConnection(connectionStringMaster);
            try
            {
                connection.Open();

                try
                {
                    EjecutarNoQuery(connection, string.Format(
                        "ALTER DATABASE {0} SET SINGLE_USER WITH ROLLBACK IMMEDIATE", nombreBaseEscapado));

                    string sqlRestore = string.Format(
                        "RESTORE DATABASE {0} FROM DISK = @RutaArchivo WITH REPLACE", nombreBaseEscapado);
                    using (SqlCommand restoreCommand = new SqlCommand(sqlRestore, connection))
                    {
                        restoreCommand.CommandTimeout = SinLimiteDeTiempo;
                        restoreCommand.Parameters.AddWithValue("@RutaArchivo", rutaArchivo);
                        restoreCommand.ExecuteNonQuery();
                    }
                }
                finally
                {
                    try
                    {
                        // Si el restore fallo, la base no debe quedar huerfana en
                        // SINGLE_USER: se intenta liberarla pase lo que pase.
                        EjecutarNoQuery(connection, string.Format("ALTER DATABASE {0} SET MULTI_USER", nombreBaseEscapado));
                    }
                    finally
                    {
                        // El SINGLE_USER de arriba mato cualquier conexion pooleada contra
                        // esta base: sin este ClearPool, ADO.NET reintenta reusarlas y el
                        // pool de la app queda roto para el resto de la sesion.
                        SqlConnection.ClearPool(connection);
                    }
                }
            }
            catch (SqlException ex)
            {
                throw new ExcepcionTecnica("Error.Technical.SqlFailure", ex);
            }
            finally
            {
                connection.Dispose();
            }
        }

        private static void EjecutarNoQuery(SqlConnection connection, string sql)
        {
            using (SqlCommand command = new SqlCommand(sql, connection))
            {
                command.CommandTimeout = SinLimiteDeTiempo;
                command.ExecuteNonQuery();
            }
        }

        // El nombre real de la base no se hardcodea: se extrae del connection string que ya
        // usa la app, asi funciona igual contra TpCatedraIDS (produccion) y
        // TpCatedraIDS_Test (tests) sin tocar codigo.
        private static string ObtenerNombreBaseDatos(string connectionString)
        {
            return new SqlConnectionStringBuilder(connectionString).InitialCatalog;
        }

        private static string ConstruirConnectionStringMaster(string connectionString)
        {
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(connectionString)
            {
                InitialCatalog = "master"
            };

            return builder.ConnectionString;
        }

        // BACKUP/RESTORE/ALTER DATABASE no aceptan el nombre de base como SqlParameter: se
        // interpola entre corchetes, escapando ']' como sql-injection-safe identifier.
        private static string EscaparNombreBase(string nombreBase)
        {
            return "[" + nombreBase.Replace("]", "]]") + "]";
        }

        private static string ResolverConnectionString()
        {
            ConnectionStringSettings connectionSettings = ConfigurationManager.ConnectionStrings["TpIDS"];
            if (connectionSettings == null || string.IsNullOrWhiteSpace(connectionSettings.ConnectionString))
            {
                throw new System.InvalidOperationException(
                    "No se encontro una connection string TpIDS valida en la configuracion.");
            }

            return connectionSettings.ConnectionString;
        }
    }
}
