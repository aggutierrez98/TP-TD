using System.Collections.Generic;
using BE;
using BLL.Tests.TestDoubles;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SERVICIOS;

namespace BLL.Tests
{
    [TestClass]
    public sealed class EntidadFinancieraBLLTests
    {
        [TestMethod]
        public void ObtenerTodas_DelegaAlRepositorio()
        {
            EntidadFinancieraFixture fixture = new EntidadFinancieraFixture();
            fixture.EntidadRepo.Entidades.Add(1, new EntidadFinanciera { Id = 1, Nombre = "Galicia" });
            fixture.EntidadRepo.Entidades.Add(2, new EntidadFinanciera { Id = 2, Nombre = "Nacion" });
            EntidadFinancieraBLL sut = fixture.CreateSut();

            List<EntidadFinanciera> todas = sut.ObtenerTodas();

            Assert.AreEqual(2, todas.Count);
        }

        [TestMethod]
        public void ObtenerMediosPagoEntidad_DevuelveElCatalogoDeLaEntidadDelRepositorio()
        {
            EntidadFinancieraFixture fixture = new EntidadFinancieraFixture();
            List<MedioPagoEntidad> catalogo = new List<MedioPagoEntidad> { new MedioPagoEntidad { Id = 10 } };
            EntidadFinanciera galicia = new EntidadFinanciera { Id = 1, Nombre = "Galicia", MediosPagoEntidad = catalogo };
            fixture.EntidadRepo.Entidades.Add(1, galicia);
            EntidadFinancieraBLL sut = fixture.CreateSut();

            Assert.AreSame(catalogo, sut.ObtenerMediosPagoEntidad(new EntidadFinanciera { Id = 1 }));
        }

        [TestMethod]
        public void ObtenerMediosPagoEntidad_EntidadInexistente_DevuelveListaVacia()
        {
            EntidadFinancieraFixture fixture = new EntidadFinancieraFixture();
            EntidadFinancieraBLL sut = fixture.CreateSut();

            Assert.AreEqual(0, sut.ObtenerMediosPagoEntidad(new EntidadFinanciera { Id = 99 }).Count);
        }

        [TestMethod]
        public void Crear_NombreVacio_ThrowsExcepcionNegocioAndDoesNotCreateUow()
        {
            EntidadFinancieraFixture fixture = new EntidadFinancieraFixture();
            EntidadFinancieraBLL sut = fixture.CreateSut();

            ExcepcionNegocio ex = Assert.ThrowsExactly<ExcepcionNegocio>(() => sut.Crear(new EntidadFinanciera { Nombre = "   " }));

            Assert.AreEqual("Error.FinancialEntityManagement.NameRequired", ex.Message);
            Assert.AreEqual(0, fixture.UnidadFactory.CrearCallCount);
        }

        [TestMethod]
        public void Crear_Valido_NormalizaInsertaAuditaYGuarda()
        {
            EntidadFinancieraFixture fixture = new EntidadFinancieraFixture();
            EntidadFinancieraBLL sut = fixture.CreateSut();

            EntidadFinanciera creada = sut.Crear(new EntidadFinanciera { Nombre = "  Galicia  " });

            Assert.AreEqual("Galicia", fixture.EntidadRepo.Entidades[creada.Id].Nombre);
            Assert.AreEqual("FormFinancialEntityManagement.Value.Activity.Creation", fixture.Bitacora.Calls[0].Actividad);
            Assert.IsTrue(fixture.Unidad.SaveCalled);
        }

        [TestMethod]
        public void Crear_NombreYaExistente_ThrowsExcepcionNegocio()
        {
            EntidadFinancieraFixture fixture = new EntidadFinancieraFixture();
            fixture.EntidadRepo.Entidades.Add(1, new EntidadFinanciera { Id = 1, Nombre = "Galicia" });
            EntidadFinancieraBLL sut = fixture.CreateSut();

            ExcepcionNegocio ex = Assert.ThrowsExactly<ExcepcionNegocio>(() => sut.Crear(new EntidadFinanciera { Nombre = "GALICIA" }));

            Assert.AreEqual("Error.FinancialEntityManagement.DuplicateName", ex.Message);
            Assert.IsFalse(fixture.Unidad.SaveCalled);
        }

