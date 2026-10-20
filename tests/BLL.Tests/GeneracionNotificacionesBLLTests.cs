using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ABSTRACCIONES;
using BE;
using BLL.Tests.TestDoubles;
using REPOSITORIO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SERVICIOS;

namespace BLL.Tests
{
    // Orquestador del proceso automatico diario (notificaciones-automaticas-diarias 4): jornada local
    // desde las 07:00, cuentas activas por pagina, reglas por tipo, entrega con transaccion corta y
    // aislamiento de fallas. Sin sesion: el reloj, los repositorios y el generador son dobles.
    [TestClass]
    public sealed class GeneracionNotificacionesBLLTests
    {
        private static readonly DateTime Jornada = new DateTime(2026, 10, 21);

        private static DateTime Local(int dia, int hora, int minuto = 0) => new DateTime(2026, 10, dia, hora, minuto, 0, DateTimeKind.Local);

        private static DateTime Utc(DateTime local) => DateTime.SpecifyKind(local, DateTimeKind.Local).ToUniversalTime();

        private sealed class FakeGenerador : IGeneradorRecomendaciones
        {
            public bool ComprasDisponible => true;

            public bool InversionesDisponible => true;

            public List<SolicitudGeneracion> Solicitudes { get; } = new List<SolicitudGeneracion>();

            public Func<SolicitudGeneracion, ResultadoGeneracion> Responder { get; set; } = s => ResultadoGeneracion.Valida("Recomendacion " + s.Categoria + " para " + s.UsuarioId);

            public Task<ResultadoGeneracion> GenerarAsync(SolicitudGeneracion solicitud, CancellationToken cancellationToken)
            {
                Solicitudes.Add(solicitud);
                try
                {
                    return Task.FromResult(Responder(solicitud));
                }
                catch (Exception ex)
                {
                    return Task.FromException<ResultadoGeneracion>(ex);
                }
            }
        }

        private sealed class FakeInversiones : IRecomendacionInversionesBLL
        {
            public NivelInversion Nivel { get; set; } = NivelInversion.Comodo;

            public int Consultas { get; private set; }

            public Func<int, ResumenInversiones> Resumen { get; set; }

            public System.Threading.Tasks.Task<ResumenInversiones> CalcularAsync(int usuarioActualId, decimal? porcentaje, CancellationToken cancellationToken, PerfilInversor perfil)
            {
                throw new InvalidOperationException("El proceso automatico no calcula con fuentes externas por si mismo.");
            }

            public ResumenInversiones CalcularSaldo(int usuarioActualId, decimal? porcentaje)
            {
                Consultas++;
                if (porcentaje.HasValue) throw new InvalidOperationException("La aptitud automatica no usa porcentaje.");
                return Resumen != null ? Resumen(usuarioActualId) : new ResumenInversiones { Nivel = Nivel };
            }
        }

        private sealed class FakePerfiles : IPerfilInversorRepositorio
        {
            public Dictionary<int, PerfilInversor> Perfiles { get; } = new Dictionary<int, PerfilInversor>();

            public PerfilInversor ObtenerPorUsuario(int usuarioId)
            {
                PerfilInversor perfil;
                return Perfiles.TryGetValue(usuarioId, out perfil) ? perfil : null;
            }

            public void Guardar(PerfilInversor perfil)
            {
                Perfiles[perfil.UsuarioId] = perfil;
            }
        }

        private sealed class Escenario
        {
            public readonly FakeUnidadDeTrabajo Unidad = new FakeUnidadDeTrabajo();
            public readonly FakeGenerador Generador = new FakeGenerador();
            public readonly FakeInversiones Inversiones = new FakeInversiones();
            public readonly FakePerfiles Perfiles = new FakePerfiles();
            public readonly SpyIdiomaBLL IdiomaBLL = new SpyIdiomaBLL();
            public DateTime Ahora = Local(21, 7);
            private int _nextItem = 1;

            public Escenario()
            {
                Unidad.PerfilesInversor = Perfiles;
                IdiomaBLL.IdiomaDelUsuario = new Idioma
                {
                    Traducciones = new List<SERVICIOS.Traduccion>
                    {
                        new SERVICIOS.Traduccion { Palabra = new Palabra { Key = "Notification.Automatic.MissingMovements" }, Valor = "Sin movimientos el {0}" }
                    }
                };
            }

            public GeneracionNotificacionesBLL Crear() => new GeneracionNotificacionesBLL(new FakeUnidadDeTrabajoFactory(Unidad), Generador, Inversiones, IdiomaBLL, () => Ahora);

            public Task<ResultadoCorridaNotificaciones> Ejecutar(CancellationToken ct = default(CancellationToken)) => Crear().EjecutarAsync(ct);

            public BE.Usuario Usuario(int id, bool activo = true)
            {
                BE.Usuario usuario = new BE.Usuario("usuario" + id) { Id = id, Activo = activo };
                Unidad.UsuarioRepo.Usuarios[id] = usuario;
                return usuario;
            }

            public void Pendiente(int usuarioId, string lista = "Lista", bool comprado = false)
            {
                ListaDeCompras existente = Unidad.ListaDeComprasRepo.Listas.Values.FirstOrDefault(l => l.Usuario.Id == usuarioId && l.Nombre == lista);
                if (existente == null)
                {
                    existente = new ListaDeCompras { Id = Unidad.ListaDeComprasRepo.Listas.Count + 1, Usuario = new BE.Usuario { Id = usuarioId }, Nombre = lista };
                    Unidad.ListaDeComprasRepo.Listas[existente.Id] = existente;
                }

                ItemLista item = new ItemLista { Id = _nextItem++, ListaDeCompras = existente, Descripcion = "Leche", Cantidad = 1, Comprado = comprado };
                Unidad.ItemListaRepo.Items[item.Id] = item;
            }

            public void LimpiarPendientes()
            {
                foreach (ItemLista item in Unidad.ItemListaRepo.Items.Values) item.Comprado = true;
            }

