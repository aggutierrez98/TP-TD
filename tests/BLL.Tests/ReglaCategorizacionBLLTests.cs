using System;
using System.Collections.Generic;
using System.Linq;
using ABSTRACCIONES;
using BE;
using BLL.Tests.TestDoubles;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SERVICIOS;

namespace BLL.Tests
{
    [TestClass]
    public sealed class ReglaCategorizacionBLLTests
    {
        [TestMethod]
        public void Crear_NombreVacio_ThrowsExcepcionNegocioAndDoesNotCreateUow()
        {
            ReglaFixture fixture = new ReglaFixture(usuarioActualId: 1);
            ReglaCategorizacionBLL sut = fixture.CreateSut();

            Assert.ThrowsExactly<ExcepcionNegocio>(() => sut.Crear(Nueva("   ", 1, "palabra")));
            Assert.AreEqual(0, fixture.UnidadFactory.CrearCallCount);
        }

        [TestMethod]
        public void Crear_PalabrasClaveVacias_ThrowsExcepcionNegocioAndDoesNotCreateUow()
        {
            ReglaFixture fixture = new ReglaFixture(usuarioActualId: 1);
            ReglaCategorizacionBLL sut = fixture.CreateSut();

            Assert.ThrowsExactly<ExcepcionNegocio>(() => sut.Crear(Nueva("Regla 1", 1, "   ")));
            Assert.AreEqual(0, fixture.UnidadFactory.CrearCallCount);
        }

        [TestMethod]
        public void Crear_CategoriaInexistente_ThrowsExcepcionNegocio()
        {
            ReglaFixture fixture = new ReglaFixture(usuarioActualId: 1);
            ReglaCategorizacionBLL sut = fixture.CreateSut();

            Assert.ThrowsExactly<ExcepcionNegocio>(() => sut.Crear(Nueva("Regla 1", 99, "palabra")));
        }

        [TestMethod]
        public void Crear_CategoriaAjena_ThrowsExcepcionNegocio()
        {
            ReglaFixture fixture = new ReglaFixture(usuarioActualId: 1);
            fixture.CategoriaRepo.Categorias.Add(5, new Categoria { Id = 5, Nombre = "Ajena", Usuario = new Usuario { Id = 99 } });
            ReglaCategorizacionBLL sut = fixture.CreateSut();

            Assert.ThrowsExactly<ExcepcionNegocio>(() => sut.Crear(Nueva("Regla 1", 5, "palabra")));
        }

        [TestMethod]
        public void Crear_ComercioInexistente_ThrowsExcepcionNegocio()
        {
            ReglaFixture fixture = new ReglaFixture(usuarioActualId: 1);
            fixture.CategoriaRepo.Categorias.Add(1, new Categoria { Id = 1, Nombre = "Global", Usuario = null });
            ReglaCategorizacionBLL sut = fixture.CreateSut();

            Assert.ThrowsExactly<ExcepcionNegocio>(() => sut.Crear(Nueva("Regla 1", 1, "palabra"), Comercios(99)));
        }

        [TestMethod]
        public void Crear_Valido_InsertaAsociaComerciosAuditaYGuarda()
        {
            ReglaFixture fixture = new ReglaFixture(usuarioActualId: 3);
            fixture.CategoriaRepo.Categorias.Add(1, new Categoria { Id = 1, Nombre = "Global", Usuario = null });
            fixture.ComercioRepo.Comercios.Add(10, new Comercio { Id = 10, Nombre = "Coto" });
            ReglaCategorizacionBLL sut = fixture.CreateSut();

            int id = sut.Crear(Nueva("Supermercado", 1, "super, coto, mercado"), Comercios(10)).Id;

            ReglaCategorizacion insertada = fixture.Unidad.ReglaCategorizacionRepo.Reglas[id];
            Assert.AreEqual("Supermercado", insertada.Nombre);
            Assert.AreEqual(1, insertada.Categoria.Id);
            Assert.AreEqual(3, insertada.Usuario.Id);
            Assert.AreEqual("super, coto, mercado", insertada.PalabrasClave);

            List<int> comerciosAsociados = fixture.Unidad.ComercioReglaCategorizacionRepo.ComercioIdsDe(id);
            Assert.AreEqual(1, comerciosAsociados.Count);
            Assert.AreEqual(10, comerciosAsociados[0]);

            Assert.AreEqual("OperacionInterno", fixture.Bitacora.Calls.Single().Kind);
            Assert.IsTrue(fixture.Unidad.SaveCalled);
        }

