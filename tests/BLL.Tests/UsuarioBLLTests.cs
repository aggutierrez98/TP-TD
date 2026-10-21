using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ABSTRACCIONES;
using BE;
using BLL.Tests.TestDoubles;
using SERVICIOS;

namespace BLL.Tests
{
    [TestClass]
    public sealed class UsuarioBLLTests
    {
        [TestMethod]
        public void Login_UsuarioInexistente_ReturnsNullAndDoesNotStartSession()
        {
            UsuarioFixture fixture = new UsuarioFixture();
            UsuarioBLL sut = fixture.CreateSut();

            Usuario result = sut.Login(new Usuario { Username = "missing" }, "pwd");

            Assert.IsNull(result);
            Assert.IsFalse(fixture.Sesion.HaySesionActiva());
            Assert.AreEqual("FalloNegocio", fixture.Bitacora.Calls.Single().Kind);
        }

        [TestMethod]
        public void Login_UsuarioInactivo_ReturnsNullAndDoesNotStartSession()
        {
            UsuarioFixture fixture = new UsuarioFixture();
            fixture.UsuarioRepo.Usuarios[1] = new Usuario { Id = 1, Username = "admin", Activo = false };
            UsuarioBLL sut = fixture.CreateSut();

            Usuario result = sut.Login(new Usuario { Username = "admin" }, "pwd");

            Assert.IsNull(result);
            Assert.IsFalse(fixture.Sesion.HaySesionActiva());
            Assert.AreEqual("FalloNegocio", fixture.Bitacora.Calls.Single().Kind);
        }

        [TestMethod]
        public void Login_PasswordInvalido_ReturnsNullAndDoesNotStartSession()
        {
            UsuarioFixture fixture = new UsuarioFixture();
            fixture.UsuarioRepo.Usuarios[1] = new Usuario { Id = 1, Username = "admin", PasswordHash = "hash", Activo = true };
            fixture.PasswordHasher.VerifyResult = false;
            UsuarioBLL sut = fixture.CreateSut();

            Usuario result = sut.Login(new Usuario { Username = "admin" }, "wrong");

            Assert.IsNull(result);
            Assert.IsFalse(fixture.Sesion.HaySesionActiva());
            // El intento fallido ahora persiste el contador dentro de una unidad de
            // trabajo (necesaria para el soft lockout), por eso pasa a ser Interno.
            Assert.AreEqual("FalloNegocioInterno", fixture.Bitacora.Calls.Single().Kind);
            Assert.AreEqual(1, fixture.Unidad.UsuarioRepo.LastEstadoBloqueoUpdate.Item2);
            Assert.IsFalse(fixture.Unidad.UsuarioRepo.LastEstadoBloqueoUpdate.Item3);
        }

        [TestMethod]
        public void Login_PrimerOSegundoIntentoFallido_NoAplicaBloqueoYMantieneMensajeGenerico()
        {
            UsuarioFixture fixture = new UsuarioFixture();
            fixture.UsuarioRepo.Usuarios[1] = new Usuario { Id = 1, Username = "admin", PasswordHash = "hash", Activo = true, IntentosFallidos = 1 };
            fixture.PasswordHasher.VerifyResult = false;
            UsuarioBLL sut = fixture.CreateSut();

            Usuario result = sut.Login(new Usuario { Username = "admin" }, "wrong");

            Assert.IsNull(result);
            Tuple<int, int, bool, DateTime?, string> update = fixture.Unidad.UsuarioRepo.LastEstadoBloqueoUpdate;
            Assert.AreEqual(2, update.Item2);
            Assert.IsFalse(update.Item3);
            Assert.IsNull(update.Item4);
        }

        [TestMethod]
        public void Login_TercerIntentoFallido_AplicaBloqueoTemporalDeUnMinutoYNoLanzaExcepcion()
        {
            UsuarioFixture fixture = new UsuarioFixture();
            fixture.UsuarioRepo.Usuarios[1] = new Usuario { Id = 1, Username = "admin", PasswordHash = "hash", Activo = true, IntentosFallidos = 2 };
            fixture.PasswordHasher.VerifyResult = false;
            UsuarioBLL sut = fixture.CreateSut();

            Usuario result = sut.Login(new Usuario { Username = "admin" }, "wrong");

            Assert.IsNull(result);
            Tuple<int, int, bool, DateTime?, string> update = fixture.Unidad.UsuarioRepo.LastEstadoBloqueoUpdate;
            Assert.AreEqual(3, update.Item2);
            Assert.IsTrue(update.Item3);
            Assert.IsTrue(update.Item4.HasValue);
            Assert.IsTrue(update.Item4.Value > DateTime.Now.AddSeconds(50) && update.Item4.Value <= DateTime.Now.AddMinutes(1).AddSeconds(5));
            Assert.IsTrue(fixture.Unidad.SaveCalled);
        }

