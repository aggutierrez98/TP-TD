using System.Linq;
using BE;
using BLL.Tests.TestDoubles;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SERVICIOS;

namespace BLL.Tests
{
    [TestClass]
    public sealed class ListaDeComprasBLLTests
    {
        [TestMethod]
        public void ObtenerOCrearParaUsuarioActual_ListaExistente_DevuelveLaExistenteSinCrearUow()
        {
            ListaDeComprasFixture fixture = new ListaDeComprasFixture(usuarioActualId: 1);
            fixture.ListaRepo.Listas.Add(1, new ListaDeCompras { Id = 1, Nombre = "Mi lista de compras", Usuario = new Usuario { Id = 1 } });
            ListaDeComprasBLL sut = fixture.CreateSut();

            ListaDeCompras lista = sut.ObtenerOCrearParaUsuarioActual();

            Assert.AreEqual(1, lista.Id);
            Assert.AreEqual(0, fixture.UnidadFactory.CrearCallCount);
        }

        [TestMethod]
        public void ObtenerOCrearParaUsuarioActual_SinLista_CreaNuevaAuditaYGuarda()
        {
            ListaDeComprasFixture fixture = new ListaDeComprasFixture(usuarioActualId: 7);
            ListaDeComprasBLL sut = fixture.CreateSut();

            ListaDeCompras lista = sut.ObtenerOCrearParaUsuarioActual();

            Assert.IsTrue(lista.Id > 0);
            Assert.AreEqual("Mi lista de compras", lista.Nombre);
            Assert.AreEqual(7, lista.Usuario.Id);
            Assert.AreEqual("OperacionInterno", fixture.Bitacora.Calls.Single().Kind);
            Assert.IsTrue(fixture.Unidad.SaveCalled);
        }

        [TestMethod]
        public void AgregarItem_DescripcionVacia_ThrowsExcepcionNegocio()
        {
            ListaDeComprasFixture fixture = new ListaDeComprasFixture(usuarioActualId: 1);
            ListaDeComprasBLL sut = fixture.CreateSut();

            Assert.ThrowsExactly<ExcepcionNegocio>(() => sut.AgregarItem(new ItemLista { ListaDeCompras = new ListaDeCompras { Id = 1 }, Descripcion = "   ", Cantidad = 2 }));
        }

        [TestMethod]
        public void AgregarItem_CantidadInvalida_ThrowsExcepcionNegocio()
        {
            ListaDeComprasFixture fixture = new ListaDeComprasFixture(usuarioActualId: 1);
            ListaDeComprasBLL sut = fixture.CreateSut();

            Assert.ThrowsExactly<ExcepcionNegocio>(() => sut.AgregarItem(new ItemLista { ListaDeCompras = new ListaDeCompras { Id = 1 }, Descripcion = "Leche", Cantidad = 0 }));
        }

        [TestMethod]
        public void AgregarItem_ListaInexistente_ThrowsExcepcionNegocio()
        {
            ListaDeComprasFixture fixture = new ListaDeComprasFixture(usuarioActualId: 1);
            ListaDeComprasBLL sut = fixture.CreateSut();

            Assert.ThrowsExactly<ExcepcionNegocio>(() => sut.AgregarItem(new ItemLista { ListaDeCompras = new ListaDeCompras { Id = 99 }, Descripcion = "Leche", Cantidad = 1 }));
        }

        [TestMethod]
        public void AgregarItem_ListaDeOtroUsuario_ThrowsExcepcionNegocio()
        {
            ListaDeComprasFixture fixture = new ListaDeComprasFixture(usuarioActualId: 1);
            fixture.Unidad.ListaDeComprasRepo.Listas.Add(2, new ListaDeCompras { Id = 2, Usuario = new Usuario { Id = 99 } });
            ListaDeComprasBLL sut = fixture.CreateSut();

            Assert.ThrowsExactly<ExcepcionNegocio>(() => sut.AgregarItem(new ItemLista { ListaDeCompras = new ListaDeCompras { Id = 2, Usuario = new Usuario { Id = 1 } }, Descripcion = "Leche", Cantidad = 1 }));
        }

