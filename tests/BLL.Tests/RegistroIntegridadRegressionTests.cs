using Microsoft.VisualStudio.TestTools.UnitTesting;
using BE;
using BLL.Tests.TestDoubles;
using SERVICIOS;

namespace BLL.Tests
{
    // Regresion T07: el DVH de Usuario incluye el Id (esta marcado con
    // [DigitoVerificador]). Si Registrar calcula el DVH antes de conocer el Id
    // real (con Id=0), la firma persistida no coincide con la que recalcula
    // ValidarIntegridadGlobal usando el Id definitivo, disparando un falso
    // positivo de corrupcion horizontal en el siguiente arranque.
    //
    // A diferencia de UsuarioBLLTests, este test usa el IntegridadService real
    // (que depende del Id via reflexion) en lugar del spy, para que la firma
    // sea sensible al Id y la regresion quede cubierta.
    [TestClass]
    public sealed class RegistroIntegridadRegressionTests
    {
        [TestMethod]
        public void Registrar_LuegoValidarIntegridadGlobal_NoDisparaFalsoPositivoHorizontal()
        {
            RegistroIntegridadFixture fixture = new RegistroIntegridadFixture();

            Usuario registrado = fixture.Usuario.Registrar(new Usuario { Username = "nuevo" }, "pwd");

            Assert.IsNotNull(registrado);

            // El DVH persistido debe corresponder al Id real asignado en el alta,
            // no a Id=0.
            Usuario persistido = fixture.Unidad.UsuarioRepo.LastInserted;
            Assert.IsTrue(persistido.Id > 0);
            Assert.AreEqual(fixture.IntegridadService.CalcularDVH(persistido), persistido.DVH);

            // Con el fix, revalidar la tabla no debe lanzar ExcepcionIntegridad.
            fixture.Integridad.ValidarIntegridadGlobal();
        }

        private sealed class RegistroIntegridadFixture
        {
            public readonly IntegridadService IntegridadService = new IntegridadService();
            public readonly FakePasswordHashService PasswordHasher = new FakePasswordHashService();
            public readonly SpyIdiomaBLL Idioma = new SpyIdiomaBLL();
            public readonly SpyPermisoBLL Permiso = new SpyPermisoBLL();
            public readonly SpyUsuarioEstadoBLL UsuarioEstado = new SpyUsuarioEstadoBLL();
            public readonly SpyBitacoraBLL Bitacora = new SpyBitacoraBLL();
            public readonly FakeUnidadDeTrabajo Unidad = new FakeUnidadDeTrabajo();
            public readonly FakeSesion Sesion = new FakeSesion();
            public readonly FakeAutorizacion Autorizacion = new FakeAutorizacion();
            public readonly FakeUnidadDeTrabajoFactory UnidadFactory;
            public readonly IntegridadBLL Integridad;
            public readonly UsuarioBLL Usuario;

            public RegistroIntegridadFixture()
            {
                UnidadFactory = new FakeUnidadDeTrabajoFactory(Unidad);
                Integridad = new IntegridadBLL(IntegridadService, Unidad.IntegridadRepo, Unidad.UsuarioRepo, Bitacora, UnidadFactory);
                Usuario = new UsuarioBLL(Unidad.UsuarioRepo, PasswordHasher, Idioma, Permiso, UsuarioEstado, Integridad, Bitacora, UnidadFactory, Sesion, Autorizacion);
            }
        }
    }
}