        [TestMethod]
        public void Actualizar_ReglaAjena_ThrowsExcepcionNegocio()
        {
            ReglaFixture fixture = new ReglaFixture(usuarioActualId: 1);
            fixture.CategoriaRepo.Categorias.Add(1, new Categoria { Id = 1, Nombre = "Global", Usuario = null });
            fixture.Unidad.ReglaCategorizacionRepo.Reglas.Add(5, new ReglaCategorizacion { Id = 5, Usuario = new Usuario { Id = 99 }, Categoria = new Categoria { Id = 1 }, Nombre = "R1", PalabrasClave = "p" });
            ReglaCategorizacionBLL sut = fixture.CreateSut();

            Assert.ThrowsExactly<ExcepcionNegocio>(() => sut.Actualizar(Editada(5, "R1", 1, "nueva")));
        }

        [TestMethod]
        public void Actualizar_Valido_ActualizaAsociaComerciosAuditaYGuarda()
        {
            ReglaFixture fixture = new ReglaFixture(usuarioActualId: 2);
            fixture.CategoriaRepo.Categorias.Add(1, new Categoria { Id = 1, Nombre = "Global", Usuario = null });
            fixture.ComercioRepo.Comercios.Add(10, new Comercio { Id = 10, Nombre = "Coto" });
            fixture.ComercioRepo.Comercios.Add(20, new Comercio { Id = 20, Nombre = "Dia" });
            fixture.Unidad.ReglaCategorizacionRepo.Reglas.Add(3, new ReglaCategorizacion { Id = 3, Usuario = new Usuario { Id = 2 }, Categoria = new Categoria { Id = 1 }, Nombre = "Vieja", PalabrasClave = "v" });
            fixture.Unidad.ComercioReglaCategorizacionRepo.ComerciosPorRegla[3] = new List<int> { 10 };
            ReglaCategorizacionBLL sut = fixture.CreateSut();

            bool actualizado = sut.Actualizar(Editada(3, "Nueva", 1, "nueva, palabra"), Comercios(20));

            Assert.IsTrue(actualizado);
            ReglaCategorizacion r = fixture.Unidad.ReglaCategorizacionRepo.Reglas[3];
            Assert.AreEqual("Nueva", r.Nombre);
            Assert.AreEqual("nueva, palabra", r.PalabrasClave);

            List<int> comercios = fixture.Unidad.ComercioReglaCategorizacionRepo.ComercioIdsDe(3);
            Assert.AreEqual(1, comercios.Count);
            Assert.AreEqual(20, comercios[0]);

            Assert.AreEqual("OperacionInterno", fixture.Bitacora.Calls.Single().Kind);
            Assert.IsTrue(fixture.Unidad.SaveCalled);
        }

        [TestMethod]
        public void Eliminar_ReglaAjena_ThrowsExcepcionNegocio()
        {
            ReglaFixture fixture = new ReglaFixture(usuarioActualId: 1);
            fixture.Unidad.ReglaCategorizacionRepo.Reglas.Add(5, new ReglaCategorizacion { Id = 5, Usuario = new Usuario { Id = 99 } });
            ReglaCategorizacionBLL sut = fixture.CreateSut();

            Assert.ThrowsExactly<ExcepcionNegocio>(() => sut.Eliminar(new ReglaCategorizacion { Id = 5 }));
        }

