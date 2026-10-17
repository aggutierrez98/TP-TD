using System;
using System.Collections.Generic;
using BE;
using BLL.Tests.TestDoubles;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SERVICIOS;

namespace BLL.Tests
{
    [TestClass]
    public sealed class PromocionBLLTests
    {
        // --- Buscar ---

        [TestMethod]
        public void Buscar_SinFiltros_DevuelveTodasOrdenadasPorVigenciaDescendente()
        {
            PromocionFixture fixture = new PromocionFixture();
            fixture.AgregarPromocion(1, "Enero", new DateTime(2026, 1, 1));
            fixture.AgregarPromocion(2, "Marzo", new DateTime(2026, 3, 1));
            fixture.AgregarPromocion(3, "Febrero", new DateTime(2026, 2, 1));
            PromocionBLL sut = fixture.CreateSut();

            List<Promocion> resultado = sut.Buscar(null, null);

            CollectionAssert.AreEqual(new[] { 2, 3, 1 }, resultado.ConvertAll(item => item.Id));
        }

        [TestMethod]
        public void Buscar_TextoEnBlanco_DevuelveTodas()
        {
            PromocionFixture fixture = new PromocionFixture();
            fixture.AgregarPromocion(1, "Martes de descuento", new DateTime(2026, 1, 1));
            fixture.AgregarPromocion(2, "Fin de semana", new DateTime(2026, 1, 1));
            PromocionBLL sut = fixture.CreateSut();

            Assert.AreEqual(2, sut.Buscar("   ", null).Count);
        }

        [TestMethod]
        public void Buscar_Texto_IgnoraMayusculasYAcentos()
        {
            PromocionFixture fixture = new PromocionFixture();
            fixture.AgregarPromocion(1, "Descuento los MARTES en supermercados", new DateTime(2026, 1, 1));
            fixture.AgregarPromocion(2, "Reintegro en combustible", new DateTime(2026, 1, 1));
            fixture.AgregarPromocion(3, "Promoción de cafetería", new DateTime(2026, 1, 1));
            PromocionBLL sut = fixture.CreateSut();

            Assert.AreEqual(1, sut.Buscar("martes", null).Count);
            Assert.AreEqual(3, sut.Buscar("promocion de cafeteria", null)[0].Id);
            Assert.AreEqual(1, sut.Buscar("COMBUSTIBLE", null).Count);
            Assert.AreEqual(0, sut.Buscar("inexistente", null).Count);
        }

        [TestMethod]
        public void Buscar_PorComercio_UsaLaReglaDeAlcance()
        {
            PromocionFixture fixture = new PromocionFixture();
            fixture.Unidad.ComercioRepo.Comercios.Add(10, new Comercio { Id = 10, RubroComercio = new RubroComercio { Id = 100 } });
            Promocion sinRestriccion = fixture.AgregarPromocion(1, "Sin restriccion", new DateTime(2026, 1, 1));
            Promocion porRubro = fixture.AgregarPromocion(2, "Por rubro", new DateTime(2026, 1, 1));
            porRubro.RubroComercioIds.Add(100);
            Promocion propia = fixture.AgregarPromocion(3, "Propia", new DateTime(2026, 1, 1));
            propia.ComercioIds.Add(10);
            Promocion ajena = fixture.AgregarPromocion(4, "Ajena", new DateTime(2026, 1, 1));
            ajena.ComercioIds.Add(11);
            Promocion otroRubro = fixture.AgregarPromocion(5, "Otro rubro", new DateTime(2026, 1, 1));
            otroRubro.RubroComercioIds.Add(101);
            PromocionBLL sut = fixture.CreateSut();

            List<Promocion> resultado = sut.Buscar(null, 10);

            CollectionAssert.AreEquivalent(new[] { 1, 2, 3 }, resultado.ConvertAll(item => item.Id));
        }

