using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ABSTRACCIONES;
using BE;
using REPOSITORIO;
using SERVICIOS;

namespace BLL
{
    // Resumen de una corrida del proceso automatico: lo que informa el ejecutable programado.
    public sealed class ResultadoCorridaNotificaciones
    {
        public int CorridaId { get; set; }

        public DateTime Jornada { get; set; }

        // La corrida arranco antes de las 07:00: no se genera nada ni se registra una corrida.
        public bool AntesDelHorario { get; set; }

        // Se pidio cancelar: no se iniciaron mas unidades de trabajo.
        public bool Cancelada { get; set; }

        public int Entregadas { get; set; }

        public int Omitidas { get; set; }

        public int Fallidas { get; set; }
    }

    // Proceso automatico diario de notificaciones. No usa sesion: recorre las cuentas activas por
    // pagina y, para cada una, evalua tres tipos (movimientos, compras, inversiones) con resultado
    // independiente. El calculo (SQL de lectura, fuentes externas, IA) corre sin transaccion; solo la
    // entrega abre una transaccion corta que bloquea la fila del usuario, revalida y guarda el aviso
    // junto con su detalle tecnico. Los detalles guardan solo codigos de motivo estables: nunca
    // mensajes de excepcion, prompts ni secretos.
    public sealed class GeneracionNotificacionesBLL
    {
        internal const int TamanoPagina = 50;

        internal const string ClavePlantillaMovimientos = "Notification.Automatic.MissingMovements";
        internal const string ErrorCorrida = "Error.Notifications.Generation.RunFailed";

        // Motivos estables de omision y falla (columna Motivo del registro tecnico).
        internal const string MotivoYaEntregada = "YaEntregada";
        internal const string MotivoCuentaInactiva = "CuentaInactiva";
        internal const string MotivoUsuarioInexistente = "UsuarioInexistente";
        internal const string MotivoMovimientosRegistrados = "MovimientosRegistrados";
        internal const string MotivoSinPendientes = "SinPendientes";
        internal const string MotivoSinPerfil = "SinPerfil";
        internal const string MotivoPerfilCambiado = "PerfilCambiado";
        internal const string MotivoIntervaloNoCumplido = "IntervaloNoCumplido";
        internal const string MotivoNivelInsuficiente = "NivelInsuficiente";
        internal const string MotivoSinAlternativas = "SinAlternativas";
        internal const string MotivoFalloLectura = "FalloLectura";
        internal const string MotivoFalloGeneracion = "FalloGeneracion";
        internal const string MotivoRespuestaNoConfiable = "RespuestaNoConfiable";
        internal const string MotivoFalloEntrega = "FalloEntrega";

        private static readonly TimeSpan HoraCorte = TimeSpan.FromHours(7);

        // Siete dias exactos (7 * 24 h en UTC) desde la ultima notificacion automatica de inversion.
        private static readonly TimeSpan IntervaloInversiones = TimeSpan.FromDays(7);

        private static readonly TipoNotificacionAutomatica[] Tipos =
        {
            TipoNotificacionAutomatica.Movimientos,
            TipoNotificacionAutomatica.Compras,
            TipoNotificacionAutomatica.Inversiones
        };

        private readonly IUnidadDeTrabajoFactory _unidadDeTrabajoFactory;
        private readonly IGeneradorRecomendaciones _generador;
        private readonly IRecomendacionInversionesBLL _inversiones;
        private readonly IIdiomaBLL _idiomaBLL;
        private readonly Func<DateTime> _ahora;

        internal GeneracionNotificacionesBLL(
            IUnidadDeTrabajoFactory unidadDeTrabajoFactory,
            IGeneradorRecomendaciones generador,
            IRecomendacionInversionesBLL inversiones,
            IIdiomaBLL idiomaBLL,
            Func<DateTime> ahora)
        {
            _unidadDeTrabajoFactory = unidadDeTrabajoFactory;
            _generador = generador;
            _inversiones = inversiones;
            _idiomaBLL = idiomaBLL;
            _ahora = ahora;
        }