            public void Movimiento(int usuarioId, DateTime fecha, DateTime? creado = null)
            {
                int id = Unidad.MovimientoRepo.Movimientos.Count + 1;
                Unidad.MovimientoRepo.Movimientos[id] = new Movimiento { Id = id, Usuario = new BE.Usuario { Id = usuarioId }, Fecha = fecha, Tipo = TipoMovimiento.Egreso, Monto = 10, FechaCreacion = creado ?? fecha };
            }

            public PerfilInversor PerfilCompleto(int usuarioId)
            {
                PerfilInversor perfil = PerfilAsistenteTests.Perfil();
                perfil.UsuarioId = usuarioId;
                Perfiles.Perfiles[usuarioId] = perfil;
                return perfil;
            }

            public DetalleGeneracionNotificacion Detalle(int usuarioId, TipoNotificacionAutomatica tipo)
            {
                return Unidad.GeneracionNotificacionesRepo.Detalles.Last(d => d.UsuarioId == usuarioId && d.Tipo == tipo);
            }

            public IEnumerable<DetalleGeneracionNotificacion> Detalles(int usuarioId)
            {
                return Unidad.GeneracionNotificacionesRepo.Detalles.Where(d => d.UsuarioId == usuarioId);
            }

            public List<Notificacion> Entregadas(int usuarioId)
            {
                return Unidad.NotificacionRepo.Notificaciones.Values
                    .Concat(Unidad.RecordatorioCompraRepo.Recordatorios.Values)
                    .Concat(Unidad.RecordatorioInversionRepo.Recordatorios.Values.Cast<Notificacion>())
                    .Where(n => n.Usuario.Id == usuarioId).ToList();
            }
        }

        private static void AssertOmitida(Escenario e, int usuarioId, TipoNotificacionAutomatica tipo, string motivo)
        {
            DetalleGeneracionNotificacion detalle = e.Detalle(usuarioId, tipo);
            Assert.AreEqual(ResultadoGeneracionNotificacion.Omitida, detalle.Resultado, "Resultado de " + tipo);
            Assert.AreEqual(motivo, detalle.Motivo);
            Assert.IsNull(detalle.NotificacionId);
        }

        // ---------------------------------------------------------------- 4.1 jornada, cuentas y omisiones

        [TestMethod]
        public async Task Ejecutar_AntesDeLas0700_NoGeneraNadaNiRegistraCorrida()
        {
            Escenario e = new Escenario();
            e.Usuario(1);
            e.Ahora = Local(21, 6, 59);

            ResultadoCorridaNotificaciones r = await e.Ejecutar();

            Assert.IsTrue(r.AntesDelHorario);
            Assert.AreEqual(0, r.Entregadas + r.Omitidas + r.Fallidas);
            Assert.IsEmpty(e.Unidad.GeneracionNotificacionesRepo.Corridas);
            Assert.IsEmpty(e.Unidad.GeneracionNotificacionesRepo.Detalles);
            Assert.IsEmpty(e.Entregadas(1));
        }

        [TestMethod]
        public async Task Ejecutar_ExactamenteALas0700_ProcesaLaJornadaDelDia()
        {
            Escenario e = new Escenario();
            e.Usuario(1);
            e.Ahora = Local(21, 7);

            ResultadoCorridaNotificaciones r = await e.Ejecutar();

            Assert.IsFalse(r.AntesDelHorario);
            Assert.AreEqual(Jornada, r.Jornada);
            Assert.AreEqual(1, e.Entregadas(1).Count, "Sin movimientos ayer recibe el recordatorio de movimientos.");
        }

        [TestMethod]
        public async Task Ejecutar_RegistraLaCorridaConJornadaLocalInicioFinYCantidades()
        {
            Escenario e = new Escenario();
            e.Usuario(1);
            e.Pendiente(1);
            e.Ahora = Local(21, 11, 5);

            ResultadoCorridaNotificaciones r = await e.Ejecutar();

            CorridaGeneracionNotificaciones corrida = e.Unidad.GeneracionNotificacionesRepo.ObtenerCorrida(r.CorridaId);
            Assert.AreEqual(Jornada, corrida.Jornada.Date);
            Assert.AreEqual(Utc(Local(21, 11, 5)), corrida.InicioUtc);
            Assert.IsTrue(corrida.FinUtc.HasValue);
            Assert.AreEqual(2, corrida.Entregadas, "Movimientos y compras.");
            Assert.AreEqual(1, corrida.Omitidas, "Inversiones sin perfil.");
            Assert.AreEqual(0, corrida.Fallidas);
            Assert.AreEqual(2, r.Entregadas);
            Assert.AreEqual(1, r.Omitidas);
            Assert.AreEqual(3, e.Unidad.GeneracionNotificacionesRepo.ObtenerDetallePorCorrida(r.CorridaId).Count);
            Assert.IsTrue(e.Unidad.GeneracionNotificacionesRepo.Detalles.All(d => d.Jornada.Date == Jornada));
        }

        [TestMethod]
        public async Task Ejecutar_CuentaInactiva_NoSeEvaluaNiGeneraDetalle()
        {
            Escenario e = new Escenario();
            e.Usuario(1, activo: false);
            e.Usuario(2);

            await e.Ejecutar();

            Assert.IsEmpty(e.Detalles(1));
            Assert.IsEmpty(e.Entregadas(1));
            Assert.AreEqual(3, e.Detalles(2).Count());
        }

        [TestMethod]
        public async Task Ejecutar_CuentaConBloqueoTemporalDelLogin_SigueSiendoElegible()
        {
            Escenario e = new Escenario();
            BE.Usuario bloqueado = e.Usuario(1);
            bloqueado.Bloqueado = true;
            bloqueado.BloqueadoHasta = e.Ahora.AddHours(3);
            bloqueado.IntentosFallidos = 5;

            await e.Ejecutar();

            Assert.AreEqual(1, e.Entregadas(1).Count);
            Assert.AreEqual(ResultadoGeneracionNotificacion.Entregada, e.Detalle(1, TipoNotificacionAutomatica.Movimientos).Resultado);
        }

