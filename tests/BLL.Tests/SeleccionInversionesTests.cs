using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ABSTRACCIONES;
using BLL;
using BLL.Tests.TestDoubles;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SERVICIOS;

namespace BLL.Tests
{
    [TestClass]
    public sealed class SeleccionInversionesTests
    {
        [TestMethod]
        [DataRow(1000, "inflacion")]
        [DataRow(950, "mercadoDinero,cuentas,inflacion")]
        [DataRow(850, "mercadoDinero,cuentas,rentaFija,plazos,inflacion")]
        [DataRow(500, "mercadoDinero,cuentas,rentaFija,plazos,rentaVariable,cedears,acciones,BTC,ETH,USDT,inflacion")]
        public async Task CalcularAsync_Nivel_ConsultaSoloTiposDelNivel(int gasto, string fuentes)
        {
            InversionFixture f = new InversionFixture(gasto);
            ResumenInversiones r = await f.Crear().CalcularAsync(7, 20, CancellationToken.None, PerfilAsistenteTests.Perfil());
            CollectionAssert.AreEquivalent(fuentes.Split(','), f.Mercado.Llamadas.Where(x => !x.StartsWith("historico:")).ToArray());
            if (gasto == 1000) Assert.IsEmpty(r.Instrumentos);
            else Assert.IsNotEmpty(r.Instrumentos);
        }

        [TestMethod]
        public async Task CalcularAsync_SinPorcentaje_SeleccionaLosMismosInstrumentosSinPorcentajeNiMonto()
        {
            InversionFixture conPorcentaje = new InversionFixture(500);
            ResumenInversiones esperado = await conPorcentaje.Crear().CalcularAsync(7, 20, CancellationToken.None, PerfilAsistenteTests.Perfil());
            InversionFixture sinPorcentaje = new InversionFixture(500);
            ResumenInversiones r = await sinPorcentaje.Crear().CalcularAsync(7, null, CancellationToken.None, PerfilAsistenteTests.Perfil());
            Assert.IsNull(r.Porcentaje);
            Assert.IsNull(r.Monto);
            Assert.AreEqual(esperado.Nivel, r.Nivel);
            Assert.AreEqual(esperado.RiesgoEfectivo, r.RiesgoEfectivo);
            CollectionAssert.AreEqual(esperado.Instrumentos.Select(i => i.Dato.Nombre).ToArray(), r.Instrumentos.Select(i => i.Dato.Nombre).ToArray());
            Assert.IsNotEmpty(r.Instrumentos);
        }

        [TestMethod]
        public async Task CalcularAsync_Fci_OrdenaPorRendimientoReal30DiasYNoInventaPrevios()
        {
            InversionFixture f = new InversionFixture(850);
            f.Mercado.Fondos = Enumerable.Range(1, 5).Select(i => Fondo("Fondo " + i, 100 + i, new DateTime(2026, 10, 16))).ToList();
            f.Mercado.Fondos.Add(Fondo("Sin previo", 100000, new DateTime(2026, 10, 16)));
            f.Mercado.Fondos.Add(Fondo("Viejo", 999999, new DateTime(2020, 1, 1)));
            f.Mercado.Previos = Enumerable.Range(1, 5).Select(i => Fondo("Fondo " + i, 100, new DateTime(2026, 9, 16))).Reverse().ToList();
            ResumenInversiones r = await f.Crear().CalcularAsync(7, 20, CancellationToken.None, PerfilAsistenteTests.Perfil());
            var fondos = r.Instrumentos.Where(i => i.Dato.Tipo.StartsWith("FCI")).ToList();
            CollectionAssert.AreEqual(new[] { "Fondo 5", "Fondo 4", "Fondo 3" }, fondos.Select(i => i.Dato.Nombre).ToArray());
            Assert.AreEqual(5m, fondos[0].Rendimiento30Dias);
            Assert.Contains("historico:2026-09-16", f.Mercado.Llamadas);
            Assert.IsFalse(f.Mercado.Llamadas.Any(x => x == "historico:2019-12-02"));
            Assert.Contains("Viejo", r.UniversoReferencias.Select(x => x.Nombre).ToList());
        }

