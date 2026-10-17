using System;
using System.IO;

namespace TpIDS.UiTests.Infrastructure
{
    /// <summary>
    /// Configuracion compartida de la suite UI: servidor/BD de test, rutas del repo
    /// y credenciales conocidas que se siembran sobre los usuarios semilla.
    /// </summary>
    internal static class TestConfig
    {
        public const string AdminUsername = "admin";
        public const string AdminPassword = "UiTest.Admin.1234";
        public const string BasicUsername = "user";
        public const string BasicPassword = "UiTest.User.1234";
        public const string OperadorUsername = "operador";
        public const string OperadorPassword = "UiTest.Operador.1234";
        public const string InactiveUsername = "uitest_inactivo";
        public const string InactivePassword = "UiTest.Inactivo.1234";

        public static string SqlServer
        {
            get { return LeerVariable("TPIDS_UITEST_SQLSERVER", @".\SQLEXPRESS"); }
        }

        public static string Database
        {
            get
            {
                string database = LeerVariable("TPIDS_UITEST_DATABASE", "TpCatedraIDS_Test");
                if (!database.StartsWith("TpCatedraIDS_Test", StringComparison.OrdinalIgnoreCase))
                {
                    throw new InvalidOperationException(
                        "La BD de tests UI debe empezar con TpCatedraIDS_Test. Valor recibido: " + database);
                }

                return database;
            }
        }

        public static string ConnectionString
        {
            get
            {
                return string.Format(
                    "Data Source={0};Initial Catalog={1};Integrated Security=True",
                    SqlServer,
                    Database);
            }
        }

        /// <summary>Configuracion de build (Debug/Release) derivada de la ruta de salida bin\Config\net472.</summary>
        public static string Configuration
        {
            get
            {
                string baseDir = AppDomain.CurrentDomain.BaseDirectory.TrimEnd(Path.DirectorySeparatorChar);
                DirectoryInfo net472 = new DirectoryInfo(baseDir);
                if (net472.Parent != null)
                {
                    return net472.Parent.Name;
                }

                return "Debug";
            }
        }

        public static string RepoRoot
        {
            get
            {
                string current = AppDomain.CurrentDomain.BaseDirectory;
                while (!string.IsNullOrEmpty(current))
                {
                    if (File.Exists(Path.Combine(current, "TpIDS.sln")))
                    {
                        return current;
                    }

                    current = Path.GetDirectoryName(current.TrimEnd(Path.DirectorySeparatorChar));
                }

                throw new InvalidOperationException("No se encontro TpIDS.sln subiendo desde " + AppDomain.CurrentDomain.BaseDirectory);
            }
        }

        public static string UiOutputDirectory
        {
            get { return Path.Combine(RepoRoot, "UI", "bin", Configuration); }
        }

        public static string StagingDirectory
        {
            get { return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "app-under-test"); }
        }

        public static string StagedExePath
        {
            get { return Path.Combine(StagingDirectory, "TpIDS.exe"); }
        }

        public static string ScreenshotDirectory
        {
            get { return Path.Combine(RepoRoot, "TestResults", "ui-screenshots"); }
        }

        private static string LeerVariable(string nombre, string valorDefault)
        {
            string valor = Environment.GetEnvironmentVariable(nombre);
            return string.IsNullOrWhiteSpace(valor) ? valorDefault : valor;
        }
    }
}