        [TestMethod]
        public void Buscar_TextoYComercio_SeCombinanConY()
        {
            PromocionFixture fixture = new PromocionFixture();
            fixture.Unidad.ComercioRepo.Comercios.Add(10, new Comercio { Id = 10, RubroComercio = new RubroComercio { Id = 100 } });
            fixture.AgregarPromocion(1, "Martes en el comercio", new DateTime(2026, 1, 1));
            Promocion ajena = fixture.AgregarPromocion(2, "Martes de otro comercio", new DateTime(2026, 1, 1));
            ajena.ComercioIds.Add(11);
            fixture.AgregarPromocion(3, "Jueves en el comercio", new DateTime(2026, 1, 1));
            PromocionBLL sut = fixture.CreateSut();

            List<Promocion> resultado = sut.Buscar("martes", 10);

            Assert.AreEqual(1, resultado.Count);
            Assert.AreEqual(1, resultado[0].Id);
        }

        [TestMethod]
        public void Buscar_ComercioInexistente_ThrowsExcepcionNegocio()
        {
            PromocionFixture fixture = new PromocionFixture();
            PromocionBLL sut = fixture.CreateSut();

            ExcepcionNegocio ex = Assert.ThrowsExactly<ExcepcionNegocio>(() => sut.Buscar(null, 99));

            Assert.AreEqual("Error.PromotionManagement.ComercioNotFound", ex.Message);
        }

        // --- ObtenerPorComercio (misma regla que el buscador) ---

        [TestMethod]
        public void ObtenerPorComercio_SinPromociones_DevuelveListaVacia()
        {
            PromocionFixture fixture = new PromocionFixture();
            fixture.Unidad.ComercioRepo.Comercios.Add(1, new Comercio { Id = 1, RubroComercio = new RubroComercio { Id = 100 } });
            PromocionBLL sut = fixture.CreateSut();

            Assert.AreEqual(0, sut.ObtenerPorComercio(1).Count);
        }

        [TestMethod]
        public void ObtenerPorComercio_AplicaLaReglaYOrdenaPorVigenciaDescendente()
        {
            PromocionFixture fixture = new PromocionFixture();
            fixture.Unidad.ComercioRepo.Comercios.Add(1, new Comercio { Id = 1, RubroComercio = new RubroComercio { Id = 100 } });
            Promocion propia = fixture.AgregarPromocion(1, "2x1", new DateTime(2026, 1, 1));
            propia.ComercioIds.Add(1);
            Promocion porRubro = fixture.AgregarPromocion(2, "20% off", new DateTime(2026, 3, 1));
            porRubro.RubroComercioIds.Add(100);
            Promocion ajena = fixture.AgregarPromocion(3, "Otro comercio", new DateTime(2026, 2, 1));
            ajena.ComercioIds.Add(2);
            PromocionBLL sut = fixture.CreateSut();

            List<Promocion> resultado = sut.ObtenerPorComercio(1);

            CollectionAssert.AreEqual(new[] { 2, 1 }, resultado.ConvertAll(item => item.Id));
        }

        [TestMethod]
        public void ObtenerPorComercio_ComercioInexistente_DevuelveListaVacia()
        {
            PromocionFixture fixture = new PromocionFixture();
            fixture.AgregarPromocion(1, "Sin restriccion", new DateTime(2026, 1, 1));
            PromocionBLL sut = fixture.CreateSut();

            Assert.AreEqual(0, sut.ObtenerPorComercio(99).Count);
        }

        // --- Catalogos ---

        [TestMethod]
        public void ObtenerMediosPago_DevuelveLosDelCatalogo()
        {
            PromocionFixture fixture = new PromocionFixture();
            fixture.Unidad.MedioPagoRepo.MediosPago.Add(1, new MedioPago { Id = 1, Tipo = TipoMedioPago.TarjetaCredito, Marca = "Visa" });
            fixture.Unidad.MedioPagoRepo.MediosPago.Add(2, new MedioPago { Id = 2, Tipo = TipoMedioPago.TarjetaDebito, Marca = "Maestro" });
            PromocionBLL sut = fixture.CreateSut();

            Assert.AreEqual(2, sut.ObtenerMediosPago().Count);
        }

