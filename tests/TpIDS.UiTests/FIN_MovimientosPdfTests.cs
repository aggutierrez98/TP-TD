using System;
using System.IO;
using System.Data.SqlClient;
using System.Linq;
using FlaUI.Core.AutomationElements;
using FlaUI.Core.Input;
using FlaUI.Core.Tools;
using FlaUI.Core.WindowsAPI;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TpIDS.UiTests.Infrastructure;
using UglyToad.PdfPig.DocumentLayoutAnalysis.TextExtractor;

namespace TpIDS.UiTests
{
    [TestClass]
    public sealed class FIN_MovimientosPdfTests : UiTestBase
    {
        [TestMethod]
        [TestCategory("UI")]
        public void FIN_MovimientosPdf_DescripcionYTipo_CombinanFiltrosYEnterBusca()
        {
            Window ventana = AbrirMovimientos();
            string prefijo = "pdf" + Guid.NewGuid().ToString("N").Substring(0, 8);
            CrearMovimiento(ventana, prefijo + " compra", false);
            CrearMovimiento(ventana, prefijo + " reintegro", true);
            Buscar(ventana, "  " + prefijo + "  ");
            EsperarFilas(ventana, 2);
            UiFind.SeleccionarItemCombo(ventana, "cmbTipoFiltro", Tr("FormMovements.Value.Type.Expense"));
            UiFind.Click(ventana, "btnBuscarMovimientos");
            EsperarFilas(ventana, 1);
            UiFind.EscribirTexto(ventana, "txtbDescripcionFiltro", prefijo + " reintegro");
            UiFind.Elemento(ventana, "txtbDescripcionFiltro").Focus();
            Keyboard.Type(VirtualKeyShort.RETURN);
            EsperarFilas(ventana, 0);
            Assert.IsFalse(UiFind.Boton(ventana, "btnSeleccionarTodosMovimientos").IsEnabled);
            Assert.AreEqual(string.Format(Tr("FormMovements.SelectionCount"), 0, 0),
                UiFind.Elemento(ventana, "lblSeleccionMovimientos").Name);
        }

        [TestMethod]
        [TestCategory("UI")]
        public void FIN_MovimientosPdf_MarcasIndividualesYMasivas_NoEditanYSeReinicianAlBuscar()
        {
            Window ventana = AbrirMovimientos();
            string prefijo = "pdf" + Guid.NewGuid().ToString("N").Substring(0, 8);
            CrearMovimiento(ventana, prefijo + " uno", false);
            CrearMovimiento(ventana, prefijo + " dos", false);
            Buscar(ventana, prefijo);
            EsperarFilas(ventana, 2);
            Assert.IsFalse(UiFind.Boton(ventana, "btnExportarMovimientosPdf").IsEnabled);
            string edicionAntes = UiFind.LeerTexto(ventana, "txtbDescripcionMovimiento");
            var grid = UiFind.Elemento(ventana, "dgvMovimientos").Patterns.Grid.Pattern;
            grid.GetItem(0, 0).Click();
            EsperarContador(ventana, 1, 2);
            Assert.AreEqual(edicionAntes, UiFind.LeerTexto(ventana, "txtbDescripcionMovimiento"));
            grid.GetItem(1, 6).Click();
            Assert.IsTrue(Retry.WhileFalse(() => UiFind.LeerTexto(ventana, "txtbDescripcionMovimiento").StartsWith(prefijo),
                TimeSpan.FromSeconds(5)).Success);
            EsperarContador(ventana, 1, 2);
            UiFind.Click(ventana, "btnSeleccionarTodosMovimientos");
            EsperarContador(ventana, 2, 2);
            UiFind.Click(ventana, "btnDesmarcarTodosMovimientos");
            EsperarContador(ventana, 0, 2);
            UiFind.Click(ventana, "btnSeleccionarTodosMovimientos");
            Buscar(ventana, prefijo);
            EsperarContador(ventana, 0, 2);
            Assert.IsFalse(UiFind.Boton(ventana, "btnExportarMovimientosPdf").IsEnabled);
        }