        public async Task<ResultadoCorridaNotificaciones> EjecutarAsync(CancellationToken cancellationToken)
        {
            DateTime inicio = Ahora();
            ResultadoCorridaNotificaciones resultado = new ResultadoCorridaNotificaciones { Jornada = inicio.Date };
            if (inicio.TimeOfDay < HoraCorte)
            {
                resultado.AntesDelHorario = true;
                return resultado;
            }

            if (cancellationToken.IsCancellationRequested)
            {
                resultado.Cancelada = true;
                return resultado;
            }

            resultado.CorridaId = Critico(() => Escribir(unidad => unidad.GeneracionNotificaciones.InsertarCorridaYObtenerId(
                new CorridaGeneracionNotificaciones { InicioUtc = AhoraUtc(), Jornada = resultado.Jornada })));

            int ultimoId = 0;
            while (!resultado.Cancelada)
            {
                int desde = ultimoId;
                List<int> ids = Critico(() => Leer(unidad => unidad.Usuarios.ObtenerIdsActivosDespuesDe(desde, TamanoPagina)));
                if (ids.Count == 0) break;

                foreach (int usuarioId in ids)
                {
                    ultimoId = usuarioId;
                    Lazy<IIdioma> idioma = new Lazy<IIdioma>(() => _idiomaBLL.ObtenerIdiomaParaUsuario(usuarioId));
                    foreach (TipoNotificacionAutomatica tipo in Tipos)
                    {
                        if (cancellationToken.IsCancellationRequested)
                        {
                            resultado.Cancelada = true;
                            break;
                        }

                        await ProcesarAsync(new Unidad(resultado.CorridaId, usuarioId, tipo, resultado.Jornada, idioma, cancellationToken), resultado).ConfigureAwait(false);
                        if (resultado.Cancelada) break;
                    }

                    if (resultado.Cancelada) break;
                }
            }

            Critico(() => Escribir(unidad => unidad.GeneracionNotificaciones.FinalizarCorrida(
                resultado.CorridaId, AhoraUtc(), resultado.Entregadas, resultado.Omitidas, resultado.Fallidas)));
            return resultado;
        }

        // ------------------------------------------------------------------ una unidad: usuario + tipo

        private async Task ProcesarAsync(Unidad u, ResultadoCorridaNotificaciones resultado)
        {
            Desenlace desenlace;
            try
            {
                desenlace = await EvaluarAsync(u).ConfigureAwait(false);
            }
            catch (FalloUnidad falla)
            {
                desenlace = Desenlace.Fallida(falla.Motivo);
            }
            catch (OperationCanceledException) when (u.Cancelacion.IsCancellationRequested)
            {
                resultado.Cancelada = true;
                return;
            }

            // Omisiones y fallas fuera de la entrega se registran aparte; si SQL no deja registrarlas,
            // la corrida se corta con error en lugar de informar un exito falso.
            if (!desenlace.Registrado)
            {
                Critico(() => Escribir(unidad =>
                {
                    InsertarDetalle(unidad, u, desenlace.Resultado, desenlace.Motivo, null);
                    return 0;
                }));
            }

            if (desenlace.Resultado == ResultadoGeneracionNotificacion.Entregada) resultado.Entregadas++;
            else if (desenlace.Resultado == ResultadoGeneracionNotificacion.Omitida) resultado.Omitidas++;
            else resultado.Fallidas++;
        }

        private Task<Desenlace> EvaluarAsync(Unidad u)
        {
            switch (u.Tipo)
            {
                case TipoNotificacionAutomatica.Movimientos: return Task.FromResult(Movimientos(u));
                case TipoNotificacionAutomatica.Compras: return ComprasAsync(u);
                default: return InversionesAsync(u);
            }
        }

        // Movimientos: plantilla traducible, sin IA. Fecha de movimiento (DATE) del dia anterior a la jornada.
        private Desenlace Movimientos(Unidad u)
        {
            DateTime ayer = u.Jornada.AddDays(-1).Date;
            Func<IUnidadDeTrabajo, string> condicion = unidad => unidad.Movimientos.ObtenerPorUsuario(u.UsuarioId, new FiltroMovimiento { FechaDesde = ayer, FechaHasta = ayer }).Count > 0
                ? MotivoMovimientosRegistrados : null;

            string omision = Fase(MotivoFalloLectura, () => Leer(unidad => OmisionPrevia(unidad, u) ?? condicion(unidad)));
            if (omision != null) return Desenlace.Omitida(omision);

            string mensaje = Fase(MotivoFalloLectura, () =>
            {
                string plantilla = u.Idioma.Value?.ObtenerTraduccion(ClavePlantillaMovimientos) ?? ClavePlantillaMovimientos;
                return string.Format(CultureInfo.InvariantCulture, plantilla, ayer.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture));
            });

