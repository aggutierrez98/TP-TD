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

        public FormMovimientos()
        {
            InitializeComponent();
            Load += FormMovimientos_Load;
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
                    Tipo = ObtenerTipoSeleccionado(cmbTipoFiltro)
                };

                List<Movimiento> movimientos = movimientoBLL.ObtenerPorUsuario(usuarioActualId, filtro);
                Dictionary<int, string> nombresCategoria = categoriaBLL
                    .ObtenerVisiblesParaUsuario(usuarioActualId)
                    .ToDictionary(item => item.Id, item => item.Nombre);

                dgvMovimientos.DataSource = null;
                dgvMovimientos.DataSource = movimientos
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
            }
            catch (Exception ex)
            {
                UiExceptionHandler.Mostrar(ex, "Consulta de movimientos");
            }
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
                cursorEsperaAntesEscaneo = UseWaitCursor;

                groupBoxFiltros.Enabled = false;
                groupBoxEdicion.Enabled = false;
                dgvMovimientos.Enabled = false;
                UseWaitCursor = true;
                pnlEscaneoTicket.Visible = true;
                pnlEscaneoTicket.BringToFront();
                return;
            }

            pnlEscaneoTicket.Visible = false;
            groupBoxFiltros.Enabled = filtrosHabilitadosAntesEscaneo;
            groupBoxEdicion.Enabled = edicionHabilitadaAntesEscaneo;
            dgvMovimientos.Enabled = grillaHabilitadaAntesEscaneo;
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
            if (e.RowIndex < 0)
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

        private static TipoMovimiento? ObtenerTipoSeleccionado(ComboBox combo)
        {
            OpcionTraducible opcion = combo.SelectedItem as OpcionTraducible;
            string valor = opcion?.Valor as string;
            if (string.IsNullOrWhiteSpace(valor))
            {
                return null;
            }

            return (TipoMovimiento)Enum.Parse(typeof(TipoMovimiento), valor);
        }

        private static void SeleccionarTipo(ComboBox combo, string valor)
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
            if (cmbComercioMovimiento.DataSource is List<Comercio> lista && lista.Count > 0 && lista[0].Id == 0)
            {
                lista[0].Nombre = UiDialogHelper.Traducir("FormMovements.Value.Commerce.None");
            }
        }
    }
}
