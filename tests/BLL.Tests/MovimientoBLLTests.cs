using System;
using System.Linq;
using ABSTRACCIONES;
using BE;
using BLL.Tests.TestDoubles;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SERVICIOS;

namespace BLL.Tests
{
    [TestClass]
    public sealed class MovimientoBLLTests
    {
        [TestMethod]
        public void Crear_MontoNoPositivo_ThrowsExcepcionNegocioAndDoesNotCreateUow()
        {
            MovimientoFixture fixture = new MovimientoFixture(usuarioActualId: 1);
            fixture.CategoriaRepo.Categorias.Add(1, new Categoria { Id = 1, Nombre = "Global", UsuarioId = null });
            MovimientoBLL sut = fixture.CreateSut();

            Assert.ThrowsExactly<ExcepcionNegocio>(() => sut.Crear(1, TipoMovimiento.Ingreso, 0m, new DateTime(2026, 8, 1), "x"));
            Assert.AreEqual(0, fixture.UnidadFactory.CrearCallCount);
        }

        [TestMethod]
        public void Crear_CategoriaInexistente_ThrowsExcepcionNegocio()
        {
            MovimientoFixture fixture = new MovimientoFixture(usuarioActualId: 1);
            MovimientoBLL sut = fixture.CreateSut();

            Assert.ThrowsExactly<ExcepcionNegocio>(() => sut.Crear(99, TipoMovimiento.Ingreso, 100m, new DateTime(2026, 8, 1), null));
        }

        [TestMethod]
        public void Crear_CategoriaDeOtroUsuario_ThrowsExcepcionNegocio()
        {
            MovimientoFixture fixture = new MovimientoFixture(usuarioActualId: 1);
            fixture.CategoriaRepo.Categorias.Add(5, new Categoria { Id = 5, Nombre = "Ajena", UsuarioId = 99 });
            MovimientoBLL sut = fixture.CreateSut();

            Assert.ThrowsExactly<ExcepcionNegocio>(() => sut.Crear(5, TipoMovimiento.Ingreso, 100m, new DateTime(2026, 8, 1), null));
        }

        [TestMethod]
        public void Crear_Valido_InsertaComoPropioDelUsuarioActualAuditaYGuarda()
        {
            MovimientoFixture fixture = new MovimientoFixture(usuarioActualId: 7);
            fixture.CategoriaRepo.Categorias.Add(1, new Categoria { Id = 1, Nombre = "Global", UsuarioId = null });
            MovimientoBLL sut = fixture.CreateSut();

            int id = sut.Crear(1, TipoMovimiento.Egreso, 250.50m, new DateTime(2026, 8, 15), "Compra");

            Movimiento insertado = fixture.Unidad.MovimientoRepo.Movimientos[id];
            Assert.AreEqual(7, insertado.UsuarioId);
            Assert.AreEqual(1, insertado.CategoriaId);
            Assert.AreEqual(TipoMovimiento.Egreso, insertado.Tipo);
            Assert.AreEqual(250.50m, insertado.Monto);
            Assert.AreEqual("Compra", insertado.Descripcion);
            Assert.AreEqual("OperacionInterno", fixture.Bitacora.Calls.Single().Kind);
            Assert.IsTrue(fixture.Unidad.SaveCalled);
        }

        [TestMethod]
        public void Actualizar_MovimientoDeOtroUsuario_ThrowsExcepcionNegocio()
        {
            MovimientoFixture fixture = new MovimientoFixture(usuarioActualId: 1);
            fixture.CategoriaRepo.Categorias.Add(1, new Categoria { Id = 1, Nombre = "Global", UsuarioId = null });
            fixture.Unidad.MovimientoRepo.Movimientos.Add(3, new Movimiento { Id = 3, UsuarioId = 99, CategoriaId = 1, Tipo = TipoMovimiento.Ingreso, Monto = 10m, Fecha = new DateTime(2026, 8, 1) });
            MovimientoBLL sut = fixture.CreateSut();

            Assert.ThrowsExactly<ExcepcionNegocio>(() => sut.Actualizar(3, 1, TipoMovimiento.Ingreso, 20m, new DateTime(2026, 8, 2), null));
        }

