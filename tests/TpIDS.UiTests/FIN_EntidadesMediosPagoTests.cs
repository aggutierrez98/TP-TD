using System;
using System.Data.SqlClient;
using System.Linq;
using FlaUI.Core.AutomationElements;
using FlaUI.Core.Definitions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TpIDS.UiTests.Infrastructure;

namespace TpIDS.UiTests
{
    /// <summary>
    /// FIN - Entidades financieras, medios de pago y cuentas financieras (proceso de negocio
    /// sin codigo N0X asignado todavia; ver ESTADO-ACTUAL-Y-PROXIMOS-PASOS.md seccion 8).
    /// Operador &gt; Medios de pago / Entidades financieras (+ detalle de MedioPagoEntidad) y
    /// Finanzas &gt; Mis cuentas financieras (solo alta y baja, scoped al usuario logueado).
    /// Los datos previos se arman por SQL directo: MedioPago, EntidadFinanciera,
    /// MedioPagoEntidad, CuentaFinanciera y su tabla puente no tienen firmas DVH/DVV.
    /// </summary>
    [TestClass]
    public class FIN_EntidadesMediosPagoTests : UiTestBase
    {
        private const string TipoMedioPagoCredito = "TarjetaCredito";
        private const string TipoCuentaCajaAhorro = "CajaAhorro";

        private static readonly string TextoMenuFinanzas = TestDatabase.ObtenerTraduccion("FormMainShell.Menu.Finance");
        private static readonly string TextoMenuOperador = TestDatabase.ObtenerTraduccion("FormMainShell.Menu.Operator");
        private static readonly string TextoTarjetaCredito = TestDatabase.ObtenerTraduccion("FormPaymentMethodManagement.Value.Type.TarjetaCredito");

        // --- Medios de pago ---

        [TestMethod]
        [TestCategory("UI")]
        public void FIN_MediosPago_Crear_PersisteMedioPago()
        {
            string marca = NombreUnico("uimarca");

            Window principal = LoginComo(TestConfig.OperadorUsername, TestConfig.OperadorPassword);
            Window mediosPago = AbrirGestionMediosPago(principal);

            UiFind.SeleccionarItemCombo(mediosPago, "cmbTipoMedioPago", TextoTarjetaCredito);
            UiFind.EscribirTexto(mediosPago, "txtbMarcaMedioPago", marca);
            UiFind.Click(mediosPago, "btnAgregarMedioPago");
            EsperarAvisoYCerrar("FormPaymentMethodManagement.Success.Created");

            Assert.AreEqual(
                1,
                ContarMediosPago(TipoMedioPagoCredito, marca),
                "El medio de pago nuevo no quedo persistido con su tipo y marca.");
        }

        [TestMethod]
        [TestCategory("UI")]
        public void FIN_MediosPago_TipoMarcaDuplicado_InformaErrorYNoInserta()
        {
            string marca = NombreUnico("uidup");
            InsertarMedioPago(TipoMedioPagoCredito, marca);

            Window principal = LoginComo(TestConfig.OperadorUsername, TestConfig.OperadorPassword);
            Window mediosPago = AbrirGestionMediosPago(principal);

            UiFind.SeleccionarItemCombo(mediosPago, "cmbTipoMedioPago", TextoTarjetaCredito);
            UiFind.EscribirTexto(mediosPago, "txtbMarcaMedioPago", marca);
            UiFind.Click(mediosPago, "btnAgregarMedioPago");
            EsperarAvisoYCerrar("Error.PaymentMethodManagement.Duplicate");

            Assert.AreEqual(
                1,
                ContarMediosPago(TipoMedioPagoCredito, marca),
                "No debe insertarse un segundo medio de pago con el mismo tipo y marca.");
        }

        [TestMethod]
        [TestCategory("UI")]
        public void FIN_MediosPago_Eliminar_QuitaMedioPago()
        {
            string marca = NombreUnico("uidel");
            InsertarMedioPago(TipoMedioPagoCredito, marca);

            Window principal = LoginComo(TestConfig.OperadorUsername, TestConfig.OperadorPassword);
            Window mediosPago = AbrirGestionMediosPago(principal);

            UiFind.ClickFilaGrilla(mediosPago, "dgvMediosPago", marca);
            UiFind.Click(mediosPago, "btnEliminarMedioPago");
            App.EsperarMessageBox().ConfirmarSi();
            EsperarAvisoYCerrar("FormPaymentMethodManagement.Success.Deleted");

            Assert.AreEqual(
                0,
                ContarMediosPago(TipoMedioPagoCredito, marca),
                "El medio de pago eliminado no deberia seguir en la tabla.");
        }

