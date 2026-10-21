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

                System.Collections.Generic.List<BE.Notificacion> propias = repositorio.ObtenerPorUsuario(new BE.Usuario { Id = usuarioId });

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

                bool eliminada = repositorio.Eliminar(repositorio.ObtenerPorId(id));

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

                System.Collections.Generic.List<BE.Notificacion> propias = repositorio.ObtenerPorUsuario(new BE.Usuario { Id = usuarioId });

                Assert.IsTrue(propias.Any(item => item.Id == idNotificacion));
                Assert.IsFalse(propias.Any(item => item.Id == idRecordatorio));

                accesoDatos.RollbackTransactionScope();
            }
        }

        [TestMethod]
        public void ContarPorPromocion_CuentaLasNotificacionesQueLaReferencian()
        {
            using (AccesoDatosSqlServer accesoDatos = new AccesoDatosSqlServer())
            {
                accesoDatos.BeginTransactionScope();
                NotificacionRepositorio repositorio = new NotificacionRepositorio(accesoDatos);
                RecordatorioCompraRepositorio recordatorios = new RecordatorioCompraRepositorio(accesoDatos);
                int usuarioId = ObtenerUsuarioBasicoId(accesoDatos);
                int listaDeComprasId = InsertarListaDeComprasTest(accesoDatos, usuarioId, "ListaTestContarPorPromocion");
                int itemListaId = InsertarItemListaTest(accesoDatos, listaDeComprasId, "ItemTestContarPorPromocion");
                int promocionUsadaId = InsertarPromocionTest(accesoDatos, "PromocionTestContarUsada");
                int promocionSinUsoId = InsertarPromocionTest(accesoDatos, "PromocionTestContarLibre");
                for (int i = 0; i < 2; i++)
                {
                    recordatorios.InsertarYObtenerId(new BE.RecordatorioCompra
                    {
                        Usuario = new BE.Usuario { Id = usuarioId },
                        Mensaje = "RecordatorioTestContarPorPromocion" + i,
                        Fecha = new System.DateTime(2026, 2, 10),
                        Leida = false,
                        ItemLista = new BE.ItemLista { Id = itemListaId },
                        Promocion = new BE.Promocion { Id = promocionUsadaId },
                        FechaVencimiento = new System.DateTime(2026, 2, 20)
                    });
                }

                Assert.AreEqual(2, repositorio.ContarPorPromocion(new BE.Promocion { Id = promocionUsadaId }));
                Assert.AreEqual(0, repositorio.ContarPorPromocion(new BE.Promocion { Id = promocionSinUsoId }));

                accesoDatos.RollbackTransactionScope();
            }
        }

        private static int InsertarPromocionTest(AccesoDatosSqlServer accesoDatos, string descripcion)
        {
            return new PromocionRepositorio(accesoDatos).InsertarYObtenerId(new BE.Promocion
            {
                Descripcion = descripcion,
                Descuento = 10m,
                VigenciaDesde = new System.DateTime(2026, 1, 1),
                VigenciaHasta = new System.DateTime(2026, 1, 31)
            });
        }

        [TestMethod]
        public void ObtenerPaginaPersonal_TresTiposYPaginado_AislaOrdenaYCuenta()
        {
            using (var acceso = new AccesoDatosSqlServer())
            {
                acceso.BeginTransactionScope();
                int usuario = ObtenerUsuarioBasicoId(acceso);
                int otro = ObtenerUsuarioAdminId(acceso);
                acceso.Write("DELETE FROM Notificacion WHERE UsuarioId = " + usuario);
                var repo = new NotificacionRepositorio(acceso);
                int lista = InsertarListaDeComprasTest(acceso, usuario, "Personal");
                int item = InsertarItemListaTest(acceso, lista, "Personal");
                var ids = new System.Collections.Generic.List<int>();
                for (int i = 0; i < 24; i++)
                {
                    string tipo = i % 3 == 0 ? "Notificacion" : i % 3 == 1 ? "RecordatorioCompra" : "RecordatorioInversion";
                    ids.Add(acceso.WriteWithResult(@"INSERT INTO Notificacion
                        (UsuarioId, TipoNotificacion, Mensaje, Fecha, Leida, ItemListaId, FechaVencimiento)
                        OUTPUT INSERTED.Id VALUES (@Usuario, @Tipo, 'Personal', '20261014 23:59:00', 0, @Item, '20261101')",
                        new System.Collections.Generic.List<System.Data.SqlClient.SqlParameter> {
                            acceso.CreateParameter("@Usuario", System.Data.SqlDbType.Int, usuario),
                            acceso.CreateParameter("@Tipo", System.Data.SqlDbType.VarChar, tipo),
                            acceso.CreateParameter("@Item", System.Data.SqlDbType.Int, tipo == "RecordatorioCompra" ? (object)item : null) }));
                }
                repo.InsertarYObtenerId(new BE.Notificacion { Usuario = new BE.Usuario { Id = otro }, Mensaje = "Ajena", Fecha = new System.DateTime(2026, 10, 15) });
                var filtro = new BE.FiltroNotificacion { FechaDesde = new System.DateTime(2026, 10, 14), FechaHasta = new System.DateTime(2026, 10, 14) };
                var destinatario = new UsuarioRepositorio(acceso).ObtenerPorId(usuario);
                var primera = repo.ObtenerPaginaPersonal(destinatario, filtro, 1);
                var segunda = repo.ObtenerPaginaPersonal(destinatario, filtro, 2);
                var tercera = repo.ObtenerPaginaPersonal(destinatario, filtro, 3);
                Assert.AreEqual(24, primera.Total);
                Assert.AreEqual(3, primera.TotalPaginas);
                Assert.AreEqual(10, primera.Elementos.Count);
                Assert.AreEqual(10, segunda.Elementos.Count);
                Assert.AreEqual(4, tercera.Elementos.Count);
                CollectionAssert.AreEqual(ids.OrderByDescending(id => id).ToList(), primera.Elementos.Concat(segunda.Elementos).Concat(tercera.Elementos).Select(n => n.Id).ToList());
                Assert.IsTrue(primera.Elementos.Any(n => n is BE.RecordatorioCompra));
                Assert.IsTrue(primera.Elementos.Any(n => n is BE.RecordatorioInversion));
                Assert.AreEqual(24, repo.ContarNoLeidas(destinatario));
                Assert.IsTrue(primera.Elementos.All(n => n.Usuario.Username == destinatario.Username));
                foreach (int id in ids.Take(3))
                {
                    BE.Notificacion antes = repo.ObtenerPersonal(new BE.Notificacion { Id = id }, destinatario);
                    Assert.IsTrue(repo.MarcarPersonalLeida(antes, destinatario));
                    BE.Notificacion despues = repo.ObtenerPersonal(antes, destinatario);
                    Assert.AreEqual(antes.GetType(), despues.GetType());
                    Assert.AreEqual(antes.Mensaje, despues.Mensaje);
                    Assert.AreEqual(antes.Fecha, despues.Fecha);
                    if (antes is BE.RecordatorioCompra compra)
                    {
                        var otra = (BE.RecordatorioCompra)despues;
                        Assert.AreEqual(compra.ItemLista.Id, otra.ItemLista.Id);
                        Assert.AreEqual(compra.FechaVencimiento, otra.FechaVencimiento);
                    }
                }
                Assert.AreEqual(21, repo.ContarNoLeidas(destinatario));
                Assert.AreEqual(3, repo.ObtenerPaginaPersonal(destinatario, filtro, 99).Pagina);
                var vacia = repo.ObtenerPaginaPersonal(destinatario, new BE.FiltroNotificacion { FechaDesde = new System.DateTime(2026, 10, 15) }, 1);
                Assert.AreEqual(0, vacia.Total);
                Assert.AreEqual(0, vacia.Elementos.Count);
                acceso.RollbackTransactionScope();
            }
        }

        [TestMethod]
        public void MarcarPersonalLeida_PropiedadIdempotenciaYRollback_ConservaMensaje()
        {
            int id;
            int usuario;
            using (var acceso = new AccesoDatosSqlServer())
            {
                usuario = ObtenerUsuarioBasicoId(acceso);
                id = new NotificacionRepositorio(acceso).InsertarYObtenerId(new BE.Notificacion { Usuario = new BE.Usuario { Id = usuario }, Mensaje = "LecturaTransaccional", Fecha = new System.DateTime(2026, 10, 14) });
            }
            try
            {
                using (var acceso = new AccesoDatosSqlServer())
                {
                    acceso.BeginTransactionScope();
                    var repo = new NotificacionRepositorio(acceso);
                    int otro = ObtenerUsuarioAdminId(acceso);
                    var notificacion = new BE.Notificacion { Id = id };
                    var destinatario = new BE.Usuario { Id = usuario };
                    var ajeno = new BE.Usuario { Id = otro };
                    Assert.IsNull(repo.ObtenerPersonal(notificacion, ajeno));
                    Assert.IsFalse(repo.MarcarPersonalLeida(notificacion, ajeno));
                    Assert.IsTrue(repo.MarcarPersonalLeida(notificacion, destinatario));
                    Assert.IsFalse(repo.MarcarPersonalLeida(notificacion, destinatario));
                    Assert.IsTrue(repo.ObtenerPersonal(notificacion, destinatario).Leida);
                    Assert.AreEqual("LecturaTransaccional", repo.ObtenerPersonal(notificacion, destinatario).Mensaje);
                    acceso.RollbackTransactionScope();
                }
                Assert.IsFalse(new NotificacionRepositorio().ObtenerPersonal(new BE.Notificacion { Id = id }, new BE.Usuario { Id = usuario }).Leida);
            }
            finally { new NotificacionRepositorio().Eliminar(new BE.Notificacion { Id = id }); }
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