        [TestMethod]
        public void Login_CuartoIntentoFallidoTrasExpirarBloqueoCorto_AplicaBloqueoTemporalDeCincoMinutos()
        {
            UsuarioFixture fixture = new UsuarioFixture();
            fixture.UsuarioRepo.Usuarios[1] = new Usuario
            {
                Id = 1,
                Username = "admin",
                PasswordHash = "hash",
                Activo = true,
                IntentosFallidos = 3,
                Bloqueado = true,
                BloqueadoHasta = DateTime.Now.AddSeconds(-1)
            };
            fixture.PasswordHasher.VerifyResult = false;
            UsuarioBLL sut = fixture.CreateSut();

            Usuario result = sut.Login(new Usuario { Username = "admin" }, "wrong");

            Assert.IsNull(result);
            Assert.AreEqual("wrong", fixture.PasswordHasher.LastVerifyPassword);
            Tuple<int, int, bool, DateTime?, string> update = fixture.Unidad.UsuarioRepo.LastEstadoBloqueoUpdate;
            Assert.AreEqual(4, update.Item2);
            Assert.IsTrue(update.Item3);
            Assert.IsTrue(update.Item4.HasValue);
            Assert.IsTrue(update.Item4.Value > DateTime.Now.AddMinutes(4).AddSeconds(50) && update.Item4.Value <= DateTime.Now.AddMinutes(5).AddSeconds(5));
        }

        [TestMethod]
        public void Login_QuintoIntentoFallidoTrasExpirarBloqueoLargo_AplicaBloqueoPermanente()
        {
            UsuarioFixture fixture = new UsuarioFixture();
            fixture.UsuarioRepo.Usuarios[1] = new Usuario
            {
                Id = 1,
                Username = "admin",
                PasswordHash = "hash",
                Activo = true,
                IntentosFallidos = 4,
                Bloqueado = true,
                BloqueadoHasta = DateTime.Now.AddSeconds(-1)
            };
            fixture.PasswordHasher.VerifyResult = false;
            UsuarioBLL sut = fixture.CreateSut();

            Usuario result = sut.Login(new Usuario { Username = "admin" }, "wrong");

            Assert.IsNull(result);
            Tuple<int, int, bool, DateTime?, string> update = fixture.Unidad.UsuarioRepo.LastEstadoBloqueoUpdate;
            Assert.AreEqual(5, update.Item2);
            Assert.IsTrue(update.Item3);
            Assert.IsNull(update.Item4);
        }

        [TestMethod]
        public void Login_ConBloqueoPermanenteActivo_LanzaExcepcionNegocioSinVerificarPasswordNiIncrementarContador()
        {
            UsuarioFixture fixture = new UsuarioFixture();
            fixture.UsuarioRepo.Usuarios[1] = new Usuario { Id = 1, Username = "admin", PasswordHash = "hash", Activo = true, IntentosFallidos = 5, Bloqueado = true, BloqueadoHasta = null };
            UsuarioBLL sut = fixture.CreateSut();

            ExcepcionNegocio ex = Assert.ThrowsExactly<ExcepcionNegocio>(() => sut.Login(new Usuario { Username = "admin" }, "cualquiera"));

            Assert.AreEqual("Error.User.LoginPermanentlyLocked", ex.Message);
            Assert.IsNull(fixture.PasswordHasher.LastVerifyPassword);
            Assert.AreEqual(0, fixture.UnidadFactory.CrearCallCount);
            Assert.AreEqual("FalloNegocio", fixture.Bitacora.Calls.Single().Kind);
        }