        [TestMethod]
        public async Task Ejecutar_MasDeUnaPaginaDeCuentas_ProcesaTodasSinRepetir()
        {
            Escenario e = new Escenario();
            int cantidad = GeneracionNotificacionesBLL.TamanoPagina * 2 + 3;
            for (int id = 1; id <= cantidad; id++) e.Usuario(id);
            e.Usuario(cantidad + 1, activo: false);

            ResultadoCorridaNotificaciones r = await e.Ejecutar();

            Assert.AreEqual(cantidad, e.Unidad.GeneracionNotificacionesRepo.Detalles.Select(d => d.UsuarioId).Distinct().Count());
            Assert.AreEqual(cantidad * 3, e.Unidad.GeneracionNotificacionesRepo.Detalles.Count);
            Assert.AreEqual(cantidad, r.Entregadas);
        }

        [TestMethod]
        public async Task Ejecutar_RecuperacionTardiaDelDia_SoloEvaluaLaJornadaActualSinAvisosRetroactivos()
        {
            Escenario e = new Escenario();
            e.Usuario(1);
            e.Ahora = Local(21, 11);

            await e.Ejecutar();

            Assert.IsTrue(e.Unidad.GeneracionNotificacionesRepo.Detalles.All(d => d.Jornada.Date == Jornada));
            Notificacion movimientos = e.Entregadas(1).Single();
            Assert.AreEqual(Jornada, movimientos.FechaJornada.Value.Date);
            Assert.AreEqual("Sin movimientos el 20/10/2026", movimientos.Mensaje, "Solo se revisa el dia anterior a la jornada actual.");
        }

        [TestMethod]
        public async Task Ejecutar_SegundaCorridaDelMismoDia_NoDuplicaNiReintentaLoEntregado()
        {
            Escenario e = new Escenario();
            e.Usuario(1);
            e.Pendiente(1);
            e.PerfilCompleto(1);
            await e.Ejecutar();
            int solicitudes = e.Generador.Solicitudes.Count;

            ResultadoCorridaNotificaciones r = await e.Ejecutar();

            Assert.AreEqual(3, e.Entregadas(1).Count);
            Assert.AreEqual(0, r.Entregadas);
            Assert.AreEqual(3, r.Omitidas);
            Assert.AreEqual(solicitudes, e.Generador.Solicitudes.Count, "No se vuelve a llamar a la IA por lo ya entregado.");
            AssertOmitida(e, 1, TipoNotificacionAutomatica.Movimientos, "YaEntregada");
            AssertOmitida(e, 1, TipoNotificacionAutomatica.Compras, "YaEntregada");
        }

        // ---------------------------------------------------------------- 4.2 compras y movimientos

        [TestMethod]
        public async Task Ejecutar_Movimientos_SinActividadAyer_EntregaPlantillaConLaFechaRevisadaSinIA()
        {
            Escenario e = new Escenario();
            e.Usuario(1);
            e.Movimiento(1, Local(19, 0).Date);

            await e.Ejecutar();

            Notificacion n = e.Entregadas(1).Single();
            Assert.AreEqual("Sin movimientos el 20/10/2026", n.Mensaje);
            Assert.AreEqual(TipoNotificacionAutomatica.Movimientos, n.TipoAutomatico);
            Assert.AreEqual(Jornada, n.FechaJornada.Value.Date);
            Assert.IsFalse(n.Leida);
            Assert.IsNotInstanceOfType(n, typeof(RecordatorioCompra));
            Assert.IsNotInstanceOfType(n, typeof(RecordatorioInversion));
            Assert.IsEmpty(e.Generador.Solicitudes, "El aviso de movimientos no usa IA.");
            Assert.AreEqual(n.Id, e.Detalle(1, TipoNotificacionAutomatica.Movimientos).NotificacionId);
        }

        [TestMethod]
        public async Task Ejecutar_Movimientos_RegistradoAyerAunCargadoRetroactivamenteHoy_NoGeneraRecordatorio()
        {
            Escenario e = new Escenario();
            e.Usuario(1);
            e.Movimiento(1, Local(20, 0).Date, creado: Local(21, 6, 30));

            await e.Ejecutar();

            AssertOmitida(e, 1, TipoNotificacionAutomatica.Movimientos, "MovimientosRegistrados");
            Assert.IsEmpty(e.Entregadas(1));
        }

        [TestMethod]
        public async Task Ejecutar_Movimientos_SoloCuentaElDiaAnteriorYNoLosDeHoyNiLosDeAntes()
        {
            Escenario e = new Escenario();
            e.Usuario(1);
            e.Movimiento(1, Local(21, 0).Date);
            e.Movimiento(1, Local(19, 0).Date);

            await e.Ejecutar();

            Assert.AreEqual("Sin movimientos el 20/10/2026", e.Entregadas(1).Single().Mensaje);
        }

        [TestMethod]
        public async Task Ejecutar_Movimientos_ActividadDeOtroUsuarioNoCuenta()
        {
            Escenario e = new Escenario();
            e.Usuario(1);
            e.Usuario(2);
            e.Movimiento(2, Local(20, 0).Date);

            await e.Ejecutar();

            Assert.AreEqual(1, e.Entregadas(1).Count);
            Assert.IsEmpty(e.Entregadas(2));
        }

        [TestMethod]
        public async Task Ejecutar_Compras_ConsolidaTodasLasListasConPendientesEnUnaSolaRecomendacionSinIA_DeMovimientos()
        {
            Escenario e = new Escenario();
            e.Usuario(1);
            e.Movimiento(1, Local(20, 0).Date);
            e.Pendiente(1, "Super");
            e.Pendiente(1, "Verduleria");
            e.Pendiente(1, "Super", comprado: true);

            await e.Ejecutar();

            RecordatorioCompra compra = e.Entregadas(1).OfType<RecordatorioCompra>().Single();
            Assert.AreEqual("Recomendacion Compras para 1", compra.Mensaje);
            Assert.AreEqual(TipoNotificacionAutomatica.Compras, compra.TipoAutomatico);
            Assert.AreEqual(Jornada, compra.FechaJornada.Value.Date);
            Assert.IsNull(compra.ItemLista);
            Assert.IsNull(compra.Promocion);
            Assert.IsNull(compra.FechaVencimiento);
            SolicitudGeneracion solicitud = e.Generador.Solicitudes.Single();
            Assert.AreEqual(CategoriaConsulta.Compras, solicitud.Categoria);
            Assert.AreEqual(1, solicitud.UsuarioId);
            Assert.IsNull(solicitud.NombreLista, "Sin filtro por nombre de lista.");
            Assert.IsNull(solicitud.Pregunta, "El proceso conoce la intencion: texto fijo traducible.");
            Assert.AreSame(e.IdiomaBLL.IdiomaDelUsuario, solicitud.Idioma);
        }