        // --- Entidades financieras ---

        [TestMethod]
        [TestCategory("UI")]
        public void FIN_EntidadesFinancieras_Crear_PersisteEntidad()
        {
            string nombre = NombreUnico("uientidad");

            Window principal = LoginComo(TestConfig.OperadorUsername, TestConfig.OperadorPassword);
            Window entidades = AbrirGestionEntidadesFinancieras(principal);

            UiFind.EscribirTexto(entidades, "txtbNombreEntidadFinanciera", nombre);
            UiFind.Click(entidades, "btnAgregarEntidadFinanciera");
            EsperarAvisoYCerrar("FormFinancialEntityManagement.Success.Created");

            Assert.AreEqual(
                1,
                TestDatabase.Contar(
                    "SELECT COUNT(*) FROM dbo.EntidadFinanciera WHERE Nombre = @n",
                    new SqlParameter("@n", nombre)),
                "La entidad financiera nueva no quedo persistida.");
        }

        [TestMethod]
        [TestCategory("UI")]
        public void FIN_EntidadesFinancieras_NombreVacio_InformaErrorYNoInserta()
        {
            int entidadesPrevias = TestDatabase.Contar("SELECT COUNT(*) FROM dbo.EntidadFinanciera");

            Window principal = LoginComo(TestConfig.OperadorUsername, TestConfig.OperadorPassword);
            Window entidades = AbrirGestionEntidadesFinancieras(principal);

            UiFind.EscribirTexto(entidades, "txtbNombreEntidadFinanciera", string.Empty);
            UiFind.Click(entidades, "btnAgregarEntidadFinanciera");
            EsperarAvisoYCerrar("Error.FinancialEntityManagement.NameRequired");

            Assert.AreEqual(
                entidadesPrevias,
                TestDatabase.Contar("SELECT COUNT(*) FROM dbo.EntidadFinanciera"),
                "Un nombre vacio no debe insertar una entidad financiera.");
        }

        [TestMethod]
        [TestCategory("UI")]
        public void FIN_EntidadesFinancieras_DetalleAgregarYQuitarMedioPago_PersisteYQuita()
        {
            string nombre = NombreUnico("uient_det");
            string marca = NombreUnico("uimarca_det");
            string segmento = NombreUnico("uiseg");
            int entidadId = InsertarEntidad(nombre);
            int medioPagoId = InsertarMedioPago(TipoMedioPagoCredito, marca);

            Window principal = LoginComo(TestConfig.OperadorUsername, TestConfig.OperadorPassword);
            Window entidades = AbrirGestionEntidadesFinancieras(principal);

            UiFind.ClickFilaGrilla(entidades, "dgvEntidadesFinancieras", nombre);
            UiFind.Click(entidades, "btnVerDetalleEntidadFinanciera");
            Window detalle = App.EsperarVentana("FormDetalleEntidadFinanciera");

            // Alta del producto de la entidad: MedioPago del catalogo global + Segmento.
            UiFind.SeleccionarItemCombo(detalle, "cmbMedioPagoEntidad", TextoTarjetaCredito + " " + marca);
            UiFind.EscribirTexto(detalle, "txtbSegmentoMedioPagoEntidad", segmento);
            UiFind.Click(detalle, "btnAgregarMedioPagoEntidad");
            EsperarAvisoYCerrar("FormFinancialEntityDetail.Success.Added");

            Assert.AreEqual(
                1,
                ContarMediosPagoEntidad(entidadId, medioPagoId, segmento),
                "El medio de pago de la entidad no quedo persistido con su segmento.");

            // Baja del mismo producto.
            UiFind.ClickFilaGrilla(detalle, "dgvMediosPagoEntidad", segmento);
            UiFind.Click(detalle, "btnQuitarMedioPagoEntidad");
            App.EsperarMessageBox().ConfirmarSi();
            EsperarAvisoYCerrar("FormFinancialEntityDetail.Success.Removed");

            Assert.AreEqual(
                0,
                ContarMediosPagoEntidad(entidadId, medioPagoId, segmento),
                "El medio de pago quitado no deberia seguir asociado a la entidad.");
        }

