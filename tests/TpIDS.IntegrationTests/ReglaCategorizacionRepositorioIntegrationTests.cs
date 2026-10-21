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
    public sealed class ReglaCategorizacionRepositorioIntegrationTests
    {
        [TestMethod]
        public void InsertarYObtenerId_ReglaValida_QuedaDisponible()
        {
            using (AccesoDatosSqlServer accesoDatos = new AccesoDatosSqlServer())
            {
                accesoDatos.BeginTransactionScope();
                ReglaCategorizacionRepositorio repositorio = new ReglaCategorizacionRepositorio(accesoDatos);
                int usuarioId = ObtenerUsuarioBasicoId(accesoDatos);
                int categoriaId = InsertarCategoriaTest(accesoDatos, usuarioId, "CategoriaTestReglaPropia");

                int id = repositorio.InsertarYObtenerId(new BE.ReglaCategorizacion
                {
                    Usuario = new BE.Usuario { Id = usuarioId },
                    Categoria = new BE.Categoria { Id = categoriaId },
                    Nombre = "ReglaTestPropia",
                    PalabrasClave = "super,mercado"
                });

                BE.ReglaCategorizacion obtenida = repositorio.ObtenerPorId(id);
                Assert.IsNotNull(obtenida);
                Assert.IsNotNull(obtenida.Usuario);
                Assert.AreEqual(usuarioId, obtenida.Usuario.Id);
                Assert.IsNotNull(obtenida.Categoria);
                Assert.AreEqual(categoriaId, obtenida.Categoria.Id);
                Assert.AreEqual("CategoriaTestReglaPropia", obtenida.Categoria.Nombre);
                Assert.AreEqual("ReglaTestPropia", obtenida.Nombre);
                Assert.AreEqual("super,mercado", obtenida.PalabrasClave);

                accesoDatos.RollbackTransactionScope();
            }
        }

        [TestMethod]
        public void ObtenerPorUsuario_ExcluyeReglasDeOtroUsuario()
        {
            using (AccesoDatosSqlServer accesoDatos = new AccesoDatosSqlServer())
            {
                accesoDatos.BeginTransactionScope();
                ReglaCategorizacionRepositorio repositorio = new ReglaCategorizacionRepositorio(accesoDatos);
                int usuarioId = ObtenerUsuarioBasicoId(accesoDatos);
                int otroUsuarioId = ObtenerUsuarioAdminId(accesoDatos);
                int categoriaId = InsertarCategoriaTest(accesoDatos, usuarioId, "CategoriaTestReglaFiltroA");
                int otraCategoriaId = InsertarCategoriaTest(accesoDatos, otroUsuarioId, "CategoriaTestReglaFiltroB");

                int idPropia = repositorio.InsertarYObtenerId(new BE.ReglaCategorizacion
                {
                    Usuario = new BE.Usuario { Id = usuarioId },
                    Categoria = new BE.Categoria { Id = categoriaId },
                    Nombre = "ReglaTestFiltroPropia",
                    PalabrasClave = "propia"
                });
                int idAjena = repositorio.InsertarYObtenerId(new BE.ReglaCategorizacion
                {
                    Usuario = new BE.Usuario { Id = otroUsuarioId },
                    Categoria = new BE.Categoria { Id = otraCategoriaId },
                    Nombre = "ReglaTestFiltroAjena",
                    PalabrasClave = "ajena"
                });

                System.Collections.Generic.List<BE.ReglaCategorizacion> propias = repositorio.ObtenerPorUsuario(new BE.Usuario { Id = usuarioId });

                Assert.IsTrue(propias.Any(item => item.Id == idPropia));
                Assert.IsFalse(propias.Any(item => item.Id == idAjena));

                accesoDatos.RollbackTransactionScope();
            }
        }

        [TestMethod]
        public void Actualizar_ReglaExistente_PersisteNuevasPalabrasClave()
        {
            using (AccesoDatosSqlServer accesoDatos = new AccesoDatosSqlServer())
            {
                accesoDatos.BeginTransactionScope();
                ReglaCategorizacionRepositorio repositorio = new ReglaCategorizacionRepositorio(accesoDatos);
                int usuarioId = ObtenerUsuarioBasicoId(accesoDatos);
                int categoriaId = InsertarCategoriaTest(accesoDatos, usuarioId, "CategoriaTestReglaOriginal");
                int otraCategoriaId = InsertarCategoriaTest(accesoDatos, usuarioId, "CategoriaTestReglaNueva");
                int id = repositorio.InsertarYObtenerId(new BE.ReglaCategorizacion
                {
                    Usuario = new BE.Usuario { Id = usuarioId },
                    Categoria = new BE.Categoria { Id = categoriaId },
                    Nombre = "ReglaTestOriginal",
                    PalabrasClave = "original"
                });

                bool actualizado = repositorio.Actualizar(new BE.ReglaCategorizacion
                {
                    Id = id,
                    Usuario = new BE.Usuario { Id = usuarioId },
                    Categoria = new BE.Categoria { Id = otraCategoriaId },
                    Nombre = "ReglaTestOriginal",
                    PalabrasClave = "nueva,palabra"
                });

                Assert.IsTrue(actualizado);
                BE.ReglaCategorizacion obtenida = repositorio.ObtenerPorId(id);
                Assert.AreEqual("nueva,palabra", obtenida.PalabrasClave);
                Assert.AreEqual(otraCategoriaId, obtenida.Categoria.Id);

                accesoDatos.RollbackTransactionScope();
            }
        }

        [TestMethod]
        public void Eliminar_ReglaExistente_DejaDeExistir()
        {
            using (AccesoDatosSqlServer accesoDatos = new AccesoDatosSqlServer())
            {
                accesoDatos.BeginTransactionScope();
                ReglaCategorizacionRepositorio repositorio = new ReglaCategorizacionRepositorio(accesoDatos);
                int usuarioId = ObtenerUsuarioBasicoId(accesoDatos);
                int categoriaId = InsertarCategoriaTest(accesoDatos, usuarioId, "CategoriaTestReglaAEliminar");
                int id = repositorio.InsertarYObtenerId(new BE.ReglaCategorizacion
                {
                    Usuario = new BE.Usuario { Id = usuarioId },
                    Categoria = new BE.Categoria { Id = categoriaId },
                    Nombre = "ReglaTestAEliminar",
                    PalabrasClave = "eliminar"
                });

                bool eliminado = repositorio.Eliminar(new BE.ReglaCategorizacion { Id = id });

                Assert.IsTrue(eliminado);
                Assert.IsNull(repositorio.ObtenerPorId(id));

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

        private static int InsertarCategoriaTest(AccesoDatosSqlServer accesoDatos, int usuarioId, string nombre)
        {
            return new CategoriaRepositorio(accesoDatos).InsertarYObtenerId(new BE.Categoria { Nombre = nombre, Usuario = new BE.Usuario { Id = usuarioId } });
        }
    }
}