        [TestMethod]
        public void Eliminar_Valido_EliminaDesasociaComerciosAuditaYGuarda()
        {
            ReglaFixture fixture = new ReglaFixture(usuarioActualId: 2);
            fixture.Unidad.ReglaCategorizacionRepo.Reglas.Add(3, new ReglaCategorizacion { Id = 3, Usuario = new Usuario { Id = 2 } });
            fixture.Unidad.ComercioReglaCategorizacionRepo.ComerciosPorRegla[3] = new List<int> { 10 };
            ReglaCategorizacionBLL sut = fixture.CreateSut();

            bool eliminado = sut.Eliminar(new ReglaCategorizacion { Id = 3 });

            Assert.IsTrue(eliminado);
            Assert.IsFalse(fixture.Unidad.ReglaCategorizacionRepo.Reglas.ContainsKey(3));
            Assert.AreEqual(0, fixture.Unidad.ComercioReglaCategorizacionRepo.ComercioIdsDe(3).Count);
            Assert.AreEqual("OperacionInterno", fixture.Bitacora.Calls.Single().Kind);
            Assert.IsTrue(fixture.Unidad.SaveCalled);
        }

        // --- Matching & Specificity tests ---

        [TestMethod]
        public void SugerirCategoria_DescripcionVacia_DevuelveNull()
        {
            ReglaFixture fixture = new ReglaFixture(usuarioActualId: 1);
            ReglaCategorizacionBLL sut = fixture.CreateSut();

            Assert.IsNull(sut.SugerirCategoria(null));
            Assert.IsNull(sut.SugerirCategoria("   "));
        }

        [TestMethod]
        public void SugerirCategoria_SinReglas_DevuelveNull()
        {
            ReglaFixture fixture = new ReglaFixture(usuarioActualId: 1);
            ReglaCategorizacionBLL sut = fixture.CreateSut();

            Assert.IsNull(sut.SugerirCategoria("Compra de verdura"));
        }

        [TestMethod]
        public void SugerirCategoria_ReglaGeneralCoincidePalabraClave_DevuelveCategoria()
        {
            ReglaFixture fixture = new ReglaFixture(usuarioActualId: 1);
            Categoria catSuper = new Categoria { Id = 1, Nombre = "Supermercado" };
            fixture.ReglaRepo.Reglas.Add(1, new ReglaCategorizacion
            {
                Id = 1,
                Usuario = new Usuario { Id = 1 },
                Categoria = catSuper,
                Nombre = "Regla Super",
                PalabrasClave = "coto, dia, carrefour"
            });
            ReglaCategorizacionBLL sut = fixture.CreateSut();

            Categoria sugerida = sut.SugerirCategoria("Fui a comprar a Coto ayer");

            Assert.IsNotNull(sugerida);
            Assert.AreEqual(catSuper.Id, sugerida.Id);
        }

        [TestMethod]
        public void SugerirCategoria_CoincidenciaParcialAccidental_NoCoincide()
        {
            ReglaFixture fixture = new ReglaFixture(usuarioActualId: 1);
            Categoria catAlimento = new Categoria { Id = 1, Nombre = "Alimentos" };
            fixture.ReglaRepo.Reglas.Add(1, new ReglaCategorizacion
            {
                Id = 1,
                Usuario = new Usuario { Id = 1 },
                Categoria = catAlimento,
                Nombre = "Regla Pan",
                PalabrasClave = "pan"
            });
            ReglaCategorizacionBLL sut = fixture.CreateSut();

            // "pantalones" o "empanadas" no deben coincidir con la palabra clave "pan"
            Assert.IsNull(sut.SugerirCategoria("Compra de pantalones"));
            Assert.IsNull(sut.SugerirCategoria("Cena de empanadas"));

            // "pan de molde" si debe coincidir
            Categoria sugerida = sut.SugerirCategoria("Compre pan de molde");
            Assert.IsNotNull(sugerida);
            Assert.AreEqual(catAlimento.Id, sugerida.Id);
        }

        [TestMethod]
        public void SugerirCategoria_NormalizaMayusculasYTildes_Coincide()
        {
            ReglaFixture fixture = new ReglaFixture(usuarioActualId: 1);
            Categoria catCafe = new Categoria { Id = 2, Nombre = "Cafeteria" };
            fixture.ReglaRepo.Reglas.Add(1, new ReglaCategorizacion
            {
                Id = 1,
                Usuario = new Usuario { Id = 1 },
                Categoria = catCafe,
                Nombre = "Regla Cafe",
                PalabrasClave = "cafe"
            });
            ReglaCategorizacionBLL sut = fixture.CreateSut();

            // Debe coincidir con "CAFÉ" con tilde y mayúsculas
            Categoria sugerida = sut.SugerirCategoria("Pago de CAFÉ con leche");
            Assert.IsNotNull(sugerida);
            Assert.AreEqual(catCafe.Id, sugerida.Id);
        }

