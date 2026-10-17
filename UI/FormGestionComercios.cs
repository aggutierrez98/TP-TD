using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using BE;
using BLL;

namespace TpIDS
{
    // Pantalla de operador (back-office): dos secciones CRUD independientes en el mismo
    // Form (Comercios y Rubros de Comercio), sin dueno de usuario (a diferencia de
    // Categoria/ListaDeCompras). La seccion Rubros alimenta el combo de la seccion
    // Comercios; al agregar/editar/eliminar un Rubro se refresca ese combo y la grilla
    // de Comercios (por el nombre de rubro mostrado), sin obligar a cerrar y reabrir.
    public partial class FormGestionComercios : FormTraducibleBase
    {
        private readonly ComercioBLL comercioBLL = CompositionRoot.Comercio;
        private readonly RubroComercioBLL rubroComercioBLL = CompositionRoot.RubroComercio;

        // Coordenadas elegidas en FormSeleccionarUbicacionComercio para el alta/edicion
        // en curso. Se guardan aparte porque no hay control de texto atado a ellas: el
        // dialogo del mapa las devuelve y aca solo se muestran en lblCoordenadasComercio.
        private decimal? _latitudSeleccionada;
        private decimal? _longitudSeleccionada;

        public FormGestionComercios()
        {
            InitializeComponent();
            Load += FormGestionComercios_Load;
        }

        // View-models planos para bindear las grillas.
        private sealed class FilaComercio
        {
            public int Id { get; set; }
            public string Nombre { get; set; }
            public string Rubro { get; set; }
            public string Direccion { get; set; }
            public string Localidad { get; set; }
            public string Provincia { get; set; }
            public decimal? Latitud { get; set; }
            public decimal? Longitud { get; set; }
        }

        private sealed class FilaRubro
        {
            public int Id { get; set; }
            public string Nombre { get; set; }
        }

        private void FormGestionComercios_Load(object sender, EventArgs e)
        {
            CargarRubros(null);
            CargarComercios(null);
            ActualizarLabelCoordenadas();
        }

        // --- Comercios ---

        private void CargarComercios(int? comercioSeleccionadoId)
        {
            try
            {
                List<Comercio> comercios = comercioBLL.ObtenerTodos();

                dgvComercios.DataSource = null;
                dgvComercios.DataSource = comercios
                    .OrderBy(item => item.Nombre)
                    .Select(ProyectarFilaComercio)
                    .ToList();

                if (comercioSeleccionadoId.HasValue)
                {
                    SeleccionarFila<FilaComercio>(dgvComercios, fila => fila.Id == comercioSeleccionadoId.Value);
                }
            }
            catch (Exception ex)
            {
                UiExceptionHandler.Mostrar(ex, "Carga de comercios");
            }
        }

        private static FilaComercio ProyectarFilaComercio(Comercio comercio)
        {
            return new FilaComercio
            {
                Id = comercio.Id,
                Nombre = comercio.Nombre,
                Rubro = comercio.RubroComercio?.Nombre ?? string.Empty,
                Direccion = comercio.Direccion,
                Localidad = comercio.Localidad,
                Provincia = comercio.Provincia,
                Latitud = comercio.Latitud,
                Longitud = comercio.Longitud
            };
        }

        private void btnAgregarComercio_Click(object sender, EventArgs e)
        {
            if (!(cmbRubroComercio.SelectedItem is RubroComercio rubroSeleccionado))
            {
                UiDialogHelper.MostrarWarning("FormMerchantManagement.Warning.SelectRubro");
                return;
            }

            try
            {
                Comercio creado = comercioBLL.Crear(
                    txtbNombreComercio.Text,
                    rubroSeleccionado.Id,
                    txtbDireccionComercio.Text,
                    txtbLocalidadComercio.Text,
                    txtbProvinciaComercio.Text,
                    _latitudSeleccionada,
                    _longitudSeleccionada);
                LimpiarFormularioComercio();
                CargarComercios(creado.Id);
                UiDialogHelper.MostrarInfo("FormMerchantManagement.Success.ComercioCreated");
            }
            catch (Exception ex)
            {
                UiExceptionHandler.Mostrar(ex, "Alta de comercio");
            }
        }

