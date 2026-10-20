using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ABSTRACCIONES;
using REPOSITORIO;
using BE;

namespace BLL
{
    internal sealed class RecomendacionInversionesBLL : IRecomendacionInversionesBLL
    {
        private readonly IMovimientoRepositorio _movimientos;
        private readonly Func<DateTime> _ahora;
        private readonly IProveedorArgentinaDatos _argentina;
        private readonly IProveedorMercadoData912 _data912;
        private readonly IProveedorCriptoYa _cripto;

        internal RecomendacionInversionesBLL(IMovimientoRepositorio movimientos, Func<DateTime> ahora, IProveedorArgentinaDatos argentina = null, IProveedorMercadoData912 data912 = null, IProveedorCriptoYa cripto = null)
        {
            _movimientos = movimientos;
            _ahora = ahora;
            _argentina = argentina;
            _data912 = data912;
            _cripto = cripto;
        }

        public async Task<ResumenInversiones> CalcularAsync(int usuarioActualId, decimal? porcentaje, CancellationToken cancellationToken, PerfilInversor perfil = null)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (perfil == null || perfil.UsuarioId != usuarioActualId || perfil.VersionCuestionario != CalculadorPerfilInversor.Version
                || !Enum.IsDefined(typeof(NivelRiesgoInversor), perfil.NivelRiesgo))
                throw new SERVICIOS.ExcepcionNegocio("Assistant.Investment.ProfileRequired");
            CalculadorPerfilInversor.Evaluar(perfil.VersionCuestionario, perfil.Respuestas);
            ResumenInversiones resumen = CalcularSaldo(usuarioActualId, porcentaje);
            string horizonte = perfil.Respuestas.Single(r => r.PreguntaId == 2).OpcionId;
            string liquidez = perfil.Respuestas.Single(r => r.PreguntaId == 3).OpcionId;
            resumen.Perfil = perfil.NivelRiesgo;
            resumen.FechaPerfilUtc = perfil.FechaEvaluacionUtc;
            resumen.VersionPerfil = perfil.VersionCuestionario;
            resumen.MaximoMesesInmovilizacion = horizonte == "A" ? 0 : horizonte == "B" ? 3 : horizonte == "C" ? 12 : 36;
            resumen.MaximoDiasHabilesDisponibilidad = liquidez == "A" || horizonte == "A" ? 0 : liquidez == "B" ? 1 : (int?)null;
            if (resumen.Nivel != NivelInversion.Negativo)
            {
                var maximo = resumen.Nivel == NivelInversion.Ajustado ? NivelRiesgoInversor.Bajo
                    : resumen.Nivel == NivelInversion.Comodo ? NivelRiesgoInversor.Moderado : NivelRiesgoInversor.Alto;
                resumen.RiesgoEfectivo = (NivelRiesgoInversor)Math.Min((int)perfil.NivelRiesgo, (int)maximo);
            }
            var peticiones = new List<Task<LoteInversion>>();
            peticiones.Add(ConsultarAsync("ArgentinaDatos inflacion", () => _argentina.ObtenerInflacionAsync(cancellationToken), datos => datos.OrderByDescending(d => d.Fecha).Take(1), cancellationToken));
            bool plazoPermitido = resumen.RiesgoEfectivo.HasValue && resumen.Nivel != NivelInversion.Ajustado && horizonte != "A" && liquidez == "C";
            bool altoPermitido = resumen.RiesgoEfectivo == NivelRiesgoInversor.Alto && horizonte == "D";
            if (resumen.RiesgoEfectivo.HasValue)
            {
                peticiones.Add(FondosAsync("mercadoDinero", resumen, cancellationToken));
                peticiones.Add(ConsultarAsync("ArgentinaDatos cuentas remuneradas", () => _argentina.ObtenerCuentasRemuneradasAsync(cancellationToken),
                    datos => datos.OrderByDescending(d => d.Tasa).ThenBy(d => d.Nombre, StringComparer.Ordinal).Take(1), cancellationToken, resumen));
                if (plazoPermitido)
                    peticiones.Add(ConsultarAsync("ArgentinaDatos plazo fijo", () => _argentina.ObtenerPlazosFijosAsync(cancellationToken),
                        datos => datos.OrderByDescending(d => d.Tasa).ThenBy(d => d.Nombre, StringComparer.Ordinal).Take(1), cancellationToken, resumen));
                if (resumen.RiesgoEfectivo >= NivelRiesgoInversor.Moderado)
                    peticiones.Add(FondosAsync("rentaFija", resumen, cancellationToken));
            }
            if (altoPermitido)
            {
                peticiones.Add(FondosAsync("rentaVariable", resumen, cancellationToken));
                foreach (bool cedears in new[] { true, false })
                    peticiones.Add(ConsultarAsync(cedears ? "data912 CEDEARs" : "data912 acciones", () => _data912.ObtenerCotizacionesAsync(cedears, cancellationToken),
                        datos => datos.OrderByDescending(d => d.Volumen).ThenBy(d => d.Nombre, StringComparer.Ordinal).Take(5), cancellationToken, resumen));
                foreach (string moneda in new[] { "BTC", "ETH", "USDT" })
                    peticiones.Add(ConsultarAsync("criptoya " + moneda, () => _cripto.ObtenerPreciosAsync(moneda, cancellationToken),
                        datos => datos.OrderBy(d => d.Precio).ThenBy(d => d.Nombre, StringComparer.Ordinal).Take(1), cancellationToken, resumen));
            }
            LoteInversion[] lotes = await Task.WhenAll(peticiones).ConfigureAwait(false);
            foreach (LoteInversion lote in lotes)
            {
                resumen.UniversoReferencias.AddRange(lote.Conocidos.Select(ResumenInversiones.Referencia));
                resumen.FuentesFaltantes.AddRange(lote.Faltantes);
                resumen.CandidatosSinCompatibilidad.AddRange(lote.Incompatibles);
                foreach (InstrumentoInversion instrumento in lote.Elegidos)
                {
                    if (instrumento.Dato.Tipo == "Inflacion") resumen.Inflacion = instrumento.Dato;
                    else resumen.Instrumentos.Add(instrumento);
                }
            }
            var excluidas = new List<string>();
            if (!altoPermitido) excluidas.AddRange(new[] { "acciones", "accion", "CEDEAR", "CEDEARs", "cripto", "criptomonedas", "Bitcoin", "Ethereum", "BTC", "ETH", "USDT", "FCI rentaVariable", "renta variable" });
            if (!plazoPermitido) excluidas.Add("plazo fijo");
            if (!resumen.RiesgoEfectivo.HasValue || resumen.RiesgoEfectivo == NivelRiesgoInversor.Bajo) excluidas.AddRange(new[] { "FCI rentaFija", "renta fija" });
            resumen.UniversoReferencias.AddRange(excluidas.Select(n => new ReferenciaAsistente(TipoReferenciaAsistente.Instrumento, n)));
            return resumen;
        }

