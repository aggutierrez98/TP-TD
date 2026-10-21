using System;
using System.Collections.Generic;
using System.Linq;
using ABSTRACCIONES;
using BE;
using BLL.Tests.TestDoubles;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SERVICIOS;

namespace BLL.Tests
{
    [TestClass]
    public sealed class DashboardBLLTests
    {
        [TestMethod]
        public void ObtenerOCrearDashboardActual_SinDashboardPrevio_CreaUnoNuevoAuditaYGuarda()
        {
            DashboardFixture fixture = new DashboardFixture(usuarioActualId: 7);
            DashboardBLL sut = fixture.CreateSut();

            Dashboard resultado = sut.ObtenerOCrearDashboardActual();

            Assert.IsNotNull(resultado);
            Assert.AreEqual(7, resultado.Usuario.Id);
            Assert.AreSame(fixture.Sesion.ObtenerUsuarioActual(), resultado.Usuario);
            Assert.AreEqual(1, fixture.UnidadFactory.CrearCallCount);
            Assert.AreEqual("OperacionInterno", fixture.Bitacora.Calls.Single().Kind);
            Assert.IsTrue(fixture.Unidad.SaveCalled);
        }

        [TestMethod]
        public void ObtenerOCrearDashboardActual_ConDashboardPrevio_ReusaSinCrearUow()
        {
            DashboardFixture fixture = new DashboardFixture(usuarioActualId: 3);
            Dashboard existente = new Dashboard { Id = 1, Usuario = new Usuario { Id = 3 }, Tarjetas = new List<TarjetaDashboard>() };
            fixture.DashboardRepo.Dashboards.Add(1, existente);

            DashboardBLL sut = fixture.CreateSut();
            Dashboard resultado = sut.ObtenerOCrearDashboardActual();

            Assert.AreSame(existente, resultado);
            Assert.AreEqual(0, fixture.UnidadFactory.CrearCallCount);
        }

        [TestMethod]
        public void AgregarTarjeta_DashboardSinTarjetas_InsertaConOrden1AuditaYGuarda()
        {
            DashboardFixture fixture = new DashboardFixture(usuarioActualId: 4);
            Dashboard existente = new Dashboard { Id = 1, Usuario = new Usuario { Id = 4 }, Tarjetas = new List<TarjetaDashboard>() };
            fixture.DashboardRepo.Dashboards.Add(1, existente);

            DashboardBLL sut = fixture.CreateSut();
            TarjetaDashboard resultado = sut.AgregarTarjeta(TipoTarjetaDashboard.GastosPorCategoria);

            Assert.IsNotNull(resultado);
            Assert.AreEqual(TipoTarjetaDashboard.GastosPorCategoria, resultado.Tipo);
            Assert.AreEqual(1, resultado.Orden);
            Assert.AreEqual("OperacionInterno", fixture.Bitacora.Calls.Single().Kind);
            Assert.IsTrue(fixture.Unidad.SaveCalled);
        }

        [TestMethod]
        public void AgregarTarjeta_DashboardConTarjetas_InsertaConOrdenSiguiente()
        {
            DashboardFixture fixture = new DashboardFixture(usuarioActualId: 4);
            Dashboard existente = new Dashboard
            {
                Id = 1,
                Usuario = new Usuario { Id = 4 },
                Tarjetas = new List<TarjetaDashboard>()
            };
            TarjetaDashboard tarjetaPrevia = new TarjetaDashboard { Id = 10, Dashboard = existente, Tipo = TipoTarjetaDashboard.GastosPorCategoria, Orden = 1 };
            existente.Tarjetas.Add(tarjetaPrevia);
            fixture.DashboardRepo.Dashboards.Add(1, existente);

            DashboardBLL sut = fixture.CreateSut();
            TarjetaDashboard resultado = sut.AgregarTarjeta(TipoTarjetaDashboard.IngresosPorCategoria);

            Assert.AreEqual(2, resultado.Orden);
        }

        [TestMethod]
        public void AgregarTarjeta_TipoYaAgregado_ThrowsExcepcionNegocio()
        {
            DashboardFixture fixture = new DashboardFixture(usuarioActualId: 4);
            Dashboard existente = new Dashboard { Id = 1, Usuario = new Usuario { Id = 4 }, Tarjetas = new List<TarjetaDashboard>() };
            TarjetaDashboard tarjetaPrevia = new TarjetaDashboard { Id = 10, Dashboard = existente, Tipo = TipoTarjetaDashboard.GastosPorCategoria, Orden = 1 };
            existente.Tarjetas.Add(tarjetaPrevia);
            fixture.DashboardRepo.Dashboards.Add(1, existente);

            DashboardBLL sut = fixture.CreateSut();

            Assert.ThrowsExactly<ExcepcionNegocio>(() => sut.AgregarTarjeta(TipoTarjetaDashboard.GastosPorCategoria));
            Assert.AreEqual(0, fixture.UnidadFactory.CrearCallCount);
        }

        [TestMethod]
        public void QuitarTarjeta_Propia_EliminaAuditaYGuarda()
        {
            DashboardFixture fixture = new DashboardFixture(usuarioActualId: 4);
            Dashboard dashboardPropio = new Dashboard { Id = 1, Usuario = new Usuario { Id = 4 } };
            fixture.Unidad.TarjetaDashboardRepo.Tarjetas.Add(10, new TarjetaDashboard { Id = 10, Dashboard = dashboardPropio, Tipo = TipoTarjetaDashboard.GastosPorCategoria, Orden = 1 });

            DashboardBLL sut = fixture.CreateSut();
            bool resultado = sut.QuitarTarjeta(fixture.Unidad.TarjetaDashboardRepo.Tarjetas[10]);

            Assert.IsTrue(resultado);
            Assert.IsFalse(fixture.Unidad.TarjetaDashboardRepo.Tarjetas.ContainsKey(10));
            Assert.AreEqual("OperacionInterno", fixture.Bitacora.Calls.Single().Kind);
            Assert.IsTrue(fixture.Unidad.SaveCalled);
        }

