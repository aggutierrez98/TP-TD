using System.Linq;
using DAL;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using REPOSITORIO;

namespace TpIDS.IntegrationTests
{
    // Patron de aislamiento: cada test abre su propia transaccion sobre
    // AccesoDatosSqlServer y hace RollbackTransactionScope() al final,
    // asi que ningun test deja datos permanentes en TpCatedraIDS_Test.
    [TestClass]
    [TestCategory("Integration")]
    public sealed class CuentaFinancieraRepositorioIntegrationTests
    {
        [TestMethod]
        public void InsertarYObtenerId_CuentaValida_QuedaVisibleParaSuDuenio()
        {
            using (AccesoDatosSqlServer accesoDatos = new AccesoDatosSqlServer())
            {
                accesoDatos.BeginTransactionScope();
                CuentaFinancieraRepositorio repositorio = new CuentaFinancieraRepositorio(accesoDatos);
                int usuarioId = ObtenerUsuarioBasicoId(accesoDatos);
                int entidadFinancieraId = InsertarEntidadFinancieraTest(accesoDatos, "BancoTestCuentaPropia");

                int id = repositorio.InsertarYObtenerId(new BE.CuentaFinanciera
                {
                    Usuario = new BE.Usuario { Id = usuarioId },
                    EntidadFinanciera = new BE.EntidadFinanciera { Id = entidadFinancieraId },
                    Alias = "CuentaTestPropia",
                    Tipo = "CajaAhorro",
                    CredencialesCifradas = "cifrado-test"
                });

                BE.CuentaFinanciera obtenida = repositorio.ObtenerPorId(id);
                Assert.IsNotNull(obtenida);
                Assert.IsNotNull(obtenida.Usuario);
                Assert.AreEqual(usuarioId, obtenida.Usuario.Id);
                Assert.IsNotNull(obtenida.EntidadFinanciera);
                Assert.AreEqual(entidadFinancieraId, obtenida.EntidadFinanciera.Id);
                Assert.AreEqual("BancoTestCuentaPropia", obtenida.EntidadFinanciera.Nombre);
                Assert.AreEqual("CuentaTestPropia", obtenida.Alias);
                Assert.AreEqual("CajaAhorro", obtenida.Tipo);
                Assert.AreEqual("cifrado-test", obtenida.CredencialesCifradas);

                accesoDatos.RollbackTransactionScope();
            }
        }

        [TestMethod]
        public void ObtenerPorUsuario_ExcluyeCuentasDeOtroUsuario()
        {
            using (AccesoDatosSqlServer accesoDatos = new AccesoDatosSqlServer())
            {
                accesoDatos.BeginTransactionScope();
                CuentaFinancieraRepositorio repositorio = new CuentaFinancieraRepositorio(accesoDatos);
                int usuarioId = ObtenerUsuarioBasicoId(accesoDatos);
                int otroUsuarioId = ObtenerUsuarioAdminId(accesoDatos);
                int entidadFinancieraId = InsertarEntidadFinancieraTest(accesoDatos, "BancoTestCuentaFiltro");

                int idPropia = repositorio.InsertarYObtenerId(new BE.CuentaFinanciera
                {
                    Usuario = new BE.Usuario { Id = usuarioId },
                    EntidadFinanciera = new BE.EntidadFinanciera { Id = entidadFinancieraId },
                    Alias = "CuentaTestUsuarioA",
                    Tipo = "CajaAhorro"
                });
                int idAjena = repositorio.InsertarYObtenerId(new BE.CuentaFinanciera
                {
                    Usuario = new BE.Usuario { Id = otroUsuarioId },
                    EntidadFinanciera = new BE.EntidadFinanciera { Id = entidadFinancieraId },
                    Alias = "CuentaTestUsuarioB",
                    Tipo = "CajaAhorro"
                });

                System.Collections.Generic.List<BE.CuentaFinanciera> propias = repositorio.ObtenerPorUsuario(usuarioId);

                Assert.IsTrue(propias.Any(item => item.Id == idPropia));
                Assert.IsFalse(propias.Any(item => item.Id == idAjena));

                accesoDatos.RollbackTransactionScope();
            }
        }

