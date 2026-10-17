using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text.Json;
using SERVICIOS;

namespace SERVICIOS.Tests
{
    [TestClass]
    public sealed class IdiomaCatalogoServiceTests
    {
        private readonly IdiomaCatalogoService _service = new IdiomaCatalogoService();

        [TestMethod]
        public void NormalizarNombre_ValidName_ReturnsTrimmedName()
        {
            Assert.AreEqual("Espanol", _service.NormalizarNombre("  Espanol  "));
        }

        [TestMethod]
        public void NormalizarNombre_EmptyName_ThrowsExcepcionNegocio()
        {
            Assert.ThrowsExactly<ExcepcionNegocio>(() => _service.NormalizarNombre("   "));
        }

        [TestMethod]
        public void ValidarNombreDuplicado_SameNameDifferentId_ThrowsExcepcionNegocio()
        {
            Idioma[] idiomas = { new Idioma { Id = 1, Nombre = "Espanol" } };

            Assert.ThrowsExactly<ExcepcionNegocio>(() => _service.ValidarNombreDuplicado(idiomas, "espanol", 2));
        }

        [TestMethod]
        public void ValidarNombreDuplicado_SameId_Allows()
        {
            Idioma[] idiomas = { new Idioma { Id = 1, Nombre = "Espanol" } };

            _service.ValidarNombreDuplicado(idiomas, "espanol", 1);
        }

        [TestMethod]
        public void EstaCompleto_NullIdioma_ReturnsFalse()
        {
            Assert.IsFalse(_service.EstaCompleto(null));
        }

        [TestMethod]
        public void EstaCompleto_AllTranslationsWithValues_ReturnsTrue()
        {
            Idioma idioma = new Idioma();
            idioma.Traducciones.Add(new Traduccion { Palabra = new Palabra { Key = "Form.Ok" }, Valor = "OK" });

            Assert.IsTrue(_service.EstaCompleto(idioma));
        }

        [TestMethod]
        public void EstaCompleto_MissingOrEmptyTranslation_ReturnsFalse()
        {
            Idioma idioma = new Idioma();
            idioma.Traducciones.Add(new Traduccion { Palabra = new Palabra { Key = "Form.Ok" }, Valor = " " });

            Assert.IsFalse(_service.EstaCompleto(idioma));
        }

        [TestMethod]
        public void ValidarIdiomaSeleccionable_Null_ThrowsExcepcionNegocio()
        {
            Assert.ThrowsExactly<ExcepcionNegocio>(() => _service.ValidarIdiomaSeleccionable(null));
        }

        [TestMethod]
        public void ValidarIdiomaSeleccionable_Incomplete_ThrowsExcepcionNegocio()
        {
            Assert.ThrowsExactly<ExcepcionNegocio>(() => _service.ValidarIdiomaSeleccionable(new Idioma()));
        }

        [TestMethod]
        public void LeerCatalogoJson_RutaVacia_ThrowsJsonFileNotFound()
        {
            ExcepcionNegocio ex = Assert.ThrowsExactly<ExcepcionNegocio>(() => _service.LeerCatalogoJson("  "));

            Assert.AreEqual("Error.Language.JsonFileNotFound", ex.Message);
        }

        [TestMethod]
        public void LeerCatalogoJson_ArchivoInexistente_ThrowsJsonFileNotFound()
        {
            string ruta = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N") + ".json");

            ExcepcionNegocio ex = Assert.ThrowsExactly<ExcepcionNegocio>(() => _service.LeerCatalogoJson(ruta));

            Assert.AreEqual("Error.Language.JsonFileNotFound", ex.Message);
        }

        [TestMethod]
        public void LeerCatalogoJson_JsonInvalido_ThrowsExcepcionTecnicaJsonInvalidFormat()
        {
            WithTempFile("{ esto no es json", ruta =>
            {
                ExcepcionTecnica ex = Assert.ThrowsExactly<ExcepcionTecnica>(() => _service.LeerCatalogoJson(ruta));

                Assert.AreEqual("Error.Language.JsonInvalidFormat", ex.Message);
                Assert.IsInstanceOfType(ex.InnerException, typeof(JsonException));
            });
        }

        [TestMethod]
        public void LeerCatalogoJson_ObjetoVacio_ThrowsJsonEmpty()
        {
            WithTempFile("{}", ruta =>
            {
                ExcepcionNegocio ex = Assert.ThrowsExactly<ExcepcionNegocio>(() => _service.LeerCatalogoJson(ruta));

                Assert.AreEqual("Error.Language.JsonEmpty", ex.Message);
            });
        }

