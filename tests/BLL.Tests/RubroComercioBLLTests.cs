using System.Collections.Generic;
using BE;
using BLL.Tests.TestDoubles;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SERVICIOS;

namespace BLL.Tests
{
    [TestClass]
    public sealed class RubroComercioBLLTests
    {
        [TestMethod]
        public void Crear_NombreVacio_ThrowsExcepcionNegocioAndDoesNotCreateUow()
        {
            RubroComercioFixture fixture = new RubroComercioFixture();
            RubroComercioBLL sut = fixture.CreateSut();

            Assert.ThrowsExactly<ExcepcionNegocio>(() => sut.Crear("   "));
            Assert.AreEqual(0, fixture.UnidadFactory.CrearCallCount);
        }

        [TestMethod]
        public void Crear_Valido_InsertaAuditaYGuarda()
        {
            RubroComercioFixture fixture = new RubroComercioFixture();
            RubroComercioBLL sut = fixture.CreateSut();

            RubroComercio creado = sut.Crear("  Supermercado  ");

            RubroComercio insertado = fixture.Unidad.RubroComercioRepo.Rubros[creado.Id];
            Assert.AreEqual("Supermercado", insertado.Nombre);
            Assert.AreEqual("OperacionInterno", fixture.Bitacora.Calls[0].Kind);
            Assert.IsTrue(fixture.Unidad.SaveCalled);
        }

        [TestMethod]
        public void Actualizar_Inexistente_ThrowsExcepcionNegocio()
        {
            RubroComercioFixture fixture = new RubroComercioFixture();
            RubroComercioBLL sut = fixture.CreateSut();

            Assert.ThrowsExactly<ExcepcionNegocio>(() => sut.Actualizar(99, "Nuevo"));
        }

        [TestMethod]
        public void Actualizar_Valido_ActualizaAuditaYGuarda()
        {
            RubroComercioFixture fixture = new RubroComercioFixture();
            fixture.Unidad.RubroComercioRepo.Rubros.Add(1, new RubroComercio { Id = 1, Nombre = "Viejo" });
            RubroComercioBLL sut = fixture.CreateSut();

            bool resultado = sut.Actualizar(1, "  Nuevo Rubro  ");

            Assert.IsTrue(resultado);
            Assert.AreEqual("Nuevo Rubro", fixture.Unidad.RubroComercioRepo.Rubros[1].Nombre);
            Assert.AreEqual("OperacionInterno", fixture.Bitacora.Calls[0].Kind);
            Assert.IsTrue(fixture.Unidad.SaveCalled);
        }

        [TestMethod]
        public void Eliminar_Inexistente_ThrowsExcepcionNegocio()
        {
            RubroComercioFixture fixture = new RubroComercioFixture();
            RubroComercioBLL sut = fixture.CreateSut();

            Assert.ThrowsExactly<ExcepcionNegocio>(() => sut.Eliminar(99));
        }

        [TestMethod]
        public void Eliminar_ConComerciosAsociados_ThrowsExcepcionNegocio()
        {
            RubroComercioFixture fixture = new RubroComercioFixture();
            fixture.Unidad.RubroComercioRepo.Rubros.Add(1, new RubroComercio { Id = 1, Nombre = "Supermercado" });
            fixture.Unidad.ComercioRepo.Comercios.Add(10, new Comercio { Id = 10, Nombre = "Dia", RubroComercio = new RubroComercio { Id = 1 } });
            RubroComercioBLL sut = fixture.CreateSut();

            Assert.ThrowsExactly<ExcepcionNegocio>(() => sut.Eliminar(1));
            Assert.IsNull(fixture.Unidad.RubroComercioRepo.LastDeletedId);
            Assert.IsFalse(fixture.Unidad.SaveCalled);
        }

        [TestMethod]
        public void Eliminar_FallaDelBorradoConViolacionForanea_InformaDeleteFailed()
        {
            RubroComercioFixture fixture = new RubroComercioFixture();
            fixture.Unidad.RubroComercioRepo.Rubros.Add(1, new RubroComercio { Id = 1, Nombre = "Supermercado" });
            fixture.Unidad.RubroComercioRepo.ExcepcionEnEliminar = FakeSqlExceptionFactory.CrearViolacionForanea();
            RubroComercioBLL sut = fixture.CreateSut();

            ExcepcionTecnica ex = Assert.ThrowsExactly<ExcepcionTecnica>(() => sut.Eliminar(1));

            Assert.AreEqual("Error.MerchantManagement.RubroDeleteFailed", ex.Message);
            Assert.IsFalse(fixture.Unidad.SaveCalled);
        }

