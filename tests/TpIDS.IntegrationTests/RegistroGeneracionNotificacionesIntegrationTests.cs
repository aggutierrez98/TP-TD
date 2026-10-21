using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using BE;
using DAL;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using REPOSITORIO;
using SERVICIOS;

namespace TpIDS.IntegrationTests
{
    // Registro tecnico del proceso automatico de notificaciones (notificaciones-automaticas-diarias 3.2).
    // Los tests con AccesoDatosSqlServer abren una transaccion y la revierten; los de unidad de
    // trabajo nunca llaman a Save.
    [TestClass]
    [TestCategory("Integration")]
    public sealed class RegistroGeneracionNotificacionesIntegrationTests
    {
        private static readonly DateTime Entrega = new DateTime(2026, 10, 20, 10, 30, 0, DateTimeKind.Utc);
        private static readonly DateTime Jornada = new DateTime(2026, 10, 20);

        [TestMethod]
        public void Registro_CorridaConDetalles_SePersisteYSeLee()
        {
            using (AccesoDatosSqlServer acceso = new AccesoDatosSqlServer())
            {
                acceso.BeginTransactionScope();
                var repositorio = new GeneracionNotificacionesRepositorio(acceso);
                int user = Usuario(acceso, "user");
                int admin = Usuario(acceso, "admin");
                int notificacionId = Notificacion(acceso, user);
                int corridaId = repositorio.InsertarCorridaYObtenerId(new CorridaGeneracionNotificaciones { InicioUtc = Entrega, Jornada = Jornada });

                repositorio.InsertarDetalleYObtenerId(Detalle(corridaId, user, TipoNotificacionAutomatica.Movimientos, ResultadoGeneracionNotificacion.Entregada, null, notificacionId));
                repositorio.InsertarDetalleYObtenerId(Detalle(corridaId, user, TipoNotificacionAutomatica.Compras, ResultadoGeneracionNotificacion.Omitida, "SinPendientes", null));
                repositorio.InsertarDetalleYObtenerId(Detalle(corridaId, admin, TipoNotificacionAutomatica.Inversiones, ResultadoGeneracionNotificacion.Fallida, "FalloProveedorIA", null));
                Assert.IsTrue(repositorio.FinalizarCorrida(repositorio.ObtenerPorId(corridaId), Entrega.AddMinutes(2), 1, 1, 1));

                CorridaGeneracionNotificaciones corrida = repositorio.ObtenerPorId(corridaId);
                Assert.AreEqual(Entrega, corrida.InicioUtc);
                Assert.AreEqual(Entrega.AddMinutes(2), corrida.FinUtc);
                Assert.AreEqual(Jornada, corrida.Jornada);
                Assert.AreEqual(1, corrida.Entregadas);
                Assert.AreEqual(1, corrida.Omitidas);
                Assert.AreEqual(1, corrida.Fallidas);
                List<DetalleGeneracionNotificacion> detalles = repositorio.ObtenerDetallePorCorrida(corrida);
                Assert.IsTrue(detalles.All(d => ReferenceEquals(corrida, d.Corrida)));
                Assert.IsTrue(detalles.All(d => !string.IsNullOrWhiteSpace(d.Usuario.Username)));
                Assert.AreEqual("Aviso de prueba", detalles[0].Notificacion.Mensaje);
                Assert.AreEqual(3, detalles.Count);
                Assert.AreEqual(notificacionId, detalles[0].Notificacion.Id);
                Assert.AreEqual(TipoNotificacionAutomatica.Movimientos, detalles[0].Tipo);
                Assert.AreEqual(Jornada, detalles[0].Jornada);
                Assert.IsNull(detalles[0].Motivo);
                Assert.AreEqual("SinPendientes", detalles[1].Motivo);
                Assert.AreEqual(ResultadoGeneracionNotificacion.Omitida, detalles[1].Resultado);
                Assert.IsNull(detalles[1].Notificacion);
                Assert.AreEqual(admin, detalles[2].Usuario.Id);
                Assert.AreEqual(ResultadoGeneracionNotificacion.Fallida, detalles[2].Resultado);
                Assert.IsTrue(repositorio.ObtenerRecientes(5).Any(c => c.Id == corridaId));
                acceso.RollbackTransactionScope();
            }
        }

        [TestMethod]
        public void Registro_CorridaSinFinalizar_QuedaConFinNuloYCantidadesEnCero()
        {
            using (AccesoDatosSqlServer acceso = new AccesoDatosSqlServer())
            {
                acceso.BeginTransactionScope();
                var repositorio = new GeneracionNotificacionesRepositorio(acceso);

                int corridaId = repositorio.InsertarCorridaYObtenerId(new CorridaGeneracionNotificaciones { InicioUtc = Entrega, Jornada = Jornada });

                CorridaGeneracionNotificaciones corrida = repositorio.ObtenerPorId(corridaId);
                Assert.IsNull(corrida.FinUtc);
                Assert.AreEqual(0, corrida.Entregadas + corrida.Omitidas + corrida.Fallidas);
                Assert.IsNull(repositorio.ObtenerPorId(2000000000));
                acceso.RollbackTransactionScope();
            }
        }

