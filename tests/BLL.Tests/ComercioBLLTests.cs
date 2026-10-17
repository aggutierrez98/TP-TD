using System.Collections.Generic;
using BE;
using BLL.Tests.TestDoubles;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SERVICIOS;

namespace BLL.Tests
{
    [TestClass]
    public sealed class ComercioBLLTests
    {
        [TestMethod]
        public void Crear_NombreVacio_ThrowsExcepcionNegocioAndDoesNotCreateUow()
        {
            ComercioFixture fixture = new ComercioFixture();
            ComercioBLL sut = fixture.CreateSut();

            Assert.ThrowsExactly<ExcepcionNegocio>(() => sut.Crear("   ", 1, null, null, null, null, null));
            Assert.AreEqual(0, fixture.UnidadFactory.CrearCallCount);
        }

        [TestMethod]
        public void Crear_RubroInexistente_ThrowsExcepcionNegocio()
        {
            ComercioFixture fixture = new ComercioFixture();
            ComercioBLL sut = fixture.CreateSut();

            Assert.ThrowsExactly<ExcepcionNegocio>(() => sut.Crear("Dia", 99, null, null, null, null, null));
            Assert.IsFalse(fixture.Unidad.SaveCalled);
        }

        [TestMethod]
        public void Crear_Valido_InsertaAuditaYGuarda()
        {
            ComercioFixture fixture = new ComercioFixture();
            fixture.Unidad.RubroComercioRepo.Rubros.Add(1, new RubroComercio { Id = 1, Nombre = "Supermercado" });
            ComercioBLL sut = fixture.CreateSut();

            Comercio creado = sut.Crear("  Dia  ", 1, "  Av. Siempre Viva 123  ", "  CABA  ", "  Buenos Aires  ", -34.6037m, -58.3816m);

            Comercio insertado = fixture.Unidad.ComercioRepo.Comercios[creado.Id];
            Assert.AreEqual("Dia", insertado.Nombre);
            Assert.AreEqual(1, insertado.RubroComercio.Id);
            Assert.AreEqual("Av. Siempre Viva 123", insertado.Direccion);
            Assert.AreEqual("CABA", insertado.Localidad);
            Assert.AreEqual("Buenos Aires", insertado.Provincia);
            Assert.AreEqual(-34.6037m, insertado.Latitud);
            Assert.AreEqual(-58.3816m, insertado.Longitud);
            Assert.AreEqual("OperacionInterno", fixture.Bitacora.Calls[0].Kind);
            Assert.IsTrue(fixture.Unidad.SaveCalled);
        }

        [TestMethod]
        public void Crear_TextosVacios_SeGuardanComoNull()
        {
            ComercioFixture fixture = new ComercioFixture();
            fixture.Unidad.RubroComercioRepo.Rubros.Add(1, new RubroComercio { Id = 1, Nombre = "Supermercado" });
            ComercioBLL sut = fixture.CreateSut();

            Comercio creado = sut.Crear("Dia", 1, "   ", "   ", "   ", null, null);

            Comercio insertado = fixture.Unidad.ComercioRepo.Comercios[creado.Id];
            Assert.IsNull(insertado.Direccion);
            Assert.IsNull(insertado.Localidad);
            Assert.IsNull(insertado.Provincia);
            Assert.IsNull(insertado.Latitud);
            Assert.IsNull(insertado.Longitud);
        }

        [TestMethod]
        public void Crear_SoloLatitudSinLongitud_ThrowsExcepcionNegocio()
        {
            ComercioFixture fixture = new ComercioFixture();
            fixture.Unidad.RubroComercioRepo.Rubros.Add(1, new RubroComercio { Id = 1, Nombre = "Supermercado" });
            ComercioBLL sut = fixture.CreateSut();

            Assert.ThrowsExactly<ExcepcionNegocio>(() => sut.Crear("Dia", 1, null, null, null, -34.6037m, null));
            Assert.AreEqual(0, fixture.UnidadFactory.CrearCallCount);
        }

        [TestMethod]
        public void Crear_SoloLongitudSinLatitud_ThrowsExcepcionNegocio()
        {
            ComercioFixture fixture = new ComercioFixture();
            fixture.Unidad.RubroComercioRepo.Rubros.Add(1, new RubroComercio { Id = 1, Nombre = "Supermercado" });
            ComercioBLL sut = fixture.CreateSut();

            Assert.ThrowsExactly<ExcepcionNegocio>(() => sut.Crear("Dia", 1, null, null, null, null, -58.3816m));
            Assert.AreEqual(0, fixture.UnidadFactory.CrearCallCount);
        }

        [TestMethod]
        public void Actualizar_ComercioInexistente_ThrowsExcepcionNegocio()
        {
            ComercioFixture fixture = new ComercioFixture();
            ComercioBLL sut = fixture.CreateSut();

            Assert.ThrowsExactly<ExcepcionNegocio>(() => sut.Actualizar(99, "Nuevo", 1, null, null, null, null, null));
        }

        [TestMethod]
        public void Actualizar_RubroInexistente_ThrowsExcepcionNegocio()
        {
            ComercioFixture fixture = new ComercioFixture();
            fixture.Unidad.ComercioRepo.Comercios.Add(1, new Comercio { Id = 1, Nombre = "Dia", RubroComercio = new RubroComercio { Id = 1 } });
            ComercioBLL sut = fixture.CreateSut();

            Assert.ThrowsExactly<ExcepcionNegocio>(() => sut.Actualizar(1, "Nuevo", 99, null, null, null, null, null));
        }

