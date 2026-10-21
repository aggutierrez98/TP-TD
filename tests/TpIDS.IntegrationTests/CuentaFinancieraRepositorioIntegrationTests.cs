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
                    Tipo = BE.TipoCuentaFinanciera.CajaAhorro,
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
                Assert.AreEqual(BE.TipoCuentaFinanciera.CajaAhorro, obtenida.Tipo);
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
                    Tipo = BE.TipoCuentaFinanciera.CajaAhorro
                });
                int idAjena = repositorio.InsertarYObtenerId(new BE.CuentaFinanciera
                {
                    Usuario = new BE.Usuario { Id = otroUsuarioId },
                    EntidadFinanciera = new BE.EntidadFinanciera { Id = entidadFinancieraId },
                    Alias = "CuentaTestUsuarioB",
                    Tipo = BE.TipoCuentaFinanciera.CajaAhorro
                });

                System.Collections.Generic.List<BE.CuentaFinanciera> propias = repositorio.ObtenerPorUsuario(new BE.Usuario { Id = usuarioId });

                Assert.IsTrue(propias.Any(item => item.Id == idPropia));
                Assert.IsFalse(propias.Any(item => item.Id == idAjena));

                accesoDatos.RollbackTransactionScope();
            }
        }

        [TestMethod]
        public void ObtenerPorEntidadFinanciera_ExcluyeCuentasDeOtraEntidad()
        {
            using (AccesoDatosSqlServer accesoDatos = new AccesoDatosSqlServer())
            {
                accesoDatos.BeginTransactionScope();
                CuentaFinancieraRepositorio repositorio = new CuentaFinancieraRepositorio(accesoDatos);
                int usuarioId = ObtenerUsuarioBasicoId(accesoDatos);
                int entidadFinancieraId = InsertarEntidadFinancieraTest(accesoDatos, "BancoTestCuentaPorEntidadA");
                int otraEntidadFinancieraId = InsertarEntidadFinancieraTest(accesoDatos, "BancoTestCuentaPorEntidadB");
                int idPropia = InsertarCuentaTest(repositorio, usuarioId, entidadFinancieraId, "CuentaTestEntidadA");
                int idAjena = InsertarCuentaTest(repositorio, usuarioId, otraEntidadFinancieraId, "CuentaTestEntidadB");

                System.Collections.Generic.List<BE.CuentaFinanciera> resultado = repositorio.ObtenerPorEntidadFinanciera(new BE.EntidadFinanciera { Id = entidadFinancieraId });

                Assert.IsTrue(resultado.Any(item => item.Id == idPropia));
                Assert.IsFalse(resultado.Any(item => item.Id == idAjena));

                accesoDatos.RollbackTransactionScope();
            }
        }

        [TestMethod]
        public void ReemplazarMediosPagoEntidadDeCuenta_AsignaCorrectamente()
        {
            using (AccesoDatosSqlServer accesoDatos = new AccesoDatosSqlServer())
            {
                accesoDatos.BeginTransactionScope();
                CuentaFinancieraRepositorio repositorio = new CuentaFinancieraRepositorio(accesoDatos);
                int usuarioId = ObtenerUsuarioBasicoId(accesoDatos);
                int entidadFinancieraId = InsertarEntidadFinancieraTest(accesoDatos, "BancoTestCuentaMedios");
                int id = InsertarCuentaTest(repositorio, usuarioId, entidadFinancieraId, "CuentaTestMedios");
                int medioPagoEntidadId1 = InsertarMedioPagoEntidadTest(accesoDatos, entidadFinancieraId, "MarcaTestCuentaMedios1");
                int medioPagoEntidadId2 = InsertarMedioPagoEntidadTest(accesoDatos, entidadFinancieraId, "MarcaTestCuentaMedios2");

                bool reemplazado = repositorio.ReemplazarMediosPagoEntidadDeCuenta(repositorio.ObtenerPorId(id), new[] { new MedioPagoEntidadRepositorio(accesoDatos).ObtenerPorId(medioPagoEntidadId1), new MedioPagoEntidadRepositorio(accesoDatos).ObtenerPorId(medioPagoEntidadId2) });

                Assert.IsTrue(reemplazado);
                BE.CuentaFinanciera releida = repositorio.ObtenerPorId(id);
                Assert.AreEqual(2, releida.MediosPagoEntidad.Count);
                Assert.IsTrue(releida.MediosPagoEntidad.Any(item => item.Id == medioPagoEntidadId1 && item.MedioPago.Marca == "MarcaTestCuentaMedios1"));
                Assert.IsTrue(releida.MediosPagoEntidad.Any(item => item.Id == medioPagoEntidadId2 && item.MedioPago.Marca == "MarcaTestCuentaMedios2"));

                accesoDatos.RollbackTransactionScope();
            }
        }

        [TestMethod]
        public void ReemplazarMediosPagoEntidadDeCuenta_LlamadaConNuevaLista_ReemplazaLaAnterior()
        {
            using (AccesoDatosSqlServer accesoDatos = new AccesoDatosSqlServer())
            {
                accesoDatos.BeginTransactionScope();
                CuentaFinancieraRepositorio repositorio = new CuentaFinancieraRepositorio(accesoDatos);
                int usuarioId = ObtenerUsuarioBasicoId(accesoDatos);
                int entidadFinancieraId = InsertarEntidadFinancieraTest(accesoDatos, "BancoTestCuentaMediosReemplazo");
                int id = InsertarCuentaTest(repositorio, usuarioId, entidadFinancieraId, "CuentaTestMediosReemplazo");
                int medioPagoEntidadIdA = InsertarMedioPagoEntidadTest(accesoDatos, entidadFinancieraId, "MarcaTestCuentaReemplazoA");
                int medioPagoEntidadIdB = InsertarMedioPagoEntidadTest(accesoDatos, entidadFinancieraId, "MarcaTestCuentaReemplazoB");

                repositorio.ReemplazarMediosPagoEntidadDeCuenta(repositorio.ObtenerPorId(id), new[] { new MedioPagoEntidadRepositorio(accesoDatos).ObtenerPorId(medioPagoEntidadIdA) });
                repositorio.ReemplazarMediosPagoEntidadDeCuenta(repositorio.ObtenerPorId(id), new[] { new MedioPagoEntidadRepositorio(accesoDatos).ObtenerPorId(medioPagoEntidadIdB) });

                BE.CuentaFinanciera releida = repositorio.ObtenerPorId(id);
                Assert.AreEqual(1, releida.MediosPagoEntidad.Count);
                Assert.AreEqual(medioPagoEntidadIdB, releida.MediosPagoEntidad[0].Id);

                accesoDatos.RollbackTransactionScope();
            }
        }

        [TestMethod]
        public void ContarPorMedioPagoEntidad_CuentaLasCuentasQueLoTienenContratado()
        {
            using (AccesoDatosSqlServer accesoDatos = new AccesoDatosSqlServer())
            {
                accesoDatos.BeginTransactionScope();
                CuentaFinancieraRepositorio repositorio = new CuentaFinancieraRepositorio(accesoDatos);
                int usuarioId = ObtenerUsuarioBasicoId(accesoDatos);
                int entidadFinancieraId = InsertarEntidadFinancieraTest(accesoDatos, "BancoTestContarMedio");
                int cuentaAId = InsertarCuentaTest(repositorio, usuarioId, entidadFinancieraId, "CuentaTestContarMedioA");
                int cuentaBId = InsertarCuentaTest(repositorio, usuarioId, entidadFinancieraId, "CuentaTestContarMedioB");
                int medioContratadoId = InsertarMedioPagoEntidadTest(accesoDatos, entidadFinancieraId, "MarcaTestContarMedioUsado");
                int medioSinUsoId = InsertarMedioPagoEntidadTest(accesoDatos, entidadFinancieraId, "MarcaTestContarMedioLibre");
                repositorio.ReemplazarMediosPagoEntidadDeCuenta(repositorio.ObtenerPorId(cuentaAId), new[] { new MedioPagoEntidadRepositorio(accesoDatos).ObtenerPorId(medioContratadoId) });
                repositorio.ReemplazarMediosPagoEntidadDeCuenta(repositorio.ObtenerPorId(cuentaBId), new[] { new MedioPagoEntidadRepositorio(accesoDatos).ObtenerPorId(medioContratadoId) });

                Assert.AreEqual(2, repositorio.ContarPorMedioPagoEntidad(new MedioPagoEntidadRepositorio(accesoDatos).ObtenerPorId(medioContratadoId)));
                Assert.AreEqual(0, repositorio.ContarPorMedioPagoEntidad(new MedioPagoEntidadRepositorio(accesoDatos).ObtenerPorId(medioSinUsoId)));

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
                    Tipo = BE.TipoCuentaFinanciera.CajaAhorro
                });

                bool actualizado = repositorio.Actualizar(new BE.CuentaFinanciera
                {
                    Id = id,
                    Usuario = new BE.Usuario { Id = usuarioId },
                    EntidadFinanciera = new BE.EntidadFinanciera { Id = entidadFinancieraId },
                    Alias = "CuentaTestRenombrada",
                    Tipo = BE.TipoCuentaFinanciera.CuentaCorriente,
                    CredencialesCifradas = "cifrado-nuevo"
                });

                Assert.IsTrue(actualizado);
                BE.CuentaFinanciera releida = repositorio.ObtenerPorId(id);
                Assert.AreEqual("CuentaTestRenombrada", releida.Alias);
                Assert.AreEqual(BE.TipoCuentaFinanciera.CuentaCorriente, releida.Tipo);
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
                    Tipo = BE.TipoCuentaFinanciera.CajaAhorro
                });

                bool eliminado = repositorio.Eliminar(repositorio.ObtenerPorId(id));

                Assert.IsTrue(eliminado);
                Assert.IsNull(repositorio.ObtenerPorId(id));

                accesoDatos.RollbackTransactionScope();
            }
        }

        private static int InsertarCuentaTest(CuentaFinancieraRepositorio repositorio, int usuarioId, int entidadFinancieraId, string alias)
        {
            return repositorio.InsertarYObtenerId(new BE.CuentaFinanciera
            {
                Usuario = new BE.Usuario { Id = usuarioId },
                EntidadFinanciera = new BE.EntidadFinanciera { Id = entidadFinancieraId },
                Alias = alias,
                Tipo = BE.TipoCuentaFinanciera.CajaAhorro
            });
        }

        private static int InsertarMedioPagoEntidadTest(AccesoDatosSqlServer accesoDatos, int entidadFinancieraId, string marca)
        {
            int medioPagoId = new MedioPagoRepositorio(accesoDatos).InsertarYObtenerId(new BE.MedioPago
            {
                Tipo = BE.TipoMedioPago.TarjetaCredito,
                Marca = marca
            });

            return new MedioPagoEntidadRepositorio(accesoDatos).InsertarYObtenerId(new BE.MedioPagoEntidad
            {
                EntidadFinanciera = new BE.EntidadFinanciera { Id = entidadFinancieraId },
                MedioPago = new BE.MedioPago { Id = medioPagoId },
                Segmento = null
            });
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