        [TestMethod]
        public void Actualizar_Inexistente_ThrowsExcepcionNegocio()
        {
            EntidadFinancieraFixture fixture = new EntidadFinancieraFixture();
            EntidadFinancieraBLL sut = fixture.CreateSut();

            ExcepcionNegocio ex = Assert.ThrowsExactly<ExcepcionNegocio>(() => sut.Actualizar(new EntidadFinanciera { Id = 99, Nombre = "Nuevo" }));

            Assert.AreEqual("Error.FinancialEntityManagement.NotFound", ex.Message);
        }

        [TestMethod]
        public void Actualizar_Valido_ActualizaAuditaYGuarda()
        {
            EntidadFinancieraFixture fixture = new EntidadFinancieraFixture();
            fixture.EntidadRepo.Entidades.Add(1, new EntidadFinanciera { Id = 1, Nombre = "Galicia" });
            EntidadFinancieraBLL sut = fixture.CreateSut();

            bool resultado = sut.Actualizar(new EntidadFinanciera { Id = 1, Nombre = "  Banco Galicia  " });

            Assert.IsTrue(resultado);
            Assert.AreEqual("Banco Galicia", fixture.EntidadRepo.Entidades[1].Nombre);
            Assert.AreEqual("FormFinancialEntityManagement.Value.Activity.Update", fixture.Bitacora.Calls[0].Actividad);
            Assert.IsTrue(fixture.Unidad.SaveCalled);
        }

        [TestMethod]
        public void Actualizar_NombreDeOtraEntidad_ThrowsExcepcionNegocio()
        {
            EntidadFinancieraFixture fixture = new EntidadFinancieraFixture();
            fixture.EntidadRepo.Entidades.Add(1, new EntidadFinanciera { Id = 1, Nombre = "Galicia" });
            fixture.EntidadRepo.Entidades.Add(2, new EntidadFinanciera { Id = 2, Nombre = "Nacion" });
            EntidadFinancieraBLL sut = fixture.CreateSut();

            ExcepcionNegocio ex = Assert.ThrowsExactly<ExcepcionNegocio>(() => sut.Actualizar(new EntidadFinanciera { Id = 2, Nombre = "galicia" }));

            Assert.AreEqual("Error.FinancialEntityManagement.DuplicateName", ex.Message);
        }

        [TestMethod]
        public void Eliminar_Inexistente_ThrowsExcepcionNegocio()
        {
            EntidadFinancieraFixture fixture = new EntidadFinancieraFixture();
            EntidadFinancieraBLL sut = fixture.CreateSut();

            ExcepcionNegocio ex = Assert.ThrowsExactly<ExcepcionNegocio>(() => sut.Eliminar(new EntidadFinanciera { Id = 99 }));

            Assert.AreEqual("Error.FinancialEntityManagement.NotFound", ex.Message);
        }

        [TestMethod]
        public void Eliminar_ConMediosPagoEnSuCatalogo_ThrowsExcepcionNegocio()
        {
            EntidadFinancieraFixture fixture = new EntidadFinancieraFixture();
            fixture.EntidadRepo.Entidades.Add(1, new EntidadFinanciera { Id = 1, Nombre = "Galicia" });
            fixture.Unidad.MedioPagoEntidadRepo.MediosPagoEntidad.Add(10, new MedioPagoEntidad
            {
                Id = 10,
                EntidadFinanciera = new EntidadFinanciera { Id = 1 },
                MedioPago = new MedioPago { Id = 3 }
            });
            EntidadFinancieraBLL sut = fixture.CreateSut();

            ExcepcionNegocio ex = Assert.ThrowsExactly<ExcepcionNegocio>(() => sut.Eliminar(new EntidadFinanciera { Id = 1 }));

            Assert.AreEqual("Error.FinancialEntityManagement.InUse", ex.Message);
            Assert.IsTrue(fixture.EntidadRepo.Entidades.ContainsKey(1));
            Assert.IsNull(fixture.EntidadRepo.LastDeleted);
            Assert.IsFalse(fixture.Unidad.SaveCalled);
        }