        [TestMethod]
        public void ObtenerSegmentosDisponibles_DevuelveLosSegmentosYaCargadosSinRepetir()
        {
            PromocionFixture fixture = new PromocionFixture();
            fixture.Unidad.MedioPagoEntidadRepo.MediosPagoEntidad.Add(1, new MedioPagoEntidad { Id = 1, Segmento = "Black" });
            fixture.Unidad.MedioPagoEntidadRepo.MediosPagoEntidad.Add(2, new MedioPagoEntidad { Id = 2, Segmento = "Black" });
            fixture.Unidad.MedioPagoEntidadRepo.MediosPagoEntidad.Add(3, new MedioPagoEntidad { Id = 3, Segmento = "Classic" });
            fixture.Unidad.MedioPagoEntidadRepo.MediosPagoEntidad.Add(4, new MedioPagoEntidad { Id = 4, Segmento = null });
            PromocionBLL sut = fixture.CreateSut();

            CollectionAssert.AreEqual(new[] { "Black", "Classic" }, sut.ObtenerSegmentosDisponibles());
        }

        // --- Crear: validaciones previas (no abren la unidad de trabajo) ---

        [TestMethod]
        public void Crear_DescripcionVacia_ThrowsExcepcionNegocioAndDoesNotCreateUow()
        {
            PromocionFixture fixture = new PromocionFixture();
            Promocion datos = fixture.DatosValidos();
            datos.Descripcion = "   ";
            PromocionBLL sut = fixture.CreateSut();

            ExcepcionNegocio ex = Assert.ThrowsExactly<ExcepcionNegocio>(() => sut.Crear(datos));

            Assert.AreEqual("Error.PromotionManagement.DescriptionRequired", ex.Message);
            Assert.AreEqual(0, fixture.UnidadFactory.CrearCallCount);
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(-5)]
        [DataRow(100.01)]
        [DataRow(250)]
        public void Crear_DescuentoFueraDeRango_ThrowsExcepcionNegocio(double descuento)
        {
            PromocionFixture fixture = new PromocionFixture();
            Promocion datos = fixture.DatosValidos();
            datos.Descuento = (decimal)descuento;
            PromocionBLL sut = fixture.CreateSut();

            ExcepcionNegocio ex = Assert.ThrowsExactly<ExcepcionNegocio>(() => sut.Crear(datos));

            Assert.AreEqual("Error.PromotionManagement.DiscountInvalid", ex.Message);
            Assert.AreEqual(0, fixture.UnidadFactory.CrearCallCount);
        }

        [TestMethod]
        public void Crear_DescuentoCien_EsValido()
        {
            PromocionFixture fixture = new PromocionFixture();
            Promocion datos = fixture.DatosValidos();
            datos.Descuento = 100m;
            PromocionBLL sut = fixture.CreateSut();

            Promocion creada = sut.Crear(datos);

            Assert.IsTrue(creada.Id > 0);
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(-1)]
        public void Crear_TopeNoPositivo_ThrowsExcepcionNegocio(double tope)
        {
            PromocionFixture fixture = new PromocionFixture();
            Promocion datos = fixture.DatosValidos();
            datos.TopeDescuento = (decimal)tope;
            PromocionBLL sut = fixture.CreateSut();

            ExcepcionNegocio ex = Assert.ThrowsExactly<ExcepcionNegocio>(() => sut.Crear(datos));

            Assert.AreEqual("Error.PromotionManagement.CapInvalid", ex.Message);
        }

        [TestMethod]
        public void Crear_VigenciaDesdePosteriorAHasta_ThrowsExcepcionNegocio()
        {
            PromocionFixture fixture = new PromocionFixture();
            Promocion datos = fixture.DatosValidos();
            datos.VigenciaDesde = new DateTime(2026, 4, 1);
            datos.VigenciaHasta = new DateTime(2026, 3, 1);
            PromocionBLL sut = fixture.CreateSut();

            ExcepcionNegocio ex = Assert.ThrowsExactly<ExcepcionNegocio>(() => sut.Crear(datos));

            Assert.AreEqual("Error.PromotionManagement.VigenciaInvalid", ex.Message);
        }

        [TestMethod]
        public void Crear_MismoProductoIncluidoYExcluido_ThrowsExcepcionNegocio()
        {
            PromocionFixture fixture = new PromocionFixture();
            Promocion datos = fixture.DatosValidos();
            datos.ProductoIncluidoIds.Add(7);
            datos.ProductoExcluidoIds.Add(7);
            PromocionBLL sut = fixture.CreateSut();

            ExcepcionNegocio ex = Assert.ThrowsExactly<ExcepcionNegocio>(() => sut.Crear(datos));

            Assert.AreEqual("Error.PromotionManagement.ProductoIncludedAndExcluded", ex.Message);
            Assert.AreEqual(0, fixture.UnidadFactory.CrearCallCount);
        }