        [TestMethod]
        public void Actualizar_CuentaExistente_PersisteNuevoAlias()
        {
            using (AccesoDatosSqlServer accesoDatos = new AccesoDatosSqlServer())
            {
                accesoDatos.BeginTransactionScope();
                CuentaFinancieraRepositorio repositorio = new CuentaFinancieraRepositorio(accesoDatos);
                int usuarioId = ObtenerUsuarioBasicoId(accesoDatos);
                int entidadFinancieraId = InsertarEntidadFinancieraTest(accesoDatos, "BancoTestCuentaOriginal");
                int id = repositorio.InsertarYObtenerId(new BE.CuentaFinanciera
                {
                    Usuario = new BE.Usuario { Id = usuarioId },
                    EntidadFinanciera = new BE.EntidadFinanciera { Id = entidadFinancieraId },
                    Alias = "CuentaTestOriginal",
                    Tipo = "CajaAhorro"
                });

                bool actualizado = repositorio.Actualizar(new BE.CuentaFinanciera
                {
                    Id = id,
                    Usuario = new BE.Usuario { Id = usuarioId },
                    EntidadFinanciera = new BE.EntidadFinanciera { Id = entidadFinancieraId },
                    Alias = "CuentaTestRenombrada",
                    Tipo = "CuentaCorriente",
                    CredencialesCifradas = "cifrado-nuevo"
                });

                Assert.IsTrue(actualizado);
                BE.CuentaFinanciera releida = repositorio.ObtenerPorId(id);
                Assert.AreEqual("CuentaTestRenombrada", releida.Alias);
                Assert.AreEqual("CuentaCorriente", releida.Tipo);
                Assert.AreEqual("cifrado-nuevo", releida.CredencialesCifradas);

                accesoDatos.RollbackTransactionScope();
            }
        }

        [TestMethod]
        public void Eliminar_CuentaExistente_DejaDeExistir()
        {
            using (AccesoDatosSqlServer accesoDatos = new AccesoDatosSqlServer())
            {
                accesoDatos.BeginTransactionScope();
                CuentaFinancieraRepositorio repositorio = new CuentaFinancieraRepositorio(accesoDatos);
                int usuarioId = ObtenerUsuarioBasicoId(accesoDatos);
                int entidadFinancieraId = InsertarEntidadFinancieraTest(accesoDatos, "BancoTestCuentaAEliminar");
                int id = repositorio.InsertarYObtenerId(new BE.CuentaFinanciera
                {
                    Usuario = new BE.Usuario { Id = usuarioId },
                    EntidadFinanciera = new BE.EntidadFinanciera { Id = entidadFinancieraId },
                    Alias = "CuentaTestAEliminar",
                    Tipo = "CajaAhorro"
                });

                bool eliminado = repositorio.Eliminar(id);

                Assert.IsTrue(eliminado);
                Assert.IsNull(repositorio.ObtenerPorId(id));

                accesoDatos.RollbackTransactionScope();
            }
        }

        private static int InsertarEntidadFinancieraTest(AccesoDatosSqlServer accesoDatos, string nombre)
        {
            return new EntidadFinancieraRepositorio(accesoDatos).InsertarYObtenerId(new BE.EntidadFinanciera { Nombre = nombre });
        }

        private static int ObtenerUsuarioBasicoId(AccesoDatosSqlServer accesoDatos)
        {
            return new UsuarioDAL().ObtenerPorUsername("user", accesoDatos).Id;
        }

        private static int ObtenerUsuarioAdminId(AccesoDatosSqlServer accesoDatos)
        {
            return new UsuarioDAL().ObtenerPorUsername("admin", accesoDatos).Id;
        }
    }
}
