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
    public sealed class ComercioRepositorioIntegrationTests
    {
        [TestMethod]
        public void InsertarYObtenerId_ComercioValido_QuedaDisponible()
        {
            using (AccesoDatosSqlServer accesoDatos = new AccesoDatosSqlServer())
            {
                accesoDatos.BeginTransactionScope();
                ComercioRepositorio repositorio = new ComercioRepositorio(accesoDatos);
                int rubroComercioId = InsertarRubroComercioTest(accesoDatos, "RubroTestComercioPropio");

                int id = repositorio.InsertarYObtenerId(new BE.Comercio
                {
                    RubroComercio = new BE.RubroComercio { Id = rubroComercioId },
                    Nombre = "ComercioTestPropio",
                    Ubicacion = "UbicacionTest"
                });

                BE.Comercio obtenido = repositorio.ObtenerPorId(id);
                Assert.IsNotNull(obtenido);
                Assert.IsNotNull(obtenido.RubroComercio);
                Assert.AreEqual(rubroComercioId, obtenido.RubroComercio.Id);
                Assert.AreEqual("RubroTestComercioPropio", obtenido.RubroComercio.Nombre);
                Assert.AreEqual("ComercioTestPropio", obtenido.Nombre);
                Assert.AreEqual("UbicacionTest", obtenido.Ubicacion);

                accesoDatos.RollbackTransactionScope();
            }
        }

        [TestMethod]
        public void ObtenerPorRubro_ExcluyeComerciosDeOtroRubro()
        {
            using (AccesoDatosSqlServer accesoDatos = new AccesoDatosSqlServer())
            {
                accesoDatos.BeginTransactionScope();
                ComercioRepositorio repositorio = new ComercioRepositorio(accesoDatos);
                int rubroComercioId = InsertarRubroComercioTest(accesoDatos, "RubroTestComercioFiltroA");
                int otroRubroComercioId = InsertarRubroComercioTest(accesoDatos, "RubroTestComercioFiltroB");

                int idPropio = repositorio.InsertarYObtenerId(new BE.Comercio
                {
                    RubroComercio = new BE.RubroComercio { Id = rubroComercioId },
                    Nombre = "ComercioTestRubroA"
                });
                int idAjeno = repositorio.InsertarYObtenerId(new BE.Comercio
                {
                    RubroComercio = new BE.RubroComercio { Id = otroRubroComercioId },
                    Nombre = "ComercioTestRubroB"
                });

                System.Collections.Generic.List<BE.Comercio> propios = repositorio.ObtenerPorRubro(rubroComercioId);

                Assert.IsTrue(propios.Any(item => item.Id == idPropio));
                Assert.IsFalse(propios.Any(item => item.Id == idAjeno));

                accesoDatos.RollbackTransactionScope();
            }
        }

        [TestMethod]
        public void Actualizar_ComercioExistente_PersisteNuevoNombre()
        {
            using (AccesoDatosSqlServer accesoDatos = new AccesoDatosSqlServer())
            {
                accesoDatos.BeginTransactionScope();
                ComercioRepositorio repositorio = new ComercioRepositorio(accesoDatos);
                int rubroComercioId = InsertarRubroComercioTest(accesoDatos, "RubroTestComercioOriginal");
                int id = repositorio.InsertarYObtenerId(new BE.Comercio
                {
                    RubroComercio = new BE.RubroComercio { Id = rubroComercioId },
                    Nombre = "ComercioTestOriginal",
                    Ubicacion = "UbicacionOriginal"
                });

                bool actualizado = repositorio.Actualizar(new BE.Comercio
                {
                    Id = id,
                    RubroComercio = new BE.RubroComercio { Id = rubroComercioId },
                    Nombre = "ComercioTestRenombrado",
                    Ubicacion = "UbicacionNueva"
                });

                Assert.IsTrue(actualizado);
                BE.Comercio releido = repositorio.ObtenerPorId(id);
                Assert.AreEqual("ComercioTestRenombrado", releido.Nombre);
                Assert.AreEqual("UbicacionNueva", releido.Ubicacion);

                accesoDatos.RollbackTransactionScope();
            }
        }

        [TestMethod]
        public void Eliminar_ComercioExistente_DejaDeExistir()
        {
            using (AccesoDatosSqlServer accesoDatos = new AccesoDatosSqlServer())
            {
                accesoDatos.BeginTransactionScope();
                ComercioRepositorio repositorio = new ComercioRepositorio(accesoDatos);
                int rubroComercioId = InsertarRubroComercioTest(accesoDatos, "RubroTestComercioAEliminar");
                int id = repositorio.InsertarYObtenerId(new BE.Comercio
                {
                    RubroComercio = new BE.RubroComercio { Id = rubroComercioId },
                    Nombre = "ComercioTestAEliminar"
                });

                bool eliminado = repositorio.Eliminar(id);

                Assert.IsTrue(eliminado);
                Assert.IsNull(repositorio.ObtenerPorId(id));

                accesoDatos.RollbackTransactionScope();
            }
        }

        private static int InsertarRubroComercioTest(AccesoDatosSqlServer accesoDatos, string nombre)
        {
            return new RubroComercioRepositorio(accesoDatos).InsertarYObtenerId(new BE.RubroComercio { Nombre = nombre });
        }
    }
}