        // --- Cuentas financieras (usuario basico: gestioncuentasfinancieras) ---

        [TestMethod]
        [TestCategory("UI")]
        public void FIN_CuentasFinancieras_CrearConMediosMarcados_PersisteCuentaYPuente()
        {
            string entidad = NombreUnico("uient_cta");
            string marcaContratada = NombreUnico("uimc");
            string marcaNoContratada = NombreUnico("uimn");
            string alias = NombreUnico("uialias");
            int entidadId = InsertarEntidad(entidad);
            int medioPagoEntidadContratadoId = InsertarMedioPagoEntidad(
                entidadId, InsertarMedioPago(TipoMedioPagoCredito, marcaContratada), "Gold");
            InsertarMedioPagoEntidad(entidadId, InsertarMedioPago(TipoMedioPagoCredito, marcaNoContratada), null);

            Window principal = LoginComo(TestConfig.BasicUsername, TestConfig.BasicPassword);
            Window cuentas = AbrirGestionCuentasFinancieras(principal);

            UiFind.SeleccionarItemComboConTeclado(cuentas, "cmbEntidadFinancieraCuenta", entidad);
            UiFind.EscribirTexto(cuentas, "txtbAliasCuenta", alias);
            UiFind.SeleccionarItemCombo(
                cuentas,
                "cmbTipoCuentaFinanciera",
                TestDatabase.ObtenerTraduccion("FormFinancialAccountManagement.Value.Type.CajaAhorro"));
            UiFind.MarcarItemListaChequeable(cuentas, "clbMediosPagoContratados", marcaContratada);
            UiFind.Click(cuentas, "btnAgregarCuenta");
            EsperarAvisoYCerrar("FormFinancialAccountManagement.Success.Created");

            Assert.AreEqual(
                1,
                TestDatabase.Contar(
                    "SELECT COUNT(*) FROM dbo.CuentaFinanciera c " +
                    "JOIN dbo.Usuarios u ON u.Id = c.UsuarioId " +
                    "WHERE c.Alias = @a AND c.EntidadFinancieraId = @e AND c.Tipo = @t AND u.Username = @u",
                    new SqlParameter("@a", alias),
                    new SqlParameter("@e", entidadId),
                    new SqlParameter("@t", TipoCuentaCajaAhorro),
                    new SqlParameter("@u", TestConfig.BasicUsername)),
                "La cuenta no quedo persistida para el usuario logueado con su entidad y tipo.");

            Assert.AreEqual(
                1,
                ContarPuenteDeCuenta(alias),
                "Solo el medio de pago marcado deberia quedar contratado en la tabla puente.");
            Assert.AreEqual(
                1,
                TestDatabase.Contar(
                    "SELECT COUNT(*) FROM dbo.CuentaFinancieraMedioPagoEntidad p " +
                    "JOIN dbo.CuentaFinanciera c ON c.Id = p.CuentaFinancieraId " +
                    "WHERE c.Alias = @a AND p.MedioPagoEntidadId = @m",
                    new SqlParameter("@a", alias),
                    new SqlParameter("@m", medioPagoEntidadContratadoId)),
                "El medio de pago contratado no es el que se marco en la pantalla.");
        }

        [TestMethod]
        [TestCategory("UI")]
        public void FIN_CuentasFinancieras_AliasVacio_InformaErrorYNoInserta()
        {
            string entidad = NombreUnico("uient_sinalias");
            int entidadId = InsertarEntidad(entidad);

            Window principal = LoginComo(TestConfig.BasicUsername, TestConfig.BasicPassword);
            Window cuentas = AbrirGestionCuentasFinancieras(principal);

            UiFind.SeleccionarItemCombo(cuentas, "cmbEntidadFinancieraCuenta", entidad);
            UiFind.EscribirTexto(cuentas, "txtbAliasCuenta", string.Empty);
            UiFind.SeleccionarItemCombo(
                cuentas,
                "cmbTipoCuentaFinanciera",
                TestDatabase.ObtenerTraduccion("FormFinancialAccountManagement.Value.Type.CajaAhorro"));
            UiFind.Click(cuentas, "btnAgregarCuenta");
            EsperarAvisoYCerrar("Error.FinancialAccountManagement.AliasRequired");

            Assert.AreEqual(
                0,
                TestDatabase.Contar(
                    "SELECT COUNT(*) FROM dbo.CuentaFinanciera WHERE EntidadFinancieraId = @e",
                    new SqlParameter("@e", entidadId)),
                "Un alias vacio no debe insertar la cuenta.");
        }