        [TestMethod]
        public void Eliminar_FallaTecnicaConViolacionForaneaEnvuelta_NoSeConvierteEnUso()
        {
            RubroComercioFixture fixture = new RubroComercioFixture();
            fixture.Unidad.RubroComercioRepo.Rubros.Add(1, new RubroComercio { Id = 1, Nombre = "Supermercado" });
            fixture.Unidad.RubroComercioRepo.ExcepcionEnEliminar = FakeSqlExceptionFactory.CrearViolacionForaneaEnvuelta();
            RubroComercioBLL sut = fixture.CreateSut();

            ExcepcionTecnica ex = Assert.ThrowsExactly<ExcepcionTecnica>(() => sut.Eliminar(1));

            Assert.AreEqual("Error.Technical.SqlFailure", ex.Message);
            Assert.IsFalse(fixture.Unidad.SaveCalled);
        }

        [TestMethod]
        public void Eliminar_Valido_EliminaAuditaYGuarda()
        {
            RubroComercioFixture fixture = new RubroComercioFixture();
            fixture.Unidad.RubroComercioRepo.Rubros.Add(1, new RubroComercio { Id = 1, Nombre = "Supermercado" });
            RubroComercioBLL sut = fixture.CreateSut();

            bool resultado = sut.Eliminar(1);

            Assert.IsTrue(resultado);
            Assert.IsFalse(fixture.Unidad.RubroComercioRepo.Rubros.ContainsKey(1));
            Assert.AreEqual("OperacionInterno", fixture.Bitacora.Calls[0].Kind);
            Assert.IsTrue(fixture.Unidad.SaveCalled);
        }

        [TestMethod]
        public void ObtenerTodas_DelegaAlRepositorio()
        {
            RubroComercioFixture fixture = new RubroComercioFixture();
            fixture.RubroComercioRepo.Rubros.Add(1, new RubroComercio { Id = 1, Nombre = "Supermercado" });
            fixture.RubroComercioRepo.Rubros.Add(2, new RubroComercio { Id = 2, Nombre = "Farmacia" });
            RubroComercioBLL sut = fixture.CreateSut();

            List<RubroComercio> todas = sut.ObtenerTodas();

            Assert.AreEqual(2, todas.Count);
        }

        [TestMethod]
        public void Eliminar_ReferenciadoPorUnaPromocion_ThrowsExcepcionNegocioYNoElimina()
        {
            RubroComercioFixture fixture = new RubroComercioFixture();
            fixture.Unidad.RubroComercioRepo.Rubros.Add(1, new RubroComercio { Id = 1, Nombre = "Supermercado" });
            fixture.Unidad.PromocionRubroComercioRepo.RubroComercioIdsPorPromocion[7] = new System.Collections.Generic.List<int> { 1 };
            RubroComercioBLL sut = fixture.CreateSut();

            ExcepcionNegocio ex = Assert.ThrowsExactly<ExcepcionNegocio>(() => sut.Eliminar(1));

            Assert.AreEqual("Error.MerchantManagement.RubroInUse", ex.Message);
            Assert.IsNull(fixture.Unidad.RubroComercioRepo.LastDeletedId);
            Assert.IsFalse(fixture.Unidad.SaveCalled);
        }

        private sealed class RubroComercioFixture
        {
            public readonly FakeRubroComercioRepositorio RubroComercioRepo = new FakeRubroComercioRepositorio();
            public readonly SpyBitacoraBLL Bitacora = new SpyBitacoraBLL();
            public readonly FakeUnidadDeTrabajo Unidad;
            public readonly FakeUnidadDeTrabajoFactory UnidadFactory;

            public RubroComercioFixture()
            {
                Unidad = new FakeUnidadDeTrabajo { RubroComercioRepo = RubroComercioRepo };
                UnidadFactory = new FakeUnidadDeTrabajoFactory(Unidad);
            }

            public RubroComercioBLL CreateSut()
            {
                return new RubroComercioBLL(RubroComercioRepo, UnidadFactory, Bitacora);
            }
        }
    }
}