        [TestMethod]
        public void Login_ConBloqueoTemporalVigente_LanzaExcepcionNegocioSinVerificarPasswordNiIncrementarContador()
        {
            UsuarioFixture fixture = new UsuarioFixture();
            fixture.UsuarioRepo.Usuarios[1] = new Usuario { Id = 1, Username = "admin", PasswordHash = "hash", Activo = true, IntentosFallidos = 3, Bloqueado = true, BloqueadoHasta = DateTime.Now.AddMinutes(1) };
            UsuarioBLL sut = fixture.CreateSut();

            ExcepcionNegocio ex = Assert.ThrowsExactly<ExcepcionNegocio>(() => sut.Login(new Usuario { Username = "admin" }, "cualquiera"));

            Assert.AreEqual("Error.User.LoginTemporarilyLocked", ex.Message);
            Assert.IsNull(fixture.PasswordHasher.LastVerifyPassword);
            Assert.AreEqual(0, fixture.UnidadFactory.CrearCallCount);
            Assert.AreEqual("FalloNegocio", fixture.Bitacora.Calls.Single().Kind);
        }

        [TestMethod]
        public void Login_ConBloqueoTemporalVencido_PermiteVerificarPassword()
        {
            UsuarioFixture fixture = new UsuarioFixture();
            fixture.UsuarioRepo.Usuarios[1] = new Usuario { Id = 1, Username = "admin", PasswordHash = "hash", Activo = true, IntentosFallidos = 1, Bloqueado = true, BloqueadoHasta = DateTime.Now.AddSeconds(-1) };
            fixture.PasswordHasher.VerifyResult = false;
            UsuarioBLL sut = fixture.CreateSut();

            Usuario result = sut.Login(new Usuario { Username = "admin" }, "wrong");

            Assert.IsNull(result);
            Assert.AreEqual("wrong", fixture.PasswordHasher.LastVerifyPassword);
            Assert.AreEqual(2, fixture.Unidad.UsuarioRepo.LastEstadoBloqueoUpdate.Item2);
        }

        [TestMethod]
        public void Login_Exitoso_ReseteaContadorYBloqueoAunqueHubieraIntentosFallidosPrevios()
        {
            UsuarioFixture fixture = new UsuarioFixture();
            Usuario user = new Usuario { Id = 1, Username = "admin", PasswordHash = "hash", Activo = true, IntentosFallidos = 3, Bloqueado = true, BloqueadoHasta = DateTime.Now.AddSeconds(-1) };
            fixture.UsuarioRepo.Usuarios[1] = user;
            UsuarioBLL sut = fixture.CreateSut();

            Usuario result = sut.Login(new Usuario { Username = "admin" }, "pwd");

            Assert.AreSame(user, result);
            Assert.IsTrue(fixture.Sesion.HaySesionActiva());
            Tuple<int, int, bool, DateTime?, string> update = fixture.Unidad.UsuarioRepo.LastEstadoBloqueoUpdate;
            Assert.AreEqual(0, update.Item2);
            Assert.IsFalse(update.Item3);
            Assert.IsNull(update.Item4);
            Assert.AreEqual(0, user.IntentosFallidos);
            Assert.IsFalse(user.Bloqueado);
            Assert.IsNull(user.BloqueadoHasta);
        }

        [TestMethod]
        public void DesbloquearUsuario_UsuarioBloqueadoPermanente_LimpiaBloqueoYResetContador()
        {
            UsuarioFixture fixture = new UsuarioFixture();
            fixture.Unidad.UsuarioRepo.Usuarios[2] = new Usuario { Id = 2, Username = "user", DVH = "old-dvh", IntentosFallidos = 5, Bloqueado = true, BloqueadoHasta = null };
            UsuarioBLL sut = fixture.CreateSut();

            bool result = sut.DesbloquearUsuario(new Usuario { Id = 2 });

            Assert.IsTrue(result);
            Tuple<int, int, bool, DateTime?, string> update = fixture.Unidad.UsuarioRepo.LastEstadoBloqueoUpdate;
            Assert.AreEqual(2, update.Item1);
            Assert.AreEqual(0, update.Item2);
            Assert.IsFalse(update.Item3);
            Assert.IsNull(update.Item4);
            Assert.AreEqual("old-dvh", fixture.Integridad.ActualizarDvvCalls.Single().Item2);
            Assert.IsTrue(fixture.Unidad.SaveCalled);
            Assert.AreEqual("OperacionInterno", fixture.Bitacora.Calls.Single().Kind);
        }

