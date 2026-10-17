using System.Collections.Generic;
using System.Linq;
using DAL;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using REPOSITORIO;
using SERVICIOS;

namespace TpIDS.IntegrationTests
{
    // Patron de aislamiento: cada test abre su propia transaccion sobre
    // AccesoDatosSqlServer y hace RollbackTransactionScope() al final,
    // asi que ningun test deja datos permanentes en TpCatedraIDS_Test.
    [TestClass]
    [TestCategory("Integration")]
    public sealed class MedioPagoRepositorioIntegrationTests
    {
        [TestMethod]
        public void InsertarYObtenerId_MedioPagoValido_QuedaDisponible()
        {
            using (AccesoDatosSqlServer accesoDatos = new AccesoDatosSqlServer())
            {
                accesoDatos.BeginTransactionScope();
                MedioPagoRepositorio repositorio = new MedioPagoRepositorio(accesoDatos);

                int id = repositorio.InsertarYObtenerId(new BE.MedioPago
                {
                    Tipo = BE.TipoMedioPago.TarjetaCredito,
                    Marca = "MarcaTestPropia"
                });

                BE.MedioPago obtenido = repositorio.ObtenerPorId(id);
                Assert.IsNotNull(obtenido);
                Assert.AreEqual(BE.TipoMedioPago.TarjetaCredito, obtenido.Tipo);
                Assert.AreEqual("MarcaTestPropia", obtenido.Marca);

                accesoDatos.RollbackTransactionScope();
            }
        }

        [TestMethod]
        public void ObtenerTodos_IncluyeInsertado()
        {
            using (AccesoDatosSqlServer accesoDatos = new AccesoDatosSqlServer())
            {
                accesoDatos.BeginTransactionScope();
                MedioPagoRepositorio repositorio = new MedioPagoRepositorio(accesoDatos);

                int id = repositorio.InsertarYObtenerId(new BE.MedioPago
                {
                    Tipo = BE.TipoMedioPago.CuentaDNI,
                    Marca = null
                });

                List<BE.MedioPago> todos = repositorio.ObtenerTodos();

                Assert.IsTrue(todos.Any(item => item.Id == id && item.Tipo == BE.TipoMedioPago.CuentaDNI && item.Marca == null));

                accesoDatos.RollbackTransactionScope();
            }
        }

        [TestMethod]
        public void InsertarYObtenerId_TipoYMarcaDuplicados_FallaPorUnicidad()
        {
            using (AccesoDatosSqlServer accesoDatos = new AccesoDatosSqlServer())
            {
                accesoDatos.BeginTransactionScope();
                MedioPagoRepositorio repositorio = new MedioPagoRepositorio(accesoDatos);
                repositorio.InsertarYObtenerId(new BE.MedioPago { Tipo = BE.TipoMedioPago.TarjetaDebito, Marca = "MarcaTestDuplicada" });

                Assert.ThrowsExactly<ExcepcionTecnica>(() =>
                    repositorio.InsertarYObtenerId(new BE.MedioPago { Tipo = BE.TipoMedioPago.TarjetaDebito, Marca = "MarcaTestDuplicada" }));

                accesoDatos.RollbackTransactionScope();
            }
        }

        [TestMethod]
        public void Actualizar_MedioPagoExistente_PersisteNuevaMarca()
        {
            using (AccesoDatosSqlServer accesoDatos = new AccesoDatosSqlServer())
            {
                accesoDatos.BeginTransactionScope();
                MedioPagoRepositorio repositorio = new MedioPagoRepositorio(accesoDatos);
                int id = repositorio.InsertarYObtenerId(new BE.MedioPago { Tipo = BE.TipoMedioPago.TarjetaPrepaga, Marca = "MarcaTestOriginal" });

                bool actualizado = repositorio.Actualizar(new BE.MedioPago
                {
                    Id = id,
                    Tipo = BE.TipoMedioPago.CuentaBilleteraVirtual,
                    Marca = "MarcaTestRenombrada"
                });

                Assert.IsTrue(actualizado);
                BE.MedioPago releido = repositorio.ObtenerPorId(id);
                Assert.AreEqual(BE.TipoMedioPago.CuentaBilleteraVirtual, releido.Tipo);
                Assert.AreEqual("MarcaTestRenombrada", releido.Marca);

                accesoDatos.RollbackTransactionScope();
            }
        }

        [TestMethod]
        public void Eliminar_MedioPagoExistente_DejaDeExistir()
        {
            using (AccesoDatosSqlServer accesoDatos = new AccesoDatosSqlServer())
            {
                accesoDatos.BeginTransactionScope();
                MedioPagoRepositorio repositorio = new MedioPagoRepositorio(accesoDatos);
                int id = repositorio.InsertarYObtenerId(new BE.MedioPago { Tipo = BE.TipoMedioPago.TarjetaCredito, Marca = "MarcaTestAEliminar" });

                bool eliminado = repositorio.Eliminar(id);

                Assert.IsTrue(eliminado);
                Assert.IsNull(repositorio.ObtenerPorId(id));

                accesoDatos.RollbackTransactionScope();
            }
        }
    }
}