        [TestMethod]
        public void SugerirCategoria_ReglaConComercio_SinComercioEnMovimiento_NoEsElegible()
        {
            ReglaFixture fixture = new ReglaFixture(usuarioActualId: 1);
            Categoria catCombustible = new Categoria { Id = 3, Nombre = "Combustible" };
            fixture.ReglaRepo.Reglas.Add(1, new ReglaCategorizacion
            {
                Id = 1,
                Usuario = new Usuario { Id = 1 },
                Categoria = catCombustible,
                Nombre = "YPF",
                PalabrasClave = "nafta, estacion"
            });
            // La regla esta vinculada al comercio 5
            fixture.ComercioReglaRepo.ComerciosPorRegla.Add(1, new List<int> { 5 });
            ReglaCategorizacionBLL sut = fixture.CreateSut();

            // Si el movimiento no especifica comercio, la regla con comercio no es elegible
            Assert.IsNull(sut.SugerirCategoria("Carga de nafta", comercio: null));
            // Si el comercio es otro, tampoco
            Assert.IsNull(sut.SugerirCategoria("Carga de nafta", comercio: new Comercio { Id = 99 }));

            // Si el comercio coincide, es elegible y devuelve la categoria
            Categoria sugerida = sut.SugerirCategoria("Carga de nafta", comercio: new Comercio { Id = 5 });
            Assert.IsNotNull(sugerida);
            Assert.AreEqual(catCombustible.Id, sugerida.Id);
        }

        [TestMethod]
        public void SugerirCategoria_DesempatePorCantidadDeCoincidencias_GanaLaDeMayorCantidad()
        {
            ReglaFixture fixture = new ReglaFixture(usuarioActualId: 1);
            Categoria catGeneral = new Categoria { Id = 1, Nombre = "General" };
            Categoria catEspecifica = new Categoria { Id = 2, Nombre = "Especifica" };

            // Regla 1 tiene 1 palabra que coincide ("supermercado")
            fixture.ReglaRepo.Reglas.Add(1, new ReglaCategorizacion
            {
                Id = 1,
                Usuario = new Usuario { Id = 1 },
                Categoria = catGeneral,
                Nombre = "Regla 1",
                PalabrasClave = "supermercado"
            });
            // Regla 2 tiene 2 palabras que coinciden ("supermercado", "coto")
            fixture.ReglaRepo.Reglas.Add(2, new ReglaCategorizacion
            {
                Id = 2,
                Usuario = new Usuario { Id = 1 },
                Categoria = catEspecifica,
                Nombre = "Regla 2",
                PalabrasClave = "supermercado, coto"
            });
            ReglaCategorizacionBLL sut = fixture.CreateSut();

            Categoria sugerida = sut.SugerirCategoria("Compra en supermercado coto");
            Assert.IsNotNull(sugerida);
            Assert.AreEqual(catEspecifica.Id, sugerida.Id);
        }

        [TestMethod]
        public void SugerirCategoria_DesempatePorCoberturaDeTexto_GanaLaDeMayorCobertura()
        {
            ReglaFixture fixture = new ReglaFixture(usuarioActualId: 1);
            Categoria catA = new Categoria { Id = 1, Nombre = "Cat A" };
            Categoria catB = new Categoria { Id = 2, Nombre = "Cat B" };

            // Regla A coincide con "supermercado" (12 caracteres)
            fixture.ReglaRepo.Reglas.Add(1, new ReglaCategorizacion
            {
                Id = 1,
                Usuario = new Usuario { Id = 1 },
                Categoria = catA,
                Nombre = "Regla A",
                PalabrasClave = "supermercado"
            });
            // Regla B coincide con "te" (2 caracteres)
            fixture.ReglaRepo.Reglas.Add(2, new ReglaCategorizacion
            {
                Id = 2,
                Usuario = new Usuario { Id = 1 },
                Categoria = catB,
                Nombre = "Regla B",
                PalabrasClave = "te"
            });
            ReglaCategorizacionBLL sut = fixture.CreateSut();

            Categoria sugerida = sut.SugerirCategoria("Compre te en el supermercado");
            Assert.IsNotNull(sugerida);
            Assert.AreEqual(catA.Id, sugerida.Id);
        }