        [TestMethod]
        public void DesbloquearUsuario_UsuarioBloqueadoTemporal_LimpiaBloqueoYResetContador()
        {
            UsuarioFixture fixture = new UsuarioFixture();
            fixture.Unidad.UsuarioRepo.Usuarios[2] = new Usuario { Id = 2, Username = "user", DVH = "old-dvh", IntentosFallidos = 4, Bloqueado = true, BloqueadoHasta = DateTime.Now.AddMinutes(5) };
            UsuarioBLL sut = fixture.CreateSut();

            bool result = sut.DesbloquearUsuario(new Usuario { Id = 2 });

            Assert.IsTrue(result);
            Tuple<int, int, bool, DateTime?, string> update = fixture.Unidad.UsuarioRepo.LastEstadoBloqueoUpdate;
            Assert.AreEqual(0, update.Item2);
            Assert.IsFalse(update.Item3);
            Assert.IsNull(update.Item4);
            Assert.IsTrue(fixture.Unidad.SaveCalled);
        }

        [TestMethod]
        public void DesbloquearUsuario_UsuarioNoBloqueado_ThrowsExcepcionNegocio()
        {
            UsuarioFixture fixture = new UsuarioFixture();
            fixture.Unidad.UsuarioRepo.Usuarios[2] = new Usuario { Id = 2, Username = "user", Bloqueado = false };
            UsuarioBLL sut = fixture.CreateSut();

            ExcepcionNegocio ex = Assert.ThrowsExactly<ExcepcionNegocio>(() => sut.DesbloquearUsuario(new Usuario { Id = 2 }));

            Assert.AreEqual("Error.User.NotLocked", ex.Message);
            Assert.IsFalse(fixture.Unidad.SaveCalled);
        }

        [TestMethod]
        public void DesbloquearUsuario_UsuarioInexistente_ThrowsExcepcionNegocio()
        {
            UsuarioFixture fixture = new UsuarioFixture();
            UsuarioBLL sut = fixture.CreateSut();

            ExcepcionNegocio ex = Assert.ThrowsExactly<ExcepcionNegocio>(() => sut.DesbloquearUsuario(new Usuario { Id = 99 }));

            Assert.AreEqual("Error.User.NotFound", ex.Message);
            Assert.IsFalse(fixture.Unidad.SaveCalled);
        }

        [TestMethod]
        public void DesbloquearUsuario_RepoExceptionAlActualizar_WrapsInExcepcionTecnicaAndDoesNotSave()
        {
            UsuarioFixture fixture = new UsuarioFixture();
            fixture.Unidad.UsuarioRepo.Usuarios[2] = new Usuario { Id = 2, Username = "user", Bloqueado = true };
            fixture.Unidad.UsuarioRepo.ThrowOnActualizarEstadoBloqueo = true;
            UsuarioBLL sut = fixture.CreateSut();

            Assert.ThrowsExactly<ExcepcionTecnica>(() => sut.DesbloquearUsuario(new Usuario { Id = 2 }));
            Assert.IsFalse(fixture.Unidad.SaveCalled);
        }

        [TestMethod]
        public void Login_Valido_StartsSessionLoadsPermissionsLanguageAndAudit()
        {
            UsuarioFixture fixture = new UsuarioFixture();
            Usuario user = new Usuario { Id = 1, Username = "admin", PasswordHash = "hash", Activo = true };
            fixture.UsuarioRepo.Usuarios[user.Id] = user;
            fixture.Permiso.PermisosDirectos.Add(new PermisoSimple { Id = 10, Nombre = "usuarios" });
            UsuarioBLL sut = fixture.CreateSut();

            Usuario result = sut.Login(new Usuario { Username = "admin" }, "pwd");

            Assert.AreSame(user, result);
            Assert.AreSame(user, fixture.Sesion.GetUsuarioActual());
            Assert.AreEqual(1, fixture.Autorizacion.ActualizarPermisosCallCount);
            Assert.AreEqual(1, fixture.Idioma.AplicarIdiomaSesionUsuarioId);
            Assert.AreEqual("Operacion", fixture.Bitacora.Calls.Single().Kind);
        }

        [TestMethod]
        public void Registrar_UsernameVacio_ThrowsExcepcionNegocioAndDoesNotCreateUow()
        {
            UsuarioFixture fixture = new UsuarioFixture();
            UsuarioBLL sut = fixture.CreateSut();

            Assert.ThrowsExactly<ExcepcionNegocio>(() => sut.Registrar(new Usuario { Username = " " }, "pwd"));
            Assert.AreEqual(0, fixture.UnidadFactory.CrearCallCount);
            Assert.AreEqual("FalloNegocio", fixture.Bitacora.Calls.Single().Kind);
        }

