using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ABSTRACCIONES;
using BE;
using BLL.Tests.TestDoubles;
using SERVICIOS;

namespace BLL.Tests
{
    [TestClass]
    public sealed class PermisoBLLTests
    {
        [TestMethod]
        public void ObtenerCatalogo_RepoData_ReturnsCompositeTree()
        {
            PermisoFixture fixture = new PermisoFixture();
            fixture.PermisoRepo.Nodos.Add(Grupo(1, "admin"));
            fixture.PermisoRepo.Nodos.Add(Simple(2, "usuarios"));
            fixture.PermisoRepo.Relaciones.Add(Tuple.Create(1, 2));
            PermisoBLL sut = fixture.CreateSut();

            List<IPermiso> result = sut.ObtenerCatalogo();

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(1, result[0].Hijos.Count);
            Assert.AreEqual(2, result[0].Hijos[0].Id);
        }

        [TestMethod]
        public void GuardarCatalogo_NullOrEmpty_ThrowsExcepcionNegocio()
        {
            PermisoFixture fixture = new PermisoFixture();
            PermisoBLL sut = fixture.CreateSut();

            Assert.ThrowsExactly<ExcepcionNegocio>(() => sut.GuardarCatalogo(new List<IPermiso>()));
            Assert.AreEqual(0, fixture.UnidadFactory.CrearCallCount);
        }

        [TestMethod]
        public void GuardarCatalogo_InvalidName_ThrowsExcepcionNegocio()
        {
            PermisoFixture fixture = new PermisoFixture();
            PermisoBLL sut = fixture.CreateSut();

            Assert.ThrowsExactly<ExcepcionNegocio>(() => sut.GuardarCatalogo(new List<IPermiso> { Simple(1, "usuarios.ver") }));
            Assert.AreEqual(0, fixture.UnidadFactory.CrearCallCount);
        }

        [TestMethod]
        public void GuardarCatalogo_ValidCatalog_NormalizesNamesDeletesMissingAndSaves()
        {
            PermisoFixture fixture = new PermisoFixture();
            fixture.Unidad.PermisoRepo.Nodos.Add(Simple(99, "obsolete"));
            IPermiso root = Grupo(0, " Admin ");
            IPermiso child = Simple(0, " Usuarios ");
            root.Agregar(child);
            PermisoBLL sut = fixture.CreateSut();

            sut.GuardarCatalogo(new List<IPermiso> { root });

            Assert.AreEqual("admin", root.Nombre);
            Assert.AreEqual("usuarios", child.Nombre);
            Assert.IsTrue(fixture.Unidad.PermisoRepo.Guardados.Count >= 2);
            Assert.IsTrue(fixture.Unidad.PermisoRepo.RelacionesReemplazadas.Count > 0);
            Assert.IsTrue(fixture.Unidad.PermisoRepo.IdsEliminados.Contains(99));
            Assert.AreEqual("OperacionInterno", fixture.Bitacora.Calls.Single().Kind);
            Assert.IsTrue(fixture.Unidad.SaveCalled);
        }

        [TestMethod]
        public void GuardarCatalogo_RepoException_WrapsInExcepcionTecnica()
        {
            PermisoFixture fixture = new PermisoFixture();
            fixture.Unidad.PermisoRepo.ThrowOnGuardar = true;
            IPermiso root = Grupo(0, "admin");
            root.Agregar(Simple(0, "usuarios"));
            PermisoBLL sut = fixture.CreateSut();

            Assert.ThrowsExactly<ExcepcionTecnica>(() => sut.GuardarCatalogo(new List<IPermiso> { root }));
            Assert.IsFalse(fixture.Unidad.SaveCalled);
        }

        [TestMethod]
        public void GuardarPermisosUsuario_UserNotFound_ThrowsExcepcionNegocio()
        {
            PermisoFixture fixture = new PermisoFixture();
            PermisoBLL sut = fixture.CreateSut();

            Assert.ThrowsExactly<ExcepcionNegocio>(() => sut.GuardarPermisosUsuario(1, new List<int> { 2 }));
            Assert.IsFalse(fixture.Unidad.SaveCalled);
        }

        [TestMethod]
        public void GuardarPermisosUsuario_NullPermissions_ReplacesWithEmptyList()
        {
            PermisoFixture fixture = new PermisoFixture();
            fixture.Unidad.UsuarioRepo.Usuarios[1] = new Usuario { Id = 1, Username = "admin" };
            PermisoBLL sut = fixture.CreateSut();

            sut.GuardarPermisosUsuario(1, null);

            Assert.AreEqual(0, fixture.Unidad.UsuarioPermisoRepo.LastPermisos.Count);
            Assert.AreEqual(1, fixture.UsuarioEstado.GuardarUsuarioId);
            Assert.IsTrue(fixture.Unidad.SaveCalled);
        }