        [TestMethod]
        [TestCategory("UI")]
        public void FIN_CuentasFinancieras_Eliminar_QuitaCuentaYPuente()
        {
            string entidad = NombreUnico("uient_delcta");
            string alias = NombreUnico("uialias_del");
            int entidadId = InsertarEntidad(entidad);
            int medioPagoEntidadId = InsertarMedioPagoEntidad(
                entidadId, InsertarMedioPago(TipoMedioPagoCredito, NombreUnico("uimdel")), null);
            int cuentaId = InsertarCuenta(TestConfig.BasicUsername, entidadId, alias, TipoCuentaCajaAhorro);
            TestDatabase.EjecutarSql(
                "INSERT INTO dbo.CuentaFinancieraMedioPagoEntidad (CuentaFinancieraId, MedioPagoEntidadId) VALUES (@c, @m)",
                new SqlParameter("@c", cuentaId),
                new SqlParameter("@m", medioPagoEntidadId));

            Window principal = LoginComo(TestConfig.BasicUsername, TestConfig.BasicPassword);
            Window cuentas = AbrirGestionCuentasFinancieras(principal);

            UiFind.ClickFilaGrilla(cuentas, "dgvCuentasFinancieras", alias);
            UiFind.Click(cuentas, "btnEliminarCuenta");
            App.EsperarMessageBox().ConfirmarSi();
            EsperarAvisoYCerrar("FormFinancialAccountManagement.Success.Deleted");

            Assert.AreEqual(
                0,
                TestDatabase.Contar(
                    "SELECT COUNT(*) FROM dbo.CuentaFinanciera WHERE Id = @c",
                    new SqlParameter("@c", cuentaId)),
                "La cuenta eliminada no deberia seguir en la tabla.");
            Assert.AreEqual(
                0,
                TestDatabase.Contar(
                    "SELECT COUNT(*) FROM dbo.CuentaFinancieraMedioPagoEntidad WHERE CuentaFinancieraId = @c",
                    new SqlParameter("@c", cuentaId)),
                "Los medios contratados de la cuenta eliminada no deberian quedar en la tabla puente.");
        }

        [TestMethod]
        [TestCategory("UI")]
        public void FIN_CuentasFinancieras_Pantalla_SoloPermiteAltaYBaja()
        {
            Window principal = LoginComo(TestConfig.BasicUsername, TestConfig.BasicPassword);
            Window cuentas = AbrirGestionCuentasFinancieras(principal);

            // Espera a que el formulario termine de cargar antes de inventariar sus botones.
            UiFind.Boton(cuentas, "btnAgregarCuenta");

            string[] botonesDelFormulario = cuentas
                .FindAllDescendants(condicion => condicion.ByControlType(ControlType.Button))
                .Select(boton => LeerAutomationId(boton))
                .Where(id => id.StartsWith("btn", StringComparison.Ordinal))
                .OrderBy(id => id, StringComparer.Ordinal)
                .ToArray();

            CollectionAssert.AreEqual(
                new[] { "btnAgregarCuenta", "btnEliminarCuenta" },
                botonesDelFormulario,
                "Una cuenta financiera solo se crea o se elimina: no debe haber accion de edicion. Botones: " +
                string.Join(", ", botonesDelFormulario));
        }

        // --- Permisos ---

        [TestMethod]
        [TestCategory("UI")]
        public void FIN_Permisos_UsuarioBasico_AccedeACuentasPeroNoAlMenuOperador()
        {
            Window principal = LoginComo(TestConfig.BasicUsername, TestConfig.BasicPassword);

            Assert.IsFalse(
                UiFind.MenuTopHabilitado(principal, TextoMenuOperador),
                "basico no gestiona medios de pago ni entidades financieras: el menu Operador debe estar deshabilitado.");
            Assert.IsTrue(
                UiFind.MenuTopHabilitado(principal, TextoMenuFinanzas),
                "basico gestiona sus propias cuentas financieras.");

            AbrirGestionCuentasFinancieras(principal);
        }

