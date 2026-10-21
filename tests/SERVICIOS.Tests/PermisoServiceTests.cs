using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ABSTRACCIONES;
using SERVICIOS;

namespace SERVICIOS.Tests
{
    [TestClass]
    public sealed class PermisoServiceTests
    {
        private readonly PermisoService _service = new PermisoService();

        [TestMethod]
        public void ConstruirArbol_ValidNodesAndRelations_ReturnsRootNodes()
        {
            IPermiso root = Grupo(1, "admin");
            IPermiso child = Simple(2, "usuarios.ver");
            List<IPermiso> nodes = new List<IPermiso> { root, child };

            List<IPermiso> roots = _service.ConstruirArbol(nodes, new List<Tuple<int, int>> { Tuple.Create(1, 2) });

            Assert.AreEqual(1, roots.Count);
            Assert.AreSame(root, roots[0]);
            Assert.AreSame(child, root.Hijos[0]);
        }

        [TestMethod]
        public void ConstruirArbol_MissingRelationNode_IgnoresInvalidRelation()
        {
            IPermiso root = Grupo(1, "admin");
            IPermiso child = Simple(2, "usuarios.ver");
            IPermiso standalone = Simple(3, "reportes.ver");

            List<IPermiso> roots = _service.ConstruirArbol(
                new List<IPermiso> { root, child, standalone },
                new List<Tuple<int, int>> { Tuple.Create(1, 2), Tuple.Create(1, 99) });

            Assert.AreEqual(2, roots.Count);
            Assert.AreEqual(1, root.Hijos.Count);
            Assert.AreSame(child, root.Hijos[0]);
            Assert.IsTrue(roots.Contains(standalone));
        }

        [TestMethod]
        public void Aplanar_ReusedGroup_DoesNotDuplicateById()
        {
            IPermiso shared = Grupo(10, "shared");
            IPermiso firstRoot = Grupo(1, "first");
            IPermiso secondRoot = Grupo(2, "second");
            firstRoot.Agregar(shared);
            secondRoot.Agregar(shared);

            List<IPermiso> flattened = _service.Aplanar(new[] { firstRoot, secondRoot });

            Assert.AreEqual(1, flattened.Count(item => item.Id == shared.Id));
        }

        [TestMethod]
        public void ObtenerRelaciones_ReusedNode_DoesNotDuplicateRelation()
        {
            IPermiso root = Grupo(1, "admin");
            root.Agregar(Simple(2, "usuarios.ver"));

            List<Tuple<int, int>> relations = _service.ObtenerRelaciones(new[] { root, root });

            Assert.AreEqual(1, relations.Count(item => item.Item1 == 1 && item.Item2 == 2));
        }

        [TestMethod]
        public void ValidarSinCiclos_CyclicGraph_ThrowsExcepcionNegocio()
        {
            IPermiso first = Grupo(1, "first");
            IPermiso second = Grupo(2, "second");
            first.Agregar(second);
            second.Agregar(first);

            Assert.ThrowsExactly<ExcepcionNegocio>(() => _service.ValidarSinCiclos(new[] { first }));
        }

        [TestMethod]
        public void ValidarSinCiclos_CyclicGraphWithUnsavedNodes_ThrowsExcepcionNegocio()
        {
            IPermiso first = Grupo(0, "first");
            IPermiso second = Grupo(0, "second");
            first.Agregar(second);
            second.Agregar(first);

            Assert.ThrowsExactly<ExcepcionNegocio>(() => _service.ValidarSinCiclos(new[] { first }));
        }

        [TestMethod]
        public void ValidarSinCiclos_SharedGroupInTwoBranches_DoesNotThrow()
        {
            IPermiso shared = Grupo(10, "shared");
            shared.Agregar(Simple(11, "usuarios.ver"));
            IPermiso root = Grupo(1, "root");
            IPermiso branch = Grupo(2, "branch");
            root.Agregar(shared);
            root.Agregar(branch);
            branch.Agregar(shared);

            _service.ValidarSinCiclos(new[] { root });
        }

        [TestMethod]
        public void GeneraCiclo_ParentReachableFromCandidateChild_ReturnsTrue()
        {
            IPermiso first = Grupo(0, "first");
            IPermiso second = Grupo(0, "second");
            second.Agregar(first);

            Assert.IsTrue(_service.GeneraCiclo(first, second));
        }

        [TestMethod]
        public void GeneraCiclo_UnrelatedGroups_ReturnsFalse()
        {
            IPermiso first = Grupo(1, "first");
            IPermiso second = Grupo(2, "second");
            second.Agregar(Simple(3, "usuarios.ver"));

            Assert.IsFalse(_service.GeneraCiclo(first, second));
        }

        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow("   ")]
        public void ValidarNombre_NullOrWhitespace_ThrowsExcepcionNegocio(string name)
        {
            Assert.ThrowsExactly<ExcepcionNegocio>(() => _service.ValidarNombre(name));
        }

        [TestMethod]
        public void ValidarNombre_NameWithDot_ThrowsExcepcionNegocio()
        {
            Assert.ThrowsExactly<ExcepcionNegocio>(() => _service.ValidarNombre("usuarios.ver"));
        }