        [TestMethod]
        public void QuitarTarjeta_Ajena_ThrowsExcepcionNegocioYNoElimina()
        {
            DashboardFixture fixture = new DashboardFixture(usuarioActualId: 4);
            Dashboard dashboardAjeno = new Dashboard { Id = 2, Usuario = new Usuario { Id = 99 } };
            fixture.Unidad.TarjetaDashboardRepo.Tarjetas.Add(10, new TarjetaDashboard { Id = 10, Dashboard = dashboardAjeno, Tipo = TipoTarjetaDashboard.GastosPorCategoria, Orden = 1 });

            DashboardBLL sut = fixture.CreateSut();

            TarjetaDashboard recibida = new TarjetaDashboard
            {
                Id = 10,
                Dashboard = new Dashboard { Id = 2, Usuario = new Usuario { Id = 4 } }
            };
            Assert.ThrowsExactly<ExcepcionNegocio>(() => sut.QuitarTarjeta(recibida));
            Assert.IsTrue(fixture.Unidad.TarjetaDashboardRepo.Tarjetas.ContainsKey(10));
            Assert.IsFalse(fixture.Unidad.SaveCalled);
        }

        [TestMethod]
        public void QuitarTarjeta_Inexistente_ThrowsExcepcionNegocio()
        {
            DashboardFixture fixture = new DashboardFixture(usuarioActualId: 4);
            DashboardBLL sut = fixture.CreateSut();

            Assert.ThrowsExactly<ExcepcionNegocio>(() => sut.QuitarTarjeta(new TarjetaDashboard { Id = 999 }));
        }

        [TestMethod]
        public void ObtenerTiposDisponibles_DashboardVacio_DevuelveLosSeisTipos()
        {
            DashboardFixture fixture = new DashboardFixture(usuarioActualId: 4);
            Dashboard existente = new Dashboard { Id = 1, Usuario = new Usuario { Id = 4 }, Tarjetas = new List<TarjetaDashboard>() };
            fixture.DashboardRepo.Dashboards.Add(1, existente);

            DashboardBLL sut = fixture.CreateSut();
            IReadOnlyList<TipoTarjetaDashboard> disponibles = sut.ObtenerTiposDisponibles();

            Assert.AreEqual(6, disponibles.Count);
        }

        [TestMethod]
        public void ObtenerTiposDisponibles_ExcluyeLosYaAgregados()
        {
            DashboardFixture fixture = new DashboardFixture(usuarioActualId: 4);
            Dashboard existente = new Dashboard { Id = 1, Usuario = new Usuario { Id = 4 }, Tarjetas = new List<TarjetaDashboard>() };
            existente.Tarjetas.Add(new TarjetaDashboard { Id = 10, Dashboard = existente, Tipo = TipoTarjetaDashboard.GastosPorCategoria, Orden = 1 });
            existente.Tarjetas.Add(new TarjetaDashboard { Id = 11, Dashboard = existente, Tipo = TipoTarjetaDashboard.EvolucionBalance, Orden = 2 });
            fixture.DashboardRepo.Dashboards.Add(1, existente);

            DashboardBLL sut = fixture.CreateSut();
            IReadOnlyList<TipoTarjetaDashboard> disponibles = sut.ObtenerTiposDisponibles();

            Assert.AreEqual(4, disponibles.Count);
            Assert.IsFalse(disponibles.Contains(TipoTarjetaDashboard.GastosPorCategoria));
            Assert.IsFalse(disponibles.Contains(TipoTarjetaDashboard.EvolucionBalance));
            Assert.IsTrue(disponibles.Contains(TipoTarjetaDashboard.IngresosPorCategoria));
            Assert.IsTrue(disponibles.Contains(TipoTarjetaDashboard.ComparativoMensual));
            Assert.IsTrue(disponibles.Contains(TipoTarjetaDashboard.GastosPorComercio));
            Assert.IsTrue(disponibles.Contains(TipoTarjetaDashboard.GastoAcumuladoMensual));
        }

        private sealed class DashboardFixture
        {
            public readonly FakeDashboardRepositorio DashboardRepo = new FakeDashboardRepositorio();
            public readonly FakeTarjetaDashboardRepositorio TarjetaDashboardRepo = new FakeTarjetaDashboardRepositorio();
            public readonly SpyBitacoraBLL Bitacora = new SpyBitacoraBLL();
            public readonly FakeSesion Sesion = new FakeSesion();
            public readonly FakeUnidadDeTrabajo Unidad;
            public readonly FakeUnidadDeTrabajoFactory UnidadFactory;

            public DashboardFixture(int usuarioActualId)
            {
                Unidad = new FakeUnidadDeTrabajo { DashboardRepo = DashboardRepo, TarjetaDashboardRepo = TarjetaDashboardRepo };
                UnidadFactory = new FakeUnidadDeTrabajoFactory(Unidad);
                Sesion.IniciarSesion(new Usuario { Id = usuarioActualId, Username = "test" });
            }

            public DashboardBLL CreateSut()
            {
                return new DashboardBLL(DashboardRepo, TarjetaDashboardRepo, UnidadFactory, Sesion, Bitacora);
            }
        }
    }
}
