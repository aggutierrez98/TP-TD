using System.Collections.Generic;
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
    public sealed class MedioPagoEntidadRepositorioIntegrationTests
    {
        [TestMethod]
        public void InsertarYObtenerId_MedioPagoEntidadValido_QuedaDisponibleHidratado()
        {
            using (AccesoDatosSqlServer accesoDatos = new AccesoDatosSqlServer())
            {
                accesoDatos.BeginTransactionScope();
                MedioPagoEntidadRepositorio repositorio = new MedioPagoEntidadRepositorio(accesoDatos);
                int entidadFinancieraId = InsertarEntidadFinancieraTest(accesoDatos, "EntidadTestMedioPagoEntidadPropio");
                int medioPagoId = InsertarMedioPagoTest(accesoDatos, BE.TipoMedioPago.TarjetaCredito, "MarcaTestMpeVisa");

                int id = repositorio.InsertarYObtenerId(new BE.MedioPagoEntidad
                {
                    EntidadFinanciera = new BE.EntidadFinanciera { Id = entidadFinancieraId },
                    MedioPago = new BE.MedioPago { Id = medioPagoId },
                    Segmento = "Eminent"
                });

                BE.MedioPagoEntidad obtenido = repositorio.ObtenerPorId(id);
                Assert.IsNotNull(obtenido);
                Assert.AreEqual(entidadFinancieraId, obtenido.EntidadFinanciera.Id);
                Assert.AreEqual("EntidadTestMedioPagoEntidadPropio", obtenido.EntidadFinanciera.Nombre);
                Assert.AreEqual(medioPagoId, obtenido.MedioPago.Id);
                Assert.AreEqual(BE.TipoMedioPago.TarjetaCredito, obtenido.MedioPago.Tipo);
                Assert.AreEqual("MarcaTestMpeVisa", obtenido.MedioPago.Marca);
                Assert.AreEqual("Eminent", obtenido.Segmento);

                accesoDatos.RollbackTransactionScope();
            }
        }

        [TestMethod]
        public void ObtenerPorEntidadFinanciera_ExcluyeMediosDeOtraEntidad()
        {
            using (AccesoDatosSqlServer accesoDatos = new AccesoDatosSqlServer())
            {
                accesoDatos.BeginTransactionScope();
                MedioPagoEntidadRepositorio repositorio = new MedioPagoEntidadRepositorio(accesoDatos);
                int entidadFinancieraId = InsertarEntidadFinancieraTest(accesoDatos, "EntidadTestMpeFiltroA");
                int otraEntidadFinancieraId = InsertarEntidadFinancieraTest(accesoDatos, "EntidadTestMpeFiltroB");
                int medioPagoId = InsertarMedioPagoTest(accesoDatos, BE.TipoMedioPago.TarjetaDebito, "MarcaTestMpeFiltro");

                int idPropio = InsertarMedioPagoEntidadTest(repositorio, entidadFinancieraId, medioPagoId, "Classic");
                int idAjeno = InsertarMedioPagoEntidadTest(repositorio, otraEntidadFinancieraId, medioPagoId, "Classic");

                List<BE.MedioPagoEntidad> propios = repositorio.ObtenerPorEntidadFinanciera(entidadFinancieraId);

                Assert.IsTrue(propios.Any(item => item.Id == idPropio && item.MedioPago.Marca == "MarcaTestMpeFiltro"));
                Assert.IsFalse(propios.Any(item => item.Id == idAjeno));

                accesoDatos.RollbackTransactionScope();
            }
        }

        [TestMethod]
        public void ObtenerSegmentosDistintos_ColapsaRepetidosYDescartaLosNulos()
        {
            using (AccesoDatosSqlServer accesoDatos = new AccesoDatosSqlServer())
            {
                accesoDatos.BeginTransactionScope();
                MedioPagoEntidadRepositorio repositorio = new MedioPagoEntidadRepositorio(accesoDatos);
                int entidadAId = InsertarEntidadFinancieraTest(accesoDatos, "EntidadTestMpeSegmentosA");
                int entidadBId = InsertarEntidadFinancieraTest(accesoDatos, "EntidadTestMpeSegmentosB");
                int medioPagoId = InsertarMedioPagoTest(accesoDatos, BE.TipoMedioPago.TarjetaCredito, "MarcaTestMpeSegmentos");

                InsertarMedioPagoEntidadTest(repositorio, entidadAId, medioPagoId, "SegTestPlatinum");
                InsertarMedioPagoEntidadTest(repositorio, entidadBId, medioPagoId, "SegTestPlatinum");
                InsertarMedioPagoEntidadTest(repositorio, entidadBId, InsertarMedioPagoTest(accesoDatos, BE.TipoMedioPago.TarjetaDebito, "MarcaTestMpeSegmentosDeb"), null);

                List<string> segmentos = repositorio.ObtenerSegmentosDistintos();

                Assert.AreEqual(1, segmentos.Count(item => item == "SegTestPlatinum"), "El segmento repetido aparece una sola vez.");
                Assert.IsFalse(segmentos.Any(item => item == null), "Los segmentos nulos no se listan.");

                accesoDatos.RollbackTransactionScope();
            }
        }

        [TestMethod]
        public void ObtenerPorMedioPago_DevuelveSoloLosDeEseMedioPago()
        {
            using (AccesoDatosSqlServer accesoDatos = new AccesoDatosSqlServer())
            {
                accesoDatos.BeginTransactionScope();
                MedioPagoEntidadRepositorio repositorio = new MedioPagoEntidadRepositorio(accesoDatos);
                int entidadFinancieraId = InsertarEntidadFinancieraTest(accesoDatos, "EntidadTestMpePorMedio");
                int medioPagoId = InsertarMedioPagoTest(accesoDatos, BE.TipoMedioPago.TarjetaCredito, "MarcaTestMpePorMedioA");
                int otroMedioPagoId = InsertarMedioPagoTest(accesoDatos, BE.TipoMedioPago.TarjetaCredito, "MarcaTestMpePorMedioB");

                int idPropio = InsertarMedioPagoEntidadTest(repositorio, entidadFinancieraId, medioPagoId, null);
                int idAjeno = InsertarMedioPagoEntidadTest(repositorio, entidadFinancieraId, otroMedioPagoId, null);

                List<BE.MedioPagoEntidad> resultado = repositorio.ObtenerPorMedioPago(medioPagoId);

                Assert.IsTrue(resultado.Any(item => item.Id == idPropio));
                Assert.IsFalse(resultado.Any(item => item.Id == idAjeno));

                accesoDatos.RollbackTransactionScope();
            }
        }

        [TestMethod]
        public void Actualizar_MedioPagoEntidadExistente_PersisteNuevoSegmento()
        {
            using (AccesoDatosSqlServer accesoDatos = new AccesoDatosSqlServer())
            {
                accesoDatos.BeginTransactionScope();
                MedioPagoEntidadRepositorio repositorio = new MedioPagoEntidadRepositorio(accesoDatos);
                int entidadFinancieraId = InsertarEntidadFinancieraTest(accesoDatos, "EntidadTestMpeOriginal");
                int medioPagoId = InsertarMedioPagoTest(accesoDatos, BE.TipoMedioPago.CuentaBilleteraVirtual, "MarcaTestMpeOriginal");
                int id = InsertarMedioPagoEntidadTest(repositorio, entidadFinancieraId, medioPagoId, "Basico");

                bool actualizado = repositorio.Actualizar(new BE.MedioPagoEntidad
                {
                    Id = id,
                    EntidadFinanciera = new BE.EntidadFinanciera { Id = entidadFinancieraId },
                    MedioPago = new BE.MedioPago { Id = medioPagoId },
                    Segmento = "Premium"
                });

                Assert.IsTrue(actualizado);
                Assert.AreEqual("Premium", repositorio.ObtenerPorId(id).Segmento);

                accesoDatos.RollbackTransactionScope();
            }
        }

        [TestMethod]
        public void Eliminar_MedioPagoEntidadExistente_DejaDeExistir()
        {
            using (AccesoDatosSqlServer accesoDatos = new AccesoDatosSqlServer())
            {
                accesoDatos.BeginTransactionScope();
                MedioPagoEntidadRepositorio repositorio = new MedioPagoEntidadRepositorio(accesoDatos);
                int entidadFinancieraId = InsertarEntidadFinancieraTest(accesoDatos, "EntidadTestMpeAEliminar");
                int medioPagoId = InsertarMedioPagoTest(accesoDatos, BE.TipoMedioPago.CuentaDNI, null);
                int id = InsertarMedioPagoEntidadTest(repositorio, entidadFinancieraId, medioPagoId, null);

                bool eliminado = repositorio.Eliminar(id);

                Assert.IsTrue(eliminado);
                Assert.IsNull(repositorio.ObtenerPorId(id));

                accesoDatos.RollbackTransactionScope();
            }
        }

        private static int InsertarMedioPagoEntidadTest(MedioPagoEntidadRepositorio repositorio, int entidadFinancieraId, int medioPagoId, string segmento)
        {
            return repositorio.InsertarYObtenerId(new BE.MedioPagoEntidad
            {
                EntidadFinanciera = new BE.EntidadFinanciera { Id = entidadFinancieraId },
                MedioPago = new BE.MedioPago { Id = medioPagoId },
                Segmento = segmento
            });
        }

        private static int InsertarEntidadFinancieraTest(AccesoDatosSqlServer accesoDatos, string nombre)
        {
            return new EntidadFinancieraRepositorio(accesoDatos).InsertarYObtenerId(new BE.EntidadFinanciera { Nombre = nombre });
        }

        private static int InsertarMedioPagoTest(AccesoDatosSqlServer accesoDatos, BE.TipoMedioPago tipo, string marca)
        {
            return new MedioPagoRepositorio(accesoDatos).InsertarYObtenerId(new BE.MedioPago { Tipo = tipo, Marca = marca });
        }
    }
}
