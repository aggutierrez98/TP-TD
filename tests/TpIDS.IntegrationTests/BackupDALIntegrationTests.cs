using System;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using DAL;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TpIDS.IntegrationTests
{
    // T08: BackupDAL no pasa por AccesoDatosSqlServer (ver comentario en DAL.BackupDAL), asi
    // que el patron de aislamiento RollbackTransactionScope() del resto de esta suite no le
    // aplica: BACKUP DATABASE / RESTORE DATABASE corren fuera de cualquier transaccion.
    [TestClass]
    [TestCategory("Integration")]
    public sealed class BackupDALIntegrationTests
    {
        [TestMethod]
        public void CrearBackup_GeneraUnArchivoDeBackupValido()
        {
            BackupDAL backupDAL = new BackupDAL();

            // BACKUP DATABASE corre del lado del servidor: la carpeta destino tiene que ser
            // escribible por la cuenta de servicio de SQL Server (ej. "NT Service\MSSQL$..."),
            // no por el usuario que corre este test. Path.GetTempPath() del proceso de test
            // falla con "Acceso denegado" contra esa cuenta (confirmado empiricamente); se le
            // pregunta a la propia instancia su carpeta de backup por defecto, que la cuenta de
            // servicio siempre puede escribir.
            string carpetaBackup = ObtenerCarpetaBackupPorDefecto();
            string ruta = Path.Combine(carpetaBackup, "TpIDS-IntegrationTest-" + Guid.NewGuid().ToString("N") + ".bak");

            // Verificacion tambien del lado del servidor (xp_fileexist), no con System.IO: la
            // carpeta de backup por defecto de una instancia SQL Server tipicamente tiene ACLs
            // que excluyen al usuario interactivo (confirmado empiricamente: ni siquiera se
            // puede listar desde una sesion normal), aunque la cuenta de servicio si haya
            // escrito el archivo ahi sin problema. No se borra el archivo generado: el cliente
            // no tiene acceso de escritura a esa carpeta para eliminarlo, y usar xp_cmdshell o
            // xp_delete_file para lograrlo introduciria mas riesgo (habilitar ejecucion de
            // shell, o borrar por extension/fecha arriesgando otros .bak legitimos de la
            // carpeta) que el que vale la pena para limpiar un archivo de prueba.
            backupDAL.CrearBackup(ruta);

            Assert.IsTrue(ArchivoExisteEnElServidor(ruta), "El backup deberia haberse generado en la ruta indicada.");
        }

        private static string ObtenerCarpetaBackupPorDefecto()
        {
            using (SqlConnection connection = AbrirConexion())
            using (SqlCommand command = new SqlCommand("SELECT CAST(SERVERPROPERTY('InstanceDefaultBackupPath') AS NVARCHAR(4000))", connection))
            {
                return (string)command.ExecuteScalar();
            }
        }

        private static bool ArchivoExisteEnElServidor(string ruta)
        {
            using (SqlConnection connection = AbrirConexion())
            using (SqlCommand command = new SqlCommand("EXEC master.dbo.xp_fileexist @Ruta, @Existe OUTPUT", connection))
            {
                command.Parameters.AddWithValue("@Ruta", ruta);
                SqlParameter existe = command.Parameters.Add("@Existe", System.Data.SqlDbType.Int);
                existe.Direction = System.Data.ParameterDirection.Output;
                command.ExecuteNonQuery();
                return (int)existe.Value == 1;
            }
        }

        private static SqlConnection AbrirConexion()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["TpIDS"].ConnectionString;
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            return connection;
        }

        // No se automatiza un test de RestaurarBackup contra la base compartida de esta
        // suite: RESTORE DATABASE exige poner la base en SINGLE_USER WITH ROLLBACK IMMEDIATE,
        // lo que tira cualquier otra conexion activa (incluida la de otros tests corriendo en
        // paralelo o en cola) y es incompatible con el patron RollbackTransactionScope() que
        // aisla al resto de esta suite. Se ejerce manualmente y via UI tests (ver
        // TpIDS.UiTests/T08_BackupTests.cs, que tampoco lo automatiza por el mismo motivo).
    }
}
