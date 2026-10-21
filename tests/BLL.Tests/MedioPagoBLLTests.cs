using System.Collections.Generic;
using BE;
using BLL.Tests.TestDoubles;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SERVICIOS;

namespace BLL.Tests
{
    [TestClass]
    public sealed class MedioPagoBLLTests
    {
        [TestMethod]
        public void ObtenerTodos_DelegaAlRepositorio()
        {
            MedioPagoFixture fixture = new MedioPagoFixture();
            fixture.MedioPagoRepo.MediosPago.Add(1, new MedioPago { Id = 1, Tipo = TipoMedioPago.TarjetaCredito, Marca = "Visa" });
            fixture.MedioPagoRepo.MediosPago.Add(2, new MedioPago { Id = 2, Tipo = TipoMedioPago.CuentaDNI });
            MedioPagoBLL sut = fixture.CreateSut();

            List<MedioPago> todos = sut.ObtenerTodos();

            Assert.AreEqual(2, todos.Count);
        }

        [TestMethod]
        public void Crear_TipoInvalido_ThrowsExcepcionNegocioAndDoesNotCreateUow()
        {
            MedioPagoFixture fixture = new MedioPagoFixture();
            MedioPagoBLL sut = fixture.CreateSut();

            ExcepcionNegocio ex = Assert.ThrowsExactly<ExcepcionNegocio>(() => sut.Crear(new MedioPago { Tipo = (TipoMedioPago)99, Marca = "Visa" }));

            Assert.AreEqual("Error.PaymentMethodManagement.InvalidType", ex.Message);
            Assert.AreEqual(0, fixture.UnidadFactory.CrearCallCount);
        }

        [TestMethod]
        public void Crear_Valido_NormalizaMarcaInsertaAuditaYGuarda()
        {
            MedioPagoFixture fixture = new MedioPagoFixture();
            MedioPagoBLL sut = fixture.CreateSut();

            MedioPago creado = sut.Crear(new MedioPago { Tipo = TipoMedioPago.TarjetaCredito, Marca = "  Visa  " });

            MedioPago insertado = fixture.MedioPagoRepo.MediosPago[creado.Id];
            Assert.AreEqual(TipoMedioPago.TarjetaCredito, insertado.Tipo);
            Assert.AreEqual("Visa", insertado.Marca);
            Assert.AreEqual("OperacionInterno", fixture.Bitacora.Calls[0].Kind);
            Assert.AreEqual("FormPaymentMethodManagement.Value.Activity.Creation", fixture.Bitacora.Calls[0].Actividad);
            Assert.IsTrue(fixture.Unidad.SaveCalled);
        }

        [TestMethod]
        public void Crear_MarcaEnBlanco_PersisteMarcaNula()
        {
            MedioPagoFixture fixture = new MedioPagoFixture();
            MedioPagoBLL sut = fixture.CreateSut();

            MedioPago creado = sut.Crear(new MedioPago { Tipo = TipoMedioPago.CuentaDNI, Marca = "   " });

            Assert.IsNull(fixture.MedioPagoRepo.MediosPago[creado.Id].Marca);
        }

        [TestMethod]
        public void Crear_TipoYMarcaYaExistentes_ThrowsExcepcionNegocio()
        {
            MedioPagoFixture fixture = new MedioPagoFixture();
            fixture.MedioPagoRepo.MediosPago.Add(1, new MedioPago { Id = 1, Tipo = TipoMedioPago.TarjetaCredito, Marca = "Visa" });
            MedioPagoBLL sut = fixture.CreateSut();

            ExcepcionNegocio ex = Assert.ThrowsExactly<ExcepcionNegocio>(() => sut.Crear(new MedioPago { Tipo = TipoMedioPago.TarjetaCredito, Marca = "VISA" }));

            Assert.AreEqual("Error.PaymentMethodManagement.Duplicate", ex.Message);
            Assert.IsFalse(fixture.Unidad.SaveCalled);
        }