        [TestMethod]
        [TestCategory("UI")]
        public void FIN_MovimientosPdf_CancelarYGuardar_ConservanMarcasYCriteriosAplicados()
        {
            Window ventana = AbrirMovimientos();
            string prefijo = "pdf" + Guid.NewGuid().ToString("N").Substring(0, 8);
            CrearMovimiento(ventana, prefijo + " elegido", false);
            CrearMovimiento(ventana, prefijo + " excluido", false);
            Buscar(ventana, prefijo);
            EsperarFilas(ventana, 2);
            var grid = UiFind.Elemento(ventana, "dgvMovimientos").Patterns.Grid.Pattern;
            grid.GetItem(0, 0).Click();
            EsperarContador(ventana, 1, 2);
            string seleccionado = grid.GetItem(0, 6).Patterns.Value.Pattern.Value;
            string excluido = grid.GetItem(1, 6).Patterns.Value.Pattern.Value;
            string sinAplicar = "sin-aplicar-" + Guid.NewGuid().ToString("N");
            UiFind.EscribirTexto(ventana, "txtbDescripcionFiltro", sinAplicar);
            UiFind.Click(ventana, "btnExportarMovimientosPdf");
            Window dialogo = EsperarGuardar(ventana);
            dialogo.Focus();
            Keyboard.Type(VirtualKeyShort.ESCAPE);
            EsperarContador(ventana, 1, 2);
            string destino = Path.Combine(Path.GetTempPath(), "finazapp-ui-" + Guid.NewGuid().ToString("N") + ".pdf");
            try
            {
                UiFind.Click(ventana, "btnExportarMovimientosPdf");
                dialogo = EsperarGuardar(ventana);
                var nombre = dialogo.FindFirstDescendant(cf => cf.ByAutomationId("1001")).AsTextBox();
                nombre.Click();
                // IFileDialog conserva su propio nombre: ValuePattern cambia lo
                // visible pero no notifica la edicion al shell. Escribir de verdad.
                Keyboard.TypeSimultaneously(VirtualKeyShort.CONTROL, VirtualKeyShort.KEY_A);
                Keyboard.Type(destino);
                Keyboard.Type(VirtualKeyShort.RETURN);
                Assert.IsTrue(Retry.WhileFalse(() => File.Exists(destino), TimeSpan.FromSeconds(15)).Success,
                    "No se guardo el PDF en la ruta elegida.");
                App.EsperarMessageBox(TimeSpan.FromSeconds(30)).CerrarConOk();
                Assert.IsTrue(File.Exists(destino));
                using (var pdf = UglyToad.PdfPig.PdfDocument.Open(destino))
                {
                    string texto = string.Join(" ", pdf.GetPages().Select(p => ContentOrderTextExtractor.GetText(p)));
                    Assert.Contains(seleccionado, texto);
                    Assert.DoesNotContain(excluido, texto);
                    Assert.DoesNotContain(sinAplicar, texto);
                    Assert.Contains(string.Format(Tr("FormMovements.Report.Description"), prefijo), texto);
                    Assert.Contains(Tr("FormMovements.Report.Count") + ": 1", texto);
                }
                EsperarContador(ventana, 1, 2);
                Assert.IsTrue(UiFind.Boton(ventana, "btnExportarMovimientosPdf").IsEnabled);
            }
            finally { if (File.Exists(destino)) File.Delete(destino); }
        }

