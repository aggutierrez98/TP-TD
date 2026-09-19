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
    public sealed class CategoriaProductoRepositorioIntegrationTests
    {
        [TestMethod]
        public void InsertarYObtenerId_CategoriaValida_QuedaDisponible()
        {
            using (AccesoDatosSqlServer accesoDatos = new AccesoDatosSqlServer())
            {
                accesoDatos.BeginTransactionScope();
                CategoriaProductoRepositorio repositorio = new CategoriaProductoRepositorio(accesoDatos);

                int id = repositorio.InsertarYObtenerId(new BE.CategoriaProducto
                {
                    Nombre = "CategoriaTestLimpieza"
                });

                BE.CategoriaProducto obtenida = repositorio.ObtenerPorId(id);
                Assert.IsNotNull(obtenida);
                Assert.AreEqual("CategoriaTestLimpieza", obtenida.Nombre);

                accesoDatos.RollbackTransactionScope();
            }
        }

        [TestMethod]
        public void ObtenerTodas_IncluyeInsertada()
        {
            using (AccesoDatosSqlServer accesoDatos = new AccesoDatosSqlServer())
            {
                accesoDatos.BeginTransactionScope();
                CategoriaProductoRepositorio repositorio = new CategoriaProductoRepositorio(accesoDatos);

                int id = repositorio.InsertarYObtenerId(new BE.CategoriaProducto { Nombre = "CategoriaTestBebidas" });

                System.Collections.Generic.List<BE.CategoriaProducto> todas = repositorio.ObtenerTodas();

                Assert.IsTrue(todas.Any(item => item.Id == id && item.Nombre == "CategoriaTestBebidas"));

                accesoDatos.RollbackTransactionScope();
            }
        }

        [TestMethod]
        public void Actualizar_CategoriaExistente_PersisteNuevoNombre()
        {
            using (AccesoDatosSqlServer accesoDatos = new AccesoDatosSqlServer())
            {
                accesoDatos.BeginTransactionScope();
                CategoriaProductoRepositorio repositorio = new CategoriaProductoRepositorio(accesoDatos);
                int id = repositorio.InsertarYObtenerId(new BE.CategoriaProducto { Nombre = "CategoriaTestOriginal" });

                bool actualizado = repositorio.Actualizar(new BE.CategoriaProducto { Id = id, Nombre = "CategoriaTestRenombrada" });

                Assert.IsTrue(actualizado);
                Assert.AreEqual("CategoriaTestRenombrada", repositorio.ObtenerPorId(id).Nombre);

                accesoDatos.RollbackTransactionScope();
            }
        }

        [TestMethod]
        public void Eliminar_CategoriaExistente_DejaDeExistir()
        {
            using (AccesoDatosSqlServer accesoDatos = new AccesoDatosSqlServer())
            {
                accesoDatos.BeginTransactionScope();
                CategoriaProductoRepositorio repositorio = new CategoriaProductoRepositorio(accesoDatos);
                int id = repositorio.InsertarYObtenerId(new BE.CategoriaProducto { Nombre = "CategoriaTestAEliminar" });

                bool eliminado = repositorio.Eliminar(id);

                Assert.IsTrue(eliminado);
                Assert.IsNull(repositorio.ObtenerPorId(id));

                accesoDatos.RollbackTransactionScope();
            }
        }
    }
}