        [TestMethod]
        public void SugerirCategoria_DesempatePorComercio_GanaLaDeComercioEspecifico()
        {
            ReglaFixture fixture = new ReglaFixture(usuarioActualId: 1);
            Categoria catGeneral = new Categoria { Id = 1, Nombre = "General" };
            Categoria catComercio = new Categoria { Id = 2, Nombre = "PorComercio" };

            // Ambas coinciden con "coto" (misma cantidad de palabras y misma longitud)
            // Regla 1 es general (sin comercios)
            fixture.ReglaRepo.Reglas.Add(1, new ReglaCategorizacion
            {
                Id = 1,
                Usuario = new Usuario { Id = 1 },
                Categoria = catGeneral,
                Nombre = "General",
                PalabrasClave = "coto"
            });
            // Regla 2 esta asociada al comercio 10
            fixture.ReglaRepo.Reglas.Add(2, new ReglaCategorizacion
            {
                Id = 2,
                Usuario = new Usuario { Id = 1 },
                Categoria = catComercio,
                Nombre = "Especifica Comercio",
                PalabrasClave = "coto"
            });
            fixture.ComercioReglaRepo.ComerciosPorRegla.Add(2, new List<int> { 10 });

            ReglaCategorizacionBLL sut = fixture.CreateSut();

            // Ante empate textual, el match de comercio desempata
            Categoria sugerida = sut.SugerirCategoria("Compra coto", comercio: new Comercio { Id = 10 });
            Assert.IsNotNull(sugerida);
            Assert.AreEqual(catComercio.Id, sugerida.Id);
        }

        [TestMethod]
        public void SugerirCategoria_EmpateResidual_DevuelveNullParaForzarSeleccionManual()
        {
            ReglaFixture fixture = new ReglaFixture(usuarioActualId: 1);
            Categoria catA = new Categoria { Id = 1, Nombre = "Cat A" };
            Categoria catB = new Categoria { Id = 2, Nombre = "Cat B" };

            // Dos reglas generales con palabras de la misma longitud que ambas coinciden
            fixture.ReglaRepo.Reglas.Add(1, new ReglaCategorizacion
            {
                Id = 1,
                Usuario = new Usuario { Id = 1 },
                Categoria = catA,
                Nombre = "Regla 1",
                PalabrasClave = "pan" // 3 chars
            });
            fixture.ReglaRepo.Reglas.Add(2, new ReglaCategorizacion
            {
                Id = 2,
                Usuario = new Usuario { Id = 1 },
                Categoria = catB,
                Nombre = "Regla 2",
                PalabrasClave = "sal" // 3 chars
            });
            ReglaCategorizacionBLL sut = fixture.CreateSut();

            // "Compre pan y sal" -> ambas tienen 1 match, cobertura 3, y son generales -> empate residual
            Categoria sugerida = sut.SugerirCategoria("Compre pan y sal");
            Assert.IsNull(sugerida);
        }

        [TestMethod]
        public void ObtenerParaUsuarioActual_DelegaAlRepositorioConElUsuarioDeSesion()
        {
            ReglaFixture fixture = new ReglaFixture(usuarioActualId: 5);
            fixture.ReglaRepo.Reglas.Add(1, new ReglaCategorizacion { Id = 1, Usuario = new Usuario { Id = 5 }, Nombre = "Propia" });
            fixture.ReglaRepo.Reglas.Add(2, new ReglaCategorizacion { Id = 2, Usuario = new Usuario { Id = 99 }, Nombre = "Ajena" });
            ReglaCategorizacionBLL sut = fixture.CreateSut();

            List<ReglaCategorizacion> reglas = sut.ObtenerParaUsuarioActual();

            Assert.AreEqual(1, reglas.Count);
            Assert.AreEqual("Propia", reglas[0].Nombre);
        }

