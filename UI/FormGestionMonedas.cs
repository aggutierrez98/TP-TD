using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using BE;
using BLL;

namespace TpIDS
{
    // Pantalla de operador (back-office): seccion "Monedas" de solo lectura (el usuario
    // solo pidio poder verlas) y seccion "Tasas de Conversion" con CRUD completo, en el
    // mismo Form (igual criterio de dos secciones que FormGestionComercios).
    public partial class FormGestionMonedas : FormTraducibleBase
    {
        private readonly MonedaBLL monedaBLL = CompositionRoot.Moneda;
        private readonly TasaConversionBLL tasaConversionBLL = CompositionRoot.TasaConversion;

        public FormGestionMonedas()
        {
            InitializeComponent();
            Load += FormGestionMonedas_Load;
        }

        // View-models planos para bindear las grillas.
        private sealed class FilaMoneda
        {
            public int Id { get; set; }
            public string Codigo { get; set; }
            public string Nombre { get; set; }
        }

        private sealed class FilaTasa
        {
            public int Id { get; set; }
            public int MonedaBaseId { get; set; }
            public int MonedaConversionId { get; set; }
            public string MonedaBase { get; set; }
            public string MonedaConversion { get; set; }
            public decimal Calculo { get; set; }
            public DateTime FechaActualizacion { get; set; }
        }

        private void FormGestionMonedas_Load(object sender, EventArgs e)
        {
            CargarMonedas();
            CargarTasas(null);
        }

        // --- Monedas (solo lectura) ---

        private void CargarMonedas()
        {
            try
            {
                List<Moneda> monedas = monedaBLL.ObtenerTodas()
                    .OrderBy(item => item.Codigo)
                    .ToList();

                dgvMonedas.DataSource = null;
                dgvMonedas.DataSource = monedas.Select(ProyectarFilaMoneda).ToList();

                cmbMonedaBase.DataSource = null;
                cmbMonedaBase.DisplayMember = "Codigo";
                cmbMonedaBase.ValueMember = "Id";
                cmbMonedaBase.DataSource = monedas.ToList();

                cmbMonedaConversion.DataSource = null;
                cmbMonedaConversion.DisplayMember = "Codigo";
                cmbMonedaConversion.ValueMember = "Id";
                cmbMonedaConversion.DataSource = monedas.ToList();

                cmbMonedaBase.SelectedIndex = -1;
                cmbMonedaConversion.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                UiExceptionHandler.Mostrar(ex, "Carga de monedas");
            }
        }

        private static FilaMoneda ProyectarFilaMoneda(Moneda moneda)
        {
            return new FilaMoneda { Id = moneda.Id, Codigo = moneda.Codigo, Nombre = moneda.Nombre };
        }

        // --- Tasas de Conversion ---

        private void CargarTasas(int? tasaSeleccionadaId)
        {
            try
            {
                List<TasaConversion> tasas = tasaConversionBLL.ObtenerTodas();

                dgvTasas.DataSource = null;
                dgvTasas.DataSource = tasas.Select(ProyectarFilaTasa).ToList();

                if (tasaSeleccionadaId.HasValue)
                {
                    SeleccionarFila<FilaTasa>(dgvTasas, fila => fila.Id == tasaSeleccionadaId.Value);
                }
            }
            catch (Exception ex)
            {
                UiExceptionHandler.Mostrar(ex, "Carga de tasas de conversion");
            }
        }

        private static FilaTasa ProyectarFilaTasa(TasaConversion tasa)
        {
            return new FilaTasa
            {
                Id = tasa.Id,
                MonedaBaseId = tasa.MonedaBase?.Id ?? 0,
                MonedaConversionId = tasa.MonedaConversion?.Id ?? 0,
                MonedaBase = tasa.MonedaBase?.Codigo ?? string.Empty,
                MonedaConversion = tasa.MonedaConversion?.Codigo ?? string.Empty,
                Calculo = tasa.CalculoConversion,
                FechaActualizacion = tasa.FechaActualizacion
            };
        }

        private void btnAgregarTasa_Click(object sender, EventArgs e)
        {
            if (!(cmbMonedaBase.SelectedItem is Moneda monedaBase))
            {
                UiDialogHelper.MostrarWarning("FormCurrencyManagement.Warning.SelectMonedaBase");
                return;
            }

            if (!(cmbMonedaConversion.SelectedItem is Moneda monedaConversion))
            {
                UiDialogHelper.MostrarWarning("FormCurrencyManagement.Warning.SelectMonedaConversion");
                return;
            }

            try
            {
                TasaConversion creada = tasaConversionBLL.Crear(monedaBase.Id, monedaConversion.Id, nudCalculo.Value);
                LimpiarFormularioTasa();
                CargarTasas(creada.Id);
                UiDialogHelper.MostrarInfo("FormCurrencyManagement.Success.TasaCreated");
            }
            catch (Exception ex)
            {
                UiExceptionHandler.Mostrar(ex, "Alta de tasa de conversion");
            }
        }

