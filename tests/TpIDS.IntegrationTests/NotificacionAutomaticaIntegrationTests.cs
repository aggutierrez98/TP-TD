using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using BE;
using DAL;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using REPOSITORIO;
using SERVICIOS;

namespace TpIDS.IntegrationTests
{
    // Persistencia de avisos completos y metadata de automatizacion (notificaciones-automaticas-diarias 3.1).
    // Cada test abre su propia transaccion y la revierte: no deja datos en la base de tests.
    [TestClass]
    [TestCategory("Integration")]
    public sealed class NotificacionAutomaticaIntegrationTests
    {
        private static readonly DateTime Jornada = new DateTime(2026, 10, 20);

        [TestMethod]
        public void Insertar_MensajeDeMasDe300CaracteresConSaltosYUnicode_SeConservaCompleto()
        {
            using (AccesoDatosSqlServer acceso = new AccesoDatosSqlServer())
            {
                acceso.BeginTransactionScope();
                var repositorio = new NotificacionRepositorio(acceso);
                int usuarioId = Usuario(acceso, "user");
                string mensaje = ConstruirMensajeLargo();
                Assert.IsTrue(mensaje.Length > 4000);

                int id = repositorio.InsertarYObtenerId(Nueva(usuarioId, mensaje));

                Notificacion leida = repositorio.ObtenerPersonal(new Notificacion { Id = id }, new Usuario { Id = usuarioId });
                Assert.AreEqual(mensaje, leida.Mensaje);
                Assert.Contains("\r\n", leida.Mensaje);
                Assert.Contains("Ωß—ñ", leida.Mensaje);
                acceso.RollbackTransactionScope();
            }
        }

        [TestMethod]
        public void Insertar_RecordatorioDeInversionConMensajeLargo_SeConservaCompleto()
        {
            using (AccesoDatosSqlServer acceso = new AccesoDatosSqlServer())
            {
                acceso.BeginTransactionScope();
                var repositorio = new RecordatorioInversionRepositorio(acceso);
                int usuarioId = Usuario(acceso, "user");
                string mensaje = ConstruirMensajeLargo();

                int id = repositorio.InsertarYObtenerId(new RecordatorioInversion
                {
                    Usuario = new BE.Usuario { Id = usuarioId }, Mensaje = mensaje, Fecha = Jornada
                });

                Assert.AreEqual(mensaje, repositorio.ObtenerPorId(id).Mensaje);
                acceso.RollbackTransactionScope();
            }
        }

        [TestMethod]
        public void Insertar_ActualizarMensajeLargo_SeConservaCompleto()
        {
            using (AccesoDatosSqlServer acceso = new AccesoDatosSqlServer())
            {
                acceso.BeginTransactionScope();
                var repositorio = new NotificacionRepositorio(acceso);
                int usuarioId = Usuario(acceso, "user");
                int id = repositorio.InsertarYObtenerId(Nueva(usuarioId, "corto"));
                string mensaje = ConstruirMensajeLargo();

                Assert.IsTrue(repositorio.Actualizar(new Notificacion { Id = id, Usuario = new BE.Usuario { Id = usuarioId }, Mensaje = mensaje, Fecha = Jornada, Leida = false }));

                Assert.AreEqual(mensaje, repositorio.ObtenerPersonal(new Notificacion { Id = id }, new Usuario { Id = usuarioId }).Mensaje);
                acceso.RollbackTransactionScope();
            }
        }

        [TestMethod]
        public void Insertar_MetadataAutomatica_SeLeeEnTodosLosTipos()
        {
            using (AccesoDatosSqlServer acceso = new AccesoDatosSqlServer())
            {
                acceso.BeginTransactionScope();
                var notificaciones = new NotificacionRepositorio(acceso);
                int usuarioId = Usuario(acceso, "user");

                int movimientos = notificaciones.InsertarYObtenerId(Automatica(usuarioId, TipoNotificacionAutomatica.Movimientos, Jornada));
                int inversiones = new RecordatorioInversionRepositorio(acceso).InsertarYObtenerId(
                    new RecordatorioInversion { Usuario = new BE.Usuario { Id = usuarioId }, Mensaje = "inv", Fecha = Jornada, TipoAutomatico = TipoNotificacionAutomatica.Inversiones, FechaJornada = Jornada });
                int compras = new RecordatorioCompraRepositorio(acceso).InsertarYObtenerId(CompraAgregada(usuarioId, Jornada));

                Notificacion m = notificaciones.ObtenerPersonal(new Notificacion { Id = movimientos }, new Usuario { Id = usuarioId });
                Assert.AreEqual(TipoNotificacionAutomatica.Movimientos, m.TipoAutomatico);
                Assert.AreEqual(Jornada, m.FechaJornada);
                Assert.IsNull(m.ClaveEjemplo);
                Notificacion i = notificaciones.ObtenerPersonal(new Notificacion { Id = inversiones }, new Usuario { Id = usuarioId });
                Assert.IsInstanceOfType<RecordatorioInversion>(i);
                Assert.AreEqual(TipoNotificacionAutomatica.Inversiones, i.TipoAutomatico);
                Assert.AreEqual(Jornada, i.FechaJornada);
                Notificacion c = notificaciones.ObtenerPersonal(new Notificacion { Id = compras }, new Usuario { Id = usuarioId });
                Assert.AreEqual(TipoNotificacionAutomatica.Compras, c.TipoAutomatico);
                Assert.AreEqual(Jornada, c.FechaJornada);
                acceso.RollbackTransactionScope();
            }
        }

