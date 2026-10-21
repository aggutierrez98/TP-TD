using System;
using System.Collections.Generic;
using BE;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BLL.Tests
{
    [TestClass]
    public sealed class AlcancePromocionTests
    {
        private static readonly EntidadFinanciera Galicia = new EntidadFinanciera { Id = 1 };
        private static readonly EntidadFinanciera Macro = new EntidadFinanciera { Id = 2 };
        private static readonly MedioPago VisaCredito = new MedioPago { Id = 10 };
        private static readonly MedioPago MastercardCredito = new MedioPago { Id = 11 };
        private const int Supermercados = 100;
        private const int Indumentaria = 101;
        private const int Carrefour = 1000;
        private const int Coto = 1001;
        private const int Carnes = 500;
        private const int Bebidas = 501;

        // --- Sin filtros ---

        [TestMethod]
        public void SinFiltros_AplicaACualquierComercioPagoDiaYProducto()
        {
            Promocion promocion = NuevaPromocion();

            Assert.IsTrue(AlcancePromocion.AplicaAComercio(promocion, NuevoComercio(Carrefour, Supermercados)));
            Assert.IsTrue(AlcancePromocion.AplicaAPago(promocion, Galicia, VisaCredito, "Black"));
            Assert.IsTrue(AlcancePromocion.AplicaAPago(promocion, null, null, null));
            Assert.IsTrue(AlcancePromocion.Vigente(promocion, new DateTime(2026, 3, 10)));
            Assert.IsTrue(AlcancePromocion.AplicaAProducto(promocion, NuevoProducto(7, Bebidas)));
        }

        // --- Y entre filtros, O dentro de uno ---

        [TestMethod]
        public void EntidadYMedioDePago_AplicaSoloSiCumpleAmbos()
        {
            Promocion promocion = NuevaPromocion();
            promocion.EntidadesFinancieras.Add(Galicia);
            promocion.MediosPago.Add(VisaCredito);

            Assert.IsTrue(AlcancePromocion.AplicaAPago(promocion, Galicia, VisaCredito, null));
            Assert.IsFalse(AlcancePromocion.AplicaAPago(promocion, Macro, VisaCredito, null), "Visa de otra entidad no aplica.");
            Assert.IsFalse(AlcancePromocion.AplicaAPago(promocion, Galicia, MastercardCredito, null), "Mastercard de Galicia no aplica.");
        }

        [TestMethod]
        public void VariosMediosDePago_BastaConUno()
        {
            Promocion promocion = NuevaPromocion();
            promocion.MediosPago.AddRange(new[] { VisaCredito, MastercardCredito });

            Assert.IsTrue(AlcancePromocion.AplicaAPago(promocion, Galicia, VisaCredito, null));
            Assert.IsTrue(AlcancePromocion.AplicaAPago(promocion, Macro, MastercardCredito, null));
            Assert.IsFalse(AlcancePromocion.AplicaAPago(promocion, Galicia, new MedioPago { Id = 99 }, null));
        }

        [TestMethod]
        public void Segmento_EsFiltroPropioYSeCombinaConLaEntidad()
        {
            Promocion promocion = NuevaPromocion();
            promocion.EntidadesFinancieras.Add(Galicia);
            promocion.Segmentos.Add("Black");

            Assert.IsTrue(AlcancePromocion.AplicaAPago(promocion, Galicia, VisaCredito, "black"), "El segmento no distingue mayusculas.");
            Assert.IsFalse(AlcancePromocion.AplicaAPago(promocion, Galicia, VisaCredito, "Classic"));
            Assert.IsFalse(AlcancePromocion.AplicaAPago(promocion, Macro, VisaCredito, "Black"));
            Assert.IsFalse(AlcancePromocion.AplicaAPago(promocion, Galicia, VisaCredito, null), "Un pago sin segmento no cumple un filtro de segmentos.");
        }

        // --- Medio de pago independiente de la entidad ---

        [TestMethod]
        public void MedioDePagoSinEntidad_AplicaAUnaVisaDeCualquierEntidad()
        {
            Promocion promocion = NuevaPromocion();
            promocion.MediosPago.Add(VisaCredito);

            Assert.IsTrue(AlcancePromocion.AplicaAPago(promocion, Galicia, VisaCredito, null));
            Assert.IsTrue(AlcancePromocion.AplicaAPago(promocion, Macro, VisaCredito, null));
            Assert.IsTrue(AlcancePromocion.AplicaAPago(promocion, null, VisaCredito, null), "Dinero en cuenta u otro pago sin entidad tambien cumple.");
        }

        [TestMethod]
        public void SinEntidadNiMedioDePago_AplicaConCualquierFormaDePago()
        {
            Promocion promocion = NuevaPromocion();

            Assert.IsTrue(AlcancePromocion.AplicaAPago(promocion, Galicia, VisaCredito, "Black"));
            Assert.IsTrue(AlcancePromocion.AplicaAPago(promocion, null, null, null));
        }

        [TestMethod]
        public void EntidadSinMedioDePago_AplicaAcualquierMedioDeEsaEntidad()
        {
            Promocion promocion = NuevaPromocion();
            promocion.EntidadesFinancieras.Add(Galicia);

            Assert.IsTrue(AlcancePromocion.AplicaAPago(promocion, Galicia, MastercardCredito, null));
            Assert.IsFalse(AlcancePromocion.AplicaAPago(promocion, null, MastercardCredito, null), "Sin entidad no cumple un filtro de entidades.");
        }

        // --- Comercios y rubros ---

        [TestMethod]
        public void SoloRubro_AplicaALosComerciosDeEseRubro()
        {
            Promocion promocion = NuevaPromocion();
            promocion.RubrosComercio.Add(new RubroComercio { Id = Supermercados });

            Assert.IsTrue(AlcancePromocion.AplicaAComercio(promocion, NuevoComercio(Carrefour, Supermercados)));
        }

        [TestMethod]
        public void SoloRubro_NoAplicaAUnComercioDeOtroRubro()
        {
            Promocion promocion = NuevaPromocion();
            promocion.RubrosComercio.Add(new RubroComercio { Id = Supermercados });

            Assert.IsFalse(AlcancePromocion.AplicaAComercio(promocion, NuevoComercio(Coto, Indumentaria)));
        }

        [TestMethod]
        public void SoloComercio_AplicaSoloAEseComercio()
        {
            Promocion promocion = NuevaPromocion();
            promocion.Comercios.Add(new Comercio { Id = Carrefour });

            Assert.IsTrue(AlcancePromocion.AplicaAComercio(promocion, NuevoComercio(Carrefour, Supermercados)));
            Assert.IsFalse(AlcancePromocion.AplicaAComercio(promocion, NuevoComercio(Coto, Supermercados)));
        }

        [TestMethod]
        public void ComercioYRubro_SeCombinanConY()
        {
            Promocion promocion = NuevaPromocion();
            promocion.Comercios.AddRange(new[] { new Comercio { Id = Carrefour }, new Comercio { Id = Coto } });
            promocion.RubrosComercio.Add(new RubroComercio { Id = Supermercados });

            Assert.IsTrue(AlcancePromocion.AplicaAComercio(promocion, NuevoComercio(Carrefour, Supermercados)));
            Assert.IsFalse(AlcancePromocion.AplicaAComercio(promocion, NuevoComercio(Coto, Indumentaria)), "Listado pero de otro rubro.");
            Assert.IsFalse(AlcancePromocion.AplicaAComercio(promocion, NuevoComercio(2000, Supermercados)), "Del rubro pero no listado.");
        }

        [TestMethod]
        public void ComercioSinRubroCargado_NoCumpleUnFiltroDeRubros()
        {
            Promocion promocion = NuevaPromocion();
            promocion.RubrosComercio.Add(new RubroComercio { Id = Supermercados });

            Assert.IsFalse(AlcancePromocion.AplicaAComercio(promocion, new Comercio { Id = Carrefour }));
        }

        // --- Productos y categorias ---

        [TestMethod]
        public void SoloCategoriaExcluida_ParticipaTodoSalvoEsaCategoria()
        {
            Promocion promocion = NuevaPromocion();
            promocion.CategoriasExcluidas.Add(new CategoriaProducto { Id = Carnes });

            Assert.IsTrue(AlcancePromocion.AplicaAProducto(promocion, NuevoProducto(1, Bebidas)));
            Assert.IsFalse(AlcancePromocion.AplicaAProducto(promocion, NuevoProducto(2, Carnes)));
        }

        [TestMethod]
        public void CategoriaIncluidaYProductoExcluido_ParticipanLasBebidasSalvoEseProducto()
        {
            Promocion promocion = NuevaPromocion();
            promocion.CategoriasIncluidas.Add(new CategoriaProducto { Id = Bebidas });
            promocion.ProductosExcluidos.Add(new Producto { Id = 7 });

            Assert.IsTrue(AlcancePromocion.AplicaAProducto(promocion, NuevoProducto(8, Bebidas)));
            Assert.IsFalse(AlcancePromocion.AplicaAProducto(promocion, NuevoProducto(7, Bebidas)), "El producto excluido queda afuera.");
            Assert.IsFalse(AlcancePromocion.AplicaAProducto(promocion, NuevoProducto(9, Carnes)), "Otra categoria no esta incluida.");
        }

        [TestMethod]
        public void ProductoIncluido_ParticipaSoloEseProducto()
        {
            Promocion promocion = NuevaPromocion();
            promocion.ProductosIncluidos.Add(new Producto { Id = 7 });

            Assert.IsTrue(AlcancePromocion.AplicaAProducto(promocion, NuevoProducto(7, Carnes)));
            Assert.IsFalse(AlcancePromocion.AplicaAProducto(promocion, NuevoProducto(8, Carnes)));
        }

        [TestMethod]
        public void ProductoIncluidoYCategoriaIncluida_BastaConUnaInclusion()
        {
            Promocion promocion = NuevaPromocion();
            promocion.ProductosIncluidos.Add(new Producto { Id = 7 });
            promocion.CategoriasIncluidas.Add(new CategoriaProducto { Id = Bebidas });

            Assert.IsTrue(AlcancePromocion.AplicaAProducto(promocion, NuevoProducto(7, Carnes)), "Coincide por producto.");
            Assert.IsTrue(AlcancePromocion.AplicaAProducto(promocion, NuevoProducto(8, Bebidas)), "Coincide por categoria.");
            Assert.IsFalse(AlcancePromocion.AplicaAProducto(promocion, NuevoProducto(9, Carnes)));
        }

        [TestMethod]
        public void ProductoIncluidoConSuCategoriaExcluida_LaExclusionGana()
        {
            Promocion promocion = NuevaPromocion();
            promocion.ProductosIncluidos.Add(new Producto { Id = 7 });
            promocion.CategoriasExcluidas.Add(new CategoriaProducto { Id = Carnes });

            Assert.IsFalse(AlcancePromocion.AplicaAProducto(promocion, NuevoProducto(7, Carnes)));
        }

        [TestMethod]
        public void CategoriaIncluidaConProductoExcluidoDeOtraCategoria_NoAfectaALosDemas()
        {
            Promocion promocion = NuevaPromocion();
            promocion.CategoriasIncluidas.Add(new CategoriaProducto { Id = Bebidas });
            promocion.ProductosExcluidos.Add(new Producto { Id = 99 });

            Assert.IsTrue(AlcancePromocion.AplicaAProducto(promocion, NuevoProducto(8, Bebidas)));
        }

        // --- Vigencia y dias ---

        [TestMethod]
        public void Vigencia_ExtremosInclusivosYFueraDeRangoNoAplica()
        {
            Promocion promocion = NuevaPromocion();

            Assert.IsTrue(AlcancePromocion.Vigente(promocion, new DateTime(2026, 3, 1)));
            Assert.IsTrue(AlcancePromocion.Vigente(promocion, new DateTime(2026, 3, 31, 23, 59, 0)), "El dia hasta es inclusivo aunque tenga hora.");
            Assert.IsFalse(AlcancePromocion.Vigente(promocion, new DateTime(2026, 4, 1)));
            Assert.IsFalse(AlcancePromocion.Vigente(promocion, new DateTime(2026, 2, 28)));
        }

        [TestMethod]
        public void Dias_SinDiasCargadosAplicaTodosLosDias()
        {
            Promocion promocion = NuevaPromocion();

            for (int dia = 2; dia <= 8; dia++)
            {
                Assert.IsTrue(AlcancePromocion.Vigente(promocion, new DateTime(2026, 3, dia)));
            }
        }

        [TestMethod]
        public void Dias_ConDiasCargadosAplicaSoloEsosDias()
        {
            Promocion promocion = NuevaPromocion();
            promocion.DiasSemana.Add(DiaSemana.Martes);
            promocion.DiasSemana.Add(DiaSemana.Domingo);

            Assert.IsTrue(AlcancePromocion.Vigente(promocion, new DateTime(2026, 3, 3)), "03/03/2026 es martes.");
            Assert.IsTrue(AlcancePromocion.Vigente(promocion, new DateTime(2026, 3, 8)), "08/03/2026 es domingo.");
            Assert.IsFalse(AlcancePromocion.Vigente(promocion, new DateTime(2026, 3, 4)), "04/03/2026 es miercoles.");
        }

        // --- Promociones que le aplican a un comercio (regla compartida) ---

        [TestMethod]
        public void PromocionSinComercioNiRubro_AparecePorCualquierComercio()
        {
            Promocion promocion = NuevaPromocion();

            List<Promocion> resultado = AlcancePromocion.FiltrarPorComercio(
                new List<Promocion> { promocion },
                NuevoComercio(Coto, Indumentaria));

            Assert.AreEqual(1, resultado.Count);
        }

        [TestMethod]
        public void PromocionDeOtroComercio_NoAparece()
        {
            Promocion promocion = NuevaPromocion();
            promocion.Comercios.Add(new Comercio { Id = Carrefour });

            List<Promocion> resultado = AlcancePromocion.FiltrarPorComercio(
                new List<Promocion> { promocion },
                NuevoComercio(Coto, Supermercados));

            Assert.AreEqual(0, resultado.Count);
        }

        [TestMethod]
        public void FiltrarPorComercio_ConservaSoloLasQueAplican()
        {
            Promocion propia = NuevaPromocion();
            propia.Comercios.Add(new Comercio { Id = Coto });
            Promocion porRubro = NuevaPromocion();
            porRubro.RubrosComercio.Add(new RubroComercio { Id = Supermercados });
            Promocion ajena = NuevaPromocion();
            ajena.Comercios.Add(new Comercio { Id = Carrefour });

            List<Promocion> resultado = AlcancePromocion.FiltrarPorComercio(
                new List<Promocion> { propia, porRubro, ajena },
                NuevoComercio(Coto, Supermercados));

            CollectionAssert.AreEquivalent(new List<Promocion> { propia, porRubro }, resultado);
        }

        private static Promocion NuevaPromocion()
        {
            return new Promocion
            {
                Id = 1,
                Descripcion = "Promo",
                Descuento = 10m,
                VigenciaDesde = new DateTime(2026, 3, 1),
                VigenciaHasta = new DateTime(2026, 3, 31)
            };
        }

        private static Producto NuevoProducto(int id, int categoriaId)
        {
            return new Producto { Id = id, CategoriaProducto = new CategoriaProducto { Id = categoriaId } };
        }

        private static Comercio NuevoComercio(int id, int rubroId)
        {
            return new Comercio { Id = id, RubroComercio = new RubroComercio { Id = rubroId } };
        }
    }
}
