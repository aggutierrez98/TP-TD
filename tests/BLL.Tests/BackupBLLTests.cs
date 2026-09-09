using System.IO;
using BLL.Tests.TestDoubles;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SERVICIOS;

namespace BLL.Tests
{
    [TestClass]
    public sealed class BackupBLLTests
    {
        [TestMethod]
        public void CrearBackup_RutaVacia_ThrowsExcepcionNegocioYNoDelegaEnRepositorio()
        {
            BackupFixture fixture = new BackupFixture();
            BackupBLL sut = fixture.CreateSut();

            Assert.ThrowsExactly<ExcepcionNegocio>(() => sut.CrearBackup("   "));
            Assert.AreEqual(0, fixture.Repositorio.CrearBackupCallCount);
            Assert.AreEqual("FalloNegocio", fixture.Bitacora.Calls[0].Kind);
        }

        [TestMethod]
        public void CrearBackup_ExtensionInvalida_ThrowsExcepcionNegocio()
        {
            BackupFixture fixture = new BackupFixture();
            BackupBLL sut = fixture.CreateSut();
            string ruta = Path.Combine(Path.GetTempPath(), "backup-test.txt");

            Assert.ThrowsExactly<ExcepcionNegocio>(() => sut.CrearBackup(ruta));
            Assert.AreEqual(0, fixture.Repositorio.CrearBackupCallCount);
            Assert.AreEqual("FalloNegocio", fixture.Bitacora.Calls[0].Kind);
        }

        [TestMethod]
        public void CrearBackup_DirectorioInexistente_ThrowsExcepcionNegocio()
        {
            BackupFixture fixture = new BackupFixture();
            BackupBLL sut = fixture.CreateSut();
            string ruta = Path.Combine(Path.GetTempPath(), "carpeta-inexistente-uitest", "backup.bak");

            Assert.ThrowsExactly<ExcepcionNegocio>(() => sut.CrearBackup(ruta));
            Assert.AreEqual(0, fixture.Repositorio.CrearBackupCallCount);
            Assert.AreEqual("FalloNegocio", fixture.Bitacora.Calls[0].Kind);
        }

        [TestMethod]
        public void CrearBackup_RutaValida_DelegaEnRepositorioYRegistraOperacion()
        {
            BackupFixture fixture = new BackupFixture();
            BackupBLL sut = fixture.CreateSut();
            string ruta = Path.Combine(Path.GetTempPath(), "backup-valido.bak");

            sut.CrearBackup(ruta);

            Assert.AreEqual(1, fixture.Repositorio.CrearBackupCallCount);
            Assert.AreEqual(ruta, fixture.Repositorio.UltimaRutaCreada);
            Assert.AreEqual("Operacion", fixture.Bitacora.Calls[0].Kind);
        }

        [TestMethod]
        public void CrearBackup_RepositorioLanzaExcepcionTecnica_SePropagaSinSerTragada()
        {
            BackupFixture fixture = new BackupFixture();
            fixture.Repositorio.ThrowOnCrear = true;
            BackupBLL sut = fixture.CreateSut();
            string ruta = Path.Combine(Path.GetTempPath(), "backup-falla.bak");

            Assert.ThrowsExactly<ExcepcionTecnica>(() => sut.CrearBackup(ruta));
        }

        [TestMethod]
        public void RestaurarBackup_RutaVacia_ThrowsExcepcionNegocioYNoDelegaEnRepositorio()
        {
            BackupFixture fixture = new BackupFixture();
            BackupBLL sut = fixture.CreateSut();

            Assert.ThrowsExactly<ExcepcionNegocio>(() => sut.RestaurarBackup(string.Empty));
            Assert.AreEqual(0, fixture.Repositorio.RestaurarBackupCallCount);
            Assert.AreEqual("FalloNegocio", fixture.Bitacora.Calls[0].Kind);
        }

        [TestMethod]
        public void RestaurarBackup_ExtensionInvalida_ThrowsExcepcionNegocio()
        {
            BackupFixture fixture = new BackupFixture();
            BackupBLL sut = fixture.CreateSut();
            string ruta = Path.Combine(Path.GetTempPath(), "backup-test.txt");

            Assert.ThrowsExactly<ExcepcionNegocio>(() => sut.RestaurarBackup(ruta));
            Assert.AreEqual(0, fixture.Repositorio.RestaurarBackupCallCount);
        }

        [TestMethod]
        public void RestaurarBackup_ArchivoInexistente_ThrowsExcepcionNegocio()
        {
            BackupFixture fixture = new BackupFixture();
            BackupBLL sut = fixture.CreateSut();
            string ruta = Path.Combine(Path.GetTempPath(), "archivo-inexistente-uitest.bak");

            Assert.ThrowsExactly<ExcepcionNegocio>(() => sut.RestaurarBackup(ruta));
            Assert.AreEqual(0, fixture.Repositorio.RestaurarBackupCallCount);
            Assert.AreEqual("FalloNegocio", fixture.Bitacora.Calls[0].Kind);
        }

        [TestMethod]
        public void RestaurarBackup_ArchivoValido_DelegaEnRepositorioYRegistraOperacion()
        {
            BackupFixture fixture = new BackupFixture();
            BackupBLL sut = fixture.CreateSut();
            string ruta = Path.Combine(Path.GetTempPath(), "backup-existente-uitest.bak");
            File.WriteAllText(ruta, "contenido de prueba");

            try
            {
                sut.RestaurarBackup(ruta);

                Assert.AreEqual(1, fixture.Repositorio.RestaurarBackupCallCount);
                Assert.AreEqual(ruta, fixture.Repositorio.UltimaRutaRestaurada);
                Assert.AreEqual("Operacion", fixture.Bitacora.Calls[0].Kind);
            }
            finally
            {
                File.Delete(ruta);
            }
        }

        [TestMethod]
        public void RestaurarBackup_RepositorioLanzaExcepcionTecnica_SePropagaSinSerTragada()
        {
            BackupFixture fixture = new BackupFixture();
            fixture.Repositorio.ThrowOnRestaurar = true;
            BackupBLL sut = fixture.CreateSut();
            string ruta = Path.Combine(Path.GetTempPath(), "backup-existente-falla.bak");
            File.WriteAllText(ruta, "contenido de prueba");

            try
            {
                Assert.ThrowsExactly<ExcepcionTecnica>(() => sut.RestaurarBackup(ruta));
            }
            finally
            {
                File.Delete(ruta);
            }
        }

        private sealed class BackupFixture
        {
            public readonly FakeBackupRepositorio Repositorio = new FakeBackupRepositorio();
            public readonly SpyBitacoraBLL Bitacora = new SpyBitacoraBLL();

            public BackupBLL CreateSut()
            {
                return new BackupBLL(Repositorio, Bitacora);
            }
        }
    }
}
