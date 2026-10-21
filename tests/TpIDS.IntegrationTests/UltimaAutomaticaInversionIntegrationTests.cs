using System;
using System.Threading.Tasks;
using BE;
using DAL;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using REPOSITORIO;

namespace TpIDS.IntegrationTests
{
    // Base del intervalo de siete dias de inversiones (notificaciones-automaticas-diarias): la fecha de
    // la ultima notificacion automatica de inversiones del usuario, y el bloqueo por usuario del que
    // dependera la entrega por lotes. Los tests con AccesoDatosSqlServer abren una transaccion y la
    // revierten; los de bloqueo usan unidades de trabajo que nunca llaman a Save.
    [TestClass]
    [TestCategory("Integration")]
    public sealed class UltimaAutomaticaInversionIntegrationTests
    {
        // Notificacion.Fecha es la hora local del equipo; el repositorio la informa en UTC.
        private static readonly DateTime Fecha = new DateTime(2026, 10, 13, 7, 0, 0, DateTimeKind.Local);

        private static DateTime Utc(DateTime local) => DateTime.SpecifyKind(local, DateTimeKind.Local).ToUniversalTime();

        [TestMethod]
        public void Obtener_UsuarioSinNotificacionesAutomaticasDeInversion_DevuelveNull()
        {
            using (AccesoDatosSqlServer acceso = new AccesoDatosSqlServer())
            {
                acceso.BeginTransactionScope();
                int usuarioId = Usuario(acceso, "user");

                Assert.IsNull(new NotificacionRepositorio(acceso).ObtenerFechaUltimaAutomaticaInversionUtc(new BE.Usuario { Id = usuarioId }));
                acceso.RollbackTransactionScope();
            }
        }

        [TestMethod]
        public void Obtener_VariasEntregas_DevuelveLaMasRecienteEnUtcSinRedondeo()
        {
            using (AccesoDatosSqlServer acceso = new AccesoDatosSqlServer())
            {
                acceso.BeginTransactionScope();
                var repositorio = new NotificacionRepositorio(acceso);
                int usuarioId = Usuario(acceso, "user");
                Insertar(acceso, usuarioId, Fecha, Fecha.Date);
                Insertar(acceso, usuarioId, Fecha.AddDays(14), Fecha.AddDays(14).Date);
                Insertar(acceso, usuarioId, Fecha.AddDays(7), Fecha.AddDays(7).Date);

                DateTime? ultima = repositorio.ObtenerFechaUltimaAutomaticaInversionUtc(new BE.Usuario { Id = usuarioId });

                Assert.IsTrue(ultima.HasValue);
                Assert.AreEqual(Utc(Fecha.AddDays(14)), ultima.Value);
                Assert.AreEqual(DateTimeKind.Utc, ultima.Value.Kind);
                acceso.RollbackTransactionScope();
            }
        }

        [TestMethod]
        public void Obtener_OtroUsuarioConEntregas_NoAfectaAlConsultado()
        {
            using (AccesoDatosSqlServer acceso = new AccesoDatosSqlServer())
            {
                acceso.BeginTransactionScope();
                var repositorio = new NotificacionRepositorio(acceso);
                int user = Usuario(acceso, "user");
                int admin = Usuario(acceso, "admin");
                Insertar(acceso, user, Fecha, Fecha.Date);

                Assert.IsNull(repositorio.ObtenerFechaUltimaAutomaticaInversionUtc(new BE.Usuario { Id = admin }));
                Insertar(acceso, admin, Fecha.AddDays(3), Fecha.AddDays(3).Date);
                Assert.AreEqual(Utc(Fecha), repositorio.ObtenerFechaUltimaAutomaticaInversionUtc(new BE.Usuario { Id = user }));
                Assert.AreEqual(Utc(Fecha.AddDays(3)), repositorio.ObtenerFechaUltimaAutomaticaInversionUtc(new BE.Usuario { Id = admin }));
                acceso.RollbackTransactionScope();
            }
        }

        [TestMethod]
        public void Obtener_IgnoraOtrosTiposEjemplosYRecordatoriosManuales()
        {
            using (AccesoDatosSqlServer acceso = new AccesoDatosSqlServer())
            {
                acceso.BeginTransactionScope();
                var repositorio = new NotificacionRepositorio(acceso);
                int usuarioId = Usuario(acceso, "user");
                DateTime fechaFutura = Fecha.AddDays(30);
                repositorio.InsertarYObtenerId(new Notificacion
                {
                    Usuario = new BE.Usuario { Id = usuarioId }, Mensaje = "movimientos", Fecha = fechaFutura,
                    TipoAutomatico = TipoNotificacionAutomatica.Movimientos, FechaJornada = fechaFutura.Date
                });
                new RecordatorioCompraRepositorio(acceso).InsertarYObtenerId(new RecordatorioCompra
                {
                    Usuario = new BE.Usuario { Id = usuarioId }, Mensaje = "compras", Fecha = fechaFutura,
                    TipoAutomatico = TipoNotificacionAutomatica.Compras, FechaJornada = fechaFutura.Date
                });
                new RecordatorioInversionRepositorio(acceso).InsertarYObtenerId(new RecordatorioInversion
                {
                    Usuario = new BE.Usuario { Id = usuarioId }, Mensaje = "ejemplo", Fecha = fechaFutura, ClaveEjemplo = "Demo.Inversiones"
                });
                new RecordatorioInversionRepositorio(acceso).InsertarYObtenerId(new RecordatorioInversion
                {
                    Usuario = new BE.Usuario { Id = usuarioId }, Mensaje = "manual", Fecha = fechaFutura
                });

                Assert.IsNull(repositorio.ObtenerFechaUltimaAutomaticaInversionUtc(new BE.Usuario { Id = usuarioId }));
                acceso.RollbackTransactionScope();
            }
        }