        [TestMethod]
        public void Crear_MismaCategoriaIncluidaYExcluida_ThrowsExcepcionNegocio()
        {
            PromocionFixture fixture = new PromocionFixture();
            Promocion datos = fixture.DatosValidos();
            datos.CategoriaIncluidaIds.Add(3);
            datos.CategoriaExcluidaIds.Add(3);
            PromocionBLL sut = fixture.CreateSut();

            ExcepcionNegocio ex = Assert.ThrowsExactly<ExcepcionNegocio>(() => sut.Crear(datos));

            Assert.AreEqual("Error.PromotionManagement.CategoriaIncludedAndExcluded", ex.Message);
        }

        // --- Crear: existencia de cada id referenciado ---

        [TestMethod]
        public void Crear_ComercioInexistente_ThrowsExcepcionNegocioYNoGuarda()
        {
            PromocionFixture fixture = new PromocionFixture();
            Promocion datos = fixture.DatosValidos();
            datos.ComercioIds.Add(99);
            PromocionBLL sut = fixture.CreateSut();

            ExcepcionNegocio ex = Assert.ThrowsExactly<ExcepcionNegocio>(() => sut.Crear(datos));

            Assert.AreEqual("Error.PromotionManagement.ComercioNotFound", ex.Message);
            Assert.IsFalse(fixture.Unidad.SaveCalled);
        }

        [TestMethod]
        public void Crear_RubroInexistente_ThrowsExcepcionNegocio()
        {
            PromocionFixture fixture = new PromocionFixture();
            Promocion datos = fixture.DatosValidos();
            datos.RubroComercioIds.Add(99);
            PromocionBLL sut = fixture.CreateSut();

            ExcepcionNegocio ex = Assert.ThrowsExactly<ExcepcionNegocio>(() => sut.Crear(datos));

            Assert.AreEqual("Error.PromotionManagement.RubroNotFound", ex.Message);
        }

        [TestMethod]
        public void Crear_EntidadInexistente_ThrowsExcepcionNegocio()
        {
            PromocionFixture fixture = new PromocionFixture();
            Promocion datos = fixture.DatosValidos();
            datos.EntidadFinancieraIds.Add(99);
            PromocionBLL sut = fixture.CreateSut();

            ExcepcionNegocio ex = Assert.ThrowsExactly<ExcepcionNegocio>(() => sut.Crear(datos));

            Assert.AreEqual("Error.PromotionManagement.EntidadNotFound", ex.Message);
        }

        [TestMethod]
        public void Crear_MedioPagoInexistente_ThrowsExcepcionNegocio()
        {
            PromocionFixture fixture = new PromocionFixture();
            Promocion datos = fixture.DatosValidos();
            datos.MedioPagoIds.Add(99);
            PromocionBLL sut = fixture.CreateSut();

            ExcepcionNegocio ex = Assert.ThrowsExactly<ExcepcionNegocio>(() => sut.Crear(datos));

            Assert.AreEqual("Error.PromotionManagement.MedioPagoNotFound", ex.Message);
        }

        [TestMethod]
        public void Crear_ProductoInexistente_ThrowsExcepcionNegocio()
        {
            PromocionFixture fixture = new PromocionFixture();
            Promocion datos = fixture.DatosValidos();
            datos.ProductoExcluidoIds.Add(99);
            PromocionBLL sut = fixture.CreateSut();

            ExcepcionNegocio ex = Assert.ThrowsExactly<ExcepcionNegocio>(() => sut.Crear(datos));

            Assert.AreEqual("Error.PromotionManagement.ProductoNotFound", ex.Message);
        }

        [TestMethod]
        public void Crear_CategoriaInexistente_ThrowsExcepcionNegocio()
        {
            PromocionFixture fixture = new PromocionFixture();
            Promocion datos = fixture.DatosValidos();
            datos.CategoriaIncluidaIds.Add(99);
            PromocionBLL sut = fixture.CreateSut();

            ExcepcionNegocio ex = Assert.ThrowsExactly<ExcepcionNegocio>(() => sut.Crear(datos));

            Assert.AreEqual("Error.PromotionManagement.CategoriaProductoNotFound", ex.Message);
        }