        [TestMethod]
        public void Actualizar_Inexistente_ThrowsExcepcionNegocio()
        {
            MedioPagoFixture fixture = new MedioPagoFixture();
            MedioPagoBLL sut = fixture.CreateSut();

            ExcepcionNegocio ex = Assert.ThrowsExactly<ExcepcionNegocio>(() => sut.Actualizar(new MedioPago { Id = 99, Tipo = TipoMedioPago.TarjetaCredito, Marca = "Visa" }));

            Assert.AreEqual("Error.PaymentMethodManagement.NotFound", ex.Message);
        }

        [TestMethod]
        public void Actualizar_Valido_ActualizaAuditaYGuarda()
        {
            MedioPagoFixture fixture = new MedioPagoFixture();
            fixture.MedioPagoRepo.MediosPago.Add(1, new MedioPago { Id = 1, Tipo = TipoMedioPago.TarjetaCredito, Marca = "Visa" });
            MedioPagoBLL sut = fixture.CreateSut();

            bool resultado = sut.Actualizar(new MedioPago { Id = 1, Tipo = TipoMedioPago.TarjetaDebito, Marca = " Mastercard " });

            Assert.IsTrue(resultado);
            Assert.AreEqual(TipoMedioPago.TarjetaDebito, fixture.MedioPagoRepo.MediosPago[1].Tipo);
            Assert.AreEqual("Mastercard", fixture.MedioPagoRepo.MediosPago[1].Marca);
            Assert.AreEqual("FormPaymentMethodManagement.Value.Activity.Update", fixture.Bitacora.Calls[0].Actividad);
            Assert.IsTrue(fixture.Unidad.SaveCalled);
        }

        [TestMethod]
        public void Actualizar_MismoTipoYMarcaQueSiMismo_NoSeConsideraDuplicado()
        {
            MedioPagoFixture fixture = new MedioPagoFixture();
            fixture.MedioPagoRepo.MediosPago.Add(1, new MedioPago { Id = 1, Tipo = TipoMedioPago.TarjetaCredito, Marca = "Visa" });
            MedioPagoBLL sut = fixture.CreateSut();

            Assert.IsTrue(sut.Actualizar(new MedioPago { Id = 1, Tipo = TipoMedioPago.TarjetaCredito, Marca = "Visa" }));
        }

        [TestMethod]
        public void Actualizar_TipoYMarcaDeOtroMedioPago_ThrowsExcepcionNegocio()
        {
            MedioPagoFixture fixture = new MedioPagoFixture();
            fixture.MedioPagoRepo.MediosPago.Add(1, new MedioPago { Id = 1, Tipo = TipoMedioPago.TarjetaCredito, Marca = "Visa" });
            fixture.MedioPagoRepo.MediosPago.Add(2, new MedioPago { Id = 2, Tipo = TipoMedioPago.TarjetaCredito, Marca = "Amex" });
            MedioPagoBLL sut = fixture.CreateSut();

            ExcepcionNegocio ex = Assert.ThrowsExactly<ExcepcionNegocio>(() => sut.Actualizar(new MedioPago { Id = 2, Tipo = TipoMedioPago.TarjetaCredito, Marca = "visa" }));

            Assert.AreEqual("Error.PaymentMethodManagement.Duplicate", ex.Message);
        }

        [TestMethod]
        public void Eliminar_Inexistente_ThrowsExcepcionNegocio()
        {
            MedioPagoFixture fixture = new MedioPagoFixture();
            MedioPagoBLL sut = fixture.CreateSut();

            ExcepcionNegocio ex = Assert.ThrowsExactly<ExcepcionNegocio>(() => sut.Eliminar(new MedioPago { Id = 99 }));

            Assert.AreEqual("Error.PaymentMethodManagement.NotFound", ex.Message);
        }

        [TestMethod]
        public void Eliminar_EnUsoPorAlgunaEntidad_ThrowsExcepcionNegocio()
        {
            MedioPagoFixture fixture = new MedioPagoFixture();
            fixture.MedioPagoRepo.MediosPago.Add(1, new MedioPago { Id = 1, Tipo = TipoMedioPago.TarjetaCredito, Marca = "Visa" });
            fixture.Unidad.MedioPagoEntidadRepo.MediosPagoEntidad.Add(10, new MedioPagoEntidad
            {
                Id = 10,
                EntidadFinanciera = new EntidadFinanciera { Id = 5 },
                MedioPago = new MedioPago { Id = 1 }
            });
            MedioPagoBLL sut = fixture.CreateSut();

            ExcepcionNegocio ex = Assert.ThrowsExactly<ExcepcionNegocio>(() => sut.Eliminar(new MedioPago { Id = 1 }));

            Assert.AreEqual("Error.PaymentMethodManagement.InUse", ex.Message);
            Assert.IsTrue(fixture.MedioPagoRepo.MediosPago.ContainsKey(1));
            Assert.IsNull(fixture.MedioPagoRepo.LastDeleted);
            Assert.IsFalse(fixture.Unidad.SaveCalled);
        }

