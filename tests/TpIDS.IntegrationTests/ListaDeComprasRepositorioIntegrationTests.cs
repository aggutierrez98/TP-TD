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
    public sealed class ListaDeComprasRepositorioIntegrationTests
    {
        [TestMethod]
        public void InsertarYObtenerId_ListaValida_QuedaDisponible()
        {
            using (AccesoDatosSqlServer accesoDatos = new AccesoDatosSqlServer())
            {
                accesoDatos.BeginTransactionScope();
                ListaDeComprasRepositorio repositorio = new ListaDeComprasRepositorio(accesoDatos);
                int usuarioId = ObtenerUsuarioBasicoId(accesoDatos);

                int id = repositorio.InsertarYObtenerId(new BE.ListaDeCompras
                {
                    Usuario = new BE.Usuario { Id = usuarioId },
                    Nombre = "ListaTestPropia",
                    FechaCreacion = new System.DateTime(2026, 1, 15)
                });

                BE.ListaDeCompras obtenida = repositorio.ObtenerPorId(id);
                Assert.IsNotNull(obtenida);
                Assert.AreEqual(usuarioId, obtenida.Usuario.Id);
                Assert.AreEqual("ListaTestPropia", obtenida.Nombre);
                Assert.AreEqual(new System.DateTime(2026, 1, 15), obtenida.FechaCreacion);
                Assert.IsNotNull(obtenida.ItemsLista);
                Assert.AreEqual(0, obtenida.ItemsLista.Count);

                accesoDatos.RollbackTransactionScope();
            }
        }

        [TestMethod]
        public void ObtenerPorUsuario_ExcluyeListasDeOtroUsuario()
        {
            using (AccesoDatosSqlServer accesoDatos = new AccesoDatosSqlServer())
            {
                accesoDatos.BeginTransactionScope();
                ListaDeComprasRepositorio repositorio = new ListaDeComprasRepositorio(accesoDatos);
                int usuarioId = ObtenerUsuarioBasicoId(accesoDatos);
                int otroUsuarioId = ObtenerUsuarioAdminId(accesoDatos);

                int idPropia = repositorio.InsertarYObtenerId(new BE.ListaDeCompras
                {
                    Usuario = new BE.Usuario { Id = usuarioId },
                    Nombre = "ListaTestUsuarioA",
                    FechaCreacion = new System.DateTime(2026, 1, 1)
                });
                int idAjena = repositorio.InsertarYObtenerId(new BE.ListaDeCompras
                {
                    Usuario = new BE.Usuario { Id = otroUsuarioId },
                    Nombre = "ListaTestUsuarioB",
                    FechaCreacion = new System.DateTime(2026, 1, 2)
                });

                System.Collections.Generic.List<BE.ListaDeCompras> propias = repositorio.ObtenerPorUsuario(new BE.Usuario { Id = usuarioId });

                Assert.IsTrue(propias.Any(item => item.Id == idPropia));
                Assert.IsFalse(propias.Any(item => item.Id == idAjena));

                accesoDatos.RollbackTransactionScope();
            }
        }

        [TestMethod]
        public void Actualizar_ListaExistente_PersisteNuevoNombre()
        {
            using (AccesoDatosSqlServer accesoDatos = new AccesoDatosSqlServer())
            {
                accesoDatos.BeginTransactionScope();
                ListaDeComprasRepositorio repositorio = new ListaDeComprasRepositorio(accesoDatos);
                int usuarioId = ObtenerUsuarioBasicoId(accesoDatos);
                int id = repositorio.InsertarYObtenerId(new BE.ListaDeCompras
                {
                    Usuario = new BE.Usuario { Id = usuarioId },
                    Nombre = "ListaTestOriginal",
                    FechaCreacion = new System.DateTime(2026, 1, 3)
                });

                bool actualizada = repositorio.Actualizar(new BE.ListaDeCompras
                {
                    Id = id,
                    Nombre = "ListaTestRenombrada"
                });

                Assert.IsTrue(actualizada);
                Assert.AreEqual("ListaTestRenombrada", repositorio.ObtenerPorId(id).Nombre);

                accesoDatos.RollbackTransactionScope();
            }
        }

        [TestMethod]
        public void Eliminar_ListaExistente_DejaDeExistir()
        {
            using (AccesoDatosSqlServer accesoDatos = new AccesoDatosSqlServer())
            {
                accesoDatos.BeginTransactionScope();
                ListaDeComprasRepositorio repositorio = new ListaDeComprasRepositorio(accesoDatos);
                int usuarioId = ObtenerUsuarioBasicoId(accesoDatos);
                int id = repositorio.InsertarYObtenerId(new BE.ListaDeCompras
                {
                    Usuario = new BE.Usuario { Id = usuarioId },
                    Nombre = "ListaTestAEliminar",
                    FechaCreacion = new System.DateTime(2026, 1, 4)
                });

                bool eliminada = repositorio.Eliminar(repositorio.ObtenerPorId(id));

                Assert.IsTrue(eliminada);
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
    }
}