        [TestMethod]
        public void Registrar_UsernameDuplicado_ThrowsExcepcionNegocioAndDoesNotSave()
        {
            UsuarioFixture fixture = new UsuarioFixture();
            fixture.Unidad.UsuarioRepo.Usuarios[1] = new Usuario { Id = 1, Username = "admin" };
            UsuarioBLL sut = fixture.CreateSut();

            Assert.ThrowsExactly<ExcepcionNegocio>(() => sut.Registrar(new Usuario { Username = "admin" }, "pwd"));
            Assert.IsFalse(fixture.Unidad.SaveCalled);
            Assert.IsTrue(fixture.Unidad.DisposeCalled);
            Assert.AreEqual("FalloNegocioInterno", fixture.Bitacora.Calls.Single().Kind);
        }

        [TestMethod]
        public void Registrar_AltaValida_HashesPersistsAuditsUpdatesDvvAndSaves()
        {
            UsuarioFixture fixture = new UsuarioFixture();
            UsuarioBLL sut = fixture.CreateSut();

            Usuario entrada = new Usuario { Username = " nuevo " };

            Usuario result = sut.Registrar(entrada, " pwd ");

            Assert.AreSame(entrada, result);
            Assert.AreSame(fixture.Unidad.UsuarioRepo.LastInserted, result);
            Assert.IsTrue(result.Id > 0);
            Assert.AreEqual("pwd", fixture.PasswordHasher.LastHashInput);
            Assert.AreEqual("nuevo", fixture.Unidad.UsuarioRepo.LastInserted.Username);
            Assert.AreEqual("HASH:pwd", fixture.Unidad.UsuarioRepo.LastInserted.PasswordHash);
            Assert.AreEqual(fixture.Unidad.UsuarioRepo.LastInserted.Id, fixture.Permiso.AsignarPerfilUsuarioId);
            Assert.AreEqual(fixture.Unidad.UsuarioRepo.LastInserted.Id, fixture.UsuarioEstado.GuardarUsuarioId);
            Assert.AreEqual("OperacionInterno", fixture.Bitacora.Calls.Single().Kind);
            Assert.AreEqual("Usuarios", fixture.Integridad.ActualizarDvvCalls.Single().Item1);
            Assert.IsNull(fixture.Integridad.ActualizarDvvCalls.Single().Item2);
            Assert.AreEqual(fixture.Unidad.UsuarioRepo.LastInserted.DVH, fixture.Integridad.ActualizarDvvCalls.Single().Item3);
            Assert.IsTrue(fixture.Unidad.SaveCalled);
        }