        private void btnActualizarComercio_Click(object sender, EventArgs e)
        {
            if (!ObtenerFilaSeleccionada(dgvComercios, out FilaComercio fila))
            {
                UiDialogHelper.MostrarWarning("FormMerchantManagement.Warning.SelectComercio");
                return;
            }

            if (!(cmbRubroComercio.SelectedItem is RubroComercio rubroSeleccionado))
            {
                UiDialogHelper.MostrarWarning("FormMerchantManagement.Warning.SelectRubro");
                return;
            }

            try
            {
                bool actualizado = comercioBLL.Actualizar(
                    fila.Id,
                    txtbNombreComercio.Text,
                    rubroSeleccionado.Id,
                    txtbDireccionComercio.Text,
                    txtbLocalidadComercio.Text,
                    txtbProvinciaComercio.Text,
                    _latitudSeleccionada,
                    _longitudSeleccionada);
                if (actualizado)
                {
                    CargarComercios(fila.Id);
                    UiDialogHelper.MostrarInfo("FormMerchantManagement.Success.ComercioUpdated");
                }
            }
            catch (Exception ex)
            {
                UiExceptionHandler.Mostrar(ex, "Edicion de comercio");
            }
        }

        private void btnEliminarComercio_Click(object sender, EventArgs e)
        {
            if (!ObtenerFilaSeleccionada(dgvComercios, out FilaComercio fila))
            {
                UiDialogHelper.MostrarWarning("FormMerchantManagement.Warning.SelectComercio");
                return;
            }

            DialogResult confirmacion = UiDialogHelper.Confirmar(
                "FormMerchantManagement.Confirm.DeleteComercioMessage",
                "FormMerchantManagement.Confirm.DeleteComercioTitle",
                MessageBoxIcon.Warning,
                fila.Nombre);

            if (confirmacion != DialogResult.Yes)
            {
                return;
            }

            try
            {
                bool eliminado = comercioBLL.Eliminar(fila.Id);
                if (eliminado)
                {
                    LimpiarFormularioComercio();
                    CargarComercios(null);
                    UiDialogHelper.MostrarInfo("FormMerchantManagement.Success.ComercioDeleted");
                }
            }
            catch (Exception ex)
            {
                UiExceptionHandler.Mostrar(ex, "Baja de comercio");
            }
        }

        private void btnVerDetalleComercio_Click(object sender, EventArgs e)
        {
            if (!ObtenerFilaSeleccionada(dgvComercios, out FilaComercio fila))
            {
                UiDialogHelper.MostrarWarning("FormMerchantManagement.Warning.SelectComercio");
                return;
            }

            using (FormDetalleComercio formDetalle = new FormDetalleComercio(fila.Id, fila.Nombre))
            {
                formDetalle.ShowDialog(this);
            }
        }

        private void btnUbicarComercioEnMapa_Click(object sender, EventArgs e)
        {
            using (FormSeleccionarUbicacionComercio formMapa = new FormSeleccionarUbicacionComercio(_latitudSeleccionada, _longitudSeleccionada, CompositionRoot.BusquedaUbicaciones))
            {
                if (formMapa.ShowDialog(this) == DialogResult.OK)
                {
                    _latitudSeleccionada = formMapa.LatitudSeleccionada;
                    _longitudSeleccionada = formMapa.LongitudSeleccionada;
                    ActualizarLabelCoordenadas();
                }
            }
        }

        private void ActualizarLabelCoordenadas()
        {
            lblCoordenadasComercio.Text = (_latitudSeleccionada.HasValue && _longitudSeleccionada.HasValue)
                ? string.Format(CultureInfo.InvariantCulture, "{0:0.000000}, {1:0.000000}", _latitudSeleccionada.Value, _longitudSeleccionada.Value)
                : UiDialogHelper.Traducir("FormMerchantManagement.SinUbicarLabel");
        }

        private void dgvComercios_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
            {
                return;
            }

            if (ObtenerFilaSeleccionada(dgvComercios, out FilaComercio fila))
            {
                txtbNombreComercio.Text = fila.Nombre;
                txtbDireccionComercio.Text = fila.Direccion;
                txtbLocalidadComercio.Text = fila.Localidad;
                txtbProvinciaComercio.Text = fila.Provincia;
                SeleccionarRubroEnCombo(fila.Rubro);
                _latitudSeleccionada = fila.Latitud;
                _longitudSeleccionada = fila.Longitud;
                ActualizarLabelCoordenadas();
            }
        }

