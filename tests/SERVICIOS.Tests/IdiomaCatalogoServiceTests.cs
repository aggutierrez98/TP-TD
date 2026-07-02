using Microsoft.VisualStudio.TestTools.UnitTesting;
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
    }
}