        [TestMethod]
        public async Task Ejecutar_Compras_MismosPendientesEnDosJornadas_EntregaCadaDia()
        {
            Escenario e = new Escenario();
            e.Usuario(1);
            e.Movimiento(1, Local(20, 0).Date);
            e.Movimiento(1, Local(21, 0).Date);
            e.Pendiente(1);
            await e.Ejecutar();

            e.Ahora = Local(22, 7);
            ResultadoCorridaNotificaciones r = await e.Ejecutar();

            List<RecordatorioCompra> compras = e.Entregadas(1).OfType<RecordatorioCompra>().OrderBy(c => c.FechaJornada).ToList();
            Assert.AreEqual(2, compras.Count);
            Assert.AreEqual(compras[0].Mensaje, compras[1].Mensaje, "Se repite aunque el contenido coincida.");
            Assert.AreEqual(new DateTime(2026, 10, 21), compras[0].FechaJornada.Value.Date);
            Assert.AreEqual(new DateTime(2026, 10, 22), compras[1].FechaJornada.Value.Date);
            Assert.AreEqual(1, r.Entregadas, "Solo compras: el dia anterior ya tiene movimientos y no hay perfil.");
        }

        [TestMethod]
        public async Task Ejecutar_Compras_SinPendientesOTodoCompletado_NoGeneraNiLlamaALaIA()
        {
            Escenario e = new Escenario();
            e.Usuario(1);
            e.Usuario(2);
            e.Pendiente(2, comprado: true);

            await e.Ejecutar();

            AssertOmitida(e, 1, TipoNotificacionAutomatica.Compras, "SinPendientes");
            AssertOmitida(e, 2, TipoNotificacionAutomatica.Compras, "SinPendientes");
            Assert.IsEmpty(e.Generador.Solicitudes);
        }

        [TestMethod]
        public async Task Ejecutar_Compras_RespuestaNoConfiable_RegistraFallaSinSustituirlaPorUnaRecomendacionInventada()
        {
            Escenario e = new Escenario();
            e.Usuario(1);
            e.Pendiente(1);
            e.Generador.Responder = s => ResultadoGeneracion.NoConfiable();

            ResultadoCorridaNotificaciones r = await e.Ejecutar();

            DetalleGeneracionNotificacion detalle = e.Detalle(1, TipoNotificacionAutomatica.Compras);
            Assert.AreEqual(ResultadoGeneracionNotificacion.Fallida, detalle.Resultado);
            Assert.AreEqual("RespuestaNoConfiable", detalle.Motivo);
            Assert.IsEmpty(e.Entregadas(1).OfType<RecordatorioCompra>());
            Assert.AreEqual(1, r.Fallidas);
        }

        // ---------------------------------------------------------------- 4.3 inversiones proactivas

        [TestMethod]
        [DataRow("Comodo")]
        [DataRow("Alto")]
        public async Task Ejecutar_Inversiones_PrimeraOportunidadConPerfilCompletoYNivelComodoOAlto_EntregaSinMonto(string nivel)
        {
            Escenario e = new Escenario();
            e.Usuario(1);
            e.Inversiones.Nivel = (NivelInversion)Enum.Parse(typeof(NivelInversion), nivel);
            PerfilInversor perfil = e.PerfilCompleto(1);

            await e.Ejecutar();

            RecordatorioInversion n = e.Entregadas(1).OfType<RecordatorioInversion>().Single();
            Assert.AreEqual("Recomendacion Inversiones para 1", n.Mensaje);
            Assert.AreEqual(TipoNotificacionAutomatica.Inversiones, n.TipoAutomatico);
            Assert.AreEqual(Jornada, n.FechaJornada.Value.Date);
            SolicitudGeneracion s = e.Generador.Solicitudes.Single(x => x.Categoria == CategoriaConsulta.Inversiones);
            Assert.IsNull(s.Porcentaje);
            Assert.AreSame(perfil, s.Perfil);
            Assert.IsTrue(s.OmitirSiSinAlternativas);
            Assert.IsNull(s.Pregunta);
            Assert.AreEqual(1, s.UsuarioId);
        }

        [TestMethod]
        [DataRow("Negativo")]
        [DataRow("Ajustado")]
        public async Task Ejecutar_Inversiones_NivelNegativoOAjustado_SeOmiteSinLlamarALaIA(string nivel)
        {
            Escenario e = new Escenario();
            e.Usuario(1);
            e.Inversiones.Nivel = (NivelInversion)Enum.Parse(typeof(NivelInversion), nivel);
            e.PerfilCompleto(1);

            await e.Ejecutar();

            AssertOmitida(e, 1, TipoNotificacionAutomatica.Inversiones, "NivelInsuficiente");
            Assert.IsFalse(e.Generador.Solicitudes.Any(x => x.Categoria == CategoriaConsulta.Inversiones));
        }

        [TestMethod]
        public async Task Ejecutar_Inversiones_SinPerfilOPerfilDeOtraVersion_SeOmiteAntesDeCalcular()
        {
            Escenario e = new Escenario();
            e.Usuario(1);
            e.Usuario(2);
            e.PerfilCompleto(2).VersionCuestionario = 99;

            await e.Ejecutar();

            AssertOmitida(e, 1, TipoNotificacionAutomatica.Inversiones, "SinPerfil");
            AssertOmitida(e, 2, TipoNotificacionAutomatica.Inversiones, "SinPerfil");
            Assert.AreEqual(0, e.Inversiones.Consultas, "El perfil se consulta antes del calculo economico.");
        }