        [TestMethod]
        [TestCategory("UI")]
        public void FIN_MovimientosPdf_SeleccionarTodosConScroll_ExportaTodasLasFilas()
        {
            const int total = 30;
            string prefijo = "pdf" + Guid.NewGuid().ToString("N").Substring(0, 8);
            // Movimiento no lleva DVH/DVV: armar el volumen por SQL evita 30 altas por UI.
            TestDatabase.EjecutarSql(@"
                INSERT INTO dbo.Movimiento (UsuarioId, CategoriaId, Tipo, Monto, Fecha, Descripcion)
                SELECT u.Id, c.Id, 'Egreso', 5, CAST(GETDATE() AS DATE), @p + ' fila ' + CAST(n.Numero AS VARCHAR(5))
                FROM (SELECT TOP (1) Id FROM dbo.Usuarios WHERE Username = @u) u
                CROSS JOIN (SELECT TOP (1) Id FROM dbo.Categoria WHERE UsuarioId IS NULL ORDER BY Id) c
                CROSS JOIN (SELECT TOP (" + total + @") ROW_NUMBER() OVER (ORDER BY object_id) AS Numero FROM sys.all_objects) n",
                new SqlParameter("@p", prefijo), new SqlParameter("@u", TestConfig.BasicUsername));
            string destino = Path.Combine(Path.GetTempPath(), "finazapp-ui-" + Guid.NewGuid().ToString("N") + ".pdf");
            try
            {
                Window ventana = AbrirMovimientos();
                Buscar(ventana, prefijo);
                EsperarFilas(ventana, total);
                UiFind.Click(ventana, "btnSeleccionarTodosMovimientos");
                EsperarContador(ventana, total, total);
                UiFind.Click(ventana, "btnExportarMovimientosPdf");
                GuardarEnRuta(ventana, destino);
                App.EsperarMessageBox(TimeSpan.FromSeconds(30)).CerrarConOk();
                using (var pdf = UglyToad.PdfPig.PdfDocument.Open(destino))
                {
                    string texto = string.Join(" ", pdf.GetPages().Select(p => ContentOrderTextExtractor.GetText(p)));
                    Assert.Contains(Tr("FormMovements.Report.Count") + ": " + total, texto);
                    Assert.Contains(prefijo + " fila 1", texto);
                    Assert.Contains(prefijo + " fila " + total, texto);
                }
            }
            finally
            {
                TestDatabase.EjecutarSql("DELETE FROM dbo.Movimiento WHERE Descripcion LIKE @p + '%'", new SqlParameter("@p", prefijo));
                if (File.Exists(destino)) File.Delete(destino);
            }
        }

        [TestMethod]
        [TestCategory("UI")]
        public void FIN_MovimientosPdf_ConsultaFallida_ConservaListadoMarcasYCriteriosAplicados()
        {
            Window ventana = AbrirMovimientos();
            string prefijo = "pdf" + Guid.NewGuid().ToString("N").Substring(0, 8);
            CrearMovimiento(ventana, prefijo + " uno", false);
            CrearMovimiento(ventana, prefijo + " dos", false);
            Buscar(ventana, prefijo);
            EsperarFilas(ventana, 2);
            var grid = UiFind.Elemento(ventana, "dgvMovimientos").Patterns.Grid.Pattern;
            grid.GetItem(0, 0).Click();
            EsperarContador(ventana, 1, 2);
            string seleccionado = grid.GetItem(0, 6).Patterns.Value.Pattern.Value;
            string noAplicado = "no-aplicado-" + Guid.NewGuid().ToString("N");
            UiFind.EscribirTexto(ventana, "txtbDescripcionFiltro", noAplicado);
            string destino = Path.Combine(Path.GetTempPath(), "finazapp-ui-" + Guid.NewGuid().ToString("N") + ".pdf");
            // Un bloqueo exclusivo de la tabla hace fallar la consulta por timeout (~30 s);
            // si el proceso de test muere, la conexion se cierra y el bloqueo se libera solo.
            using (var bloqueo = new SqlConnection(TestConfig.ConnectionString))
            {
                bloqueo.Open();
                using (var tx = bloqueo.BeginTransaction())
                {
                    new SqlCommand("SELECT TOP (1) Id FROM dbo.Movimiento WITH (TABLOCKX, HOLDLOCK)", bloqueo, tx).ExecuteScalar();
                    UiFind.Click(ventana, "btnBuscarMovimientos");
                    App.EsperarMessageBox(TimeSpan.FromSeconds(90)).CerrarConOk();
                    tx.Rollback();
                }
            }
            try
            {
                EsperarFilas(ventana, 2);
                EsperarContador(ventana, 1, 2);
                Assert.IsTrue(UiFind.Boton(ventana, "btnExportarMovimientosPdf").IsEnabled);
                UiFind.Click(ventana, "btnExportarMovimientosPdf");
                GuardarEnRuta(ventana, destino);
                App.EsperarMessageBox(TimeSpan.FromSeconds(30)).CerrarConOk();
                using (var pdf = UglyToad.PdfPig.PdfDocument.Open(destino))
                {
                    string texto = string.Join(" ", pdf.GetPages().Select(p => ContentOrderTextExtractor.GetText(p)));
                    Assert.Contains(seleccionado, texto);
                    Assert.DoesNotContain(noAplicado, texto);
                    Assert.Contains(string.Format(Tr("FormMovements.Report.Description"), prefijo), texto);
                    Assert.Contains(Tr("FormMovements.Report.Count") + ": 1", texto);
                }
            }
            finally { if (File.Exists(destino)) File.Delete(destino); }
        }

