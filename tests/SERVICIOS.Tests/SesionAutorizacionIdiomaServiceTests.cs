using Microsoft.VisualStudio.TestTools.UnitTesting;
using ABSTRACCIONES;
using SERVICIOS;

namespace SERVICIOS.Tests
{
    [TestClass]
    public sealed class SesionAutorizacionIdiomaServiceTests
    {
        private TestObserver _observer;

        [TestCleanup]
        public void Cleanup()
        {
            Sesion.GetInstance().CerrarSesion();
            AutorizacionService.GetInstance().Limpiar();

            if (_observer != null)
            {
                IdiomaService.GetInstance().DesregistrarObservador(_observer);
                _observer = null;
            }
        }

        [TestMethod]
        public void Sesion_IniciarSesion_SetsCurrentUser()
        {
            Sesion.GetInstance().IniciarSesion(5, "admin");

            Assert.IsTrue(Sesion.GetInstance().HaySesionActiva());
            Assert.AreEqual(5, Sesion.GetInstance().GetUsuarioActualId());
            Assert.AreEqual("admin", Sesion.GetInstance().GetUsuarioActualNombre());
        }

        [TestMethod]
        public void Sesion_ActualizarUsuarioActualNombre_ChangesCurrentName()
        {
            Sesion.GetInstance().IniciarSesion(5, "admin");

            Sesion.GetInstance().ActualizarUsuarioActualNombre("renamed");

            Assert.AreEqual("renamed", Sesion.GetInstance().GetUsuarioActualNombre());
        }

        [TestMethod]
        public void Sesion_CerrarSesion_ClearsCurrentUser()
        {
            Sesion.GetInstance().IniciarSesion(5, "admin");

            Sesion.GetInstance().CerrarSesion();

            Assert.IsFalse(Sesion.GetInstance().HaySesionActiva());
            Assert.IsNull(Sesion.GetInstance().GetUsuarioActual());
        }

        [TestMethod]
        public void Autorizacion_TienePermiso_DirectPermission_ReturnsTrue()
        {
            AutorizacionService.GetInstance().ActualizarPermisosUsuarioActual(new[] { Simple(1, "usuarios.ver") });

            Assert.IsTrue(AutorizacionService.GetInstance().TienePermiso("usuarios.ver"));
        }

        [TestMethod]
        public void Autorizacion_TienePermiso_NestedPermission_ReturnsTrue()
        {
            PermisoGrupo group = new PermisoGrupo { Id = 1, Nombre = "admin" };
            group.Agregar(Simple(2, "usuarios.ver"));
            AutorizacionService.GetInstance().ActualizarPermisosUsuarioActual(new[] { group });

            Assert.IsTrue(AutorizacionService.GetInstance().TienePermiso("usuarios.ver"));
        }

        [TestMethod]
        public void Autorizacion_TienePermiso_MissingPermission_ReturnsFalse()
        {
            AutorizacionService.GetInstance().ActualizarPermisosUsuarioActual(new[] { Simple(1, "usuarios.ver") });

            Assert.IsFalse(AutorizacionService.GetInstance().TienePermiso("usuarios.editar"));
        }

        [TestMethod]
        public void Autorizacion_Limpiar_RemovesPermissions()
        {
            AutorizacionService.GetInstance().ActualizarPermisosUsuarioActual(new[] { Simple(1, "usuarios.ver") });

            AutorizacionService.GetInstance().Limpiar();

            Assert.IsFalse(AutorizacionService.GetInstance().TienePermiso("usuarios.ver"));
        }

        [TestMethod]
        public void IdiomaService_CambiarIdioma_NotifiesObservers()
        {
            _observer = new TestObserver();
            IdiomaService.GetInstance().RegistrarObservador(_observer);
            int before = _observer.UpdateCount;

            IdiomaService.GetInstance().CambiarIdioma(new Idioma { Id = 1, Nombre = "Espanol" });

            Assert.AreEqual(before + 1, _observer.UpdateCount);
        }

        [TestMethod]
        public void IdiomaService_DesregistrarObservador_StopsNotifications()
        {
            _observer = new TestObserver();
            IdiomaService.GetInstance().RegistrarObservador(_observer);
            IdiomaService.GetInstance().DesregistrarObservador(_observer);
            int before = _observer.UpdateCount;

            IdiomaService.GetInstance().CambiarIdioma(new Idioma { Id = 2, Nombre = "English" });

            Assert.AreEqual(before, _observer.UpdateCount);
        }

        private static PermisoSimple Simple(int id, string name)
        {
            return new PermisoSimple { Id = id, Nombre = name };
        }

        private sealed class TestObserver : IObservador
        {
            public int UpdateCount { get; private set; }

            public void Actualizar(IObservado observado)
            {
                UpdateCount++;
            }
        }
    }
}
