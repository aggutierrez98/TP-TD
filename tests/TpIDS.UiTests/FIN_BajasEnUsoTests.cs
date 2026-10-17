using System;
using System.Data.SqlClient;
using FlaUI.Core.AutomationElements;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TpIDS.UiTests.Infrastructure;

namespace TpIDS.UiTests
{
    /// <summary>
    /// Change retirar-helper-violacion-fk: las bajas bloqueadas por uso se deciden con una consulta
    /// previa al borrado y se informan con el mensaje propio de cada entidad. Se cubren los casos
    /// clave por pantalla (comercio, categoria de producto y medio de pago de una entidad); el
    /// resto de las bajas esta cubierto por los tests unitarios de cada BLL. Los datos previos se
    /// arman por SQL directo y cada test deja nombres unicos.
    /// </summary>
    [TestClass]
    public class FIN_BajasEnUsoTests : UiTestBase
    {
        private const string TipoMedioPagoCredito = "TarjetaCredito";
        private const string TipoCuentaCajaAhorro = "CajaAhorro";

        private static readonly string TextoMenuOperador = TestDatabase.ObtenerTraduccion("FormMainShell.Menu.Operator");

        [TestMethod]
        [TestCategory("UI")]
        public void FIN_BajaEnUso_ComercioConProductoAsignado_InformaEnUsoYNoElimina()
        {
            string nombreComercio = NombreUnico("uicomuso");
            int comercioId = InsertarComercio(nombreComercio);
            int productoId = InsertarProducto(NombreUnico("uiprod"), InsertarCategoriaProducto(NombreUnico("uicat")));
            AsignarProductoAComercio(comercioId, productoId, 10m);

            Window principal = LoginComo(TestConfig.OperadorUsername, TestConfig.OperadorPassword);
            UiFind.ClickMenu(principal, TextoMenuOperador, TestDatabase.ObtenerTraduccion("FormMainShell.Menu.Merchants"));
            Window comercios = App.EsperarVentana("FormGestionComercios");

            UiFind.ClickFilaGrilla(comercios, "dgvComercios", nombreComercio);
            UiFind.Click(comercios, "btnEliminarComercio");
            App.EsperarMessageBox().ConfirmarSi();
            EsperarAvisoYCerrar("Error.MerchantManagement.ComercioInUse");

            Assert.AreEqual(
                1,
                TestDatabase.Contar("SELECT COUNT(*) FROM dbo.Comercio WHERE Id = @c", new SqlParameter("@c", comercioId)),
                "Un comercio con productos asignados no se puede eliminar.");
        }

        [TestMethod]
        [TestCategory("UI")]
        public void FIN_BajaEnUso_CategoriaDeProductoConProductos_InformaEnUsoYNoElimina()
        {
            string nombreCategoria = NombreUnico("uicatuso");
            int categoriaId = InsertarCategoriaProducto(nombreCategoria);
            InsertarProducto(NombreUnico("uiprod"), categoriaId);
            Window principal = LoginComo(TestConfig.OperadorUsername, TestConfig.OperadorPassword);
            UiFind.ClickMenu(principal, TextoMenuOperador, TestDatabase.ObtenerTraduccion("FormMainShell.Menu.ProductCategories"));
            Window detalle = App.EsperarVentana("FormGestionCategoriasProducto");

            UiFind.ClickFilaGrilla(detalle, "dgvCategoriasProducto", nombreCategoria);
            UiFind.Click(detalle, "btnEliminarCategoriaProducto");
            App.EsperarMessageBox().ConfirmarSi();
            EsperarAvisoYCerrar("Error.MerchantManagement.CategoriaProductoInUse");

            Assert.AreEqual(
                1,
                TestDatabase.Contar("SELECT COUNT(*) FROM dbo.CategoriaProducto WHERE Id = @c", new SqlParameter("@c", categoriaId)),
                "Una categoria con productos no se puede eliminar.");
        }