        [TestMethod]
        [TestCategory("UI")]
        public void FIN_MovimientosPdf_DestinoNoEscribible_MuestraErrorYConservaSeleccionYOriginal()
        {
            Window ventana = AbrirMovimientos();
            string prefijo = "pdf" + Guid.NewGuid().ToString("N").Substring(0, 8);
            CrearMovimiento(ventana, prefijo + " uno", false);
            Buscar(ventana, prefijo);
            EsperarFilas(ventana, 1);
            UiFind.Click(ventana, "btnSeleccionarTodosMovimientos");
            EsperarContador(ventana, 1, 1);
            // El dialogo comun de Windows rechaza carpetas sin permiso antes de devolver la ruta
            // a la app; un archivo existente abierto por otro proceso si llega a la escritura.
            string destino = Path.Combine(Path.GetTempPath(), "finazapp-ui-bloqueado-" + Guid.NewGuid().ToString("N") + ".pdf");
            byte[] original = { 1, 2, 3, 4 };
            File.WriteAllBytes(destino, original);
            int temporalesAntes = Directory.GetFiles(Path.GetTempPath(), ".finazapp-*.tmp").Length;
            try
            {
                using (new FileStream(destino, FileMode.Open, FileAccess.ReadWrite, FileShare.None))
                {
                    UiFind.Click(ventana, "btnExportarMovimientosPdf");
                    GuardarEnRuta(ventana, destino, true);
                    MessageBoxWindow error = App.EsperarMessageBox(TimeSpan.FromSeconds(30));
                    Assert.IsTrue(error.Texto.Contains(Tr("Error.Movement.ExportWriteFailed")),
                        "Se esperaba el error de escritura pero se mostro: " + error.Texto);
                    error.CerrarConOk();
                }
                CollectionAssert.AreEqual(original, File.ReadAllBytes(destino));
                Assert.HasCount(temporalesAntes, Directory.GetFiles(Path.GetTempPath(), ".finazapp-*.tmp"));
                EsperarFilas(ventana, 1);
                EsperarContador(ventana, 1, 1);
                Assert.IsTrue(UiFind.Boton(ventana, "btnExportarMovimientosPdf").IsEnabled);
                Assert.IsTrue(UiFind.Boton(ventana, "btnBuscarMovimientos").IsEnabled);
            }
            finally { if (File.Exists(destino)) File.Delete(destino); }
        }