        [TestMethod]
        public void Insertar_ComprasAgregadaSinItemPromocionNiVencimiento_SeLeeConNulos()
        {
            using (AccesoDatosSqlServer acceso = new AccesoDatosSqlServer())
            {
                acceso.BeginTransactionScope();
                var repositorio = new RecordatorioCompraRepositorio(acceso);
                int usuarioId = Usuario(acceso, "user");

                int id = repositorio.InsertarYObtenerId(CompraAgregada(usuarioId, Jornada));

                RecordatorioCompra porRepositorio = repositorio.ObtenerPorId(id);
                Assert.IsNull(porRepositorio.ItemLista);
                Assert.IsNull(porRepositorio.Promocion);
                Assert.IsNull(porRepositorio.FechaVencimiento);
                Assert.AreEqual(1, repositorio.ObtenerPorUsuario(new BE.Usuario { Id = usuarioId }).Count(r => r.Id == id));
                Assert.AreEqual(1, repositorio.ObtenerTodas().Count(r => r.Id == id));
                RecordatorioCompra personal = (RecordatorioCompra)new NotificacionRepositorio(acceso).ObtenerPersonal(new Notificacion { Id = id }, new Usuario { Id = usuarioId });
                Assert.IsNull(personal.ItemLista);
                Assert.IsNull(personal.Promocion);
                Assert.IsNull(personal.FechaVencimiento);
                acceso.RollbackTransactionScope();
            }
        }

        [TestMethod]
        public void Actualizar_ComprasAgregadaMarcadaLeida_ConservaNulos()
        {
            using (AccesoDatosSqlServer acceso = new AccesoDatosSqlServer())
            {
                acceso.BeginTransactionScope();
                var repositorio = new RecordatorioCompraRepositorio(acceso);
                int usuarioId = Usuario(acceso, "user");
                int id = repositorio.InsertarYObtenerId(CompraAgregada(usuarioId, Jornada));
                RecordatorioCompra actual = repositorio.ObtenerPorId(id);
                actual.Leida = true;

                Assert.IsTrue(repositorio.Actualizar(actual));

                RecordatorioCompra releida = repositorio.ObtenerPorId(id);
                Assert.IsTrue(releida.Leida);
                Assert.IsNull(releida.FechaVencimiento);
                acceso.RollbackTransactionScope();
            }
        }

        [TestMethod]
        public void Insertar_EjemploDeComprasSinItemNiVencimiento_Acepta()
        {
            using (AccesoDatosSqlServer acceso = new AccesoDatosSqlServer())
            {
                acceso.BeginTransactionScope();
                var repositorio = new RecordatorioCompraRepositorio(acceso);
                int usuarioId = Usuario(acceso, "user");

                int id = repositorio.InsertarYObtenerId(new RecordatorioCompra
                {
                    Usuario = new BE.Usuario { Id = usuarioId }, Mensaje = "Ejemplo", Fecha = Jornada, ClaveEjemplo = "Demo.Compras"
                });

                RecordatorioCompra leido = repositorio.ObtenerPorId(id);
                Assert.AreEqual("Demo.Compras", leido.ClaveEjemplo);
                Assert.IsNull(leido.TipoAutomatico);
                Assert.IsNull(leido.FechaJornada);
                acceso.RollbackTransactionScope();
            }
        }