        [TestMethod]
        public void Eliminar_ConCuentasFinancierasAsociadas_ThrowsExcepcionNegocio()
        {
            EntidadFinancieraFixture fixture = new EntidadFinancieraFixture();
            fixture.EntidadRepo.Entidades.Add(1, new EntidadFinanciera { Id = 1, Nombre = "Galicia" });
            fixture.Unidad.CuentaFinancieraRepo.Cuentas.Add(20, new CuentaFinanciera
            {
                Id = 20,
                Usuario = new Usuario { Id = 7 },
                EntidadFinanciera = new EntidadFinanciera { Id = 1 },
                Alias = "Mi cuenta"
            });
            EntidadFinancieraBLL sut = fixture.CreateSut();

            ExcepcionNegocio ex = Assert.ThrowsExactly<ExcepcionNegocio>(() => sut.Eliminar(new EntidadFinanciera { Id = 1 }));

            Assert.AreEqual("Error.FinancialEntityManagement.InUse", ex.Message);
            Assert.IsFalse(fixture.Unidad.SaveCalled);
        }

        [TestMethod]
        public void Eliminar_FallaDelBorradoConViolacionForanea_InformaDeleteFailed()
        {
            EntidadFinancieraFixture fixture = new EntidadFinancieraFixture();
            fixture.EntidadRepo.Entidades.Add(1, new EntidadFinanciera { Id = 1, Nombre = "Galicia" });
            fixture.EntidadRepo.ExcepcionEnEliminar = FakeSqlExceptionFactory.CrearViolacionForanea();
            EntidadFinancieraBLL sut = fixture.CreateSut();

            ExcepcionTecnica ex = Assert.ThrowsExactly<ExcepcionTecnica>(() => sut.Eliminar(new EntidadFinanciera { Id = 1 }));

            Assert.AreEqual("Error.FinancialEntityManagement.DeleteFailed", ex.Message);
            Assert.IsFalse(fixture.Unidad.SaveCalled);
        }

        [TestMethod]
        public void Eliminar_FallaTecnicaConViolacionForaneaEnvuelta_NoSeConvierteEnUso()
        {
            EntidadFinancieraFixture fixture = new EntidadFinancieraFixture();
            fixture.EntidadRepo.Entidades.Add(1, new EntidadFinanciera { Id = 1, Nombre = "Galicia" });
            fixture.EntidadRepo.ExcepcionEnEliminar = FakeSqlExceptionFactory.CrearViolacionForaneaEnvuelta();
            EntidadFinancieraBLL sut = fixture.CreateSut();

            ExcepcionTecnica ex = Assert.ThrowsExactly<ExcepcionTecnica>(() => sut.Eliminar(new EntidadFinanciera { Id = 1 }));

            Assert.AreEqual("Error.Technical.SqlFailure", ex.Message);
            Assert.IsFalse(fixture.Unidad.SaveCalled);
        }

        [TestMethod]
        public void Eliminar_Valido_EliminaAuditaYGuarda()
        {
            EntidadFinancieraFixture fixture = new EntidadFinancieraFixture();
            fixture.EntidadRepo.Entidades.Add(1, new EntidadFinanciera { Id = 1, Nombre = "Galicia" });
            EntidadFinancieraBLL sut = fixture.CreateSut();

            bool resultado = sut.Eliminar(new EntidadFinanciera { Id = 1 });

            Assert.IsTrue(resultado);
            Assert.IsFalse(fixture.EntidadRepo.Entidades.ContainsKey(1));
            Assert.AreEqual("FormFinancialEntityManagement.Value.Activity.Deletion", fixture.Bitacora.Calls[0].Actividad);
            Assert.IsTrue(fixture.Unidad.SaveCalled);
        }