        private void GuardarEnRuta(Window ventana, string destino, bool confirmarReemplazo = false)
        {
            Window dialogo = EsperarGuardar(ventana);
            var nombre = dialogo.FindFirstDescendant(cf => cf.ByAutomationId("1001")).AsTextBox();
            nombre.Click();
            // Mismo motivo que en el test de guardado: escribir de verdad, no ValuePattern.
            Keyboard.TypeSimultaneously(VirtualKeyShort.CONTROL, VirtualKeyShort.KEY_A);
            Keyboard.Type(destino);
            Keyboard.Type(VirtualKeyShort.RETURN);
            if (!confirmarReemplazo) return;
            var confirmacion = Retry.WhileNull(() => dialogo.ModalWindows.FirstOrDefault(), TimeSpan.FromSeconds(10));
            Assert.IsNotNull(confirmacion.Result, "No aparecio la confirmacion de reemplazo.");
            // El shell no expone ids numericos de MessageBox en esta confirmacion: ubicar "Si" por nombre.
            var afirmativo = confirmacion.Result
                .FindAllDescendants(cf => cf.ByControlType(FlaUI.Core.Definitions.ControlType.Button))
                .FirstOrDefault(boton => boton.Name == "Sí" || boton.Name == "Yes");
            Assert.IsNotNull(afirmativo, "No se encontro el boton afirmativo de la confirmacion.");
            afirmativo.AsButton().Invoke();
        }

        private Window AbrirMovimientos()
        {
            // Limpiar teclas que pudo dejar abajo una corrida interrumpida.
            Keyboard.Release(VirtualKeyShort.RETURN);
            Keyboard.Release(VirtualKeyShort.ESCAPE);
            LoginComo(TestConfig.BasicUsername, TestConfig.BasicPassword);
            return App.EsperarVentana("FormMovimientos");
        }

        private void CrearMovimiento(Window ventana, string descripcion, bool ingreso)
        {
            string categoria = Convert.ToString(TestDatabase.EscalarOrNull("SELECT TOP (1) Nombre FROM dbo.Categoria WHERE UsuarioId IS NULL ORDER BY Id"));
            UiFind.SeleccionarItemCombo(ventana, "cmbCategoriaMovimiento", categoria);
            UiFind.SeleccionarItemCombo(ventana, "cmbTipoMovimiento", Tr(ingreso ? "FormMovements.Value.Type.Income" : "FormMovements.Value.Type.Expense"));
            UiFind.EscribirTexto(ventana, "txtbMontoMovimiento", "12");
            UiFind.EscribirTexto(ventana, "txtbDescripcionMovimiento", descripcion);
            // ValuePattern no reproduce el foco de un usuario. Preparar el estado
            // de sugerencia explicitamente; no esperar un Leave supuesto.
            TestContext.WriteLine("Preparacion: solicitar sugerencia para la descripcion unica.");
            UiFind.Click(ventana, "btnSugerirCategoriaMovimiento");
            EsperarAvisoYCerrar("FormMovements.Warning.NoCategorySuggested");
            TestContext.WriteLine("Preparacion: aviso de sugerencia cerrado; crear movimiento.");
            UiFind.Click(ventana, "btnAgregarMovimiento");
            EsperarAvisoYCerrar("FormMovements.Success.MovementCreated");
        }

        private static void Buscar(Window ventana, string texto)
        {
            UiFind.EscribirTexto(ventana, "txtbDescripcionFiltro", texto);
            UiFind.Click(ventana, "btnBuscarMovimientos");
        }

        private static void EsperarFilas(Window ventana, int cantidad)
        {
            Assert.IsTrue(Retry.WhileFalse(() => UiFind.ContarFilasGrilla(ventana, "dgvMovimientos") == cantidad,
                TimeSpan.FromSeconds(10)).Success, "Cantidad de movimientos inesperada.");
        }

        private static void EsperarContador(Window ventana, int marcados, int total)
        {
            string esperado = string.Format(Tr("FormMovements.SelectionCount"), marcados, total);
            Assert.IsTrue(Retry.WhileFalse(() => UiFind.Elemento(ventana, "lblSeleccionMovimientos").Name == esperado,
                TimeSpan.FromSeconds(5)).Success, "Contador de seleccion inesperado: " + esperado);
        }

        private Window EsperarGuardar(Window ventana)
        {
            // El dialogo comun es otra ventana del proceso, no un hijo UIA del MDI.
            return App.EsperarVentanaPorTitulo(Tr("FormMovements.Export.SaveTitle"));
        }

        private static string Tr(string key) => TestDatabase.ObtenerTraduccion(key);
    }
}
