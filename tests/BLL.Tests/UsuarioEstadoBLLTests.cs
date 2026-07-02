using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BE;
using BLL.Tests.TestDoubles;
using SERVICIOS;

namespace BLL.Tests
{
    [TestClass]
    public sealed class UsuarioEstadoBLLTests
    {
        [TestMethod]
        public void ConstruirSnapshotActualInterno_CurrentUserDataPermissionsPreferenceAndActor_ReturnsSnapshot()
        {
            UsuarioEstadoFixture fixture = new UsuarioEstadoFixture();
            fixture.Unidad.UsuarioRepo.Usuarios[1] = new Usuario { Id = 1, Username = "admin", PasswordHash = "hash", Activo = true };
            fixture.Unidad.UsuarioPermisoRepo.PermisosPorUsuario[1] = new List<int> { 2, 3 };
            fixture.Unidad.UsuarioPreferenciaRepo.Preferencias[1] = new UsuarioPreferencia { UsuarioId = 1, IdiomaId = 5 };
            fixture.Sesion.IniciarSesion(new Usuario { Id = 9, Username = "actor" });
            UsuarioEstadoBLL sut = fixture.CreateSut();

            UsuarioHistorialSnapshot snapshot = sut.ConstruirSnapshotActualInterno(1, "accion", "obs", fixture.Unidad);

            Assert.AreEqual(1, snapshot.UsuarioId);
            Assert.AreEqual("admin", snapshot.Username);
            Assert.AreEqual("hash", snapshot.PasswordHash);
            Assert.AreEqual(5, snapshot.IdiomaId);
            CollectionAssert.AreEqual(new List<int> { 2, 3 }, snapshot.PermisosIds.ToList());
            Assert.AreEqual(9, snapshot.ActorUsuarioId);
            Assert.AreEqual("actor", snapshot.ActorUsernameSnapshot);
        }

        [TestMethod]
        public void GuardarVersionActualInterno_UsesUnidadUsuarioHistorial()
        {
            UsuarioEstadoFixture fixture = new UsuarioEstadoFixture();
            fixture.Unidad.UsuarioRepo.Usuarios[1] = new Usuario { Id = 1, Username = "admin", PasswordHash = "hash", Activo = true };
            UsuarioEstadoBLL sut = fixture.CreateSut();

            int historialId = sut.GuardarVersionActualInterno(1, "accion", "obs", fixture.Unidad);

            Assert.AreEqual(historialId, fixture.Unidad.UsuarioHistorialRepo.LastSaved.HistorialId);
            Assert.AreEqual("accion", fixture.Unidad.UsuarioHistorialRepo.LastSaved.Accion);
        }

        [TestMethod]
        public void CompararVersiones_DetectsRelevantChanges()
        {
            UsuarioEstadoFixture fixture = new UsuarioEstadoFixture();
            UsuarioEstadoBLL sut = fixture.CreateSut();
            UsuarioHistorialSnapshot previous = new UsuarioHistorialSnapshot
            {
                Username = "admin",
                PasswordHash = "old",
                Activo = true,
                IdiomaId = 1,
                PermisosIds = new List<int> { 1 }
            };
            UsuarioHistorialSnapshot current = new UsuarioHistorialSnapshot
            {
                Username = "renamed",
                PasswordHash = "new",
                Activo = false,
                IdiomaId = 2,
                PermisosIds = new List<int> { 1, 2 }
            };

            List<ResultadoComparacionEstado> differences = sut.CompararVersiones(current, previous);

            Assert.IsTrue(differences.Any(item => item.Campo == "FormUserHistory.Value.Field.Username"));
            Assert.IsTrue(differences.Any(item => item.Campo == "FormUserHistory.Value.Field.PasswordHash"));
            Assert.IsTrue(differences.Any(item => item.Campo == "FormUserHistory.Value.Field.Active"));
            Assert.IsTrue(differences.Any(item => item.Campo == "FormUserHistory.Value.Field.LanguageId"));
            Assert.IsTrue(differences.Any(item => item.Campo == "FormUserHistory.Value.Field.Permissions"));
        }

        [TestMethod]
        public void AplicarSnapshotInterno_UpdatesUserPermissionsAndPreferenceInsideSameUow()
        {
            UsuarioEstadoFixture fixture = new UsuarioEstadoFixture();
            UsuarioEstadoBLL sut = fixture.CreateSut();
            UsuarioHistorialSnapshot snapshot = new UsuarioHistorialSnapshot
            {
                UsuarioId = 1,
                Username = "restored",
                PasswordHash = "hash",
                Activo = true,
                IdiomaId = 3,
                PermisosIds = new List<int> { 5, 6 }
            };

            sut.AplicarSnapshotInterno(snapshot, fixture.Unidad);

            Assert.AreEqual("restored", fixture.Unidad.UsuarioRepo.Usuarios[1].Username);
            CollectionAssert.AreEqual(new List<int> { 5, 6 }, fixture.Unidad.UsuarioPermisoRepo.LastPermisos);
            Assert.AreEqual(3, fixture.Unidad.UsuarioPreferenciaRepo.LastIdiomaId);
        }

        private sealed class UsuarioEstadoFixture
        {
            public readonly FakeUsuarioRepositorio UsuarioRepo = new FakeUsuarioRepositorio();
            public readonly FakeUsuarioPermisoRepositorio UsuarioPermisoRepo = new FakeUsuarioPermisoRepositorio();
            public readonly FakeUsuarioPreferenciaRepositorio UsuarioPreferenciaRepo = new FakeUsuarioPreferenciaRepositorio();
            public readonly FakeUnidadDeTrabajo Unidad = new FakeUnidadDeTrabajo();
            public readonly FakeSesion Sesion = new FakeSesion();
            public readonly FakeUnidadDeTrabajoFactory UnidadFactory;

            public UsuarioEstadoFixture()
            {
                UnidadFactory = new FakeUnidadDeTrabajoFactory(Unidad);
            }

            public UsuarioEstadoBLL CreateSut()
            {
                return new UsuarioEstadoBLL(UsuarioRepo, UsuarioPermisoRepo, UsuarioPreferenciaRepo, UnidadFactory, Sesion);
            }
        }
    }
}
