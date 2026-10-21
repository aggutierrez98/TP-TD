using System.Collections.Generic;
using System.Linq;
using BE;
using BLL.Tests.TestDoubles;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SERVICIOS;

namespace BLL.Tests
{
    [TestClass]
    public sealed class CuentaFinancieraBLLTests
    {
        private const int UsuarioActualId = 7;
        private const int OtroUsuarioId = 8;

        [TestMethod]
        public void ObtenerMisCuentas_DevuelveSoloLasDelUsuarioActual()
        {
            CuentaFinancieraFixture fixture = new CuentaFinancieraFixture();
            fixture.AgregarCuenta(1, UsuarioActualId, 100);
            fixture.AgregarCuenta(2, OtroUsuarioId, 100);
            CuentaFinancieraBLL sut = fixture.CreateSut();

            List<CuentaFinanciera> resultado = sut.ObtenerMisCuentas();

            Assert.AreEqual(1, resultado.Count);
            Assert.AreEqual(1, resultado[0].Id);
        }

        [TestMethod]
        public void Crear_AliasVacio_ThrowsExcepcionNegocioAndDoesNotCreateUow()
        {
            CuentaFinancieraFixture fixture = new CuentaFinancieraFixture();
            fixture.AgregarEntidad(100);
            CuentaFinancieraBLL sut = fixture.CreateSut();

            ExcepcionNegocio ex = Assert.ThrowsExactly<ExcepcionNegocio>(() => sut.Crear(Cuenta(100, "  ", TipoCuentaFinanciera.CajaAhorro, null)));

            Assert.AreEqual("Error.FinancialAccountManagement.AliasRequired", ex.Message);
            Assert.AreEqual(0, fixture.UnidadFactory.CrearCallCount);
        }

        [TestMethod]
        public void Crear_TipoInvalido_ThrowsExcepcionNegocioAndDoesNotCreateUow()
        {
            CuentaFinancieraFixture fixture = new CuentaFinancieraFixture();
            fixture.AgregarEntidad(100);
            CuentaFinancieraBLL sut = fixture.CreateSut();

            ExcepcionNegocio ex = Assert.ThrowsExactly<ExcepcionNegocio>(() => sut.Crear(Cuenta(100, "Sueldo", (TipoCuentaFinanciera)99, null)));

            Assert.AreEqual("Error.FinancialAccountManagement.InvalidType", ex.Message);
            Assert.AreEqual(0, fixture.UnidadFactory.CrearCallCount);
        }

        [TestMethod]
        public void Crear_EntidadInexistente_ThrowsExcepcionNegocio()
        {
            CuentaFinancieraFixture fixture = new CuentaFinancieraFixture();
            CuentaFinancieraBLL sut = fixture.CreateSut();

            ExcepcionNegocio ex = Assert.ThrowsExactly<ExcepcionNegocio>(() => sut.Crear(Cuenta(999, "Sueldo", TipoCuentaFinanciera.CajaAhorro, null)));

            Assert.AreEqual("Error.FinancialAccountManagement.EntityNotFound", ex.Message);
            Assert.IsFalse(fixture.Unidad.SaveCalled);
        }

        [TestMethod]
        public void Crear_Valido_InsertaParaElUsuarioActualSinCredencialesAuditaYGuarda()
        {
            CuentaFinancieraFixture fixture = new CuentaFinancieraFixture();
            fixture.AgregarEntidad(100);
            fixture.AgregarMedioPagoEntidad(10, 100);
            fixture.AgregarMedioPagoEntidad(11, 100);
            CuentaFinancieraBLL sut = fixture.CreateSut();

            CuentaFinanciera recibida = Cuenta(100, "  Sueldo  ", TipoCuentaFinanciera.CajaAhorro, new List<int> { 10, 11, 10 });
            CuentaFinanciera creada = sut.Crear(recibida);
            Assert.AreSame(recibida, creada);

            CuentaFinanciera insertada = fixture.CuentaRepo.Cuentas[creada.Id];
            Assert.AreEqual(UsuarioActualId, insertada.Usuario.Id);
            Assert.AreEqual(100, insertada.EntidadFinanciera.Id);
            Assert.AreEqual("Sueldo", insertada.Alias);
            Assert.AreEqual(TipoCuentaFinanciera.CajaAhorro, insertada.Tipo);
            Assert.IsNull(insertada.CredencialesCifradas);
            CollectionAssert.AreEqual(new List<int> { 10, 11 }, fixture.CuentaRepo.MedioPagoEntidadIdsPorCuenta[creada.Id]);
            Assert.AreEqual(1, fixture.Bitacora.Calls.Count);
            Assert.AreEqual("FormFinancialAccountManagement.Value.Activity.Creation", fixture.Bitacora.Calls[0].Actividad);
            Assert.AreEqual(1, fixture.UnidadFactory.CrearCallCount);
            Assert.IsTrue(fixture.Unidad.SaveCalled);
        }

