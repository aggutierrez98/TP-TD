using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using ABSTRACCIONES;
using BE;
using BLL.Tests.TestDoubles;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using REPOSITORIO;

namespace BLL.Tests
{
    [TestClass]
    public sealed class RecomendacionComprasBLLTests
    {
        [TestMethod]
        public async Task CalcularAsync_ConZonas_SoloCercanosYOnlineConDistanciaRedondeada()
        {
            ComprasFixture f = new ComprasFixture();
            f.Zonas.Zonas[1] = new ZonaUsuario { Id = 1, Usuario = new Usuario { Id = 7 }, Nombre = "Trabajo", RadioMetros = 1000 };
            f.Oferta(1, "Cerca", 100, 600m / 111194.93m);
            f.Oferta(2, "Lejos", 10, 1m);
            f.Oferta(3, "Online", 120, null);
            ResumenCompras resultado = await f.CalcularAsync();
            CollectionAssert.AreEquivalent(new[] { "Cerca", "Online" }, resultado.Items.Single().Ofertas.Select(o => o.Comercio).ToArray());
            OfertaCompra cerca = resultado.Items.Single().Ofertas.Single(o => o.Comercio == "Cerca");
            Assert.AreEqual("Trabajo", cerca.Zona);
            Assert.AreEqual(600, cerca.DistanciaMetros);
            Assert.IsTrue(resultado.Items.Single().Ofertas.Single(o => o.Comercio == "Online").SinUbicacionFisica);
        }

        [TestMethod]
        public async Task CalcularAsync_SinZonas_NoFiltraYSugiereCargarlas()
        {
            ComprasFixture f = new ComprasFixture();
            f.Oferta(1, "Lejos", 100, 1m);
            ResumenCompras r = await f.CalcularAsync();
            Assert.IsTrue(r.SinZonas);
            Assert.AreEqual(1, r.Items.Single().Ofertas.Count);
        }

        [TestMethod]
        public async Task CalcularAsync_PromocionAplicable_RespetaDiasVigenciaCuentaYTopePorCantidad()
        {
            ComprasFixture f = new ComprasFixture();
            f.Repo.Items[0].Cantidad = 2;
            f.Oferta(1, "Super", 1000);
            Promocion promo = f.Promocion();
            promo.Descuento = 20;
            promo.TopeDescuento = 300;
            promo.DiasSemana.Add(DiaSemana.Jueves);
            promo.VigenciaHasta = ComprasFixture.Hoy.AddDays(4);
            ResumenCompras r = await f.CalcularAsync();
            PromocionCompra p = r.Items.Single().Ofertas.Single().Promociones.Single();
            Assert.AreEqual(new DateTime(2026, 10, 22), p.Fecha);
            Assert.AreEqual("Mi cuenta", p.Cuenta);
            Assert.AreEqual("Visa", p.MedioPago);
            Assert.AreEqual(300m, p.Ahorro);
            Assert.AreEqual(1700m, p.PrecioFinal);
        }

        [TestMethod]
        public async Task CalcularAsync_PromocionesNoAplicables_NoLasIncluye()
        {
            ComprasFixture f = new ComprasFixture();
            f.Oferta(1, "Super", 100);
            f.Promocion().EntidadesFinancieras.Add(new EntidadFinanciera { Id = 999 });
            f.Promocion().ProductosExcluidos.Add(new Producto { Id = 1 });
            f.Promocion().Comercios.Add(new Comercio { Id = 999 });
            f.Promocion().VigenciaDesde = ComprasFixture.Hoy.AddDays(7);
            f.Promocion().Segmentos.Add("Otro segmento");
            ResumenCompras r = await f.CalcularAsync();
            Assert.AreEqual(0, r.Items.Single().Ofertas.Single().Promociones.Count);
        }

        [TestMethod]
        public async Task CalcularAsync_PromocionTodosLosDias_SoloProximosSiete()
        {
            ComprasFixture f = new ComprasFixture();
            f.Oferta(1, "Super", 100);
            f.Promocion();
            ResumenCompras r = await f.CalcularAsync();
            CollectionAssert.AreEqual(Enumerable.Range(0, 7).Select(n => ComprasFixture.Hoy.AddDays(n)).ToArray(), r.Items.Single().Ofertas.Single().Promociones.Select(p => p.Fecha).ToArray());
        }

