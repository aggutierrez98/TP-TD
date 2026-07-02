using System;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BLL.Tests.TestDoubles;
using SERVICIOS;

namespace BLL.Tests
{
    [TestClass]
    public sealed class IdiomaBLLTests
    {
        [TestMethod]
        public void InicializarAplicacion_DefaultExists_AppliesDefaultLanguage()
        {
            IdiomaFixture fixture = new IdiomaFixture();
            Idioma defaultLanguage = IdiomaCompleto(1, "Espanol", true);
            fixture.IdiomaRepo.Idiomas.Add(defaultLanguage);
            IdiomaBLL sut = fixture.CreateSut();

            sut.InicializarAplicacion();

            Assert.AreSame(defaultLanguage, fixture.Observado.IdiomaActual);
        }

        [TestMethod]
        public void CrearIdioma_DuplicateName_ThrowsExcepcionNegocioAndDoesNotCreateUow()
        {
            IdiomaFixture fixture = new IdiomaFixture();
            fixture.IdiomaRepo.Idiomas.Add(new Idioma { Id = 1, Nombre = "Espanol" });
            IdiomaBLL sut = fixture.CreateSut();

            Assert.ThrowsExactly<ExcepcionNegocio>(() => sut.CrearIdioma("espanol"));
            Assert.AreEqual(0, fixture.UnidadFactory.CrearCallCount);
        }

        [TestMethod]
        public void CrearIdioma_ValidName_UsesUowAuditsAndSaves()
        {
            IdiomaFixture fixture = new IdiomaFixture();
            IdiomaBLL sut = fixture.CreateSut();

            int id = sut.CrearIdioma("  English  ");

            Assert.AreEqual(id, fixture.Unidad.IdiomaRepo.Insertados.Single().Id);
            Assert.AreEqual("English", fixture.Unidad.IdiomaRepo.Insertados.Single().Nombre);
            Assert.AreEqual("OperacionInterno", fixture.Bitacora.Calls.Single().Kind);
            Assert.IsTrue(fixture.Unidad.SaveCalled);
        }

        [TestMethod]
        public void HacerIdiomaDefault_SwitchesDefaultInsideUow()
        {
            IdiomaFixture fixture = new IdiomaFixture();
            fixture.Unidad.IdiomaRepo.Idiomas.Add(new Idioma { Id = 1, Nombre = "Espanol", Default = true });
            fixture.Unidad.IdiomaRepo.Idiomas.Add(new Idioma { Id = 2, Nombre = "English", Default = false });
            IdiomaBLL sut = fixture.CreateSut();

            bool result = sut.HacerIdiomaDefault(2);

            Assert.IsTrue(result);
            Assert.IsFalse(fixture.Unidad.IdiomaRepo.ObtenerIdiomaCompleto(1).Default);
            Assert.IsTrue(fixture.Unidad.IdiomaRepo.ObtenerIdiomaCompleto(2).Default);
            Assert.IsTrue(fixture.Unidad.SaveCalled);
        }

        [TestMethod]
        public void ActualizarIdioma_DuplicateName_ThrowsExcepcionNegocio()
        {
            IdiomaFixture fixture = new IdiomaFixture();
            fixture.IdiomaRepo.Idiomas.Add(new Idioma { Id = 1, Nombre = "Espanol" });
            fixture.IdiomaRepo.Idiomas.Add(new Idioma { Id = 2, Nombre = "English" });
            IdiomaBLL sut = fixture.CreateSut();

            Assert.ThrowsExactly<ExcepcionNegocio>(() => sut.ActualizarIdioma(2, "espanol", false));
            Assert.AreEqual(0, fixture.UnidadFactory.CrearCallCount);
        }

        [TestMethod]
        public void EliminarIdioma_DefaultLanguage_ThrowsExcepcionNegocio()
        {
            IdiomaFixture fixture = new IdiomaFixture();
            fixture.IdiomaRepo.Idiomas.Add(new Idioma { Id = 1, Nombre = "Espanol", Default = true });
            IdiomaBLL sut = fixture.CreateSut();

            Assert.ThrowsExactly<ExcepcionNegocio>(() => sut.EliminarIdioma(1));
        }

        [TestMethod]
        public void GuardarTraduccion_ValidData_UsesGuardarTraduccionInternoAndSaves()
        {
            IdiomaFixture fixture = new IdiomaFixture();
            fixture.Unidad.TraduccionRepo.PalabraIdsPorKey["Form.Ok"] = 10;
            IdiomaBLL sut = fixture.CreateSut();

            sut.GuardarTraduccion(1, " Form.Ok ", " Aceptar ");

            Tuple<int, int, string> saved = fixture.Unidad.TraduccionRepo.TraduccionesGuardadas.Single();
            Assert.AreEqual(1, saved.Item1);
            Assert.AreEqual(10, saved.Item2);
            Assert.AreEqual("Aceptar", saved.Item3);
            Assert.IsTrue(fixture.Unidad.SaveCalled);
        }

