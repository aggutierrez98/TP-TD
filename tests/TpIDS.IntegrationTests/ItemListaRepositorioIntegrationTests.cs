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
    public sealed class ItemListaRepositorioIntegrationTests
    {
        [TestMethod]
        public void InsertarYObtenerId_ItemValido_QuedaDisponible()
        {
            using (AccesoDatosSqlServer accesoDatos = new AccesoDatosSqlServer())
            {
                accesoDatos.BeginTransactionScope();
                ItemListaRepositorio repositorio = new ItemListaRepositorio(accesoDatos);
                int usuarioId = ObtenerUsuarioBasicoId(accesoDatos);
                int listaDeComprasId = InsertarListaDeComprasTest(accesoDatos, usuarioId, "ListaTestParaItemPropio");

                int id = repositorio.InsertarYObtenerId(new BE.ItemLista
                {
                    ListaDeCompras = new BE.ListaDeCompras { Id = listaDeComprasId },
                    Descripcion = "Leche descremada 1L",
                    Cantidad = 3,
                    Comprado = false
                });

                BE.ItemLista obtenido = repositorio.ObtenerPorId(id);
                Assert.IsNotNull(obtenido);
                Assert.AreEqual(listaDeComprasId, obtenido.ListaDeCompras.Id);
                Assert.AreEqual("Leche descremada 1L", obtenido.Descripcion);
                Assert.AreEqual(3, obtenido.Cantidad);
                Assert.IsFalse(obtenido.Comprado);

                accesoDatos.RollbackTransactionScope();
            }
        }

        [TestMethod]
        public void ObtenerPorListaDeCompras_ExcluyeItemsDeOtraLista()
        {
            using (AccesoDatosSqlServer accesoDatos = new AccesoDatosSqlServer())
            {
                accesoDatos.BeginTransactionScope();
                ItemListaRepositorio repositorio = new ItemListaRepositorio(accesoDatos);
                int usuarioId = ObtenerUsuarioBasicoId(accesoDatos);
                int listaDeComprasId = InsertarListaDeComprasTest(accesoDatos, usuarioId, "ListaTestFiltroA");
                int otraListaDeComprasId = InsertarListaDeComprasTest(accesoDatos, usuarioId, "ListaTestFiltroB");

                int idPropio = repositorio.InsertarYObtenerId(new BE.ItemLista
                {
                    ListaDeCompras = new BE.ListaDeCompras { Id = listaDeComprasId },
                    Descripcion = "Item propio",
                    Cantidad = 1,
                    Comprado = false
                });
                int idAjeno = repositorio.InsertarYObtenerId(new BE.ItemLista
                {
                    ListaDeCompras = new BE.ListaDeCompras { Id = otraListaDeComprasId },
                    Descripcion = "Item ajeno",
                    Cantidad = 2,
                    Comprado = false
                });

                System.Collections.Generic.List<BE.ItemLista> propios = repositorio.ObtenerPorListaDeCompras(listaDeComprasId);

                Assert.IsTrue(propios.Any(item => item.Id == idPropio));
                Assert.IsFalse(propios.Any(item => item.Id == idAjeno));

                accesoDatos.RollbackTransactionScope();
            }
        }

        [TestMethod]
        public void Actualizar_ItemExistente_PersisteNuevaDescripcionCantidadYComprado()
        {
            using (AccesoDatosSqlServer accesoDatos = new AccesoDatosSqlServer())
            {
                accesoDatos.BeginTransactionScope();
                ItemListaRepositorio repositorio = new ItemListaRepositorio(accesoDatos);
                int usuarioId = ObtenerUsuarioBasicoId(accesoDatos);
                int listaDeComprasId = InsertarListaDeComprasTest(accesoDatos, usuarioId, "ListaTestOriginal");
                int id = repositorio.InsertarYObtenerId(new BE.ItemLista
                {
                    ListaDeCompras = new BE.ListaDeCompras { Id = listaDeComprasId },
                    Descripcion = "Descripcion original",
                    Cantidad = 1,
                    Comprado = false
                });

                bool actualizado = repositorio.Actualizar(new BE.ItemLista
                {
                    Id = id,
                    ListaDeCompras = new BE.ListaDeCompras { Id = listaDeComprasId },
                    Descripcion = "Descripcion actualizada",
                    Cantidad = 5,
                    Comprado = true
                });

                Assert.IsTrue(actualizado);
                BE.ItemLista obtenido = repositorio.ObtenerPorId(id);
                Assert.AreEqual("Descripcion actualizada", obtenido.Descripcion);
                Assert.AreEqual(5, obtenido.Cantidad);
                Assert.IsTrue(obtenido.Comprado);

                accesoDatos.RollbackTransactionScope();
            }
        }

        [TestMethod]
        public void Eliminar_ItemExistente_DejaDeExistir()
        {
            using (AccesoDatosSqlServer accesoDatos = new AccesoDatosSqlServer())
            {
                accesoDatos.BeginTransactionScope();
                ItemListaRepositorio repositorio = new ItemListaRepositorio(accesoDatos);
                int usuarioId = ObtenerUsuarioBasicoId(accesoDatos);
                int listaDeComprasId = InsertarListaDeComprasTest(accesoDatos, usuarioId, "ListaTestAEliminar");
                int id = repositorio.InsertarYObtenerId(new BE.ItemLista
                {
                    ListaDeCompras = new BE.ListaDeCompras { Id = listaDeComprasId },
                    Descripcion = "Item a eliminar",
                    Cantidad = 1,
                    Comprado = false
                });

                bool eliminado = repositorio.Eliminar(id);

                Assert.IsTrue(eliminado);
                Assert.IsNull(repositorio.ObtenerPorId(id));

                accesoDatos.RollbackTransactionScope();
            }
        }

        private static int ObtenerUsuarioBasicoId(AccesoDatosSqlServer accesoDatos)
        {
            return new UsuarioDAL().ObtenerPorUsername("user", accesoDatos).Id;
        }

        private static int InsertarListaDeComprasTest(AccesoDatosSqlServer accesoDatos, int usuarioId, string nombre)
        {
            return new ListaDeComprasRepositorio(accesoDatos).InsertarYObtenerId(new BE.ListaDeCompras
            {
                Usuario = new BE.Usuario { Id = usuarioId },
                Nombre = nombre,
                FechaCreacion = new System.DateTime(2026, 3, 1)
            });
        }
    }
}