        [TestMethod]
        public void Eliminar_FallaDelBorradoConViolacionForanea_InformaDeleteFailed()
        {
            MedioPagoFixture fixture = new MedioPagoFixture();
            fixture.MedioPagoRepo.MediosPago.Add(1, new MedioPago { Id = 1, Tipo = TipoMedioPago.TarjetaCredito, Marca = "Visa" });
            fixture.MedioPagoRepo.ExcepcionEnEliminar = FakeSqlExceptionFactory.CrearViolacionForanea();
            MedioPagoBLL sut = fixture.CreateSut();

            ExcepcionTecnica ex = Assert.ThrowsExactly<ExcepcionTecnica>(() => sut.Eliminar(new MedioPago { Id = 1 }));

            Assert.AreEqual("Error.PaymentMethodManagement.DeleteFailed", ex.Message);
            Assert.IsFalse(fixture.Unidad.SaveCalled);
        }

        [TestMethod]
        public void Eliminar_FallaTecnicaConViolacionForaneaEnvuelta_NoSeConvierteEnUso()
        {
            MedioPagoFixture fixture = new MedioPagoFixture();
            fixture.MedioPagoRepo.MediosPago.Add(1, new MedioPago { Id = 1, Tipo = TipoMedioPago.TarjetaCredito, Marca = "Visa" });
            fixture.MedioPagoRepo.ExcepcionEnEliminar = FakeSqlExceptionFactory.CrearViolacionForaneaEnvuelta();
            MedioPagoBLL sut = fixture.CreateSut();

            ExcepcionTecnica ex = Assert.ThrowsExactly<ExcepcionTecnica>(() => sut.Eliminar(new MedioPago { Id = 1 }));

            Assert.AreEqual("Error.Technical.SqlFailure", ex.Message);
            Assert.IsFalse(fixture.Unidad.SaveCalled);
        }

        [TestMethod]
        public void Eliminar_Valido_EliminaAuditaYGuarda()
        {
            MedioPagoFixture fixture = new MedioPagoFixture();
            fixture.MedioPagoRepo.MediosPago.Add(1, new MedioPago { Id = 1, Tipo = TipoMedioPago.TarjetaCredito, Marca = "Visa" });
            MedioPagoBLL sut = fixture.CreateSut();

            bool resultado = sut.Eliminar(new MedioPago { Id = 1 });

            Assert.IsTrue(resultado);
            Assert.IsFalse(fixture.MedioPagoRepo.MediosPago.ContainsKey(1));
            Assert.AreEqual("FormPaymentMethodManagement.Value.Activity.Deletion", fixture.Bitacora.Calls[0].Actividad);
            Assert.IsTrue(fixture.Unidad.SaveCalled);
        }

        [TestMethod]
        public void Eliminar_ReferenciadoPorUnaPromocion_ThrowsExcepcionNegocioYNoElimina()
        {
            MedioPagoFixture fixture = new MedioPagoFixture();
            fixture.MedioPagoRepo.MediosPago.Add(1, new MedioPago { Id = 1, Tipo = TipoMedioPago.TarjetaCredito, Marca = "Visa" });
            fixture.Unidad.PromocionMedioPagoRepo.MedioPagoIdsPorPromocion[7] = new System.Collections.Generic.List<int> { 1 };
            MedioPagoBLL sut = fixture.CreateSut();

            ExcepcionNegocio ex = Assert.ThrowsExactly<ExcepcionNegocio>(() => sut.Eliminar(new MedioPago { Id = 1 }));

            Assert.AreEqual("Error.PaymentMethodManagement.InUse", ex.Message);
            Assert.IsNull(fixture.MedioPagoRepo.LastDeleted);
            Assert.IsFalse(fixture.Unidad.SaveCalled);
        }