        // --- Crear: persistencia ---

        [TestMethod]
        public void Crear_Valido_PersisteDatosYCadaFiltroAuditaYGuardaUnaSolaVez()
        {
            PromocionFixture fixture = new PromocionFixture();
            fixture.SembrarReferencias();
            Promocion datos = fixture.DatosValidos();
            datos.Descripcion = "  Martes de descuento  ";
            datos.TopeDescuento = 5000m;
            datos.DiasSemana.Add(DiaSemana.Martes);
            datos.ComercioIds.Add(1);
            datos.RubroComercioIds.Add(2);
            datos.EntidadFinancieraIds.Add(3);
            datos.MedioPagoIds.Add(4);
            datos.Segmentos.Add("Black");
            datos.ProductoIncluidoIds.Add(5);
            datos.ProductoExcluidoIds.Add(6);
            datos.CategoriaIncluidaIds.Add(7);
            datos.CategoriaExcluidaIds.Add(8);
            PromocionBLL sut = fixture.CreateSut();

            Promocion creada = sut.Crear(datos);

            Promocion guardada = fixture.Unidad.PromocionRepo.Promociones[creada.Id];
            Assert.AreEqual("Martes de descuento", guardada.Descripcion);
            Assert.AreEqual(20m, guardada.Descuento);
            Assert.AreEqual(5000m, guardada.TopeDescuento);
            CollectionAssert.AreEqual(new[] { DiaSemana.Martes }, guardada.DiasSemana);
            CollectionAssert.AreEqual(new[] { 1 }, fixture.Unidad.PromocionComercioRepo.ComercioIdsPorPromocion[creada.Id]);
            CollectionAssert.AreEqual(new[] { 2 }, fixture.Unidad.PromocionRubroComercioRepo.RubroComercioIdsPorPromocion[creada.Id]);
            CollectionAssert.AreEqual(new[] { 3 }, fixture.Unidad.PromocionEntidadFinancieraRepo.EntidadFinancieraIdsPorPromocion[creada.Id]);
            CollectionAssert.AreEqual(new[] { 4 }, fixture.Unidad.PromocionMedioPagoRepo.MedioPagoIdsPorPromocion[creada.Id]);
            CollectionAssert.AreEqual(new[] { "Black" }, fixture.Unidad.PromocionSegmentoRepo.SegmentosPorPromocion[creada.Id]);
            CollectionAssert.AreEqual(new[] { 5 }, fixture.Unidad.PromocionProductoRepo.IncluidosPorPromocion[creada.Id]);
            CollectionAssert.AreEqual(new[] { 6 }, fixture.Unidad.PromocionProductoRepo.ExcluidosPorPromocion[creada.Id]);
            CollectionAssert.AreEqual(new[] { 7 }, fixture.Unidad.PromocionCategoriaProductoRepo.IncluidasPorPromocion[creada.Id]);
            CollectionAssert.AreEqual(new[] { 8 }, fixture.Unidad.PromocionCategoriaProductoRepo.ExcluidasPorPromocion[creada.Id]);
            Assert.AreEqual(1, fixture.Bitacora.Calls.Count);
            Assert.AreEqual("OperacionInterno", fixture.Bitacora.Calls[0].Kind);
            Assert.AreEqual("FormPromotionManagement.Value.Activity.PromocionCreation", fixture.Bitacora.Calls[0].Actividad);
            Assert.IsTrue(fixture.Unidad.SaveCalled);
        }

        [TestMethod]
        public void Crear_ElementosRepetidosEnUnFiltro_SeGuardanUnaSolaVez()
        {
            PromocionFixture fixture = new PromocionFixture();
            fixture.SembrarReferencias();
            Promocion datos = fixture.DatosValidos();
            datos.ComercioIds.AddRange(new[] { 1, 1 });
            datos.Segmentos.AddRange(new[] { "Black", "black", " Black " });
            PromocionBLL sut = fixture.CreateSut();

            Promocion creada = sut.Crear(datos);

            CollectionAssert.AreEqual(new[] { 1 }, fixture.Unidad.PromocionComercioRepo.ComercioIdsPorPromocion[creada.Id]);
            CollectionAssert.AreEqual(new[] { "Black" }, fixture.Unidad.PromocionSegmentoRepo.SegmentosPorPromocion[creada.Id]);
        }