            return Entregar(u, condicion, unidad => unidad.Notificaciones.InsertarYObtenerId(Nueva<Notificacion>(u, mensaje)));
        }

        // Compras: una recomendacion consolidada de todas las listas con pendientes.
        private async Task<Desenlace> ComprasAsync(Unidad u)
        {
            Func<IUnidadDeTrabajo, string> condicion = unidad => TienePendientes(unidad, u.UsuarioId) ? null : MotivoSinPendientes;

            string omision = Fase(MotivoFalloLectura, () => Leer(unidad => OmisionPrevia(unidad, u) ?? condicion(unidad)));
            if (omision != null) return Desenlace.Omitida(omision);

            ResultadoGeneracion generado = await FaseAsync(MotivoFalloGeneracion, u.Cancelacion, () => _generador.GenerarAsync(
                new SolicitudGeneracion { Categoria = CategoriaConsulta.Compras, UsuarioId = u.UsuarioId, Idioma = u.Idioma.Value }, u.Cancelacion)).ConfigureAwait(false);
            if (!generado.Aceptada || string.IsNullOrWhiteSpace(generado.Texto)) return Desenlace.Fallida(MotivoRespuestaNoConfiable);

            return Entregar(u, condicion, unidad => unidad.RecordatoriosCompra.InsertarYObtenerId(Nueva<RecordatorioCompra>(u, generado.Texto)));
        }

        // Inversiones: perfil completo, siete dias desde la ultima automatica del usuario, nivel Comodo o Alto,
        // sin porcentaje ni monto. El chat no interviene.
        private async Task<Desenlace> InversionesAsync(Unidad u)
        {
            PerfilInversor perfil = null;
            string omision = Fase(MotivoFalloLectura, () => Leer(unidad =>
            {
                string yaEntregada = OmisionPrevia(unidad, u);
                if (yaEntregada != null) return yaEntregada;
                perfil = unidad.PerfilesInversor.ObtenerPorUsuario(u.UsuarioId);
                if (!PerfilCompleto(perfil, u.UsuarioId)) return MotivoSinPerfil;
                return MotivoIntervalo(unidad, u.UsuarioId);
            }));
            if (omision != null) return Desenlace.Omitida(omision);

            NivelInversion nivel = Fase(MotivoFalloLectura, () => _inversiones.CalcularSaldo(u.UsuarioId, null).Nivel);
            if (nivel != NivelInversion.Comodo && nivel != NivelInversion.Alto) return Desenlace.Omitida(MotivoNivelInsuficiente);

            PerfilInversor snapshot = perfil;
            ResultadoGeneracion generado = await FaseAsync(MotivoFalloGeneracion, u.Cancelacion, () => _generador.GenerarAsync(
                new SolicitudGeneracion
                {
                    Categoria = CategoriaConsulta.Inversiones,
                    UsuarioId = u.UsuarioId,
                    Idioma = u.Idioma.Value,
                    Perfil = snapshot,
                    OmitirSiSinAlternativas = true
                }, u.Cancelacion)).ConfigureAwait(false);
            if (generado.SinAlternativas) return Desenlace.Omitida(MotivoSinAlternativas);
            if (!generado.Aceptada || string.IsNullOrWhiteSpace(generado.Texto)) return Desenlace.Fallida(MotivoRespuestaNoConfiable);

            return Entregar(u,
                unidad => MotivoIntervalo(unidad, u.UsuarioId) ?? (MismoPerfil(unidad.PerfilesInversor.ObtenerPorUsuario(u.UsuarioId), snapshot) ? null : MotivoPerfilCambiado),
                unidad => unidad.RecordatoriosInversion.InsertarYObtenerId(Nueva<RecordatorioInversion>(u, generado.Texto)));
        }

        // ------------------------------------------------------------------ entrega con transaccion corta