        [TestMethod]
        [TestCategory("UI")]
        public void FIN_Permisos_Operador_AccedeAMediosPagoYEntidadesPeroNoAFinanzas()
        {
            Window principal = LoginComo(TestConfig.OperadorUsername, TestConfig.OperadorPassword);

            Assert.IsTrue(
                UiFind.MenuTopHabilitado(principal, TextoMenuOperador),
                "operador gestiona medios de pago y entidades financieras.");
            Assert.IsFalse(
                UiFind.MenuTopHabilitado(principal, TextoMenuFinanzas),
                "operador no tiene permisos de finanzas personales (cuentas financieras incluidas).");

            AbrirGestionMediosPago(principal);
            AbrirGestionEntidadesFinancieras(principal);
        }

        // --- Navegacion ---

        private Window AbrirGestionMediosPago(Window principal)
        {
            UiFind.ClickMenu(principal, TextoMenuOperador, TestDatabase.ObtenerTraduccion("FormMainShell.Menu.PaymentMethods"));
            return App.EsperarVentana("FormGestionMediosPago");
        }

        private Window AbrirGestionEntidadesFinancieras(Window principal)
        {
            UiFind.ClickMenu(principal, TextoMenuOperador, TestDatabase.ObtenerTraduccion("FormMainShell.Menu.FinancialEntities"));
            return App.EsperarVentana("FormGestionEntidadesFinancieras");
        }

        private Window AbrirGestionCuentasFinancieras(Window principal)
        {
            UiFind.ClickMenu(principal, TextoMenuFinanzas, TestDatabase.ObtenerTraduccion("FormMainShell.Menu.FinancialAccounts"));
            return App.EsperarVentana("FormGestionCuentasFinancieras");
        }

        // --- Datos previos (tablas sin firmas DVH/DVV) ---

        private static int InsertarMedioPago(string tipo, string marca)
        {
            return InsertarYObtenerId(
                "INSERT INTO dbo.MedioPago (Tipo, Marca) VALUES (@t, @m)",
                new SqlParameter("@t", tipo),
                new SqlParameter("@m", marca));
        }

        private static int InsertarEntidad(string nombre)
        {
            return InsertarYObtenerId(
                "INSERT INTO dbo.EntidadFinanciera (Nombre) VALUES (@n)",
                new SqlParameter("@n", nombre));
        }

        private static int InsertarMedioPagoEntidad(int entidadId, int medioPagoId, string segmento)
        {
            return InsertarYObtenerId(
                "INSERT INTO dbo.MedioPagoEntidad (EntidadFinancieraId, MedioPagoId, Segmento) VALUES (@e, @m, @s)",
                new SqlParameter("@e", entidadId),
                new SqlParameter("@m", medioPagoId),
                new SqlParameter("@s", (object)segmento ?? DBNull.Value));
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

        // --- Asserts de BD ---

        private static int ContarMediosPago(string tipo, string marca)
        {
            return TestDatabase.Contar(
                "SELECT COUNT(*) FROM dbo.MedioPago WHERE Tipo = @t AND Marca = @m",
                new SqlParameter("@t", tipo),
                new SqlParameter("@m", marca));
        }

        private static int ContarMediosPagoEntidad(int entidadId, int medioPagoId, string segmento)
        {
            return TestDatabase.Contar(
                "SELECT COUNT(*) FROM dbo.MedioPagoEntidad WHERE EntidadFinancieraId = @e AND MedioPagoId = @m AND Segmento = @s",
                new SqlParameter("@e", entidadId),
                new SqlParameter("@m", medioPagoId),
                new SqlParameter("@s", segmento));
        }

        private static int ContarPuenteDeCuenta(string alias)
        {
            return TestDatabase.Contar(
                "SELECT COUNT(*) FROM dbo.CuentaFinancieraMedioPagoEntidad p " +
                "JOIN dbo.CuentaFinanciera c ON c.Id = p.CuentaFinancieraId WHERE c.Alias = @a",
                new SqlParameter("@a", alias));
        }

        private static string LeerAutomationId(AutomationElement elemento)
        {
            try
            {
                return elemento.AutomationId ?? string.Empty;
            }
            catch (FlaUI.Core.Exceptions.PropertyNotSupportedException)
            {
                return string.Empty;
            }
        }

        private static string NombreUnico(string prefijo)
        {
            return prefijo + DateTime.Now.ToString("HHmmssfff");
        }
    }
}
