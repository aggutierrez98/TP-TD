using System.Linq;
using DAL;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using REPOSITORIO;

namespace TpIDS.IntegrationTests
{
    // Patron de aislamiento: cada test abre su propia transaccion sobre
    // AccesoDatosSqlServer y hace RollbackTransactionScope() al final (via Dispose),
    // asi que ningun test deja datos permanentes en TpCatedraIDS_Test.
    [TestClass]
    [TestCategory("Integration")]
    public sealed class CategoriaRepositorioIntegrationTests
    {
        [TestMethod]
        public void InsertarYObtenerId_CategoriaPropia_QuedaVisibleParaSuDuenio()
        {
            using (AccesoDatosSqlServer accesoDatos = new AccesoDatosSqlServer())
            {
                accesoDatos.BeginTransactionScope();
                CategoriaRepositorio repositorio = new CategoriaRepositorio(accesoDatos);
                int usuarioId = ObtenerUsuarioBasicoId(accesoDatos);

                int id = repositorio.InsertarYObtenerId(new BE.Categoria
                {
                    Nombre = "CategoriaTestPropia",
                    UsuarioId = usuarioId
                });

                BE.Categoria obtenida = repositorio.ObtenerPorId(id);
                Assert.IsNotNull(obtenida);
                Assert.AreEqual("CategoriaTestPropia", obtenida.Nombre);
                Assert.AreEqual(usuarioId, obtenida.UsuarioId);

                System.Collections.Generic.List<BE.Categoria> visibles = repositorio.ObtenerVisiblesParaUsuario(usuarioId);
                Assert.IsTrue(visibles.Any(item => item.Id == id));

                accesoDatos.RollbackTransactionScope();
            }
        }

        [TestMethod]
        public void ObtenerVisiblesParaUsuario_IncluyeGlobalesYExcluyePropiasDeOtroUsuario()
        {
            using (AccesoDatosSqlServer accesoDatos = new AccesoDatosSqlServer())
            {
                accesoDatos.BeginTransactionScope();
                CategoriaRepositorio repositorio = new CategoriaRepositorio(accesoDatos);
                int usuarioId = ObtenerUsuarioBasicoId(accesoDatos);
                int otroUsuarioId = ObtenerUsuarioAdminId(accesoDatos);

                int idPropia = repositorio.InsertarYObtenerId(new BE.Categoria { Nombre = "CategoriaTestUsuarioA", UsuarioId = usuarioId });
                int idAjena = repositorio.InsertarYObtenerId(new BE.Categoria { Nombre = "CategoriaTestUsuarioB", UsuarioId = otroUsuarioId });

                System.Collections.Generic.List<BE.Categoria> visibles = repositorio.ObtenerVisiblesParaUsuario(usuarioId);

                Assert.IsTrue(visibles.Any(item => item.Id == idPropia));
                Assert.IsTrue(visibles.Any(item => item.UsuarioId == null), "Debe incluir las categorias globales sembradas.");
                Assert.IsFalse(visibles.Any(item => item.Id == idAjena));

                accesoDatos.RollbackTransactionScope();
            }
        }

        [TestMethod]
        public void Actualizar_CategoriaExistente_PersisteNuevoNombre()
        {
            using (AccesoDatosSqlServer accesoDatos = new AccesoDatosSqlServer())
            {
                accesoDatos.BeginTransactionScope();
                CategoriaRepositorio repositorio = new CategoriaRepositorio(accesoDatos);
                int usuarioId = ObtenerUsuarioBasicoId(accesoDatos);
                int id = repositorio.InsertarYObtenerId(new BE.Categoria { Nombre = "CategoriaTestOriginal", UsuarioId = usuarioId });

                bool actualizado = repositorio.Actualizar(new BE.Categoria { Id = id, Nombre = "CategoriaTestRenombrada", UsuarioId = usuarioId });

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
                CategoriaRepositorio repositorio = new CategoriaRepositorio(accesoDatos);
                int usuarioId = ObtenerUsuarioBasicoId(accesoDatos);
                int id = repositorio.InsertarYObtenerId(new BE.Categoria { Nombre = "CategoriaTestAEliminar", UsuarioId = usuarioId });

                bool eliminado = repositorio.Eliminar(id);

                Assert.IsTrue(eliminado);
                Assert.IsNull(repositorio.ObtenerPorId(id));

                accesoDatos.RollbackTransactionScope();
            }
        }

        [TestMethod]
        public void ExisteNombreEnScope_RespetaScopeGlobalYPersonal()
        {
            using (AccesoDatosSqlServer accesoDatos = new AccesoDatosSqlServer())
            {
                accesoDatos.BeginTransactionScope();
                CategoriaRepositorio repositorio = new CategoriaRepositorio(accesoDatos);
                int usuarioId = ObtenerUsuarioBasicoId(accesoDatos);

                Assert.IsTrue(repositorio.ExisteNombreEnScope("Alimentacion", null, null), "Categoria global sembrada por script_bd.sql.");
                Assert.IsFalse(repositorio.ExisteNombreEnScope("Alimentacion", usuarioId, null), "El scope personal no debe ver el nombre global como propio.");

                int id = repositorio.InsertarYObtenerId(new BE.Categoria { Nombre = "CategoriaTestDuplicado", UsuarioId = usuarioId });
                Assert.IsTrue(repositorio.ExisteNombreEnScope("CategoriaTestDuplicado", usuarioId, null));
                Assert.IsFalse(repositorio.ExisteNombreEnScope("CategoriaTestDuplicado", usuarioId, id), "Excluyendo su propio Id no debe considerarse duplicado.");

                accesoDatos.RollbackTransactionScope();
            }
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