        [TestMethod]
        public void AgregarItem_Valido_InsertaAuditaYGuarda()
        {
            ListaDeComprasFixture fixture = new ListaDeComprasFixture(usuarioActualId: 5);
            fixture.Unidad.ListaDeComprasRepo.Listas.Add(3, new ListaDeCompras { Id = 3, Usuario = new Usuario { Id = 5 } });
            ListaDeComprasBLL sut = fixture.CreateSut();

            ItemLista recibido = new ItemLista { ListaDeCompras = new ListaDeCompras { Id = 3 }, Descripcion = "  Leche descremada 1L  ", Cantidad = 2 };
            ItemLista item = sut.AgregarItem(recibido);
            Assert.AreSame(recibido, item);

            ItemLista insertado = fixture.Unidad.ItemListaRepo.Items[item.Id];
            Assert.AreEqual("Leche descremada 1L", insertado.Descripcion);
            Assert.AreEqual(2, insertado.Cantidad);
            Assert.IsFalse(insertado.Comprado);
            Assert.AreEqual(3, insertado.ListaDeCompras.Id);
            Assert.AreEqual("OperacionInterno", fixture.Bitacora.Calls.Single().Kind);
            Assert.IsTrue(fixture.Unidad.SaveCalled);
        }

        [TestMethod]
        public void ActualizarCantidad_CantidadInvalida_ThrowsExcepcionNegocio()
        {
            ListaDeComprasFixture fixture = new ListaDeComprasFixture(usuarioActualId: 1);
            ListaDeComprasBLL sut = fixture.CreateSut();

            Assert.ThrowsExactly<ExcepcionNegocio>(() => sut.ActualizarCantidad(new ItemLista { Id = 1 }, -1));
        }

        [TestMethod]
        public void ActualizarCantidad_ItemInexistente_ThrowsExcepcionNegocio()
        {
            ListaDeComprasFixture fixture = new ListaDeComprasFixture(usuarioActualId: 1);
            ListaDeComprasBLL sut = fixture.CreateSut();

            Assert.ThrowsExactly<ExcepcionNegocio>(() => sut.ActualizarCantidad(new ItemLista { Id = 99 }, 3));
        }

        [TestMethod]
        public void ActualizarCantidad_ItemDeOtroUsuario_ThrowsExcepcionNegocio()
        {
            ListaDeComprasFixture fixture = new ListaDeComprasFixture(usuarioActualId: 1);
            fixture.Unidad.ItemListaRepo.Items.Add(4, new ItemLista { Id = 4, Descripcion = "Ajeno", Cantidad = 1, ListaDeCompras = new ListaDeCompras { Id = 10, Usuario = new Usuario { Id = 99 } } });
            ListaDeComprasBLL sut = fixture.CreateSut();

            Assert.ThrowsExactly<ExcepcionNegocio>(() => sut.ActualizarCantidad(new ItemLista { Id = 4, ListaDeCompras = new ListaDeCompras { Usuario = new Usuario { Id = 1 } } }, 3));
        }

        [TestMethod]
        public void ActualizarCantidad_Valido_ActualizaAuditaYGuarda()
        {
            ListaDeComprasFixture fixture = new ListaDeComprasFixture(usuarioActualId: 5);
            fixture.Unidad.ItemListaRepo.Items.Add(6, new ItemLista { Id = 6, Descripcion = "Pan", Cantidad = 1, Comprado = false, ListaDeCompras = new ListaDeCompras { Id = 3, Usuario = new Usuario { Id = 5 } } });
            ListaDeComprasBLL sut = fixture.CreateSut();

            bool resultado = sut.ActualizarCantidad(new ItemLista { Id = 6 }, 8);

            Assert.IsTrue(resultado);
            Assert.AreEqual(8, fixture.Unidad.ItemListaRepo.Items[6].Cantidad);
            Assert.AreEqual("OperacionInterno", fixture.Bitacora.Calls.Single().Kind);
            Assert.IsTrue(fixture.Unidad.SaveCalled);
        }

