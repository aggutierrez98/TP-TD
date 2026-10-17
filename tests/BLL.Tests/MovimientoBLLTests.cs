using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
            fixture.CategoriaRepo.Categorias.Add(1, new Categoria { Id = 1, Nombre = "Global", Usuario = null });
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
            fixture.CategoriaRepo.Categorias.Add(5, new Categoria { Id = 5, Nombre = "Ajena", Usuario = new Usuario { Id = 99 } });
            MovimientoBLL sut = fixture.CreateSut();

            Assert.ThrowsExactly<ExcepcionNegocio>(() => sut.Crear(5, TipoMovimiento.Ingreso, 100m, new DateTime(2026, 8, 1), null));
        }

        [TestMethod]
        public void Crear_Valido_InsertaComoPropioDelUsuarioActualAuditaYGuarda()
        {
            MovimientoFixture fixture = new MovimientoFixture(usuarioActualId: 7);
            fixture.CategoriaRepo.Categorias.Add(1, new Categoria { Id = 1, Nombre = "Global", Usuario = null });
            MovimientoBLL sut = fixture.CreateSut();

            int id = sut.Crear(1, TipoMovimiento.Egreso, 250.50m, new DateTime(2026, 8, 15), "Compra");

            Movimiento insertado = fixture.Unidad.MovimientoRepo.Movimientos[id];
            Assert.AreEqual(7, insertado.Usuario.Id);
            Assert.AreEqual(1, insertado.Categoria.Id);
            Assert.AreEqual(TipoMovimiento.Egreso, insertado.Tipo);
            Assert.AreEqual(250.50m, insertado.Monto);
            Assert.AreEqual("Compra", insertado.Descripcion);
            Assert.AreEqual("OperacionInterno", fixture.Bitacora.Calls.Single().Kind);
            Assert.IsTrue(fixture.Unidad.SaveCalled);
        }

        [TestMethod]
        public void Crear_ConComercioValido_InsertaConComercioAuditaYGuarda()
        {
            MovimientoFixture fixture = new MovimientoFixture(usuarioActualId: 7);
            fixture.CategoriaRepo.Categorias.Add(1, new Categoria { Id = 1, Nombre = "Global", Usuario = null });
            fixture.ComercioRepo.Comercios.Add(10, new Comercio { Id = 10, Nombre = "Coto" });
            MovimientoBLL sut = fixture.CreateSut();

            int id = sut.Crear(1, TipoMovimiento.Egreso, 500m, new DateTime(2026, 8, 15), "Super", comercioId: 10);

            Movimiento insertado = fixture.Unidad.MovimientoRepo.Movimientos[id];
            Assert.IsNotNull(insertado.Comercio);
            Assert.AreEqual(10, insertado.Comercio.Id);
            Assert.IsTrue(fixture.Unidad.SaveCalled);
        }

        [TestMethod]
        public void Crear_ConComercioInexistente_ThrowsExcepcionNegocio()
        {
            MovimientoFixture fixture = new MovimientoFixture(usuarioActualId: 7);
            fixture.CategoriaRepo.Categorias.Add(1, new Categoria { Id = 1, Nombre = "Global", Usuario = null });
            MovimientoBLL sut = fixture.CreateSut();

            Assert.ThrowsExactly<ExcepcionNegocio>(() => sut.Crear(1, TipoMovimiento.Egreso, 500m, new DateTime(2026, 8, 15), "Super", comercioId: 99));
        }

        [TestMethod]
        public void Actualizar_MovimientoDeOtroUsuario_ThrowsExcepcionNegocio()
        {
            MovimientoFixture fixture = new MovimientoFixture(usuarioActualId: 1);
            fixture.CategoriaRepo.Categorias.Add(1, new Categoria { Id = 1, Nombre = "Global", Usuario = null });
            fixture.Unidad.MovimientoRepo.Movimientos.Add(3, new Movimiento { Id = 3, Usuario = new Usuario { Id = 99 }, Categoria = new Categoria { Id = 1 }, Tipo = TipoMovimiento.Ingreso, Monto = 10m, Fecha = new DateTime(2026, 8, 1) });
            MovimientoBLL sut = fixture.CreateSut();

            Assert.ThrowsExactly<ExcepcionNegocio>(() => sut.Actualizar(3, 1, TipoMovimiento.Ingreso, 20m, new DateTime(2026, 8, 2), null));
        }

        [TestMethod]
        public void Actualizar_Valido_ActualizaAuditaYGuarda()
        {
            MovimientoFixture fixture = new MovimientoFixture(usuarioActualId: 5);
            fixture.CategoriaRepo.Categorias.Add(1, new Categoria { Id = 1, Nombre = "Global", Usuario = null });
            fixture.Unidad.MovimientoRepo.Movimientos.Add(3, new Movimiento { Id = 3, Usuario = new Usuario { Id = 5 }, Categoria = new Categoria { Id = 1 }, Tipo = TipoMovimiento.Ingreso, Monto = 10m, Fecha = new DateTime(2026, 8, 1) });
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
        public void Actualizar_ConComercioValido_ActualizaConComercioAuditaYGuarda()
        {
            MovimientoFixture fixture = new MovimientoFixture(usuarioActualId: 5);
            fixture.CategoriaRepo.Categorias.Add(1, new Categoria { Id = 1, Nombre = "Global", Usuario = null });
            fixture.ComercioRepo.Comercios.Add(20, new Comercio { Id = 20, Nombre = "Dia" });
            fixture.Unidad.MovimientoRepo.Movimientos.Add(3, new Movimiento { Id = 3, Usuario = new Usuario { Id = 5 }, Categoria = new Categoria { Id = 1 }, Tipo = TipoMovimiento.Ingreso, Monto = 10m, Fecha = new DateTime(2026, 8, 1) });
            MovimientoBLL sut = fixture.CreateSut();

            bool resultado = sut.Actualizar(3, 1, TipoMovimiento.Egreso, 999m, new DateTime(2026, 8, 20), "Editado", comercioId: 20);

            Assert.IsTrue(resultado);
            Movimiento actualizado = fixture.Unidad.MovimientoRepo.Movimientos[3];
            Assert.IsNotNull(actualizado.Comercio);
            Assert.AreEqual(20, actualizado.Comercio.Id);
            Assert.IsTrue(fixture.Unidad.SaveCalled);
        }

        [TestMethod]
        public void Actualizar_ConComercioInexistente_ThrowsExcepcionNegocio()
        {
            MovimientoFixture fixture = new MovimientoFixture(usuarioActualId: 5);
            fixture.CategoriaRepo.Categorias.Add(1, new Categoria { Id = 1, Nombre = "Global", Usuario = null });
            fixture.Unidad.MovimientoRepo.Movimientos.Add(3, new Movimiento { Id = 3, Usuario = new Usuario { Id = 5 }, Categoria = new Categoria { Id = 1 }, Tipo = TipoMovimiento.Ingreso, Monto = 10m, Fecha = new DateTime(2026, 8, 1) });
            MovimientoBLL sut = fixture.CreateSut();

            Assert.ThrowsExactly<ExcepcionNegocio>(() => sut.Actualizar(3, 1, TipoMovimiento.Egreso, 999m, new DateTime(2026, 8, 20), "Editado", comercioId: 99));
        }

        [TestMethod]
        public void Eliminar_MovimientoDeOtroUsuario_ThrowsExcepcionNegocio()
        {
            MovimientoFixture fixture = new MovimientoFixture(usuarioActualId: 1);
            fixture.Unidad.MovimientoRepo.Movimientos.Add(3, new Movimiento { Id = 3, Usuario = new Usuario { Id = 99 }, Categoria = new Categoria { Id = 1 }, Tipo = TipoMovimiento.Ingreso, Monto = 10m, Fecha = new DateTime(2026, 8, 1) });
            MovimientoBLL sut = fixture.CreateSut();

            Assert.ThrowsExactly<ExcepcionNegocio>(() => sut.Eliminar(3));
        }

        [TestMethod]
        public void Eliminar_Valido_EliminaAuditaYGuarda()
        {
            MovimientoFixture fixture = new MovimientoFixture(usuarioActualId: 5);
            fixture.Unidad.MovimientoRepo.Movimientos.Add(3, new Movimiento { Id = 3, Usuario = new Usuario { Id = 5 }, Categoria = new Categoria { Id = 1 }, Tipo = TipoMovimiento.Ingreso, Monto = 10m, Fecha = new DateTime(2026, 8, 1) });
            MovimientoBLL sut = fixture.CreateSut();

            bool resultado = sut.Eliminar(3);

            Assert.IsTrue(resultado);
            Assert.IsFalse(fixture.Unidad.MovimientoRepo.Movimientos.ContainsKey(3));
            Assert.AreEqual("OperacionInterno", fixture.Bitacora.Calls.Single().Kind);
            Assert.IsTrue(fixture.Unidad.SaveCalled);
        }

        [TestMethod]
        public async Task EscanearBorrador_ResultadoCompleto_ArmaMovimientoSinPersistir()
        {
            MovimientoFixture fixture = new MovimientoFixture(usuarioActualId: 7);
            fixture.EscaneoTicket.NextResultado = new TicketEscaneado
            {
                Monto = 1234.56m,
                Fecha = new DateTime(2026, 3, 15),
                Descripcion = "Supermercado Ejemplo",
                TextoCrudo = "Supermercado Ejemplo\nTOTAL $ 1.234,56"
            };
            MovimientoBLL sut = fixture.CreateSut();

            EscaneoBorradorResultado resultado = await sut.EscanearBorradorAsync(new byte[] { 1, 2, 3 });
            Movimiento borrador = resultado.Borrador;

            Assert.AreEqual(7, borrador.Usuario.Id);
            Assert.IsNull(borrador.Categoria);
            Assert.AreEqual(TipoMovimiento.Egreso, borrador.Tipo);
            Assert.AreEqual(1234.56m, borrador.Monto);
            Assert.AreEqual(new DateTime(2026, 3, 15), borrador.Fecha);
            Assert.AreEqual("Supermercado Ejemplo", borrador.Descripcion);
            Assert.AreEqual("Supermercado Ejemplo\nTOTAL $ 1.234,56", resultado.TextoCrudo);
            Assert.AreEqual(0, fixture.UnidadFactory.CrearCallCount);
        }

        [TestMethod]
        public async Task EscanearBorrador_SinMontoNiFecha_UsaValoresPorDefecto()
        {
            MovimientoFixture fixture = new MovimientoFixture(usuarioActualId: 3);
            fixture.EscaneoTicket.NextResultado = new TicketEscaneado
            {
                Monto = null,
                Fecha = null,
                Descripcion = null,
                TextoCrudo = "texto ilegible"
            };
            MovimientoBLL sut = fixture.CreateSut();

            EscaneoBorradorResultado resultado = await sut.EscanearBorradorAsync(new byte[] { 1 });

            Assert.AreEqual(0m, resultado.Borrador.Monto);
            Assert.AreEqual(DateTime.Today, resultado.Borrador.Fecha);
            Assert.AreEqual("texto ilegible", resultado.TextoCrudo);
        }

        [TestMethod]
        public async Task EscanearBorrador_ServicioLanzaExcepcionTecnica_PropagaSinEnvolver()
        {
            MovimientoFixture fixture = new MovimientoFixture(usuarioActualId: 1);
            fixture.EscaneoTicket.ThrowException = new ExcepcionTecnica("Error.Ticket.ApiKeyMissing");
            MovimientoBLL sut = fixture.CreateSut();

            ExcepcionTecnica excepcion = await Assert.ThrowsExactlyAsync<ExcepcionTecnica>(() => sut.EscanearBorradorAsync(new byte[] { 1 }));
            Assert.AreEqual("Error.Ticket.ApiKeyMissing", excepcion.Message);
        }

        [TestMethod]
        public async Task EscanearBorrador_FalloInesperado_SeEnvuelveEnExcepcionTecnica()
        {
            MovimientoFixture fixture = new MovimientoFixture(usuarioActualId: 1);
            fixture.EscaneoTicket.ThrowException = new InvalidOperationException("boom");
            MovimientoBLL sut = fixture.CreateSut();

            await Assert.ThrowsExactlyAsync<ExcepcionTecnica>(() => sut.EscanearBorradorAsync(new byte[] { 1 }));
        }

        [TestMethod]
        public void ObtenerPorUsuario_DelegaAlRepositorio()
        {
            MovimientoFixture fixture = new MovimientoFixture(usuarioActualId: 5);
            fixture.MovimientoRepo.Movimientos.Add(1, new Movimiento { Id = 1, Usuario = new Usuario { Id = 5 }, Categoria = new Categoria { Id = 1 }, Tipo = TipoMovimiento.Ingreso, Monto = 10m, Fecha = new DateTime(2026, 8, 1) });
            MovimientoBLL sut = fixture.CreateSut();

            System.Collections.Generic.List<Movimiento> resultado = sut.ObtenerPorUsuario(5, null);

            Assert.AreEqual(1, resultado.Count);
        }

        [TestMethod]
        public void ObtenerTotalesPorCategoria_ResuelveUsuarioDeSesionYDelegaAlRepositorio()
        {
            MovimientoFixture fixture = new MovimientoFixture(usuarioActualId: 5);
            List<TotalPorCategoria> esperado = new List<TotalPorCategoria> { new TotalPorCategoria { CategoriaNombre = "Comida", Total = 100m } };
            fixture.MovimientoRepo.ObtenerTotalesPorCategoriaResult = esperado;
            MovimientoBLL sut = fixture.CreateSut();

            List<TotalPorCategoria> resultado = sut.ObtenerTotalesPorCategoria(TipoMovimiento.Egreso, new DateTime(2026, 8, 1), new DateTime(2026, 8, 31));

            Assert.AreSame(esperado, resultado);
            Assert.AreEqual(5, fixture.MovimientoRepo.LastObtenerTotalesPorCategoriaArgs.Item1);
            Assert.AreEqual(TipoMovimiento.Egreso, fixture.MovimientoRepo.LastObtenerTotalesPorCategoriaArgs.Item2);
            Assert.AreEqual(new DateTime(2026, 8, 1), fixture.MovimientoRepo.LastObtenerTotalesPorCategoriaArgs.Item3);
            Assert.AreEqual(new DateTime(2026, 8, 31), fixture.MovimientoRepo.LastObtenerTotalesPorCategoriaArgs.Item4);
        }

        [TestMethod]
        public void ObtenerTotalesPorMes_ResuelveUsuarioDeSesionYDelegaAlRepositorio()
        {
            MovimientoFixture fixture = new MovimientoFixture(usuarioActualId: 9);
            List<TotalPorMes> esperado = new List<TotalPorMes> { new TotalPorMes { Anio = 2026, Mes = 8, TotalIngresos = 1000m, TotalEgresos = 400m } };
            fixture.MovimientoRepo.ObtenerTotalesPorMesResult = esperado;
            MovimientoBLL sut = fixture.CreateSut();

            List<TotalPorMes> resultado = sut.ObtenerTotalesPorMes(new DateTime(2026, 8, 1), new DateTime(2026, 9, 30));

            Assert.AreSame(esperado, resultado);
            Assert.AreEqual(9, fixture.MovimientoRepo.LastObtenerTotalesPorMesArgs.Item1);
            Assert.AreEqual(new DateTime(2026, 8, 1), fixture.MovimientoRepo.LastObtenerTotalesPorMesArgs.Item2);
            Assert.AreEqual(new DateTime(2026, 9, 30), fixture.MovimientoRepo.LastObtenerTotalesPorMesArgs.Item3);
        }

        [TestMethod]
        public void ObtenerResumenBalance_ResuelveUsuarioDeSesionYDelegaAlRepositorio()
        {
            MovimientoFixture fixture = new MovimientoFixture(usuarioActualId: 11);
            ResumenMovimientos esperado = new ResumenMovimientos { TotalIngresos = 500m, TotalEgresos = 200m };
            fixture.MovimientoRepo.ObtenerResumenResult = esperado;
            MovimientoBLL sut = fixture.CreateSut();

            ResumenMovimientos resultado = sut.ObtenerResumenBalance(new DateTime(2026, 8, 1), new DateTime(2026, 8, 31));

            Assert.AreSame(esperado, resultado);
            Assert.AreEqual(11, fixture.MovimientoRepo.LastObtenerResumenArgs.Item1);
            Assert.AreEqual(new DateTime(2026, 8, 1), fixture.MovimientoRepo.LastObtenerResumenArgs.Item2);
            Assert.AreEqual(new DateTime(2026, 8, 31), fixture.MovimientoRepo.LastObtenerResumenArgs.Item3);
        }

        private sealed class MovimientoFixture
        {
            public readonly FakeMovimientoRepositorio MovimientoRepo = new FakeMovimientoRepositorio();
            public readonly FakeCategoriaRepositorio CategoriaRepo = new FakeCategoriaRepositorio();
            public readonly FakeComercioRepositorio ComercioRepo = new FakeComercioRepositorio();
            public readonly SpyBitacoraBLL Bitacora = new SpyBitacoraBLL();
            public readonly FakeSesion Sesion = new FakeSesion();
            public readonly FakeEscaneoTicketService EscaneoTicket = new FakeEscaneoTicketService();
            public readonly FakeUnidadDeTrabajo Unidad;
            public readonly FakeUnidadDeTrabajoFactory UnidadFactory;

            public MovimientoFixture(int usuarioActualId)
            {
                Unidad = new FakeUnidadDeTrabajo { MovimientoRepo = MovimientoRepo, CategoriaRepo = CategoriaRepo, ComercioRepo = ComercioRepo };
                UnidadFactory = new FakeUnidadDeTrabajoFactory(Unidad);
                Sesion.IniciarSesion(new Usuario { Id = usuarioActualId, Username = "test" });
            }

            public MovimientoBLL CreateSut()
            {
                return new MovimientoBLL(MovimientoRepo, CategoriaRepo, ComercioRepo, UnidadFactory, Sesion, Bitacora, EscaneoTicket);
            }
        }
    }
}
