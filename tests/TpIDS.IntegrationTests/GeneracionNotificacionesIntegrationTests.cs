using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ABSTRACCIONES;
using BE;
using BLL;
using DAL;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using REPOSITORIO;
using SERVICIOS;

namespace TpIDS.IntegrationTests
{
    // Entrega del proceso automatico contra SQL Server real (notificaciones-automaticas-diarias 4.4 y 4.5):
    // transaccion corta con bloqueo de usuario, indice diario, revalidacion final y rollback. La
    // entrega confirma de verdad, asi que cada test usa una cuenta propia y jornadas en 2099 que
    // limpia al terminar (nunca se solapan con datos reales ni con otros tests).
    [TestClass]
    [TestCategory("Integration")]
    public sealed class GeneracionNotificacionesIntegrationTests
    {
        private static DateTime Reloj(int mes, int dia, int milisegundos = 0) => new DateTime(2099, mes, dia, 7, 0, 0, milisegundos, DateTimeKind.Local);

        private sealed class GeneradorFijo : IGeneradorRecomendaciones
        {
            public bool ComprasDisponible => true;

            public bool InversionesDisponible => true;

            public Func<SolicitudGeneracion, ResultadoGeneracion> Responder { get; set; } = s => ResultadoGeneracion.Valida("Recomendacion " + s.Categoria + ".\nSegunda linea.");