        [TestMethod]
        public void Crear_Valido_DevuelveLaMismaInstanciaConId()
        {
            MedioPagoFixture fixture = new MedioPagoFixture();
            MedioPagoBLL sut = fixture.CreateSut();
            MedioPago medioPago = new MedioPago { Tipo = TipoMedioPago.TarjetaCredito, Marca = "Visa" };

            MedioPago creado = sut.Crear(medioPago);

            Assert.AreSame(medioPago, creado);
            Assert.IsTrue(creado.Id > 0);
        }

        [TestMethod]
        public void Crear_MedioPagoNulo_ThrowsExcepcionNegocioInvalidType()
        {
            MedioPagoFixture fixture = new MedioPagoFixture();
            MedioPagoBLL sut = fixture.CreateSut();

            ExcepcionNegocio ex = Assert.ThrowsExactly<ExcepcionNegocio>(() => sut.Crear(null));

            Assert.AreEqual("Error.PaymentMethodManagement.InvalidType", ex.Message);
            Assert.AreEqual(0, fixture.UnidadFactory.CrearCallCount);
        }

        [TestMethod]
        public void Actualizar_Valido_CopiaTipoYMarcaSobreLaPersistida()
        {
            MedioPagoFixture fixture = new MedioPagoFixture();
            MedioPago persistido = new MedioPago { Id = 1, Tipo = TipoMedioPago.TarjetaCredito, Marca = "Visa" };
            fixture.MedioPagoRepo.MediosPago.Add(1, persistido);
            MedioPagoBLL sut = fixture.CreateSut();
            MedioPago recibido = new MedioPago { Id = 1, Tipo = TipoMedioPago.TarjetaDebito, Marca = "Mastercard" };

            sut.Actualizar(recibido);

            Assert.AreSame(persistido, fixture.MedioPagoRepo.LastUpdated);
            Assert.AreEqual(TipoMedioPago.TarjetaDebito, persistido.Tipo);
            Assert.AreEqual("Mastercard", persistido.Marca);
        }

        [TestMethod]
        public void Eliminar_MedioPagoNulo_ThrowsExcepcionNegocioNotFound()
        {
            MedioPagoFixture fixture = new MedioPagoFixture();
            MedioPagoBLL sut = fixture.CreateSut();

            ExcepcionNegocio ex = Assert.ThrowsExactly<ExcepcionNegocio>(() => sut.Eliminar(null));

            Assert.AreEqual("Error.PaymentMethodManagement.NotFound", ex.Message);
            Assert.IsNull(fixture.MedioPagoRepo.LastDeleted);
        }

        [TestMethod]
        public void Eliminar_Valido_EliminaLaPersistida()
        {
            MedioPagoFixture fixture = new MedioPagoFixture();
            MedioPago persistido = new MedioPago { Id = 1, Tipo = TipoMedioPago.TarjetaCredito, Marca = "Visa" };
            fixture.MedioPagoRepo.MediosPago.Add(1, persistido);
            MedioPagoBLL sut = fixture.CreateSut();

            sut.Eliminar(new MedioPago { Id = 1 });

            Assert.AreSame(persistido, fixture.MedioPagoRepo.LastDeleted);
        }

        private sealed class MedioPagoFixture
        {
            public readonly FakeMedioPagoRepositorio MedioPagoRepo = new FakeMedioPagoRepositorio();
            public readonly SpyBitacoraBLL Bitacora = new SpyBitacoraBLL();
            public readonly FakeUnidadDeTrabajo Unidad;
            public readonly FakeUnidadDeTrabajoFactory UnidadFactory;

            public MedioPagoFixture()
            {
                Unidad = new FakeUnidadDeTrabajo { MedioPagoRepo = MedioPagoRepo };
                UnidadFactory = new FakeUnidadDeTrabajoFactory(Unidad);
            }

            public MedioPagoBLL CreateSut()
            {
                return new MedioPagoBLL(MedioPagoRepo, UnidadFactory, Bitacora);
            }
        }
    }
}