        [TestMethod]
        public void Insertar_RecordatorioIndividualSinItem_SigueRechazado()
        {
            using (AccesoDatosSqlServer acceso = new AccesoDatosSqlServer())
            {
                acceso.BeginTransactionScope();
                var repositorio = new RecordatorioCompraRepositorio(acceso);
                int usuarioId = Usuario(acceso, "user");

                Assert.ThrowsExactly<ExcepcionTecnica>(() => repositorio.InsertarYObtenerId(new RecordatorioCompra
                {
                    Usuario = new BE.Usuario { Id = usuarioId }, Mensaje = "sin item", Fecha = Jornada, FechaVencimiento = Jornada
                }));
                acceso.RollbackTransactionScope();
            }
        }

        [TestMethod]
        public void Insertar_RecordatorioIndividualSinVencimiento_SigueRechazado()
        {
            using (AccesoDatosSqlServer acceso = new AccesoDatosSqlServer())
            {
                acceso.BeginTransactionScope();
                var repositorio = new RecordatorioCompraRepositorio(acceso);
                int usuarioId = Usuario(acceso, "user");
                int itemId = Item(acceso, usuarioId);

                Assert.ThrowsExactly<ExcepcionTecnica>(() => repositorio.InsertarYObtenerId(new RecordatorioCompra
                {
                    Usuario = new BE.Usuario { Id = usuarioId }, Mensaje = "sin vencimiento", Fecha = Jornada, ItemLista = new ItemLista { Id = itemId }
                }));
                acceso.RollbackTransactionScope();
            }
        }

        [TestMethod]
        public void Insertar_RecordatorioIndividualCompleto_SigueFuncionando()
        {
            using (AccesoDatosSqlServer acceso = new AccesoDatosSqlServer())
            {
                acceso.BeginTransactionScope();
                var repositorio = new RecordatorioCompraRepositorio(acceso);
                int usuarioId = Usuario(acceso, "user");
                int itemId = Item(acceso, usuarioId);

                int id = repositorio.InsertarYObtenerId(new RecordatorioCompra
                {
                    Usuario = new BE.Usuario { Id = usuarioId }, Mensaje = "individual", Fecha = Jornada,
                    ItemLista = new ItemLista { Id = itemId }, FechaVencimiento = new DateTime(2026, 11, 1)
                });

                RecordatorioCompra leido = repositorio.ObtenerPorId(id);
                Assert.AreEqual(itemId, leido.ItemLista.Id);
                Assert.AreEqual(new DateTime(2026, 11, 1), leido.FechaVencimiento);
                Assert.IsNull(leido.TipoAutomatico);
                acceso.RollbackTransactionScope();
            }
        }

        [TestMethod]
        public void Insertar_SegundoAvisoAutomaticoDelMismoUsuarioTipoYJornada_SeRechaza()
        {
            using (AccesoDatosSqlServer acceso = new AccesoDatosSqlServer())
            {
                acceso.BeginTransactionScope();
                var repositorio = new NotificacionRepositorio(acceso);
                int usuarioId = Usuario(acceso, "user");
                repositorio.InsertarYObtenerId(Automatica(usuarioId, TipoNotificacionAutomatica.Movimientos, Jornada));

                Assert.ThrowsExactly<ExcepcionTecnica>(() => repositorio.InsertarYObtenerId(Automatica(usuarioId, TipoNotificacionAutomatica.Movimientos, Jornada)));
                acceso.RollbackTransactionScope();
            }
        }

        [TestMethod]
        public void Insertar_ComprasAgregadaRepetidaEnLaMismaJornada_SeRechaza()
        {
            using (AccesoDatosSqlServer acceso = new AccesoDatosSqlServer())
            {
                acceso.BeginTransactionScope();
                var repositorio = new RecordatorioCompraRepositorio(acceso);
                int usuarioId = Usuario(acceso, "user");
                repositorio.InsertarYObtenerId(CompraAgregada(usuarioId, Jornada));

                Assert.ThrowsExactly<ExcepcionTecnica>(() => repositorio.InsertarYObtenerId(CompraAgregada(usuarioId, Jornada)));
                acceso.RollbackTransactionScope();
            }
        }

