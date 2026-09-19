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
    public sealed class MedioDePagoRepositorioIntegrationTests
    {
        [TestMethod]
        public void InsertarYObtenerId_MedioDePagoValido_QuedaDisponible()
        {
            using (AccesoDatosSqlServer accesoDatos = new AccesoDatosSqlServer())
            {
                accesoDatos.BeginTransactionScope();
                MedioDePagoRepositorio repositorio = new MedioDePagoRepositorio(accesoDatos);
                int entidadFinancieraId = InsertarEntidadFinancieraTest(accesoDatos, "EntidadTestMedioDePagoPropio");

                int id = repositorio.InsertarYObtenerId(new BE.MedioDePago
                {
                    EntidadFinanciera = new BE.EntidadFinanciera { Id = entidadFinancieraId },
                    Tipo = BE.TipoMedioDePago.TarjetaCredito,
                    Marca = "Visa",
                    Segmento = "Gold"
                });

                BE.MedioDePago obtenido = repositorio.ObtenerPorId(id);
                Assert.IsNotNull(obtenido);
                Assert.IsNotNull(obtenido.EntidadFinanciera);
                Assert.AreEqual(entidadFinancieraId, obtenido.EntidadFinanciera.Id);
                Assert.AreEqual("EntidadTestMedioDePagoPropio", obtenido.EntidadFinanciera.Nombre);
                Assert.AreEqual(BE.TipoMedioDePago.TarjetaCredito, obtenido.Tipo);
                Assert.AreEqual("Visa", obtenido.Marca);
                Assert.AreEqual("Gold", obtenido.Segmento);

                accesoDatos.RollbackTransactionScope();
            }
        }

        [TestMethod]
        public void ObtenerPorEntidadFinanciera_ExcluyeMediosDeOtraEntidad()
        {
            using (AccesoDatosSqlServer accesoDatos = new AccesoDatosSqlServer())
            {
                accesoDatos.BeginTransactionScope();
                MedioDePagoRepositorio repositorio = new MedioDePagoRepositorio(accesoDatos);
                int entidadFinancieraId = InsertarEntidadFinancieraTest(accesoDatos, "EntidadTestMedioDePagoFiltroA");
                int otraEntidadFinancieraId = InsertarEntidadFinancieraTest(accesoDatos, "EntidadTestMedioDePagoFiltroB");

                int idPropio = repositorio.InsertarYObtenerId(new BE.MedioDePago
                {
                    EntidadFinanciera = new BE.EntidadFinanciera { Id = entidadFinancieraId },
                    Tipo = BE.TipoMedioDePago.TarjetaDebito,
                    Marca = "Mastercard",
                    Segmento = "Classic"
                });
                int idAjeno = repositorio.InsertarYObtenerId(new BE.MedioDePago
                {
                    EntidadFinanciera = new BE.EntidadFinanciera { Id = otraEntidadFinancieraId },
                    Tipo = BE.TipoMedioDePago.TarjetaDebito,
                    Marca = "Mastercard",
                    Segmento = "Classic"
                });

                System.Collections.Generic.List<BE.MedioDePago> propios = repositorio.ObtenerPorEntidadFinanciera(entidadFinancieraId);

                Assert.IsTrue(propios.Any(item => item.Id == idPropio));
                Assert.IsFalse(propios.Any(item => item.Id == idAjeno));

                accesoDatos.RollbackTransactionScope();
            }
        }

        [TestMethod]
        public void Actualizar_MedioDePagoExistente_PersisteNuevoSegmento()
        {
            using (AccesoDatosSqlServer accesoDatos = new AccesoDatosSqlServer())
            {
                accesoDatos.BeginTransactionScope();
                MedioDePagoRepositorio repositorio = new MedioDePagoRepositorio(accesoDatos);
                int entidadFinancieraId = InsertarEntidadFinancieraTest(accesoDatos, "EntidadTestMedioDePagoOriginal");
                int id = repositorio.InsertarYObtenerId(new BE.MedioDePago
                {
                    EntidadFinanciera = new BE.EntidadFinanciera { Id = entidadFinancieraId },
                    Tipo = BE.TipoMedioDePago.CuentaBilleteraVirtual,
                    Marca = "MercadoPago",
                    Segmento = "Basico"
                });

                bool actualizado = repositorio.Actualizar(new BE.MedioDePago
                {
                    Id = id,
                    EntidadFinanciera = new BE.EntidadFinanciera { Id = entidadFinancieraId },
                    Tipo = BE.TipoMedioDePago.CuentaBilleteraVirtual,
                    Marca = "MercadoPago",
                    Segmento = "Premium"
                });

                Assert.IsTrue(actualizado);
                Assert.AreEqual("Premium", repositorio.ObtenerPorId(id).Segmento);

                accesoDatos.RollbackTransactionScope();
            }
        }

        [TestMethod]
        public void Eliminar_MedioDePagoExistente_DejaDeExistir()
        {
            using (AccesoDatosSqlServer accesoDatos = new AccesoDatosSqlServer())
            {
                accesoDatos.BeginTransactionScope();
                MedioDePagoRepositorio repositorio = new MedioDePagoRepositorio(accesoDatos);
                int entidadFinancieraId = InsertarEntidadFinancieraTest(accesoDatos, "EntidadTestMedioDePagoAEliminar");
                int id = repositorio.InsertarYObtenerId(new BE.MedioDePago
                {
                    EntidadFinanciera = new BE.EntidadFinanciera { Id = entidadFinancieraId },
                    Tipo = BE.TipoMedioDePago.CuentaDNI,
                    Marca = null,
                    Segmento = null
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
    }
}
