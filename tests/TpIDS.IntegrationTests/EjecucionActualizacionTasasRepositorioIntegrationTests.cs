using System;
using System.Collections.Generic;
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
    public sealed class EjecucionActualizacionTasasRepositorioIntegrationTests
    {
        [TestMethod]
        public void InsertarYObtenerId_EjecucionValida_PersisteTodosLosCampos()
        {
            using (AccesoDatosSqlServer accesoDatos = new AccesoDatosSqlServer())
            {
                accesoDatos.BeginTransactionScope();
                LimpiarHistorial(accesoDatos);
                EjecucionActualizacionTasasRepositorio repositorio = new EjecucionActualizacionTasasRepositorio(accesoDatos);
                DateTime inicio = new DateTime(2031, 3, 10, 14, 30, 0);

                int id = repositorio.InsertarYObtenerId(NuevaEjecucion(
                    inicio, BE.OrigenActualizacionTasas.Manual, BE.ResultadoActualizacionTasas.Fallida, true, "Clave invalida"));

                BE.EjecucionActualizacionTasas obtenida = repositorio.ObtenerRecientes(1).Single();
                Assert.AreEqual(id, obtenida.Id);
                Assert.AreEqual(inicio, obtenida.FechaInicio);
                Assert.AreEqual(BE.OrigenActualizacionTasas.Manual, obtenida.Origen);
                Assert.AreEqual(BE.ResultadoActualizacionTasas.Fallida, obtenida.Resultado);
                Assert.IsTrue(obtenida.ConsumioRequest);
                Assert.AreEqual("Clave invalida", obtenida.Mensaje);

                accesoDatos.RollbackTransactionScope();
            }
        }

        [TestMethod]
        public void InsertarYObtenerId_MensajeNulo_SeRecuperaNulo()
        {
            using (AccesoDatosSqlServer accesoDatos = new AccesoDatosSqlServer())
            {
                accesoDatos.BeginTransactionScope();
                LimpiarHistorial(accesoDatos);
                EjecucionActualizacionTasasRepositorio repositorio = new EjecucionActualizacionTasasRepositorio(accesoDatos);

                repositorio.InsertarYObtenerId(NuevaEjecucion(
                    new DateTime(2031, 3, 10, 9, 0, 0), BE.OrigenActualizacionTasas.Programada, BE.ResultadoActualizacionTasas.Exitosa, true, null));

                Assert.IsNull(repositorio.ObtenerRecientes(1).Single().Mensaje);

                accesoDatos.RollbackTransactionScope();
            }
        }

        [TestMethod]
        public void ObtenerRecientes_VariasEjecuciones_DevuelveLaMasNuevaPrimeroYRespetaLaCantidad()
        {
            using (AccesoDatosSqlServer accesoDatos = new AccesoDatosSqlServer())
            {
                accesoDatos.BeginTransactionScope();
                LimpiarHistorial(accesoDatos);
                EjecucionActualizacionTasasRepositorio repositorio = new EjecucionActualizacionTasasRepositorio(accesoDatos);
                int idVieja = InsertarEjecucion(repositorio, new DateTime(2031, 3, 10, 8, 0, 0), BE.ResultadoActualizacionTasas.Exitosa, true);
                int idNueva = InsertarEjecucion(repositorio, new DateTime(2031, 3, 10, 10, 0, 0), BE.ResultadoActualizacionTasas.Exitosa, true);
                int idMedia = InsertarEjecucion(repositorio, new DateTime(2031, 3, 10, 9, 0, 0), BE.ResultadoActualizacionTasas.Omitida, false);

                List<BE.EjecucionActualizacionTasas> todas = repositorio.ObtenerRecientes(10);
                List<BE.EjecucionActualizacionTasas> dos = repositorio.ObtenerRecientes(2);

                CollectionAssert.AreEqual(new[] { idNueva, idMedia, idVieja }, todas.Select(item => item.Id).ToArray());
                CollectionAssert.AreEqual(new[] { idNueva, idMedia }, dos.Select(item => item.Id).ToArray());

                accesoDatos.RollbackTransactionScope();
            }
        }

        [TestMethod]
        public void ObtenerDetallePorEjecucion_DetalleConValorNuevoNulo_SeRecuperaConMonedasYEstado()
        {
            using (AccesoDatosSqlServer accesoDatos = new AccesoDatosSqlServer())
            {
                accesoDatos.BeginTransactionScope();
                LimpiarHistorial(accesoDatos);
                EjecucionActualizacionTasasRepositorio repositorio = new EjecucionActualizacionTasasRepositorio(accesoDatos);
                MonedaRepositorio monedaRepositorio = new MonedaRepositorio(accesoDatos);
                BE.Moneda ars = monedaRepositorio.ObtenerTodas().Single(item => item.Codigo == "ARS");
                BE.Moneda usd = monedaRepositorio.ObtenerTodas().Single(item => item.Codigo == "USD");
                int ejecucionId = InsertarEjecucion(repositorio, new DateTime(2031, 3, 10, 10, 0, 0), BE.ResultadoActualizacionTasas.Exitosa, true);
                int otraEjecucionId = InsertarEjecucion(repositorio, new DateTime(2031, 3, 10, 11, 0, 0), BE.ResultadoActualizacionTasas.Exitosa, true);

                repositorio.InsertarDetalleYObtenerId(new BE.ActualizacionTasaDetalle
                {
                    Ejecucion = new BE.EjecucionActualizacionTasas { Id = ejecucionId },
                    MonedaBase = usd,
                    MonedaConversion = ars,
                    ValorAnterior = 1500.5m,
                    ValorNuevo = 1511.123456m,
                    Estado = BE.EstadoActualizacionTasa.Actualizado
                });
                repositorio.InsertarDetalleYObtenerId(new BE.ActualizacionTasaDetalle
                {
                    Ejecucion = new BE.EjecucionActualizacionTasas { Id = ejecucionId },
                    MonedaBase = ars,
                    MonedaConversion = usd,
                    ValorAnterior = 0.0007m,
                    ValorNuevo = null,
                    Estado = BE.EstadoActualizacionTasa.SaltadoManual
                });
                repositorio.InsertarDetalleYObtenerId(new BE.ActualizacionTasaDetalle
                {
                    Ejecucion = new BE.EjecucionActualizacionTasas { Id = otraEjecucionId },
                    MonedaBase = usd,
                    MonedaConversion = ars,
                    ValorAnterior = 1m,
                    ValorNuevo = 2m,
                    Estado = BE.EstadoActualizacionTasa.SaltadoSinDato
                });

                BE.EjecucionActualizacionTasas ejecucion = repositorio.ObtenerRecientes(10).Single(item => item.Id == ejecucionId);
                List<BE.ActualizacionTasaDetalle> detalles = repositorio.ObtenerDetallePorEjecucion(ejecucion);

                Assert.AreEqual(2, detalles.Count);
                Assert.AreSame(ejecucion, detalles[0].Ejecucion);
                Assert.AreEqual("USD", detalles[0].MonedaBase.Codigo);
                Assert.AreEqual("ARS", detalles[0].MonedaConversion.Codigo);
                Assert.AreEqual(1500.5m, detalles[0].ValorAnterior);
                Assert.AreEqual(1511.123456m, detalles[0].ValorNuevo);
                Assert.AreEqual(BE.EstadoActualizacionTasa.Actualizado, detalles[0].Estado);
                Assert.AreEqual("ARS", detalles[1].MonedaBase.Codigo);
                Assert.IsNull(detalles[1].ValorNuevo);
                Assert.AreEqual(BE.EstadoActualizacionTasa.SaltadoManual, detalles[1].Estado);

                accesoDatos.RollbackTransactionScope();
            }
        }

        [TestMethod]
        public void ObtenerUltimaExitosa_HayFallidasYOmitidasMasNuevas_DevuelveLaUltimaExitosa()
        {
            using (AccesoDatosSqlServer accesoDatos = new AccesoDatosSqlServer())
            {
                accesoDatos.BeginTransactionScope();
                LimpiarHistorial(accesoDatos);
                EjecucionActualizacionTasasRepositorio repositorio = new EjecucionActualizacionTasasRepositorio(accesoDatos);
                InsertarEjecucion(repositorio, new DateTime(2031, 3, 10, 7, 0, 0), BE.ResultadoActualizacionTasas.Exitosa, true);
                int idUltimaExitosa = InsertarEjecucion(repositorio, new DateTime(2031, 3, 10, 8, 0, 0), BE.ResultadoActualizacionTasas.Exitosa, true);
                InsertarEjecucion(repositorio, new DateTime(2031, 3, 10, 9, 0, 0), BE.ResultadoActualizacionTasas.Fallida, true);
                InsertarEjecucion(repositorio, new DateTime(2031, 3, 10, 10, 0, 0), BE.ResultadoActualizacionTasas.Omitida, false);

                BE.EjecucionActualizacionTasas ultima = repositorio.ObtenerUltimaExitosa();

                Assert.IsNotNull(ultima);
                Assert.AreEqual(idUltimaExitosa, ultima.Id);
                Assert.AreEqual(new DateTime(2031, 3, 10, 8, 0, 0), ultima.FechaInicio);
                Assert.AreEqual(BE.ResultadoActualizacionTasas.Exitosa, ultima.Resultado);

                accesoDatos.RollbackTransactionScope();
            }
        }

        [TestMethod]
        public void ObtenerUltimaExitosa_SinExitos_DevuelveNull()
        {
            using (AccesoDatosSqlServer accesoDatos = new AccesoDatosSqlServer())
            {
                accesoDatos.BeginTransactionScope();
                LimpiarHistorial(accesoDatos);
                EjecucionActualizacionTasasRepositorio repositorio = new EjecucionActualizacionTasasRepositorio(accesoDatos);
                InsertarEjecucion(repositorio, new DateTime(2031, 3, 10, 9, 0, 0), BE.ResultadoActualizacionTasas.Fallida, true);

                Assert.IsNull(repositorio.ObtenerUltimaExitosa());
                Assert.IsNull(repositorio.ObtenerUltimaExitosaConBloqueo());

                accesoDatos.RollbackTransactionScope();
            }
        }

        [TestMethod]
        public void ObtenerUltimaExitosaConBloqueo_DevuelveLaMismaEjecucionQueSinBloqueo()
        {
            using (AccesoDatosSqlServer accesoDatos = new AccesoDatosSqlServer())
            {
                accesoDatos.BeginTransactionScope();
                LimpiarHistorial(accesoDatos);
                EjecucionActualizacionTasasRepositorio repositorio = new EjecucionActualizacionTasasRepositorio(accesoDatos);
                int id = InsertarEjecucion(repositorio, new DateTime(2031, 3, 10, 8, 0, 0), BE.ResultadoActualizacionTasas.Exitosa, true);

                BE.EjecucionActualizacionTasas conBloqueo = repositorio.ObtenerUltimaExitosaConBloqueo();

                Assert.IsNotNull(conBloqueo);
                Assert.AreEqual(id, conBloqueo.Id);

                accesoDatos.RollbackTransactionScope();
            }
        }

        [TestMethod]
        public void ObtenerUltimaExitosaConBloqueo_OtraConexionConLaTransaccionAbierta_QuedaBloqueada()
        {
            using (AccesoDatosSqlServer primera = new AccesoDatosSqlServer())
            using (AccesoDatosSqlServer segunda = new AccesoDatosSqlServer())
            {
                primera.BeginTransactionScope();
                segunda.BeginTransactionScope();
                segunda.Write("SET LOCK_TIMEOUT 1000");
                EjecucionActualizacionTasasRepositorio repositorioPrimera = new EjecucionActualizacionTasasRepositorio(primera);
                EjecucionActualizacionTasasRepositorio repositorioSegunda = new EjecucionActualizacionTasasRepositorio(segunda);

                repositorioPrimera.ObtenerUltimaExitosaConBloqueo();

                Assert.ThrowsExactly<SERVICIOS.ExcepcionTecnica>(() => repositorioSegunda.ObtenerUltimaExitosaConBloqueo());

                segunda.RollbackTransactionScope();
                primera.RollbackTransactionScope();
            }
        }

        [TestMethod]
        public void ContarRequestsDelMes_CuentaSoloLasConsumidasDelMesCalendario()
        {
            using (AccesoDatosSqlServer accesoDatos = new AccesoDatosSqlServer())
            {
                accesoDatos.BeginTransactionScope();
                LimpiarHistorial(accesoDatos);
                EjecucionActualizacionTasasRepositorio repositorio = new EjecucionActualizacionTasasRepositorio(accesoDatos);
                InsertarEjecucion(repositorio, new DateTime(2031, 2, 28, 23, 59, 59), BE.ResultadoActualizacionTasas.Exitosa, true);
                InsertarEjecucion(repositorio, new DateTime(2031, 3, 1, 0, 0, 0), BE.ResultadoActualizacionTasas.Exitosa, true);
                InsertarEjecucion(repositorio, new DateTime(2031, 3, 15, 12, 0, 0), BE.ResultadoActualizacionTasas.Fallida, true);
                InsertarEjecucion(repositorio, new DateTime(2031, 3, 20, 12, 0, 0), BE.ResultadoActualizacionTasas.Omitida, false);
                InsertarEjecucion(repositorio, new DateTime(2031, 3, 31, 23, 59, 59), BE.ResultadoActualizacionTasas.Exitosa, true);
                InsertarEjecucion(repositorio, new DateTime(2031, 4, 1, 0, 0, 0), BE.ResultadoActualizacionTasas.Exitosa, true);

                Assert.AreEqual(3, repositorio.ContarRequestsDelMes(new DateTime(2031, 3, 17, 9, 0, 0)));
                Assert.AreEqual(1, repositorio.ContarRequestsDelMes(new DateTime(2031, 2, 1)));
                Assert.AreEqual(0, repositorio.ContarRequestsDelMes(new DateTime(2031, 5, 10)));

                accesoDatos.RollbackTransactionScope();
            }
        }

        private static BE.EjecucionActualizacionTasas NuevaEjecucion(
            DateTime inicio,
            BE.OrigenActualizacionTasas origen,
            BE.ResultadoActualizacionTasas resultado,
            bool consumioRequest,
            string mensaje)
        {
            return new BE.EjecucionActualizacionTasas
            {
                FechaInicio = inicio,
                Origen = origen,
                Resultado = resultado,
                ConsumioRequest = consumioRequest,
                Mensaje = mensaje
            };
        }

        private static int InsertarEjecucion(
            EjecucionActualizacionTasasRepositorio repositorio,
            DateTime inicio,
            BE.ResultadoActualizacionTasas resultado,
            bool consumioRequest)
        {
            return repositorio.InsertarYObtenerId(NuevaEjecucion(
                inicio, BE.OrigenActualizacionTasas.Programada, resultado, consumioRequest, "Test"));
        }

        // La base de pruebas puede tener historial de otras corridas; dentro de la transaccion
        // del test se vacia para que los conteos y los "ultimos" sean deterministas.
        private static void LimpiarHistorial(AccesoDatosSqlServer accesoDatos)
        {
            accesoDatos.Write("DELETE FROM ActualizacionTasaDetalle");
            accesoDatos.Write("DELETE FROM EjecucionActualizacionTasas");
        }
    }
}
