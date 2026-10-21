using System;
using System.Linq;
using System.Threading.Tasks;
using BE;
using BLL.Tests.TestDoubles;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using REPOSITORIO;
using SERVICIOS;

namespace BLL.Tests
{
    [TestClass]
    public sealed class PerfilInversorBLLTests
    {
        [TestMethod]
        public void GuardarPostRegistro_PropietarioFijoSinSesion_ConsumeSoloTrasCommitYPermiteReintento()
        {
            var f = new Fixture();
            f.Sesion.CerrarSesion();
            var sut = f.Crear();
            var continuacion = new ContinuacionPerfilInversor(new Usuario { Id = 19 }, f.Fecha);
            f.Unidad.ThrowOnSave = true;
            Assert.ThrowsExactly<ExcepcionTecnica>(() => sut.GuardarPostRegistro(continuacion, 1, CalculadorPerfilInversorTests.Respuestas("CDCDCCDDDC")));
            Assert.IsFalse(continuacion.Finalizada);
            f.Unidad.ThrowOnSave = false;
            var perfil = sut.GuardarPostRegistro(continuacion, 1, CalculadorPerfilInversorTests.Respuestas("CDCDCCDDDC"));
            Assert.AreEqual(19, perfil.Usuario.Id);
            Assert.AreSame(continuacion.Usuario, perfil.Usuario);
            Assert.IsTrue(continuacion.Finalizada);
            Assert.ThrowsExactly<ExcepcionNegocio>(() => sut.GuardarPostRegistro(continuacion, 1, CalculadorPerfilInversorTests.Respuestas("CDCDCCDDDC")));
            Assert.IsFalse(f.Sesion.HaySesionActiva());
        }

        [TestMethod]
        [DataRow(29, true)]
        [DataRow(30, false)]
        [DataRow(31, false)]
        public void GuardarPostRegistro_VencimientoTreintaMinutos_NoAceptaContinuacionVencida(int minutos, bool valida)
        {
            var f = new Fixture();
            var continuacion = new ContinuacionPerfilInversor(new Usuario { Id = 19 }, f.Fecha);
            f.Fecha = f.Fecha.AddMinutes(minutos);
            if (valida) Assert.IsNotNull(f.Crear().GuardarPostRegistro(continuacion, 1, CalculadorPerfilInversorTests.Respuestas("CDCDCCDDDC")));
            else
            {
                Assert.ThrowsExactly<ExcepcionNegocio>(() => f.Crear().GuardarPostRegistro(continuacion, 1, CalculadorPerfilInversorTests.Respuestas("CDCDCCDDDC")));
                Assert.AreEqual(0, f.Factory.CrearCallCount);
            }
        }

        [TestMethod]
        public void GuardarPostRegistro_CanceladaNulaORespuestasInvalidas_NoGuarda()
        {
            var f = new Fixture();
            var sut = f.Crear();
            var continuacion = new ContinuacionPerfilInversor(new Usuario { Id = 19 }, f.Fecha);
            Assert.ThrowsExactly<ExcepcionNegocio>(() => sut.GuardarPostRegistro(continuacion, 2, CalculadorPerfilInversorTests.Respuestas("CDCDCCDDDC")));
            Assert.IsFalse(continuacion.Finalizada);
            sut.CancelarPostRegistro(continuacion);
            sut.CancelarPostRegistro(continuacion);
            Assert.ThrowsExactly<ExcepcionNegocio>(() => sut.GuardarPostRegistro(continuacion, 1, CalculadorPerfilInversorTests.Respuestas("CDCDCCDDDC")));
            Assert.ThrowsExactly<ExcepcionNegocio>(() => sut.GuardarPostRegistro(null, 1, CalculadorPerfilInversorTests.Respuestas("CDCDCCDDDC")));
            Assert.AreEqual(0, f.Factory.CrearCallCount);
        }

        [TestMethod]
        public async Task GuardarPostRegistro_ConsumoConcurrente_SoloUnCommit()
        {
            var f = new Fixture();
            var sut = f.Crear();
            var continuacion = new ContinuacionPerfilInversor(new Usuario { Id = 19 }, f.Fecha);
            var intentos = Enumerable.Range(0, 8).Select(_ => Task.Run(() =>
            {
                try { sut.GuardarPostRegistro(continuacion, 1, CalculadorPerfilInversorTests.Respuestas("CDCDCCDDDC")); return true; }
                catch (ExcepcionNegocio) { return false; }
            }));
            Assert.AreEqual(1, (await Task.WhenAll(intentos)).Count(ok => ok));
            Assert.AreEqual(1, f.Factory.CrearCallCount);
        }

        [TestMethod]
        public void GuardarMiPerfil_RecalculaDesdeRespuestasYCapturaSesionAuditoriaYFecha()
        {
            var f = new Fixture();
            var sut = f.Crear();
            var respuestas = CalculadorPerfilInversorTests.Respuestas("CDCDCCDDDC");
            var guardado = sut.GuardarMiPerfil(1, respuestas);
            Assert.AreEqual(7, guardado.Usuario.Id);
            Assert.AreSame(f.Sesion.GetUsuarioActual(), guardado.Usuario);
            Assert.AreEqual(100m, guardado.Puntaje);
            Assert.AreEqual(NivelRiesgoInversor.Alto, guardado.NivelRiesgo);
            Assert.AreEqual(f.Fecha, guardado.FechaEvaluacionUtc);
            Assert.AreSame(guardado, f.Repo.Ultimo);
            Assert.IsTrue(f.Unidad.SaveCalled);
            var auditoria = Assert.ContainsSingle(f.Bitacora.Calls);
            Assert.AreSame(f.Unidad, auditoria.Unidad);
            Assert.AreEqual("FormInvestorProfile.Value.Info.Saved", auditoria.Informacion);
            respuestas[0].OpcionId = "A";
            Assert.AreEqual("C", guardado.Respuestas[0].OpcionId);
        }