        // Bloquea la fila del usuario (serializa lotes concurrentes), revalida destinatario, deduplicacion y
        // la condicion del tipo, y guarda el aviso con su detalle en una sola transaccion. Si algo falla antes
        // de Save no queda aviso, detalle ni intervalo consumido.
        private Desenlace Entregar(Unidad u, Func<IUnidadDeTrabajo, string> revalidar, Func<IUnidadDeTrabajo, int> insertar)
        {
            return Fase(MotivoFalloEntrega, () =>
            {
                using (IUnidadDeTrabajo unidad = _unidadDeTrabajoFactory.Crear())
                {
                    string omision = Bloquear(unidad, u) ?? revalidar(unidad);
                    if (omision != null)
                    {
                        InsertarDetalle(unidad, u, ResultadoGeneracionNotificacion.Omitida, omision, null);
                        unidad.Save();
                        return Desenlace.Omitida(omision, registrado: true);
                    }

                    int notificacionId = insertar(unidad);
                    InsertarDetalle(unidad, u, ResultadoGeneracionNotificacion.Entregada, null, notificacionId);
                    unidad.Save();
                    return Desenlace.Entregada();
                }
            });
        }

        private static string Bloquear(IUnidadDeTrabajo unidad, Unidad u)
        {
            if (!unidad.Notificaciones.BloquearUsuarioParaEntrega(u.UsuarioId)) return MotivoUsuarioInexistente;
            BE.Usuario cuenta = unidad.Usuarios.ObtenerPorId(u.UsuarioId);
            if (cuenta == null || !cuenta.Activo) return MotivoCuentaInactiva;
            return OmisionPrevia(unidad, u);
        }

        // Una entrega por usuario, tipo y jornada: el indice unico es la garantia; esto evita trabajo y errores.
        private static string OmisionPrevia(IUnidadDeTrabajo unidad, Unidad u)
        {
            return unidad.Notificaciones.ExisteAutomatica(u.UsuarioId, u.Tipo, u.Jornada) ? MotivoYaEntregada : null;
        }

        private void InsertarDetalle(IUnidadDeTrabajo unidad, Unidad u, ResultadoGeneracionNotificacion resultado, string motivo, int? notificacionId)
        {
            unidad.GeneracionNotificaciones.InsertarDetalleYObtenerId(new DetalleGeneracionNotificacion
            {
                CorridaId = u.CorridaId,
                UsuarioId = u.UsuarioId,
                Tipo = u.Tipo,
                Jornada = u.Jornada,
                Resultado = resultado,
                Motivo = motivo,
                NotificacionId = notificacionId,
                FechaUtc = AhoraUtc()
            });
        }

        private T Nueva<T>(Unidad u, string mensaje) where T : Notificacion, new()
        {
            return new T
            {
                Usuario = new BE.Usuario { Id = u.UsuarioId },
                Mensaje = mensaje,
                Fecha = Ahora(),
                Leida = false,
                TipoAutomatico = u.Tipo,
                FechaJornada = u.Jornada
            };
        }

        // ------------------------------------------------------------------ reglas de cada tipo

        private static bool TienePendientes(IUnidadDeTrabajo unidad, int usuarioId)
        {
            foreach (ListaDeCompras lista in unidad.ListasDeCompras.ObtenerPorUsuario(usuarioId))
            {
                if (unidad.ItemsLista.ObtenerPorListaDeCompras(lista.Id).Any(item => !item.Comprado)) return true;
            }

            return false;
        }

        // Perfil completo: el mismo criterio que exige el calculo con perfil (version vigente y respuestas validas).
        private static bool PerfilCompleto(PerfilInversor perfil, int usuarioId)
        {
            if (perfil == null || perfil.UsuarioId != usuarioId || perfil.VersionCuestionario != CalculadorPerfilInversor.Version
                || !Enum.IsDefined(typeof(NivelRiesgoInversor), perfil.NivelRiesgo))
            {
                return false;
            }

            try
            {
                CalculadorPerfilInversor.Evaluar(perfil.VersionCuestionario, perfil.Respuestas);
                return true;
            }
            catch (ExcepcionNegocio)
            {
                return false;
            }
        }

        private static bool MismoPerfil(PerfilInversor actual, PerfilInversor snapshot)
        {
            return actual != null && snapshot != null
                && actual.VersionCuestionario == snapshot.VersionCuestionario
                && actual.FechaEvaluacionUtc == snapshot.FechaEvaluacionUtc
                && actual.NivelRiesgo == snapshot.NivelRiesgo;
        }

