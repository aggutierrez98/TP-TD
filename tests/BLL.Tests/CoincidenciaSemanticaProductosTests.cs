using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BE;
using BLL.Tests.TestDoubles;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BLL.Tests
{
    [TestClass]
    public sealed class CoincidenciaSemanticaProductosTests
    {
        private static readonly DateTime Ahora = new DateTime(2026, 10, 19, 9, 0, 0);

        [TestMethod]
        public async Task BuscarAsync_SinonimoSobreElUmbral_CoincideConElProducto()
        {
            Escenario escenario = new Escenario();
            Producto cerveza = escenario.AgregarProducto(155, "Cerveza rubia Quilmes", "Bebidas alcohólicas", 1f, 0f);
            escenario.AgregarProducto(91, "Lavandina Ayudín", "Limpieza", 0f, 1f);
            escenario.Servicio.Vectores["birra"] = Vector(0.95f, 0.31f);

            Dictionary<int, List<CoincidenciaProducto>> resultado = await escenario.BuscarAsync(Item(1, "birra"));

            CoincidenciaProducto coincidencia = resultado[1].Single();
            Assert.AreEqual(cerveza.Id, coincidencia.Producto.Id);
            Assert.IsTrue(coincidencia.EsSemantica);
            Assert.IsTrue(coincidencia.Puntaje >= CoincidenciaSemanticaProductos.UmbralSemantico);
        }

        [TestMethod]
        public async Task BuscarAsync_CosenoBajoElUmbral_NoCoincide()
        {
            Escenario escenario = new Escenario();
            escenario.AgregarProducto(155, "Cerveza rubia Quilmes", "Bebidas alcohólicas", 1f, 0f);
            escenario.Servicio.Vectores["fernet"] = Vector(0.83f, 0.5577f);

            Dictionary<int, List<CoincidenciaProducto>> resultado = await escenario.BuscarAsync(Item(1, "fernet"));

            Assert.AreEqual(0, resultado[1].Count);
        }

        [TestMethod]
        public void Coseno_VectoresConocidos_CalculaElCosenoSinDependerDeLaNorma()
        {
            Assert.AreEqual(1.0, CoincidenciaSemanticaProductos.Coseno(new[] { 2f, 0f }, new[] { 5f, 0f }), 1e-6);
            Assert.AreEqual(0.0, CoincidenciaSemanticaProductos.Coseno(new[] { 1f, 0f }, new[] { 0f, 3f }), 1e-6);
            Assert.AreEqual(0.0, CoincidenciaSemanticaProductos.Coseno(new[] { 1f, 0f }, new[] { 1f, 0f, 0f }), 1e-6);
            Assert.AreEqual(0.85, CoincidenciaSemanticaProductos.UmbralSemantico, 1e-9);
        }

        [TestMethod]
        public async Task BuscarAsync_ProductoSinVector_LoCalculaLoGuardaYLoCompara()
        {
            Escenario escenario = new Escenario();
            Producto cerveza = escenario.AgregarProductoSinVector(155, "Cerveza rubia Quilmes", "Bebidas alcohólicas");
            escenario.Servicio.Vectores["Cerveza rubia Quilmes Bebidas alcohólicas"] = Vector(1f, 0f);
            escenario.Servicio.Vectores["birra"] = Vector(0.95f, 0.31f);

            Dictionary<int, List<CoincidenciaProducto>> resultado = await escenario.BuscarAsync(Item(1, "birra"));

            Assert.AreEqual(cerveza.Id, resultado[1].Single().Producto.Id);
            ProductoEmbedding guardado = escenario.Unidad.ProductoEmbeddingRepo.Guardados.Single();
            Assert.AreEqual(155, guardado.Producto.Id);
            Assert.AreSame(cerveza, guardado.Producto);
            Assert.AreEqual(FakeServicioEmbeddings.ModeloFalso, guardado.Modelo);
            Assert.AreEqual("Cerveza rubia Quilmes Bebidas alcohólicas", guardado.TextoFuente);
            Assert.AreEqual(Ahora, guardado.FechaCalculo);
            Assert.IsTrue(escenario.Unidad.SaveCalled);
        }

        [TestMethod]
        public async Task BuscarAsync_ProductoRenombrado_RecalculaAntesDeCompararYNoUsaElVectorViejo()
        {
            Escenario escenario = new Escenario();
            Producto producto = escenario.AgregarProducto(155, "Cerveza rubia Quilmes", "Bebidas alcohólicas", 1f, 0f);
            producto.Nombre = "Lavandina Ayudín";
            escenario.Servicio.Vectores["Lavandina Ayudín Bebidas alcohólicas"] = Vector(0f, 1f);
            escenario.Servicio.Vectores["birra"] = Vector(0.95f, 0.31f);

            Dictionary<int, List<CoincidenciaProducto>> resultado = await escenario.BuscarAsync(Item(1, "birra"));

            Assert.AreEqual(0, resultado[1].Count);
            CollectionAssert.Contains(escenario.Servicio.TextosRecibidos.ToList(), "Lavandina Ayudín Bebidas alcohólicas");
            Assert.AreEqual("Lavandina Ayudín Bebidas alcohólicas", escenario.Unidad.ProductoEmbeddingRepo.Guardados.Single().TextoFuente);
        }

        [TestMethod]
        public async Task BuscarAsync_VectorDeOtroModelo_LoRecalcula()
        {
            Escenario escenario = new Escenario();
            escenario.AgregarProducto(155, "Cerveza rubia Quilmes", "Bebidas alcohólicas", 1f, 0f, modelo: "modelo-anterior");
            escenario.Servicio.Vectores["Cerveza rubia Quilmes Bebidas alcohólicas"] = Vector(1f, 0f);
            escenario.Servicio.Vectores["birra"] = Vector(0.95f, 0.31f);

            await escenario.BuscarAsync(Item(1, "birra"));

            Assert.AreEqual(FakeServicioEmbeddings.ModeloFalso, escenario.Unidad.ProductoEmbeddingRepo.Guardados.Single().Modelo);
        }

        [TestMethod]
        public async Task BuscarAsync_VectoresVigentes_SoloCalculaLosItemsYNoGuarda()
        {
            Escenario escenario = new Escenario();
            escenario.AgregarProducto(155, "Cerveza rubia Quilmes", "Bebidas alcohólicas", 1f, 0f);
            escenario.Servicio.Vectores["birra"] = Vector(0.95f, 0.31f);

            await escenario.BuscarAsync(Item(1, "birra"));

            CollectionAssert.AreEqual(new[] { "birra" }, escenario.Servicio.TextosRecibidos.ToArray());
            Assert.AreEqual(1, escenario.Servicio.Llamadas);
            Assert.AreEqual(0, escenario.Unidad.ProductoEmbeddingRepo.Guardados.Count);
            Assert.AreEqual(0, escenario.UnidadFactory.CrearCallCount);
        }

        [TestMethod]
        public async Task BuscarAsync_ServicioFalla_DejaLosItemsSinCoincidenciaSinLanzarNiGuardar()
        {
            Escenario escenario = new Escenario();
            escenario.AgregarProductoSinVector(155, "Cerveza rubia Quilmes", "Bebidas alcohólicas");
            escenario.Servicio.ThrowException = new SERVICIOS.ExcepcionTecnica("Error.Assistant.ResponseFailed");

            Dictionary<int, List<CoincidenciaProducto>> resultado = await escenario.BuscarAsync(Item(1, "birra"), Item(2, "detergente"));

            Assert.AreEqual(0, resultado[1].Count);
            Assert.AreEqual(0, resultado[2].Count);
            Assert.AreEqual(0, escenario.Unidad.ProductoEmbeddingRepo.Guardados.Count);
        }

        [TestMethod]
        public async Task BuscarAsync_MuchosProductosSinVector_RespetaElLimiteDeTextosYPrimeroLosItems()
        {
            Escenario escenario = new Escenario();
            for (int id = 1; id <= 150; id++)
            {
                escenario.AgregarProductoSinVector(id, "Producto " + id, "Cat");
            }

            await escenario.BuscarAsync(Item(1, "birra"), Item(2, "detergente"));

            List<string> textos = escenario.Servicio.TextosRecibidos.ToList();
            Assert.AreEqual(CoincidenciaSemanticaProductos.LimiteTextosPorConsulta, textos.Count);
            Assert.AreEqual("birra", textos[0]);
            Assert.AreEqual("detergente", textos[1]);
            Assert.AreEqual("Producto 1 Cat", textos[2]);
            Assert.AreEqual(CoincidenciaSemanticaProductos.LimiteTextosPorConsulta - 2, escenario.Unidad.ProductoEmbeddingRepo.Guardados.Count);
        }

        [TestMethod]
        public async Task BuscarAsync_VariosProductosSobreElUmbral_ConservaComoMaximoTres()
        {
            Escenario escenario = new Escenario();
            for (int id = 1; id <= 5; id++)
            {
                escenario.AgregarProducto(id, "Cerveza " + id, "Bebidas", 1f, 0.01f * id);
            }

            escenario.Servicio.Vectores["birra"] = Vector(1f, 0f);

            Dictionary<int, List<CoincidenciaProducto>> resultado = await escenario.BuscarAsync(Item(1, "birra"));

            CollectionAssert.AreEqual(new[] { 1, 2, 3 }, resultado[1].Select(item => item.Producto.Id).ToArray());
        }

        [TestMethod]
        public async Task BuscarAsync_SinItems_NoLlamaAlServicio()
        {
            Escenario escenario = new Escenario();
            escenario.AgregarProductoSinVector(155, "Cerveza", "Bebidas");

            Dictionary<int, List<CoincidenciaProducto>> resultado = await escenario.BuscarAsync();

            Assert.AreEqual(0, resultado.Count);
            Assert.AreEqual(0, escenario.Servicio.Llamadas);
        }

        private static ItemLista Item(int id, string descripcion)
        {
            return new ItemLista { Id = id, Descripcion = descripcion, Cantidad = 1 };
        }

        private static float[] Vector(params float[] valores)
        {
            return valores;
        }

        private sealed class Escenario
        {
            public readonly FakeServicioEmbeddings Servicio = new FakeServicioEmbeddings();
            public readonly FakeProductoEmbeddingRepositorio Lectura = new FakeProductoEmbeddingRepositorio();
            public readonly FakeUnidadDeTrabajo Unidad = new FakeUnidadDeTrabajo();
            public readonly FakeUnidadDeTrabajoFactory UnidadFactory;
            public readonly List<Producto> Productos = new List<Producto>();

            public Escenario()
            {
                UnidadFactory = new FakeUnidadDeTrabajoFactory(Unidad);
            }

            public Producto AgregarProducto(int id, string nombre, string categoria, float x, float y, string modelo = FakeServicioEmbeddings.ModeloFalso)
            {
                Producto producto = AgregarProductoSinVector(id, nombre, categoria);
                Lectura.Embeddings[id] = new ProductoEmbedding
                {
                    Producto = producto,
                    Modelo = modelo,
                    TextoFuente = nombre + " " + categoria,
                    Vector = new[] { x, y },
                    FechaCalculo = Ahora.AddDays(-1)
                };
                return producto;
            }

            public Producto AgregarProductoSinVector(int id, string nombre, string categoria)
            {
                Producto producto = new Producto { Id = id, Nombre = nombre, CategoriaProducto = new CategoriaProducto { Id = 1, Nombre = categoria } };
                Productos.Add(producto);
                return producto;
            }

            public Task<Dictionary<int, List<CoincidenciaProducto>>> BuscarAsync(params ItemLista[] items)
            {
                CoincidenciaSemanticaProductos sut = new CoincidenciaSemanticaProductos(Servicio, Lectura, UnidadFactory, () => Ahora);
                return sut.BuscarAsync(items, Productos, CancellationToken.None);
            }
        }
    }
}