        [TestMethod]
        public void AgregarMedioPagoEntidad_EntidadInexistente_ThrowsExcepcionNegocio()
        {
            EntidadFinancieraFixture fixture = new EntidadFinancieraFixture();
            fixture.Unidad.MedioPagoRepo.MediosPago.Add(3, new MedioPago { Id = 3, Tipo = TipoMedioPago.TarjetaCredito, Marca = "Visa" });
            EntidadFinancieraBLL sut = fixture.CreateSut();

            ExcepcionNegocio ex = Assert.ThrowsExactly<ExcepcionNegocio>(() => sut.AgregarMedioPagoEntidad(new MedioPagoEntidad { EntidadFinanciera = new EntidadFinanciera { Id = 99 }, MedioPago = new MedioPago { Id = 3 }, Segmento = "Eminent" }));

            Assert.AreEqual("Error.FinancialEntityManagement.NotFound", ex.Message);
        }

        [TestMethod]
        public void AgregarMedioPagoEntidad_MedioPagoInexistente_ThrowsExcepcionNegocio()
        {
            EntidadFinancieraFixture fixture = new EntidadFinancieraFixture();
            fixture.EntidadRepo.Entidades.Add(1, new EntidadFinanciera { Id = 1, Nombre = "Galicia" });
            EntidadFinancieraBLL sut = fixture.CreateSut();

            ExcepcionNegocio ex = Assert.ThrowsExactly<ExcepcionNegocio>(() => sut.AgregarMedioPagoEntidad(new MedioPagoEntidad { EntidadFinanciera = new EntidadFinanciera { Id = 1 }, MedioPago = new MedioPago { Id = 99 }, Segmento = "Eminent" }));

            Assert.AreEqual("Error.FinancialEntityManagement.PaymentMethodNotFound", ex.Message);
        }

        [TestMethod]
        public void AgregarMedioPagoEntidad_MismoMedioYSegmentoYaEnElCatalogo_ThrowsExcepcionNegocio()
        {
            EntidadFinancieraFixture fixture = new EntidadFinancieraFixture();
            fixture.EntidadRepo.Entidades.Add(1, new EntidadFinanciera { Id = 1, Nombre = "Galicia" });
            fixture.Unidad.MedioPagoRepo.MediosPago.Add(3, new MedioPago { Id = 3, Tipo = TipoMedioPago.TarjetaCredito, Marca = "Visa" });
            fixture.Unidad.MedioPagoEntidadRepo.MediosPagoEntidad.Add(10, new MedioPagoEntidad
            {
                Id = 10,
                EntidadFinanciera = new EntidadFinanciera { Id = 1 },
                MedioPago = new MedioPago { Id = 3 },
                Segmento = "Eminent"
            });
            EntidadFinancieraBLL sut = fixture.CreateSut();

            ExcepcionNegocio ex = Assert.ThrowsExactly<ExcepcionNegocio>(() => sut.AgregarMedioPagoEntidad(new MedioPagoEntidad { EntidadFinanciera = new EntidadFinanciera { Id = 1 }, MedioPago = new MedioPago { Id = 3 }, Segmento = " eminent " }));

            Assert.AreEqual("Error.FinancialEntityManagement.PaymentMethodAlreadyAdded", ex.Message);
            Assert.IsFalse(fixture.Unidad.SaveCalled);
        }