        [TestMethod]
        public async Task Ejecutar_Inversiones_NotificacionAutomaticaRecienteMenosDeSieteDias_SeOmite()
        {
            Escenario e = new Escenario();
            e.Usuario(1);
            e.PerfilCompleto(1);
            e.Unidad.NotificacionRepo.UltimaAutomaticaInversionUtc = Utc(e.Ahora).AddDays(-7).AddSeconds(1);

            await e.Ejecutar();

            AssertOmitida(e, 1, TipoNotificacionAutomatica.Inversiones, "IntervaloNoCumplido");
            Assert.IsEmpty(e.Entregadas(1).OfType<RecordatorioInversion>());
        }

        [TestMethod]
        public async Task Ejecutar_Inversiones_ExactamenteSieteDiasDesdeLaUltima_PuedeRecibirOtra()
        {
            Escenario e = new Escenario();
            e.Usuario(1);
            e.PerfilCompleto(1);
            e.Unidad.NotificacionRepo.UltimaAutomaticaInversionUtc = Utc(e.Ahora).AddDays(-7);

            await e.Ejecutar();

            Assert.AreEqual(1, e.Entregadas(1).OfType<RecordatorioInversion>().Count());
        }

        [TestMethod]
        public async Task Ejecutar_Entrega_FechaDeLaNotificacionSeGuardaRedondeadaAlSegundo()
        {
            Escenario e = new Escenario();
            e.Usuario(1);
            e.Ahora = Local(21, 7).AddMilliseconds(987);

            await e.Ejecutar();

            Notificacion n = e.Entregadas(1).Single();
            Assert.AreEqual(Local(21, 7), n.Fecha, "Notificacion.Fecha es DATETIME (~3 ms): se guarda sin fraccion para medir exactamente siete dias.");
        }

        [TestMethod]
        public async Task Ejecutar_Inversiones_UltimaNotificacionEliminada_TomaLaAnteriorOPermiteOtra()
        {
            Escenario e = new Escenario();
            e.Usuario(1);
            e.PerfilCompleto(1);
            RecordatorioInversion anterior = new RecordatorioInversion { Usuario = new BE.Usuario { Id = 1 }, Mensaje = "vieja", Fecha = e.Ahora.AddDays(-9), TipoAutomatico = TipoNotificacionAutomatica.Inversiones, FechaJornada = e.Ahora.AddDays(-9).Date };
            RecordatorioInversion reciente = new RecordatorioInversion { Usuario = new BE.Usuario { Id = 1 }, Mensaje = "nueva", Fecha = e.Ahora.AddDays(-2), TipoAutomatico = TipoNotificacionAutomatica.Inversiones, FechaJornada = e.Ahora.AddDays(-2).Date };
            e.Unidad.RecordatorioInversionRepo.InsertarYObtenerId(anterior);
            int idReciente = e.Unidad.RecordatorioInversionRepo.InsertarYObtenerId(reciente);
            await e.Ejecutar();
            AssertOmitida(e, 1, TipoNotificacionAutomatica.Inversiones, "IntervaloNoCumplido");

            e.Unidad.RecordatorioInversionRepo.Eliminar(idReciente);
            await e.Ejecutar();

            Assert.AreEqual(1, e.Entregadas(1).OfType<RecordatorioInversion>().Count(n => n.Mensaje.StartsWith("Recomendacion")), "La anterior ya paso los siete dias.");
        }

        [TestMethod]
        public async Task Ejecutar_Inversiones_EjemplosYRecordatoriosManualesNoCuentanParaElIntervalo()
        {
            Escenario e = new Escenario();
            e.Usuario(1);
            e.PerfilCompleto(1);
            e.Unidad.RecordatorioInversionRepo.InsertarYObtenerId(new RecordatorioInversion { Usuario = new BE.Usuario { Id = 1 }, Mensaje = "ejemplo", Fecha = e.Ahora.AddHours(-1), ClaveEjemplo = "Demo.Inversiones" });
            e.Unidad.RecordatorioInversionRepo.InsertarYObtenerId(new RecordatorioInversion { Usuario = new BE.Usuario { Id = 1 }, Mensaje = "manual", Fecha = e.Ahora.AddHours(-1) });

            await e.Ejecutar();

            Assert.AreEqual(1, e.Entregadas(1).OfType<RecordatorioInversion>().Count(n => n.TipoAutomatico.HasValue));
        }

        [TestMethod]
        public async Task Ejecutar_Inversiones_ElChatNoParticipaDelIntervalo()
        {
            Escenario e = new Escenario();
            e.Usuario(1);
            e.PerfilCompleto(1);
            e.Unidad.ConsultaChatbotRepo.InsertarYObtenerId(new ConsultaChatbot { UsuarioId = 1, Pregunta = "en que invierto?", Respuesta = "Fondo X", Fecha = e.Ahora.AddDays(-1), EsPregunta = true });

            await e.Ejecutar();

            Assert.AreEqual(1, e.Entregadas(1).OfType<RecordatorioInversion>().Count());
        }

        [TestMethod]
        public async Task Ejecutar_Inversiones_SinCandidatos_OmiteComoSinAlternativasYNoConsumeElIntervalo()
        {
            Escenario e = new Escenario();
            e.Usuario(1);
            e.PerfilCompleto(1);
            e.Generador.Responder = s => s.Categoria == CategoriaConsulta.Inversiones ? ResultadoGeneracion.SinCandidatos() : ResultadoGeneracion.Valida("texto");
            await e.Ejecutar();
            AssertOmitida(e, 1, TipoNotificacionAutomatica.Inversiones, "SinAlternativas");
            Assert.IsNull(e.Unidad.NotificacionRepo.ObtenerFechaUltimaAutomaticaInversionUtc(1));

            e.Generador.Responder = s => ResultadoGeneracion.Valida("texto");
            e.Ahora = Local(21, 11);
            await e.Ejecutar();

            Assert.AreEqual(1, e.Entregadas(1).OfType<RecordatorioInversion>().Count(), "El mismo dia, mas tarde, puede recibirla.");
        }