        [TestMethod]
        public void Crear_FallaTecnicaAlInsertar_ThrowsExcepcionTecnicaYNoGuarda()
        {
            PromocionFixture fixture = new PromocionFixture();
            fixture.Unidad.PromocionRepo.ExcepcionEnInsertar = new InvalidOperationException("boom");
            PromocionBLL sut = fixture.CreateSut();

            ExcepcionTecnica ex = Assert.ThrowsExactly<ExcepcionTecnica>(() => sut.Crear(fixture.DatosValidos()));

            Assert.AreEqual("Error.PromotionManagement.PromocionCreateFailed", ex.Message);
            Assert.IsFalse(fixture.Unidad.SaveCalled);
        }

        // --- Actualizar ---

        [TestMethod]
        public void Actualizar_PromocionInexistente_ThrowsExcepcionNegocio()
        {
            PromocionFixture fixture = new PromocionFixture();
            Promocion datos = fixture.DatosValidos();
            datos.Id = 99;
            PromocionBLL sut = fixture.CreateSut();

            ExcepcionNegocio ex = Assert.ThrowsExactly<ExcepcionNegocio>(() => sut.Actualizar(datos));

            Assert.AreEqual("Error.PromotionManagement.PromocionNotFound", ex.Message);
            Assert.IsFalse(fixture.Unidad.SaveCalled);
        }

        [TestMethod]
        public void Actualizar_DescuentoInvalido_ThrowsExcepcionNegocioAndDoesNotCreateUow()
        {
            PromocionFixture fixture = new PromocionFixture();
            Promocion datos = fixture.DatosValidos();
            datos.Id = 1;
            datos.Descuento = 0m;
            PromocionBLL sut = fixture.CreateSut();

            ExcepcionNegocio ex = Assert.ThrowsExactly<ExcepcionNegocio>(() => sut.Actualizar(datos));

            Assert.AreEqual("Error.PromotionManagement.DiscountInvalid", ex.Message);
            Assert.AreEqual(0, fixture.UnidadFactory.CrearCallCount);
        }

        [TestMethod]
        public void Actualizar_Valido_ActualizaDatosReemplazaFiltrosAuditaYGuarda()
        {
            PromocionFixture fixture = new PromocionFixture();
            fixture.SembrarReferencias();
            fixture.AgregarPromocion(1, "Original", new DateTime(2026, 3, 1));
            Promocion datos = fixture.DatosValidos();
            datos.Id = 1;
            datos.Descripcion = "Editada";
            datos.TopeDescuento = 3000m;
            datos.CategoriaExcluidaIds.Add(8);
            datos.DiasSemana.Add(DiaSemana.Viernes);
            PromocionBLL sut = fixture.CreateSut();

            bool actualizado = sut.Actualizar(datos);

            Assert.IsTrue(actualizado);
            Promocion guardada = fixture.Unidad.PromocionRepo.Promociones[1];
            Assert.AreEqual("Editada", guardada.Descripcion);
            Assert.AreEqual(3000m, guardada.TopeDescuento);
            CollectionAssert.AreEqual(new[] { DiaSemana.Viernes }, guardada.DiasSemana);
            CollectionAssert.AreEqual(new[] { 8 }, fixture.Unidad.PromocionCategoriaProductoRepo.ExcluidasPorPromocion[1]);
            CollectionAssert.AreEqual(new int[0], fixture.Unidad.PromocionComercioRepo.ComercioIdsPorPromocion[1], "Un filtro vacio reemplaza al anterior.");
            Assert.AreEqual("FormPromotionManagement.Value.Activity.PromocionUpdate", fixture.Bitacora.Calls[0].Actividad);
            Assert.IsTrue(fixture.Unidad.SaveCalled);
        }