        [TestMethod]
        public void RegistrarParaOnboarding_AltaConfirmada_ContinuacionPropietarioFijoSinSesionNiCredenciales()
        {
            var fixture = new UsuarioFixture();
            fixture.Unidad.UsuarioRepo.CopiarAlLeer = true;
            var entrada = new Usuario { Id = 999, Username = " nuevo " };
            var continuacion = fixture.CreateSut().RegistrarParaOnboarding(entrada, "secreto");
            Assert.IsNotNull(continuacion);
            int idInsertado = fixture.Unidad.UsuarioRepo.LastInserted.Id;
            Assert.AreEqual(idInsertado, continuacion.Usuario.Id);
            Assert.AreNotEqual(999, continuacion.Usuario.Id);
            // El alta guarda la misma instancia recibida: alterarla despues no mueve al propietario.
            entrada.Id = 123;
            Assert.AreEqual(idInsertado, continuacion.Usuario.Id);
            Assert.AreNotSame(entrada, continuacion.Usuario);
            Assert.IsTrue(fixture.Unidad.SaveCalled);
            Assert.IsFalse(fixture.Sesion.HaySesionActiva());
            Assert.IsEmpty(typeof(ContinuacionPerfilInversor).GetConstructors());
            Assert.IsEmpty(typeof(ContinuacionPerfilInversor).GetProperties());
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public void RegistrarParaOnboarding_AltaRechazadaONoConfirmada_NoEntregaContinuacion(bool falloCommit)
        {
            var fixture = new UsuarioFixture();
            fixture.Unidad.ThrowOnSave = falloCommit;
            if (!falloCommit) fixture.Unidad.UsuarioRepo.Usuarios[1] = new Usuario { Id = 1, Username = "nuevo" };
            ContinuacionPerfilInversor resultado = null;
            if (falloCommit)
                Assert.ThrowsExactly<ExcepcionTecnica>(() => resultado = fixture.CreateSut().RegistrarParaOnboarding(new Usuario { Username = "nuevo" }, "pwd"));
            else
                Assert.ThrowsExactly<ExcepcionNegocio>(() => resultado = fixture.CreateSut().RegistrarParaOnboarding(new Usuario { Username = "nuevo" }, "pwd"));
            Assert.IsNull(resultado);
            Assert.IsFalse(fixture.Sesion.HaySesionActiva());
        }

        [TestMethod]
        public void Registrar_RepoException_WrapsInExcepcionTecnicaAndDoesNotSave()
        {
            UsuarioFixture fixture = new UsuarioFixture();
            fixture.Unidad.UsuarioRepo.ThrowOnInsert = true;
            UsuarioBLL sut = fixture.CreateSut();

            Assert.ThrowsExactly<ExcepcionTecnica>(() => sut.Registrar(new Usuario { Username = "nuevo" }, "pwd"));
            Assert.IsFalse(fixture.Unidad.SaveCalled);
        }

        [TestMethod]
        public void Actualizar_UsernameVacio_ThrowsExcepcionNegocio()
        {
            UsuarioFixture fixture = new UsuarioFixture();
            UsuarioBLL sut = fixture.CreateSut();

            Assert.ThrowsExactly<ExcepcionNegocio>(() => sut.Actualizar(new Usuario { Id = 1, Username = " ", Activo = true }, null));
            Assert.AreEqual(0, fixture.UnidadFactory.CrearCallCount);
        }

        [TestMethod]
        public void Actualizar_Valido_UpdatesHistoryDvvSaveAndRefreshesCurrentSessionName()
        {
            UsuarioFixture fixture = new UsuarioFixture();
            fixture.Unidad.UsuarioRepo.Usuarios[1] = new Usuario { Id = 1, Username = "admin", PasswordHash = "old", Activo = true, DVH = "old-dvh" };
            fixture.Sesion.IniciarSesion(new Usuario { Id = 1, Username = "admin" });
            UsuarioBLL sut = fixture.CreateSut();

            bool result = sut.Actualizar(new Usuario { Id = 1, Username = "renamed", Activo = true }, "newpwd");

            Assert.IsTrue(result);
            Assert.AreEqual("HASH:newpwd", fixture.Unidad.UsuarioRepo.LastUpdated.PasswordHash);
            Assert.AreEqual("FormUserHistory.Value.Obs.PasswordUpdate", fixture.UsuarioEstado.GuardarObservaciones);
            Assert.AreEqual("old-dvh", fixture.Integridad.ActualizarDvvCalls.Single().Item2);
            Assert.IsTrue(fixture.Unidad.SaveCalled);
            Assert.AreEqual("renamed", fixture.Sesion.GetUsuarioActualNombre());
            Assert.AreEqual(1, fixture.Sesion.ActualizarNombreCallCount);
        }

        [TestMethod]
        public void Eliminar_UsuarioActual_ThrowsExcepcionNegocioAndDoesNotCreateUow()
        {
            UsuarioFixture fixture = new UsuarioFixture();
            fixture.Sesion.IniciarSesion(new Usuario { Id = 1, Username = "admin" });
            UsuarioBLL sut = fixture.CreateSut();

            Assert.ThrowsExactly<ExcepcionNegocio>(() => sut.Eliminar(new Usuario { Id = 1 }));
            Assert.AreEqual(0, fixture.UnidadFactory.CrearCallCount);
            Assert.AreEqual("FalloNegocio", fixture.Bitacora.Calls.Single().Kind);
        }

        [TestMethod]
        public void Eliminar_Valido_DeletesAuditsUpdatesDvvAndSaves()
        {
            UsuarioFixture fixture = new UsuarioFixture();
            fixture.Unidad.UsuarioRepo.Usuarios[2] = new Usuario { Id = 2, Username = "user", DVH = "old-dvh" };
            UsuarioBLL sut = fixture.CreateSut();

            bool result = sut.Eliminar(new Usuario { Id = 2 });

            Assert.IsTrue(result);
            Assert.AreEqual(2, fixture.Unidad.UsuarioRepo.LastDeletedId);
            Assert.AreEqual("old-dvh", fixture.Integridad.ActualizarDvvCalls.Single().Item2);
            Assert.IsNull(fixture.Integridad.ActualizarDvvCalls.Single().Item3);
            Assert.IsTrue(fixture.Unidad.SaveCalled);
        }

        [TestMethod]
        public void Eliminar_Valido_ValidaCoberturaDePatentesAntesDeEliminar()
        {
            UsuarioFixture fixture = new UsuarioFixture();
            fixture.Unidad.UsuarioRepo.Usuarios[2] = new Usuario { Id = 2, Username = "user", DVH = "old-dvh" };
            UsuarioBLL sut = fixture.CreateSut();

            sut.Eliminar(new Usuario { Id = 2 });

            Assert.AreEqual(2, fixture.Permiso.ValidarPuedeEliminarUsuarioId);
        }

        [TestMethod]
        public void Eliminar_PermisoBLLRechazaPorPatenteHuerfana_ThrowsExcepcionNegocioAndDoesNotDelete()
        {
            UsuarioFixture fixture = new UsuarioFixture();
            fixture.Unidad.UsuarioRepo.Usuarios[2] = new Usuario { Id = 2, Username = "user", DVH = "old-dvh" };
            fixture.Permiso.ThrowOnValidarPuedeEliminarUsuario = true;
            UsuarioBLL sut = fixture.CreateSut();

            Assert.ThrowsExactly<ExcepcionNegocio>(() => sut.Eliminar(new Usuario { Id = 2 }));
            Assert.IsNull(fixture.Unidad.UsuarioRepo.LastDeletedId);
            Assert.IsFalse(fixture.Unidad.SaveCalled);
        }

        [TestMethod]
        public void RestaurarVersion_HistorialInexistente_ThrowsExcepcionNegocio()
        {
            UsuarioFixture fixture = new UsuarioFixture();
            UsuarioBLL sut = fixture.CreateSut();

            Assert.ThrowsExactly<ExcepcionNegocio>(() => sut.RestaurarVersion(new UsuarioHistorialSnapshot { HistorialId = 99 }));
        }

        [TestMethod]
        public void RestaurarVersion_Valida_AppliesSnapshotUpdatesDvvAndSaves()
        {
            UsuarioFixture fixture = new UsuarioFixture();
            fixture.UsuarioRepo.Usuarios[1] = new Usuario { Id = 1, Username = "restored", Activo = true };
            fixture.Unidad.UsuarioRepo.Usuarios[1] = new Usuario { Id = 1, Username = "current", PasswordHash = "old", Activo = true, DVH = "old-dvh" };
            fixture.UsuarioEstado.Versiones[5] = new UsuarioHistorialSnapshot { HistorialId = 5, Usuario = new Usuario { Id = 1 }, Username = "restored", PasswordHash = "new", Activo = true };
            UsuarioBLL sut = fixture.CreateSut();

            bool result = sut.RestaurarVersion(new UsuarioHistorialSnapshot { HistorialId = 5, Username = "alterado" });

            Assert.IsTrue(result);
            Assert.AreEqual(1, fixture.UsuarioEstado.AplicarSnapshotUsuarioId);
            Assert.AreEqual("old-dvh", fixture.Integridad.ActualizarDvvCalls.Single().Item2);
            Assert.IsTrue(fixture.Unidad.SaveCalled);
        }

        [TestMethod]
        public void Actualizar_UsuarioNulo_ThrowsExcepcionNegocioAndDoesNotCreateUow()
        {
            UsuarioFixture fixture = new UsuarioFixture();
            UsuarioBLL sut = fixture.CreateSut();

            ExcepcionNegocio ex = Assert.ThrowsExactly<ExcepcionNegocio>(() => sut.Actualizar(null, "pwd"));

            Assert.AreEqual("Error.User.NotFound", ex.Message);
            Assert.AreEqual(0, fixture.UnidadFactory.CrearCallCount);
        }

        [TestMethod]
        public void Actualizar_UsuarioRecibidoAlterado_CopiaSoloCamposEditablesSobreElPersistido()
        {
            UsuarioFixture fixture = new UsuarioFixture();
            Usuario persistido = new Usuario { Id = 1, Username = "admin", PasswordHash = "old", Activo = true, DVH = "old-dvh", IntentosFallidos = 2 };
            fixture.Unidad.UsuarioRepo.Usuarios[1] = persistido;
            UsuarioBLL sut = fixture.CreateSut();

            sut.Actualizar(new Usuario { Id = 1, Username = "renamed", Activo = true, PasswordHash = "alterado", IntentosFallidos = 0, Bloqueado = true }, null);

            Assert.AreSame(persistido, fixture.Unidad.UsuarioRepo.LastUpdated);
            Assert.AreEqual("renamed", persistido.Username);
            Assert.AreEqual("old", persistido.PasswordHash);
            Assert.AreEqual(2, persistido.IntentosFallidos);
            Assert.IsFalse(persistido.Bloqueado);
            Assert.AreSame(persistido, fixture.UsuarioEstado.GuardarUsuario);
        }

        [TestMethod]
        public void Eliminar_UsuarioNulo_ThrowsExcepcionNegocioAndDoesNotCreateUow()
        {
            UsuarioFixture fixture = new UsuarioFixture();
            UsuarioBLL sut = fixture.CreateSut();

            ExcepcionNegocio ex = Assert.ThrowsExactly<ExcepcionNegocio>(() => sut.Eliminar(null));

            Assert.AreEqual("Error.User.NotFound", ex.Message);
            Assert.AreEqual(0, fixture.UnidadFactory.CrearCallCount);
        }

        [TestMethod]
        public void Eliminar_UsuarioRecibidoAlterado_EliminaElPersistidoConSuDvh()
        {
            UsuarioFixture fixture = new UsuarioFixture();
            fixture.Unidad.UsuarioRepo.Usuarios[2] = new Usuario { Id = 2, Username = "user", DVH = "old-dvh" };
            UsuarioBLL sut = fixture.CreateSut();

            sut.Eliminar(new Usuario { Id = 2, Username = "alterado", DVH = "alterado" });

            Assert.AreEqual(2, fixture.Unidad.UsuarioRepo.LastDeletedId);
            Assert.AreEqual("old-dvh", fixture.Integridad.ActualizarDvvCalls.Single().Item2);
        }

        [TestMethod]
        public void DesbloquearUsuario_UsuarioNulo_ThrowsExcepcionNegocioAndDoesNotCreateUow()
        {
            UsuarioFixture fixture = new UsuarioFixture();
            UsuarioBLL sut = fixture.CreateSut();

            ExcepcionNegocio ex = Assert.ThrowsExactly<ExcepcionNegocio>(() => sut.DesbloquearUsuario(null));

            Assert.AreEqual("Error.User.NotFound", ex.Message);
            Assert.AreEqual(0, fixture.UnidadFactory.CrearCallCount);
        }

        [TestMethod]
        public void DesbloquearUsuario_UsuarioRecibidoBloqueadoPeroPersistidoNo_ValidaSobreElPersistido()
        {
            UsuarioFixture fixture = new UsuarioFixture();
            fixture.Unidad.UsuarioRepo.Usuarios[2] = new Usuario { Id = 2, Username = "user", Bloqueado = false };
            UsuarioBLL sut = fixture.CreateSut();

            ExcepcionNegocio ex = Assert.ThrowsExactly<ExcepcionNegocio>(() => sut.DesbloquearUsuario(new Usuario { Id = 2, Bloqueado = true }));

            Assert.AreEqual("Error.User.NotLocked", ex.Message);
            Assert.IsFalse(fixture.Unidad.SaveCalled);
        }

        private sealed class UsuarioFixture
        {
            public readonly FakeUsuarioRepositorio UsuarioRepo = new FakeUsuarioRepositorio();
            public readonly FakePasswordHashService PasswordHasher = new FakePasswordHashService();
            public readonly SpyIdiomaBLL Idioma = new SpyIdiomaBLL();
            public readonly SpyPermisoBLL Permiso = new SpyPermisoBLL();
            public readonly SpyUsuarioEstadoBLL UsuarioEstado = new SpyUsuarioEstadoBLL();
            public readonly SpyIntegridadBLL Integridad = new SpyIntegridadBLL();
            public readonly SpyBitacoraBLL Bitacora = new SpyBitacoraBLL();
            public readonly FakeUnidadDeTrabajo Unidad = new FakeUnidadDeTrabajo();
            public readonly FakeSesion Sesion = new FakeSesion();
            public readonly FakeAutorizacion Autorizacion = new FakeAutorizacion();
            public readonly FakeUnidadDeTrabajoFactory UnidadFactory;

            public UsuarioFixture()
            {
                UnidadFactory = new FakeUnidadDeTrabajoFactory(Unidad);
            }

            public UsuarioBLL CreateSut()
            {
                return new UsuarioBLL(UsuarioRepo, PasswordHasher, Idioma, Permiso, UsuarioEstado, Integridad, Bitacora, UnidadFactory, Sesion, Autorizacion);
            }
        }
    }
}