        [TestMethod]
        [TestCategory("UI")]
        public void FIN_BajaEnUso_MedioPagoDeEntidadContratadoEnCuenta_InformaEnUsoYNoQuita()
        {
            string nombreEntidad = NombreUnico("uientuso");
            string marca = NombreUnico("uimarca");
            string segmento = NombreUnico("uiseg");
            int entidadId = InsertarEntidad(nombreEntidad);
            int medioPagoId = InsertarMedioPago(TipoMedioPagoCredito, marca);
            int medioPagoEntidadId = InsertarMedioPagoEntidad(entidadId, medioPagoId, segmento);
            int cuentaId = InsertarCuenta(TestConfig.BasicUsername, entidadId, NombreUnico("uialias"), TipoCuentaCajaAhorro);
            TestDatabase.EjecutarSql(
                "INSERT INTO dbo.CuentaFinancieraMedioPagoEntidad (CuentaFinancieraId, MedioPagoEntidadId) VALUES (@c, @m)",
                new SqlParameter("@c", cuentaId),
                new SqlParameter("@m", medioPagoEntidadId));

            Window principal = LoginComo(TestConfig.OperadorUsername, TestConfig.OperadorPassword);
            UiFind.ClickMenu(principal, TextoMenuOperador, TestDatabase.ObtenerTraduccion("FormMainShell.Menu.FinancialEntities"));
            Window entidades = App.EsperarVentana("FormGestionEntidadesFinancieras");
            UiFind.ClickFilaGrilla(entidades, "dgvEntidadesFinancieras", nombreEntidad);
            UiFind.Click(entidades, "btnVerDetalleEntidadFinanciera");
            Window detalle = App.EsperarVentana("FormDetalleEntidadFinanciera");

            UiFind.ClickFilaGrilla(detalle, "dgvMediosPagoEntidad", segmento);
            UiFind.Click(detalle, "btnQuitarMedioPagoEntidad");
            App.EsperarMessageBox().ConfirmarSi();
            EsperarAvisoYCerrar("Error.FinancialEntityManagement.EntityPaymentMethodInUse");

            Assert.AreEqual(
                1,
                TestDatabase.Contar("SELECT COUNT(*) FROM dbo.MedioPagoEntidad WHERE Id = @m", new SqlParameter("@m", medioPagoEntidadId)),
                "Un medio de pago contratado en una cuenta no se puede quitar de la entidad.");
        }

        // --- Datos previos ---

        private static int InsertarCategoriaProducto(string nombre)
        {
            return InsertarYObtenerId("INSERT INTO dbo.CategoriaProducto (Nombre) VALUES (@n)", new SqlParameter("@n", nombre));
        }

        private static int InsertarProducto(string nombre, int categoriaId)
        {
            return InsertarYObtenerId(
                "INSERT INTO dbo.Producto (CategoriaProductoId, Nombre) VALUES (@c, @n)",
                new SqlParameter("@c", categoriaId),
                new SqlParameter("@n", nombre));
        }

        private static int InsertarComercio(string nombre)
        {
            int rubroId = InsertarYObtenerId("INSERT INTO dbo.RubroComercio (Nombre) VALUES (@r)", new SqlParameter("@r", "rubro-" + nombre));
            return InsertarYObtenerId(
                "INSERT INTO dbo.Comercio (RubroComercioId, Nombre) VALUES (@r, @n)",
                new SqlParameter("@r", rubroId),
                new SqlParameter("@n", nombre));
        }

        private static void AsignarProductoAComercio(int comercioId, int productoId, decimal precio)
        {
            int filas = TestDatabase.EjecutarSql(
                "INSERT INTO dbo.ComercioProducto (ComercioId, ProductoId, MonedaId, Precio) " +
                "SELECT @c, @p, Id, @precio FROM dbo.Moneda WHERE Codigo = 'ARS'",
                new SqlParameter("@c", comercioId),
                new SqlParameter("@p", productoId),
                new SqlParameter("@precio", precio));

            Assert.AreEqual(1, filas, "No se pudo armar el dato previo: producto asignado al comercio.");
        }

        private static int InsertarMedioPago(string tipo, string marca)
        {
            return InsertarYObtenerId(
                "INSERT INTO dbo.MedioPago (Tipo, Marca) VALUES (@t, @m)",
                new SqlParameter("@t", tipo),
                new SqlParameter("@m", marca));
        }

        private static int InsertarEntidad(string nombre)
        {
            return InsertarYObtenerId("INSERT INTO dbo.EntidadFinanciera (Nombre) VALUES (@n)", new SqlParameter("@n", nombre));
        }

        private static int InsertarMedioPagoEntidad(int entidadId, int medioPagoId, string segmento)
        {
            return InsertarYObtenerId(
                "INSERT INTO dbo.MedioPagoEntidad (EntidadFinancieraId, MedioPagoId, Segmento) VALUES (@e, @m, @s)",
                new SqlParameter("@e", entidadId),
                new SqlParameter("@m", medioPagoId),
                new SqlParameter("@s", segmento));
        }

        private static int InsertarCuenta(string username, int entidadId, string alias, string tipo)
        {
            return InsertarYObtenerId(
                "INSERT INTO dbo.CuentaFinanciera (UsuarioId, EntidadFinancieraId, Alias, Tipo) " +
                "SELECT u.Id, @e, @a, @t FROM dbo.Usuarios u WHERE u.Username = @u",
                new SqlParameter("@u", username),
                new SqlParameter("@e", entidadId),
                new SqlParameter("@a", alias),
                new SqlParameter("@t", tipo));
        }

        private static int InsertarYObtenerId(string insert, params SqlParameter[] parametros)
        {
            object id = TestDatabase.EscalarOrNull(insert + "; SELECT CAST(SCOPE_IDENTITY() AS INT);", parametros);
            if (!(id is int valor))
            {
                throw new InvalidOperationException("No se pudo armar el dato previo del test: " + insert);
            }

            return valor;
        }

        private static string NombreUnico(string prefijo)
        {
            return prefijo + DateTime.Now.ToString("HHmmssfff");
        }
    }
}