        [TestMethod]
        public async Task CalcularAsync_Alto_TopCincoPorVolumenYMenorPrecioTotalPorCripto()
        {
            InversionFixture f = new InversionFixture(500);
            f.Mercado.Cotizaciones = Enumerable.Range(1, 8).Select(i => new DatoMercado { Nombre = "Ticker" + i, Volumen = i, Precio = 100 - i, Fecha = f.Hoy, Tipo = "Accion", MesesInmovilizacion = 0, DiasHabilesDisponibilidad = 0, RequierePreaviso = false, RequierePermanencia = false, FuenteCondiciones = "Fixture v1" }).ToList();
            ResumenInversiones r = await f.Crear().CalcularAsync(7, 20, CancellationToken.None, PerfilAsistenteTests.Perfil());
            var acciones = r.Instrumentos.Where(x => x.Dato.Tipo == "Accion").ToList();
            // El fake devuelve los mismos tickers para ambas especies: cinco de cada consulta.
            Assert.AreEqual(10, acciones.Count);
            Assert.IsTrue(acciones.All(x => x.Dato.Volumen >= 4));
            Assert.AreEqual(3, r.Instrumentos.Count(x => x.Dato.Tipo.StartsWith("Cripto")));
            Assert.IsTrue(r.Instrumentos.Where(x => x.Dato.Tipo.StartsWith("Cripto")).All(x => x.Dato.Precio == 10));
        }

        [TestMethod]
        public async Task CalcularAsync_HistoricoCaido_MantieneFondosSinRendimientoYReportaFaltante()
        {
            InversionFixture f = new InversionFixture(850);
            f.Mercado.Fallas.Add("historico");
            ResumenInversiones r = await f.Crear().CalcularAsync(7, 20, CancellationToken.None, PerfilAsistenteTests.Perfil());
            Assert.IsTrue(r.Instrumentos.Any(x => x.Dato.Tipo.StartsWith("FCI") && !x.Rendimiento30Dias.HasValue));
            Assert.IsNotEmpty(r.FuentesFaltantes);
        }

        [TestMethod]
        public async Task CalcularAsync_FuentesCaidas_ConservaNivelMontoYFuentesDisponibles()
        {
            InversionFixture f = new InversionFixture(500);
            f.Mercado.Fallas.Add("BTC");
            ResumenInversiones parcial = await f.Crear().CalcularAsync(7, 20, CancellationToken.None, PerfilAsistenteTests.Perfil());
            Assert.IsFalse(parcial.Instrumentos.Any(x => x.Dato.Tipo == "Cripto BTC"));
            Assert.IsTrue(parcial.Instrumentos.Any(x => x.Dato.Tipo == "Cripto ETH"));
            Assert.Contains("criptoya BTC", parcial.FuentesFaltantes);
            f.Mercado.FallaTodo = true;
            ResumenInversiones vacio = await f.Crear().CalcularAsync(7, 20, CancellationToken.None, PerfilAsistenteTests.Perfil());
            Assert.IsEmpty(vacio.Instrumentos);
            Assert.AreEqual(NivelInversion.Alto, vacio.Nivel);
            Assert.AreEqual(100m, vacio.Monto);
            Assert.IsNotEmpty(vacio.FuentesFaltantes);
        }

        [TestMethod]
        public async Task CalcularAsync_Cancelacion_SePropaga()
        {
            await Assert.ThrowsAsync<OperationCanceledException>(() => new InversionFixture(500).Crear().CalcularAsync(7, 20, new CancellationToken(true), PerfilAsistenteTests.Perfil()));
        }

        private static DatoMercado Fondo(string nombre, decimal vcp, DateTime fecha) => new DatoMercado { Nombre = nombre, Vcp = vcp, Fecha = fecha, Tipo = "FCI rentaFija", Fuente = "ArgentinaDatos", MesesInmovilizacion = 0, DiasHabilesDisponibilidad = 0, RequierePreaviso = false, RequierePermanencia = false, FuenteCondiciones = "Fixture v1" };
    }