        [TestMethod]
        public void ObtenerComerciosPorRegla_DevuelveLosComerciosAsociadosALaRegla()
        {
            ReglaFixture fixture = new ReglaFixture(usuarioActualId: 1);
            fixture.ComercioReglaRepo.ComerciosPorRegla.Add(3, new List<int> { 10, 20 });
            ReglaCategorizacionBLL sut = fixture.CreateSut();

            List<Comercio> comercios = sut.ObtenerComerciosPorRegla(new ReglaCategorizacion { Id = 3 });

            CollectionAssert.AreEqual(new[] { 10, 20 }, comercios.Select(item => item.Id).ToList());
        }

        [TestMethod]
        public void Crear_Valido_DevuelveLaMismaInstanciaConIdYUsuarioDeSesion()
        {
            ReglaFixture fixture = new ReglaFixture(usuarioActualId: 7);
            fixture.CategoriaRepo.Categorias.Add(1, new Categoria { Id = 1, Nombre = "Global", Usuario = null });
            ReglaCategorizacionBLL sut = fixture.CreateSut();
            ReglaCategorizacion regla = Nueva("Super", 1, "coto");

            ReglaCategorizacion creada = sut.Crear(regla);

            Assert.AreSame(regla, creada);
            Assert.IsTrue(creada.Id > 0);
            Assert.AreSame(fixture.UsuarioActual, creada.Usuario);
        }

        [TestMethod]
        public void Crear_ReglaNula_ThrowsExcepcionNegocioNameRequired()
        {
            ReglaFixture fixture = new ReglaFixture(usuarioActualId: 1);
            ReglaCategorizacionBLL sut = fixture.CreateSut();

            ExcepcionNegocio ex = Assert.ThrowsExactly<ExcepcionNegocio>(() => sut.Crear(null));
            Assert.AreEqual("Error.Rule.NameRequired", ex.Message);
            Assert.AreEqual(0, fixture.UnidadFactory.CrearCallCount);
        }

        [TestMethod]
        public void Actualizar_UsuarioAdulteradoEnLaEntidadRecibida_ValidaPropiedadContraLaPersistida()
        {
            ReglaFixture fixture = new ReglaFixture(usuarioActualId: 1);
            fixture.CategoriaRepo.Categorias.Add(1, new Categoria { Id = 1, Nombre = "Global", Usuario = null });
            fixture.Unidad.ReglaCategorizacionRepo.Reglas.Add(5, new ReglaCategorizacion { Id = 5, Usuario = new Usuario { Id = 99 }, Categoria = new Categoria { Id = 1 }, Nombre = "Ajena", PalabrasClave = "p" });
            ReglaCategorizacionBLL sut = fixture.CreateSut();
            ReglaCategorizacion adulterada = Editada(5, "Nueva", 1, "nueva");
            adulterada.Usuario = fixture.UsuarioActual;

            ExcepcionNegocio ex = Assert.ThrowsExactly<ExcepcionNegocio>(() => sut.Actualizar(adulterada));

            Assert.AreEqual("Error.Rule.NotOwned", ex.Message);
            Assert.AreEqual("Ajena", fixture.Unidad.ReglaCategorizacionRepo.Reglas[5].Nombre);
            Assert.IsNull(fixture.Unidad.ReglaCategorizacionRepo.LastUpdated);
        }

        [TestMethod]
        public void Actualizar_Valido_CopiaSoloLosCamposEditablesSobreLaPersistida()
        {
            ReglaFixture fixture = new ReglaFixture(usuarioActualId: 2);
            fixture.CategoriaRepo.Categorias.Add(4, new Categoria { Id = 4, Nombre = "Global", Usuario = null });
            ReglaCategorizacion persistida = new ReglaCategorizacion { Id = 3, Usuario = new Usuario { Id = 2 }, Categoria = new Categoria { Id = 1 }, Nombre = "Vieja", PalabrasClave = "v" };
            fixture.Unidad.ReglaCategorizacionRepo.Reglas.Add(3, persistida);
            ReglaCategorizacionBLL sut = fixture.CreateSut();
            ReglaCategorizacion recibida = Editada(3, "  Nueva  ", 4, "  nueva  ");
            recibida.Usuario = new Usuario { Id = 99 };

            sut.Actualizar(recibida);

            Assert.AreSame(persistida, fixture.Unidad.ReglaCategorizacionRepo.LastUpdated);
            Assert.AreEqual("Nueva", persistida.Nombre);
            Assert.AreEqual("nueva", persistida.PalabrasClave);
            Assert.AreEqual(4, persistida.Categoria.Id);
            Assert.AreEqual(2, persistida.Usuario.Id);
        }