        [TestMethod]
        public void Insertar_MismoTipoEnOtraJornadaOEnOtroUsuarioOEnOtroTipo_SeAcepta()
        {
            using (AccesoDatosSqlServer acceso = new AccesoDatosSqlServer())
            {
                acceso.BeginTransactionScope();
                var repositorio = new NotificacionRepositorio(acceso);
                int user = Usuario(acceso, "user");
                int admin = Usuario(acceso, "admin");
                repositorio.InsertarYObtenerId(Automatica(user, TipoNotificacionAutomatica.Movimientos, Jornada));

                repositorio.InsertarYObtenerId(Automatica(user, TipoNotificacionAutomatica.Movimientos, Jornada.AddDays(1)));
                repositorio.InsertarYObtenerId(Automatica(admin, TipoNotificacionAutomatica.Movimientos, Jornada));
                new RecordatorioCompraRepositorio(acceso).InsertarYObtenerId(CompraAgregada(user, Jornada));
                new RecordatorioInversionRepositorio(acceso).InsertarYObtenerId(
                    new RecordatorioInversion { Usuario = new BE.Usuario { Id = user }, Mensaje = "i", Fecha = Jornada, TipoAutomatico = TipoNotificacionAutomatica.Inversiones, FechaJornada = Jornada });
                acceso.RollbackTransactionScope();
            }
        }

        [TestMethod]
        public void Insertar_AvisosManualesSinMetadata_NoCompitenPorElIndiceUnico()
        {
            using (AccesoDatosSqlServer acceso = new AccesoDatosSqlServer())
            {
                acceso.BeginTransactionScope();
                var repositorio = new NotificacionRepositorio(acceso);
                int usuarioId = Usuario(acceso, "user");

                repositorio.InsertarYObtenerId(Nueva(usuarioId, "manual 1"));
                repositorio.InsertarYObtenerId(Nueva(usuarioId, "manual 2"));
                acceso.RollbackTransactionScope();
            }
        }

        [TestMethod]
        public void Insertar_EjemploConClaveRepetidaDelMismoUsuario_SeRechazaPeroOtroUsuarioPuede()
        {
            using (AccesoDatosSqlServer acceso = new AccesoDatosSqlServer())
            {
                acceso.BeginTransactionScope();
                var repositorio = new NotificacionRepositorio(acceso);
                int user = Usuario(acceso, "user");
                int admin = Usuario(acceso, "admin");
                repositorio.InsertarYObtenerId(Ejemplo(user, "Demo.Movimientos"));

                Assert.ThrowsExactly<ExcepcionTecnica>(() => repositorio.InsertarYObtenerId(Ejemplo(user, "Demo.Movimientos")));
                repositorio.InsertarYObtenerId(Ejemplo(admin, "Demo.Movimientos"));
                acceso.RollbackTransactionScope();
            }
        }

        [TestMethod]
        public void Insertar_EjemploNoOcupaElCupoDiarioDelTipo()
        {
            using (AccesoDatosSqlServer acceso = new AccesoDatosSqlServer())
            {
                acceso.BeginTransactionScope();
                var repositorio = new NotificacionRepositorio(acceso);
                int usuarioId = Usuario(acceso, "user");

                repositorio.InsertarYObtenerId(Ejemplo(usuarioId, "Demo.Movimientos"));
                repositorio.InsertarYObtenerId(Automatica(usuarioId, TipoNotificacionAutomatica.Movimientos, Jornada));
                acceso.RollbackTransactionScope();
            }
        }

        [TestMethod]
        public void Insertar_TipoAutomaticoSinJornada_SeRechaza()
        {
            using (AccesoDatosSqlServer acceso = new AccesoDatosSqlServer())
            {
                acceso.BeginTransactionScope();
                int usuarioId = Usuario(acceso, "user");
                Notificacion incompleta = Automatica(usuarioId, TipoNotificacionAutomatica.Movimientos, Jornada);
                incompleta.FechaJornada = null;

                Assert.ThrowsExactly<ExcepcionTecnica>(() => new NotificacionRepositorio(acceso).InsertarYObtenerId(incompleta));
                acceso.RollbackTransactionScope();
            }
        }

        [TestMethod]
        public void Insertar_JornadaSinTipoAutomatico_SeRechaza()
        {
            using (AccesoDatosSqlServer acceso = new AccesoDatosSqlServer())
            {
                acceso.BeginTransactionScope();
                int usuarioId = Usuario(acceso, "user");
                Notificacion incompleta = Nueva(usuarioId, "x");
                incompleta.FechaJornada = Jornada;

                Assert.ThrowsExactly<ExcepcionTecnica>(() => new NotificacionRepositorio(acceso).InsertarYObtenerId(incompleta));
                acceso.RollbackTransactionScope();
            }
        }