        [TestMethod]
        public async Task Ejecutar_Inversiones_FuentesParciales_EntregaLoQueElColaboradorRedactoConSusAvisos()
        {
            Escenario e = new Escenario();
            e.Usuario(1);
            e.PerfilCompleto(1);
            e.Generador.Responder = s => ResultadoGeneracion.Valida("Orientacion" + Environment.NewLine + "Faltan fuentes: criptoya BTC" + Environment.NewLine + "Aviso legal");

            await e.Ejecutar();

            RecordatorioInversion n = e.Entregadas(1).OfType<RecordatorioInversion>().Single();
            StringAssert.Contains(n.Mensaje, "Faltan fuentes: criptoya BTC");
            StringAssert.EndsWith(n.Mensaje, "Aviso legal");
        }

        // ---------------------------------------------------------------- 4.4 entrega con transaccion corta

        [TestMethod]
        public async Task Ejecutar_Entrega_BloqueaAlUsuarioAntesDeInsertarYGuardaNotificacionYDetalleEnUnaSolaTransaccion()
        {
            Escenario e = new Escenario();
            e.Usuario(1);
            int guardadosAntes = e.Unidad.SaveCallCount;

            await e.Ejecutar();

            CollectionAssert.Contains(e.Unidad.NotificacionRepo.Llamadas, "bloquear:1");
            Assert.AreEqual(1, e.Entregadas(1).Count);
            Assert.AreEqual(e.Entregadas(1).Single().Id, e.Detalle(1, TipoNotificacionAutomatica.Movimientos).NotificacionId);
            // Una transaccion para la corrida, una por entrega o registro de omision y una para cerrar la corrida.
            Assert.AreEqual(guardadosAntes + 1 + 3 + 1, e.Unidad.SaveCallCount);
        }

        [TestMethod]
        public async Task Ejecutar_Entrega_CuentaDesactivadaDuranteElCalculo_NoEntregaYRegistraLaOmision()
        {
            Escenario e = new Escenario();
            BE.Usuario usuario = e.Usuario(1);
            e.Movimiento(1, Local(20, 0).Date);
            e.Pendiente(1);
            e.Generador.Responder = s => { usuario.Activo = false; return ResultadoGeneracion.Valida("texto"); };

            await e.Ejecutar();

            AssertOmitida(e, 1, TipoNotificacionAutomatica.Compras, "CuentaInactiva");
            Assert.IsEmpty(e.Entregadas(1));
        }

        [TestMethod]
        public async Task Ejecutar_Entrega_PendientesCompletadosDuranteElCalculo_NoEntrega()
        {
            Escenario e = new Escenario();
            e.Usuario(1);
            e.Movimiento(1, Local(20, 0).Date);
            e.Pendiente(1);
            e.Generador.Responder = s => { e.LimpiarPendientes(); return ResultadoGeneracion.Valida("texto"); };

            await e.Ejecutar();

            AssertOmitida(e, 1, TipoNotificacionAutomatica.Compras, "SinPendientes");
            Assert.IsEmpty(e.Entregadas(1));
        }

        [TestMethod]
        public async Task Ejecutar_Entrega_MovimientoCargadoDuranteLaRevision_NoEntregaElRecordatorio()
        {
            Escenario e = new Escenario();
            e.Usuario(1);
            e.IdiomaBLL.AlObtenerIdioma = id => e.Movimiento(1, Local(20, 0).Date);

            await e.Ejecutar();

            AssertOmitida(e, 1, TipoNotificacionAutomatica.Movimientos, "MovimientosRegistrados");
            Assert.IsEmpty(e.Entregadas(1));
        }

        [TestMethod]
        public async Task Ejecutar_Entrega_PerfilCambiadoDuranteElCalculo_OmiteSinConsumirElIntervalo()
        {
            Escenario e = new Escenario();
            e.Usuario(1);
            e.PerfilCompleto(1);
            e.Generador.Responder = s =>
            {
                PerfilInversor nuevo = e.PerfilCompleto(1);
                nuevo.FechaEvaluacionUtc = nuevo.FechaEvaluacionUtc.AddMinutes(5);
                return ResultadoGeneracion.Valida("texto");
            };

            await e.Ejecutar();

            AssertOmitida(e, 1, TipoNotificacionAutomatica.Inversiones, "PerfilCambiado");
            Assert.IsNull(e.Unidad.NotificacionRepo.ObtenerFechaUltimaAutomaticaInversionUtc(1));
        }

        [TestMethod]
        public async Task Ejecutar_Entrega_PerfilEliminadoDuranteElCalculo_Omite()
        {
            Escenario e = new Escenario();
            e.Usuario(1);
            e.PerfilCompleto(1);
            e.Generador.Responder = s => { e.Perfiles.Perfiles.Remove(1); return ResultadoGeneracion.Valida("texto"); };

            await e.Ejecutar();

            AssertOmitida(e, 1, TipoNotificacionAutomatica.Inversiones, "PerfilCambiado");
        }