    internal sealed class InversionFixture
    {
        internal DateTime Hoy = new DateTime(2026, 10, 19);
        internal FakeMovimientoRepositorio Repo = new FakeMovimientoRepositorio();
        internal FakeMercado Mercado = new FakeMercado();
        internal InversionFixture(int gasto)
        {
            Repo.ObtenerTotalesPorMesResult.Add(new TotalPorMes { Anio = 2026, Mes = 9, TotalIngresos = 3000, TotalEgresos = gasto * 3 });
        }
        internal RecomendacionInversionesBLL Crear() => new RecomendacionInversionesBLL(Repo, () => Hoy, Mercado, Mercado, Mercado);
    }

    internal sealed class FakeMercado : IProveedorMercadoData912, IProveedorArgentinaDatos, IProveedorCriptoYa
    {
        internal List<string> Llamadas = new List<string>();
        internal HashSet<string> Fallas = new HashSet<string>();
        internal bool FallaTodo;
        internal List<DatoMercado> Fondos;
        internal List<DatoMercado> Previos;
        internal List<DatoMercado> Cotizaciones;
        private Task<IReadOnlyList<DatoMercado>> Responder(string clave, List<DatoMercado> datos)
        {
            Llamadas.Add(clave);
            if (FallaTodo || Fallas.Contains(clave) || (clave.StartsWith("historico:") && Fallas.Contains("historico"))) return Task.FromException<IReadOnlyList<DatoMercado>>(new ExcepcionTecnica("fuente"));
            return Task.FromResult<IReadOnlyList<DatoMercado>>(datos);
        }
        private DatoMercado Dato(string tipo) => new DatoMercado { Nombre = tipo + " conocido", Tipo = tipo, Fecha = new DateTime(2026, 10, 19), Precio = 1, Volumen = 1, Vcp = 100, Tasa = 20, MesesInmovilizacion = 0, DiasHabilesDisponibilidad = 0, RequierePreaviso = false, RequierePermanencia = false, FuenteCondiciones = "Fixture v1" };
        public Task<IReadOnlyList<DatoMercado>> ObtenerFondosAsync(string tipo, DateTime? fecha, CancellationToken ct) => Responder(fecha.HasValue ? "historico:" + fecha.Value.ToString("yyyy-MM-dd") : tipo, fecha.HasValue ? Previos ?? new List<DatoMercado>() : Fondos == null ? new List<DatoMercado> { Dato("FCI " + tipo) } : Fondos.Where(d => d.Tipo == "FCI " + tipo).ToList());
        public Task<IReadOnlyList<DatoMercado>> ObtenerCotizacionesAsync(bool cedears, CancellationToken ct) => Responder(cedears ? "cedears" : "acciones", Cotizaciones ?? new List<DatoMercado> { Dato(cedears ? "CEDEAR" : "Accion") });
        public Task<IReadOnlyList<DatoMercado>> ObtenerPreciosAsync(string moneda, CancellationToken ct)
        {
            var caro = Dato("Cripto " + moneda); caro.Nombre = moneda + " caro"; caro.Precio = 20;
            var barato = Dato("Cripto " + moneda); barato.Nombre = moneda + " barato"; barato.Precio = 10;
            return Responder(moneda, new List<DatoMercado> { caro, barato });
        }
        public Task<IReadOnlyList<DatoMercado>> ObtenerPlazosFijosAsync(CancellationToken ct) => Responder("plazos", new List<DatoMercado> { Dato("Plazo fijo") });
        public Task<IReadOnlyList<DatoMercado>> ObtenerCuentasRemuneradasAsync(CancellationToken ct) => Responder("cuentas", new List<DatoMercado> { Dato("Cuenta remunerada") });
        public Task<IReadOnlyList<DatoMercado>> ObtenerInflacionAsync(CancellationToken ct) => Responder("inflacion", new List<DatoMercado> { Dato("Inflacion") });
    }
}
