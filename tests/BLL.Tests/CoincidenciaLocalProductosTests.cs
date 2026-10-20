using System.Collections.Generic;
using System.Linq;
using BE;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BLL.Tests
{
    // Casos tomados del catalogo real de datos-de-prueba (nombres de productos con marca y
    // presentacion) contra items de texto libre como los que carga el usuario.
    [TestClass]
    public sealed class CoincidenciaLocalProductosTests
    {
        private static readonly List<Producto> Catalogo = new List<Producto>
        {
            Crear(6, "Arroz largo fino Gallo 1 kg", "Almacén"),
            Crear(13, "Leche entera clásica La Serenísima sachet 1 L", "Lácteos"),
            Crear(14, "Leche UAT entera La Serenísima 1 L", "Lácteos"),
            Crear(15, "Leche entera con calcio La Serenísima 1 L", "Lácteos"),
            Crear(16, "Manteca clásica La Serenísima 200 g", "Lácteos"),
            Crear(19, "Yogur griego natural endulzado Yogurísimo 300 g", "Lácteos"),
            Crear(42, "Banana nacional, por kg", "Frutas y verduras"),
            Crear(61, "Gaseosa Coca-Cola 2,25 L", "Bebidas"),
            Crear(70, "Jugo de manzana mix Nutri Defensas Cepita 1 L", "Bebidas"),
            Crear(11, "Yerba mate con palo Playadito 500 g", "Almacén"),
            Crear(121, "Pañales Pants Premium Care Pampers G 30 unidades", "Mundo bebé"),
            Crear(155, "Cerveza rubia Quilmes lata 473 ml", "Bebidas alcohólicas"),
            Crear(900, "galletita", "Almacén")
        };

        [TestMethod]
        public void Buscar_PluralYMayuscula_CoincideConElSingular()
        {
            List<CoincidenciaProducto> resultado = CoincidenciaLocalProductos.Buscar("Galletitas", Catalogo);

            Assert.AreEqual(900, resultado.First().Producto.Id);
            Assert.IsFalse(resultado.First().EsSemantica);
        }

        [TestMethod]
        public void Buscar_SinAcentosEnElItem_CoincideConElNombreAcentuado()
        {
            List<CoincidenciaProducto> resultado = CoincidenciaLocalProductos.Buscar("manteca serenisima", Catalogo);

            Assert.AreEqual(16, resultado.Single().Producto.Id);
        }

        [TestMethod]
        public void Buscar_TodoEnMayusculasYTokensDesordenados_Coincide()
        {
            List<CoincidenciaProducto> resultado = CoincidenciaLocalProductos.Buscar("GALLO ARROZ", Catalogo);

            Assert.AreEqual(6, resultado.Single().Producto.Id);
        }

        [TestMethod]
        public void Buscar_PluralDelItemContraSingularDelProducto_Coincide()
        {
            Assert.AreEqual(42, CoincidenciaLocalProductos.Buscar("bananas", Catalogo).Single().Producto.Id);
            Assert.AreEqual(121, CoincidenciaLocalProductos.Buscar("pañal", Catalogo).Single().Producto.Id);
        }

        [TestMethod]
        public void Buscar_SignosEnElNombre_SeIgnoran()
        {
            Assert.AreEqual(61, CoincidenciaLocalProductos.Buscar("coca cola", Catalogo).Single().Producto.Id);
        }

        [TestMethod]
        public void Buscar_ErrorDeTipeoChico_Coincide()
        {
            Assert.AreEqual(11, CoincidenciaLocalProductos.Buscar("yerva playadito", Catalogo).Single().Producto.Id);
        }

        [TestMethod]
        public void Buscar_CategoriaDelProducto_CuentaComoTextoDelProducto()
        {
            List<int> ids = CoincidenciaLocalProductos.Buscar("cerveza bebidas alcoholicas", Catalogo).Select(item => item.Producto.Id).ToList();

            CollectionAssert.AreEqual(new List<int> { 155 }, ids);
        }

        [TestMethod]
        public void Buscar_SinonimoColoquial_NoSuperaElUmbralLocal()
        {
            Assert.AreEqual(0, CoincidenciaLocalProductos.Buscar("birra", Catalogo).Count);
        }

        [TestMethod]
        public void Buscar_ItemConPalabrasQueElProductoNoTiene_NoSuperaElUmbral()
        {
            Assert.AreEqual(0, CoincidenciaLocalProductos.Buscar("yogurt frutilla", Catalogo).Count);
            Assert.AreEqual(0, CoincidenciaLocalProductos.Buscar("gatorade manzana", Catalogo).Count);
            Assert.AreEqual(0, CoincidenciaLocalProductos.Buscar("yerba organica de la abuela", Catalogo).Count);
        }

        [TestMethod]
        public void Buscar_VariosProductosIguales_DevuelveComoMaximoTresOrdenadosPorPuntaje()
        {
            List<CoincidenciaProducto> resultado = CoincidenciaLocalProductos.Buscar("leche", Catalogo);

            Assert.AreEqual(CoincidenciaLocalProductos.MaximoProductosPorItem, resultado.Count);
            Assert.IsTrue(resultado.All(item => new[] { 13, 14, 15 }.Contains(item.Producto.Id)));
            for (int i = 1; i < resultado.Count; i++)
            {
                Assert.IsTrue(resultado[i - 1].Puntaje >= resultado[i].Puntaje);
            }
        }

        [TestMethod]
        public void Buscar_ItemVacioOSoloPalabrasVacias_NoCoincide()
        {
            Assert.AreEqual(0, CoincidenciaLocalProductos.Buscar("   ", Catalogo).Count);
            Assert.AreEqual(0, CoincidenciaLocalProductos.Buscar("de la", Catalogo).Count);
            Assert.AreEqual(0, CoincidenciaLocalProductos.Buscar(null, Catalogo).Count);
        }

        [TestMethod]
        public void Similitud_RespetaElUmbralConfigurado()
        {
            Assert.AreEqual(0.85, CoincidenciaLocalProductos.UmbralLocal, 0.0001);
            Assert.IsTrue(CoincidenciaLocalProductos.Similitud("leche", "Leche UAT entera La Serenísima 1 L") >= CoincidenciaLocalProductos.UmbralLocal);
            Assert.IsTrue(CoincidenciaLocalProductos.Similitud("birra", "Cerveza rubia Quilmes lata 473 ml") < CoincidenciaLocalProductos.UmbralLocal);
        }

        [TestMethod]
        public void Normalizar_QuitaAcentosSignosYMayusculas()
        {
            Assert.AreEqual("pan lacteado fargo 460 g", CoincidenciaLocalProductos.Normalizar("Pan  LÁCTEADO, Fargo (460 g)"));
        }

        private static Producto Crear(int id, string nombre, string categoria)
        {
            return new Producto
            {
                Id = id,
                Nombre = nombre,
                CategoriaProducto = new CategoriaProducto { Id = id + 1000, Nombre = categoria }
            };
        }
    }
}