        [TestMethod]
        public async Task Ejecutar_Entrega_OtroLoteEntregoDuranteElCalculo_RevalidaDeduplicacionEIntervalo()
        {
            Escenario e = new Escenario();
            e.Usuario(1);
            e.Movimiento(1, Local(20, 0).Date);
            e.Pendiente(1);
            e.PerfilCompleto(1);
            e.Generador.Responder = s =>
            {
                // Otro proceso entrega mientras este espera a la IA.
                if (s.Categoria == CategoriaConsulta.Compras)
                    e.Unidad.RecordatorioCompraRepo.InsertarYObtenerId(new RecordatorioCompra { Usuario = new BE.Usuario { Id = 1 }, Mensaje = "otro lote", Fecha = e.Ahora, TipoAutomatico = TipoNotificacionAutomatica.Compras, FechaJornada = Jornada });
                else
                    e.Unidad.RecordatorioInversionRepo.InsertarYObtenerId(new RecordatorioInversion { Usuario = new BE.Usuario { Id = 1 }, Mensaje = "otro lote", Fecha = e.Ahora.AddMinutes(-1), TipoAutomatico = TipoNotificacionAutomatica.Inversiones, FechaJornada = Jornada });
                return ResultadoGeneracion.Valida("texto");
            };

            await e.Ejecutar();

            AssertOmitida(e, 1, TipoNotificacionAutomatica.Compras, "YaEntregada");
            Assert.AreEqual(1, e.Entregadas(1).OfType<RecordatorioCompra>().Count());
            AssertOmitida(e, 1, TipoNotificacionAutomatica.Inversiones, "YaEntregada");
            Assert.AreEqual(1, e.Entregadas(1).OfType<RecordatorioInversion>().Count());
        }

        [TestMethod]
        public async Task Ejecutar_Entrega_OtroLoteEntregoInversionDeOtraJornadaDuranteElCalculo_RespetaElIntervalo()
        {
            Escenario e = new Escenario();
            e.Usuario(1);
            e.PerfilCompleto(1);
            e.Generador.Responder = s =>
            {
                e.Unidad.NotificacionRepo.UltimaAutomaticaInversionUtc = Utc(e.Ahora).AddHours(-1);
                return ResultadoGeneracion.Valida("texto");
            };

            await e.Ejecutar();

            AssertOmitida(e, 1, TipoNotificacionAutomatica.Inversiones, "IntervaloNoCumplido");
            Assert.IsEmpty(e.Entregadas(1).OfType<RecordatorioInversion>());
        }

        [TestMethod]
        public async Task Ejecutar_Entrega_UsuarioInexistenteAlBloquear_Omite()
        {
            Escenario e = new Escenario();
            e.Usuario(1);
            e.Unidad.NotificacionRepo.UsuarioExiste = false;

            await e.Ejecutar();

            AssertOmitida(e, 1, TipoNotificacionAutomatica.Movimientos, "UsuarioInexistente");
        }

        // ---------------------------------------------------------------- 4.5 aislamiento de fallas

        [TestMethod]
        public async Task Ejecutar_FallaDeIAEnCompras_NoImpideMovimientosInversionesNiOtrosUsuarios()
        {
            Escenario e = new Escenario();
            e.Usuario(1);
            e.Usuario(2);
            e.Pendiente(1);
            e.Pendiente(2);
            e.PerfilCompleto(1);
            e.Generador.Responder = s =>
            {
                if (s.Categoria == CategoriaConsulta.Compras && s.UsuarioId == 1) throw new ExcepcionTecnica("Error.Assistant.ResponseFailed");
                return ResultadoGeneracion.Valida("texto " + s.Categoria + s.UsuarioId);
            };

            ResultadoCorridaNotificaciones r = await e.Ejecutar();

            Assert.AreEqual(ResultadoGeneracionNotificacion.Fallida, e.Detalle(1, TipoNotificacionAutomatica.Compras).Resultado);
            Assert.AreEqual("FalloGeneracion", e.Detalle(1, TipoNotificacionAutomatica.Compras).Motivo);
            Assert.AreEqual(ResultadoGeneracionNotificacion.Entregada, e.Detalle(1, TipoNotificacionAutomatica.Movimientos).Resultado);
            Assert.AreEqual(ResultadoGeneracionNotificacion.Entregada, e.Detalle(1, TipoNotificacionAutomatica.Inversiones).Resultado);
            Assert.AreEqual(ResultadoGeneracionNotificacion.Entregada, e.Detalle(2, TipoNotificacionAutomatica.Compras).Resultado);
            Assert.AreEqual(1, r.Fallidas);
            Assert.IsEmpty(e.Entregadas(1).OfType<RecordatorioCompra>());
        }

        [TestMethod]
        public async Task Ejecutar_FallaDeIAEnInversiones_NoImpideLosDemasUsuarios()
        {
            Escenario e = new Escenario();
            e.Usuario(1);
            e.Usuario(2);
            e.PerfilCompleto(1);
            e.PerfilCompleto(2);
            e.Generador.Responder = s => { if (s.UsuarioId == 1) throw new InvalidOperationException("proveedor caido"); return ResultadoGeneracion.Valida("texto"); };

            ResultadoCorridaNotificaciones r = await e.Ejecutar();

            Assert.AreEqual(ResultadoGeneracionNotificacion.Fallida, e.Detalle(1, TipoNotificacionAutomatica.Inversiones).Resultado);
            Assert.AreEqual(ResultadoGeneracionNotificacion.Entregada, e.Detalle(2, TipoNotificacionAutomatica.Inversiones).Resultado);
            Assert.AreEqual(1, r.Fallidas);
        }

        [TestMethod]
        public async Task Ejecutar_FallaRegistrada_NoGuardaMensajesDeExcepcionNiSecretos()
        {
            Escenario e = new Escenario();
            e.Usuario(1);
            e.Pendiente(1);
            e.Generador.Responder = s => throw new InvalidOperationException("https://api.example.com/?key=SECRETO-123");

            await e.Ejecutar();

            Assert.IsTrue(e.Unidad.GeneracionNotificacionesRepo.Detalles.All(d => d.Motivo == null || !d.Motivo.Contains("SECRETO")));
            Assert.AreEqual("FalloGeneracion", e.Detalle(1, TipoNotificacionAutomatica.Compras).Motivo);
        }

        [TestMethod]
        public async Task Ejecutar_TimeoutDelProveedorSinCancelarLaCorrida_EsUnaFallaYContinua()
        {
            Escenario e = new Escenario();
            e.Usuario(1);
            e.Usuario(2);
            e.Pendiente(1);
            e.Pendiente(2);
            e.Generador.Responder = s => { if (s.UsuarioId == 1) throw new TaskCanceledException("timeout"); return ResultadoGeneracion.Valida("texto"); };

            ResultadoCorridaNotificaciones r = await e.Ejecutar();

            Assert.IsFalse(r.Cancelada);
            Assert.AreEqual(ResultadoGeneracionNotificacion.Fallida, e.Detalle(1, TipoNotificacionAutomatica.Compras).Resultado);
            Assert.AreEqual(ResultadoGeneracionNotificacion.Entregada, e.Detalle(2, TipoNotificacionAutomatica.Compras).Resultado);
        }