        [TestMethod]
        public void Crear_MedioDeOtraEntidad_ThrowsExcepcionNegocioSinCrearLaCuenta()
        {
            CuentaFinancieraFixture fixture = new CuentaFinancieraFixture();
            fixture.AgregarEntidad(100);
            fixture.AgregarEntidad(200);
            fixture.AgregarMedioPagoEntidad(10, 100);
            fixture.AgregarMedioPagoEntidad(20, 200);
            CuentaFinancieraBLL sut = fixture.CreateSut();

            ExcepcionNegocio ex = Assert.ThrowsExactly<ExcepcionNegocio>(() => sut.Crear(Cuenta(100, "Sueldo", TipoCuentaFinanciera.CajaAhorro, new List<int> { 10, 20 })));

            Assert.AreEqual("Error.FinancialAccountManagement.PaymentMethodNotFromEntity", ex.Message);
            Assert.AreEqual(0, fixture.CuentaRepo.Cuentas.Count);
            Assert.AreEqual(0, fixture.CuentaRepo.ReemplazarCallCount);
            Assert.IsFalse(fixture.Unidad.SaveCalled);
        }

        [TestMethod]
        public void Crear_ListaNula_CreaLaCuentaSinMediosContratados()
        {
            CuentaFinancieraFixture fixture = new CuentaFinancieraFixture();
            fixture.AgregarEntidad(100);
            CuentaFinancieraBLL sut = fixture.CreateSut();

            CuentaFinanciera creada = sut.Crear(Cuenta(100, "Sueldo", TipoCuentaFinanciera.CajaAhorro, null));

            Assert.IsTrue(fixture.CuentaRepo.Cuentas.ContainsKey(creada.Id));
            List<int> contratados;
            bool tieneContratados = fixture.CuentaRepo.MedioPagoEntidadIdsPorCuenta.TryGetValue(creada.Id, out contratados) && contratados.Count > 0;
            Assert.IsFalse(tieneContratados);
            Assert.IsTrue(fixture.Unidad.SaveCalled);
        }

        [TestMethod]
        public void Actualizar_AliasYTipoNuevos_PersisteRecortadoAuditaYGuarda()
        {
            CuentaFinancieraFixture fixture = new CuentaFinancieraFixture();
            fixture.AgregarEntidad(100);
            fixture.AgregarCuenta(1, UsuarioActualId, 100);
            CuentaFinancieraBLL sut = fixture.CreateSut();

            CuentaFinanciera actualizada = sut.Actualizar(Cuenta(100, "  Ahorros  ", TipoCuentaFinanciera.CuentaCorriente, null, 1));

            Assert.AreEqual(1, actualizada.Id);
            CuentaFinanciera guardada = fixture.CuentaRepo.Cuentas[1];
            Assert.AreEqual("Ahorros", guardada.Alias);
            Assert.AreEqual(TipoCuentaFinanciera.CuentaCorriente, guardada.Tipo);
            Assert.AreEqual(100, guardada.EntidadFinanciera.Id);
            Assert.AreEqual(UsuarioActualId, guardada.Usuario.Id);
            Assert.AreEqual(1, fixture.Bitacora.Calls.Count);
            Assert.AreEqual("FormFinancialAccountManagement.Value.Activity.Update", fixture.Bitacora.Calls[0].Actividad);
            Assert.AreEqual("FormFinancialAccountManagement.Value.Info.Update", fixture.Bitacora.Calls[0].Informacion);
            Assert.IsTrue(fixture.Unidad.SaveCalled);
        }

