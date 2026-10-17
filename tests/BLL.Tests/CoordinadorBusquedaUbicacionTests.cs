using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ABSTRACCIONES;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TpIDS;

namespace BLL.Tests
{
    // Ciclo de vida de la consulta del selector de ubicacion (version, cancelacion, descarte de
    // respuestas obsoletas, doble envio) con un servicio falso: sin red, sin formulario ni reloj real.
    [TestClass]
    public sealed class CoordinadorBusquedaUbicacionTests
    {
        private static ConsultaUbicacion Consulta(string texto = "Obelisco")
        {
            return new ConsultaUbicacion(texto, "es", null);
        }

        private static ResultadoBusquedaUbicacion Lugar(string descripcion)
        {
            return new ResultadoBusquedaUbicacion(descripcion, -34.6, -58.38, "attraction", null);
        }

        [TestMethod]
        public async Task BuscarAsync_VariasCoincidencias_LasDevuelveYRestauraElEstado()
        {
            var servicio = new ServicioFalso { Respuesta = new[] { Lugar("A"), Lugar("B"), Lugar("C") } };
            var coordinador = new CoordinadorBusquedaUbicacion(servicio);

            ResultadoCoordinado resultado = await coordinador.BuscarAsync(Consulta());

            Assert.AreEqual(TipoResultadoBusqueda.Coincidencias, resultado.Tipo);
            Assert.AreEqual(3, resultado.Coincidencias.Count);
            Assert.AreEqual(1, servicio.Llamadas.Count);
            Assert.AreEqual("Obelisco", servicio.Llamadas[0].Texto);
            Assert.IsFalse(coordinador.EnCurso);
        }

        [TestMethod]
        public async Task BuscarAsync_CoincidenciaUnica_LaDevuelveSinElegirla()
        {
            var coordinador = new CoordinadorBusquedaUbicacion(new ServicioFalso { Respuesta = new[] { Lugar("Unico") } });

            ResultadoCoordinado resultado = await coordinador.BuscarAsync(Consulta());

            Assert.AreEqual(TipoResultadoBusqueda.Coincidencias, resultado.Tipo);
            Assert.AreEqual(1, resultado.Coincidencias.Count);
        }

        [TestMethod]
        public async Task BuscarAsync_ListaVacia_InformaSinCoincidencias()
        {
            var coordinador = new CoordinadorBusquedaUbicacion(new ServicioFalso { Respuesta = new ResultadoBusquedaUbicacion[0] });

            ResultadoCoordinado resultado = await coordinador.BuscarAsync(Consulta());

            Assert.AreEqual(TipoResultadoBusqueda.SinCoincidencias, resultado.Tipo);
            Assert.IsFalse(coordinador.EnCurso);
        }

        [TestMethod]
        [DataRow(EstadoBusquedaUbicacion.ConexionFallida)]
        [DataRow(EstadoBusquedaUbicacion.TiempoAgotado)]
        [DataRow(EstadoBusquedaUbicacion.LimiteExcedido)]
        [DataRow(EstadoBusquedaUbicacion.RespuestaInvalida)]
        [DataRow(EstadoBusquedaUbicacion.ConfiguracionInvalida)]
        public async Task BuscarAsync_ErrorTipado_ConservaElEstadoYRestauraLaDisponibilidad(EstadoBusquedaUbicacion estado)
        {
            var coordinador = new CoordinadorBusquedaUbicacion(new ServicioFalso { Error = new BusquedaUbicacionException(estado) });

            ResultadoCoordinado resultado = await coordinador.BuscarAsync(Consulta());

            Assert.AreEqual(TipoResultadoBusqueda.Error, resultado.Tipo);
            Assert.AreEqual(estado, resultado.Estado);
            Assert.IsFalse(coordinador.EnCurso);
        }

        [TestMethod]
        public async Task BuscarAsync_FalloInesperado_SeInformaComoConexionFallida()
        {
            var coordinador = new CoordinadorBusquedaUbicacion(new ServicioFalso { Error = new InvalidOperationException() });

            ResultadoCoordinado resultado = await coordinador.BuscarAsync(Consulta());

            Assert.AreEqual(TipoResultadoBusqueda.Error, resultado.Tipo);
            Assert.AreEqual(EstadoBusquedaUbicacion.ConexionFallida, resultado.Estado);
            Assert.IsFalse(coordinador.EnCurso);
        }

