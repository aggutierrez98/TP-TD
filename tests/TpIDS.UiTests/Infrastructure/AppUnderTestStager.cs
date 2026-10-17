using System;
using System.Data.SqlClient;
using System.IO;
using System.Xml;
using SERVICIOS;

namespace TpIDS.UiTests.Infrastructure
{
    /// <summary>
    /// Prepara una copia aislada de la app productiva ("app under test"):
    /// copia UI\bin\Config a una carpeta staging y reescribe su .exe.config para
    /// apuntar a la BD de test. El App.config productivo nunca se toca.
    /// Tambien siembra credenciales conocidas sobre la BD de test virgen, usando el
    /// mismo PasswordHasher del sistema, ANTES del primer arranque (con DVH aun nulos,
    /// para que la inicializacion de firmas de integridad los tome como base).
    /// </summary>
    internal static class AppUnderTestStager
    {
        public static void Stage()
        {
            string sourceDir = TestConfig.UiOutputDirectory;
            string sourceExe = Path.Combine(sourceDir, "TpIDS.exe");
            if (!File.Exists(sourceExe))
            {
                throw new InvalidOperationException(
                    "No existe " + sourceExe + ". Compilar tests\\TpIDS.Tests.sln (configuracion " +
                    TestConfig.Configuration + ") antes de correr los tests UI.");
            }

            string stagingDir = TestConfig.StagingDirectory;
            if (Directory.Exists(stagingDir))
            {
                Directory.Delete(stagingDir, true);
            }

            CopiarDirectorio(sourceDir, stagingDir);
            ReescribirConnectionStrings(TestConfig.StagedExePath + ".config");
        }

        public static void SembrarCredencialesConocidas()
        {
            PasswordHasher hasher = new PasswordHasher();

            using (SqlConnection connection = new SqlConnection(TestConfig.ConnectionString))
            {
                connection.Open();

                NormalizarPassword(connection, hasher, TestConfig.AdminUsername, TestConfig.AdminPassword);
                NormalizarPassword(connection, hasher, TestConfig.BasicUsername, TestConfig.BasicPassword);
                NormalizarPassword(connection, hasher, TestConfig.OperadorUsername, TestConfig.OperadorPassword);
                InsertarUsuarioInactivoSiFalta(connection, hasher);
            }
        }

        private static void NormalizarPassword(SqlConnection connection, PasswordHasher hasher, string username, string passwordConocida)
        {
            string storedHash;
            bool dvhInicializado;

            using (SqlCommand select = new SqlCommand(
                "SELECT PasswordHash, DVH FROM dbo.Usuarios WHERE Username = @username", connection))
            {
                select.Parameters.AddWithValue("@username", username);
                using (SqlDataReader reader = select.ExecuteReader())
                {
                    if (!reader.Read())
                    {
                        throw new InvalidOperationException(
                            "No existe el usuario semilla '" + username + "' en " + TestConfig.Database +
                            ". Ejecutar tests\\tools\\prepare-test-database.ps1 primero.");
                    }

                    storedHash = reader.GetString(0);
                    dvhInicializado = !reader.IsDBNull(1);
                }
            }

            if (dvhInicializado)
            {
                // La BD ya paso por un arranque: solo se acepta si la password conocida sigue vigente,
                // porque reescribir el hash por SQL directo romperia el DVH.
                if (!hasher.VerifyPassword(passwordConocida, storedHash))
                {
                    throw new InvalidOperationException(
                        "La BD " + TestConfig.Database + " ya tiene firmas de integridad y la password de '" +
                        username + "' no es la esperada por la suite. Re-ejecutar tests\\tools\\prepare-test-database.ps1.");
                }

                return;
            }

            using (SqlCommand update = new SqlCommand(
                "UPDATE dbo.Usuarios SET PasswordHash = @hash WHERE Username = @username AND DVH IS NULL", connection))
            {
                update.Parameters.AddWithValue("@hash", hasher.HashPassword(passwordConocida));
                update.Parameters.AddWithValue("@username", username);
                update.ExecuteNonQuery();
            }
        }

        private static void InsertarUsuarioInactivoSiFalta(SqlConnection connection, PasswordHasher hasher)
        {
            using (SqlCommand exists = new SqlCommand(
                "SELECT COUNT(*) FROM dbo.Usuarios WHERE Username = @username", connection))
            {
                exists.Parameters.AddWithValue("@username", TestConfig.InactiveUsername);
                if ((int)exists.ExecuteScalar() > 0)
                {
                    return;
                }
            }

            using (SqlCommand firmas = new SqlCommand(
                "SELECT COUNT(*) FROM dbo.DigitoVerificadorVertical", connection))
            {
                if ((int)firmas.ExecuteScalar() > 0)
                {
                    throw new InvalidOperationException(
                        "La BD " + TestConfig.Database + " ya tiene firmas de integridad y falta el usuario inactivo " +
                        "de la suite. Re-ejecutar tests\\tools\\prepare-test-database.ps1.");
                }
            }

            using (SqlCommand insert = new SqlCommand(
                "INSERT INTO dbo.Usuarios (Username, PasswordHash, Activo) VALUES (@username, @hash, 0)", connection))
            {
                insert.Parameters.AddWithValue("@username", TestConfig.InactiveUsername);
                insert.Parameters.AddWithValue("@hash", hasher.HashPassword(TestConfig.InactivePassword));
                insert.ExecuteNonQuery();
            }
        }

        private static void CopiarDirectorio(string origen, string destino)
        {
            Directory.CreateDirectory(destino);

            foreach (string archivo in Directory.GetFiles(origen))
            {
                File.Copy(archivo, Path.Combine(destino, Path.GetFileName(archivo)), true);
            }

            foreach (string subdirectorio in Directory.GetDirectories(origen))
            {
                CopiarDirectorio(subdirectorio, Path.Combine(destino, Path.GetFileName(subdirectorio)));
            }
        }

        private static void ReescribirConnectionStrings(string configPath)
        {
            if (!File.Exists(configPath))
            {
                throw new InvalidOperationException("No se encontro el config staged: " + configPath);
            }

            XmlDocument doc = new XmlDocument();
            doc.Load(configPath);

            XmlNodeList nodos = doc.SelectNodes("/configuration/connectionStrings/add");
            if (nodos == null || nodos.Count == 0)
            {
                throw new InvalidOperationException("El config staged no tiene connection strings: " + configPath);
            }

            foreach (XmlNode nodo in nodos)
            {
                XmlAttribute connectionString = nodo.Attributes["connectionString"];
                if (connectionString == null)
                {
                    continue;
                }

                SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(connectionString.Value)
                {
                    DataSource = TestConfig.SqlServer,
                    InitialCatalog = TestConfig.Database
                };
                connectionString.Value = builder.ConnectionString;
            }

            doc.Save(configPath);
        }
    }
}
