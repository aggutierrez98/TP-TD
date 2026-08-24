using Microsoft.VisualStudio.TestTools.UnitTesting;
using BLL;

namespace TpIDS.IntegrationTests
{
    [TestClass]
    [TestCategory("Integration")]
    public sealed class CompositionRootIntegrationTests
    {
        [TestCleanup]
        public void Cleanup()
        {
            CompositionRoot.Sesion.CerrarSesion();
            CompositionRoot.Autorizacion.Limpiar();
        }

        [TestMethod]
        public void Inicializar_DoesNotThrow()
        {
            CompositionRoot.Inicializar();
        }

        [TestMethod]
        public void Getters_ReturnSameSharedInstances()
        {
            CompositionRoot.Inicializar();

            Assert.AreSame(CompositionRoot.Sesion, CompositionRoot.Sesion);
            Assert.AreSame(CompositionRoot.Autorizacion, CompositionRoot.Autorizacion);
            Assert.AreSame(CompositionRoot.Usuario, CompositionRoot.Usuario);
            Assert.AreSame(CompositionRoot.Permiso, CompositionRoot.Permiso);
        }

        [TestMethod]
        public void UsuarioPermisoIdiomaIntegridad_AreComposed()
        {
            CompositionRoot.Inicializar();

            Assert.IsNotNull(CompositionRoot.Usuario);
            Assert.IsNotNull(CompositionRoot.Permiso);
            Assert.IsNotNull(CompositionRoot.Idioma);
            Assert.IsNotNull(CompositionRoot.Integridad);
            Assert.IsNotNull(CompositionRoot.GestorBitacora);
        }
    }
}