        [TestMethod]
        public void GuardarPermisosUsuario_CurrentUser_RefreshesAuthorization()
        {
            PermisoFixture fixture = new PermisoFixture();
            fixture.Unidad.UsuarioRepo.Usuarios[1] = new Usuario { Id = 1, Username = "admin" };
            fixture.UsuarioPermisoRepo.PermisosPorUsuario[1] = new List<int> { 5 };
            fixture.PermisoRepo.Nodos.Add(Simple(5, "usuarios"));
            fixture.Sesion.IniciarSesion(new Usuario { Id = 1, Username = "admin" });
            PermisoBLL sut = fixture.CreateSut();

            sut.GuardarPermisosUsuario(1, new List<int> { 5 });

            Assert.AreEqual(1, fixture.Autorizacion.ActualizarPermisosCallCount);
        }

        [TestMethod]
        public void GuardarPermisosUsuario_QuitaUnicaCoberturaDirectaDeUnaPatente_ThrowsExcepcionNegocio()
        {
            PermisoFixture fixture = new PermisoFixture();
            fixture.Unidad.UsuarioRepo.Usuarios[1] = new Usuario { Id = 1, Username = "un" };
            fixture.Unidad.PermisoRepo.Nodos.Add(Simple(5, "patentex"));
            fixture.Unidad.UsuarioPermisoRepo.PermisosPorUsuario[1] = new List<int> { 5 };
            PermisoBLL sut = fixture.CreateSut();

            Assert.ThrowsExactly<ExcepcionNegocio>(() => sut.GuardarPermisosUsuario(1, new List<int>()));
            Assert.IsFalse(fixture.Unidad.SaveCalled);
        }

        [TestMethod]
        public void GuardarPermisosUsuario_OtroUsuarioActivoCubreLaPatente_Permite()
        {
            PermisoFixture fixture = new PermisoFixture();
            fixture.Unidad.UsuarioRepo.Usuarios[1] = new Usuario { Id = 1, Username = "un" };
            fixture.Unidad.UsuarioRepo.Usuarios[2] = new Usuario { Id = 2, Username = "un2" };
            fixture.Unidad.PermisoRepo.Nodos.Add(Simple(5, "patentex"));
            fixture.Unidad.UsuarioPermisoRepo.PermisosPorUsuario[1] = new List<int> { 5 };
            fixture.Unidad.UsuarioPermisoRepo.PermisosPorUsuario[2] = new List<int> { 5 };
            PermisoBLL sut = fixture.CreateSut();

            sut.GuardarPermisosUsuario(1, new List<int>());

            Assert.IsTrue(fixture.Unidad.SaveCalled);
        }

        [TestMethod]
        public void GuardarPermisosUsuario_PatenteSigueCubiertaPorFamilia_Permite()
        {
            PermisoFixture fixture = new PermisoFixture();
            fixture.Unidad.UsuarioRepo.Usuarios[1] = new Usuario { Id = 1, Username = "un" };
            IPermiso familia = Grupo(10, "nf");
            IPermiso patenteX = Simple(5, "patentex");
            familia.Agregar(patenteX);
            fixture.Unidad.PermisoRepo.Nodos.Add(familia);
            fixture.Unidad.PermisoRepo.Nodos.Add(patenteX);
            fixture.Unidad.PermisoRepo.Relaciones.Add(Tuple.Create(10, 5));
            fixture.Unidad.UsuarioPermisoRepo.PermisosPorUsuario[1] = new List<int> { 5, 10 };
            PermisoBLL sut = fixture.CreateSut();

            sut.GuardarPermisosUsuario(1, new List<int> { 10 });

            Assert.IsTrue(fixture.Unidad.SaveCalled);
        }

        [TestMethod]
        public void GuardarCatalogo_BorrarFamiliaUnicaCoberturaDeUnUsuario_ThrowsExcepcionNegocio()
        {
            PermisoFixture fixture = new PermisoFixture();
            IPermiso familiaExistente = Grupo(10, "nf");
            IPermiso patenteXExistente = Simple(5, "patentex");
            familiaExistente.Agregar(patenteXExistente);
            fixture.Unidad.PermisoRepo.Nodos.Add(familiaExistente);
            fixture.Unidad.PermisoRepo.Nodos.Add(patenteXExistente);
            fixture.Unidad.PermisoRepo.Relaciones.Add(Tuple.Create(10, 5));
            fixture.Unidad.UsuarioRepo.Usuarios[1] = new Usuario { Id = 1, Username = "un" };
            fixture.Unidad.UsuarioPermisoRepo.PermisosPorUsuario[1] = new List<int> { 10 };
            PermisoBLL sut = fixture.CreateSut();

            Assert.ThrowsExactly<ExcepcionNegocio>(() => sut.GuardarCatalogo(new List<IPermiso> { Simple(5, "patentex") }));
            Assert.IsFalse(fixture.Unidad.SaveCalled);
        }