        [TestMethod]
        public void Eliminar_UsuarioAdulteradoEnLaEntidadRecibida_ValidaPropiedadContraLaPersistida()
        {
            ReglaFixture fixture = new ReglaFixture(usuarioActualId: 1);
            fixture.Unidad.ReglaCategorizacionRepo.Reglas.Add(5, new ReglaCategorizacion { Id = 5, Usuario = new Usuario { Id = 99 } });
            fixture.Unidad.ComercioReglaCategorizacionRepo.ComerciosPorRegla[5] = new List<int> { 10 };
            ReglaCategorizacionBLL sut = fixture.CreateSut();
            ReglaCategorizacion adulterada = new ReglaCategorizacion { Id = 5, Usuario = fixture.UsuarioActual };

            ExcepcionNegocio ex = Assert.ThrowsExactly<ExcepcionNegocio>(() => sut.Eliminar(adulterada));

            Assert.AreEqual("Error.Rule.NotOwned", ex.Message);
            Assert.IsTrue(fixture.Unidad.ReglaCategorizacionRepo.Reglas.ContainsKey(5));
            Assert.AreEqual(1, fixture.Unidad.ComercioReglaCategorizacionRepo.ComercioIdsDe(5).Count);
            Assert.IsNull(fixture.Unidad.ReglaCategorizacionRepo.LastDeleted);
        }

        private static ReglaCategorizacion Nueva(string nombre, int categoriaId, string palabrasClave)
        {
            return new ReglaCategorizacion { Nombre = nombre, Categoria = new Categoria { Id = categoriaId }, PalabrasClave = palabrasClave };
        }

        private static ReglaCategorizacion Editada(int id, string nombre, int categoriaId, string palabrasClave)
        {
            ReglaCategorizacion regla = Nueva(nombre, categoriaId, palabrasClave);
            regla.Id = id;
            return regla;
        }

        private static List<Comercio> Comercios(params int[] ids)
        {
            return ids.Select(id => new Comercio { Id = id }).ToList();
        }

        private sealed class ReglaFixture
        {
            public readonly FakeReglaCategorizacionRepositorio ReglaRepo = new FakeReglaCategorizacionRepositorio();
            public readonly FakeComercioReglaCategorizacionRepositorio ComercioReglaRepo = new FakeComercioReglaCategorizacionRepositorio();
            public readonly FakeCategoriaRepositorio CategoriaRepo = new FakeCategoriaRepositorio();
            public readonly FakeComercioRepositorio ComercioRepo = new FakeComercioRepositorio();
            public readonly SpyBitacoraBLL Bitacora = new SpyBitacoraBLL();
            public readonly FakeSesion Sesion = new FakeSesion();
            public readonly FakeUnidadDeTrabajo Unidad;
            public readonly FakeUnidadDeTrabajoFactory UnidadFactory;
            public readonly Usuario UsuarioActual;

            public ReglaFixture(int usuarioActualId)
            {
                Unidad = new FakeUnidadDeTrabajo
                {
                    ReglaCategorizacionRepo = ReglaRepo,
                    ComercioReglaCategorizacionRepo = ComercioReglaRepo,
                    CategoriaRepo = CategoriaRepo,
                    ComercioRepo = ComercioRepo
                };
                UnidadFactory = new FakeUnidadDeTrabajoFactory(Unidad);
                UsuarioActual = new Usuario { Id = usuarioActualId, Username = "test" };
                Sesion.IniciarSesion(UsuarioActual);
            }

            public ReglaCategorizacionBLL CreateSut()
            {
                return new ReglaCategorizacionBLL(
                    ReglaRepo,
                    ComercioReglaRepo,
                    CategoriaRepo,
                    ComercioRepo,
                    UnidadFactory,
                    Sesion,
                    Bitacora);
            }
        }
    }
}
