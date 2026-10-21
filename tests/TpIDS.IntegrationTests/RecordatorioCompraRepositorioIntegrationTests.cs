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
    public sealed class RecordatorioCompraRepositorioIntegrationTests
    {
        [TestMethod]
        public void InsertarYObtenerId_ConPromocion_QuedaDisponible()
        {
            using (AccesoDatosSqlServer accesoDatos = new AccesoDatosSqlServer())
            {
                accesoDatos.BeginTransactionScope();
                RecordatorioCompraRepositorio repositorio = new RecordatorioCompraRepositorio(accesoDatos);
                int usuarioId = ObtenerUsuarioBasicoId(accesoDatos);
                int listaDeComprasId = InsertarListaDeComprasTest(accesoDatos, usuarioId, "ListaTestConPromocion");
                int itemListaId = InsertarItemListaTest(accesoDatos, listaDeComprasId, "ItemTestConPromocion");
                int promocionId = InsertarPromocionTest(accesoDatos, "PromocionTestParaRecordatorio");

                int id = repositorio.InsertarYObtenerId(new BE.RecordatorioCompra
                {
                    Usuario = new BE.Usuario { Id = usuarioId },
                    Mensaje = "RecordatorioTestConPromocion",
                    Fecha = new System.DateTime(2026, 2, 10),
                    Leida = false,
                    ItemLista = new BE.ItemLista { Id = itemListaId },
                    Promocion = new BE.Promocion { Id = promocionId },
                    FechaVencimiento = new System.DateTime(2026, 2, 20)
                });

                BE.RecordatorioCompra obtenido = repositorio.ObtenerPorId(id);
                Assert.IsNotNull(obtenido);
                Assert.AreEqual(usuarioId, obtenido.Usuario.Id);
                Assert.AreEqual("RecordatorioTestConPromocion", obtenido.Mensaje);
                Assert.AreEqual(new System.DateTime(2026, 2, 10), obtenido.Fecha);
                Assert.IsFalse(obtenido.Leida);
                Assert.AreEqual(itemListaId, obtenido.ItemLista.Id);
                Assert.IsNotNull(obtenido.Promocion);
                Assert.AreEqual(promocionId, obtenido.Promocion.Id);
                Assert.AreEqual(new System.DateTime(2026, 2, 20), obtenido.FechaVencimiento);

                accesoDatos.RollbackTransactionScope();
            }
        }

        [TestMethod]
        public void InsertarYObtenerId_SinPromocion_QuedaDisponible()
        {
            using (AccesoDatosSqlServer accesoDatos = new AccesoDatosSqlServer())
            {
                accesoDatos.BeginTransactionScope();
                RecordatorioCompraRepositorio repositorio = new RecordatorioCompraRepositorio(accesoDatos);
                int usuarioId = ObtenerUsuarioBasicoId(accesoDatos);
                int listaDeComprasId = InsertarListaDeComprasTest(accesoDatos, usuarioId, "ListaTestSinPromocion");
                int itemListaId = InsertarItemListaTest(accesoDatos, listaDeComprasId, "ItemTestSinPromocion");

                int id = repositorio.InsertarYObtenerId(new BE.RecordatorioCompra
                {
                    Usuario = new BE.Usuario { Id = usuarioId },
                    Mensaje = "RecordatorioTestSinPromocion",
                    Fecha = new System.DateTime(2026, 2, 11),
                    Leida = false,
                    ItemLista = new BE.ItemLista { Id = itemListaId },
                    Promocion = null,
                    FechaVencimiento = new System.DateTime(2026, 2, 21)
                });

                BE.RecordatorioCompra obtenido = repositorio.ObtenerPorId(id);
                Assert.IsNotNull(obtenido);
                Assert.AreEqual(itemListaId, obtenido.ItemLista.Id);
                Assert.IsNull(obtenido.Promocion);
                Assert.AreEqual(new System.DateTime(2026, 2, 21), obtenido.FechaVencimiento);

                accesoDatos.RollbackTransactionScope();
            }
        }

        [TestMethod]
        public void ObtenerPorUsuario_ExcluyeDeOtroUsuario()
        {
            using (AccesoDatosSqlServer accesoDatos = new AccesoDatosSqlServer())
            {
                accesoDatos.BeginTransactionScope();
                RecordatorioCompraRepositorio repositorio = new RecordatorioCompraRepositorio(accesoDatos);
                int usuarioId = ObtenerUsuarioBasicoId(accesoDatos);
                int otroUsuarioId = ObtenerUsuarioAdminId(accesoDatos);
                int listaDeComprasId = InsertarListaDeComprasTest(accesoDatos, usuarioId, "ListaTestFiltroPropia");
                int itemListaId = InsertarItemListaTest(accesoDatos, listaDeComprasId, "ItemTestFiltroPropia");
                int otraListaDeComprasId = InsertarListaDeComprasTest(accesoDatos, otroUsuarioId, "ListaTestFiltroAjena");
                int otroItemListaId = InsertarItemListaTest(accesoDatos, otraListaDeComprasId, "ItemTestFiltroAjena");

                int idPropio = repositorio.InsertarYObtenerId(new BE.RecordatorioCompra
                {
                    Usuario = new BE.Usuario { Id = usuarioId },
                    Mensaje = "RecordatorioTestPropio",
                    Fecha = new System.DateTime(2026, 2, 12),
                    Leida = false,
                    ItemLista = new BE.ItemLista { Id = itemListaId },
                    Promocion = null,
                    FechaVencimiento = new System.DateTime(2026, 2, 22)
                });
                int idAjeno = repositorio.InsertarYObtenerId(new BE.RecordatorioCompra
                {
                    Usuario = new BE.Usuario { Id = otroUsuarioId },
                    Mensaje = "RecordatorioTestAjeno",
                    Fecha = new System.DateTime(2026, 2, 13),
                    Leida = false,
                    ItemLista = new BE.ItemLista { Id = otroItemListaId },
                    Promocion = null,
                    FechaVencimiento = new System.DateTime(2026, 2, 23)
                });

                System.Collections.Generic.List<BE.RecordatorioCompra> propios = repositorio.ObtenerPorUsuario(new BE.Usuario { Id = usuarioId });

                Assert.IsTrue(propios.Any(item => item.Id == idPropio));
                Assert.IsFalse(propios.Any(item => item.Id == idAjeno));

                accesoDatos.RollbackTransactionScope();
            }
        }

        [TestMethod]
        public void ObtenerTodas_IncluyeDeTodosLosUsuarios()
        {
            using (AccesoDatosSqlServer accesoDatos = new AccesoDatosSqlServer())
            {
                accesoDatos.BeginTransactionScope();
                RecordatorioCompraRepositorio repositorio = new RecordatorioCompraRepositorio(accesoDatos);
                int usuarioId = ObtenerUsuarioBasicoId(accesoDatos);
                int otroUsuarioId = ObtenerUsuarioAdminId(accesoDatos);
                int listaDeComprasId = InsertarListaDeComprasTest(accesoDatos, usuarioId, "ListaTestObtenerTodasPropia");
                int itemListaId = InsertarItemListaTest(accesoDatos, listaDeComprasId, "ItemTestObtenerTodasPropia");
                int otraListaDeComprasId = InsertarListaDeComprasTest(accesoDatos, otroUsuarioId, "ListaTestObtenerTodasAjena");
                int otroItemListaId = InsertarItemListaTest(accesoDatos, otraListaDeComprasId, "ItemTestObtenerTodasAjena");

                int idPropio = repositorio.InsertarYObtenerId(new BE.RecordatorioCompra
                {
                    Usuario = new BE.Usuario { Id = usuarioId },
                    Mensaje = "RecordatorioTestObtenerTodasPropio",
                    Fecha = new System.DateTime(2026, 2, 16),
                    Leida = false,
                    ItemLista = new BE.ItemLista { Id = itemListaId },
                    Promocion = null,
                    FechaVencimiento = new System.DateTime(2026, 2, 26)
                });
                int idAjeno = repositorio.InsertarYObtenerId(new BE.RecordatorioCompra
                {
                    Usuario = new BE.Usuario { Id = otroUsuarioId },
                    Mensaje = "RecordatorioTestObtenerTodasAjeno",
                    Fecha = new System.DateTime(2026, 2, 17),
                    Leida = false,
                    ItemLista = new BE.ItemLista { Id = otroItemListaId },
                    Promocion = null,
                    FechaVencimiento = new System.DateTime(2026, 2, 27)
                });

                System.Collections.Generic.List<BE.RecordatorioCompra> todos = repositorio.ObtenerTodas();

                Assert.IsTrue(todos.Any(item => item.Id == idPropio));
                Assert.IsTrue(todos.Any(item => item.Id == idAjeno));

                accesoDatos.RollbackTransactionScope();
            }
        }

        [TestMethod]
        public void Actualizar_RecordatorioExistente_PersisteNuevaFechaVencimiento()
        {
            using (AccesoDatosSqlServer accesoDatos = new AccesoDatosSqlServer())
            {
                accesoDatos.BeginTransactionScope();
                RecordatorioCompraRepositorio repositorio = new RecordatorioCompraRepositorio(accesoDatos);
                int usuarioId = ObtenerUsuarioBasicoId(accesoDatos);
                int listaDeComprasId = InsertarListaDeComprasTest(accesoDatos, usuarioId, "ListaTestActualizar");
                int itemListaId = InsertarItemListaTest(accesoDatos, listaDeComprasId, "ItemTestActualizar");
                int id = repositorio.InsertarYObtenerId(new BE.RecordatorioCompra
                {
                    Usuario = new BE.Usuario { Id = usuarioId },
                    Mensaje = "RecordatorioTestOriginal",
                    Fecha = new System.DateTime(2026, 2, 14),
                    Leida = false,
                    ItemLista = new BE.ItemLista { Id = itemListaId },
                    Promocion = null,
                    FechaVencimiento = new System.DateTime(2026, 2, 24)
                });

                bool actualizado = repositorio.Actualizar(new BE.RecordatorioCompra
                {
                    Id = id,
                    Usuario = new BE.Usuario { Id = usuarioId },
                    Mensaje = "RecordatorioTestOriginal",
                    Fecha = new System.DateTime(2026, 2, 14),
                    Leida = false,
                    ItemLista = new BE.ItemLista { Id = itemListaId },
                    Promocion = null,
                    FechaVencimiento = new System.DateTime(2026, 3, 1)
                });

                Assert.IsTrue(actualizado);
                Assert.AreEqual(new System.DateTime(2026, 3, 1), repositorio.ObtenerPorId(id).FechaVencimiento);

                accesoDatos.RollbackTransactionScope();
            }
        }

        [TestMethod]
        public void Eliminar_RecordatorioExistente_DejaDeExistir()
        {
            using (AccesoDatosSqlServer accesoDatos = new AccesoDatosSqlServer())
            {
                accesoDatos.BeginTransactionScope();
                RecordatorioCompraRepositorio repositorio = new RecordatorioCompraRepositorio(accesoDatos);
                int usuarioId = ObtenerUsuarioBasicoId(accesoDatos);
                int listaDeComprasId = InsertarListaDeComprasTest(accesoDatos, usuarioId, "ListaTestAEliminar");
                int itemListaId = InsertarItemListaTest(accesoDatos, listaDeComprasId, "ItemTestAEliminar");
                int id = repositorio.InsertarYObtenerId(new BE.RecordatorioCompra
                {
                    Usuario = new BE.Usuario { Id = usuarioId },
                    Mensaje = "RecordatorioTestAEliminar",
                    Fecha = new System.DateTime(2026, 2, 15),
                    Leida = false,
                    ItemLista = new BE.ItemLista { Id = itemListaId },
                    Promocion = null,
                    FechaVencimiento = new System.DateTime(2026, 2, 25)
                });

                bool eliminado = repositorio.Eliminar(repositorio.ObtenerPorId(id));

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

        private static int InsertarListaDeComprasTest(AccesoDatosSqlServer accesoDatos, int usuarioId, string nombre)
        {
            return new ListaDeComprasRepositorio(accesoDatos).InsertarYObtenerId(new BE.ListaDeCompras
            {
                Usuario = new BE.Usuario { Id = usuarioId },
                Nombre = nombre,
                FechaCreacion = new System.DateTime(2026, 3, 1)
            });
        }

        private static int InsertarItemListaTest(AccesoDatosSqlServer accesoDatos, int listaDeComprasId, string descripcion)
        {
            return new ItemListaRepositorio(accesoDatos).InsertarYObtenerId(new BE.ItemLista
            {
                ListaDeCompras = new BE.ListaDeCompras { Id = listaDeComprasId },
                Descripcion = descripcion,
                Cantidad = 1,
                Comprado = false
            });
        }

        private static int InsertarPromocionTest(AccesoDatosSqlServer accesoDatos, string descripcion)
        {
            return new PromocionRepositorio(accesoDatos).InsertarYObtenerId(new BE.Promocion
            {
                Descripcion = descripcion,
                Descuento = 10m,
                VigenciaDesde = new System.DateTime(2026, 1, 1),
                VigenciaHasta = new System.DateTime(2026, 12, 31)
            });
        }
    }
}
