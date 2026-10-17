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
    public sealed class EntidadFinancieraRepositorioIntegrationTests
    {
        [TestMethod]
        public void InsertarYObtenerId_EntidadValida_QuedaDisponible()
        {
            using (AccesoDatosSqlServer accesoDatos = new AccesoDatosSqlServer())
            {
                accesoDatos.BeginTransactionScope();
                EntidadFinancieraRepositorio repositorio = new EntidadFinancieraRepositorio(accesoDatos);

                int id = repositorio.InsertarYObtenerId(new BE.EntidadFinanciera
                {
                    Nombre = "BancoTestGalicia"
                });

                BE.EntidadFinanciera obtenida = repositorio.ObtenerPorId(id);
                Assert.IsNotNull(obtenida);
                Assert.AreEqual("BancoTestGalicia", obtenida.Nombre);

                accesoDatos.RollbackTransactionScope();
            }
        }

        [TestMethod]
        public void ObtenerTodas_IncluyeInsertada()
        {
            using (AccesoDatosSqlServer accesoDatos = new AccesoDatosSqlServer())
            {
                accesoDatos.BeginTransactionScope();
                EntidadFinancieraRepositorio repositorio = new EntidadFinancieraRepositorio(accesoDatos);

                int id = repositorio.InsertarYObtenerId(new BE.EntidadFinanciera { Nombre = "BancoTestNacion" });

                System.Collections.Generic.List<BE.EntidadFinanciera> todas = repositorio.ObtenerTodas();

                Assert.IsTrue(todas.Any(item => item.Id == id && item.Nombre == "BancoTestNacion"));

                accesoDatos.RollbackTransactionScope();
            }
        }

        [TestMethod]
        public void ObtenerPorId_ConMediosPagoEntidad_HidrataElCatalogoConSuMedioPago()
        {
            using (AccesoDatosSqlServer accesoDatos = new AccesoDatosSqlServer())
            {
                accesoDatos.BeginTransactionScope();
                EntidadFinancieraRepositorio repositorio = new EntidadFinancieraRepositorio(accesoDatos);
                int id = repositorio.InsertarYObtenerId(new BE.EntidadFinanciera { Nombre = "BancoTestCatalogo" });
                int medioPagoId = new MedioPagoRepositorio(accesoDatos).InsertarYObtenerId(new BE.MedioPago
                {
                    Tipo = BE.TipoMedioPago.TarjetaCredito,
                    Marca = "MarcaTestCatalogoEntidad"
                });
                int medioPagoEntidadId = new MedioPagoEntidadRepositorio(accesoDatos).InsertarYObtenerId(new BE.MedioPagoEntidad
                {
                    EntidadFinanciera = new BE.EntidadFinanciera { Id = id },
                    MedioPago = new BE.MedioPago { Id = medioPagoId },
                    Segmento = "Eminent"
                });

                BE.EntidadFinanciera obtenida = repositorio.ObtenerPorId(id);

                Assert.AreEqual(1, obtenida.MediosPagoEntidad.Count);
                BE.MedioPagoEntidad item = obtenida.MediosPagoEntidad[0];
                Assert.AreEqual(medioPagoEntidadId, item.Id);
                Assert.AreEqual("Eminent", item.Segmento);
                Assert.AreEqual(BE.TipoMedioPago.TarjetaCredito, item.MedioPago.Tipo);
                Assert.AreEqual("MarcaTestCatalogoEntidad", item.MedioPago.Marca);
                Assert.AreSame(obtenida, item.EntidadFinanciera);

                accesoDatos.RollbackTransactionScope();
            }
        }

        [TestMethod]
        public void Actualizar_EntidadExistente_PersisteNuevoNombre()
        {
            using (AccesoDatosSqlServer accesoDatos = new AccesoDatosSqlServer())
            {
                accesoDatos.BeginTransactionScope();
                EntidadFinancieraRepositorio repositorio = new EntidadFinancieraRepositorio(accesoDatos);
                int id = repositorio.InsertarYObtenerId(new BE.EntidadFinanciera { Nombre = "BancoTestOriginal" });

                bool actualizado = repositorio.Actualizar(new BE.EntidadFinanciera { Id = id, Nombre = "BancoTestRenombrado" });

                Assert.IsTrue(actualizado);
                Assert.AreEqual("BancoTestRenombrado", repositorio.ObtenerPorId(id).Nombre);

                accesoDatos.RollbackTransactionScope();
            }
        }

        [TestMethod]
        public void Eliminar_EntidadExistente_DejaDeExistir()
        {
            using (AccesoDatosSqlServer accesoDatos = new AccesoDatosSqlServer())
            {
                accesoDatos.BeginTransactionScope();
                EntidadFinancieraRepositorio repositorio = new EntidadFinancieraRepositorio(accesoDatos);
                int id = repositorio.InsertarYObtenerId(new BE.EntidadFinanciera { Nombre = "BancoTestAEliminar" });

                bool eliminado = repositorio.Eliminar(id);

                Assert.IsTrue(eliminado);
                Assert.IsNull(repositorio.ObtenerPorId(id));

                accesoDatos.RollbackTransactionScope();
            }
        }
    }
}