        [TestMethod]
        public void ImportarJson_FileNotFound_ThrowsExcepcionNegocio()
        {
            IdiomaFixture fixture = new IdiomaFixture();
            IdiomaBLL sut = fixture.CreateSut();

            Assert.ThrowsExactly<ExcepcionNegocio>(() => sut.ImportarJson(1, "missing-file.json"));
            Assert.AreEqual(0, fixture.UnidadFactory.CrearCallCount);
        }

        [TestMethod]
        public void ImportarJson_ValidJson_PersistsAllTranslationsInSingleUow()
        {
            string path = Path.GetTempFileName();
            try
            {
                File.WriteAllText(path, "{\"Form.Ok\":\"Aceptar\",\"Form.Cancel\":\"Cancelar\"}");
                IdiomaFixture fixture = new IdiomaFixture();
                fixture.Unidad.TraduccionRepo.PalabraIdsPorKey["Form.Ok"] = 10;
                fixture.Unidad.TraduccionRepo.PalabraIdsPorKey["Form.Cancel"] = 11;
                IdiomaBLL sut = fixture.CreateSut();

                sut.ImportarJson(1, path);

                Assert.AreEqual(1, fixture.UnidadFactory.CrearCallCount);
                Assert.AreEqual(2, fixture.Unidad.TraduccionRepo.TraduccionesGuardadas.Count);
                Assert.IsTrue(fixture.Unidad.SaveCalled);
            }
            finally
            {
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
            }
        }

        [TestMethod]
        public void ImportarJson_TranslationFails_DoesNotCallSave()
        {
            string path = Path.GetTempFileName();
            try
            {
                File.WriteAllText(path, "{\"Form.Ok\":\"Aceptar\"}");
                IdiomaFixture fixture = new IdiomaFixture();
                fixture.Unidad.TraduccionRepo.PalabraIdsPorKey["Form.Ok"] = 10;
                fixture.Unidad.TraduccionRepo.ThrowOnGuardar = true;
                IdiomaBLL sut = fixture.CreateSut();

                Assert.ThrowsExactly<InvalidOperationException>(() => sut.ImportarJson(1, path));
                Assert.IsFalse(fixture.Unidad.SaveCalled);
            }
            finally
            {
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
            }
        }

        [TestMethod]
        public void ExportarJson_ValidLanguage_WritesJsonAndAudits()
        {
            string path = Path.GetTempFileName();
            try
            {
                IdiomaFixture fixture = new IdiomaFixture();
                fixture.IdiomaRepo.Idiomas.Add(IdiomaCompleto(1, "Espanol", true));
                IdiomaBLL sut = fixture.CreateSut();

                sut.ExportarJson(1, path);

                string content = File.ReadAllText(path);
                StringAssert.Contains(content, "Form.Ok");
                Assert.IsTrue(fixture.Unidad.SaveCalled);
            }
            finally
            {
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
            }
        }

        private static Idioma IdiomaCompleto(int id, string nombre, bool isDefault)
        {
            Idioma idioma = new Idioma { Id = id, Nombre = nombre, Default = isDefault };
            idioma.Traducciones.Add(new Traduccion { PalabraId = 10, Palabra = new Palabra { Id = 10, Key = "Form.Ok" }, Valor = "Aceptar" });
            return idioma;
        }

        private sealed class IdiomaFixture
        {
            public readonly FakeIdiomaRepositorio IdiomaRepo = new FakeIdiomaRepositorio();
            public readonly FakeUsuarioPreferenciaRepositorio UsuarioPreferenciaRepo = new FakeUsuarioPreferenciaRepositorio();
            public readonly FakeTraduccionRepositorio TraduccionRepo = new FakeTraduccionRepositorio();
            public readonly SpyUsuarioEstadoBLL UsuarioEstado = new SpyUsuarioEstadoBLL();
            public readonly SpyBitacoraBLL Bitacora = new SpyBitacoraBLL();
            public readonly FakeUnidadDeTrabajo Unidad = new FakeUnidadDeTrabajo();
            public readonly FakeObservado Observado = new FakeObservado();
            public readonly FakeUnidadDeTrabajoFactory UnidadFactory;

            public IdiomaFixture()
            {
                UnidadFactory = new FakeUnidadDeTrabajoFactory(Unidad);
            }

            public IdiomaBLL CreateSut()
            {
                return new IdiomaBLL(IdiomaRepo, UsuarioPreferenciaRepo, TraduccionRepo, new IdiomaCatalogoService(), UsuarioEstado, Bitacora, UnidadFactory, Observado);
            }
        }
    }
}
