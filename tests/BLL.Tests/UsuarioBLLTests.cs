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
            Assert.AreEqual("FalloNegocio", fixture.Bitacora.Calls.Single().Kind);
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

            bool result = sut.Registrar(new Usuario { Username = " nuevo " }, " pwd ");

            Assert.IsTrue(result);
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

            Assert.ThrowsExactly<ExcepcionNegocio>(() => sut.Actualizar(1, " ", null, true));
            Assert.AreEqual(0, fixture.UnidadFactory.CrearCallCount);
        }

        [TestMethod]
        public void Actualizar_Valido_UpdatesHistoryDvvSaveAndRefreshesCurrentSessionName()
        {
            UsuarioFixture fixture = new UsuarioFixture();
            fixture.Unidad.UsuarioRepo.Usuarios[1] = new Usuario { Id = 1, Username = "admin", PasswordHash = "old", Activo = true, DVH = "old-dvh" };
            fixture.Sesion.IniciarSesion(new Usuario { Id = 1, Username = "admin" });
            UsuarioBLL sut = fixture.CreateSut();

            bool result = sut.Actualizar(1, "renamed", "newpwd", true);

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

            Assert.ThrowsExactly<ExcepcionNegocio>(() => sut.Eliminar(1));
            Assert.AreEqual(0, fixture.UnidadFactory.CrearCallCount);
            Assert.AreEqual("FalloNegocio", fixture.Bitacora.Calls.Single().Kind);
        }

        [TestMethod]
        public void Eliminar_Valido_DeletesAuditsUpdatesDvvAndSaves()
        {
            UsuarioFixture fixture = new UsuarioFixture();
            fixture.Unidad.UsuarioRepo.Usuarios[2] = new Usuario { Id = 2, Username = "user", DVH = "old-dvh" };
            UsuarioBLL sut = fixture.CreateSut();

            bool result = sut.Eliminar(2);

            Assert.IsTrue(result);
            Assert.AreEqual(2, fixture.Unidad.UsuarioRepo.LastDeletedId);
            Assert.AreEqual("old-dvh", fixture.Integridad.ActualizarDvvCalls.Single().Item2);
            Assert.IsNull(fixture.Integridad.ActualizarDvvCalls.Single().Item3);
            Assert.IsTrue(fixture.Unidad.SaveCalled);
        }

        [TestMethod]
        public void RestaurarVersion_HistorialInexistente_ThrowsExcepcionNegocio()
        {
            UsuarioFixture fixture = new UsuarioFixture();
            UsuarioBLL sut = fixture.CreateSut();

            Assert.ThrowsExactly<ExcepcionNegocio>(() => sut.RestaurarVersion(99));
        }

        [TestMethod]
        public void RestaurarVersion_Valida_AppliesSnapshotUpdatesDvvAndSaves()
        {
            UsuarioFixture fixture = new UsuarioFixture();
            fixture.UsuarioRepo.Usuarios[1] = new Usuario { Id = 1, Username = "restored", Activo = true };
            fixture.Unidad.UsuarioRepo.Usuarios[1] = new Usuario { Id = 1, Username = "current", PasswordHash = "old", Activo = true, DVH = "old-dvh" };
            fixture.UsuarioEstado.Versiones[5] = new UsuarioHistorialSnapshot { HistorialId = 5, UsuarioId = 1, Username = "restored", PasswordHash = "new", Activo = true };
            UsuarioBLL sut = fixture.CreateSut();

            bool result = sut.RestaurarVersion(5);

            Assert.IsTrue(result);
            Assert.AreEqual(1, fixture.UsuarioEstado.AplicarSnapshotUsuarioId);
            Assert.AreEqual("old-dvh", fixture.Integridad.ActualizarDvvCalls.Single().Item2);
            Assert.IsTrue(fixture.Unidad.SaveCalled);
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