        [TestMethod]
        public void Actualizar_RepositorioDevuelveFalse_ThrowsExcepcionTecnicaYNoGuarda()
        {
            PromocionFixture fixture = new PromocionFixture();
            fixture.AgregarPromocion(1, "Original", new DateTime(2026, 3, 1));
            fixture.Unidad.PromocionRepo.ActualizarResult = false;
            Promocion datos = fixture.DatosValidos();
            datos.Id = 1;
            PromocionBLL sut = fixture.CreateSut();

            ExcepcionTecnica ex = Assert.ThrowsExactly<ExcepcionTecnica>(() => sut.Actualizar(datos));

            Assert.AreEqual("Error.PromotionManagement.PromocionUpdateFailed", ex.Message);
            Assert.IsFalse(fixture.Unidad.SaveCalled);
        }

        // --- Eliminar ---

        [TestMethod]
        public void Eliminar_PromocionInexistente_ThrowsExcepcionNegocio()
        {
            PromocionFixture fixture = new PromocionFixture();
            PromocionBLL sut = fixture.CreateSut();

            ExcepcionNegocio ex = Assert.ThrowsExactly<ExcepcionNegocio>(() => sut.Eliminar(99));

            Assert.AreEqual("Error.PromotionManagement.PromocionNotFound", ex.Message);
        }

        [TestMethod]
        public void Eliminar_ReferenciadaPorUnaNotificacion_ThrowsEnUsoYNoBorraNada()
        {
            PromocionFixture fixture = new PromocionFixture();
            fixture.AgregarPromocion(1, "Con notificacion", new DateTime(2026, 3, 1));
            fixture.Unidad.NotificacionRepo.NotificacionesPorPromocion[1] = 1;
            PromocionBLL sut = fixture.CreateSut();

            ExcepcionNegocio ex = Assert.ThrowsExactly<ExcepcionNegocio>(() => sut.Eliminar(1));

            Assert.AreEqual("Error.PromotionManagement.PromocionInUse", ex.Message);
            Assert.IsNull(fixture.Unidad.PromocionRepo.LastDeletedId, "No se emitio ninguna baja.");
            Assert.IsFalse(fixture.Unidad.PromocionComercioRepo.LastPromocionIdReemplazado.HasValue, "No se vaciaron filtros.");
            Assert.IsTrue(fixture.Unidad.PromocionRepo.Promociones.ContainsKey(1));
            Assert.IsFalse(fixture.Unidad.SaveCalled);
        }

        [TestMethod]
        public void Eliminar_SinUso_VaciaLosFiltrosEliminaAuditaYGuarda()
        {
            PromocionFixture fixture = new PromocionFixture();
            Promocion promocion = fixture.AgregarPromocion(1, "Sin uso", new DateTime(2026, 3, 1));
            promocion.ComercioIds.Add(1);
            promocion.ProductoIncluidoIds.Add(5);
            PromocionBLL sut = fixture.CreateSut();

            bool eliminado = sut.Eliminar(1);

            Assert.IsTrue(eliminado);
            Assert.AreEqual(1, fixture.Unidad.PromocionRepo.LastDeletedId);
            Assert.IsFalse(fixture.Unidad.PromocionRepo.Promociones.ContainsKey(1));
            Assert.AreEqual(0, fixture.Unidad.PromocionComercioRepo.ComercioIdsPorPromocion[1].Count);
            Assert.AreEqual(0, fixture.Unidad.PromocionRubroComercioRepo.RubroComercioIdsPorPromocion[1].Count);
            Assert.AreEqual(0, fixture.Unidad.PromocionEntidadFinancieraRepo.EntidadFinancieraIdsPorPromocion[1].Count);
            Assert.AreEqual(0, fixture.Unidad.PromocionMedioPagoRepo.MedioPagoIdsPorPromocion[1].Count);
            Assert.AreEqual(0, fixture.Unidad.PromocionSegmentoRepo.SegmentosPorPromocion[1].Count);
            Assert.AreEqual(0, fixture.Unidad.PromocionProductoRepo.IncluidosPorPromocion[1].Count);
            Assert.AreEqual(0, fixture.Unidad.PromocionProductoRepo.ExcluidosPorPromocion[1].Count);
            Assert.AreEqual(0, fixture.Unidad.PromocionCategoriaProductoRepo.IncluidasPorPromocion[1].Count);
            Assert.AreEqual(0, fixture.Unidad.PromocionCategoriaProductoRepo.ExcluidasPorPromocion[1].Count);
            Assert.AreEqual("FormPromotionManagement.Value.Activity.PromocionDeletion", fixture.Bitacora.Calls[0].Actividad);
            Assert.IsTrue(fixture.Unidad.SaveCalled);
        }