        [TestMethod]
        public async Task CalcularAsync_ItemsSinProductoOVenta_LosInformaSinInventarOfertas()
        {
            ComprasFixture f = new ComprasFixture();
            f.Repo.Items.Add(new ItemLista { Id = 2, Descripcion = "teletransportador", ListaDeCompras = f.Repo.Items[0].ListaDeCompras, Cantidad = 1 });
            ResumenCompras r = await f.CalcularAsync();
            Assert.AreEqual(2, r.Items.Count);
            Assert.IsTrue(r.Items.All(i => i.NoEncontrado && i.Ofertas.Count == 0));
        }

        [TestMethod]
        public async Task CalcularAsync_MuchosItemsYComercios_LimitaVeinteItemsYTresComerciosPorItem()
        {
            ComprasFixture f = new ComprasFixture();
            for (int i = 2; i <= 25; i++) f.Repo.Items.Add(new ItemLista { Id = i, Descripcion = "Cerveza", Cantidad = 1, ListaDeCompras = f.Repo.Items[0].ListaDeCompras });
            for (int i = 1; i <= 5; i++) f.Oferta(i, "Super " + i, 100 + i);
            ResumenCompras r = await f.CalcularAsync();
            Assert.AreEqual(20, r.Items.Count);
            Assert.AreEqual(5, r.ItemsOmitidos);
            Assert.IsTrue(r.Items.All(item => item.Ofertas.Count == 3));
            CollectionAssert.AreEqual(new[] { "Super 1", "Super 2", "Super 3" }, r.Items[0].Ofertas.Select(o => o.Comercio).ToArray());
        }

        [TestMethod]
        public async Task CalcularAsync_ListasZonasYCuentasAjenas_NoSeIncluyenNiCredenciales()
        {
            ComprasFixture f = new ComprasFixture();
            f.Oferta(1, "Super", 100);
            f.Repo.Items.Add(new ItemLista { Id = 2, Descripcion = "Secreto ajeno", ListaDeCompras = new ListaDeCompras { Usuario = new Usuario { Id = 8 } } });
            f.Repo.Cuentas.Add(new CuentaFinanciera { Usuario = new Usuario { Id = 8 }, Alias = "Cuenta ajena", CredencialesCifradas = "SECRETO" });
            f.Repo.Cuentas[0].CredencialesCifradas = "CREDENCIAL";
            f.Zonas.Zonas[2] = new ZonaUsuario { Id = 2, Usuario = new Usuario { Id = 8 }, Nombre = "Zona ajena" };
            f.Promocion();
            ResumenCompras r = await f.CalcularAsync();
            string json = r.CrearContexto().DatosJson;
            Assert.DoesNotContain("ajena", json);
            Assert.DoesNotContain("ajeno", json);
            Assert.DoesNotContain("CREDENCIAL", json);
            Assert.DoesNotContain("Credenciales", json);
            Assert.AreEqual(7, f.Repo.UsuarioItems);
            Assert.AreEqual(7, f.Repo.UsuarioCuentas);
        }

