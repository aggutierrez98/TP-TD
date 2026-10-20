using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using ABSTRACCIONES;
using BE;
using BLL;
using SERVICIOS;

namespace TpIDS
{
    public partial class FormMovimientos : FormTraducibleBase
    {
        private readonly MovimientoBLL movimientoBLL = CompositionRoot.Movimiento;
        private readonly CategoriaBLL categoriaBLL = CompositionRoot.Categoria;
        private readonly ComercioBLL comercioBLL = CompositionRoot.Comercio;
        private readonly ReglaCategorizacionBLL reglaCategorizacionBLL = CompositionRoot.ReglaCategorizacion;
        private readonly ISesion sesion = CompositionRoot.Sesion;
        private bool cargandoCategorias;
        private bool filtrosHabilitadosAntesEscaneo;
        private bool edicionHabilitadaAntesEscaneo;
        private bool grillaHabilitadaAntesEscaneo;
        private bool cursorEsperaAntesEscaneo;
        private string ultimaDescripcionSugerida;
        private int? ultimoComercioIdSugerido;
        private bool evaluandoSugerencia;
        private readonly HashSet<int> movimientosMarcados = new HashSet<int>();
        private FiltroMovimiento filtroAplicado;
        private string categoriaAplicada;
        private int usuarioConsulta;
        private bool cambiandoMarcas;
        private bool exportandoPdf;
        private bool seleccionHabilitadaAntesEscaneo;

        public FormMovimientos()
        {
            InitializeComponent();
            TemaFinanzapp.TematizarBarraProgreso(prgbEscaneoTicket);
            Load += FormMovimientos_Load;
            FormClosing += FormMovimientos_FormClosing;
        }

        private void FormMovimientos_Load(object sender, EventArgs e)
        {
            dgvMovimientos.CellFormatting += dgvMovimientos_CellFormatting;
            CargarTipos();
            CargarCategorias();
            CargarComercios();
            RefrescarDatos();
        }

        // View-model plano para la grilla.
        private sealed class FilaMovimiento
        {
            public int Id { get; set; }
            public DateTime Fecha { get; set; }
            public string Tipo { get; set; }
            public string Categoria { get; set; }
            public string Comercio { get; set; }
            public int? ComercioId { get; set; }
            public decimal Monto { get; set; }
            public string Descripcion { get; set; }
        }

        private void CargarTipos()
        {
            cmbTipoFiltro.DisplayMember = "Texto";
            cmbTipoFiltro.Items.Clear();
            cmbTipoFiltro.Items.Add(new OpcionTraducible(string.Empty, string.Empty));
            cmbTipoFiltro.Items.Add(new OpcionTraducible("FormMovements.Value.Type.Income", TipoMovimiento.Ingreso.ToString()));
            cmbTipoFiltro.Items.Add(new OpcionTraducible("FormMovements.Value.Type.Expense", TipoMovimiento.Egreso.ToString()));
            cmbTipoFiltro.SelectedIndex = 0;

            cmbTipoMovimiento.DisplayMember = "Texto";
            cmbTipoMovimiento.Items.Clear();
            cmbTipoMovimiento.Items.Add(new OpcionTraducible("FormMovements.Value.Type.Income", TipoMovimiento.Ingreso.ToString()));
            cmbTipoMovimiento.Items.Add(new OpcionTraducible("FormMovements.Value.Type.Expense", TipoMovimiento.Egreso.ToString()));
            cmbTipoMovimiento.SelectedIndex = 0;
        }

        private void CargarCategorias()
        {
            cargandoCategorias = true;

            int usuarioActualId = sesion.GetUsuarioActualId().Value;
            List<Categoria> categorias = categoriaBLL.ObtenerVisiblesParaUsuario(usuarioActualId);

            cmbCategoriaFiltro.DataSource = null;
            cmbCategoriaFiltro.DisplayMember = "Nombre";
            cmbCategoriaFiltro.ValueMember = "Id";
            List<Categoria> conVacio = new List<Categoria> { new Categoria { Id = 0, Nombre = string.Empty } };
            conVacio.AddRange(categorias);
            cmbCategoriaFiltro.DataSource = conVacio;

            cmbCategoriaMovimiento.DataSource = null;
            cmbCategoriaMovimiento.DisplayMember = "Nombre";
            cmbCategoriaMovimiento.ValueMember = "Id";
            cmbCategoriaMovimiento.DataSource = categorias;

            cargandoCategorias = false;
        }