        [TestMethod]
        public void Actualizar_AliasVacio_ThrowsExcepcionNegocioSinCambios()
        {
            CuentaFinancieraFixture fixture = new CuentaFinancieraFixture();
            fixture.AgregarEntidad(100);
            fixture.AgregarCuenta(1, UsuarioActualId, 100);
            CuentaFinancieraBLL sut = fixture.CreateSut();

            ExcepcionNegocio ex = Assert.ThrowsExactly<ExcepcionNegocio>(() => sut.Actualizar(Cuenta(100, "   ", TipoCuentaFinanciera.CajaAhorro, null, 1)));

            Assert.AreEqual("Error.FinancialAccountManagement.AliasRequired", ex.Message);
            Assert.AreEqual("Alias1", fixture.CuentaRepo.Cuentas[1].Alias);
            Assert.IsFalse(fixture.Unidad.SaveCalled);
        }

        [TestMethod]
        public void Actualizar_ReemplazaMediosSinDuplicados()
        {
            CuentaFinancieraFixture fixture = new CuentaFinancieraFixture();
            fixture.AgregarEntidad(100);
            fixture.AgregarMedioPagoEntidad(10, 100);
            fixture.AgregarMedioPagoEntidad(11, 100);
            fixture.AgregarMedioPagoEntidad(12, 100);
            fixture.AgregarCuenta(1, UsuarioActualId, 100);
            fixture.CuentaRepo.MedioPagoEntidadIdsPorCuenta[1] = new List<int> { 10 };
            CuentaFinancieraBLL sut = fixture.CreateSut();

            sut.Actualizar(Cuenta(100, "Sueldo", TipoCuentaFinanciera.CajaAhorro, new List<int> { 11, 12, 11 }, 1));

            CollectionAssert.AreEqual(new List<int> { 11, 12 }, fixture.CuentaRepo.MedioPagoEntidadIdsPorCuenta[1]);
            Assert.IsTrue(fixture.Unidad.SaveCalled);
        }

        [TestMethod]
        public void Actualizar_MedioDeOtraEntidad_ThrowsExcepcionNegocioSinCambios()
        {
            CuentaFinancieraFixture fixture = new CuentaFinancieraFixture();
            fixture.AgregarEntidad(100);
            fixture.AgregarEntidad(200);
            fixture.AgregarMedioPagoEntidad(10, 100);
            fixture.AgregarMedioPagoEntidad(20, 200);
            fixture.AgregarCuenta(1, UsuarioActualId, 100);
            CuentaFinancieraBLL sut = fixture.CreateSut();

            ExcepcionNegocio ex = Assert.ThrowsExactly<ExcepcionNegocio>(() => sut.Actualizar(Cuenta(100, "Sueldo", TipoCuentaFinanciera.CajaAhorro, new List<int> { 10, 20 }, 1)));

            Assert.AreEqual("Error.FinancialAccountManagement.PaymentMethodNotFromEntity", ex.Message);
            Assert.AreEqual(0, fixture.CuentaRepo.ReemplazarCallCount);
            Assert.IsFalse(fixture.Unidad.SaveCalled);
        }

        [TestMethod]
        public void Actualizar_CambioDeEntidad_AsociaLaEntidadNuevaConSusMedios()
        {
            CuentaFinancieraFixture fixture = new CuentaFinancieraFixture();
            fixture.AgregarEntidad(100);
            fixture.AgregarEntidad(200);
            fixture.AgregarMedioPagoEntidad(20, 200);
            fixture.AgregarCuenta(1, UsuarioActualId, 100);
            CuentaFinancieraBLL sut = fixture.CreateSut();

            sut.Actualizar(Cuenta(200, "Sueldo", TipoCuentaFinanciera.CajaAhorro, new List<int> { 20 }, 1));

            Assert.AreEqual(200, fixture.CuentaRepo.Cuentas[1].EntidadFinanciera.Id);
            CollectionAssert.AreEqual(new List<int> { 20 }, fixture.CuentaRepo.MedioPagoEntidadIdsPorCuenta[1]);
        }

        [TestMethod]
        public void Actualizar_EntidadInexistente_ThrowsExcepcionNegocioSinCambios()
        {
            CuentaFinancieraFixture fixture = new CuentaFinancieraFixture();
            fixture.AgregarEntidad(100);
            fixture.AgregarCuenta(1, UsuarioActualId, 100);
            CuentaFinancieraBLL sut = fixture.CreateSut();

            ExcepcionNegocio ex = Assert.ThrowsExactly<ExcepcionNegocio>(() => sut.Actualizar(Cuenta(999, "Sueldo", TipoCuentaFinanciera.CajaAhorro, null, 1)));

            Assert.AreEqual("Error.FinancialAccountManagement.EntityNotFound", ex.Message);
            Assert.AreEqual(100, fixture.CuentaRepo.Cuentas[1].EntidadFinanciera.Id);
            Assert.IsFalse(fixture.Unidad.SaveCalled);
        }