        [TestMethod]
        public async Task Ejecutar_FallaDeLecturaDeUnUsuario_QuedaRegistradaYSeguimosConElResto()
        {
            Escenario e = new Escenario();
            e.Usuario(1);
            e.Usuario(2);
            e.PerfilCompleto(1);
            e.PerfilCompleto(2);
            e.Inversiones.Resumen = id => { if (id == 1) throw new InvalidOperationException("sql"); return new ResumenInversiones { Nivel = NivelInversion.Comodo }; };

            ResultadoCorridaNotificaciones r = await e.Ejecutar();

            Assert.AreEqual("FalloLectura", e.Detalle(1, TipoNotificacionAutomatica.Inversiones).Motivo);
            Assert.AreEqual(ResultadoGeneracionNotificacion.Entregada, e.Detalle(2, TipoNotificacionAutomatica.Inversiones).Resultado);
            Assert.AreEqual(1, r.Fallidas);
        }

        [TestMethod]
        public async Task Ejecutar_FallaAlGuardarLaEntrega_SeRegistraComoFallidaYNoCuentaComoEntregada()
        {
            Escenario e = new Escenario();
            e.Usuario(1);
            e.Usuario(2);
            // Guardado 1: la corrida. Guardado 2: entrega de movimientos del usuario 1, que falla.
            e.Unidad.FailSaveCalls.Add(2);

            ResultadoCorridaNotificaciones r = await e.Ejecutar();

            DetalleGeneracionNotificacion detalle = e.Unidad.GeneracionNotificacionesRepo.Detalles.Last(d => d.UsuarioId == 1 && d.Tipo == TipoNotificacionAutomatica.Movimientos);
            Assert.AreEqual(ResultadoGeneracionNotificacion.Fallida, detalle.Resultado);
            Assert.AreEqual("FalloEntrega", detalle.Motivo);
            Assert.AreEqual(1, r.Fallidas);
            Assert.AreEqual(ResultadoGeneracionNotificacion.Entregada, e.Detalle(2, TipoNotificacionAutomatica.Movimientos).Resultado);
        }

        [TestMethod]
        public async Task Ejecutar_SqlNoPermiteRegistrarLaFalla_SeInformaComoErrorYNoComoExito()
        {
            Escenario e = new Escenario();
            e.Usuario(1);
            e.Unidad.FailSaveCalls.Add(2);
            e.Unidad.FailSaveCalls.Add(3);

            ExcepcionTecnica ex = await Assert.ThrowsExactlyAsync<ExcepcionTecnica>(() => e.Ejecutar());

            Assert.AreEqual("Error.Notifications.Generation.RunFailed", ex.Message);
        }

        [TestMethod]
        public async Task Ejecutar_SqlNoDisponibleAlIniciar_SeInformaComoErrorTecnico()
        {
            Escenario e = new Escenario();
            e.Usuario(1);
            e.Unidad.ThrowOnSave = true;

            ExcepcionTecnica ex = await Assert.ThrowsExactlyAsync<ExcepcionTecnica>(() => e.Ejecutar());

            Assert.AreEqual("Error.Notifications.Generation.RunFailed", ex.Message);
            Assert.IsEmpty(e.Entregadas(1));
        }

        [TestMethod]
        public async Task Ejecutar_CancelacionDuranteLaCorrida_NoIniciaNuevasUnidadesYCierraLaCorrida()
        {
            Escenario e = new Escenario();
            e.Usuario(1);
            e.Usuario(2);
            e.Pendiente(1);
            e.Pendiente(2);
            e.PerfilCompleto(1);
            e.PerfilCompleto(2);
            using (CancellationTokenSource cts = new CancellationTokenSource())
            {
                e.Generador.Responder = s => { cts.Cancel(); return ResultadoGeneracion.Valida("texto"); };

                ResultadoCorridaNotificaciones r = await e.Ejecutar(cts.Token);

                Assert.IsTrue(r.Cancelada);
                Assert.IsEmpty(e.Detalles(2), "El usuario siguiente no se inicia.");
                Assert.IsFalse(e.Detalles(1).Any(d => d.Tipo == TipoNotificacionAutomatica.Inversiones), "Tampoco el tipo siguiente del mismo usuario.");
                Assert.IsTrue(e.Unidad.GeneracionNotificacionesRepo.ObtenerCorrida(r.CorridaId).FinUtc.HasValue);
                Assert.AreEqual(0, r.Fallidas);
            }
        }

        [TestMethod]
        public async Task Ejecutar_CancelacionDentroDelColaborador_NoSeRegistraComoFalla()
        {
            Escenario e = new Escenario();
            e.Usuario(1);
            e.Pendiente(1);
            using (CancellationTokenSource cts = new CancellationTokenSource())
            {
                e.Generador.Responder = s => { cts.Cancel(); throw new OperationCanceledException(cts.Token); };

                ResultadoCorridaNotificaciones r = await e.Ejecutar(cts.Token);

                Assert.IsTrue(r.Cancelada);
                Assert.AreEqual(0, r.Fallidas);
                Assert.IsFalse(e.Detalles(1).Any(d => d.Tipo == TipoNotificacionAutomatica.Compras));
            }
        }

        [TestMethod]
        public async Task Ejecutar_CanceladaAntesDeEmpezar_NoProcesaNingunUsuario()
        {
            Escenario e = new Escenario();
            e.Usuario(1);

            ResultadoCorridaNotificaciones r = await e.Ejecutar(new CancellationToken(true));

            Assert.IsTrue(r.Cancelada);
            Assert.IsEmpty(e.Unidad.GeneracionNotificacionesRepo.Detalles);
            Assert.IsEmpty(e.Entregadas(1));
        }
    }
}