        [TestMethod]
        public async Task CalcularAsync_NombreLista_IgnoraAcentosYMayusculas()
        {
            ComprasFixture f = new ComprasFixture();
            f.Repo.Items.Add(new ItemLista { Id = 2, Descripcion = "Otro item", ListaDeCompras = new ListaDeCompras { Nombre = "Otra", Usuario = new Usuario { Id = 7 } } });
            ResumenCompras r = await f.CalcularAsync("SEMANAL");
            Assert.AreEqual(1, r.Items.Count);
            Assert.AreEqual("Cerveza", r.Items[0].Descripcion);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public async Task CalcularAsync_PrecioUSD_ConvierteConTasaDirectaOInversaYAplicaTope(bool inversa)
        {
            ComprasFixture f = new ComprasFixture();
            f.Oferta(1, "Super USD", 100, moneda: "USD");
            f.Tasas.Tasas[1] = new TasaConversion { Id = 1, MonedaBase = new Moneda { Codigo = inversa ? "ARS" : "USD" }, MonedaConversion = new Moneda { Codigo = inversa ? "USD" : "ARS" }, CalculoConversion = inversa ? 0.001m : 1000m, FechaActualizacion = ComprasFixture.Hoy.AddDays(-1) };
            Promocion promo = f.Promocion();
            promo.TopeDescuento = 10000m;
            ResumenCompras r = await f.CalcularAsync();
            OfertaCompra oferta = r.Items.Single().Ofertas.Single();
            Assert.AreEqual(1000m, oferta.TasaAArs);
            Assert.AreEqual(ComprasFixture.Hoy.AddDays(-1), oferta.FechaTasa);
            Assert.AreEqual(10m, oferta.Promociones[0].Ahorro);
            Assert.AreEqual(90m, oferta.Promociones[0].PrecioFinal);
            Assert.AreEqual(90000m, oferta.PrecioFinalArs);
        }

        [TestMethod]
        public async Task CalcularAsync_SinTasa_NoInventaAhorroYOrdenaDetrasDeComparables()
        {
            ComprasFixture f = new ComprasFixture();
            f.Oferta(1, "USD sin tasa", 1, moneda: "USD");
            f.Oferta(2, "ARS", 100);
            f.Promocion();
            ResumenCompras r = await f.CalcularAsync();
            OfertaCompra oferta = r.Items.Single().Ofertas.Last();
            Assert.AreEqual("USD sin tasa", oferta.Comercio);
            Assert.IsTrue(oferta.SinTasa);
            Assert.IsNull(oferta.PrecioFinalArs);
            Assert.IsNull(oferta.Promociones[0].Ahorro);
            Assert.IsNull(oferta.Promociones[0].PrecioFinal);
        }

        [TestMethod]
        public async Task CalcularAsync_TopeConvertidoConDecimales_ElRedondeoNoSuperaElTopeEnArs()
        {
            ComprasFixture f = new ComprasFixture();
            f.Oferta(1, "Super USD", 100, moneda: "USD");
            f.Tasas.Tasas[1] = new TasaConversion { MonedaBase = new Moneda { Codigo = "USD" }, MonedaConversion = new Moneda { Codigo = "ARS" }, CalculoConversion = 1500 };
            f.Promocion().TopeDescuento = 10000m;
            ResumenCompras r = await f.CalcularAsync();
            decimal ahorro = r.Items.Single().Ofertas.Single().Promociones[0].Ahorro.Value;
            Assert.IsTrue(ahorro * 1500 <= 10000m, "El ahorro prometido no puede exceder el tope al expresarlo en otra moneda.");
            Assert.AreEqual(6.66m, ahorro);
        }

        [TestMethod]
        public async Task CalcularAsync_PreciosDistintasMonedas_OrdenaPorEquivalenteArs()
        {
            ComprasFixture f = new ComprasFixture();
            f.Oferta(1, "USD caro", 100, moneda: "USD");
            f.Oferta(2, "ARS barato", 50000);
            f.Tasas.Tasas[1] = new TasaConversion { MonedaBase = new Moneda { Codigo = "USD" }, MonedaConversion = new Moneda { Codigo = "ARS" }, CalculoConversion = 1000 };
            ResumenCompras r = await f.CalcularAsync();
            Assert.AreEqual("ARS barato", r.Items[0].Ofertas[0].Comercio);
        }

        [TestMethod]
        public async Task CalcularAsync_Contexto_SoloAutorizaReferenciasDelResumen()
        {
            ComprasFixture f = new ComprasFixture();
            f.Oferta(1, "Permitido", 100);
            f.Promocion();
            ResumenCompras r = await f.CalcularAsync();
            Assert.IsTrue(r.CrearContexto().ReferenciasPermitidas.Any(x => x.Nombre == "Permitido"));
            Assert.IsTrue(r.CrearContexto().ReferenciasPermitidas.Any(x => x.Nombre == "Mi cuenta"));
            using (JsonDocument json = JsonDocument.Parse(r.CrearContexto().DatosJson)) Assert.IsNotNull(json);
        }
    }