        [TestMethod]
        public void Obtener_NotificacionLeida_SigueContandoComoUltima()
        {
            using (AccesoDatosSqlServer acceso = new AccesoDatosSqlServer())
            {
                acceso.BeginTransactionScope();
                var repositorio = new NotificacionRepositorio(acceso);
                int usuarioId = Usuario(acceso, "user");
                int id = Insertar(acceso, usuarioId, Fecha, Fecha.Date);

                Assert.IsTrue(repositorio.MarcarPersonalLeida(new Notificacion { Id = id }, new BE.Usuario { Id = usuarioId }));

                Assert.AreEqual(Utc(Fecha), repositorio.ObtenerFechaUltimaAutomaticaInversionUtc(new BE.Usuario { Id = usuarioId }));
                acceso.RollbackTransactionScope();
            }
        }

        [TestMethod]
        public void Obtener_EliminadaLaUltima_VuelveALaAnteriorYSinNingunaEsNull()
        {
            using (AccesoDatosSqlServer acceso = new AccesoDatosSqlServer())
            {
                acceso.BeginTransactionScope();
                var repositorio = new NotificacionRepositorio(acceso);
                var recordatorios = new RecordatorioInversionRepositorio(acceso);
                int usuarioId = Usuario(acceso, "user");
                int anterior = Insertar(acceso, usuarioId, Fecha, Fecha.Date);
                int ultima = Insertar(acceso, usuarioId, Fecha.AddDays(7), Fecha.AddDays(7).Date);

                Assert.IsTrue(recordatorios.Eliminar(recordatorios.ObtenerPorId(ultima)));
                Assert.AreEqual(Utc(Fecha), repositorio.ObtenerFechaUltimaAutomaticaInversionUtc(new BE.Usuario { Id = usuarioId }));
                Assert.IsTrue(recordatorios.Eliminar(recordatorios.ObtenerPorId(anterior)));
                Assert.IsNull(repositorio.ObtenerFechaUltimaAutomaticaInversionUtc(new BE.Usuario { Id = usuarioId }));
                acceso.RollbackTransactionScope();
            }
        }

        [TestMethod]
        public void Bloquear_UsuarioExistenteEInexistente_InformaSiExiste()
        {
            using (AccesoDatosSqlServer acceso = new AccesoDatosSqlServer())
            {
                acceso.BeginTransactionScope();
                var repositorio = new NotificacionRepositorio(acceso);

                Assert.IsTrue(repositorio.BloquearUsuarioParaEntrega(new UsuarioDAL().ObtenerPorUsername("user", acceso)));
                Assert.IsFalse(repositorio.BloquearUsuarioParaEntrega(new BE.Usuario { Id = 2000000000 }));
                acceso.RollbackTransactionScope();
            }
        }

        [TestMethod]
        public async Task Bloquear_MismoUsuario_SerializaYOtroUsuarioNoEspera()
        {
            int user;
            int admin;
            using (var acceso = new AccesoDatosSqlServer())
            {
                user = Usuario(acceso, "user");
                admin = Usuario(acceso, "admin");
            }

            using (var primera = new UnidadDeTrabajoFactory().Crear())
            {
                Assert.IsTrue(primera.Notificaciones.BloquearUsuarioParaEntrega(new BE.Usuario { Id = user }));
                var mismoUsuario = Task.Run(() =>
                {
                    using (var segunda = new UnidadDeTrabajoFactory().Crear())
                        return segunda.Notificaciones.BloquearUsuarioParaEntrega(new BE.Usuario { Id = user });
                });
                var otroUsuario = Task.Run(() =>
                {
                    using (var tercera = new UnidadDeTrabajoFactory().Crear())
                        return tercera.Notificaciones.BloquearUsuarioParaEntrega(new BE.Usuario { Id = admin });
                });

                Assert.IsTrue(await Esperar(otroUsuario));
                await Task.Delay(700);
                Assert.IsFalse(mismoUsuario.IsCompleted, "La segunda entrega del mismo usuario debe esperar a la primera.");
                primera.Dispose();
                Assert.IsTrue(await Esperar(mismoUsuario));
            }
        }

        private static async Task<bool> Esperar(Task<bool> tarea)
        {
            Task terminada = await Task.WhenAny(tarea, Task.Delay(TimeSpan.FromSeconds(10)));
            Assert.AreSame(tarea, terminada, "La operacion no termino a tiempo.");
            return await tarea;
        }

        private static int Insertar(AccesoDatosSqlServer acceso, int usuarioId, DateTime fecha, DateTime jornada)
        {
            return new RecordatorioInversionRepositorio(acceso).InsertarYObtenerId(new RecordatorioInversion
            {
                Usuario = new BE.Usuario { Id = usuarioId }, Mensaje = "inversion automatica", Fecha = fecha, Leida = false,
                TipoAutomatico = TipoNotificacionAutomatica.Inversiones, FechaJornada = jornada
            });
        }

        private static int Usuario(AccesoDatosSqlServer acceso, string username)
        {
            return new UsuarioDAL().ObtenerPorUsername(username, acceso).Id;
        }
    }
}