        [TestMethod]
        public void Actualizar_SoloLatitudSinLongitud_ThrowsExcepcionNegocio()
        {
            ComercioFixture fixture = new ComercioFixture();
            fixture.Unidad.ComercioRepo.Comercios.Add(1, new Comercio { Id = 1, Nombre = "Dia", RubroComercio = new RubroComercio { Id = 1 } });
            ComercioBLL sut = fixture.CreateSut();

            Assert.ThrowsExactly<ExcepcionNegocio>(() => sut.Actualizar(1, "Nuevo", 1, null, null, null, -34.6037m, null));
        }

        [TestMethod]
        public void Actualizar_Valido_ActualizaAuditaYGuarda()
        {
            ComercioFixture fixture = new ComercioFixture();
            fixture.Unidad.RubroComercioRepo.Rubros.Add(2, new RubroComercio { Id = 2, Nombre = "Farmacia" });
            fixture.Unidad.ComercioRepo.Comercios.Add(1, new Comercio { Id = 1, Nombre = "Viejo", RubroComercio = new RubroComercio { Id = 1 } });
            ComercioBLL sut = fixture.CreateSut();

            bool resultado = sut.Actualizar(1, "  Nuevo Nombre  ", 2, "  Av. Corrientes 1234  ", "  CABA  ", "  Buenos Aires  ", -34.6m, -58.4m);

            Assert.IsTrue(resultado);
            Comercio actualizado = fixture.Unidad.ComercioRepo.Comercios[1];
            Assert.AreEqual("Nuevo Nombre", actualizado.Nombre);
            Assert.AreEqual(2, actualizado.RubroComercio.Id);
            Assert.AreEqual("Av. Corrientes 1234", actualizado.Direccion);
            Assert.AreEqual("CABA", actualizado.Localidad);
            Assert.AreEqual("Buenos Aires", actualizado.Provincia);
            Assert.AreEqual(-34.6m, actualizado.Latitud);
            Assert.AreEqual(-58.4m, actualizado.Longitud);
            Assert.AreEqual("OperacionInterno", fixture.Bitacora.Calls[0].Kind);
            Assert.IsTrue(fixture.Unidad.SaveCalled);
        }

        [TestMethod]
        public void Eliminar_ComercioInexistente_ThrowsExcepcionNegocio()
        {
            ComercioFixture fixture = new ComercioFixture();
            ComercioBLL sut = fixture.CreateSut();

            Assert.ThrowsExactly<ExcepcionNegocio>(() => sut.Eliminar(99));
        }

        [TestMethod]
        public void Eliminar_ConProductosAsociados_ThrowsExcepcionNegocio()
        {
            // Producto esta fuera de esta tanda: se simula la violacion de FK que la base
            // real levantaria (FK_Producto_Comercio) para verificar que el BLL la convierte
            // en un error de negocio en vez de dejarla explotar como ExcepcionTecnica.
            ComercioFixture fixture = new ComercioFixture();
            fixture.Unidad.ComercioRepo.Comercios.Add(1, new Comercio { Id = 1, Nombre = "Dia", RubroComercio = new RubroComercio { Id = 1 } });
            fixture.Unidad.ComercioRepo.ThrowForeignKeyOnEliminar = true;
            ComercioBLL sut = fixture.CreateSut();

            Assert.ThrowsExactly<ExcepcionNegocio>(() => sut.Eliminar(1));
        }

        [TestMethod]
        public void Eliminar_Valido_EliminaAuditaYGuarda()
        {
            ComercioFixture fixture = new ComercioFixture();
            fixture.Unidad.ComercioRepo.Comercios.Add(1, new Comercio { Id = 1, Nombre = "Dia", RubroComercio = new RubroComercio { Id = 1 } });
            ComercioBLL sut = fixture.CreateSut();

            bool resultado = sut.Eliminar(1);

            Assert.IsTrue(resultado);
            Assert.IsFalse(fixture.Unidad.ComercioRepo.Comercios.ContainsKey(1));
            Assert.AreEqual("OperacionInterno", fixture.Bitacora.Calls[0].Kind);
            Assert.IsTrue(fixture.Unidad.SaveCalled);
        }

        [TestMethod]
        public void ObtenerTodos_DelegaAlRepositorio()
        {
            ComercioFixture fixture = new ComercioFixture();
            fixture.ComercioRepo.Comercios.Add(1, new Comercio { Id = 1, Nombre = "Dia", RubroComercio = new RubroComercio { Id = 1 } });
            fixture.ComercioRepo.Comercios.Add(2, new Comercio { Id = 2, Nombre = "Coto", RubroComercio = new RubroComercio { Id = 1 } });
            ComercioBLL sut = fixture.CreateSut();

            List<Comercio> todos = sut.ObtenerTodos();

            Assert.AreEqual(2, todos.Count);
        }

        private sealed class ComercioFixture
        {
            public readonly FakeComercioRepositorio ComercioRepo = new FakeComercioRepositorio();
            public readonly SpyBitacoraBLL Bitacora = new SpyBitacoraBLL();
            public readonly FakeUnidadDeTrabajo Unidad;
            public readonly FakeUnidadDeTrabajoFactory UnidadFactory;

            public ComercioFixture()
            {
                Unidad = new FakeUnidadDeTrabajo { ComercioRepo = ComercioRepo };
                UnidadFactory = new FakeUnidadDeTrabajoFactory(Unidad);
            }

            public ComercioBLL CreateSut()
            {
                return new ComercioBLL(ComercioRepo, UnidadFactory, Bitacora);
            }
        }
    }
}