        private void btnActualizarTasa_Click(object sender, EventArgs e)
        {
            if (!ObtenerFilaSeleccionada(dgvTasas, out FilaTasa fila))
            {
                UiDialogHelper.MostrarWarning("FormCurrencyManagement.Warning.SelectTasa");
                return;
            }

            if (!(cmbMonedaBase.SelectedItem is Moneda monedaBase))
            {
                UiDialogHelper.MostrarWarning("FormCurrencyManagement.Warning.SelectMonedaBase");
                return;
            }

            if (!(cmbMonedaConversion.SelectedItem is Moneda monedaConversion))
            {
                UiDialogHelper.MostrarWarning("FormCurrencyManagement.Warning.SelectMonedaConversion");
                return;
            }

            try
            {
                bool actualizado = tasaConversionBLL.Actualizar(fila.Id, monedaBase.Id, monedaConversion.Id, nudCalculo.Value);
                if (actualizado)
                {
                    CargarTasas(fila.Id);
                    UiDialogHelper.MostrarInfo("FormCurrencyManagement.Success.TasaUpdated");
                }
            }
            catch (Exception ex)
            {
                UiExceptionHandler.Mostrar(ex, "Edicion de tasa de conversion");
            }
        }

        private void btnEliminarTasa_Click(object sender, EventArgs e)
        {
            if (!ObtenerFilaSeleccionada(dgvTasas, out FilaTasa fila))
            {
                UiDialogHelper.MostrarWarning("FormCurrencyManagement.Warning.SelectTasa");
                return;
            }

            DialogResult confirmacion = UiDialogHelper.Confirmar(
                "FormCurrencyManagement.Confirm.DeleteTasaMessage",
                "FormCurrencyManagement.Confirm.DeleteTasaTitle",
                MessageBoxIcon.Warning);

            if (confirmacion != DialogResult.Yes)
            {
                return;
            }

            try
            {
                bool eliminado = tasaConversionBLL.Eliminar(fila.Id);
                if (eliminado)
                {
                    LimpiarFormularioTasa();
                    CargarTasas(null);
                    UiDialogHelper.MostrarInfo("FormCurrencyManagement.Success.TasaDeleted");
                }
            }
            catch (Exception ex)
            {
                UiExceptionHandler.Mostrar(ex, "Baja de tasa de conversion");
            }
        }

        private void dgvTasas_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
            {
                return;
            }

            if (ObtenerFilaSeleccionada(dgvTasas, out FilaTasa fila))
            {
                SeleccionarMonedaEnCombo(cmbMonedaBase, fila.MonedaBaseId);
                SeleccionarMonedaEnCombo(cmbMonedaConversion, fila.MonedaConversionId);
                nudCalculo.Value = fila.Calculo;
            }
        }

        private static void SeleccionarMonedaEnCombo(ComboBox combo, int monedaId)
        {
            for (int i = 0; i < combo.Items.Count; i++)
            {
                if (combo.Items[i] is Moneda moneda && moneda.Id == monedaId)
                {
                    combo.SelectedIndex = i;
                    return;
                }
            }

            combo.SelectedIndex = -1;
        }

        private void LimpiarFormularioTasa()
        {
            cmbMonedaBase.SelectedIndex = -1;
            cmbMonedaConversion.SelectedIndex = -1;
            nudCalculo.Value = nudCalculo.Minimum;
        }

        // --- Helpers comunes ---

        private static bool ObtenerFilaSeleccionada<T>(DataGridView grid, out T fila) where T : class
        {
            fila = grid.CurrentRow?.DataBoundItem as T;
            return fila != null;
        }

        private static void SeleccionarFila<T>(DataGridView grid, Func<T, bool> predicado) where T : class
        {
            DataGridViewRow fila = grid.Rows
                .Cast<DataGridViewRow>()
                .FirstOrDefault(item => item.DataBoundItem is T dato && predicado(dato));

            if (fila == null)
            {
                return;
            }

            grid.ClearSelection();
            fila.Selected = true;
            if (fila.Cells.Count > 0)
            {
                grid.CurrentCell = fila.Cells[0];
            }
        }
    }
}