        private void SeleccionarRubroEnCombo(string nombreRubro)
        {
            for (int i = 0; i < cmbRubroComercio.Items.Count; i++)
            {
                if (cmbRubroComercio.Items[i] is RubroComercio rubro && string.Equals(rubro.Nombre, nombreRubro, StringComparison.Ordinal))
                {
                    cmbRubroComercio.SelectedIndex = i;
                    return;
                }
            }
        }

        private void LimpiarFormularioComercio()
        {
            txtbNombreComercio.Clear();
            txtbDireccionComercio.Clear();
            txtbLocalidadComercio.Clear();
            txtbProvinciaComercio.Clear();
            _latitudSeleccionada = null;
            _longitudSeleccionada = null;
            ActualizarLabelCoordenadas();
        }

        // --- Rubros ---

        private void CargarRubros(int? rubroSeleccionadoId)
        {
            try
            {
                List<RubroComercio> rubros = rubroComercioBLL.ObtenerTodas()
                    .OrderBy(item => item.Nombre)
                    .ToList();

                cmbRubroComercio.DataSource = null;
                cmbRubroComercio.DisplayMember = "Nombre";
                cmbRubroComercio.ValueMember = "Id";
                cmbRubroComercio.DataSource = rubros;

                dgvRubros.DataSource = null;
                dgvRubros.DataSource = rubros.Select(ProyectarFilaRubro).ToList();

                if (rubroSeleccionadoId.HasValue)
                {
                    SeleccionarFila<FilaRubro>(dgvRubros, fila => fila.Id == rubroSeleccionadoId.Value);
                }
            }
            catch (Exception ex)
            {
                UiExceptionHandler.Mostrar(ex, "Carga de rubros de comercio");
            }
        }

        private static FilaRubro ProyectarFilaRubro(RubroComercio rubro)
        {
            return new FilaRubro { Id = rubro.Id, Nombre = rubro.Nombre };
        }

        private void btnAgregarRubro_Click(object sender, EventArgs e)
        {
            try
            {
                RubroComercio creado = rubroComercioBLL.Crear(txtbNombreRubro.Text);
                txtbNombreRubro.Clear();
                CargarRubros(creado.Id);
                CargarComercios(null);
                UiDialogHelper.MostrarInfo("FormMerchantManagement.Success.RubroCreated");
            }
            catch (Exception ex)
            {
                UiExceptionHandler.Mostrar(ex, "Alta de rubro de comercio");
            }
        }

        private void btnActualizarRubro_Click(object sender, EventArgs e)
        {
            if (!ObtenerFilaSeleccionada(dgvRubros, out FilaRubro fila))
            {
                UiDialogHelper.MostrarWarning("FormMerchantManagement.Warning.SelectRubro");
                return;
            }

            try
            {
                bool actualizado = rubroComercioBLL.Actualizar(fila.Id, txtbNombreRubro.Text);
                if (actualizado)
                {
                    CargarRubros(fila.Id);
                    CargarComercios(null);
                    UiDialogHelper.MostrarInfo("FormMerchantManagement.Success.RubroUpdated");
                }
            }
            catch (Exception ex)
            {
                UiExceptionHandler.Mostrar(ex, "Edicion de rubro de comercio");
            }
        }

        private void btnEliminarRubro_Click(object sender, EventArgs e)
        {
            if (!ObtenerFilaSeleccionada(dgvRubros, out FilaRubro fila))
            {
                UiDialogHelper.MostrarWarning("FormMerchantManagement.Warning.SelectRubro");
                return;
            }

            DialogResult confirmacion = UiDialogHelper.Confirmar(
                "FormMerchantManagement.Confirm.DeleteRubroMessage",
                "FormMerchantManagement.Confirm.DeleteRubroTitle",
                MessageBoxIcon.Warning,
                fila.Nombre);

            if (confirmacion != DialogResult.Yes)
            {
                return;
            }

            try
            {
                bool eliminado = rubroComercioBLL.Eliminar(fila.Id);
                if (eliminado)
                {
                    txtbNombreRubro.Clear();
                    CargarRubros(null);
                    CargarComercios(null);
                    UiDialogHelper.MostrarInfo("FormMerchantManagement.Success.RubroDeleted");
                }
            }
            catch (Exception ex)
            {
                UiExceptionHandler.Mostrar(ex, "Baja de rubro de comercio");
            }
        }

        private void dgvRubros_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
            {
                return;
            }

            if (ObtenerFilaSeleccionada(dgvRubros, out FilaRubro fila))
            {
                txtbNombreRubro.Text = fila.Nombre;
            }
        }

        // --- Helpers comunes a ambas grillas ---

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
