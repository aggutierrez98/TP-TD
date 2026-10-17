using System;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Reflection;
using System.Xml;
using DAL;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SERVICIOS;

namespace TpIDS.IntegrationTests
{
    [TestClass]
    [TestCategory("Integration")]
    public sealed class ConexionConfiguradaIntegrationTests
    {
        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow("   ")]
        public void Read_PrincipalAusenteOVaciaConAlternativaDisponible_RechazaConfiguracion(string principal)
        {
            using (ConfiguracionSqlAislada entorno = new ConfiguracionSqlAislada(principal))
            {
                Assert.AreEqual(typeof(InvalidOperationException).FullName, entorno.Sonda.Leer());
            }
        }

        [TestMethod]
        public void Read_PrincipalInaccesibleConAlternativaDisponible_NoUsaAlternativa()
        {
            using (ConfiguracionSqlAislada entorno = new ConfiguracionSqlAislada(
                "Data Source=tcp:127.0.0.1,1;Initial Catalog=TpCatedraIDS_Test;Integrated Security=True;Connect Timeout=1;Pooling=False"))
            {
                Assert.AreEqual("Error.Technical.ConnectionUnavailable", entorno.Sonda.Leer());
            }
        }

        [TestMethod]
        public void Read_PrincipalDisponible_UsaCatalogoConfigurado()
        {
            string principal = ConfiguracionSqlAislada.ConexionPruebas;
            using (ConfiguracionSqlAislada entorno = new ConfiguracionSqlAislada(principal))
            {
                Assert.AreEqual(new SqlConnectionStringBuilder(principal).InitialCatalog, entorno.Sonda.Leer());
            }
        }

        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow("   ")]
        public void ResolverBackup_PrincipalAusenteOVaciaConAlternativa_RechazaConfiguracion(string principal)
        {
            using (ConfiguracionSqlAislada entorno = new ConfiguracionSqlAislada(principal))
            {
                Assert.AreEqual(typeof(InvalidOperationException).FullName, entorno.Sonda.ResolverBackup());
            }
        }

        [TestMethod]
        public void ResolverBackup_PrincipalDistintaDeAlternativa_UsaSoloPrincipal()
        {
            const string principal = "Data Source=servidor-configurado;Initial Catalog=CatalogoConfigurado;Integrated Security=True";
            using (ConfiguracionSqlAislada entorno = new ConfiguracionSqlAislada(principal))
            {
                Assert.AreEqual(principal, entorno.Sonda.ResolverBackup());
            }
        }
    }

    // Cada dominio lee su propio config: no se modifica ConfigurationManager del runner.
    internal sealed class ConfiguracionSqlAislada : IDisposable
    {
        private readonly string ruta;
        private readonly AppDomain dominio;
        public SondaConexionSql Sonda { get; }

        public static string ConexionPruebas => ConfigurationManager.ConnectionStrings["TpIDS"].ConnectionString;

        public ConfiguracionSqlAislada(string principal)
        {
            ruta = Path.Combine(Path.GetTempPath(), "TpIDS-Conexion-" + Guid.NewGuid().ToString("N") + ".config");
            XmlDocument config = new XmlDocument();
            XmlElement raiz = config.CreateElement("configuration");
            config.AppendChild(raiz);
            XmlElement cadenas = config.CreateElement("connectionStrings");
            raiz.AppendChild(cadenas);
            cadenas.AppendChild(config.CreateElement("clear"));
            if (principal != null)
            {
                AgregarCadena(config, cadenas, "TpIDS", principal);
            }
            AgregarCadena(config, cadenas, "TpIDS-UAI", ConexionPruebas);
            config.Save(ruta);

            try
            {
                dominio = AppDomain.CreateDomain("ConexionSql-" + Guid.NewGuid().ToString("N"), null,
                    new AppDomainSetup
                    {
                        ApplicationBase = Path.GetDirectoryName(typeof(SondaConexionSql).Assembly.Location),
                        ConfigurationFile = ruta
                    });
                Sonda = (SondaConexionSql)dominio.CreateInstanceAndUnwrap(
                    typeof(SondaConexionSql).Assembly.FullName, typeof(SondaConexionSql).FullName);
            }
            catch
            {
                Dispose();
                throw;
            }
        }

        private static void AgregarCadena(XmlDocument config, XmlElement cadenas, string nombre, string valor)
        {
            XmlElement entrada = config.CreateElement("add");
            entrada.SetAttribute("name", nombre);
            entrada.SetAttribute("connectionString", valor);
            entrada.SetAttribute("providerName", "System.Data.SqlClient");
            cadenas.AppendChild(entrada);
        }

        public void Dispose()
        {
            try
            {
                if (dominio != null) AppDomain.Unload(dominio);
            }
            finally
            {
                if (File.Exists(ruta)) File.Delete(ruta);
            }
        }
    }

    public sealed class SondaConexionSql : MarshalByRefObject
    {
        // Se prueba la seleccion compartida por backup/restore sin restaurar ni alterar la BD.
        public string ResolverBackup()
        {
            try
            {
                return (string)typeof(BackupDAL).GetMethod("ResolverConnectionString",
                    BindingFlags.NonPublic | BindingFlags.Static).Invoke(null, null);
            }
            catch (TargetInvocationException ex) when (ex.InnerException is InvalidOperationException)
            {
                return ex.InnerException.GetType().FullName;
            }
        }

        public string Leer()
        {
            try
            {
                using (AccesoDatosSqlServer acceso = new AccesoDatosSqlServer())
                {
                    return (string)acceso.Read("SELECT DB_NAME()").Rows[0][0];
                }
            }
            catch (InvalidOperationException ex) { return ex.GetType().FullName; }
            catch (ExcepcionTecnica ex) { return ex.Message; }
        }
    }
}