    internal sealed class ComprasFixture
    {
        internal static readonly DateTime Hoy = new DateTime(2026, 10, 19);
        internal readonly FakeComprasRepositorio Repo = new FakeComprasRepositorio();
        internal readonly FakeZonaUsuarioRepositorio Zonas = new FakeZonaUsuarioRepositorio();
        internal readonly FakeUsuarioRepositorio Usuarios = new FakeUsuarioRepositorio();
        internal readonly FakeTasaConversionRepositorio Tasas = new FakeTasaConversionRepositorio();
        internal readonly FakeSesion Sesion = new FakeSesion();
        internal readonly FakeServicioEmbeddings Embeddings = new FakeServicioEmbeddings();
        internal ComprasFixture()
        {
            Sesion.IniciarSesion(new Usuario { Id = 7 });
            Usuarios.Usuarios.Add(7, new Usuario { Id = 7 });
            Repo.Items.Add(new ItemLista { Id = 1, Descripcion = "Cerveza", Cantidad = 1, ListaDeCompras = new ListaDeCompras { Nombre = "Semanal", Usuario = new Usuario { Id = 7 } } });
            Repo.Productos.Add(new Producto { Id = 1, Nombre = "Cerveza", CategoriaProducto = new CategoriaProducto { Id = 1, Nombre = "Bebidas" } });
            Repo.Cuentas.Add(new CuentaFinanciera { Id = 1, Usuario = new Usuario { Id = 7 }, Alias = "Mi cuenta", EntidadFinanciera = new EntidadFinanciera { Id = 1 }, MediosPagoEntidad = new List<MedioPagoEntidad> { new MedioPagoEntidad { MedioPago = new MedioPago { Id = 1, Marca = "Visa" } } } });
        }
        internal void Oferta(int id, string nombre, decimal precio, decimal? latitud = null, string moneda = "ARS")
        {
            Repo.Ofertas.Add(new ComercioProducto { Comercio = new Comercio { Id = id, Nombre = nombre, Latitud = latitud, Longitud = latitud.HasValue ? 0m : (decimal?)null }, Producto = Repo.Productos[0], Precio = precio, Moneda = new Moneda { Codigo = moneda } });
        }
        internal Promocion Promocion()
        {
            Promocion p = new Promocion { Id = Repo.Promociones.Count + 1, Descripcion = "Ahorro semanal", Descuento = 20, VigenciaDesde = Hoy, VigenciaHasta = Hoy.AddDays(20) };
            Repo.Promociones.Add(p);
            return p;
        }
        internal RecomendacionComprasBLL Crear()
        {
            return new RecomendacionComprasBLL(Repo, Zonas, Usuarios, Tasas, new CoincidenciaSemanticaProductos(Embeddings, new FakeProductoEmbeddingRepositorio(), new FakeUnidadDeTrabajoFactory(), () => Hoy), () => Hoy);
        }
        internal Task<ResumenCompras> CalcularAsync(string lista = null) => Crear().CalcularAsync(new Usuario { Id = 7 }, lista, CancellationToken.None);
    }

    internal sealed class FakeComprasRepositorio : IRecomendacionComprasRepositorio
    {
        internal readonly List<ItemLista> Items = new List<ItemLista>();
        internal readonly List<Producto> Productos = new List<Producto>();
        internal readonly List<ComercioProducto> Ofertas = new List<ComercioProducto>();
        internal readonly List<Promocion> Promociones = new List<Promocion>();
        internal readonly List<CuentaFinanciera> Cuentas = new List<CuentaFinanciera>();
        internal int UsuarioItems, UsuarioCuentas;
        public List<ItemLista> ObtenerItemsPendientesPorUsuario(Usuario usuario) { UsuarioItems = usuario.Id; return Items; }
        public List<Producto> ObtenerProductosConCategoria() => Productos;
        public List<ComercioProducto> ObtenerOfertas() => Ofertas;
        public List<Promocion> ObtenerPromocionesVigentesEntre(DateTime desde, DateTime hasta) => Promociones;
        public List<CuentaFinanciera> ObtenerCuentasConMediosPorUsuario(Usuario usuario) { UsuarioCuentas = usuario.Id; return Cuentas; }
    }
}