        [TestMethod]
        public void AgregarMedioPagoEntidad_Valido_NormalizaSegmentoInsertaAuditaYGuarda()
        {
            EntidadFinancieraFixture fixture = new EntidadFinancieraFixture();
            fixture.EntidadRepo.Entidades.Add(1, new EntidadFinanciera { Id = 1, Nombre = "Galicia" });
            fixture.Unidad.MedioPagoRepo.MediosPago.Add(3, new MedioPago { Id = 3, Tipo = TipoMedioPago.TarjetaCredito, Marca = "Visa" });
            EntidadFinancieraBLL sut = fixture.CreateSut();

            MedioPagoEntidad creado = sut.AgregarMedioPagoEntidad(new MedioPagoEntidad { EntidadFinanciera = new EntidadFinanciera { Id = 1 }, MedioPago = new MedioPago { Id = 3 }, Segmento = "  Eminent  " });

            MedioPagoEntidad insertado = fixture.Unidad.MedioPagoEntidadRepo.MediosPagoEntidad[creado.Id];
            Assert.AreEqual(1, insertado.EntidadFinanciera.Id);
            Assert.AreEqual(3, insertado.MedioPago.Id);
            Assert.AreEqual("Eminent", insertado.Segmento);
            Assert.AreEqual("FormFinancialEntityManagement.Value.Activity.PaymentMethodAddition", fixture.Bitacora.Calls[0].Actividad);
            Assert.IsTrue(fixture.Unidad.SaveCalled);
        }

        [TestMethod]
        public void AgregarMedioPagoEntidad_SegmentoEnBlanco_PersisteSegmentoNulo()
        {
            EntidadFinancieraFixture fixture = new EntidadFinancieraFixture();
            fixture.EntidadRepo.Entidades.Add(1, new EntidadFinanciera { Id = 1, Nombre = "Galicia" });
            fixture.Unidad.MedioPagoRepo.MediosPago.Add(3, new MedioPago { Id = 3, Tipo = TipoMedioPago.TarjetaDebito, Marca = "Visa" });
            EntidadFinancieraBLL sut = fixture.CreateSut();

            MedioPagoEntidad creado = sut.AgregarMedioPagoEntidad(new MedioPagoEntidad { EntidadFinanciera = new EntidadFinanciera { Id = 1 }, MedioPago = new MedioPago { Id = 3 }, Segmento = "  " });

            Assert.IsNull(fixture.Unidad.MedioPagoEntidadRepo.MediosPagoEntidad[creado.Id].Segmento);
        }

        [TestMethod]
        public void QuitarMedioPagoEntidad_Inexistente_ThrowsExcepcionNegocio()
        {
            EntidadFinancieraFixture fixture = new EntidadFinancieraFixture();
            EntidadFinancieraBLL sut = fixture.CreateSut();

            ExcepcionNegocio ex = Assert.ThrowsExactly<ExcepcionNegocio>(() => sut.QuitarMedioPagoEntidad(new MedioPagoEntidad { Id = 99 }));

            Assert.AreEqual("Error.FinancialEntityManagement.EntityPaymentMethodNotFound", ex.Message);
        }

        [TestMethod]
        public void QuitarMedioPagoEntidad_ContratadoEnUnaCuenta_ThrowsExcepcionNegocioYNoElimina()
        {
            EntidadFinancieraFixture fixture = new EntidadFinancieraFixture();
            fixture.Unidad.MedioPagoEntidadRepo.MediosPagoEntidad.Add(10, new MedioPagoEntidad
            {
                Id = 10,
                EntidadFinanciera = new EntidadFinanciera { Id = 1 },
                MedioPago = new MedioPago { Id = 3 }
            });
            fixture.Unidad.CuentaFinancieraRepo.MedioPagoEntidadIdsPorCuenta[20] = new List<int> { 10 };
            EntidadFinancieraBLL sut = fixture.CreateSut();

            ExcepcionNegocio ex = Assert.ThrowsExactly<ExcepcionNegocio>(() => sut.QuitarMedioPagoEntidad(new MedioPagoEntidad { Id = 10 }));

            Assert.AreEqual("Error.FinancialEntityManagement.EntityPaymentMethodInUse", ex.Message);
            Assert.IsNull(fixture.Unidad.MedioPagoEntidadRepo.LastDeleted);
            Assert.IsFalse(fixture.Unidad.SaveCalled);
        }