        [TestMethod]
        public void GuardarCatalogo_BorrarFamiliaSinDependientes_Permite()
        {
            PermisoFixture fixture = new PermisoFixture();
            fixture.Unidad.PermisoRepo.Nodos.Add(Grupo(10, "nf"));
            fixture.Unidad.PermisoRepo.Nodos.Add(Simple(5, "patentex"));
            fixture.Unidad.UsuarioRepo.Usuarios[1] = new Usuario { Id = 1, Username = "un" };
            fixture.Unidad.UsuarioPermisoRepo.PermisosPorUsuario[1] = new List<int> { 5 };
            PermisoBLL sut = fixture.CreateSut();

            sut.GuardarCatalogo(new List<IPermiso> { Simple(5, "patentex") });

            Assert.IsTrue(fixture.Unidad.SaveCalled);
        }

        [TestMethod]
        public void ValidarPuedeEliminarUsuario_EsUnicoTitularDeAlgunaPatente_ThrowsExcepcionNegocio()
        {
            PermisoFixture fixture = new PermisoFixture();
            fixture.Unidad.PermisoRepo.Nodos.Add(Simple(5, "patentex"));
            fixture.Unidad.UsuarioRepo.Usuarios[1] = new Usuario { Id = 1, Username = "un" };
            fixture.Unidad.UsuarioPermisoRepo.PermisosPorUsuario[1] = new List<int> { 5 };
            PermisoBLL sut = fixture.CreateSut();

            Assert.ThrowsExactly<ExcepcionNegocio>(() => sut.ValidarPuedeEliminarUsuario(1, fixture.Unidad));
        }

        [TestMethod]
        public void ValidarPuedeEliminarUsuario_OtroUsuarioActivoCubreSusPatentes_NoLanza()
        {
            PermisoFixture fixture = new PermisoFixture();
            fixture.Unidad.PermisoRepo.Nodos.Add(Simple(5, "patentex"));
            fixture.Unidad.UsuarioRepo.Usuarios[1] = new Usuario { Id = 1, Username = "un" };
            fixture.Unidad.UsuarioRepo.Usuarios[2] = new Usuario { Id = 2, Username = "un2" };
            fixture.Unidad.UsuarioPermisoRepo.PermisosPorUsuario[1] = new List<int> { 5 };
            fixture.Unidad.UsuarioPermisoRepo.PermisosPorUsuario[2] = new List<int> { 5 };
            PermisoBLL sut = fixture.CreateSut();

            sut.ValidarPuedeEliminarUsuario(1, fixture.Unidad);
        }

        [TestMethod]
        public void AsignarPerfilBasicoInterno_BasicoExists_AssignsOnlyBasico()
        {
            PermisoFixture fixture = new PermisoFixture();
            fixture.Unidad.PermisoRepo.IdsPorNombre["basico"] = 7;
            PermisoBLL sut = fixture.CreateSut();

            sut.AsignarPerfilBasicoInterno(1, fixture.Unidad);

            CollectionAssert.AreEqual(new List<int> { 7 }, fixture.Unidad.UsuarioPermisoRepo.LastPermisos);
        }

        [TestMethod]
        public void GuardarPermisosUsuarioInterno_DuplicateIds_SavesDistinctIds()
        {
            PermisoFixture fixture = new PermisoFixture();
            PermisoBLL sut = fixture.CreateSut();

            sut.GuardarPermisosUsuarioInterno(1, new List<int> { 5, 5, 6 }, fixture.Unidad);

            CollectionAssert.AreEqual(new List<int> { 5, 6 }, fixture.Unidad.UsuarioPermisoRepo.LastPermisos);
        }

        private static IPermiso Grupo(int id, string name)
        {
            return new PermisoGrupo { Id = id, Nombre = name };
        }

        private static IPermiso Simple(int id, string name)
        {
            return new PermisoSimple { Id = id, Nombre = name };
        }

        private sealed class PermisoFixture
        {
            public readonly FakePermisoRepositorio PermisoRepo = new FakePermisoRepositorio();
            public readonly FakeUsuarioPermisoRepositorio UsuarioPermisoRepo = new FakeUsuarioPermisoRepositorio();
            public readonly FakeUsuarioRepositorio UsuarioRepo = new FakeUsuarioRepositorio();
            public readonly SpyUsuarioEstadoBLL UsuarioEstado = new SpyUsuarioEstadoBLL();
            public readonly SpyBitacoraBLL Bitacora = new SpyBitacoraBLL();
            public readonly FakeUnidadDeTrabajo Unidad = new FakeUnidadDeTrabajo();
            public readonly FakeSesion Sesion = new FakeSesion();
            public readonly FakeAutorizacion Autorizacion = new FakeAutorizacion();
            public readonly FakeUnidadDeTrabajoFactory UnidadFactory;

            public PermisoFixture()
            {
                UnidadFactory = new FakeUnidadDeTrabajoFactory(Unidad);
            }

            public PermisoBLL CreateSut()
            {
                return new PermisoBLL(PermisoRepo, UsuarioPermisoRepo, new PermisoService(), UsuarioRepo, UsuarioEstado, Bitacora, UnidadFactory, Sesion, Autorizacion);
            }
        }
    }
}
