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

            Assert.ThrowsExactly<ExcepcionNegocio>(() => sut.CrearIdioma(new Idioma { Nombre = "espanol" }));
            Assert.AreEqual(0, fixture.UnidadFactory.CrearCallCount);
        }

        [TestMethod]
        public void CrearIdioma_ValidName_UsesUowAuditsAndSaves()
        {
            IdiomaFixture fixture = new IdiomaFixture();
            IdiomaBLL sut = fixture.CreateSut();

            Idioma creado = sut.CrearIdioma(new Idioma { Nombre = "  English  " });

            Assert.AreEqual(creado.Id, fixture.Unidad.IdiomaRepo.Insertados.Single().Id);
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

            bool result = sut.HacerIdiomaDefault(new Idioma { Id = 2 });

            Assert.IsTrue(result);
            Assert.IsFalse(fixture.Unidad.IdiomaRepo.ObtenerPorId(1).Default);
            Assert.IsTrue(fixture.Unidad.IdiomaRepo.ObtenerPorId(2).Default);
            Assert.IsTrue(fixture.Unidad.SaveCalled);
        }

        [TestMethod]
        public void ActualizarIdioma_DuplicateName_ThrowsExcepcionNegocio()
        {
            IdiomaFixture fixture = new IdiomaFixture();
            fixture.IdiomaRepo.Idiomas.Add(new Idioma { Id = 1, Nombre = "Espanol" });
            fixture.IdiomaRepo.Idiomas.Add(new Idioma { Id = 2, Nombre = "English" });
            IdiomaBLL sut = fixture.CreateSut();

            Assert.ThrowsExactly<ExcepcionNegocio>(() => sut.ActualizarIdioma(new Idioma { Id = 2, Nombre = "espanol" }));
            Assert.AreEqual(0, fixture.UnidadFactory.CrearCallCount);
        }

        [TestMethod]
        public void EliminarIdioma_DefaultLanguage_ThrowsExcepcionNegocio()
        {
            IdiomaFixture fixture = new IdiomaFixture();
            fixture.IdiomaRepo.Idiomas.Add(new Idioma { Id = 1, Nombre = "Espanol", Default = true });
            IdiomaBLL sut = fixture.CreateSut();

            Assert.ThrowsExactly<ExcepcionNegocio>(() => sut.EliminarIdioma(new Idioma { Id = 1 }));
        }

        [TestMethod]
        public void GuardarTraduccion_ValidData_UsesGuardarTraduccionInternoAndSaves()
        {
            IdiomaFixture fixture = new IdiomaFixture();
            fixture.Unidad.TraduccionRepo.PalabraIdsPorKey["Form.Ok"] = 10;
            IdiomaBLL sut = fixture.CreateSut();

            sut.GuardarTraduccion(new Traduccion { Idioma = new Idioma { Id = 1 }, Palabra = new Palabra { Key = " Form.Ok " }, Valor = " Aceptar " });

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

            Assert.ThrowsExactly<ExcepcionNegocio>(() => sut.ImportarJson(new Idioma { Id = 1 }, "missing-file.json"));
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

                sut.ImportarJson(new Idioma { Id = 1 }, path);

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

                Assert.ThrowsExactly<InvalidOperationException>(() => sut.ImportarJson(new Idioma { Id = 1 }, path));
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

                sut.ExportarJson(new Idioma { Id = 1 }, path);

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

        [TestMethod]
        public void CrearIdioma_Valido_DevuelveLaMismaInstanciaConId()
        {
            IdiomaFixture fixture = new IdiomaFixture();
            IdiomaBLL sut = fixture.CreateSut();
            Idioma nuevo = new Idioma { Nombre = "English", Default = true };

            Idioma creado = sut.CrearIdioma(nuevo);

            Assert.AreSame(nuevo, creado);
            Assert.IsTrue(creado.Id > 0);
            Assert.IsFalse(creado.Default);
        }

        [TestMethod]
        public void ActualizarIdioma_IdiomaRecibidoAlterado_CopiaSoloElNombreSobreElPersistido()
        {
            IdiomaFixture fixture = new IdiomaFixture();
            Idioma persistido = new Idioma { Id = 2, Nombre = "English", Default = false };
            fixture.Unidad.IdiomaRepo.Idiomas.Add(persistido);
            IdiomaBLL sut = fixture.CreateSut();

            bool result = sut.ActualizarIdioma(new Idioma { Id = 2, Nombre = " Ingles ", Default = true });

            Assert.IsTrue(result);
            Assert.AreSame(persistido, fixture.Unidad.IdiomaRepo.LastUpdated);
            Assert.AreEqual("Ingles", persistido.Nombre);
            Assert.IsFalse(persistido.Default);
            Assert.IsTrue(fixture.Unidad.SaveCalled);
        }

        [TestMethod]
        public void ActualizarIdioma_IdiomaInexistente_ReturnsFalseAndDoesNotSave()
        {
            IdiomaFixture fixture = new IdiomaFixture();
            IdiomaBLL sut = fixture.CreateSut();

            Assert.IsFalse(sut.ActualizarIdioma(new Idioma { Id = 9, Nombre = "Ingles" }));
            Assert.IsFalse(fixture.Unidad.SaveCalled);
        }

        [TestMethod]
        public void GuardarPreferencia_IdiomaCompleto_UsaElUsuarioEnSesionYElIdiomaPersistido()
        {
            IdiomaFixture fixture = new IdiomaFixture();
            Idioma persistido = IdiomaCompleto(1, "Espanol", true);
            fixture.IdiomaRepo.Idiomas.Add(persistido);
            fixture.Sesion.IniciarSesion(new BE.Usuario { Id = 4, Username = "user" });
            IdiomaBLL sut = fixture.CreateSut();

            bool result = sut.GuardarPreferencia(new Idioma { Id = 1 });

            Assert.IsTrue(result);
            Assert.AreEqual(4, fixture.Unidad.UsuarioPreferenciaRepo.LastUsuarioId);
            Assert.AreEqual(1, fixture.Unidad.UsuarioPreferenciaRepo.LastIdiomaId);
            Assert.AreEqual(4, fixture.UsuarioEstado.GuardarUsuarioId);
            Assert.AreSame(persistido, fixture.Observado.IdiomaActual);
            Assert.IsTrue(fixture.Unidad.SaveCalled);
        }

        [TestMethod]
        public void EliminarTraduccion_TraduccionRecibida_EliminaLaPersistida()
        {
            IdiomaFixture fixture = new IdiomaFixture();
            Idioma persistido = IdiomaCompleto(1, "Espanol", true);
            fixture.Unidad.IdiomaRepo.Idiomas.Add(persistido);
            IdiomaBLL sut = fixture.CreateSut();

            bool result = sut.EliminarTraduccion(new Traduccion { Idioma = new Idioma { Id = 1 }, Palabra = new Palabra { Id = 10 }, Valor = "alterado" });

            Assert.IsTrue(result);
            Assert.AreSame(persistido.Traducciones.Single(), fixture.Unidad.TraduccionRepo.TraduccionesEliminadas.Single());
            Assert.IsTrue(fixture.Unidad.SaveCalled);
        }

        [TestMethod]
        public void EliminarTraduccion_Nula_ThrowsExcepcionNegocio()
        {
            IdiomaFixture fixture = new IdiomaFixture();
            IdiomaBLL sut = fixture.CreateSut();

            ExcepcionNegocio ex = Assert.ThrowsExactly<ExcepcionNegocio>(() => sut.EliminarTraduccion(null));

            Assert.AreEqual("Error.Language.NotFound", ex.Message);
        }

        private static Idioma IdiomaCompleto(int id, string nombre, bool isDefault)
        {
            Idioma idioma = new Idioma { Id = id, Nombre = nombre, Default = isDefault };
            idioma.Traducciones.Add(new Traduccion { Palabra = new Palabra { Id = 10, Key = "Form.Ok" }, Valor = "Aceptar" });
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
            public readonly FakeSesion Sesion = new FakeSesion();
            public readonly FakeUnidadDeTrabajoFactory UnidadFactory;

            public IdiomaFixture()
            {
                UnidadFactory = new FakeUnidadDeTrabajoFactory(Unidad);
            }

            public IdiomaBLL CreateSut()
            {
                return new IdiomaBLL(IdiomaRepo, UsuarioPreferenciaRepo, TraduccionRepo, new IdiomaCatalogoService(), UsuarioEstado, Bitacora, UnidadFactory, Observado, Sesion);
            }
        }
    }
}