        [TestMethod]
        public async Task BuscarAsync_ServicioCancelaPorSuCuenta_NoEsErrorYRestauraElEstado()
        {
            var coordinador = new CoordinadorBusquedaUbicacion(new ServicioFalso { Error = new OperationCanceledException() });

            ResultadoCoordinado resultado = await coordinador.BuscarAsync(Consulta());

            Assert.AreEqual(TipoResultadoBusqueda.Cancelada, resultado.Tipo);
            Assert.IsFalse(coordinador.EnCurso);
        }

        [TestMethod]
        public async Task BuscarAsync_DobleEnvioEnCurso_NoIniciaOtraPeticion()
        {
            var servicio = new ServicioFalso { Pendiente = new TaskCompletionSource<IReadOnlyList<ResultadoBusquedaUbicacion>>() };
            var coordinador = new CoordinadorBusquedaUbicacion(servicio);

            Task<ResultadoCoordinado> primera = coordinador.BuscarAsync(Consulta());
            Assert.IsTrue(coordinador.EnCurso);
            ResultadoCoordinado segunda = await coordinador.BuscarAsync(Consulta());

            Assert.AreEqual(TipoResultadoBusqueda.Ignorada, segunda.Tipo);
            Assert.AreEqual(1, servicio.Llamadas.Count);

            servicio.Pendiente.SetResult(new[] { Lugar("A") });
            Assert.AreEqual(TipoResultadoBusqueda.Coincidencias, (await primera).Tipo);
            Assert.IsFalse(coordinador.EnCurso);
        }

        [TestMethod]
        public async Task Invalidar_ConsultaPendienteQueObservaElToken_CancelaYDescartaYLiberaBuscar()
        {
            var servicio = new ServicioFalso { Pendiente = new TaskCompletionSource<IReadOnlyList<ResultadoBusquedaUbicacion>>(), ObservaToken = true };
            var coordinador = new CoordinadorBusquedaUbicacion(servicio);

            Task<ResultadoCoordinado> pendiente = coordinador.BuscarAsync(Consulta());
            coordinador.Invalidar();

            Assert.IsFalse(coordinador.EnCurso, "Buscar debe quedar disponible apenas se invalida.");
            Assert.IsTrue(servicio.Tokens[0].IsCancellationRequested);
            Assert.AreEqual(TipoResultadoBusqueda.Descartada, (await pendiente).Tipo);
        }

        [TestMethod]
        public async Task Invalidar_RespuestaTardiaQueIgnoraElToken_SeDescartaYNoPisaLaConsultaNueva()
        {
            var primera = new TaskCompletionSource<IReadOnlyList<ResultadoBusquedaUbicacion>>();
            var segunda = new TaskCompletionSource<IReadOnlyList<ResultadoBusquedaUbicacion>>();
            var servicio = new ServicioFalso { Pendientes = new Queue<TaskCompletionSource<IReadOnlyList<ResultadoBusquedaUbicacion>>>(new[] { primera, segunda }) };
            var coordinador = new CoordinadorBusquedaUbicacion(servicio);

            Task<ResultadoCoordinado> viejaTarea = coordinador.BuscarAsync(Consulta("viejo"));
            coordinador.Invalidar();
            Task<ResultadoCoordinado> nuevaTarea = coordinador.BuscarAsync(Consulta("nuevo"));
            Assert.IsTrue(coordinador.EnCurso);

            primera.SetResult(new[] { Lugar("del texto viejo") });
            Assert.AreEqual(TipoResultadoBusqueda.Descartada, (await viejaTarea).Tipo);
            Assert.IsTrue(coordinador.EnCurso, "La respuesta vieja no debe restaurar el estado de la consulta vigente.");

            segunda.SetResult(new[] { Lugar("del texto nuevo") });
            ResultadoCoordinado nueva = await nuevaTarea;
            Assert.AreEqual(TipoResultadoBusqueda.Coincidencias, nueva.Tipo);
            Assert.AreEqual("del texto nuevo", nueva.Coincidencias[0].Descripcion);
            Assert.IsFalse(coordinador.EnCurso);
        }

