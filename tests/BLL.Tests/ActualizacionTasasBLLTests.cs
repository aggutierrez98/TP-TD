using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ABSTRACCIONES;
using BE;
using BLL.Tests.TestDoubles;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BLL.Tests
{
    [TestClass]
    public sealed class ActualizacionTasasBLLTests
    {
        private static readonly DateTime Ahora = new DateTime(2026, 10, 16, 12, 0, 0);

        [TestMethod]
        public async Task EjecutarAsync_TresPares_ConsultaUnaSolaVezYActualizaLosTres()
        {
            ActualizacionFixture fixture = new ActualizacionFixture();
            fixture.AgregarTasa(1, "USD", "ARS", 1m);
            fixture.AgregarTasa(2, "USD", "EUR", 1m);
            fixture.AgregarTasa(3, "ARS", "EUR", 1m);
            fixture.Proveedor.Resultado = fixture.Respuesta(R("ARS", 1500m), R("EUR", 0.9m));
            ActualizacionTasasBLL sut = fixture.CreateSut();

            ResultadoEjecucionActualizacionTasas resultado = await sut.EjecutarAsync(OrigenActualizacionTasas.Programada, CancellationToken.None);

            Assert.AreEqual(1, fixture.Proveedor.Llamadas);
            Assert.AreEqual(ResultadoActualizacionTasas.Exitosa, resultado.Ejecucion.Resultado);
            Assert.AreEqual(3, resultado.Ejecucion.Detalles.Count(d => d.Estado == EstadoActualizacionTasa.Actualizado));
            Assert.AreEqual(1500m, fixture.TasaRepo.Tasas[1].CalculoConversion);
            Assert.AreEqual(0.9m, fixture.TasaRepo.Tasas[2].CalculoConversion);
            Assert.AreEqual(0.0006m, fixture.TasaRepo.Tasas[3].CalculoConversion);
        }

        [TestMethod]
        public async Task EjecutarAsync_ParSinUsd_CalculaCruceConConversionSobreBase()
        {
            ActualizacionFixture fixture = new ActualizacionFixture();
            fixture.AgregarTasa(1, "ARS", "EUR", 1m);
            fixture.Proveedor.Resultado = fixture.Respuesta(R("ARS", 1500m), R("EUR", 0.9m));

            await fixture.CreateSut().EjecutarAsync(OrigenActualizacionTasas.Programada, CancellationToken.None);

            Assert.AreEqual(0.0006m, fixture.TasaRepo.Tasas[1].CalculoConversion);
        }

        [TestMethod]
        public async Task EjecutarAsync_MonedaBaseDelProveedorNoListada_LaTomaComoUno()
        {
            ActualizacionFixture fixture = new ActualizacionFixture();
            fixture.AgregarTasa(1, "USD", "ARS", 1m);
            fixture.AgregarTasa(2, "ARS", "USD", 1m);
            fixture.Proveedor.Resultado = fixture.Respuesta(R("ARS", 1500m));

            await fixture.CreateSut().EjecutarAsync(OrigenActualizacionTasas.Programada, CancellationToken.None);

            Assert.AreEqual(1500m, fixture.TasaRepo.Tasas[1].CalculoConversion);
            Assert.AreEqual(Math.Round(1m / 1500m, 6), fixture.TasaRepo.Tasas[2].CalculoConversion);
        }

        [TestMethod]
        public async Task EjecutarAsync_CodigosEnOtraCapitalizacion_LosEmpareja()
        {
            ActualizacionFixture fixture = new ActualizacionFixture();
            fixture.AgregarTasa(1, "usd", "ars", 1m);
            fixture.Proveedor.Resultado = fixture.Respuesta(R("ARS", 1500m));

            await fixture.CreateSut().EjecutarAsync(OrigenActualizacionTasas.Programada, CancellationToken.None);

            Assert.AreEqual(1500m, fixture.TasaRepo.Tasas[1].CalculoConversion);
        }

        [TestMethod]
        public async Task EjecutarAsync_ValorConMuchosDecimales_RedondeaASeis()
        {
            ActualizacionFixture fixture = new ActualizacionFixture();
            fixture.AgregarTasa(1, "ARS", "EUR", 1m);
            fixture.Proveedor.Resultado = fixture.Respuesta(R("ARS", 3m), R("EUR", 1m));

            await fixture.CreateSut().EjecutarAsync(OrigenActualizacionTasas.Programada, CancellationToken.None);

            Assert.AreEqual(0.333333m, fixture.TasaRepo.Tasas[1].CalculoConversion);
        }

        [TestMethod]
        public async Task EjecutarAsync_ParManual_NoLoToca()
        {
            ActualizacionFixture fixture = new ActualizacionFixture();
            fixture.AgregarTasa(1, "USD", "ARS", 999m).EsManual = true;
            fixture.AgregarTasa(2, "USD", "EUR", 1m);
            fixture.Proveedor.Resultado = fixture.Respuesta(R("ARS", 1500m), R("EUR", 0.9m));

            ResultadoEjecucionActualizacionTasas resultado = await fixture.CreateSut().EjecutarAsync(OrigenActualizacionTasas.Programada, CancellationToken.None);

            Assert.AreEqual(999m, fixture.TasaRepo.Tasas[1].CalculoConversion);
            Assert.AreEqual(0.9m, fixture.TasaRepo.Tasas[2].CalculoConversion);
            ActualizacionTasaDetalle manual = resultado.Ejecucion.Detalles.Single(d => d.MonedaConversion.Codigo == "ARS");
            Assert.AreEqual(EstadoActualizacionTasa.SaltadoManual, manual.Estado);
            Assert.AreEqual(999m, manual.ValorAnterior);
            Assert.IsNull(manual.ValorNuevo);
        }

        [TestMethod]
        public async Task EjecutarAsync_MonedaAusenteEnRespuesta_SaltaElParYActualizaLosDemas()
        {
            ActualizacionFixture fixture = new ActualizacionFixture();
            fixture.AgregarTasa(1, "USD", "ARS", 10m);
            fixture.AgregarTasa(2, "USD", "EUR", 1m);
            fixture.Proveedor.Resultado = fixture.Respuesta(R("EUR", 0.9m));

            ResultadoEjecucionActualizacionTasas resultado = await fixture.CreateSut().EjecutarAsync(OrigenActualizacionTasas.Programada, CancellationToken.None);

            Assert.AreEqual(ResultadoActualizacionTasas.Exitosa, resultado.Ejecucion.Resultado);
            Assert.AreEqual(10m, fixture.TasaRepo.Tasas[1].CalculoConversion);
            Assert.AreEqual(0.9m, fixture.TasaRepo.Tasas[2].CalculoConversion);
            ActualizacionTasaDetalle saltado = resultado.Ejecucion.Detalles.Single(d => d.MonedaConversion.Codigo == "ARS");
            Assert.AreEqual(EstadoActualizacionTasa.SaltadoSinDato, saltado.Estado);
            Assert.AreEqual(10m, saltado.ValorAnterior);
            Assert.IsNull(saltado.ValorNuevo);
        }

        [TestMethod]
        public async Task EjecutarAsync_CotizacionNoPositiva_SaltaElPar()
        {
            ActualizacionFixture fixture = new ActualizacionFixture();
            fixture.AgregarTasa(1, "USD", "ARS", 10m);
            fixture.AgregarTasa(2, "USD", "EUR", 20m);
            fixture.Proveedor.Resultado = fixture.Respuesta(R("ARS", 0m), R("EUR", -1m));

            ResultadoEjecucionActualizacionTasas resultado = await fixture.CreateSut().EjecutarAsync(OrigenActualizacionTasas.Programada, CancellationToken.None);

            Assert.AreEqual(10m, fixture.TasaRepo.Tasas[1].CalculoConversion);
            Assert.AreEqual(20m, fixture.TasaRepo.Tasas[2].CalculoConversion);
            Assert.IsTrue(resultado.Ejecucion.Detalles.All(d => d.Estado == EstadoActualizacionTasa.SaltadoSinDato));
        }

        [TestMethod]
        public async Task EjecutarAsync_BaseNoPositivaEnPar_SaltaElPar()
        {
            ActualizacionFixture fixture = new ActualizacionFixture();
            fixture.AgregarTasa(1, "ARS", "EUR", 5m);
            fixture.Proveedor.Resultado = fixture.Respuesta(R("ARS", 0m), R("EUR", 0.9m));

            ResultadoEjecucionActualizacionTasas resultado = await fixture.CreateSut().EjecutarAsync(OrigenActualizacionTasas.Programada, CancellationToken.None);

            Assert.AreEqual(5m, fixture.TasaRepo.Tasas[1].CalculoConversion);
            Assert.AreEqual(EstadoActualizacionTasa.SaltadoSinDato, resultado.Ejecucion.Detalles.Single().Estado);
        }

        [TestMethod]
        public async Task EjecutarAsync_ValorQueRedondeaACero_SaltaElPar()
        {
            ActualizacionFixture fixture = new ActualizacionFixture();
            fixture.AgregarTasa(1, "ARS", "EUR", 5m);
            fixture.Proveedor.Resultado = fixture.Respuesta(R("ARS", 10000000m), R("EUR", 1m));

            ResultadoEjecucionActualizacionTasas resultado = await fixture.CreateSut().EjecutarAsync(OrigenActualizacionTasas.Programada, CancellationToken.None);

            Assert.AreEqual(5m, fixture.TasaRepo.Tasas[1].CalculoConversion);
            Assert.AreEqual(EstadoActualizacionTasa.SaltadoSinDato, resultado.Ejecucion.Detalles.Single().Estado);
        }

        [TestMethod]
        public async Task EjecutarAsync_ParQueSeVolvioManualEnLaEscritura_QuedaSaltadoManual()
        {
            ActualizacionFixture fixture = new ActualizacionFixture();
            TasaConversion tasa = fixture.AgregarTasa(1, "USD", "ARS", 10m);
            fixture.TasaRepo.OnActualizarValorAutomatico = () => tasa.EsManual = true;
            fixture.Proveedor.Resultado = fixture.Respuesta(R("ARS", 1500m));

            ResultadoEjecucionActualizacionTasas resultado = await fixture.CreateSut().EjecutarAsync(OrigenActualizacionTasas.Programada, CancellationToken.None);

            Assert.AreEqual(10m, tasa.CalculoConversion);
            Assert.AreEqual(EstadoActualizacionTasa.SaltadoManual, resultado.Ejecucion.Detalles.Single().Estado);
        }

        [TestMethod]
        public async Task EjecutarAsync_MonedaSinPar_NoCreaPares()
        {
            ActualizacionFixture fixture = new ActualizacionFixture();
            fixture.AgregarTasa(1, "USD", "ARS", 10m);
            fixture.Proveedor.Resultado = fixture.Respuesta(R("ARS", 1500m), R("EUR", 0.9m), R("BRL", 5m));

            await fixture.CreateSut().EjecutarAsync(OrigenActualizacionTasas.Programada, CancellationToken.None);

            Assert.AreEqual(1, fixture.TasaRepo.Tasas.Count);
            Assert.IsNull(fixture.TasaRepo.LastInserted);
        }

        [TestMethod]
        public async Task EjecutarAsync_Exitosa_RegistraEjecucionConsumoOrigenYDetalle()
        {
            ActualizacionFixture fixture = new ActualizacionFixture();
            fixture.AgregarTasa(1, "USD", "ARS", 10m);
            fixture.Proveedor.Resultado = fixture.Respuesta(R("ARS", 1500m));

            ResultadoEjecucionActualizacionTasas resultado = await fixture.CreateSut().EjecutarAsync(OrigenActualizacionTasas.Manual, CancellationToken.None);

            EjecucionActualizacionTasas guardada = fixture.EjecucionRepo.Ejecuciones.Single();
            Assert.AreSame(guardada, resultado.Ejecucion);
            Assert.AreEqual(Ahora, guardada.FechaInicio);
            Assert.AreEqual(OrigenActualizacionTasas.Manual, guardada.Origen);
            Assert.AreEqual(ResultadoActualizacionTasas.Exitosa, guardada.Resultado);
            Assert.IsTrue(guardada.ConsumioRequest);
            ActualizacionTasaDetalle detalle = fixture.EjecucionRepo.Detalles.Single();
            Assert.AreEqual(guardada.Id, detalle.EjecucionId);
            Assert.AreEqual(10m, detalle.ValorAnterior);
            Assert.AreEqual(1500m, detalle.ValorNuevo);
            Assert.AreEqual(EstadoActualizacionTasa.Actualizado, detalle.Estado);
            Assert.IsTrue(fixture.Unidad.SaveCalled);
            Assert.IsNull(resultado.ProximaConsultaPermitida);
        }

        [TestMethod]
        public async Task EjecutarAsync_ChequeaTopeDentroDeLaMismaUnidadConBloqueo()
        {
            ActualizacionFixture fixture = new ActualizacionFixture();
            fixture.AgregarTasa(1, "USD", "ARS", 10m);
            fixture.Proveedor.Resultado = fixture.Respuesta(R("ARS", 1500m));

            await fixture.CreateSut().EjecutarAsync(OrigenActualizacionTasas.Programada, CancellationToken.None);

            Assert.AreEqual(1, fixture.UnidadFactory.CrearCallCount);
            Assert.AreEqual(1, fixture.EjecucionRepo.ObtenerUltimaExitosaConBloqueoCalls);
        }

        [TestMethod]
        public async Task EjecutarAsync_UltimoExitoHaceTreintaMinutos_OmiteSinConsultarNiModificar()
        {
            ActualizacionFixture fixture = new ActualizacionFixture();
            fixture.AgregarTasa(1, "USD", "ARS", 10m);
            fixture.AgregarExito(Ahora.AddMinutes(-30));
            fixture.Proveedor.Resultado = fixture.Respuesta(R("ARS", 1500m));

            ResultadoEjecucionActualizacionTasas resultado = await fixture.CreateSut().EjecutarAsync(OrigenActualizacionTasas.Programada, CancellationToken.None);

            Assert.AreEqual(0, fixture.Proveedor.Llamadas);
            Assert.AreEqual(10m, fixture.TasaRepo.Tasas[1].CalculoConversion);
            Assert.AreEqual(ResultadoActualizacionTasas.Omitida, resultado.Ejecucion.Resultado);
            Assert.IsFalse(resultado.Ejecucion.ConsumioRequest);
            Assert.AreEqual(Ahora.AddMinutes(30), resultado.ProximaConsultaPermitida);
            Assert.AreEqual(0, fixture.EjecucionRepo.Detalles.Count);
            Assert.AreEqual(2, fixture.EjecucionRepo.Ejecuciones.Count);
            Assert.IsTrue(fixture.Unidad.SaveCalled);
        }

        [TestMethod]
        public async Task EjecutarAsync_OmitidaPorTope_RegistraOrigenYMensaje()
        {
            ActualizacionFixture fixture = new ActualizacionFixture();
            fixture.AgregarExito(Ahora.AddMinutes(-5));

            ResultadoEjecucionActualizacionTasas resultado = await fixture.CreateSut().EjecutarAsync(OrigenActualizacionTasas.Manual, CancellationToken.None);

            Assert.AreEqual(OrigenActualizacionTasas.Manual, resultado.Ejecucion.Origen);
            Assert.AreEqual(Ahora, resultado.Ejecucion.FechaInicio);
            Assert.AreEqual("Info.TasasCambio.OmitidaPorTope", resultado.Ejecucion.Mensaje);
        }

        [TestMethod]
        public async Task EjecutarAsync_UltimoExitoHaceExactamenteSesentaMinutos_Consulta()
        {
            ActualizacionFixture fixture = new ActualizacionFixture();
            fixture.AgregarTasa(1, "USD", "ARS", 10m);
            fixture.AgregarExito(Ahora.AddMinutes(-60));
            fixture.Proveedor.Resultado = fixture.Respuesta(R("ARS", 1500m));

            ResultadoEjecucionActualizacionTasas resultado = await fixture.CreateSut().EjecutarAsync(OrigenActualizacionTasas.Programada, CancellationToken.None);

            Assert.AreEqual(1, fixture.Proveedor.Llamadas);
            Assert.AreEqual(ResultadoActualizacionTasas.Exitosa, resultado.Ejecucion.Resultado);
        }

        [TestMethod]
        public async Task EjecutarAsync_UltimoExitoHaceMenosDeSesentaPorUnSegundo_Omite()
        {
            ActualizacionFixture fixture = new ActualizacionFixture();
            fixture.AgregarExito(Ahora.AddMinutes(-60).AddSeconds(1));

            ResultadoEjecucionActualizacionTasas resultado = await fixture.CreateSut().EjecutarAsync(OrigenActualizacionTasas.Programada, CancellationToken.None);

            Assert.AreEqual(0, fixture.Proveedor.Llamadas);
            Assert.AreEqual(ResultadoActualizacionTasas.Omitida, resultado.Ejecucion.Resultado);
        }

        [TestMethod]
        public async Task EjecutarAsync_MideDesdeElInicioDelUltimoExito_NoDesdeSuFin()
        {
            ActualizacionFixture fixture = new ActualizacionFixture();
            fixture.AgregarTasa(1, "USD", "ARS", 10m);
            // La corrida anterior inicio a las 11:00:00 y termino segundos despues; la tarea horaria arranca 12:00:00.
            fixture.AgregarExito(Ahora.AddHours(-1));
            fixture.Proveedor.Resultado = fixture.Respuesta(R("ARS", 1500m));

            ResultadoEjecucionActualizacionTasas resultado = await fixture.CreateSut().EjecutarAsync(OrigenActualizacionTasas.Programada, CancellationToken.None);

            Assert.AreEqual(ResultadoActualizacionTasas.Exitosa, resultado.Ejecucion.Resultado);
        }

        [TestMethod]
        public async Task EjecutarAsync_UltimaEjecucionFallida_NoCuentaParaElTope()
        {
            ActualizacionFixture fixture = new ActualizacionFixture();
            fixture.AgregarTasa(1, "USD", "ARS", 10m);
            fixture.EjecucionRepo.InsertarYObtenerId(new EjecucionActualizacionTasas
            {
                FechaInicio = Ahora.AddMinutes(-10),
                Resultado = ResultadoActualizacionTasas.Fallida,
                ConsumioRequest = true
            });
            fixture.EjecucionRepo.InsertarYObtenerId(new EjecucionActualizacionTasas
            {
                FechaInicio = Ahora.AddMinutes(-5),
                Resultado = ResultadoActualizacionTasas.Omitida
            });
            fixture.Proveedor.Resultado = fixture.Respuesta(R("ARS", 1500m));

            ResultadoEjecucionActualizacionTasas resultado = await fixture.CreateSut().EjecutarAsync(OrigenActualizacionTasas.Programada, CancellationToken.None);

            Assert.AreEqual(1, fixture.Proveedor.Llamadas);
            Assert.AreEqual(ResultadoActualizacionTasas.Exitosa, resultado.Ejecucion.Resultado);
        }

        [TestMethod]
        public async Task EjecutarAsync_ProveedorRechaza_RegistraFallaConConsumoYConservaValores()
        {
            ActualizacionFixture fixture = new ActualizacionFixture();
            fixture.AgregarTasa(1, "USD", "ARS", 10m);
            fixture.Proveedor.Excepcion = new ProveedorTasasException(EstadoProveedorTasas.Rechazado, 500, null);

            ResultadoEjecucionActualizacionTasas resultado = await fixture.CreateSut().EjecutarAsync(OrigenActualizacionTasas.Programada, CancellationToken.None);

            Assert.AreEqual(1, fixture.Proveedor.Llamadas);
            Assert.AreEqual(ResultadoActualizacionTasas.Fallida, resultado.Ejecucion.Resultado);
            Assert.IsTrue(resultado.Ejecucion.ConsumioRequest);
            Assert.AreEqual("Error.TasasCambio.Provider.Rechazado", resultado.Ejecucion.Mensaje);
            Assert.AreEqual(10m, fixture.TasaRepo.Tasas[1].CalculoConversion);
            Assert.AreEqual(0, fixture.EjecucionRepo.Detalles.Count);
            Assert.IsNull(resultado.ProximaConsultaPermitida);
            Assert.IsTrue(fixture.Unidad.SaveCalled);
        }

        [TestMethod]
        [DataRow(EstadoProveedorTasas.ClaveInvalida)]
        [DataRow(EstadoProveedorTasas.LimiteExcedido)]
        [DataRow(EstadoProveedorTasas.ConexionFallida)]
        [DataRow(EstadoProveedorTasas.RespuestaInvalida)]
        public async Task EjecutarAsync_ProveedorFallaTrasConsultar_CuentaComoRequestConsumida(EstadoProveedorTasas estado)
        {
            ActualizacionFixture fixture = new ActualizacionFixture();
            fixture.Proveedor.Excepcion = new ProveedorTasasException(estado);

            ResultadoEjecucionActualizacionTasas resultado = await fixture.CreateSut().EjecutarAsync(OrigenActualizacionTasas.Programada, CancellationToken.None);

            Assert.AreEqual(ResultadoActualizacionTasas.Fallida, resultado.Ejecucion.Resultado);
            Assert.IsTrue(resultado.Ejecucion.ConsumioRequest);
            Assert.AreEqual("Error.TasasCambio.Provider." + estado, resultado.Ejecucion.Mensaje);
        }

        [TestMethod]
        public async Task EjecutarAsync_ClaveFaltante_RegistraFallaSinConsumo()
        {
            ActualizacionFixture fixture = new ActualizacionFixture();
            fixture.Proveedor.Excepcion = new ProveedorTasasException(EstadoProveedorTasas.ClaveFaltante);

            ResultadoEjecucionActualizacionTasas resultado = await fixture.CreateSut().EjecutarAsync(OrigenActualizacionTasas.Programada, CancellationToken.None);

            Assert.AreEqual(ResultadoActualizacionTasas.Fallida, resultado.Ejecucion.Resultado);
            Assert.IsFalse(resultado.Ejecucion.ConsumioRequest);
            Assert.AreEqual("Error.TasasCambio.Provider.ClaveFaltante", resultado.Ejecucion.Mensaje);
        }

        [TestMethod]
        public async Task EjecutarAsync_ErrorInesperadoDelProveedor_RegistraFallaConConsumoSinPropagar()
        {
            ActualizacionFixture fixture = new ActualizacionFixture();
            fixture.Proveedor.Excepcion = new InvalidOperationException("boom");

            ResultadoEjecucionActualizacionTasas resultado = await fixture.CreateSut().EjecutarAsync(OrigenActualizacionTasas.Programada, CancellationToken.None);

            Assert.AreEqual(ResultadoActualizacionTasas.Fallida, resultado.Ejecucion.Resultado);
            Assert.IsTrue(resultado.Ejecucion.ConsumioRequest);
            Assert.AreEqual("Error.TasasCambio.Unexpected", resultado.Ejecucion.Mensaje);
        }

        [TestMethod]
        public async Task EjecutarAsync_Cancelada_RegistraFallaConConsumoYPropagaLaCancelacion()
        {
            ActualizacionFixture fixture = new ActualizacionFixture();
            fixture.Proveedor.Excepcion = new OperationCanceledException();
            ActualizacionTasasBLL sut = fixture.CreateSut();

            await Assert.ThrowsExactlyAsync<OperationCanceledException>(() => sut.EjecutarAsync(OrigenActualizacionTasas.Manual, CancellationToken.None));

            EjecucionActualizacionTasas guardada = fixture.EjecucionRepo.Ejecuciones.Single();
            Assert.AreEqual(ResultadoActualizacionTasas.Fallida, guardada.Resultado);
            Assert.IsTrue(guardada.ConsumioRequest);
            Assert.AreEqual("Error.TasasCambio.Cancelled", guardada.Mensaje);
            Assert.IsTrue(fixture.Unidad.SaveCalled);
        }

        [TestMethod]
        public async Task EjecutarAsync_PasaElTokenDeCancelacionAlProveedor()
        {
            ActualizacionFixture fixture = new ActualizacionFixture();
            fixture.Proveedor.Resultado = fixture.Respuesta();
            using (CancellationTokenSource cts = new CancellationTokenSource())
            {
                await fixture.CreateSut().EjecutarAsync(OrigenActualizacionTasas.Programada, cts.Token);

                Assert.AreEqual(cts.Token, fixture.Proveedor.UltimoToken);
            }
        }

        [TestMethod]
        public async Task EjecutarAsync_SinPares_RegistraExitosaSinDetalles()
        {
            ActualizacionFixture fixture = new ActualizacionFixture();
            fixture.Proveedor.Resultado = fixture.Respuesta(R("ARS", 1500m));

            ResultadoEjecucionActualizacionTasas resultado = await fixture.CreateSut().EjecutarAsync(OrigenActualizacionTasas.Programada, CancellationToken.None);

            Assert.AreEqual(ResultadoActualizacionTasas.Exitosa, resultado.Ejecucion.Resultado);
            Assert.AreEqual(0, resultado.Ejecucion.Detalles.Count);
            Assert.IsTrue(resultado.Ejecucion.ConsumioRequest);
        }

        [TestMethod]
        public async Task EjecutarAsync_FalloDeEscrituraDeUnPar_NoGuardaYLanzaExcepcionTecnica()
        {
            ActualizacionFixture fixture = new ActualizacionFixture();
            fixture.AgregarTasa(1, "USD", "ARS", 10m);
            fixture.TasaRepo.OnActualizarValorAutomatico = () => { throw new InvalidOperationException("db"); };
            fixture.Proveedor.Resultado = fixture.Respuesta(R("ARS", 1500m));
            ActualizacionTasasBLL sut = fixture.CreateSut();

            await Assert.ThrowsExactlyAsync<SERVICIOS.ExcepcionTecnica>(() => sut.EjecutarAsync(OrigenActualizacionTasas.Programada, CancellationToken.None));

            Assert.IsFalse(fixture.Unidad.SaveCalled);
        }

        [TestMethod]
        public void ObtenerUltimasEjecuciones_VariasEjecuciones_DevuelveLaMasNuevaPrimeroYRespetaLaCantidad()
        {
            ActualizacionFixture fixture = new ActualizacionFixture();
            fixture.AgregarExito(Ahora.AddHours(-3));
            fixture.AgregarExito(Ahora.AddHours(-1));
            fixture.AgregarExito(Ahora.AddHours(-2));

            List<EjecucionActualizacionTasas> ejecuciones = fixture.CreateSut().ObtenerUltimasEjecuciones(2);

            Assert.AreEqual(2, ejecuciones.Count);
            Assert.AreEqual(Ahora.AddHours(-1), ejecuciones[0].FechaInicio);
            Assert.AreEqual(Ahora.AddHours(-2), ejecuciones[1].FechaInicio);
        }

        [TestMethod]
        public void ObtenerUltimasEjecuciones_SinHistorial_DevuelveListaVacia()
        {
            ActualizacionFixture fixture = new ActualizacionFixture();

            List<EjecucionActualizacionTasas> ejecuciones = fixture.CreateSut().ObtenerUltimasEjecuciones(10);

            Assert.AreEqual(0, ejecuciones.Count);
        }

        [TestMethod]
        public void ObtenerDetalleDeEjecucion_EjecucionConPares_DevuelveSoloLosParesDeEsaEjecucion()
        {
            ActualizacionFixture fixture = new ActualizacionFixture();
            fixture.AgregarExito(Ahora.AddHours(-2));
            fixture.AgregarExito(Ahora.AddHours(-1));
            fixture.EjecucionRepo.InsertarDetalleYObtenerId(Detalle(1, EstadoActualizacionTasa.Actualizado));
            fixture.EjecucionRepo.InsertarDetalleYObtenerId(Detalle(1, EstadoActualizacionTasa.SaltadoManual));
            fixture.EjecucionRepo.InsertarDetalleYObtenerId(Detalle(2, EstadoActualizacionTasa.Actualizado));

            List<ActualizacionTasaDetalle> detalle = fixture.CreateSut().ObtenerDetalleDeEjecucion(1);

            Assert.AreEqual(2, detalle.Count);
            Assert.IsTrue(detalle.All(d => d.EjecucionId == 1));
        }

        [TestMethod]
        public void ObtenerRequestsConsumidasDelMes_EjecucionesDeVariosMeses_CuentaSoloLasConsumidasDelMesActual()
        {
            ActualizacionFixture fixture = new ActualizacionFixture();
            fixture.AgregarExito(Ahora.AddDays(-30));
            fixture.AgregarExito(Ahora.AddHours(-5));
            fixture.EjecucionRepo.InsertarYObtenerId(new EjecucionActualizacionTasas
            {
                FechaInicio = Ahora.AddHours(-4),
                Resultado = ResultadoActualizacionTasas.Fallida,
                ConsumioRequest = true
            });
            fixture.EjecucionRepo.InsertarYObtenerId(new EjecucionActualizacionTasas
            {
                FechaInicio = Ahora.AddHours(-3),
                Resultado = ResultadoActualizacionTasas.Omitida,
                ConsumioRequest = false
            });

            int consumidas = fixture.CreateSut().ObtenerRequestsConsumidasDelMes();

            Assert.AreEqual(2, consumidas);
        }

        [TestMethod]
        public void LimiteMensualDeRequests_PlanGratuito_EsMil()
        {
            Assert.AreEqual(1000, ActualizacionTasasBLL.LimiteMensualDeRequests);
        }

        private static ActualizacionTasaDetalle Detalle(int ejecucionId, EstadoActualizacionTasa estado)
        {
            return new ActualizacionTasaDetalle { EjecucionId = ejecucionId, ValorAnterior = 1m, ValorNuevo = 2m, Estado = estado };
        }

        private static KeyValuePair<string, decimal> R(string codigo, decimal valor)
        {
            return new KeyValuePair<string, decimal>(codigo, valor);
        }

        private sealed class ActualizacionFixture
        {
            public readonly FakeTasaConversionRepositorio TasaRepo = new FakeTasaConversionRepositorio();
            public readonly FakeEjecucionActualizacionTasasRepositorio EjecucionRepo = new FakeEjecucionActualizacionTasasRepositorio();
            public readonly FakeProveedorTasasCambio Proveedor = new FakeProveedorTasasCambio();
            public readonly FakeUnidadDeTrabajo Unidad;
            public readonly FakeUnidadDeTrabajoFactory UnidadFactory;

            private int _nextMonedaId = 1;
            private readonly Dictionary<string, Moneda> _monedas = new Dictionary<string, Moneda>(StringComparer.OrdinalIgnoreCase);

            public ActualizacionFixture()
            {
                Unidad = new FakeUnidadDeTrabajo { TasaConversionRepo = TasaRepo, EjecucionActualizacionTasasRepo = EjecucionRepo };
                UnidadFactory = new FakeUnidadDeTrabajoFactory(Unidad);
            }

            public ActualizacionTasasBLL CreateSut()
            {
                return new ActualizacionTasasBLL(UnidadFactory, Proveedor, () => Ahora);
            }

            public TasaConversion AgregarTasa(int id, string codigoBase, string codigoConversion, decimal valor)
            {
                TasaConversion tasa = new TasaConversion
                {
                    Id = id,
                    MonedaBase = ObtenerMoneda(codigoBase),
                    MonedaConversion = ObtenerMoneda(codigoConversion),
                    CalculoConversion = valor,
                    FechaActualizacion = Ahora.AddDays(-1)
                };
                TasaRepo.Tasas[id] = tasa;
                return tasa;
            }

            public void AgregarExito(DateTime inicio)
            {
                EjecucionRepo.InsertarYObtenerId(new EjecucionActualizacionTasas
                {
                    FechaInicio = inicio,
                    Resultado = ResultadoActualizacionTasas.Exitosa,
                    ConsumioRequest = true
                });
            }

            public TasasCambioObtenidas Respuesta(params KeyValuePair<string, decimal>[] tasas)
            {
                Dictionary<string, decimal> diccionario = tasas.ToDictionary(t => t.Key, t => t.Value);
                return new TasasCambioObtenidas("USD", Ahora, diccionario);
            }

            private Moneda ObtenerMoneda(string codigo)
            {
                Moneda moneda;
                if (!_monedas.TryGetValue(codigo, out moneda))
                {
                    moneda = new Moneda { Id = _nextMonedaId++, Codigo = codigo, Nombre = codigo };
                    _monedas[codigo] = moneda;
                }

                return moneda;
            }
        }

        private sealed class FakeProveedorTasasCambio : IProveedorTasasCambio
        {
            public TasasCambioObtenidas Resultado { get; set; }

            public Exception Excepcion { get; set; }

            public int Llamadas { get; private set; }

            public CancellationToken UltimoToken { get; private set; }

            public Task<TasasCambioObtenidas> ObtenerTasasAsync(CancellationToken cancellationToken)
            {
                Llamadas++;
                UltimoToken = cancellationToken;
                if (Excepcion != null)
                {
                    throw Excepcion;
                }

                return Task.FromResult(Resultado);
            }
        }
    }
}
