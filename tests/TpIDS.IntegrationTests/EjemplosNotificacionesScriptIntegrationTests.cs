using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using BE;
using DAL;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using REPOSITORIO;

namespace TpIDS.IntegrationTests
{
    // Ejemplos de laboratorio de script_datos_prueba.sql (notificaciones-automaticas-diarias 6.1/6.2).
    // El script completo apunta a la base de desarrollo y presupone catalogos cargados, asi que el test
    // ejecuta solo el bloque delimitado por marcadores, dentro de una transaccion que se revierte.
    [TestClass]
    [TestCategory("Integration")]
    public sealed class EjemplosNotificacionesScriptIntegrationTests
    {
        private const string Inicio = "-- INICIO ejemplos de notificaciones";
        private const string Fin = "-- FIN ejemplos de notificaciones";
        private static readonly string[] Claves = { "Demo.Compras", "Demo.Inversiones", "Demo.Movimientos" };

        [TestMethod]
        public void Bloque_PrimeraCarga_DejaTresEjemplosCompletosSinLeerYSinMetadataAutomatica()
        {
            using (AccesoDatosSqlServer acceso = new AccesoDatosSqlServer())
            {
                acceso.BeginTransactionScope();
                var repositorio = new NotificacionRepositorio(acceso);
                int usuarioId = new UsuarioDAL().ObtenerPorUsername("user", acceso).Id;
                int basal = repositorio.ContarNoLeidas(new Usuario { Id = usuarioId });

                CargarBloque(acceso, usuarioId);

                List<Notificacion> ejemplos = Ejemplos(repositorio, usuarioId, null);
                Assert.AreEqual(3, ejemplos.Count);
                Assert.AreEqual(basal + 3, repositorio.ContarNoLeidas(new Usuario { Id = usuarioId }));
                foreach (Notificacion e in ejemplos)
                {
                    Assert.IsFalse(e.Leida, e.ClaveEjemplo);
                    Assert.IsNull(e.TipoAutomatico, e.ClaveEjemplo);
                    Assert.IsNull(e.FechaJornada, e.ClaveEjemplo);
                    Assert.Contains("ejemplo", e.Mensaje.ToLowerInvariant(), e.ClaveEjemplo);
                    Assert.Contains("\r\n", e.Mensaje, e.ClaveEjemplo);
                    Assert.IsFalse(e.Mensaje.Contains("?"), "Unicode degradado a '?' en " + e.ClaveEjemplo);
                }
                RecordatorioCompra compras = (RecordatorioCompra)ejemplos.Single(e => e.ClaveEjemplo == "Demo.Compras");
                Assert.IsNull(compras.FechaVencimiento);
                Assert.IsInstanceOfType<RecordatorioInversion>(ejemplos.Single(e => e.ClaveEjemplo == "Demo.Inversiones"));
                Assert.IsFalse(ejemplos.Single(e => e.ClaveEjemplo == "Demo.Movimientos") is RecordatorioCompra);
                Assert.Contains("Serenísima", compras.Mensaje);
                // Lectura de Mis notificaciones: los tres aparecen en la primera pagina y se abren completos.
                PaginaNotificaciones primera = repositorio.ObtenerPaginaPersonal(new Usuario { Id = usuarioId }, null, 1);
                foreach (Notificacion e in ejemplos)
                {
                    Notificacion enPagina = primera.Elementos.Single(p => p.Id == e.Id);
                    Assert.AreEqual(e.Mensaje, repositorio.ObtenerPersonal(e, new Usuario { Id = usuarioId }).Mensaje);
                    Assert.IsFalse(enPagina.Leida);
                }
                acceso.RollbackTransactionScope();
            }
        }

        [TestMethod]
        public void Bloque_SegundaCargaOtroDia_NoDuplicaNiReiniciaLecturaNiFechas()
        {
            using (AccesoDatosSqlServer acceso = new AccesoDatosSqlServer())
            {
                acceso.BeginTransactionScope();
                var repositorio = new NotificacionRepositorio(acceso);
                int usuarioId = new UsuarioDAL().ObtenerPorUsername("user", acceso).Id;
                CargarBloque(acceso, usuarioId);
                Notificacion compras = Ejemplos(repositorio, usuarioId, null).Single(e => e.ClaveEjemplo == "Demo.Compras");
                Assert.IsTrue(repositorio.MarcarPersonalLeida(compras, new Usuario { Id = usuarioId }));
                DateTime antigua = new DateTime(2020, 1, 1, 8, 0, 0);
                acceso.Write("UPDATE dbo.Notificacion SET Fecha = @F WHERE UsuarioId = @U AND ClaveEjemplo LIKE 'Demo.%'",
                    new List<SqlParameter>
                    {
                        acceso.CreateParameter("@F", System.Data.SqlDbType.DateTime, antigua),
                        acceso.CreateParameter("@U", System.Data.SqlDbType.Int, usuarioId)
                    });

                CargarBloque(acceso, usuarioId);

                List<Notificacion> ejemplos = Ejemplos(repositorio, usuarioId, new FiltroNotificacion { FechaDesde = antigua.Date, FechaHasta = antigua.Date });
                Assert.AreEqual(3, ejemplos.Count);
                Assert.IsTrue(ejemplos.Single(e => e.ClaveEjemplo == "Demo.Compras").Leida);
                Assert.AreEqual(2, ejemplos.Count(e => !e.Leida));
                Assert.IsTrue(ejemplos.All(e => e.Fecha == antigua));
                acceso.RollbackTransactionScope();
            }
        }

        [TestMethod]
        public void Bloque_NoModificaUsuarios()
        {
            Assert.IsFalse(BloqueDelScript().Contains("dbo.Usuarios SET") || BloqueDelScript().Contains("UPDATE dbo.Usuarios"));
        }

        // ObtenerPaginaPersonal devuelve las tres especializaciones; ObtenerPorUsuario solo la base.
        private static List<Notificacion> Ejemplos(NotificacionRepositorio repositorio, int usuarioId, FiltroNotificacion filtro)
        {
            return repositorio.ObtenerPaginaPersonal(new Usuario { Id = usuarioId }, filtro, 1).Elementos.Where(n => n.ClaveEjemplo != null && Claves.Contains(n.ClaveEjemplo)).ToList();
        }

        private static void CargarBloque(AccesoDatosSqlServer acceso, int usuarioId)
        {
            acceso.Write("DECLARE @uid INT = " + usuarioId + ";" + Environment.NewLine + BloqueDelScript());
        }

        private static string BloqueDelScript()
        {
            string dir = AppDomain.CurrentDomain.BaseDirectory;
            while (dir != null && !File.Exists(Path.Combine(dir, "script_datos_prueba.sql"))) dir = Path.GetDirectoryName(dir);
            Assert.IsNotNull(dir, "No se encontro script_datos_prueba.sql subiendo desde el directorio de pruebas.");
            string texto = File.ReadAllText(Path.Combine(dir, "script_datos_prueba.sql"));
            int desde = texto.IndexOf(Inicio, StringComparison.Ordinal);
            int hasta = texto.IndexOf(Fin, StringComparison.Ordinal);
            Assert.IsTrue(desde >= 0 && hasta > desde, "Faltan los marcadores del bloque de ejemplos en script_datos_prueba.sql.");
            return texto.Substring(desde, hasta - desde);
        }
    }
}