        [TestMethod]
        public void Insertar_EjemploConTipoAutomatico_SeRechaza()
        {
            using (AccesoDatosSqlServer acceso = new AccesoDatosSqlServer())
            {
                acceso.BeginTransactionScope();
                int usuarioId = Usuario(acceso, "user");
                Notificacion mezcla = Automatica(usuarioId, TipoNotificacionAutomatica.Movimientos, Jornada);
                mezcla.ClaveEjemplo = "Demo.Movimientos";

                Assert.ThrowsExactly<ExcepcionTecnica>(() => new NotificacionRepositorio(acceso).InsertarYObtenerId(mezcla));
                acceso.RollbackTransactionScope();
            }
        }

        [TestMethod]
        public void Insertar_TipoAutomaticoEnDiscriminadorIncorrecto_SeRechaza()
        {
            using (AccesoDatosSqlServer acceso = new AccesoDatosSqlServer())
            {
                acceso.BeginTransactionScope();
                int usuarioId = Usuario(acceso, "user");
                // Compras exige RecordatorioCompra, Inversiones exige RecordatorioInversion y Movimientos exige Notificacion.
                Notificacion comprasComoBase = Automatica(usuarioId, TipoNotificacionAutomatica.Compras, Jornada);
                Notificacion inversionesComoBase = Automatica(usuarioId, TipoNotificacionAutomatica.Inversiones, Jornada);

                Assert.ThrowsExactly<ExcepcionTecnica>(() => new NotificacionRepositorio(acceso).InsertarYObtenerId(comprasComoBase));
                Assert.ThrowsExactly<ExcepcionTecnica>(() => new NotificacionRepositorio(acceso).InsertarYObtenerId(inversionesComoBase));
                Assert.ThrowsExactly<ExcepcionTecnica>(() => new RecordatorioInversionRepositorio(acceso).InsertarYObtenerId(
                    new RecordatorioInversion { Usuario = new BE.Usuario { Id = usuarioId }, Mensaje = "x", Fecha = Jornada, TipoAutomatico = TipoNotificacionAutomatica.Movimientos, FechaJornada = Jornada }));
                acceso.RollbackTransactionScope();
            }
        }

        [TestMethod]
        public void SqlDirecto_TipoAutomaticoDesconocido_SeRechaza()
        {
            using (AccesoDatosSqlServer acceso = new AccesoDatosSqlServer())
            {
                acceso.BeginTransactionScope();
                int usuarioId = Usuario(acceso, "user");

                Assert.ThrowsExactly<ExcepcionTecnica>(() => acceso.Write(
                    "INSERT INTO Notificacion (UsuarioId, TipoNotificacion, Mensaje, Fecha, Leida, TipoAutomatico, FechaJornada) VALUES (@U, 'Notificacion', N'x', GETDATE(), 0, 'Otro', '2026-10-20')",
                    new System.Collections.Generic.List<SqlParameter> { acceso.CreateParameter("@U", SqlDbType.Int, usuarioId) }));
                acceso.RollbackTransactionScope();
            }
        }

        [TestMethod]
        public void Leer_AvisosDeOtroUsuario_NoSeExponenPorLecturaPersonal()
        {
            using (AccesoDatosSqlServer acceso = new AccesoDatosSqlServer())
            {
                acceso.BeginTransactionScope();
                var repositorio = new NotificacionRepositorio(acceso);
                int user = Usuario(acceso, "user");
                int admin = Usuario(acceso, "admin");
                int ajena = repositorio.InsertarYObtenerId(Nueva(admin, ConstruirMensajeLargo()));

                Assert.IsNull(repositorio.ObtenerPersonal(new Notificacion { Id = ajena }, new Usuario { Id = user }));
                Assert.IsFalse(repositorio.MarcarPersonalLeida(new Notificacion { Id = ajena }, new Usuario { Id = user }));
                Assert.IsFalse(repositorio.ObtenerPaginaPersonal(new Usuario { Id = user }, null, 1).Elementos.Any(n => n.Id == ajena));
                acceso.RollbackTransactionScope();
            }
        }

