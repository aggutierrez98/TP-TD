using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
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
        private readonly ISesion sesion = CompositionRoot.Sesion;
        private bool cargandoCategorias;

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
            RefrescarDatos();
        }

        // View-model plano para la grilla.
        private sealed class FilaMovimiento
        {
            public int Id { get; set; }
            public DateTime Fecha { get; set; }
            public string Tipo { get; set; }
            public string Categoria { get; set; }
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
                        Categoria = nombresCategoria.ContainsKey(item.CategoriaId) ? nombresCategoria[item.CategoriaId] : string.Empty,
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
                movimientoBLL.Crear(categoriaId, tipo, monto, dtpFechaMovimiento.Value.Date, txtbDescripcionMovimiento.Text);
                LimpiarEdicion();
                RefrescarDatos();
                UiDialogHelper.MostrarInfo("FormMovements.Success.MovementCreated");
            }
            catch (Exception ex)
            {
                UiExceptionHandler.Mostrar(ex, "Alta de movimiento");
            }
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
                bool actualizado = movimientoBLL.Actualizar(fila.Id, categoriaId, tipo, monto, dtpFechaMovimiento.Value.Date, txtbDescripcionMovimiento.Text);
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
        }
    }
}