        [TestMethod]
        public void QuitarMedioPagoEntidad_FallaDelBorradoConViolacionForanea_InformaDeleteFailed()
        {
            EntidadFinancieraFixture fixture = new EntidadFinancieraFixture();
            fixture.Unidad.MedioPagoEntidadRepo.MediosPagoEntidad.Add(10, new MedioPagoEntidad
            {
                Id = 10,
                EntidadFinanciera = new EntidadFinanciera { Id = 1 },
                MedioPago = new MedioPago { Id = 3 }
            });
            fixture.Unidad.MedioPagoEntidadRepo.ExcepcionEnEliminar = FakeSqlExceptionFactory.CrearViolacionForanea();
            EntidadFinancieraBLL sut = fixture.CreateSut();

            ExcepcionTecnica ex = Assert.ThrowsExactly<ExcepcionTecnica>(() => sut.QuitarMedioPagoEntidad(new MedioPagoEntidad { Id = 10 }));

            Assert.AreEqual("Error.FinancialEntityManagement.PaymentMethodRemoveFailed", ex.Message);
            Assert.IsFalse(fixture.Unidad.SaveCalled);
        }

        [TestMethod]
        public void QuitarMedioPagoEntidad_FallaTecnicaConViolacionForaneaEnvuelta_NoSeConvierteEnUso()
        {
            EntidadFinancieraFixture fixture = new EntidadFinancieraFixture();
            fixture.Unidad.MedioPagoEntidadRepo.MediosPagoEntidad.Add(10, new MedioPagoEntidad
            {
                Id = 10,
                EntidadFinanciera = new EntidadFinanciera { Id = 1 },
                MedioPago = new MedioPago { Id = 3 }
            });
            fixture.Unidad.MedioPagoEntidadRepo.ExcepcionEnEliminar = FakeSqlExceptionFactory.CrearViolacionForaneaEnvuelta();
            EntidadFinancieraBLL sut = fixture.CreateSut();

            ExcepcionTecnica ex = Assert.ThrowsExactly<ExcepcionTecnica>(() => sut.QuitarMedioPagoEntidad(new MedioPagoEntidad { Id = 10 }));

            Assert.AreEqual("Error.Technical.SqlFailure", ex.Message);
            Assert.IsFalse(fixture.Unidad.SaveCalled);
        }

        [TestMethod]
        public void QuitarMedioPagoEntidad_Valido_EliminaAuditaYGuarda()
        {
            EntidadFinancieraFixture fixture = new EntidadFinancieraFixture();
            fixture.Unidad.MedioPagoEntidadRepo.MediosPagoEntidad.Add(10, new MedioPagoEntidad
            {
                Id = 10,
                EntidadFinanciera = new EntidadFinanciera { Id = 1 },
                MedioPago = new MedioPago { Id = 3 }
            });
            EntidadFinancieraBLL sut = fixture.CreateSut();

            bool resultado = sut.QuitarMedioPagoEntidad(new MedioPagoEntidad { Id = 10 });

            Assert.IsTrue(resultado);
            Assert.IsFalse(fixture.Unidad.MedioPagoEntidadRepo.MediosPagoEntidad.ContainsKey(10));
            Assert.AreEqual("FormFinancialEntityManagement.Value.Activity.PaymentMethodRemoval", fixture.Bitacora.Calls[0].Actividad);
            Assert.IsTrue(fixture.Unidad.SaveCalled);
        }