            public Task<ResultadoGeneracion> GenerarAsync(SolicitudGeneracion solicitud, CancellationToken cancellationToken)
            {
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

        private sealed class InversionesComodo : IRecomendacionInversionesBLL
        {
            public System.Threading.Tasks.Task<ResumenInversiones> CalcularAsync(int usuarioActualId, decimal? porcentaje, CancellationToken cancellationToken, PerfilInversor perfil)
            {
                throw new InvalidOperationException();
            }

            public ResumenInversiones CalcularSaldo(int usuarioActualId, decimal? porcentaje) => new ResumenInversiones { Nivel = NivelInversion.Comodo };
        }

        private sealed class IdiomaFijo : IIdiomaBLL
        {
            public void AplicarIdiomaSesionUsuario(int usuarioId)
            {
            }

            public Idioma ObtenerIdiomaParaUsuario(int usuarioId)
            {
                return new Idioma
                {
                    Traducciones = new List<SERVICIOS.Traduccion>
                    {
                        new SERVICIOS.Traduccion { Palabra = new Palabra { Key = "Notification.Automatic.MissingMovements" }, Valor = "Sin movimientos el {0}" }
                    }
                };
            }
        }

        // Cuenta activa propia y limpieza total al terminar (incluye lo entregado a las demas cuentas
        // activas de la base en las jornadas del test).
        private sealed class Entorno : IDisposable
        {
            private readonly DateTime[] _jornadas;

            internal Entorno(params DateTime[] jornadas)
            {
                _jornadas = jornadas.Select(j => j.Date).ToArray();
                using (AccesoDatosSqlServer acceso = new AccesoDatosSqlServer())
                {
                    UsuarioId = acceso.WriteWithResult(
                        "INSERT INTO Usuarios (Username, PasswordHash, Activo, IntentosFallidos, Bloqueado, DVH) OUTPUT INSERTED.Id VALUES (@U, 'x', 1, 0, 0, 'x')",
                        new List<SqlParameter> { acceso.CreateParameter("@U", SqlDbType.VarChar, "ent_" + Guid.NewGuid().ToString("N").Substring(0, 12)) });
                }
            }

            internal int UsuarioId { get; }

            internal void Sql(string sql, params object[] parametros)
            {
                using (AccesoDatosSqlServer acceso = new AccesoDatosSqlServer())
                {
                    List<SqlParameter> lista = new List<SqlParameter> { acceso.CreateParameter("@U", SqlDbType.Int, UsuarioId) };
                    for (int i = 0; i < parametros.Length; i++) lista.Add(acceso.CreateParameter("@P" + i, SqlDbType.Int, parametros[i]));
                    acceso.Write(sql, lista);
                }
            }

            internal void ConPendiente()
            {
                Sql("INSERT INTO ListaDeCompras (UsuarioId, Nombre) VALUES (@U, 'Lista entrega'); INSERT INTO ItemLista (ListaDeComprasId, Descripcion, Cantidad, Comprado) VALUES (SCOPE_IDENTITY(), 'Leche', 1, 0)");
            }

            internal void ConPerfil(string opciones)
            {
                new PerfilInversorRepositorio().Guardar(CrearPerfil(UsuarioId, opciones));
            }

            internal DataTable Notificaciones(string tipo)
            {
                using (AccesoDatosSqlServer acceso = new AccesoDatosSqlServer())
                {
                    return acceso.Read("SELECT * FROM Notificacion WHERE UsuarioId = @U AND TipoAutomatico = @T ORDER BY FechaJornada, Id",
                        new List<SqlParameter> { acceso.CreateParameter("@U", SqlDbType.Int, UsuarioId), acceso.CreateParameter("@T", SqlDbType.VarChar, tipo) });
                }
            }

            internal DataTable Detalles(string tipo)
            {
                using (AccesoDatosSqlServer acceso = new AccesoDatosSqlServer())
                {
                    return acceso.Read("SELECT * FROM DetalleGeneracionNotificacion WHERE UsuarioId = @U AND TipoAutomatico = @T ORDER BY Id",
                        new List<SqlParameter> { acceso.CreateParameter("@U", SqlDbType.Int, UsuarioId), acceso.CreateParameter("@T", SqlDbType.VarChar, tipo) });
                }
            }

            public void Dispose()
            {
                using (AccesoDatosSqlServer acceso = new AccesoDatosSqlServer())
                {
                    foreach (DateTime jornada in _jornadas)
                    {
                        foreach (string sql in new[]
                        {
                            "DELETE FROM DetalleGeneracionNotificacion WHERE Jornada = @J",
                            "DELETE FROM Notificacion WHERE FechaJornada = @J",
                            "DELETE FROM CorridaGeneracionNotificaciones WHERE Jornada = @J"
                        })
                        {
                            acceso.Write(sql, new List<SqlParameter> { acceso.CreateParameter("@J", SqlDbType.Date, jornada) });
                        }
                    }

                    foreach (string sql in new[]
                    {
                        "DELETE FROM DetalleGeneracionNotificacion WHERE UsuarioId = @U",
                        "DELETE FROM Notificacion WHERE UsuarioId = @U",
                        "DELETE FROM ItemLista WHERE ListaDeComprasId IN (SELECT Id FROM ListaDeCompras WHERE UsuarioId = @U)",
                        "DELETE FROM ListaDeCompras WHERE UsuarioId = @U",
                        "DELETE FROM PerfilInversor WHERE UsuarioId = @U",
                        "DELETE FROM Usuarios WHERE Id = @U"
                    })
                    {
                        acceso.Write(sql, new List<SqlParameter> { acceso.CreateParameter("@U", SqlDbType.Int, UsuarioId) });
                    }
                }
            }
        }

        private static PerfilInversor CrearPerfil(int usuarioId, string opciones)
        {
            PerfilInversor perfil = CalculadorPerfilInversor.Evaluar(1, opciones.Select((o, i) =>
                new RespuestaPerfilInversor { PreguntaId = i + 1, OpcionId = o.ToString() }));
            perfil.UsuarioId = usuarioId;
            perfil.FechaEvaluacionUtc = DateTime.UtcNow;
            return perfil;
        }

        private static GeneracionNotificacionesBLL Crear(GeneradorFijo generador, Func<DateTime> reloj)
        {
            return new GeneracionNotificacionesBLL(new UnidadDeTrabajoFactory(), generador, new InversionesComodo(), new IdiomaFijo(), reloj);
        }

        private static string Resultado(DataTable detalles, string resultado, string motivo = null)
        {
            return string.Join(",", detalles.Rows.Cast<DataRow>().Where(r => r["Resultado"].ToString() == resultado && (motivo == null || r["Motivo"].ToString() == motivo)).Select(r => r["Id"].ToString()));
        }

        // ------------------------------------------------------------------ flujo completo

        [TestMethod]
        public async Task Ejecutar_FlujoCompleto_EntregaLosTresTiposSinLeerYRespetaJornadaEIntervalo()
        {
            using (Entorno e = new Entorno(new DateTime(2099, 6, 10), new DateTime(2099, 6, 11), new DateTime(2099, 6, 17)))
            {
                e.ConPendiente();
                e.ConPerfil("CDCDCCDDDC");
                GeneradorFijo generador = new GeneradorFijo();

                await Crear(generador, () => Reloj(6, 10, 987)).EjecutarAsync(CancellationToken.None);

                DataTable movimientos = e.Notificaciones("Movimientos");
                DataTable compras = e.Notificaciones("Compras");
                DataTable inversiones = e.Notificaciones("Inversiones");
                Assert.AreEqual(1, movimientos.Rows.Count);
                Assert.AreEqual(1, compras.Rows.Count);
                Assert.AreEqual(1, inversiones.Rows.Count);
                Assert.AreEqual("Sin movimientos el 09/06/2099", movimientos.Rows[0]["Mensaje"]);
                Assert.AreEqual("RecordatorioCompra", compras.Rows[0]["TipoNotificacion"]);
                Assert.AreEqual("RecordatorioInversion", inversiones.Rows[0]["TipoNotificacion"]);
                Assert.AreEqual("Recomendacion Compras.\nSegunda linea.", compras.Rows[0]["Mensaje"]);
                Assert.IsTrue(compras.Rows[0]["ItemListaId"] == DBNull.Value && compras.Rows[0]["FechaVencimiento"] == DBNull.Value);
                Assert.IsFalse((bool)compras.Rows[0]["Leida"]);
                Assert.AreEqual(new DateTime(2099, 6, 10, 7, 0, 0), (DateTime)inversiones.Rows[0]["Fecha"], "Fecha DATETIME guardada sin fraccion.");
                Assert.AreEqual(1, e.Detalles("Inversiones").Rows.Count);
                Assert.AreEqual((int)inversiones.Rows[0]["Id"], (int)e.Detalles("Inversiones").Rows[0]["NotificacionId"]);

                // Misma jornada: no se duplica nada.
                await Crear(generador, () => Reloj(6, 10)).EjecutarAsync(CancellationToken.None);
                Assert.AreEqual(1, e.Notificaciones("Compras").Rows.Count);
                Assert.AreEqual(1, e.Notificaciones("Inversiones").Rows.Count);
                Assert.AreEqual(1, e.Detalles("Compras").Rows.Cast<DataRow>().Count(r => r["Resultado"].ToString() == "Omitida" && r["Motivo"].ToString() == "YaEntregada"));

                // Dia siguiente: compras y movimientos se repiten; inversiones espera los siete dias.
                await Crear(generador, () => Reloj(6, 11)).EjecutarAsync(CancellationToken.None);
                Assert.AreEqual(2, e.Notificaciones("Compras").Rows.Count);
                Assert.AreEqual(2, e.Notificaciones("Movimientos").Rows.Count);
                Assert.AreEqual(1, e.Notificaciones("Inversiones").Rows.Count);

                // Exactamente siete dias despues de la ultima notificacion automatica de inversion.
                await Crear(generador, () => Reloj(6, 17, 123)).EjecutarAsync(CancellationToken.None);
                Assert.AreEqual(2, e.Notificaciones("Inversiones").Rows.Count);
            }
        }

        [TestMethod]
        public async Task Ejecutar_NotificacionDeInversionEliminada_ReiniciaElIntervaloSinError()
        {
            using (Entorno e = new Entorno(new DateTime(2099, 6, 20), new DateTime(2099, 6, 21)))
            {
                e.ConPerfil("CDCDCCDDDC");
                GeneradorFijo generador = new GeneradorFijo();
                await Crear(generador, () => Reloj(6, 20)).EjecutarAsync(CancellationToken.None);
                await Crear(generador, () => Reloj(6, 21)).EjecutarAsync(CancellationToken.None);
                Assert.AreEqual(1, e.Notificaciones("Inversiones").Rows.Count, "Menos de siete dias: se omite.");

                e.Sql("DELETE FROM Notificacion WHERE UsuarioId = @U AND TipoAutomatico = 'Inversiones'");
                await Crear(generador, () => Reloj(6, 21)).EjecutarAsync(CancellationToken.None);

                Assert.AreEqual(1, e.Notificaciones("Inversiones").Rows.Count, "Sin la notificacion no hay forma de validar: se crea otra.");
            }
        }

        // ------------------------------------------------------------------ 4.4 concurrencia y revalidacion

        [TestMethod]
        public async Task Ejecutar_DosLotesConcurrentes_DejanUnaSolaNotificacionPorUsuarioTipoYJornada()
        {
            using (Entorno e = new Entorno(new DateTime(2099, 6, 1)))
            {
                e.ConPendiente();
                using (Barrier ambosCalcularon = new Barrier(2))
                {
                    bool sincronizados = true;
                    GeneradorFijo generador = new GeneradorFijo();
                    generador.Responder = s =>
                    {
                        // Ambos lotes terminan de calcular las compras del mismo usuario antes de que alguno entregue.
                        if (s.UsuarioId == e.UsuarioId && s.Categoria == CategoriaConsulta.Compras)
                        {
                            if (!ambosCalcularon.SignalAndWait(TimeSpan.FromSeconds(15))) sincronizados = false;
                        }

                        return ResultadoGeneracion.Valida("Compra concurrente");
                    };

                    Task<ResultadoCorridaNotificaciones> a = Task.Run(() => Crear(generador, () => Reloj(6, 1)).EjecutarAsync(CancellationToken.None));
                    Task<ResultadoCorridaNotificaciones> b = Task.Run(() => Crear(generador, () => Reloj(6, 1)).EjecutarAsync(CancellationToken.None));
                    ResultadoCorridaNotificaciones[] corridas = await Task.WhenAll(a, b);

                    Assert.IsTrue(sincronizados, "Los dos lotes debian calcular antes de entregar.");
                    Assert.AreEqual(1, e.Notificaciones("Compras").Rows.Count);
                    Assert.AreEqual(1, e.Notificaciones("Movimientos").Rows.Count);
                    DataTable detalles = e.Detalles("Compras");
                    Assert.AreEqual(2, detalles.Rows.Count);
                    Assert.AreEqual(1, detalles.Rows.Cast<DataRow>().Count(r => r["Resultado"].ToString() == "Entregada"));
                    Assert.AreEqual(1, detalles.Rows.Cast<DataRow>().Count(r => r["Resultado"].ToString() == "Omitida" && r["Motivo"].ToString() == "YaEntregada"));
                    Assert.IsTrue(corridas.All(c => c.Fallidas == 0));
                    Assert.AreEqual(1, e.Detalles("Movimientos").Rows.Cast<DataRow>().Count(r => r["Resultado"].ToString() == "Entregada"));
                }
            }
        }

        [TestMethod]
        public async Task Ejecutar_CuentaDesactivadaDuranteElCalculo_NoRecibeLaNotificacion()
        {
            using (Entorno e = new Entorno(new DateTime(2099, 6, 2)))
            {
                e.ConPendiente();
                GeneradorFijo generador = new GeneradorFijo();
                generador.Responder = s =>
                {
                    if (s.UsuarioId == e.UsuarioId) e.Sql("UPDATE Usuarios SET Activo = 0 WHERE Id = @U");
                    return ResultadoGeneracion.Valida("Compra");
                };

                await Crear(generador, () => Reloj(6, 2)).EjecutarAsync(CancellationToken.None);

                Assert.AreEqual(0, e.Notificaciones("Compras").Rows.Count);
                DataRow detalle = e.Detalles("Compras").Rows.Cast<DataRow>().Single();
                Assert.AreEqual("Omitida", detalle["Resultado"]);
                Assert.AreEqual("CuentaInactiva", detalle["Motivo"]);
            }
        }

        [TestMethod]
        public async Task Ejecutar_PerfilCambiadoDuranteElCalculo_OmiteSinOcuparElIntervalo()
        {
            using (Entorno e = new Entorno(new DateTime(2099, 6, 3)))
            {
                e.ConPerfil("CDCDCCDDDC");
                GeneradorFijo generador = new GeneradorFijo();
                generador.Responder = s =>
                {
                    if (s.Categoria == CategoriaConsulta.Inversiones) e.ConPerfil("BCCCCCCCCB");
                    return ResultadoGeneracion.Valida("Inversion calculada con el perfil anterior");
                };

                await Crear(generador, () => Reloj(6, 3)).EjecutarAsync(CancellationToken.None);

                Assert.AreEqual(0, e.Notificaciones("Inversiones").Rows.Count);
                Assert.AreEqual("PerfilCambiado", e.Detalles("Inversiones").Rows.Cast<DataRow>().Single()["Motivo"]);
                Assert.IsNull(new NotificacionRepositorio().ObtenerFechaUltimaAutomaticaInversionUtc(e.UsuarioId));
            }
        }

        [TestMethod]
        public async Task Ejecutar_FallaSqlAlGuardarElDetalle_RevierteLaNotificacionYSeInformaSinExitoFalso()
        {
            using (Entorno e = new Entorno(new DateTime(2099, 6, 4)))
            {
                e.ConPerfil("CDCDCCDDDC");
                GeneradorFijo generador = new GeneradorFijo();
                generador.Responder = s =>
                {
                    // Quita el registro de la corrida mientras se calcula: el detalle de la entrega ya no tiene a quien
                    // referenciar y la transaccion falla despues de insertar el aviso.
                    if (s.Categoria == CategoriaConsulta.Inversiones && s.UsuarioId == e.UsuarioId)
                    {
                        e.Sql("DELETE FROM DetalleGeneracionNotificacion WHERE CorridaId IN (SELECT Id FROM CorridaGeneracionNotificaciones WHERE Jornada = '2099-06-04'); "
                            + "DELETE FROM CorridaGeneracionNotificaciones WHERE Jornada = '2099-06-04'");
                    }

                    return ResultadoGeneracion.Valida("Inversion");
                };

                ExcepcionTecnica ex = await Assert.ThrowsExactlyAsync<ExcepcionTecnica>(() => Crear(generador, () => Reloj(6, 4)).EjecutarAsync(CancellationToken.None));

                Assert.AreEqual("Error.Notifications.Generation.RunFailed", ex.Message);
                Assert.AreEqual(0, e.Notificaciones("Inversiones").Rows.Count, "El aviso insertado en la transaccion fallida no queda.");
                Assert.IsNull(new NotificacionRepositorio().ObtenerFechaUltimaAutomaticaInversionUtc(e.UsuarioId), "El intervalo no se ocupa.");
            }
        }

        [TestMethod]
        public async Task Ejecutar_FalloDeIAParaUnUsuario_NoImpideLasDemasRevisionesYQuedaRegistrado()
        {
            using (Entorno e = new Entorno(new DateTime(2099, 6, 5)))
            {
                e.ConPendiente();
                GeneradorFijo generador = new GeneradorFijo();
                generador.Responder = s => { if (s.Categoria == CategoriaConsulta.Compras) throw new ExcepcionTecnica("Error.Assistant.ResponseFailed"); return ResultadoGeneracion.Valida("x"); };

                ResultadoCorridaNotificaciones r = await Crear(generador, () => Reloj(6, 5)).EjecutarAsync(CancellationToken.None);

                Assert.IsTrue(r.Fallidas >= 1);
                DataRow compras = e.Detalles("Compras").Rows.Cast<DataRow>().Single();
                Assert.AreEqual("Fallida", compras["Resultado"]);
                Assert.AreEqual("FalloGeneracion", compras["Motivo"]);
                Assert.AreEqual(1, e.Notificaciones("Movimientos").Rows.Count);
                using (AccesoDatosSqlServer acceso = new AccesoDatosSqlServer())
                {
                    DataTable corrida = acceso.Read("SELECT * FROM CorridaGeneracionNotificaciones WHERE Id = @I", new List<SqlParameter> { acceso.CreateParameter("@I", SqlDbType.Int, r.CorridaId) });
                    Assert.AreEqual(r.Fallidas, (int)corrida.Rows[0]["Fallidas"]);
                    Assert.AreNotEqual(DBNull.Value, corrida.Rows[0]["FinUtc"]);
                }
            }
        }
    }
}