        [TestMethod]
        public void MarcarComprado_ItemDeOtroUsuario_ThrowsExcepcionNegocio()
        {
            ListaDeComprasFixture fixture = new ListaDeComprasFixture(usuarioActualId: 1);
            fixture.Unidad.ItemListaRepo.Items.Add(7, new ItemLista { Id = 7, Descripcion = "Ajeno", Cantidad = 1, ListaDeCompras = new ListaDeCompras { Id = 10, Usuario = new Usuario { Id = 99 } } });
            ListaDeComprasBLL sut = fixture.CreateSut();

            Assert.ThrowsExactly<ExcepcionNegocio>(() => sut.MarcarComprado(new ItemLista { Id = 7, ListaDeCompras = new ListaDeCompras { Usuario = new Usuario { Id = 1 } } }, true));
        }

        [TestMethod]
        public void MarcarComprado_Valido_ActualizaAuditaYGuarda()
        {
            ListaDeComprasFixture fixture = new ListaDeComprasFixture(usuarioActualId: 5);
            fixture.Unidad.ItemListaRepo.Items.Add(8, new ItemLista { Id = 8, Descripcion = "Fideos", Cantidad = 1, Comprado = false, ListaDeCompras = new ListaDeCompras { Id = 3, Usuario = new Usuario { Id = 5 } } });
            ListaDeComprasBLL sut = fixture.CreateSut();

            bool resultado = sut.MarcarComprado(new ItemLista { Id = 8 }, true);

            Assert.IsTrue(resultado);
            Assert.IsTrue(fixture.Unidad.ItemListaRepo.Items[8].Comprado);
            Assert.AreEqual("OperacionInterno", fixture.Bitacora.Calls.Single().Kind);
            Assert.IsTrue(fixture.Unidad.SaveCalled);
        }

        [TestMethod]
        public void EliminarItem_ItemDeOtroUsuario_ThrowsExcepcionNegocio()
        {
            ListaDeComprasFixture fixture = new ListaDeComprasFixture(usuarioActualId: 1);
            fixture.Unidad.ItemListaRepo.Items.Add(9, new ItemLista { Id = 9, Descripcion = "Ajeno", Cantidad = 1, ListaDeCompras = new ListaDeCompras { Id = 10, Usuario = new Usuario { Id = 99 } } });
            ListaDeComprasBLL sut = fixture.CreateSut();

            Assert.ThrowsExactly<ExcepcionNegocio>(() => sut.EliminarItem(new ItemLista { Id = 9, ListaDeCompras = new ListaDeCompras { Usuario = new Usuario { Id = 1 } } }));
        }

        [TestMethod]
        public void EliminarItem_Valido_EliminaAuditaYGuarda()
        {
            ListaDeComprasFixture fixture = new ListaDeComprasFixture(usuarioActualId: 5);
            fixture.Unidad.ItemListaRepo.Items.Add(10, new ItemLista { Id = 10, Descripcion = "Yerba", Cantidad = 1, ListaDeCompras = new ListaDeCompras { Id = 3, Usuario = new Usuario { Id = 5 } } });
            ListaDeComprasBLL sut = fixture.CreateSut();

            bool resultado = sut.EliminarItem(new ItemLista { Id = 10 });

            Assert.IsTrue(resultado);
            Assert.IsFalse(fixture.Unidad.ItemListaRepo.Items.ContainsKey(10));
            Assert.AreEqual("OperacionInterno", fixture.Bitacora.Calls.Single().Kind);
            Assert.IsTrue(fixture.Unidad.SaveCalled);
        }

        private sealed class ListaDeComprasFixture
        {
            public readonly FakeListaDeComprasRepositorio ListaRepo = new FakeListaDeComprasRepositorio();
            public readonly SpyBitacoraBLL Bitacora = new SpyBitacoraBLL();
            public readonly FakeSesion Sesion = new FakeSesion();
            public readonly FakeUnidadDeTrabajo Unidad;
            public readonly FakeUnidadDeTrabajoFactory UnidadFactory;

            public ListaDeComprasFixture(int usuarioActualId)
            {
                Unidad = new FakeUnidadDeTrabajo { ListaDeComprasRepo = ListaRepo };
                UnidadFactory = new FakeUnidadDeTrabajoFactory(Unidad);
                Sesion.IniciarSesion(new Usuario { Id = usuarioActualId, Username = "test" });
            }

            public ListaDeComprasBLL CreateSut()
            {
                return new ListaDeComprasBLL(ListaRepo, UnidadFactory, Sesion, Bitacora);
            }
        }
    }
}