        [TestMethod]
        public void NormalizarNombre_MixedCaseAndSpaces_ReturnsTrimLowerInvariant()
        {
            Assert.AreEqual("admin", _service.NormalizarNombre("  AdMiN  "));
        }

        [TestMethod]
        public void ValidarNombresUnicos_DuplicateSiblings_ThrowsExcepcionNegocio()
        {
            IPermiso first = Simple(1, "admin");
            IPermiso second = Simple(2, " ADMIN ");

            Assert.ThrowsExactly<ExcepcionNegocio>(() => _service.ValidarNombresUnicos(new[] { first, second }));
        }

        [TestMethod]
        public void ValidarNombresUnicos_SameNameInDifferentBranches_ThrowsExcepcionNegocio()
        {
            IPermiso firstRoot = Grupo(1, "first");
            IPermiso secondRoot = Grupo(2, "second");
            firstRoot.Agregar(Simple(3, "ver"));
            secondRoot.Agregar(Simple(4, "ver"));

            Assert.ThrowsExactly<ExcepcionNegocio>(() => _service.ValidarNombresUnicos(new[] { firstRoot, secondRoot }));
        }

        [TestMethod]
        public void ValidarGruposNoVacios_EmptyGroup_ThrowsExcepcionNegocio()
        {
            Assert.ThrowsExactly<ExcepcionNegocio>(() => _service.ValidarGruposNoVacios(new[] { Grupo(1, "empty") }));
        }

        [TestMethod]
        public void ObtenerPermisosEfectivos_GroupWithLeaves_ReturnsLeavesOnly()
        {
            IPermiso group = Grupo(1, "admin");
            IPermiso leaf = Simple(2, "usuarios");
            group.Agregar(leaf);

            List<IPermiso> effective = _service.ObtenerPermisosEfectivos(new[] { group });

            Assert.AreEqual(1, effective.Count);
            Assert.AreSame(leaf, effective[0]);
        }

        [TestMethod]
        public void ObtenerPermisosEfectivos_RepeatedLeaf_ReturnsOnce()
        {
            IPermiso firstGroup = Grupo(1, "first");
            IPermiso secondGroup = Grupo(2, "second");
            IPermiso leaf = Simple(3, "usuarios");
            firstGroup.Agregar(leaf);
            secondGroup.Agregar(leaf);

            List<IPermiso> effective = _service.ObtenerPermisosEfectivos(new[] { firstGroup, secondGroup });

            Assert.AreEqual(1, effective.Count(item => item.Id == leaf.Id));
        }

        [TestMethod]
        public void PermisoGrupo_Agregar_DuplicateById_DoesNotDuplicate()
        {
            IPermiso group = Grupo(1, "admin");

            group.Agregar(Simple(2, "usuarios"));
            group.Agregar(Simple(2, "usuarios.duplicado"));

            Assert.AreEqual(1, group.Hijos.Count);
        }

        [TestMethod]
        public void PermisoGrupo_Quitar_ExistingChild_RemovesIt()
        {
            IPermiso group = Grupo(1, "admin");
            IPermiso child = Simple(2, "usuarios");
            group.Agregar(child);

            group.Quitar(child);

            Assert.AreEqual(0, group.Hijos.Count);
        }

        [TestMethod]
        public void PermisoSimple_Agregar_DoesNothing()
        {
            IPermiso permission = Simple(1, "usuarios");

            permission.Agregar(Simple(2, "other"));

            Assert.AreEqual(0, permission.Hijos.Count);
        }

        [TestMethod]
        public void PermisoSimple_ObtenerPermisosHoja_ReturnsItself()
        {
            IPermiso permission = Simple(1, "usuarios");

            IList<IPermiso> leaves = permission.ObtenerPermisosHoja();

            Assert.AreEqual(1, leaves.Count);
            Assert.AreSame(permission, leaves[0]);
        }

        [TestMethod]
        public void PermisoSimple_Contiene_ComparaPorIdentidadPersistida()
        {
            IPermiso permission = Simple(1, "usuarios");

            Assert.IsTrue(permission.Contiene(Simple(1, "otro")));
            Assert.IsFalse(permission.Contiene(Simple(2, "usuarios")));
            Assert.IsFalse(permission.Contiene(null));
        }

        [TestMethod]
        public void PermisoGrupo_Contiene_BuscaEnSiMismoYEnSusHijos()
        {
            IPermiso root = Grupo(1, "admin");
            IPermiso familia = Grupo(2, "familia");
            familia.Agregar(Simple(3, "usuarios"));
            root.Agregar(familia);

            Assert.IsTrue(root.Contiene(Grupo(1, "admin")));
            Assert.IsTrue(root.Contiene(Simple(3, "usuarios")));
            Assert.IsFalse(root.Contiene(Simple(4, "bitacora")));
            Assert.IsFalse(root.Contiene(null));
        }

        private static IPermiso Grupo(int id, string name)
        {
            return new PermisoGrupo { Id = id, Nombre = name };
        }

        private static IPermiso Simple(int id, string name)
        {
            return new PermisoSimple { Id = id, Nombre = name };
        }
    }
}
