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
    public sealed class NotificacionRepositorioIntegrationTests
    {
        [TestMethod]
        public void InsertarYObtenerId_NotificacionValida_QuedaDisponible()
        {
            using (AccesoDatosSqlServer accesoDatos = new AccesoDatosSqlServer())
            {
                accesoDatos.BeginTransactionScope();
                NotificacionRepositorio repositorio = new NotificacionRepositorio(accesoDatos);
                int usuarioId = ObtenerUsuarioBasicoId(accesoDatos);

                int id = repositorio.InsertarYObtenerId(new BE.Notificacion
                {
                    Usuario = new BE.Usuario { Id = usuarioId },
                    Mensaje = "NotificacionTestPropia",
                    Fecha = new System.DateTime(2026, 2, 10),
                    Leida = false
                });

                BE.Notificacion obtenida = repositorio.ObtenerPorId(id);
                Assert.IsNotNull(obtenida);
                Assert.IsNotNull(obtenida.Usuario);
                Assert.AreEqual(usuarioId, obtenida.Usuario.Id);
                Assert.AreEqual("NotificacionTestPropia", obtenida.Mensaje);
                Assert.AreEqual(new System.DateTime(2026, 2, 10), obtenida.Fecha);
                Assert.IsFalse(obtenida.Leida);

                accesoDatos.RollbackTransactionScope();
            }
        }

        [TestMethod]
        public void ObtenerPorUsuario_ExcluyeNotificacionesDeOtroUsuario()
        {
            using (AccesoDatosSqlServer accesoDatos = new AccesoDatosSqlServer())
            {
                accesoDatos.BeginTransactionScope();
                NotificacionRepositorio repositorio = new NotificacionRepositorio(accesoDatos);
                int usuarioId = ObtenerUsuarioBasicoId(accesoDatos);
                int otroUsuarioId = ObtenerUsuarioAdminId(accesoDatos);

                int idPropia = repositorio.InsertarYObtenerId(new BE.Notificacion
                {
                    Usuario = new BE.Usuario { Id = usuarioId },
                    Mensaje = "NotificacionTestUsuarioA",
                    Fecha = new System.DateTime(2026, 2, 1),
                    Leida = false
                });
                int idAjena = repositorio.InsertarYObtenerId(new BE.Notificacion
                {
                    Usuario = new BE.Usuario { Id = otroUsuarioId },
                    Mensaje = "NotificacionTestUsuarioB",
                    Fecha = new System.DateTime(2026, 2, 2),
                    Leida = false
                });

                System.Collections.Generic.List<BE.Notificacion> propias = repositorio.ObtenerPorUsuario(usuarioId);

                Assert.IsTrue(propias.Any(item => item.Id == idPropia));
                Assert.IsFalse(propias.Any(item => item.Id == idAjena));

                accesoDatos.RollbackTransactionScope();
            }
        }

        [TestMethod]
        public void Actualizar_NotificacionExistente_MarcaComoLeida()
        {
            using (AccesoDatosSqlServer accesoDatos = new AccesoDatosSqlServer())
            {
                accesoDatos.BeginTransactionScope();
                NotificacionRepositorio repositorio = new NotificacionRepositorio(accesoDatos);
                int usuarioId = ObtenerUsuarioBasicoId(accesoDatos);
                int id = repositorio.InsertarYObtenerId(new BE.Notificacion
                {
                    Usuario = new BE.Usuario { Id = usuarioId },
                    Mensaje = "NotificacionTestOriginal",
                    Fecha = new System.DateTime(2026, 2, 3),
                    Leida = false
                });

                bool actualizada = repositorio.Actualizar(new BE.Notificacion
                {
                    Id = id,
                    Usuario = new BE.Usuario { Id = usuarioId },
                    Mensaje = "NotificacionTestOriginal",
                    Fecha = new System.DateTime(2026, 2, 3),
                    Leida = true
                });

                Assert.IsTrue(actualizada);
                Assert.IsTrue(repositorio.ObtenerPorId(id).Leida);

                accesoDatos.RollbackTransactionScope();
            }
        }

        [TestMethod]
        public void Eliminar_NotificacionExistente_DejaDeExistir()
        {
            using (AccesoDatosSqlServer accesoDatos = new AccesoDatosSqlServer())
            {
                accesoDatos.BeginTransactionScope();
                NotificacionRepositorio repositorio = new NotificacionRepositorio(accesoDatos);
                int usuarioId = ObtenerUsuarioBasicoId(accesoDatos);
                int id = repositorio.InsertarYObtenerId(new BE.Notificacion
                {
                    Usuario = new BE.Usuario { Id = usuarioId },
                    Mensaje = "NotificacionTestAEliminar",
                    Fecha = new System.DateTime(2026, 2, 4),
                    Leida = false
                });

                bool eliminada = repositorio.Eliminar(id);

                Assert.IsTrue(eliminada);
                Assert.IsNull(repositorio.ObtenerPorId(id));

                accesoDatos.RollbackTransactionScope();
            }
        }

        [TestMethod]
        public void ObtenerTodas_IncluyeNotificacionesDeTodosLosUsuarios()
        {
            using (AccesoDatosSqlServer accesoDatos = new AccesoDatosSqlServer())
            {
                accesoDatos.BeginTransactionScope();
                NotificacionRepositorio repositorio = new NotificacionRepositorio(accesoDatos);
                int usuarioId = ObtenerUsuarioBasicoId(accesoDatos);
                int otroUsuarioId = ObtenerUsuarioAdminId(accesoDatos);

                int idPropia = repositorio.InsertarYObtenerId(new BE.Notificacion
                {
                    Usuario = new BE.Usuario { Id = usuarioId },
                    Mensaje = "NotificacionTestObtenerTodasA",
                    Fecha = new System.DateTime(2026, 2, 20),
                    Leida = false
                });
                int idAjena = repositorio.InsertarYObtenerId(new BE.Notificacion
                {
                    Usuario = new BE.Usuario { Id = otroUsuarioId },
                    Mensaje = "NotificacionTestObtenerTodasB",
                    Fecha = new System.DateTime(2026, 2, 21),
                    Leida = false
                });

                System.Collections.Generic.List<BE.Notificacion> todas = repositorio.ObtenerTodas();

                Assert.IsTrue(todas.Any(item => item.Id == idPropia));
                Assert.IsTrue(todas.Any(item => item.Id == idAjena));

                accesoDatos.RollbackTransactionScope();
            }
        }

        [TestMethod]
        public void ObtenerTodas_NoIncluyeRecordatorios()
        {
            using (AccesoDatosSqlServer accesoDatos = new AccesoDatosSqlServer())
            {
                accesoDatos.BeginTransactionScope();
                NotificacionRepositorio repositorio = new NotificacionRepositorio(accesoDatos);
                int usuarioId = ObtenerUsuarioBasicoId(accesoDatos);

                int idNotificacion = repositorio.InsertarYObtenerId(new BE.Notificacion
                {
                    Usuario = new BE.Usuario { Id = usuarioId },
                    Mensaje = "NotificacionTestObtenerTodasDiscriminador",
                    Fecha = new System.DateTime(2026, 2, 22),
                    Leida = false
                });

                int listaDeComprasId = InsertarListaDeComprasTest(accesoDatos, usuarioId, "ListaTestParaRecordatorioObtenerTodas");
                int itemListaId = InsertarItemListaTest(accesoDatos, listaDeComprasId, "ItemTestParaRecordatorioObtenerTodas");

                RecordatorioCompraRepositorio recordatorioRepositorio = new RecordatorioCompraRepositorio(accesoDatos);
                int idRecordatorio = recordatorioRepositorio.InsertarYObtenerId(new BE.RecordatorioCompra
                {
                    Usuario = new BE.Usuario { Id = usuarioId },
                    Mensaje = "RecordatorioTestObtenerTodasDiscriminador",
                    Fecha = new System.DateTime(2026, 2, 23),
                    Leida = false,
                    ItemLista = new BE.ItemLista { Id = itemListaId },
                    Promocion = null,
                    FechaVencimiento = new System.DateTime(2026, 3, 1)
                });

                System.Collections.Generic.List<BE.Notificacion> todas = repositorio.ObtenerTodas();

                Assert.IsTrue(todas.Any(item => item.Id == idNotificacion));
                Assert.IsFalse(todas.Any(item => item.Id == idRecordatorio));

                accesoDatos.RollbackTransactionScope();
            }
        }

        [TestMethod]
        public void ObtenerPorUsuario_NoIncluyeRecordatorios()
        {
            using (AccesoDatosSqlServer accesoDatos = new AccesoDatosSqlServer())
            {
                accesoDatos.BeginTransactionScope();
                NotificacionRepositorio repositorio = new NotificacionRepositorio(accesoDatos);
                int usuarioId = ObtenerUsuarioBasicoId(accesoDatos);

                int idNotificacion = repositorio.InsertarYObtenerId(new BE.Notificacion
                {
                    Usuario = new BE.Usuario { Id = usuarioId },
                    Mensaje = "NotificacionTestDiscriminador",
                    Fecha = new System.DateTime(2026, 2, 5),
                    Leida = false
                });

                int listaDeComprasId = InsertarListaDeComprasTest(accesoDatos, usuarioId, "ListaTestParaRecordatorioDiscriminador");
                int itemListaId = InsertarItemListaTest(accesoDatos, listaDeComprasId, "ItemTestParaRecordatorioDiscriminador");

                RecordatorioCompraRepositorio recordatorioRepositorio = new RecordatorioCompraRepositorio(accesoDatos);
                int idRecordatorio = recordatorioRepositorio.InsertarYObtenerId(new BE.RecordatorioCompra
                {
                    Usuario = new BE.Usuario { Id = usuarioId },
                    Mensaje = "RecordatorioTestDiscriminador",
                    Fecha = new System.DateTime(2026, 2, 6),
                    Leida = false,
                    ItemLista = new BE.ItemLista { Id = itemListaId },
                    Promocion = null,
                    FechaVencimiento = new System.DateTime(2026, 2, 10)
                });

                System.Collections.Generic.List<BE.Notificacion> propias = repositorio.ObtenerPorUsuario(usuarioId);

                Assert.IsTrue(propias.Any(item => item.Id == idNotificacion));
                Assert.IsFalse(propias.Any(item => item.Id == idRecordatorio));

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
    }
}