        private async Task<LoteInversion> ConsultarAsync(string fuente, Func<Task<IReadOnlyList<DatoMercado>>> consultar, Func<IEnumerable<DatoMercado>, IEnumerable<DatoMercado>> seleccionar, CancellationToken ct, ResumenInversiones resumen = null)
        {
            var lote = new LoteInversion();
            try
            {
                ct.ThrowIfCancellationRequested();
                IReadOnlyList<DatoMercado> datos = await consultar().ConfigureAwait(false);
                lote.Conocidos.AddRange(datos);
                var vigentes = datos.Where(d => d.Fecha.Date <= _ahora().Date && d.Fecha.Date >= _ahora().Date.AddDays(d.Tipo == "Inflacion" ? -62 : -7)).ToList();
                if (resumen != null)
                {
                    lote.Incompatibles.AddRange(vigentes.Where(d => !Compatible(d, resumen)).Select(d => d.Nombre));
                    vigentes = vigentes.Where(d => Compatible(d, resumen)).ToList();
                }
                lote.Elegidos.AddRange(seleccionar(vigentes).Select(d => new InstrumentoInversion { Dato = d }));
                if (lote.Elegidos.Count == 0) lote.Faltantes.Add(fuente);
            }
            catch (OperationCanceledException) when (ct.IsCancellationRequested) { throw; }
            catch (SERVICIOS.ExcepcionTecnica) { lote.Faltantes.Add(fuente); }
            return lote;
        }

