using System;
using System.Collections.Generic;
using System.Linq;
using BE;
using DAL;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using REPOSITORIO;

namespace TpIDS.IntegrationTests
{
    // Consultas de soporte del proceso automatico de notificaciones (notificaciones-automaticas-diarias 4):
    // deduplicacion previa por usuario, tipo y jornada, y pagina de cuentas activas. Los tests con
    // AccesoDatosSqlServer abren una transaccion y la revierten.
    [TestClass]
    [TestCategory("Integration")]
    public sealed class EntregaNotificacionesIntegrationTests
    {
        private static readonly DateTime Jornada = new DateTime(2099, 3, 4);

        [TestMethod]
        public void ExisteAutomatica_SoloParaElUsuarioTipoYJornadaEntregados()
        {
            using (AccesoDatosSqlServer acceso = new AccesoDatosSqlServer())
            {
                acceso.BeginTransactionScope();
                var repositorio = new NotificacionRepositorio(acceso);
                int user = Usuario(acceso, "user");
                int admin = Usuario(acceso, "admin");
                Assert.IsFalse(repositorio.ExisteAutomatica(new BE.Usuario { Id = user }, TipoNotificacionAutomatica.Movimientos, Jornada));

                repositorio.InsertarYObtenerId(new Notificacion
                {
                    Usuario = new BE.Usuario { Id = user }, Mensaje = "movimientos", Fecha = Jornada.AddHours(7),
                    TipoAutomatico = TipoNotificacionAutomatica.Movimientos, FechaJornada = Jornada
                });

                Assert.IsTrue(repositorio.ExisteAutomatica(new BE.Usuario { Id = user }, TipoNotificacionAutomatica.Movimientos, Jornada));
                Assert.IsTrue(repositorio.ExisteAutomatica(new BE.Usuario { Id = user }, TipoNotificacionAutomatica.Movimientos, Jornada.AddHours(9)), "La jornada es una fecha, sin hora.");
                Assert.IsFalse(repositorio.ExisteAutomatica(new BE.Usuario { Id = user }, TipoNotificacionAutomatica.Movimientos, Jornada.AddDays(1)));
                Assert.IsFalse(repositorio.ExisteAutomatica(new BE.Usuario { Id = user }, TipoNotificacionAutomatica.Compras, Jornada));
                Assert.IsFalse(repositorio.ExisteAutomatica(new BE.Usuario { Id = admin }, TipoNotificacionAutomatica.Movimientos, Jornada));
                acceso.RollbackTransactionScope();
            }
        }

        [TestMethod]
        public void ExisteAutomatica_RecordatorioDeInversionYEjemploDeLaboratorio()
        {
            using (AccesoDatosSqlServer acceso = new AccesoDatosSqlServer())
            {
                acceso.BeginTransactionScope();
                var repositorio = new NotificacionRepositorio(acceso);
                int user = Usuario(acceso, "user");
                new RecordatorioInversionRepositorio(acceso).InsertarYObtenerId(new RecordatorioInversion
                {
                    Usuario = new BE.Usuario { Id = user }, Mensaje = "ejemplo", Fecha = Jornada.AddHours(7), ClaveEjemplo = "Demo.Inversiones"
                });
                Assert.IsFalse(repositorio.ExisteAutomatica(new BE.Usuario { Id = user }, TipoNotificacionAutomatica.Inversiones, Jornada), "Un ejemplo no es una entrega automatica.");

                new RecordatorioInversionRepositorio(acceso).InsertarYObtenerId(new RecordatorioInversion
                {
                    Usuario = new BE.Usuario { Id = user }, Mensaje = "inversion", Fecha = Jornada.AddHours(7),
                    TipoAutomatico = TipoNotificacionAutomatica.Inversiones, FechaJornada = Jornada
                });

                Assert.IsTrue(repositorio.ExisteAutomatica(new BE.Usuario { Id = user }, TipoNotificacionAutomatica.Inversiones, Jornada));
                acceso.RollbackTransactionScope();
            }
        }

        [TestMethod]
        public void ObtenerActivosDespuesDe_PaginaPorIdAscendenteYExcluyeInactivas()
        {
            using (AccesoDatosSqlServer acceso = new AccesoDatosSqlServer())
            {
                acceso.BeginTransactionScope();
                var repositorio = new UsuarioRepositorio(acceso);
                int user = Usuario(acceso, "user");
                int admin = Usuario(acceso, "admin");
                List<BE.Usuario> todosActivos = repositorio.ObtenerActivosDespuesDe(null, 1000);
                Assert.IsTrue(todosActivos.Any(u => u.Id == user) && todosActivos.Any(u => u.Id == admin));
                CollectionAssert.AreEqual(todosActivos.OrderBy(u => u.Id).ToList(), todosActivos);
                Assert.IsTrue(todosActivos.All(u => !string.IsNullOrWhiteSpace(u.Username)));

                // Paginacion por clave: la pagina siguiente empieza despues del ultimo Id visto.
                List<BE.Usuario> primera = repositorio.ObtenerActivosDespuesDe(null, 1);
                List<BE.Usuario> segunda = repositorio.ObtenerActivosDespuesDe(primera[0], 1);
                Assert.AreEqual(1, primera.Count);
                Assert.AreEqual(todosActivos[0].Id, primera[0].Id);
                Assert.AreEqual(todosActivos[1].Id, segunda[0].Id);
                Assert.IsEmpty(repositorio.ObtenerActivosDespuesDe(todosActivos.Last(), 10));

                acceso.Write("UPDATE Usuarios SET Activo = 0 WHERE Id = @Id", new List<System.Data.SqlClient.SqlParameter> { acceso.CreateParameter("@Id", System.Data.SqlDbType.Int, admin) });
                Assert.IsFalse(repositorio.ObtenerActivosDespuesDe(null, 1000).Any(u => u.Id == admin));
                acceso.RollbackTransactionScope();
            }
        }

        private static int Usuario(AccesoDatosSqlServer acceso, string username)
        {
            return new UsuarioDAL().ObtenerPorUsername(username, acceso).Id;
        }
    }
}