        private void btnBuscarMovimientos_Click(object sender, EventArgs e)
        {
            RefrescarDatos();
        }

        private void txtbDescripcionFiltro_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter) return;
            e.Handled = true;
            e.SuppressKeyPress = true;
            RefrescarDatos();
        }

        private void RefrescarDatos()
        {
            try
            {
                int usuarioActualId = sesion.GetUsuarioActualId().Value;

                FiltroMovimiento filtro = new FiltroMovimiento
                {
                    FechaDesde = chkbFechaDesdeMovimientos.Checked ? (DateTime?)dtpFechaDesdeMovimientos.Value.Date : null,
                    FechaHasta = chkbFechaHastaMovimientos.Checked ? (DateTime?)dtpFechaHastaMovimientos.Value.Date : null,
                    CategoriaId = ObtenerCategoriaFiltroSeleccionada(),
                    Tipo = ObtenerTipoSeleccionado(cmbTipoFiltro),
                    Texto = string.IsNullOrWhiteSpace(txtbDescripcionFiltro.Text) ? null : txtbDescripcionFiltro.Text.Trim()
                };

                List<Movimiento> movimientos = movimientoBLL.ObtenerPorUsuario(usuarioActualId, filtro);
                Dictionary<int, string> nombresCategoria = categoriaBLL
                    .ObtenerVisiblesParaUsuario(usuarioActualId)
                    .ToDictionary(item => item.Id, item => item.Nombre);

                var filas = movimientos
                    .Select(item => new FilaMovimiento
                    {
                        Id = item.Id,
                        Fecha = item.Fecha,
                        Tipo = item.Tipo.ToString(),
                        Categoria = nombresCategoria.ContainsKey(item.Categoria.Id) ? nombresCategoria[item.Categoria.Id] : string.Empty,
                        Comercio = item.Comercio != null ? item.Comercio.Nombre : string.Empty,
                        ComercioId = item.Comercio != null ? (int?)item.Comercio.Id : null,
                        Monto = item.Monto,
                        Descripcion = item.Descripcion
                    })
                    .ToList();

                // Obtener todo antes de reemplazar el listado: una consulta fallida
                // conserva resultado, marcas y criterios anteriores juntos.
                cambiandoMarcas = true;
                try
                {
                    dgvMovimientos.DataSource = filas;
                    foreach (DataGridViewRow row in dgvMovimientos.Rows)
                        row.Cells[colMovimientosSeleccionar.Index].Value = false;
                    filtroAplicado = filtro;
                    categoriaAplicada = filtro.CategoriaId.HasValue && nombresCategoria.ContainsKey(filtro.CategoriaId.Value)
                        ? nombresCategoria[filtro.CategoriaId.Value] : string.Empty;
                    usuarioConsulta = usuarioActualId;
                    movimientosMarcados.Clear();
                }
                finally { cambiandoMarcas = false; }
                ActualizarSeleccionMovimientos();
            }
            catch (Exception ex)
            {
                UiExceptionHandler.Mostrar(ex, "Consulta de movimientos");
            }
        }

        private void dgvMovimientos_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (dgvMovimientos.IsCurrentCellDirty && dgvMovimientos.CurrentCell?.OwningColumn == colMovimientosSeleccionar)
                dgvMovimientos.CommitEdit(DataGridViewDataErrorContexts.Commit);
        }

        private void dgvMovimientos_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (cambiandoMarcas || e.RowIndex < 0 || e.ColumnIndex != colMovimientosSeleccionar.Index) return;
            var row = dgvMovimientos.Rows[e.RowIndex];
            if (!(row.DataBoundItem is FilaMovimiento fila)) return;
            if (Equals(row.Cells[colMovimientosSeleccionar.Index].Value, true)) movimientosMarcados.Add(fila.Id);
            else movimientosMarcados.Remove(fila.Id);
            ActualizarSeleccionMovimientos();
        }

        private void btnSeleccionarTodosMovimientos_Click(object sender, EventArgs e)
        {
            MarcarMovimientos(true);
        }

        private void btnDesmarcarTodosMovimientos_Click(object sender, EventArgs e)
        {
            MarcarMovimientos(false);
        }

        private void MarcarMovimientos(bool marcar)
        {
            dgvMovimientos.EndEdit();
            cambiandoMarcas = true;
            try
            {
                movimientosMarcados.Clear();
                foreach (DataGridViewRow row in dgvMovimientos.Rows)
                {
                    row.Cells[colMovimientosSeleccionar.Index].Value = marcar;
                    if (marcar && row.DataBoundItem is FilaMovimiento fila) movimientosMarcados.Add(fila.Id);
                }
            }
            finally { cambiandoMarcas = false; }
            ActualizarSeleccionMovimientos();
        }

        private void ActualizarSeleccionMovimientos()
        {
            lblSeleccionMovimientos.Values.Text = UiDialogHelper.Traducir("FormMovements.SelectionCount",
                movimientosMarcados.Count, dgvMovimientos.Rows.Count);
            btnExportarMovimientosPdf.Enabled = movimientosMarcados.Count > 0 && !exportandoPdf;
            btnSeleccionarTodosMovimientos.Enabled = dgvMovimientos.Rows.Count > 0 && !exportandoPdf;
            btnDesmarcarTodosMovimientos.Enabled = movimientosMarcados.Count > 0 && !exportandoPdf;
        }

        private ReporteMovimientosPdf CapturarReporteMovimientos()
        {
            dgvMovimientos.EndEdit();
            var columnas = new[] { colMovimientosFecha, colMovimientosTipo, colMovimientosCategoria,
                colMovimientosComercio, colMovimientosMonto, colMovimientosDescripcion };
            var filas = dgvMovimientos.Rows.Cast<DataGridViewRow>()
                .Where(row => row.DataBoundItem is FilaMovimiento fila && movimientosMarcados.Contains(fila.Id))
                .Select(row => columnas.Select(col => Convert.ToString(row.Cells[col.Index].FormattedValue,
                    CultureInfo.CurrentCulture)).ToArray()).ToArray();
            string sinLimite = UiDialogHelper.Traducir("FormMovements.Report.NoLimit");
            string todos = UiDialogHelper.Traducir("FormMovements.Report.All");
            var filtros = new[]
            {
                UiDialogHelper.Traducir("FormMovements.Report.From", filtroAplicado.FechaDesde?.ToString("d") ?? sinLimite),
                UiDialogHelper.Traducir("FormMovements.Report.To", filtroAplicado.FechaHasta?.ToString("d") ?? sinLimite),
                UiDialogHelper.Traducir("FormMovements.Report.Category", filtroAplicado.CategoriaId.HasValue ? categoriaAplicada : todos),
                UiDialogHelper.Traducir("FormMovements.Report.Type", filtroAplicado.Tipo.HasValue ?
                    UiDialogHelper.Traducir(filtroAplicado.Tipo.Value == TipoMovimiento.Ingreso ?
                        "FormMovements.Value.Type.Income" : "FormMovements.Value.Type.Expense") : todos),
                UiDialogHelper.Traducir("FormMovements.Report.Description", filtroAplicado.Texto ??
                    UiDialogHelper.Traducir("FormMovements.Report.NoText"))
            };
            byte[] logo;
            using (var resource = typeof(FormMovimientos).Assembly.GetManifestResourceStream("TpIDS.Resources.logo_reporte_finanzapp.png"))
            using (var buffer = new MemoryStream())
            {
                if (resource == null) throw new ExcepcionTecnica("Error.Movement.ExportFailed");
                resource.CopyTo(buffer);
                logo = buffer.ToArray();
            }
            return new ReporteMovimientosPdf(usuarioConsulta, UiDialogHelper.Traducir("FormMovements.Report.Title"),
                columnas.Select(col => col.HeaderText), filas, filtros, logo, DateTime.Now,
                UiDialogHelper.Traducir("FormMovements.Report.Generated"), UiDialogHelper.Traducir("FormMovements.Report.Count"),
                UiDialogHelper.Traducir("FormMovements.Report.PageFormat"), UiDialogHelper.Traducir("FormMovements.Report.Continuation"));
        }

        private async void btnExportarMovimientosPdf_Click(object sender, EventArgs e)
        {
            if (exportandoPdf || movimientosMarcados.Count == 0 || filtroAplicado == null) return;
            using (var dialogo = new SaveFileDialog
            {
                Title = UiDialogHelper.Traducir("FormMovements.Export.SaveTitle"),
                Filter = "PDF (*.pdf)|*.pdf", DefaultExt = "pdf", AddExtension = true,
                OverwritePrompt = true, FileName = "FINAZAPP-movimientos-" + DateTime.Now.ToString("yyyyMMdd-HHmmss") + ".pdf"
            })
            {
                if (dialogo.ShowDialog(this) != DialogResult.OK) return;
                bool filtrosAntes = groupBoxFiltros.Enabled;
                bool edicionAntes = groupBoxEdicion.Enabled;
                bool grillaAntes = dgvMovimientos.Enabled;
                bool seleccionAntes = tlpSeleccionMovimientos.Enabled;
                bool cursorAntes = UseWaitCursor;
                try
                {
                    var reporte = CapturarReporteMovimientos();
                    string destino = dialogo.FileName;
                    exportandoPdf = true;
                    groupBoxFiltros.Enabled = groupBoxEdicion.Enabled = dgvMovimientos.Enabled = tlpSeleccionMovimientos.Enabled = false;
                    UseWaitCursor = true;
                    await Task.Run(() => ArchivoPdf.Guardar(destino, movimientoBLL.ExportarPdf(reporte)));
                    UiDialogHelper.MostrarInfo("FormMovements.Export.Success", destino);
                }
                catch (Exception ex)
                {
                    UiExceptionHandler.Mostrar(ex is IOException || ex is UnauthorizedAccessException
                        ? new ExcepcionTecnica("Error.Movement.ExportWriteFailed", ex) : ex, "Exportacion de movimientos");
                }
                finally
                {
                    exportandoPdf = false;
                    groupBoxFiltros.Enabled = filtrosAntes;
                    groupBoxEdicion.Enabled = edicionAntes;
                    dgvMovimientos.Enabled = grillaAntes;
                    tlpSeleccionMovimientos.Enabled = seleccionAntes;
                    UseWaitCursor = cursorAntes;
                    ActualizarSeleccionMovimientos();
                }
            }
        }

        private void FormMovimientos_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (exportandoPdf) e.Cancel = true;
        }

        private void btnAgregarMovimiento_Click(object sender, EventArgs e)
        {
            if (!ValidarFormularioEdicion(out int categoriaId, out TipoMovimiento tipo, out decimal monto))
            {
                return;
            }

            try
            {
                int? comercioId = ObtenerComercioSeleccionado();
                movimientoBLL.Crear(categoriaId, tipo, monto, dtpFechaMovimiento.Value.Date, txtbDescripcionMovimiento.Text, comercioId);
                LimpiarEdicion();
                RefrescarDatos();
                UiDialogHelper.MostrarInfo("FormMovements.Success.MovementCreated");
            }
            catch (Exception ex)
            {
                UiExceptionHandler.Mostrar(ex, "Alta de movimiento");
            }
        }

        private async void btnEscanearTicket_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog dialogo = new OpenFileDialog())
            {
                dialogo.Filter = "Imagenes (*.jpg;*.jpeg;*.png)|*.jpg;*.jpeg;*.png";
                dialogo.Title = UiDialogHelper.Traducir("FormMovements.Scan.SelectFileTitle");
                dialogo.CheckFileExists = true;

                if (dialogo.ShowDialog(this) != DialogResult.OK)
                {
                    return;
                }

                CambiarEstadoEscaneoTicket(true);

                try
                {
                    byte[] imagenBytes = File.ReadAllBytes(dialogo.FileName);
                    EscaneoBorradorResultado resultado = await movimientoBLL.EscanearBorradorAsync(imagenBytes);
                    Movimiento borrador = resultado.Borrador;

                    SeleccionarTipo(cmbTipoMovimiento, borrador.Tipo.ToString());
                    dtpFechaMovimiento.Value = borrador.Fecha;
                    txtbDescripcionMovimiento.Text = borrador.Descripcion;
                    EvaluarSugerenciaCategoria(mostrarMensajeSiNoCoincide: false);

                    if (borrador.Monto > 0)
                    {
                        txtbMontoMovimiento.Text = borrador.Monto.ToString(CultureInfo.InvariantCulture);
                    }
                    else
                    {
                        txtbMontoMovimiento.Clear();
                        UiDialogHelper.MostrarWarning("FormMovements.Warning.ScanAmountNotDetected", resultado.TextoCrudo);
                    }
                }
                catch (Exception ex)
                {
                    UiExceptionHandler.Mostrar(ex, "Escaneo de ticket");
                }
                finally
                {
                    CambiarEstadoEscaneoTicket(false);
                }
            }
        }

        private void CambiarEstadoEscaneoTicket(bool enCurso)
        {
            if (enCurso)
            {
                filtrosHabilitadosAntesEscaneo = groupBoxFiltros.Enabled;
                edicionHabilitadaAntesEscaneo = groupBoxEdicion.Enabled;
                grillaHabilitadaAntesEscaneo = dgvMovimientos.Enabled;
                seleccionHabilitadaAntesEscaneo = tlpSeleccionMovimientos.Enabled;
                cursorEsperaAntesEscaneo = UseWaitCursor;

                groupBoxFiltros.Enabled = false;
                groupBoxEdicion.Enabled = false;
                dgvMovimientos.Enabled = false;
                tlpSeleccionMovimientos.Enabled = false;
                UseWaitCursor = true;
                pnlEscaneoTicket.Visible = true;
                pnlEscaneoTicket.BringToFront();
                return;
            }

            pnlEscaneoTicket.Visible = false;
            groupBoxFiltros.Enabled = filtrosHabilitadosAntesEscaneo;
            groupBoxEdicion.Enabled = edicionHabilitadaAntesEscaneo;
            dgvMovimientos.Enabled = grillaHabilitadaAntesEscaneo;
            tlpSeleccionMovimientos.Enabled = seleccionHabilitadaAntesEscaneo;
            UseWaitCursor = cursorEsperaAntesEscaneo;
        }

        private void btnActualizarMovimiento_Click(object sender, EventArgs e)
        {
            if (!ObtenerFilaSeleccionada(out FilaMovimiento fila))
            {
                return;
            }

            if (!ValidarFormularioEdicion(out int categoriaId, out TipoMovimiento tipo, out decimal monto))
            {
                return;
            }

            try
            {
                int? comercioId = ObtenerComercioSeleccionado();
                bool actualizado = movimientoBLL.Actualizar(fila.Id, categoriaId, tipo, monto, dtpFechaMovimiento.Value.Date, txtbDescripcionMovimiento.Text, comercioId);
                if (actualizado)
                {
                    LimpiarEdicion();
                    RefrescarDatos();
                    UiDialogHelper.MostrarInfo("FormMovements.Success.MovementUpdated");
                }
            }
            catch (Exception ex)
            {
                UiExceptionHandler.Mostrar(ex, "Edicion de movimiento");
            }
        }

        private void btnEliminarMovimiento_Click(object sender, EventArgs e)
        {
            if (!ObtenerFilaSeleccionada(out FilaMovimiento fila))
            {
                return;
            }

            DialogResult confirmacion = UiDialogHelper.Confirmar(
                "FormMovements.Confirm.DeleteMovementMessage",
                "FormMovements.Confirm.DeleteMovementTitle",
                MessageBoxIcon.Warning);

            if (confirmacion != DialogResult.Yes)
            {
                return;
            }

            try
            {
                bool eliminado = movimientoBLL.Eliminar(fila.Id);
                if (eliminado)
                {
                    LimpiarEdicion();
                    RefrescarDatos();
                    UiDialogHelper.MostrarInfo("FormMovements.Success.MovementDeleted");
                }
            }
            catch (Exception ex)
            {
                UiExceptionHandler.Mostrar(ex, "Baja de movimiento");
            }
        }

        private void dgvMovimientos_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex == colMovimientosSeleccionar.Index)
            {
                return;
            }

            if (ObtenerFilaSeleccionada(out FilaMovimiento fila))
            {
                cmbCategoriaMovimiento.SelectedValue = ObtenerCategoriaIdPorNombre(fila.Categoria);
                SeleccionarTipo(cmbTipoMovimiento, fila.Tipo);
                txtbMontoMovimiento.Text = fila.Monto.ToString(CultureInfo.InvariantCulture);
                dtpFechaMovimiento.Value = fila.Fecha;
                txtbDescripcionMovimiento.Text = fila.Descripcion;

                if (fila.ComercioId.HasValue && fila.ComercioId.Value > 0)
                {
                    cmbComercioMovimiento.SelectedValue = fila.ComercioId.Value;
                }
                else
                {
                    cmbComercioMovimiento.SelectedIndex = 0;
                }

                ultimaDescripcionSugerida = fila.Descripcion;
                ultimoComercioIdSugerido = fila.ComercioId;
            }
        }

        private void dgvMovimientos_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.Value == null || e.RowIndex < 0)
            {
                return;
            }

            if (!string.Equals(dgvMovimientos.Columns[e.ColumnIndex].Name, "Tipo", StringComparison.Ordinal))
            {
                return;
            }

            IIdioma idioma = IdiomaService.GetInstance().IdiomaActual;
            if (idioma == null)
            {
                return;
            }

            string key = string.Equals(e.Value.ToString(), TipoMovimiento.Ingreso.ToString(), StringComparison.Ordinal)
                ? "FormMovements.Value.Type.Income"
                : "FormMovements.Value.Type.Expense";

            e.Value = idioma.ObtenerTraduccion(key);
            e.FormattingApplied = true;
        }

        private bool ValidarFormularioEdicion(out int categoriaId, out TipoMovimiento tipo, out decimal monto)
        {
            categoriaId = 0;
            tipo = TipoMovimiento.Ingreso;
            monto = 0;

            if (!(cmbCategoriaMovimiento.SelectedItem is Categoria categoria))
            {
                UiDialogHelper.MostrarWarning("FormMovements.Warning.SelectCategory");
                return false;
            }

            TipoMovimiento? tipoSeleccionado = ObtenerTipoSeleccionado(cmbTipoMovimiento);
            if (!tipoSeleccionado.HasValue)
            {
                UiDialogHelper.MostrarWarning("FormMovements.Warning.SelectType");
                return false;
            }

            if (!decimal.TryParse(txtbMontoMovimiento.Text, NumberStyles.Number, CultureInfo.InvariantCulture, out decimal montoParseado) || montoParseado <= 0)
            {
                UiDialogHelper.MostrarWarning("FormMovements.Warning.InvalidAmount");
                return false;
            }

            categoriaId = categoria.Id;
            tipo = tipoSeleccionado.Value;
            monto = montoParseado;
            return true;
        }

        private bool ObtenerFilaSeleccionada(out FilaMovimiento fila)
        {
            fila = dgvMovimientos.CurrentRow?.DataBoundItem as FilaMovimiento;
            if (fila == null)
            {
                UiDialogHelper.MostrarWarning("FormMovements.Warning.SelectMovement");
                return false;
            }

            return true;
        }

        private int? ObtenerCategoriaFiltroSeleccionada()
        {
            if (cargandoCategorias || !(cmbCategoriaFiltro.SelectedItem is Categoria categoria) || categoria.Id == 0)
            {
                return null;
            }

            return categoria.Id;
        }

        private int? ObtenerCategoriaIdPorNombre(string nombre)
        {
            return (cmbCategoriaMovimiento.DataSource as List<Categoria>)
                ?.FirstOrDefault(item => string.Equals(item.Nombre, nombre, StringComparison.Ordinal))
                ?.Id;
        }

        private static TipoMovimiento? ObtenerTipoSeleccionado(Krypton.Toolkit.KryptonComboBox combo)
        {
            OpcionTraducible opcion = combo.SelectedItem as OpcionTraducible;
            string valor = opcion?.Valor as string;
            if (string.IsNullOrWhiteSpace(valor))
            {
                return null;
            }

            return (TipoMovimiento)Enum.Parse(typeof(TipoMovimiento), valor);
        }

        private static void SeleccionarTipo(Krypton.Toolkit.KryptonComboBox combo, string valor)
        {
            foreach (object item in combo.Items)
            {
                if (item is OpcionTraducible opcion && string.Equals(opcion.Valor as string, valor, StringComparison.Ordinal))
                {
                    combo.SelectedItem = opcion;
                    return;
                }
            }
        }

        private void LimpiarEdicion()
        {
            txtbMontoMovimiento.Clear();
            txtbDescripcionMovimiento.Clear();
            dtpFechaMovimiento.Value = DateTime.Today;
            cmbTipoMovimiento.SelectedIndex = 0;
            if (cmbComercioMovimiento.Items.Count > 0)
            {
                cmbComercioMovimiento.SelectedIndex = 0;
            }
            ultimaDescripcionSugerida = null;
            ultimoComercioIdSugerido = null;
        }

        private void CargarComercios()
        {
            List<Comercio> comercios = comercioBLL.ObtenerTodos();
            List<Comercio> opciones = new List<Comercio>
            {
                new Comercio { Id = 0, Nombre = UiDialogHelper.Traducir("FormMovements.Value.Commerce.None") }
            };

            if (comercios != null)
            {
                opciones.AddRange(comercios.OrderBy(c => c.Nombre));
            }

            cmbComercioMovimiento.DataSource = null;
            cmbComercioMovimiento.DisplayMember = "Nombre";
            cmbComercioMovimiento.ValueMember = "Id";
            cmbComercioMovimiento.DataSource = opciones;
        }

        private int? ObtenerComercioSeleccionado()
        {
            if (cmbComercioMovimiento.SelectedItem is Comercio comercio && comercio.Id > 0)
            {
                return comercio.Id;
            }

            return null;
        }

        private void btnSugerirCategoriaMovimiento_Click(object sender, EventArgs e)
        {
            EvaluarSugerenciaCategoria(mostrarMensajeSiNoCoincide: true);
        }

        private void txtbDescripcionMovimiento_Leave(object sender, EventArgs e)
        {
            if (evaluandoSugerencia || Disposing || IsDisposed)
            {
                return;
            }

            string descripcionActual = txtbDescripcionMovimiento.Text?.Trim() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(descripcionActual))
            {
                return;
            }

            int? comercioId = ObtenerComercioSeleccionado();
            if (string.Equals(descripcionActual, ultimaDescripcionSugerida, StringComparison.OrdinalIgnoreCase)
                && comercioId == ultimoComercioIdSugerido)
            {
                return;
            }

            EvaluarSugerenciaCategoria(mostrarMensajeSiNoCoincide: true);
        }

        private void EvaluarSugerenciaCategoria(bool mostrarMensajeSiNoCoincide)
        {
            if (evaluandoSugerencia)
            {
                return;
            }

            evaluandoSugerencia = true;
            try
            {
                string descripcion = txtbDescripcionMovimiento.Text?.Trim();
                if (string.IsNullOrWhiteSpace(descripcion))
                {
                    if (mostrarMensajeSiNoCoincide)
                    {
                        UiDialogHelper.MostrarWarning("FormMovements.Warning.DescriptionRequiredForSuggestion");
                    }
                    return;
                }

                int? comercioId = ObtenerComercioSeleccionado();
                ultimaDescripcionSugerida = descripcion;
                ultimoComercioIdSugerido = comercioId;

                Categoria sugerida = reglaCategorizacionBLL.SugerirCategoria(descripcion, comercioId);
                if (sugerida != null)
                {
                    cmbCategoriaMovimiento.SelectedValue = sugerida.Id;
                }
                else
                {
                    if (mostrarMensajeSiNoCoincide)
                    {
                        UiDialogHelper.MostrarWarning("FormMovements.Warning.NoCategorySuggested");
                        cmbCategoriaMovimiento.Focus();
                    }
                }
            }
            catch (Exception ex)
            {
                UiExceptionHandler.Mostrar(ex, "Sugerencia de categoria");
            }
            finally
            {
                evaluandoSugerencia = false;
            }
        }

        public override void Actualizar(IObservado observado)
        {
            base.Actualizar(observado);
            ActualizarSeleccionMovimientos();
            if (cmbComercioMovimiento.DataSource is List<Comercio> lista && lista.Count > 0 && lista[0].Id == 0)
            {
                lista[0].Nombre = UiDialogHelper.Traducir("FormMovements.Value.Commerce.None");
            }
        }
    }
}