        [TestMethod]
        public void Actualizar_ConservaLasCredencialesCifradas()
        {
            CuentaFinancieraFixture fixture = new CuentaFinancieraFixture();
            fixture.AgregarEntidad(100);
            CuentaFinanciera existente = fixture.AgregarCuenta(1, UsuarioActualId, 100);
            existente.CredencialesCifradas = "secreto-cifrado";
            CuentaFinancieraBLL sut = fixture.CreateSut();

            CuentaFinanciera recibida = Cuenta(100, "Nuevo", TipoCuentaFinanciera.CajaAhorro, null, 1);
            recibida.CredencialesCifradas = "no-confiable";
            sut.Actualizar(recibida);

            Assert.AreEqual("secreto-cifrado", fixture.CuentaRepo.LastUpdated.CredencialesCifradas);
        }

        [TestMethod]
        public void Actualizar_CuentaDeOtroUsuario_ThrowsExcepcionNegocioSinCambios()
        {
            CuentaFinancieraFixture fixture = new CuentaFinancieraFixture();
            fixture.AgregarEntidad(100);
            fixture.AgregarCuenta(1, OtroUsuarioId, 100);
            CuentaFinancieraBLL sut = fixture.CreateSut();

            CuentaFinanciera recibida = Cuenta(100, "Hack", TipoCuentaFinanciera.CajaAhorro, null, 1);
            recibida.Usuario = new Usuario { Id = UsuarioActualId };
            ExcepcionNegocio ex = Assert.ThrowsExactly<ExcepcionNegocio>(() => sut.Actualizar(recibida));

            Assert.AreEqual("Error.FinancialAccountManagement.NotOwned", ex.Message);
            Assert.AreEqual("Alias1", fixture.CuentaRepo.Cuentas[1].Alias);
            Assert.IsFalse(fixture.Unidad.SaveCalled);
        }

        [TestMethod]
        public void Actualizar_CuentaInexistente_ThrowsExcepcionNegocio()
        {
            CuentaFinancieraFixture fixture = new CuentaFinancieraFixture();
            fixture.AgregarEntidad(100);
            CuentaFinancieraBLL sut = fixture.CreateSut();

            ExcepcionNegocio ex = Assert.ThrowsExactly<ExcepcionNegocio>(() => sut.Actualizar(Cuenta(100, "Sueldo", TipoCuentaFinanciera.CajaAhorro, null, 999)));

            Assert.AreEqual("Error.FinancialAccountManagement.NotFound", ex.Message);
            Assert.IsFalse(fixture.Unidad.SaveCalled);
        }

        [TestMethod]
        public void Actualizar_FallaAlReemplazarMedios_ThrowsExcepcionTecnicaSinGuardarNiAuditar()
        {
            CuentaFinancieraFixture fixture = new CuentaFinancieraFixture();
            fixture.AgregarEntidad(100);
            fixture.AgregarMedioPagoEntidad(10, 100);
            fixture.AgregarCuenta(1, UsuarioActualId, 100);
            fixture.CuentaRepo.ThrowOnReemplazar = true;
            CuentaFinancieraBLL sut = fixture.CreateSut();

            ExcepcionTecnica ex = Assert.ThrowsExactly<ExcepcionTecnica>(() => sut.Actualizar(Cuenta(100, "Sueldo", TipoCuentaFinanciera.CajaAhorro, new List<int> { 10 }, 1)));

            Assert.AreEqual("Error.FinancialAccountManagement.UpdateFailed", ex.Message);
            Assert.IsFalse(fixture.Unidad.SaveCalled);
            Assert.AreEqual(0, fixture.Bitacora.Calls.Count);
        }

        [TestMethod]
        public void Eliminar_CuentaDeOtroUsuario_ThrowsExcepcionNegocio()
        {
            CuentaFinancieraFixture fixture = new CuentaFinancieraFixture();
            fixture.AgregarCuenta(1, OtroUsuarioId, 100);
            CuentaFinancieraBLL sut = fixture.CreateSut();

            ExcepcionNegocio ex = Assert.ThrowsExactly<ExcepcionNegocio>(() => sut.Eliminar(new CuentaFinanciera { Id = 1, Usuario = new Usuario { Id = UsuarioActualId } }));

            Assert.AreEqual("Error.FinancialAccountManagement.NotOwned", ex.Message);
            Assert.IsTrue(fixture.CuentaRepo.Cuentas.ContainsKey(1));
        }