        [TestMethod]
        public void Actualizar_Valido_ActualizaAuditaYGuarda()
        {
            MovimientoFixture fixture = new MovimientoFixture(usuarioActualId: 5);
            fixture.CategoriaRepo.Categorias.Add(1, new Categoria { Id = 1, Nombre = "Global", UsuarioId = null });
            fixture.Unidad.MovimientoRepo.Movimientos.Add(3, new Movimiento { Id = 3, UsuarioId = 5, CategoriaId = 1, Tipo = TipoMovimiento.Ingreso, Monto = 10m, Fecha = new DateTime(2026, 8, 1) });
            MovimientoBLL sut = fixture.CreateSut();

            bool resultado = sut.Actualizar(3, 1, TipoMovimiento.Egreso, 999m, new DateTime(2026, 8, 20), "Editado");

            Assert.IsTrue(resultado);
            Movimiento actualizado = fixture.Unidad.MovimientoRepo.Movimientos[3];
            Assert.AreEqual(TipoMovimiento.Egreso, actualizado.Tipo);
            Assert.AreEqual(999m, actualizado.Monto);
            Assert.AreEqual("Editado", actualizado.Descripcion);
            Assert.AreEqual("OperacionInterno", fixture.Bitacora.Calls.Single().Kind);
            Assert.IsTrue(fixture.Unidad.SaveCalled);
        }

        [TestMethod]
        public void Eliminar_MovimientoDeOtroUsuario_ThrowsExcepcionNegocio()
        {
            MovimientoFixture fixture = new MovimientoFixture(usuarioActualId: 1);
            fixture.Unidad.MovimientoRepo.Movimientos.Add(3, new Movimiento { Id = 3, UsuarioId = 99, CategoriaId = 1, Tipo = TipoMovimiento.Ingreso, Monto = 10m, Fecha = new DateTime(2026, 8, 1) });
            MovimientoBLL sut = fixture.CreateSut();

            Assert.ThrowsExactly<ExcepcionNegocio>(() => sut.Eliminar(3));
        }

        [TestMethod]
        public void Eliminar_Valido_EliminaAuditaYGuarda()
        {
            MovimientoFixture fixture = new MovimientoFixture(usuarioActualId: 5);
            fixture.Unidad.MovimientoRepo.Movimientos.Add(3, new Movimiento { Id = 3, UsuarioId = 5, CategoriaId = 1, Tipo = TipoMovimiento.Ingreso, Monto = 10m, Fecha = new DateTime(2026, 8, 1) });
            MovimientoBLL sut = fixture.CreateSut();

            bool resultado = sut.Eliminar(3);

            Assert.IsTrue(resultado);
            Assert.IsFalse(fixture.Unidad.MovimientoRepo.Movimientos.ContainsKey(3));
            Assert.AreEqual("OperacionInterno", fixture.Bitacora.Calls.Single().Kind);
            Assert.IsTrue(fixture.Unidad.SaveCalled);
        }

        [TestMethod]
        public void ObtenerPorUsuario_DelegaAlRepositorio()
        {
            MovimientoFixture fixture = new MovimientoFixture(usuarioActualId: 5);
            fixture.MovimientoRepo.Movimientos.Add(1, new Movimiento { Id = 1, UsuarioId = 5, CategoriaId = 1, Tipo = TipoMovimiento.Ingreso, Monto = 10m, Fecha = new DateTime(2026, 8, 1) });
            MovimientoBLL sut = fixture.CreateSut();

            System.Collections.Generic.List<Movimiento> resultado = sut.ObtenerPorUsuario(5, null);

            Assert.AreEqual(1, resultado.Count);
        }

        private sealed class MovimientoFixture
        {
            public readonly FakeMovimientoRepositorio MovimientoRepo = new FakeMovimientoRepositorio();
            public readonly FakeCategoriaRepositorio CategoriaRepo = new FakeCategoriaRepositorio();
            public readonly SpyBitacoraBLL Bitacora = new SpyBitacoraBLL();
            public readonly FakeSesion Sesion = new FakeSesion();
            public readonly FakeUnidadDeTrabajo Unidad;
            public readonly FakeUnidadDeTrabajoFactory UnidadFactory;

            public MovimientoFixture(int usuarioActualId)
            {
                Unidad = new FakeUnidadDeTrabajo { MovimientoRepo = MovimientoRepo, CategoriaRepo = CategoriaRepo };
                UnidadFactory = new FakeUnidadDeTrabajoFactory(Unidad);
                Sesion.IniciarSesion(new Usuario { Id = usuarioActualId, Username = "test" });
            }

            public MovimientoBLL CreateSut()
            {
                return new MovimientoBLL(MovimientoRepo, CategoriaRepo, UnidadFactory, Sesion, Bitacora);
            }
        }
    }
}
