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

            Assert.ThrowsExactly<ExcepcionNegocio>(() => sut.Crear(Nuevo(1, TipoMovimiento.Ingreso, 0m, new DateTime(2026, 8, 1), "x")));
            Assert.AreEqual(0, fixture.UnidadFactory.CrearCallCount);
        }

        [TestMethod]
        public void Crear_CategoriaInexistente_ThrowsExcepcionNegocio()
        {
            MovimientoFixture fixture = new MovimientoFixture(usuarioActualId: 1);
            MovimientoBLL sut = fixture.CreateSut();

            Assert.ThrowsExactly<ExcepcionNegocio>(() => sut.Crear(Nuevo(99, TipoMovimiento.Ingreso, 100m, new DateTime(2026, 8, 1), null)));
        }

        [TestMethod]
        public void Crear_CategoriaDeOtroUsuario_ThrowsExcepcionNegocio()
        {
            MovimientoFixture fixture = new MovimientoFixture(usuarioActualId: 1);
            fixture.CategoriaRepo.Categorias.Add(5, new Categoria { Id = 5, Nombre = "Ajena", Usuario = new Usuario { Id = 99 } });
            MovimientoBLL sut = fixture.CreateSut();

            Assert.ThrowsExactly<ExcepcionNegocio>(() => sut.Crear(Nuevo(5, TipoMovimiento.Ingreso, 100m, new DateTime(2026, 8, 1), null)));
        }

        [TestMethod]
        public void Crear_Valido_InsertaComoPropioDelUsuarioActualAuditaYGuarda()
        {
            MovimientoFixture fixture = new MovimientoFixture(usuarioActualId: 7);
            fixture.CategoriaRepo.Categorias.Add(1, new Categoria { Id = 1, Nombre = "Global", Usuario = null });
            MovimientoBLL sut = fixture.CreateSut();

            Movimiento creado = sut.Crear(Nuevo(1, TipoMovimiento.Egreso, 250.50m, new DateTime(2026, 8, 15), "Compra"));

            Movimiento insertado = fixture.Unidad.MovimientoRepo.Movimientos[creado.Id];
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

            Movimiento creado = sut.Crear(Nuevo(1, TipoMovimiento.Egreso, 500m, new DateTime(2026, 8, 15), "Super", comercioId: 10));

            Movimiento insertado = fixture.Unidad.MovimientoRepo.Movimientos[creado.Id];
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

            Assert.ThrowsExactly<ExcepcionNegocio>(() => sut.Crear(Nuevo(1, TipoMovimiento.Egreso, 500m, new DateTime(2026, 8, 15), "Super", comercioId: 99)));
        }

        [TestMethod]
        public void Actualizar_MovimientoDeOtroUsuario_ThrowsExcepcionNegocio()
        {
            MovimientoFixture fixture = new MovimientoFixture(usuarioActualId: 1);
            fixture.CategoriaRepo.Categorias.Add(1, new Categoria { Id = 1, Nombre = "Global", Usuario = null });
            fixture.Unidad.MovimientoRepo.Movimientos.Add(3, new Movimiento { Id = 3, Usuario = new Usuario { Id = 99 }, Categoria = new Categoria { Id = 1 }, Tipo = TipoMovimiento.Ingreso, Monto = 10m, Fecha = new DateTime(2026, 8, 1) });
            MovimientoBLL sut = fixture.CreateSut();

            Assert.ThrowsExactly<ExcepcionNegocio>(() => sut.Actualizar(Editado(3, 1, TipoMovimiento.Ingreso, 20m, new DateTime(2026, 8, 2), null)));
        }

        [TestMethod]
        public void Actualizar_Valido_ActualizaAuditaYGuarda()
        {
            MovimientoFixture fixture = new MovimientoFixture(usuarioActualId: 5);
            fixture.CategoriaRepo.Categorias.Add(1, new Categoria { Id = 1, Nombre = "Global", Usuario = null });
            fixture.Unidad.MovimientoRepo.Movimientos.Add(3, new Movimiento { Id = 3, Usuario = new Usuario { Id = 5 }, Categoria = new Categoria { Id = 1 }, Tipo = TipoMovimiento.Ingreso, Monto = 10m, Fecha = new DateTime(2026, 8, 1) });
            MovimientoBLL sut = fixture.CreateSut();

            bool resultado = sut.Actualizar(Editado(3, 1, TipoMovimiento.Egreso, 999m, new DateTime(2026, 8, 20), "Editado"));

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

            bool resultado = sut.Actualizar(Editado(3, 1, TipoMovimiento.Egreso, 999m, new DateTime(2026, 8, 20), "Editado", comercioId: 20));

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

            Assert.ThrowsExactly<ExcepcionNegocio>(() => sut.Actualizar(Editado(3, 1, TipoMovimiento.Egreso, 999m, new DateTime(2026, 8, 20), "Editado", comercioId: 99)));
        }

        [TestMethod]
        public void Eliminar_MovimientoDeOtroUsuario_ThrowsExcepcionNegocio()
        {
            MovimientoFixture fixture = new MovimientoFixture(usuarioActualId: 1);
            fixture.Unidad.MovimientoRepo.Movimientos.Add(3, new Movimiento { Id = 3, Usuario = new Usuario { Id = 99 }, Categoria = new Categoria { Id = 1 }, Tipo = TipoMovimiento.Ingreso, Monto = 10m, Fecha = new DateTime(2026, 8, 1) });
            MovimientoBLL sut = fixture.CreateSut();

            Assert.ThrowsExactly<ExcepcionNegocio>(() => sut.Eliminar(new Movimiento { Id = 3 }));
        }

        [TestMethod]
        public void Eliminar_Valido_EliminaAuditaYGuarda()
        {
            MovimientoFixture fixture = new MovimientoFixture(usuarioActualId: 5);
            fixture.Unidad.MovimientoRepo.Movimientos.Add(3, new Movimiento { Id = 3, Usuario = new Usuario { Id = 5 }, Categoria = new Categoria { Id = 1 }, Tipo = TipoMovimiento.Ingreso, Monto = 10m, Fecha = new DateTime(2026, 8, 1) });
            MovimientoBLL sut = fixture.CreateSut();

            bool resultado = sut.Eliminar(new Movimiento { Id = 3 });

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
        public void ObtenerParaUsuarioActual_DelegaAlRepositorioConElUsuarioDeSesion()
        {
            MovimientoFixture fixture = new MovimientoFixture(usuarioActualId: 5);
            fixture.MovimientoRepo.Movimientos.Add(1, new Movimiento { Id = 1, Usuario = new Usuario { Id = 5 }, Categoria = new Categoria { Id = 1 }, Tipo = TipoMovimiento.Ingreso, Monto = 10m, Fecha = new DateTime(2026, 8, 1) });
            MovimientoBLL sut = fixture.CreateSut();

            System.Collections.Generic.List<Movimiento> resultado = sut.ObtenerParaUsuarioActual(null);

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
            Assert.AreEqual(5, fixture.MovimientoRepo.LastObtenerTotalesPorCategoriaArgs.Item1.Id);
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
            Assert.AreEqual(9, fixture.MovimientoRepo.LastObtenerTotalesPorMesArgs.Item1.Id);
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
            Assert.AreEqual(11, fixture.MovimientoRepo.LastObtenerResumenArgs.Item1.Id);
            Assert.AreEqual(new DateTime(2026, 8, 1), fixture.MovimientoRepo.LastObtenerResumenArgs.Item2);
            Assert.AreEqual(new DateTime(2026, 8, 31), fixture.MovimientoRepo.LastObtenerResumenArgs.Item3);
        }

        [TestMethod]
        public void ObtenerGastoPorComercio_ResuelveUsuarioYPideEgresosAlRepositorio()
        {
            MovimientoFixture fixture = new MovimientoFixture(usuarioActualId: 6);
            MovimientoBLL sut = fixture.CreateSut();

            sut.ObtenerGastoPorComercio(new DateTime(2026, 10, 1), new DateTime(2026, 10, 31), 8);

            Assert.AreEqual(6, fixture.MovimientoRepo.LastObtenerTotalesPorComercioArgs.Item1.Id);
            Assert.AreEqual(TipoMovimiento.Egreso, fixture.MovimientoRepo.LastObtenerTotalesPorComercioArgs.Item2);
            Assert.AreEqual(new DateTime(2026, 10, 1), fixture.MovimientoRepo.LastObtenerTotalesPorComercioArgs.Item3);
            Assert.AreEqual(new DateTime(2026, 10, 31), fixture.MovimientoRepo.LastObtenerTotalesPorComercioArgs.Item4);
        }

        [TestMethod]
        public void ObtenerGastoPorComercio_MasComerciosQueElTope_DevuelveSoloLosMayoresOrdenados()
        {
            MovimientoFixture fixture = new MovimientoFixture(usuarioActualId: 1);
            for (int i = 1; i <= 10; i++)
            {
                fixture.MovimientoRepo.ObtenerTotalesPorComercioResult.Add(new TotalPorComercio { ComercioNombre = "Comercio" + i, Total = i * 100m });
            }

            GastoPorComercio resultado = fixture.CreateSut().ObtenerGastoPorComercio(new DateTime(2026, 10, 1), new DateTime(2026, 10, 31), 8);

            Assert.AreEqual(8, resultado.Comercios.Count);
            Assert.AreEqual("Comercio10", resultado.Comercios[0].ComercioNombre);
            Assert.AreEqual("Comercio3", resultado.Comercios[7].ComercioNombre);
            CollectionAssert.AreEqual(resultado.Comercios.OrderByDescending(item => item.Total).ToList(), resultado.Comercios);
        }

        [TestMethod]
        public void ObtenerGastoPorComercio_EgresosSinComercio_SeInformanAparteYNoSonBarra()
        {
            MovimientoFixture fixture = new MovimientoFixture(usuarioActualId: 1);
            fixture.MovimientoRepo.ObtenerTotalesPorComercioResult.Add(new TotalPorComercio { ComercioNombre = "Coto", Total = 300m });
            fixture.MovimientoRepo.ObtenerTotalesPorComercioResult.Add(new TotalPorComercio { ComercioNombre = null, Total = 75m });

            GastoPorComercio resultado = fixture.CreateSut().ObtenerGastoPorComercio(new DateTime(2026, 10, 1), new DateTime(2026, 10, 31), 8);

            Assert.AreEqual(1, resultado.Comercios.Count);
            Assert.AreEqual("Coto", resultado.Comercios[0].ComercioNombre);
            Assert.AreEqual(75m, resultado.TotalSinComercio);
        }

        [TestMethod]
        public void ObtenerGastoPorComercio_SinDatos_DevuelveListaVaciaYSinComercioCero()
        {
            MovimientoFixture fixture = new MovimientoFixture(usuarioActualId: 1);

            GastoPorComercio resultado = fixture.CreateSut().ObtenerGastoPorComercio(new DateTime(2026, 10, 1), new DateTime(2026, 10, 31), 8);

            Assert.AreEqual(0, resultado.Comercios.Count);
            Assert.AreEqual(0m, resultado.TotalSinComercio);
        }

        [TestMethod]
        public void ObtenerGastoAcumuladoMensual_PideEgresosDesdeElMesAnteriorHastaHoy()
        {
            MovimientoFixture fixture = new MovimientoFixture(usuarioActualId: 3);

            fixture.CreateSut().ObtenerGastoAcumuladoMensual(new DateTime(2026, 10, 10));

            Assert.AreEqual(3, fixture.MovimientoRepo.LastObtenerTotalesPorDiaArgs.Item1.Id);
            Assert.AreEqual(TipoMovimiento.Egreso, fixture.MovimientoRepo.LastObtenerTotalesPorDiaArgs.Item2);
            Assert.AreEqual(new DateTime(2026, 9, 1), fixture.MovimientoRepo.LastObtenerTotalesPorDiaArgs.Item3);
            Assert.AreEqual(new DateTime(2026, 10, 10), fixture.MovimientoRepo.LastObtenerTotalesPorDiaArgs.Item4);
        }

        [TestMethod]
        public void ObtenerGastoAcumuladoMensual_DiasSinEgresos_RepitenElAcumuladoDelDiaAnterior()
        {
            MovimientoFixture fixture = new MovimientoFixture(usuarioActualId: 1);
            fixture.MovimientoRepo.ObtenerTotalesPorDiaResult.Add(new TotalPorDia { Fecha = new DateTime(2026, 10, 1), Total = 100m });
            fixture.MovimientoRepo.ObtenerTotalesPorDiaResult.Add(new TotalPorDia { Fecha = new DateTime(2026, 10, 3), Total = 50m });

            GastoAcumuladoMensual resultado = fixture.CreateSut().ObtenerGastoAcumuladoMensual(new DateTime(2026, 10, 5));

            CollectionAssert.AreEqual(new[] { 100m, 100m, 150m, 150m, 150m }, resultado.MesActual);
            Assert.AreEqual(150m, resultado.AcumuladoActual);
        }

        [TestMethod]
        public void ObtenerGastoAcumuladoMensual_MesAnteriorCompleto_AcumulaTodosSusDias()
        {
            MovimientoFixture fixture = new MovimientoFixture(usuarioActualId: 1);
            fixture.MovimientoRepo.ObtenerTotalesPorDiaResult.Add(new TotalPorDia { Fecha = new DateTime(2026, 9, 1), Total = 200m });
            fixture.MovimientoRepo.ObtenerTotalesPorDiaResult.Add(new TotalPorDia { Fecha = new DateTime(2026, 9, 30), Total = 100m });

            GastoAcumuladoMensual resultado = fixture.CreateSut().ObtenerGastoAcumuladoMensual(new DateTime(2026, 10, 10));

            Assert.AreEqual(30, resultado.MesAnterior.Count);
            Assert.AreEqual(200m, resultado.MesAnterior[0]);
            Assert.AreEqual(200m, resultado.MesAnterior[28]);
            Assert.AreEqual(300m, resultado.MesAnterior[29]);
            Assert.AreEqual(200m, resultado.AcumuladoAnteriorAlMismoDia);
        }

        [TestMethod]
        public void ObtenerGastoAcumuladoMensual_ConGastoPrevio_CalculaVariacionPorcentual()
        {
            MovimientoFixture fixture = new MovimientoFixture(usuarioActualId: 1);
            fixture.MovimientoRepo.ObtenerTotalesPorDiaResult.Add(new TotalPorDia { Fecha = new DateTime(2026, 9, 2), Total = 100m });
            fixture.MovimientoRepo.ObtenerTotalesPorDiaResult.Add(new TotalPorDia { Fecha = new DateTime(2026, 10, 4), Total = 150m });

            GastoAcumuladoMensual resultado = fixture.CreateSut().ObtenerGastoAcumuladoMensual(new DateTime(2026, 10, 10));

            Assert.AreEqual(150m, resultado.AcumuladoActual);
            Assert.AreEqual(100m, resultado.AcumuladoAnteriorAlMismoDia);
            Assert.AreEqual(50m, resultado.VariacionPorcentual);
        }

        [TestMethod]
        public void ObtenerGastoAcumuladoMensual_MesAnteriorSinEgresosHastaElMismoDia_NoTieneVariacion()
        {
            MovimientoFixture fixture = new MovimientoFixture(usuarioActualId: 1);
            fixture.MovimientoRepo.ObtenerTotalesPorDiaResult.Add(new TotalPorDia { Fecha = new DateTime(2026, 9, 20), Total = 500m });
            fixture.MovimientoRepo.ObtenerTotalesPorDiaResult.Add(new TotalPorDia { Fecha = new DateTime(2026, 10, 4), Total = 150m });

            GastoAcumuladoMensual resultado = fixture.CreateSut().ObtenerGastoAcumuladoMensual(new DateTime(2026, 10, 10));

            Assert.AreEqual(0m, resultado.AcumuladoAnteriorAlMismoDia);
            Assert.IsNull(resultado.VariacionPorcentual);
        }

        [TestMethod]
        public void ObtenerGastoAcumuladoMensual_MesAnteriorMasCorto_ComparaConSuUltimoDia()
        {
            MovimientoFixture fixture = new MovimientoFixture(usuarioActualId: 1);
            fixture.MovimientoRepo.ObtenerTotalesPorDiaResult.Add(new TotalPorDia { Fecha = new DateTime(2026, 2, 10), Total = 400m });
            fixture.MovimientoRepo.ObtenerTotalesPorDiaResult.Add(new TotalPorDia { Fecha = new DateTime(2026, 3, 31), Total = 600m });

            GastoAcumuladoMensual resultado = fixture.CreateSut().ObtenerGastoAcumuladoMensual(new DateTime(2026, 3, 31));

            Assert.AreEqual(28, resultado.MesAnterior.Count);
            Assert.AreEqual(31, resultado.MesActual.Count);
            Assert.AreEqual(400m, resultado.AcumuladoAnteriorAlMismoDia);
            Assert.AreEqual(50m, resultado.VariacionPorcentual);
        }

        [TestMethod]
        public void ObtenerGastoAcumuladoMensual_SinEgresos_DevuelveAcumuladosEnCeroSinVariacion()
        {
            MovimientoFixture fixture = new MovimientoFixture(usuarioActualId: 1);

            GastoAcumuladoMensual resultado = fixture.CreateSut().ObtenerGastoAcumuladoMensual(new DateTime(2026, 10, 10));

            Assert.AreEqual(0m, resultado.AcumuladoActual);
            Assert.AreEqual(0m, resultado.AcumuladoAnteriorAlMismoDia);
            Assert.IsNull(resultado.VariacionPorcentual);
        }

        [TestMethod]
        public void ExportarPdf_SeleccionVacia_RechazaSinGenerar()
        {
            var fixture = new MovimientoFixture(7);
            var ex = Assert.ThrowsExactly<ExcepcionNegocio>(() => fixture.CreateSut().ExportarPdf(
                CrearReporte(7, new string[0][])));
            Assert.AreEqual("Error.Movement.ExportEmpty", ex.Message);
            Assert.IsNull(fixture.Pdf.Recibido);
        }

        [TestMethod]
        public void ExportarPdf_CapturaDeOtroUsuario_RechazaSinGenerar()
        {
            var fixture = new MovimientoFixture(7);
            Assert.ThrowsExactly<ExcepcionNegocio>(() => fixture.CreateSut().ExportarPdf(
                CrearReporte(99, new[] { new[] { "Fecha", "Tipo", "Categoria", "Comercio", "Monto", "Descripcion" } })));
            Assert.IsNull(fixture.Pdf.Recibido);
        }

        [TestMethod]
        public void ExportarPdf_FilaConColumnasIncompletas_RechazaSinGenerar()
        {
            var fixture = new MovimientoFixture(7);
            Assert.ThrowsExactly<ExcepcionNegocio>(() => fixture.CreateSut().ExportarPdf(
                CrearReporte(7, new[] { new[] { "solo una columna" } })));
            Assert.IsNull(fixture.Pdf.Recibido);
        }

        [TestMethod]
        public void ExportarPdf_CapturaValida_ConservaSubconjuntoOrdenYCriteriosSinReconsultar()
        {
            var fixture = new MovimientoFixture(7);
            var filas = new[] {
                new[] { "14/10/2026", "Egreso", "Comida", "Café", "$ 20,00", "Segundo" },
                new[] { "13/10/2026", "Egreso", "Servicios", "", "$ 10,00", "Primero" }
            };
            var reporte = CrearReporte(7, filas);
            byte[] resultado = fixture.CreateSut().ExportarPdf(reporte);
            Assert.AreSame(fixture.Pdf.Resultado, resultado);
            Assert.AreEqual(2, fixture.Pdf.Recibido.Filas.Count);
            Assert.AreEqual("Segundo", fixture.Pdf.Recibido.Filas[0][5]);
            Assert.AreEqual("Primero", fixture.Pdf.Recibido.Filas[1][5]);
            Assert.AreEqual("Descripcion: compra", fixture.Pdf.Recibido.Filtros[0]);
            Assert.AreEqual(0, fixture.UnidadFactory.CrearCallCount);
            // La solicitud es una captura, no conserva referencias mutables del formulario.
            filas[0][5] = "Mutado";
            Assert.AreEqual("Segundo", reporte.Filas[0][5]);
        }

        [TestMethod]
        public void Crear_Valido_DevuelveLaMismaInstanciaConIdYUsuarioDeSesion()
        {
            MovimientoFixture fixture = new MovimientoFixture(usuarioActualId: 7);
            fixture.CategoriaRepo.Categorias.Add(1, new Categoria { Id = 1, Nombre = "Global", Usuario = null });
            MovimientoBLL sut = fixture.CreateSut();
            Movimiento movimiento = Nuevo(1, TipoMovimiento.Egreso, 10m, new DateTime(2026, 8, 15), "Compra");

            Movimiento creado = sut.Crear(movimiento);

            Assert.AreSame(movimiento, creado);
            Assert.IsTrue(creado.Id > 0);
            Assert.AreSame(fixture.UsuarioActual, creado.Usuario);
        }

        [TestMethod]
        public void Crear_MovimientoNulo_ThrowsExcepcionNegocioAmountMustBePositive()
        {
            MovimientoFixture fixture = new MovimientoFixture(usuarioActualId: 1);
            MovimientoBLL sut = fixture.CreateSut();

            ExcepcionNegocio ex = Assert.ThrowsExactly<ExcepcionNegocio>(() => sut.Crear(null));
            Assert.AreEqual("Error.Movement.AmountMustBePositive", ex.Message);
            Assert.AreEqual(0, fixture.UnidadFactory.CrearCallCount);
        }

        [TestMethod]
        public void Actualizar_UsuarioAdulteradoEnLaEntidadRecibida_ValidaPropiedadContraLaPersistida()
        {
            MovimientoFixture fixture = new MovimientoFixture(usuarioActualId: 1);
            fixture.CategoriaRepo.Categorias.Add(1, new Categoria { Id = 1, Nombre = "Global", Usuario = null });
            fixture.Unidad.MovimientoRepo.Movimientos.Add(3, new Movimiento { Id = 3, Usuario = new Usuario { Id = 99 }, Categoria = new Categoria { Id = 1 }, Tipo = TipoMovimiento.Ingreso, Monto = 10m, Fecha = new DateTime(2026, 8, 1) });
            MovimientoBLL sut = fixture.CreateSut();
            Movimiento adulterado = Editado(3, 1, TipoMovimiento.Egreso, 20m, new DateTime(2026, 8, 2), null);
            adulterado.Usuario = fixture.UsuarioActual;

            ExcepcionNegocio ex = Assert.ThrowsExactly<ExcepcionNegocio>(() => sut.Actualizar(adulterado));

            Assert.AreEqual("Error.Movement.NotOwned", ex.Message);
            Assert.AreEqual(10m, fixture.Unidad.MovimientoRepo.Movimientos[3].Monto);
            Assert.IsNull(fixture.Unidad.MovimientoRepo.LastUpdated);
        }

        [TestMethod]
        public void Actualizar_Valido_CopiaSoloLosCamposEditablesSobreElPersistido()
        {
            MovimientoFixture fixture = new MovimientoFixture(usuarioActualId: 5);
            fixture.CategoriaRepo.Categorias.Add(2, new Categoria { Id = 2, Nombre = "Global", Usuario = null });
            fixture.ComercioRepo.Comercios.Add(20, new Comercio { Id = 20, Nombre = "Dia" });
            DateTime fechaCreacion = new DateTime(2026, 7, 1);
            Movimiento persistido = new Movimiento { Id = 3, Usuario = new Usuario { Id = 5 }, Categoria = new Categoria { Id = 1 }, Tipo = TipoMovimiento.Ingreso, Monto = 10m, Fecha = new DateTime(2026, 8, 1), FechaCreacion = fechaCreacion };
            fixture.Unidad.MovimientoRepo.Movimientos.Add(3, persistido);
            MovimientoBLL sut = fixture.CreateSut();
            Movimiento recibido = Editado(3, 2, TipoMovimiento.Egreso, 999m, new DateTime(2026, 8, 20), "  Editado  ", comercioId: 20);
            recibido.Usuario = new Usuario { Id = 99 };
            recibido.FechaCreacion = new DateTime(2000, 1, 1);

            sut.Actualizar(recibido);

            Assert.AreSame(persistido, fixture.Unidad.MovimientoRepo.LastUpdated);
            Assert.AreEqual(2, persistido.Categoria.Id);
            Assert.AreEqual(20, persistido.Comercio.Id);
            Assert.AreEqual(TipoMovimiento.Egreso, persistido.Tipo);
            Assert.AreEqual(999m, persistido.Monto);
            Assert.AreEqual(new DateTime(2026, 8, 20), persistido.Fecha);
            Assert.AreEqual("Editado", persistido.Descripcion);
            Assert.AreEqual(5, persistido.Usuario.Id);
            Assert.AreEqual(fechaCreacion, persistido.FechaCreacion);
        }

        [TestMethod]
        public void Eliminar_UsuarioAdulteradoEnLaEntidadRecibida_ValidaPropiedadContraLaPersistida()
        {
            MovimientoFixture fixture = new MovimientoFixture(usuarioActualId: 1);
            fixture.Unidad.MovimientoRepo.Movimientos.Add(3, new Movimiento { Id = 3, Usuario = new Usuario { Id = 99 }, Categoria = new Categoria { Id = 1 }, Tipo = TipoMovimiento.Ingreso, Monto = 10m, Fecha = new DateTime(2026, 8, 1) });
            MovimientoBLL sut = fixture.CreateSut();
            Movimiento adulterado = new Movimiento { Id = 3, Usuario = fixture.UsuarioActual };

            ExcepcionNegocio ex = Assert.ThrowsExactly<ExcepcionNegocio>(() => sut.Eliminar(adulterado));

            Assert.AreEqual("Error.Movement.NotOwned", ex.Message);
            Assert.IsTrue(fixture.Unidad.MovimientoRepo.Movimientos.ContainsKey(3));
            Assert.IsNull(fixture.Unidad.MovimientoRepo.LastDeleted);
        }

        private static Movimiento Nuevo(int categoriaId, TipoMovimiento tipo, decimal monto, DateTime fecha, string descripcion, int? comercioId = null)
        {
            return new Movimiento
            {
                Categoria = new Categoria { Id = categoriaId },
                Comercio = comercioId.HasValue ? new Comercio { Id = comercioId.Value } : null,
                Tipo = tipo,
                Monto = monto,
                Fecha = fecha,
                Descripcion = descripcion
            };
        }

        private static Movimiento Editado(int id, int categoriaId, TipoMovimiento tipo, decimal monto, DateTime fecha, string descripcion, int? comercioId = null)
        {
            Movimiento movimiento = Nuevo(categoriaId, tipo, monto, fecha, descripcion, comercioId);
            movimiento.Id = id;
            return movimiento;
        }

        private static ReporteMovimientosPdf CrearReporte(int usuarioId, string[][] filas)
        {
            return new ReporteMovimientosPdf(new Usuario { Id = usuarioId }, "Movimientos",
                new[] { "Fecha", "Tipo", "Categoria", "Comercio", "Monto", "Descripcion" },
                filas, new[] { "Descripcion: compra" }, null, new DateTime(2026, 10, 14),
                "Generado", "Movimientos", "Pagina {0} de {1}", "Continuacion");
        }

        private sealed class FakeReportePdf : IReporteTabularPdfService
        {
            public IReporteTabularPdf Recibido;
            public readonly byte[] Resultado = { 1, 2, 3 };
            public byte[] Generar(IReporteTabularPdf reporte) { Recibido = reporte; return Resultado; }
        }

        private sealed class MovimientoFixture
        {
            public readonly FakeMovimientoRepositorio MovimientoRepo = new FakeMovimientoRepositorio();
            public readonly FakeCategoriaRepositorio CategoriaRepo = new FakeCategoriaRepositorio();
            public readonly FakeComercioRepositorio ComercioRepo = new FakeComercioRepositorio();
            public readonly SpyBitacoraBLL Bitacora = new SpyBitacoraBLL();
            public readonly FakeSesion Sesion = new FakeSesion();
            public readonly FakeEscaneoTicketService EscaneoTicket = new FakeEscaneoTicketService();
            public readonly FakeReportePdf Pdf = new FakeReportePdf();
            public readonly FakeUnidadDeTrabajo Unidad;
            public readonly FakeUnidadDeTrabajoFactory UnidadFactory;
            public readonly Usuario UsuarioActual;

            public MovimientoFixture(int usuarioActualId)
            {
                Unidad = new FakeUnidadDeTrabajo { MovimientoRepo = MovimientoRepo, CategoriaRepo = CategoriaRepo, ComercioRepo = ComercioRepo };
                UnidadFactory = new FakeUnidadDeTrabajoFactory(Unidad);
                UsuarioActual = new Usuario { Id = usuarioActualId, Username = "test" };
                Sesion.IniciarSesion(UsuarioActual);
            }

            public MovimientoBLL CreateSut()
            {
                return new MovimientoBLL(MovimientoRepo, CategoriaRepo, ComercioRepo, UnidadFactory, Sesion, Bitacora, EscaneoTicket, Pdf);
            }
        }
    }
}