        [TestMethod]
        public void Eliminar_ReferenciadaPorUnaPromocion_ThrowsExcepcionNegocioYNoElimina()
        {
            EntidadFinancieraFixture fixture = new EntidadFinancieraFixture();
            fixture.EntidadRepo.Entidades.Add(1, new EntidadFinanciera { Id = 1, Nombre = "Galicia" });
            fixture.Unidad.PromocionEntidadFinancieraRepo.EntidadFinancieraIdsPorPromocion[7] = new System.Collections.Generic.List<int> { 1 };
            EntidadFinancieraBLL sut = fixture.CreateSut();

            ExcepcionNegocio ex = Assert.ThrowsExactly<ExcepcionNegocio>(() => sut.Eliminar(new EntidadFinanciera { Id = 1 }));

            Assert.AreEqual("Error.FinancialEntityManagement.InUse", ex.Message);
            Assert.IsNull(fixture.EntidadRepo.LastDeleted);
            Assert.IsFalse(fixture.Unidad.SaveCalled);
        }

        [TestMethod]
        public void Crear_Valido_DevuelveLaMismaInstanciaConId()
        {
            EntidadFinancieraFixture fixture = new EntidadFinancieraFixture();
            EntidadFinancieraBLL sut = fixture.CreateSut();
            EntidadFinanciera entidad = new EntidadFinanciera { Nombre = "Galicia" };

            EntidadFinanciera creada = sut.Crear(entidad);

            Assert.AreSame(entidad, creada);
            Assert.IsTrue(creada.Id > 0);
        }

        [TestMethod]
        public void Crear_EntidadNula_ThrowsExcepcionNegocioNameRequired()
        {
            EntidadFinancieraFixture fixture = new EntidadFinancieraFixture();
            EntidadFinancieraBLL sut = fixture.CreateSut();

            ExcepcionNegocio ex = Assert.ThrowsExactly<ExcepcionNegocio>(() => sut.Crear(null));

            Assert.AreEqual("Error.FinancialEntityManagement.NameRequired", ex.Message);
            Assert.AreEqual(0, fixture.UnidadFactory.CrearCallCount);
        }

        [TestMethod]
        public void Actualizar_Valido_CopiaElNombreSobreLaPersistida()
        {
            EntidadFinancieraFixture fixture = new EntidadFinancieraFixture();
            EntidadFinanciera persistida = new EntidadFinanciera { Id = 1, Nombre = "Galicia" };
            fixture.EntidadRepo.Entidades.Add(1, persistida);
            EntidadFinancieraBLL sut = fixture.CreateSut();

            sut.Actualizar(new EntidadFinanciera { Id = 1, Nombre = "Banco Galicia" });

            Assert.AreSame(persistida, fixture.EntidadRepo.LastUpdated);
            Assert.AreEqual("Banco Galicia", persistida.Nombre);
        }

        [TestMethod]
        public void Eliminar_EntidadNula_ThrowsExcepcionNegocioNotFound()
        {
            EntidadFinancieraFixture fixture = new EntidadFinancieraFixture();
            EntidadFinancieraBLL sut = fixture.CreateSut();

            ExcepcionNegocio ex = Assert.ThrowsExactly<ExcepcionNegocio>(() => sut.Eliminar(null));

            Assert.AreEqual("Error.FinancialEntityManagement.NotFound", ex.Message);
            Assert.IsNull(fixture.EntidadRepo.LastDeleted);
        }

        [TestMethod]
        public void Eliminar_Valido_EliminaLaPersistida()
        {
            EntidadFinancieraFixture fixture = new EntidadFinancieraFixture();
            EntidadFinanciera persistida = new EntidadFinanciera { Id = 1, Nombre = "Galicia" };
            fixture.EntidadRepo.Entidades.Add(1, persistida);
            EntidadFinancieraBLL sut = fixture.CreateSut();

            sut.Eliminar(new EntidadFinanciera { Id = 1 });

            Assert.AreSame(persistida, fixture.EntidadRepo.LastDeleted);
        }