        [TestMethod]
        public void Registro_RecientesOrdenadasPorInicioDescendente()
        {
            using (AccesoDatosSqlServer acceso = new AccesoDatosSqlServer())
            {
                acceso.BeginTransactionScope();
                var repositorio = new GeneracionNotificacionesRepositorio(acceso);
                DateTime futuro = new DateTime(2099, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                int vieja = repositorio.InsertarCorridaYObtenerId(new CorridaGeneracionNotificaciones { InicioUtc = futuro, Jornada = Jornada });
                int nueva = repositorio.InsertarCorridaYObtenerId(new CorridaGeneracionNotificaciones { InicioUtc = futuro.AddHours(1), Jornada = Jornada });

                List<CorridaGeneracionNotificaciones> recientes = repositorio.ObtenerRecientes(2);

                CollectionAssert.AreEqual(new[] { nueva, vieja }, recientes.Select(c => c.Id).ToArray());
                acceso.RollbackTransactionScope();
            }
        }

        [TestMethod]
        public void Registro_DetallesInvalidos_SeRechazan()
        {
            using (AccesoDatosSqlServer acceso = new AccesoDatosSqlServer())
            {
                acceso.BeginTransactionScope();
                var repositorio = new GeneracionNotificacionesRepositorio(acceso);
                int user = Usuario(acceso, "user");
                int notificacionId = Notificacion(acceso, user);
                int corridaId = repositorio.InsertarCorridaYObtenerId(new CorridaGeneracionNotificaciones { InicioUtc = Entrega, Jornada = Jornada });

                // Omitida y Fallida exigen un motivo estable.
                Assert.ThrowsExactly<ExcepcionTecnica>(() => repositorio.InsertarDetalleYObtenerId(Detalle(corridaId, user, TipoNotificacionAutomatica.Compras, ResultadoGeneracionNotificacion.Omitida, null, null)));
                Assert.ThrowsExactly<ExcepcionTecnica>(() => repositorio.InsertarDetalleYObtenerId(Detalle(corridaId, user, TipoNotificacionAutomatica.Compras, ResultadoGeneracionNotificacion.Fallida, "", null)));
                // Solo una entrega referencia una notificacion.
                Assert.ThrowsExactly<ExcepcionTecnica>(() => repositorio.InsertarDetalleYObtenerId(Detalle(corridaId, user, TipoNotificacionAutomatica.Compras, ResultadoGeneracionNotificacion.Omitida, "SinPendientes", notificacionId)));
                // Referencias inexistentes.
                Assert.ThrowsExactly<ExcepcionTecnica>(() => repositorio.InsertarDetalleYObtenerId(Detalle(2000000000, user, TipoNotificacionAutomatica.Compras, ResultadoGeneracionNotificacion.Omitida, "X", null)));
                Assert.ThrowsExactly<ExcepcionTecnica>(() => repositorio.InsertarDetalleYObtenerId(Detalle(corridaId, 2000000000, TipoNotificacionAutomatica.Compras, ResultadoGeneracionNotificacion.Omitida, "X", null)));
                Assert.ThrowsExactly<ExcepcionTecnica>(() => repositorio.InsertarDetalleYObtenerId(Detalle(corridaId, user, TipoNotificacionAutomatica.Compras, ResultadoGeneracionNotificacion.Entregada, null, 2000000000)));
                acceso.RollbackTransactionScope();
            }
        }

        [TestMethod]
        public void Registro_CorridaInvalida_SeRechaza()
        {
            using (AccesoDatosSqlServer acceso = new AccesoDatosSqlServer())
            {
                acceso.BeginTransactionScope();
                var repositorio = new GeneracionNotificacionesRepositorio(acceso);
                int corridaId = repositorio.InsertarCorridaYObtenerId(new CorridaGeneracionNotificaciones { InicioUtc = Entrega, Jornada = Jornada });

                var corrida = repositorio.ObtenerPorId(corridaId);
                Assert.ThrowsExactly<ExcepcionTecnica>(() => repositorio.FinalizarCorrida(corrida, Entrega.AddSeconds(-1), 0, 0, 0));
                Assert.ThrowsExactly<ExcepcionTecnica>(() => repositorio.FinalizarCorrida(corrida, Entrega, -1, 0, 0));
                Assert.IsFalse(repositorio.FinalizarCorrida(new CorridaGeneracionNotificaciones { Id = 2000000000 }, Entrega, 0, 0, 0));
                acceso.RollbackTransactionScope();
            }
        }

        [TestMethod]
        public void SqlDirecto_ResultadoOTipoDesconocido_SeRechaza()
        {
            using (AccesoDatosSqlServer acceso = new AccesoDatosSqlServer())
            {
                acceso.BeginTransactionScope();
                var repositorio = new GeneracionNotificacionesRepositorio(acceso);
                int user = Usuario(acceso, "user");
                int corridaId = repositorio.InsertarCorridaYObtenerId(new CorridaGeneracionNotificaciones { InicioUtc = Entrega, Jornada = Jornada });
                const string Sql = "INSERT INTO DetalleGeneracionNotificacion (CorridaId, UsuarioId, TipoAutomatico, Jornada, Resultado, Motivo, FechaUtc) VALUES (@C, @U, @T, '2026-10-20', @R, 'X', SYSUTCDATETIME())";

                foreach (string[] par in new[] { new[] { "Compras", "Pendiente" }, new[] { "Otro", "Omitida" } })
                {
                    Assert.ThrowsExactly<ExcepcionTecnica>(() => acceso.Write(Sql, new List<SqlParameter>
                    {
                        acceso.CreateParameter("@C", SqlDbType.Int, corridaId), acceso.CreateParameter("@U", SqlDbType.Int, user),
                        acceso.CreateParameter("@T", SqlDbType.VarChar, par[0]), acceso.CreateParameter("@R", SqlDbType.VarChar, par[1])
                    }));
                }
                acceso.RollbackTransactionScope();
            }
        }

        [TestMethod]
        public void Registro_EliminarLaNotificacionEntregada_ConservaElDetalleSinLaReferencia()
        {
            using (AccesoDatosSqlServer acceso = new AccesoDatosSqlServer())
            {
                acceso.BeginTransactionScope();
                var repositorio = new GeneracionNotificacionesRepositorio(acceso);
                int user = Usuario(acceso, "user");
                int notificacionId = Notificacion(acceso, user);
                int corridaId = repositorio.InsertarCorridaYObtenerId(new CorridaGeneracionNotificaciones { InicioUtc = Entrega, Jornada = Jornada });
                repositorio.InsertarDetalleYObtenerId(Detalle(corridaId, user, TipoNotificacionAutomatica.Movimientos, ResultadoGeneracionNotificacion.Entregada, null, notificacionId));

                Assert.IsTrue(new NotificacionRepositorio(acceso).Eliminar(new BE.Notificacion { Id = notificacionId }));

                DetalleGeneracionNotificacion detalle = repositorio.ObtenerDetallePorCorrida(repositorio.ObtenerPorId(corridaId)).Single();
                Assert.IsNull(detalle.Notificacion);
                Assert.AreEqual(ResultadoGeneracionNotificacion.Entregada, detalle.Resultado);
                acceso.RollbackTransactionScope();
            }
        }

        [TestMethod]
        public void Registro_UnidadDeTrabajoSinSave_NoDejaCorridaNiDetalle()
        {
            int usuarioId;
            using (var acceso = new AccesoDatosSqlServer())
                usuarioId = Usuario(acceso, "user");
            DateTime inicio = new DateTime(2098, 5, 5, 5, 5, 5, DateTimeKind.Utc);
            using (var unidad = new UnidadDeTrabajoFactory().Crear())
            {
                int corridaId = unidad.GeneracionNotificaciones.InsertarCorridaYObtenerId(new CorridaGeneracionNotificaciones { InicioUtc = inicio, Jornada = Jornada });
                unidad.GeneracionNotificaciones.InsertarDetalleYObtenerId(Detalle(corridaId, usuarioId, TipoNotificacionAutomatica.Compras, ResultadoGeneracionNotificacion.Omitida, "SinPendientes", null));
            }

            Assert.IsFalse(new GeneracionNotificacionesRepositorio().ObtenerRecientes(50).Any(c => c.InicioUtc == inicio));
        }

        private static DetalleGeneracionNotificacion Detalle(int corridaId, int usuarioId, TipoNotificacionAutomatica tipo, ResultadoGeneracionNotificacion resultado, string motivo, int? notificacionId)
        {
            return new DetalleGeneracionNotificacion
            {
                Corrida = new CorridaGeneracionNotificaciones { Id = corridaId }, Usuario = new BE.Usuario { Id = usuarioId }, Tipo = tipo, Jornada = Jornada, Resultado = resultado,
                Motivo = motivo, Notificacion = notificacionId.HasValue ? new BE.Notificacion { Id = notificacionId.Value } : null, FechaUtc = Entrega
            };
        }

        private static int Notificacion(AccesoDatosSqlServer acceso, int usuarioId)
        {
            return new NotificacionRepositorio(acceso).InsertarYObtenerId(new Notificacion
            {
                Usuario = new BE.Usuario { Id = usuarioId }, Mensaje = "Aviso de prueba", Fecha = Jornada, Leida = false
            });
        }

        private static int Usuario(AccesoDatosSqlServer acceso, string username)
        {
            return new UsuarioDAL().ObtenerPorUsername(username, acceso).Id;
        }
    }
}