        // Intervalo derivado solo de la ultima notificacion automatica de inversion del usuario. Sin ninguna
        // (nunca recibio una o la elimino) no hay restriccion.
        private string MotivoIntervalo(IUnidadDeTrabajo unidad, int usuarioId)
        {
            DateTime? ultima = unidad.Notificaciones.ObtenerFechaUltimaAutomaticaInversionUtc(usuarioId);
            return ultima.HasValue && AhoraUtc() - ultima.Value < IntervaloInversiones ? MotivoIntervaloNoCumplido : null;
        }

        // ------------------------------------------------------------------ infraestructura

        // Notificacion.Fecha es DATETIME (~3 ms): todo valor del reloj se redondea al segundo para que
        // "exactamente siete dias" se cumpla sin depender de la precision del tipo.
        private DateTime Ahora()
        {
            DateTime ahora = _ahora();
            return new DateTime(ahora.Ticks - ahora.Ticks % TimeSpan.TicksPerSecond, ahora.Kind);
        }

        private DateTime AhoraUtc()
        {
            return DateTime.SpecifyKind(Ahora(), DateTimeKind.Local).ToUniversalTime();
        }

        private T Leer<T>(Func<IUnidadDeTrabajo, T> consulta)
        {
            using (IUnidadDeTrabajo unidad = _unidadDeTrabajoFactory.Crear())
            {
                return consulta(unidad);
            }
        }

        private T Escribir<T>(Func<IUnidadDeTrabajo, T> escritura)
        {
            using (IUnidadDeTrabajo unidad = _unidadDeTrabajoFactory.Crear())
            {
                T valor = escritura(unidad);
                unidad.Save();
                return valor;
            }
        }

        // Lo que impide registrar la corrida o sus resultados es una falla de la corrida, no de un usuario.
        private static T Critico<T>(Func<T> accion)
        {
            try
            {
                return accion();
            }
            catch (Exception ex)
            {
                throw new ExcepcionTecnica(ErrorCorrida, ex);
            }
        }

        private T Fase<T>(string motivo, Func<T> accion)
        {
            try
            {
                return accion();
            }
            catch (Exception)
            {
                throw new FalloUnidad(motivo);
            }
        }

        private static async Task<T> FaseAsync<T>(string motivo, CancellationToken cancellationToken, Func<Task<T>> accion)
        {
            try
            {
                return await accion().ConfigureAwait(false);
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                throw;
            }
            catch (Exception)
            {
                throw new FalloUnidad(motivo);
            }
        }

        private sealed class FalloUnidad : Exception
        {
            internal FalloUnidad(string motivo)
                : base(motivo)
            {
                Motivo = motivo;
            }

            internal string Motivo { get; }
        }

        private sealed class Unidad
        {
            internal Unidad(int corridaId, int usuarioId, TipoNotificacionAutomatica tipo, DateTime jornada, Lazy<IIdioma> idioma, CancellationToken cancelacion)
            {
                CorridaId = corridaId;
                UsuarioId = usuarioId;
                Tipo = tipo;
                Jornada = jornada;
                Idioma = idioma;
                Cancelacion = cancelacion;
            }

            internal int CorridaId { get; }
            internal int UsuarioId { get; }
            internal TipoNotificacionAutomatica Tipo { get; }
            internal DateTime Jornada { get; }
            internal Lazy<IIdioma> Idioma { get; }
            internal CancellationToken Cancelacion { get; }
        }

        private sealed class Desenlace
        {
            internal ResultadoGeneracionNotificacion Resultado { get; private set; }
            internal string Motivo { get; private set; }
            // La entrega y las omisiones detectadas dentro de la transaccion ya dejaron su detalle.
            internal bool Registrado { get; private set; }

            internal static Desenlace Entregada() => new Desenlace { Resultado = ResultadoGeneracionNotificacion.Entregada, Registrado = true };

            internal static Desenlace Omitida(string motivo, bool registrado = false) => new Desenlace { Resultado = ResultadoGeneracionNotificacion.Omitida, Motivo = motivo, Registrado = registrado };

            internal static Desenlace Fallida(string motivo) => new Desenlace { Resultado = ResultadoGeneracionNotificacion.Fallida, Motivo = motivo };
        }
    }
}