        [TestMethod]
        public void AgregarMedioPagoEntidad_Valido_DevuelveLaMismaInstanciaConLasEntidadesPersistidas()
        {
            EntidadFinancieraFixture fixture = new EntidadFinancieraFixture();
            EntidadFinanciera galicia = new EntidadFinanciera { Id = 1, Nombre = "Galicia" };
            fixture.EntidadRepo.Entidades.Add(1, galicia);
            MedioPago visa = new MedioPago { Id = 3, Tipo = TipoMedioPago.TarjetaCredito, Marca = "Visa" };
            fixture.Unidad.MedioPagoRepo.MediosPago.Add(3, visa);
            EntidadFinancieraBLL sut = fixture.CreateSut();
            MedioPagoEntidad medioPagoEntidad = new MedioPagoEntidad
            {
                EntidadFinanciera = new EntidadFinanciera { Id = 1 },
                MedioPago = new MedioPago { Id = 3 },
                Segmento = "Eminent"
            };

            MedioPagoEntidad creado = sut.AgregarMedioPagoEntidad(medioPagoEntidad);

            Assert.AreSame(medioPagoEntidad, creado);
            Assert.IsTrue(creado.Id > 0);
            Assert.AreSame(galicia, creado.EntidadFinanciera);
            Assert.AreSame(visa, creado.MedioPago);
        }

        [TestMethod]
        public void AgregarMedioPagoEntidad_Nulo_ThrowsExcepcionNegocioNotFound()
        {
            EntidadFinancieraFixture fixture = new EntidadFinancieraFixture();
            EntidadFinancieraBLL sut = fixture.CreateSut();

            ExcepcionNegocio ex = Assert.ThrowsExactly<ExcepcionNegocio>(() => sut.AgregarMedioPagoEntidad(null));

            Assert.AreEqual("Error.FinancialEntityManagement.NotFound", ex.Message);
        }

        [TestMethod]
        public void QuitarMedioPagoEntidad_Nulo_ThrowsExcepcionNegocioNotFound()
        {
            EntidadFinancieraFixture fixture = new EntidadFinancieraFixture();
            EntidadFinancieraBLL sut = fixture.CreateSut();

            ExcepcionNegocio ex = Assert.ThrowsExactly<ExcepcionNegocio>(() => sut.QuitarMedioPagoEntidad(null));

            Assert.AreEqual("Error.FinancialEntityManagement.EntityPaymentMethodNotFound", ex.Message);
            Assert.IsNull(fixture.Unidad.MedioPagoEntidadRepo.LastDeleted);
        }

        [TestMethod]
        public void QuitarMedioPagoEntidad_Valido_EliminaElPersistido()
        {
            EntidadFinancieraFixture fixture = new EntidadFinancieraFixture();
            MedioPagoEntidad persistido = new MedioPagoEntidad
            {
                Id = 10,
                EntidadFinanciera = new EntidadFinanciera { Id = 1 },
                MedioPago = new MedioPago { Id = 3 }
            };
            fixture.Unidad.MedioPagoEntidadRepo.MediosPagoEntidad.Add(10, persistido);
            EntidadFinancieraBLL sut = fixture.CreateSut();

            sut.QuitarMedioPagoEntidad(new MedioPagoEntidad { Id = 10 });

            Assert.AreSame(persistido, fixture.Unidad.MedioPagoEntidadRepo.LastDeleted);
        }

        private sealed class EntidadFinancieraFixture
        {
            public readonly FakeEntidadFinancieraRepositorio EntidadRepo = new FakeEntidadFinancieraRepositorio();
            public readonly SpyBitacoraBLL Bitacora = new SpyBitacoraBLL();
            public readonly FakeUnidadDeTrabajo Unidad;
            public readonly FakeUnidadDeTrabajoFactory UnidadFactory;

            public EntidadFinancieraFixture()
            {
                Unidad = new FakeUnidadDeTrabajo { EntidadFinancieraRepo = EntidadRepo };
                UnidadFactory = new FakeUnidadDeTrabajoFactory(Unidad);
            }

            public EntidadFinancieraBLL CreateSut()
            {
                return new EntidadFinancieraBLL(EntidadRepo, UnidadFactory, Bitacora);
            }
        }
    }
}