        [TestMethod]
        public void ObtenerMiPerfil_CambioDeSesion_LeeSoloPropietarioActual()
        {
            var f = new Fixture();
            var sut = f.Crear();
            Assert.IsNull(sut.ObtenerMiPerfil());
            Assert.AreEqual(7, f.Repo.PropietarioConsultado);
            f.Sesion.IniciarSesion(new Usuario { Id = 8 });
            sut.ObtenerMiPerfil();
            Assert.AreEqual(8, f.Repo.PropietarioConsultado);
        }

        [TestMethod]
        public void Operaciones_SinSesion_RechazanSinAccesoARepositorio()
        {
            var f = new Fixture();
            f.Sesion.CerrarSesion();
            var sut = f.Crear();
            Assert.ThrowsExactly<ExcepcionNegocio>(() => sut.ObtenerMiPerfil());
            Assert.ThrowsExactly<ExcepcionNegocio>(() => sut.GuardarMiPerfil(1, CalculadorPerfilInversorTests.Respuestas("CDCDCCDDDC")));
            Assert.AreEqual(0, f.Factory.CrearCallCount);
            Assert.IsNull(f.Repo.PropietarioConsultado);
        }

        [TestMethod]
        public void GuardarMiPerfil_VersionInvalida_NoAbreTransaccionNiAudita()
        {
            var f = new Fixture();
            Assert.ThrowsExactly<ExcepcionNegocio>(() => f.Crear().GuardarMiPerfil(2, CalculadorPerfilInversorTests.Respuestas("CDCDCCDDDC")));
            Assert.AreEqual(0, f.Factory.CrearCallCount);
            Assert.IsEmpty(f.Bitacora.Calls);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public void GuardarMiPerfil_FalloRepositorioOCommit_NoDevuelveExitoYLiberaUnidad(bool commit)
        {
            var f = new Fixture();
            f.Repo.Fallar = !commit;
            f.Unidad.ThrowOnSave = commit;
            var error = Assert.ThrowsExactly<ExcepcionTecnica>(() => f.Crear().GuardarMiPerfil(1, CalculadorPerfilInversorTests.Respuestas("CDCDCCDDDC")));
            Assert.AreEqual("Error.InvestorProfile.SaveFailed", error.Message);
            Assert.IsFalse(f.Unidad.SaveCalled);
            Assert.IsTrue(f.Unidad.DisposeCalled);
        }

        [TestMethod]
        public void ObtenerPreguntas_CuestionarioDeDiezPreguntas_InformaOpcionesDeCadaUna()
        {
            var sut = new Fixture().Crear();
            var preguntas = sut.ObtenerPreguntas();
            CollectionAssert.AreEqual(Enumerable.Range(1, 10).ToArray(), preguntas.Select(p => p.Id).ToArray());
            CollectionAssert.AreEqual(new[] { 3, 4, 3, 4, 3, 3, 4, 4, 4, 3 }, preguntas.Select(sut.ObtenerCantidadOpciones).ToArray());
        }

        [TestMethod]
        public void ObtenerCantidadOpciones_PreguntaNulaOFueraDelCuestionario_Rechaza()
        {
            var sut = new Fixture().Crear();
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => sut.ObtenerCantidadOpciones(null));
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => sut.ObtenerCantidadOpciones(new PreguntaPerfilInversor(0)));
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => sut.ObtenerCantidadOpciones(new PreguntaPerfilInversor(11)));
        }

        [TestMethod]
        public void ObtenerMiPerfil_ErrorLectura_NoConfundeConAusencia()
        {
            var f = new Fixture();
            f.Repo.Fallar = true;
            Assert.AreEqual("Error.InvestorProfile.ReadFailed", Assert.ThrowsExactly<ExcepcionTecnica>(() => f.Crear().ObtenerMiPerfil()).Message);
        }

        internal sealed class Fixture
        {
            internal readonly FakeSesion Sesion = new FakeSesion();
            internal readonly RepoPerfil Repo = new RepoPerfil();
            internal readonly FakeUnidadDeTrabajo Unidad = new FakeUnidadDeTrabajo();
            internal readonly SpyBitacoraBLL Bitacora = new SpyBitacoraBLL();
            internal readonly FakeUnidadDeTrabajoFactory Factory;
            internal DateTime Fecha = new DateTime(2026, 10, 19, 12, 0, 0, DateTimeKind.Utc);
            internal Fixture()
            {
                Sesion.IniciarSesion(new Usuario { Id = 7 });
                Unidad.PerfilesInversor = Repo;
                Factory = new FakeUnidadDeTrabajoFactory(Unidad);
            }
            internal PerfilInversorBLL Crear() => new PerfilInversorBLL(Repo, Factory, Sesion, Bitacora, () => Fecha);
        }

        internal sealed class RepoPerfil : IPerfilInversorRepositorio
        {
            internal PerfilInversor Ultimo;
            internal int? PropietarioConsultado;
            internal bool Fallar;
            public PerfilInversor ObtenerPorUsuario(Usuario usuario)
            {
                if (Fallar) throw new InvalidOperationException("lectura");
                PropietarioConsultado = usuario.Id;
                return Ultimo?.Usuario?.Id == usuario.Id ? Ultimo : null;
            }
            public void Guardar(PerfilInversor perfil)
            {
                if (Fallar) throw new InvalidOperationException("guardado");
                Ultimo = perfil;
            }
        }
    }
}