        [TestMethod]
        public void Eliminar_RepositorioDevuelveFalse_ThrowsExcepcionTecnicaYNoGuarda()
        {
            PromocionFixture fixture = new PromocionFixture();
            fixture.AgregarPromocion(1, "Sin uso", new DateTime(2026, 3, 1));
            fixture.Unidad.PromocionRepo.EliminarResult = false;
            PromocionBLL sut = fixture.CreateSut();

            ExcepcionTecnica ex = Assert.ThrowsExactly<ExcepcionTecnica>(() => sut.Eliminar(1));

            Assert.AreEqual("Error.PromotionManagement.PromocionDeleteFailed", ex.Message);
            Assert.IsFalse(fixture.Unidad.SaveCalled);
        }

        [TestMethod]
        public void Eliminar_FallaDeLaBaseConViolacionForanea_SeInformaComoFallaTecnica()
        {
            PromocionFixture fixture = new PromocionFixture();
            fixture.AgregarPromocion(1, "Sin uso", new DateTime(2026, 3, 1));
            fixture.Unidad.PromocionRepo.ExcepcionEnEliminar = FakeSqlExceptionFactory.CrearViolacionForanea();
            PromocionBLL sut = fixture.CreateSut();

            ExcepcionTecnica ex = Assert.ThrowsExactly<ExcepcionTecnica>(() => sut.Eliminar(1));

            Assert.AreEqual("Error.PromotionManagement.PromocionDeleteFailed", ex.Message);
            Assert.IsFalse(fixture.Unidad.SaveCalled);
        }

        private sealed class PromocionFixture
        {
            public readonly SpyBitacoraBLL Bitacora = new SpyBitacoraBLL();
            public readonly FakeUnidadDeTrabajo Unidad;
            public readonly FakeUnidadDeTrabajoFactory UnidadFactory;

            public PromocionFixture()
            {
                Unidad = new FakeUnidadDeTrabajo();
                UnidadFactory = new FakeUnidadDeTrabajoFactory(Unidad);
            }

            public PromocionBLL CreateSut()
            {
                return new PromocionBLL(
                    Unidad.PromocionRepo,
                    Unidad.ComercioRepo,
                    Unidad.MedioPagoRepo,
                    Unidad.MedioPagoEntidadRepo,
                    UnidadFactory,
                    Bitacora);
            }

            public Promocion AgregarPromocion(int id, string descripcion, DateTime vigenciaDesde)
            {
                Promocion promocion = new Promocion
                {
                    Id = id,
                    Descripcion = descripcion,
                    Descuento = 10m,
                    VigenciaDesde = vigenciaDesde,
                    VigenciaHasta = vigenciaDesde.AddDays(30)
                };
                Unidad.PromocionRepo.Promociones.Add(id, promocion);
                return promocion;
            }

            public Promocion DatosValidos()
            {
                return new Promocion
                {
                    Descripcion = "Promo valida",
                    Descuento = 20m,
                    VigenciaDesde = new DateTime(2026, 3, 1),
                    VigenciaHasta = new DateTime(2026, 3, 31)
                };
            }

            // Un registro por cada id que DatosValidos referencia en los tests de persistencia.
            public void SembrarReferencias()
            {
                Unidad.ComercioRepo.Comercios.Add(1, new Comercio { Id = 1 });
                Unidad.RubroComercioRepo.Rubros.Add(2, new RubroComercio { Id = 2 });
                Unidad.EntidadFinancieraRepo.Entidades.Add(3, new EntidadFinanciera { Id = 3 });
                Unidad.MedioPagoRepo.MediosPago.Add(4, new MedioPago { Id = 4 });
                Unidad.ProductoRepo.Productos.Add(5, new Producto { Id = 5 });
                Unidad.ProductoRepo.Productos.Add(6, new Producto { Id = 6 });
                Unidad.CategoriaProductoRepo.Categorias.Add(7, new CategoriaProducto { Id = 7 });
                Unidad.CategoriaProductoRepo.Categorias.Add(8, new CategoriaProducto { Id = 8 });
            }
        }
    }
}
