using ABSTRACCIONES;
using BE;
using BLL.Tests.TestDoubles;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SERVICIOS;

namespace BLL.Tests
{
    [TestClass]
    public sealed class SesionUsuarioExtensionesTests
    {
        [TestMethod]
        public void ObtenerUsuarioActual_ConSesion_DevuelveElMismoUsuario()
        {
            Usuario usuario = new Usuario { Id = 7, Username = "ana" };
            FakeSesion sesion = new FakeSesion();
            sesion.IniciarSesion(usuario);

            Assert.AreSame(usuario, sesion.ObtenerUsuarioActual());
        }

        [TestMethod]
        public void ObtenerUsuarioActual_SinSesion_LanzaExcepcionNegocio()
        {
            FakeSesion sesion = new FakeSesion();

            ExcepcionNegocio ex = Assert.ThrowsExactly<ExcepcionNegocio>(() => sesion.ObtenerUsuarioActual());

            Assert.AreEqual("FormUserSettings.Error.NoActiveSession", ex.Message);
        }

        [TestMethod]
        public void ObtenerUsuarioActual_SesionConOtroTipo_LanzaExcepcionNegocio()
        {
            FakeSesion sesion = new FakeSesion();
            sesion.IniciarSesion(new UsuarioNoEntidad { Id = 7, Username = "ana" });

            Assert.ThrowsExactly<ExcepcionNegocio>(() => sesion.ObtenerUsuarioActual());
        }

        private sealed class UsuarioNoEntidad : IUsuarioAutenticado
        {
            public int Id { get; set; }

            public string Username { get; set; }
        }
    }
}