        private async Task<LoteInversion> FondosAsync(string tipo, ResumenInversiones resumen, CancellationToken ct)
        {
            LoteInversion lote = await ConsultarAsync("ArgentinaDatos FCI " + tipo, () => _argentina.ObtenerFondosAsync(tipo, null, ct), datos => datos, ct, resumen).ConfigureAwait(false);
            DateTime[] fechas = lote.Elegidos.Select(d => d.Dato.Fecha.Date.AddDays(-30)).Distinct().ToArray();
            // Una consulta por fecha de VCP vigente, no una por fondo.
            var historicos = await Task.WhenAll(fechas.Select(async fecha =>
            {
                try { return await _argentina.ObtenerFondosAsync(tipo, fecha, ct).ConfigureAwait(false); }
                catch (OperationCanceledException) when (ct.IsCancellationRequested) { throw; }
                catch (SERVICIOS.ExcepcionTecnica) { return null; }
            })).ConfigureAwait(false);
            if (historicos.Any(h => h == null)) lote.Faltantes.Add("ArgentinaDatos historico FCI " + tipo);
            foreach (InstrumentoInversion fondo in lote.Elegidos)
            {
                DatoMercado previo = historicos.Where(h => h != null).SelectMany(h => h)
                    .FirstOrDefault(p => p.Nombre == fondo.Dato.Nombre && p.Fecha.Date == fondo.Dato.Fecha.Date.AddDays(-30) && p.Vcp > 0);
                if (previo != null) fondo.Rendimiento30Dias = (fondo.Dato.Vcp / previo.Vcp - 1m) * 100m;
            }
            lote.Elegidos = lote.Elegidos.OrderByDescending(i => i.Rendimiento30Dias.HasValue).ThenByDescending(i => i.Rendimiento30Dias)
                .ThenBy(i => i.Dato.Nombre, StringComparer.Ordinal).Take(3).ToList();
            return lote;
        }

        private sealed class LoteInversion
        {
            internal List<DatoMercado> Conocidos = new List<DatoMercado>();
            internal List<InstrumentoInversion> Elegidos = new List<InstrumentoInversion>();
            internal List<string> Faltantes = new List<string>();
            internal List<string> Incompatibles = new List<string>();
        }

        private bool Compatible(DatoMercado dato, ResumenInversiones resumen)
        {
            if (string.IsNullOrWhiteSpace(dato.FuenteCondiciones) || !dato.MesesInmovilizacion.HasValue
                || !dato.DiasHabilesDisponibilidad.HasValue || dato.RequierePreaviso != false || dato.RequierePermanencia != false)
                return false;
            if (dato.MesesInmovilizacion < 0 || dato.DiasHabilesDisponibilidad < 0
                || dato.MesesInmovilizacion > resumen.MaximoMesesInmovilizacion) return false;
            if (resumen.MaximoDiasHabilesDisponibilidad.HasValue)
                return dato.MesesInmovilizacion == 0 && dato.DiasHabilesDisponibilidad <= resumen.MaximoDiasHabilesDisponibilidad;
            if (dato.DiasHabilesDisponibilidad == 0) return true;
            // No asumir un calendario de feriados ni convertir dias habiles en meses.
            // El proveedor puede acreditar un limite contractual fechado para ese caso.
            return dato.FechaDisponibilidadMaxima.HasValue && dato.FechaDisponibilidadMaxima.Value >= _ahora()
                && dato.FechaDisponibilidadMaxima.Value <= _ahora().AddMonths(resumen.MaximoMesesInmovilizacion);
        }

        private static decimal? CalcularMonto(decimal saldo, decimal? porcentaje)
        {
            return porcentaje.HasValue ? Math.Round(Math.Max(0, saldo) * porcentaje.Value / 100m, 2, MidpointRounding.AwayFromZero) : (decimal?)null;
        }

        // El porcentaje es opcional: null conserva saldo, nivel y seleccion sin porcentaje ni
        // monto. Si viene, debe estar entre 0 (excluido) y 100; el monto es un calculo aparte.
        public ResumenInversiones CalcularSaldo(int usuarioActualId, decimal? porcentaje)
        {
            if (porcentaje.HasValue && (porcentaje.Value <= 0 || porcentaje.Value > 100)) throw new ArgumentOutOfRangeException(nameof(porcentaje));
            DateTime actual = _ahora();
            DateTime finExclusivo = new DateTime(actual.Year, actual.Month, 1);
            DateTime desde = finExclusivo.AddMonths(-3);
            var meses = _movimientos.ObtenerTotalesPorMes(usuarioActualId, desde, finExclusivo.AddDays(-1))
                .Where(m => new DateTime(m.Anio, m.Mes, 1) >= desde && new DateTime(m.Anio, m.Mes, 1) < finExclusivo).ToList();
            decimal ingresos = meses.Sum(m => m.TotalIngresos) / 3m;
            decimal gastos = meses.Sum(m => m.TotalEgresos) / 3m;
            decimal saldo = ingresos - gastos;
            decimal tasa = ingresos > 0 ? saldo / ingresos : 0;
            return new ResumenInversiones
            {
                SaldoMensual = saldo,
                GastoMensual = gastos,
                Porcentaje = porcentaje,
                Monto = CalcularMonto(saldo, porcentaje),
                Nivel = saldo <= 0 ? NivelInversion.Negativo : tasa < 0.10m ? NivelInversion.Ajustado : tasa <= 0.25m ? NivelInversion.Comodo : NivelInversion.Alto
            };
        }
    }
}