        [TestMethod]
        public async Task Invalidar_ErrorTardioDeConsultaObsoleta_SeDescarta()
        {
            var servicio = new ServicioFalso { Pendiente = new TaskCompletionSource<IReadOnlyList<ResultadoBusquedaUbicacion>>() };
            var coordinador = new CoordinadorBusquedaUbicacion(servicio);

            Task<ResultadoCoordinado> pendiente = coordinador.BuscarAsync(Consulta());
            coordinador.Invalidar();
            servicio.Pendiente.SetException(new BusquedaUbicacionException(EstadoBusquedaUbicacion.TiempoAgotado));

            Assert.AreEqual(TipoResultadoBusqueda.Descartada, (await pendiente).Tipo);
        }

        [TestMethod]
        public async Task Detener_ConsultaPendiente_DescartaLaRespuestaTardiaYRechazaNuevasConsultas()
        {
            var servicio = new ServicioFalso { Pendiente = new TaskCompletionSource<IReadOnlyList<ResultadoBusquedaUbicacion>>() };
            var coordinador = new CoordinadorBusquedaUbicacion(servicio);

            Task<ResultadoCoordinado> pendiente = coordinador.BuscarAsync(Consulta());
            coordinador.Detener();
            servicio.Pendiente.SetResult(new[] { Lugar("tardio") });

            Assert.AreEqual(TipoResultadoBusqueda.Descartada, (await pendiente).Tipo);
            Assert.AreEqual(TipoResultadoBusqueda.Ignorada, (await coordinador.BuscarAsync(Consulta())).Tipo);
            Assert.AreEqual(1, servicio.Llamadas.Count);
        }

        [TestMethod]
        public async Task BuscarAsync_SinServicio_NoHaceNada()
        {
            var coordinador = new CoordinadorBusquedaUbicacion(null);

            Assert.AreEqual(TipoResultadoBusqueda.Ignorada, (await coordinador.BuscarAsync(Consulta())).Tipo);
            Assert.IsFalse(coordinador.EnCurso);
        }

        [TestMethod]
        public async Task BuscarAsync_ConsultaTerminada_PermiteUnaNuevaBusqueda()
        {
            var servicio = new ServicioFalso { Respuesta = new[] { Lugar("A") } };
            var coordinador = new CoordinadorBusquedaUbicacion(servicio);

            await coordinador.BuscarAsync(Consulta("uno"));
            ResultadoCoordinado segunda = await coordinador.BuscarAsync(Consulta("dos"));

            Assert.AreEqual(TipoResultadoBusqueda.Coincidencias, segunda.Tipo);
            Assert.AreEqual(2, servicio.Llamadas.Count);
        }

        private sealed class ServicioFalso : IBusquedaUbicacionesService
        {
            public IReadOnlyList<ResultadoBusquedaUbicacion> Respuesta { get; set; }

            public Exception Error { get; set; }

            // Una sola respuesta manual, o una cola cuando se necesitan varias consultas controladas.
            public TaskCompletionSource<IReadOnlyList<ResultadoBusquedaUbicacion>> Pendiente { get; set; }

            public Queue<TaskCompletionSource<IReadOnlyList<ResultadoBusquedaUbicacion>>> Pendientes { get; set; }

            // Si es true, cancelar el token completa la tarea como cancelada (transporte que lo observa).
            public bool ObservaToken { get; set; }

            public List<ConsultaUbicacion> Llamadas { get; } = new List<ConsultaUbicacion>();

            public List<CancellationToken> Tokens { get; } = new List<CancellationToken>();

            public Task<IReadOnlyList<ResultadoBusquedaUbicacion>> BuscarAsync(ConsultaUbicacion consulta, CancellationToken cancellationToken)
            {
                Llamadas.Add(consulta);
                Tokens.Add(cancellationToken);

                TaskCompletionSource<IReadOnlyList<ResultadoBusquedaUbicacion>> manual = Pendientes != null ? Pendientes.Dequeue() : Pendiente;
                if (manual != null)
                {
                    if (ObservaToken)
                    {
                        cancellationToken.Register(() => manual.TrySetCanceled(cancellationToken));
                    }

                    return manual.Task;
                }

                if (Error != null)
                {
                    return Task.FromException<IReadOnlyList<ResultadoBusquedaUbicacion>>(Error);
                }

                return Task.FromResult(Respuesta);
            }
        }
    }
}