        [TestMethod]
        public void Pagina_MensajesLargosYComprasAgregada_PaginaMarcaYContabilizaNoLeidas()
        {
            using (AccesoDatosSqlServer acceso = new AccesoDatosSqlServer())
            {
                acceso.BeginTransactionScope();
                var repositorio = new NotificacionRepositorio(acceso);
                int usuarioId = Usuario(acceso, "user");
                var destinatario = new UsuarioRepositorio(acceso).ObtenerPorId(usuarioId);
                int basal = repositorio.ContarNoLeidas(destinatario);
                var ids = Enumerable.Range(0, 11).Select(i => repositorio.InsertarYObtenerId(new Notificacion
                {
                    Usuario = new BE.Usuario { Id = usuarioId }, Mensaje = ConstruirMensajeLargo() + i, Fecha = new DateTime(2030, 1, 1).AddMinutes(i), Leida = false
                })).ToList();
                int agregada = new RecordatorioCompraRepositorio(acceso).InsertarYObtenerId(CompraAgregada(usuarioId, Jornada));

                Assert.AreEqual(basal + 12, repositorio.ContarNoLeidas(destinatario));
                PaginaNotificaciones primera = repositorio.ObtenerPaginaPersonal(destinatario, new FiltroNotificacion { FechaDesde = new DateTime(2030, 1, 1) }, 1);
                PaginaNotificaciones segunda = repositorio.ObtenerPaginaPersonal(destinatario, new FiltroNotificacion { FechaDesde = new DateTime(2030, 1, 1) }, 2);
                Assert.AreEqual(11, primera.Total);
                Assert.AreEqual(10, primera.Elementos.Count);
                Assert.AreEqual(1, segunda.Elementos.Count);
                Assert.AreEqual(ConstruirMensajeLargo() + "10", primera.Elementos[0].Mensaje);
                PaginaNotificaciones conAgregada = repositorio.ObtenerPaginaPersonal(destinatario, new FiltroNotificacion { FechaDesde = Jornada, FechaHasta = Jornada }, 1);
                Assert.IsInstanceOfType<RecordatorioCompra>(conAgregada.Elementos.Single(n => n.Id == agregada));
                Assert.IsNull(((RecordatorioCompra)conAgregada.Elementos.Single(n => n.Id == agregada)).FechaVencimiento);
                var original = new Notificacion { Id = ids[0] };
                var nueva = new Notificacion { Id = agregada };
                Assert.IsTrue(repositorio.MarcarPersonalLeida(original, destinatario));
                Assert.IsTrue(repositorio.MarcarPersonalLeida(nueva, destinatario));
                Assert.IsTrue(repositorio.ObtenerPersonal(original, destinatario).Leida);
                Assert.IsTrue(repositorio.ObtenerPersonal(nueva, destinatario).Leida);
                Assert.AreEqual(basal + 10, repositorio.ContarNoLeidas(destinatario));
                acceso.RollbackTransactionScope();
            }
        }

        private static string ConstruirMensajeLargo()
        {
            var texto = new StringBuilder("Resumen de compras — Ωß ñandú\r\n");
            for (int i = 1; i <= 120; i++) texto.Append("Linea ").Append(i).Append(": Ωß—ñ producto con descuento\r\n");
            return texto.ToString();
        }

        private static Notificacion Nueva(int usuarioId, string mensaje)
        {
            return new Notificacion { Usuario = new BE.Usuario { Id = usuarioId }, Mensaje = mensaje, Fecha = Jornada, Leida = false };
        }

        private static Notificacion Automatica(int usuarioId, TipoNotificacionAutomatica tipo, DateTime jornada)
        {
            Notificacion n = Nueva(usuarioId, "automatica");
            n.TipoAutomatico = tipo;
            n.FechaJornada = jornada;
            return n;
        }

        private static Notificacion Ejemplo(int usuarioId, string clave)
        {
            Notificacion n = Nueva(usuarioId, "ejemplo");
            n.ClaveEjemplo = clave;
            return n;
        }

        private static RecordatorioCompra CompraAgregada(int usuarioId, DateTime jornada)
        {
            return new RecordatorioCompra
            {
                Usuario = new BE.Usuario { Id = usuarioId }, Mensaje = "compras", Fecha = jornada, Leida = false,
                TipoAutomatico = TipoNotificacionAutomatica.Compras, FechaJornada = jornada
            };
        }

        private static int Usuario(AccesoDatosSqlServer acceso, string username)
        {
            return new UsuarioDAL().ObtenerPorUsername(username, acceso).Id;
        }

        private static int Item(AccesoDatosSqlServer acceso, int usuarioId)
        {
            int lista = new ListaDeComprasRepositorio(acceso).InsertarYObtenerId(new ListaDeCompras
            {
                Usuario = new BE.Usuario { Id = usuarioId }, Nombre = "ListaAutomatica", FechaCreacion = Jornada
            });
            return new ItemListaRepositorio(acceso).InsertarYObtenerId(new ItemLista
            {
                ListaDeCompras = new ListaDeCompras { Id = lista }, Descripcion = "ItemAutomatico", Cantidad = 1, Comprado = false
            });
        }
    }
}