        [TestMethod]
        public void Eliminar_Valido_LimpiaMediosPagoEliminaAuditaYGuarda()
        {
            CuentaFinancieraFixture fixture = new CuentaFinancieraFixture();
            fixture.AgregarCuenta(1, UsuarioActualId, 100);
            fixture.CuentaRepo.MedioPagoEntidadIdsPorCuenta[1] = new List<int> { 10, 11 };
            CuentaFinancieraBLL sut = fixture.CreateSut();

            bool resultado = sut.Eliminar(fixture.CuentaRepo.Cuentas[1]);

            Assert.IsTrue(resultado);
            Assert.IsFalse(fixture.CuentaRepo.Cuentas.ContainsKey(1));
            Assert.AreEqual(0, fixture.CuentaRepo.MedioPagoEntidadIdsPorCuenta[1].Count);
            Assert.AreEqual("FormFinancialAccountManagement.Value.Activity.Deletion", fixture.Bitacora.Calls[0].Actividad);
            Assert.IsTrue(fixture.Unidad.SaveCalled);
        }

        private static CuentaFinanciera Cuenta(int entidadId, string alias, TipoCuentaFinanciera tipo, List<int> medios, int id = 0)
        {
            return new CuentaFinanciera
            {
                Id = id, EntidadFinanciera = new EntidadFinanciera { Id = entidadId }, Alias = alias, Tipo = tipo,
                MediosPagoEntidad = medios?.Select(medio => new MedioPagoEntidad { Id = medio }).ToList()
            };
        }

        private sealed class CuentaFinancieraFixture
        {
            public readonly FakeCuentaFinancieraRepositorio CuentaRepo = new FakeCuentaFinancieraRepositorio();
            public readonly FakeEntidadFinancieraRepositorio EntidadRepo = new FakeEntidadFinancieraRepositorio();
            public readonly FakeMedioPagoEntidadRepositorio MedioPagoEntidadRepo = new FakeMedioPagoEntidadRepositorio();
            public readonly SpyBitacoraBLL Bitacora = new SpyBitacoraBLL();
            public readonly FakeSesion Sesion = new FakeSesion();
            public readonly FakeUnidadDeTrabajo Unidad;
            public readonly FakeUnidadDeTrabajoFactory UnidadFactory;

            public CuentaFinancieraFixture()
            {
                Unidad = new FakeUnidadDeTrabajo
                {
                    CuentaFinancieraRepo = CuentaRepo,
                    EntidadFinancieraRepo = EntidadRepo,
                    MedioPagoEntidadRepo = MedioPagoEntidadRepo
                };
                UnidadFactory = new FakeUnidadDeTrabajoFactory(Unidad);
                Sesion.IniciarSesion(new Usuario { Id = UsuarioActualId, Username = "test" });
            }

            public EntidadFinanciera AgregarEntidad(int id)
            {
                EntidadFinanciera entidad = new EntidadFinanciera { Id = id, Nombre = "Entidad" + id };
                EntidadRepo.Entidades.Add(id, entidad);
                return entidad;
            }

            public CuentaFinanciera AgregarCuenta(int id, int usuarioId, int entidadFinancieraId)
            {
                CuentaFinanciera cuenta = new CuentaFinanciera
                {
                    Id = id,
                    Usuario = new Usuario { Id = usuarioId },
                    EntidadFinanciera = new EntidadFinanciera { Id = entidadFinancieraId },
                    Alias = "Alias" + id,
                    Tipo = TipoCuentaFinanciera.CajaAhorro
                };
                CuentaRepo.Cuentas.Add(id, cuenta);
                return cuenta;
            }

            public void AgregarMedioPagoEntidad(int id, int entidadFinancieraId)
            {
                MedioPagoEntidadRepo.MediosPagoEntidad.Add(id, new MedioPagoEntidad
                {
                    Id = id,
                    EntidadFinanciera = new EntidadFinanciera { Id = entidadFinancieraId },
                    MedioPago = new MedioPago { Id = 1 }
                });
            }

            public CuentaFinancieraBLL CreateSut()
            {
                return new CuentaFinancieraBLL(CuentaRepo, UnidadFactory, Sesion, Bitacora);
            }
        }
    }
}