        [TestMethod]
        public void LeerCatalogoJson_NullLiteral_ThrowsJsonEmpty()
        {
            WithTempFile("null", ruta =>
            {
                ExcepcionNegocio ex = Assert.ThrowsExactly<ExcepcionNegocio>(() => _service.LeerCatalogoJson(ruta));

                Assert.AreEqual("Error.Language.JsonEmpty", ex.Message);
            });
        }

        [TestMethod]
        public void LeerCatalogoJson_JsonValidoConAcentos_ReturnsDiccionario()
        {
            WithTempFile("{\"Form.Ok\":\"Aceptación\",\"Form.Cancel\":\"Cancelar ñandú\"}", ruta =>
            {
                Dictionary<string, string> resultado = _service.LeerCatalogoJson(ruta);

                Assert.HasCount(2, resultado);
                Assert.AreEqual("Aceptación", resultado["Form.Ok"]);
                Assert.AreEqual("Cancelar ñandú", resultado["Form.Cancel"]);
            });
        }

        [TestMethod]
        public void ValidarRutaExportacion_RutaVacia_ThrowsExportPathRequired()
        {
            ExcepcionNegocio ex = Assert.ThrowsExactly<ExcepcionNegocio>(() => _service.ValidarRutaExportacion(" "));

            Assert.AreEqual("Error.Language.ExportPathRequired", ex.Message);
        }

        [TestMethod]
        public void EscribirCatalogoJson_RutaVacia_ThrowsExportPathRequired()
        {
            ExcepcionNegocio ex = Assert.ThrowsExactly<ExcepcionNegocio>(
                () => _service.EscribirCatalogoJson(new Dictionary<string, string> { { "A", "B" } }, null));

            Assert.AreEqual("Error.Language.ExportPathRequired", ex.Message);
        }

        [TestMethod]
        public void EscribirCatalogoJson_Contenido_EscribeUtf8SinBom()
        {
            string ruta = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N") + ".json");
            try
            {
                _service.EscribirCatalogoJson(new Dictionary<string, string> { { "Form.Ok", "Aceptación" } }, ruta);

                byte[] bytes = File.ReadAllBytes(ruta);
                Assert.IsFalse(bytes.Length >= 3 && bytes[0] == 0xEF && bytes[1] == 0xBB && bytes[2] == 0xBF);
                Assert.AreEqual("{\"Form.Ok\":\"Aceptación\"}", new UTF8Encoding(false).GetString(bytes));
            }
            finally
            {
                File.Delete(ruta);
            }
        }

        [TestMethod]
        public void ConstruirCatalogoExportable_OrdenaPorKeyYOmiteInvalidas()
        {
            Idioma idioma = new Idioma();
            idioma.Traducciones.Add(new Traduccion { Palabra = new Palabra { Key = "Z.Key" }, Valor = "Z" });
            idioma.Traducciones.Add(new Traduccion { Palabra = new Palabra { Key = "M.Key" }, Valor = null });
            idioma.Traducciones.Add(new Traduccion { Palabra = new Palabra { Key = "A.Key" }, Valor = "A" });
            idioma.Traducciones.Add(new Traduccion { Palabra = new Palabra { Key = " " }, Valor = "X" });
            idioma.Traducciones.Add(new Traduccion { Palabra = null, Valor = "Y" });

            Dictionary<string, string> resultado = _service.ConstruirCatalogoExportable(idioma);

            CollectionAssert.AreEqual(new[] { "A.Key", "M.Key", "Z.Key" }, resultado.Keys.ToArray());
            Assert.AreEqual(string.Empty, resultado["M.Key"]);
        }

        [TestMethod]
        public void EscribirCatalogoJson_LuegoLeerCatalogoJson_RoundTrip()
        {
            string ruta = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N") + ".json");
            try
            {
                Dictionary<string, string> original = new Dictionary<string, string>
                {
                    { "Form.Ok", "Aceptación" },
                    { "Form.Cancel", "Cancelar ñ" }
                };

                _service.EscribirCatalogoJson(original, ruta);
                Dictionary<string, string> leido = _service.LeerCatalogoJson(ruta);

                CollectionAssert.AreEquivalent(original, leido);
            }
            finally
            {
                File.Delete(ruta);
            }
        }

        private static void WithTempFile(string contenido, Action<string> accion)
        {
            string ruta = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N") + ".json");
            File.WriteAllText(ruta, contenido, new UTF8Encoding(false));
            try
            {
                accion(ruta);
            }
            finally
            {
                File.Delete(ruta);
            }
        }
    }
}
