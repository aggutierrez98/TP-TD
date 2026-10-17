using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using ABSTRACCIONES;
using BE;
using BLL;

namespace TpIDS
{
    public partial class FormReglasCategorizacion : FormTraducibleBase
    {
        private readonly ReglaCategorizacionBLL reglaBLL = CompositionRoot.ReglaCategorizacion;
        private readonly CategoriaBLL categoriaBLL = CompositionRoot.Categoria;
        private readonly ComercioBLL comercioBLL = CompositionRoot.Comercio;
        private readonly ISesion sesion = CompositionRoot.Sesion;

        public FormReglasCategorizacion()
        {
            InitializeComponent();
            Load += FormReglasCategorizacion_Load;
        }

        private void FormReglasCategorizacion_Load(object sender, EventArgs e)
        {
            CargarCategorias();
            CargarComercios();
            CargarReglas(null);
        }

        // View-model plano para la grilla de reglas.
        private sealed class FilaRegla
        {
            public int Id { get; set; }
            public string Nombre { get; set; }
            public int CategoriaId { get; set; }
            public string Categoria { get; set; }
            public string PalabrasClave { get; set; }
            public string Comercios { get; set; }
            public List<int> ComercioIds { get; set; }
        }

        private void CargarCategorias()
        {
            int usuarioActualId = sesion.GetUsuarioActualId().Value;
            List<Categoria> categorias = categoriaBLL.ObtenerVisiblesParaUsuario(usuarioActualId);

            cmbCategoriaRegla.DataSource = null;
            cmbCategoriaRegla.DisplayMember = "Nombre";
            cmbCategoriaRegla.ValueMember = "Id";
            cmbCategoriaRegla.DataSource = categorias;
        }

        private void CargarComercios()
        {
            List<Comercio> comercios = comercioBLL.ObtenerTodos();

            lstbComerciosRegla.Items.Clear();
            if (comercios != null)
            {
                foreach (Comercio comercio in comercios.OrderBy(c => c.Nombre))
                {
                    lstbComerciosRegla.Items.Add(comercio);
                }
            }
            lstbComerciosRegla.DisplayMember = "Nombre";
        }

        private void CargarReglas(int? reglaSeleccionadaId)
        {
            try
            {
                int usuarioActualId = sesion.GetUsuarioActualId().Value;
                List<ReglaCategorizacion> reglas = reglaBLL.ObtenerPorUsuario(usuarioActualId);

                Dictionary<int, string> nombresCategoria = categoriaBLL
                    .ObtenerVisiblesParaUsuario(usuarioActualId)
                    .ToDictionary(item => item.Id, item => item.Nombre);

                Dictionary<int, string> nombresComercio = (comercioBLL.ObtenerTodos() ?? new List<Comercio>())
                    .ToDictionary(item => item.Id, item => item.Nombre);

                string textoTodosComercios = UiDialogHelper.Traducir("FormCategorizationRules.Value.AllMerchants");

                List<FilaRegla> filas = new List<FilaRegla>();
                foreach (ReglaCategorizacion regla in reglas)
                {
                    List<int> comercioIds = reglaBLL.ObtenerComercioIdsPorRegla(regla.Id) ?? new List<int>();

                    string textoComercios;
                    if (comercioIds.Count == 0)
                    {
                        textoComercios = textoTodosComercios;
                    }
                    else
                    {
                        List<string> nombres = comercioIds
                            .Where(id => nombresComercio.ContainsKey(id))
                            .Select(id => nombresComercio[id])
                            .ToList();
                        textoComercios = string.Join(", ", nombres);
                    }

                    string nombreCategoria = regla.Categoria != null && nombresCategoria.ContainsKey(regla.Categoria.Id)
                        ? nombresCategoria[regla.Categoria.Id]
                        : string.Empty;

                    filas.Add(new FilaRegla
                    {
                        Id = regla.Id,
                        Nombre = regla.Nombre,
                        CategoriaId = regla.Categoria != null ? regla.Categoria.Id : 0,
                        Categoria = nombreCategoria,
                        PalabrasClave = regla.PalabrasClave,
                        Comercios = textoComercios,
                        ComercioIds = comercioIds
                    });
                }

                dgvReglas.DataSource = null;
                dgvReglas.DataSource = filas;

                if (reglaSeleccionadaId.HasValue)
                {
                    SeleccionarRegla(reglaSeleccionadaId.Value);
                }
            }
            catch (Exception ex)
            {
                UiExceptionHandler.Mostrar(ex, "Consulta de reglas de categorizacion");
            }
        }

        private void btnAgregarRegla_Click(object sender, EventArgs e)
        {
            if (!(cmbCategoriaRegla.SelectedItem is Categoria categoria))
            {
                UiDialogHelper.MostrarWarning("FormCategorizationRules.Warning.SelectCategory");
                return;
            }

            try
            {
                List<int> comercioIds = ObtenerComerciosSeleccionados();
                int id = reglaBLL.Crear(txtbNombreRegla.Text, categoria.Id, txtbPalabrasClaveRegla.Text, comercioIds);
                LimpiarCampos();
                CargarReglas(id);
                UiDialogHelper.MostrarInfo("FormCategorizationRules.Success.RuleCreated");
            }
            catch (Exception ex)
            {
                UiExceptionHandler.Mostrar(ex, "Alta de regla de categorizacion");
            }
        }

        private void btnActualizarRegla_Click(object sender, EventArgs e)
        {
            if (!ObtenerFilaSeleccionada(out FilaRegla fila))
            {
                return;
            }

            if (!(cmbCategoriaRegla.SelectedItem is Categoria categoria))
            {
                UiDialogHelper.MostrarWarning("FormCategorizationRules.Warning.SelectCategory");
                return;
            }

            try
            {
                List<int> comercioIds = ObtenerComerciosSeleccionados();
                bool actualizado = reglaBLL.Actualizar(fila.Id, txtbNombreRegla.Text, categoria.Id, txtbPalabrasClaveRegla.Text, comercioIds);
                if (actualizado)
                {
                    LimpiarCampos();
                    CargarReglas(fila.Id);
                    UiDialogHelper.MostrarInfo("FormCategorizationRules.Success.RuleUpdated");
                }
            }
            catch (Exception ex)
            {
                UiExceptionHandler.Mostrar(ex, "Edicion de regla de categorizacion");
            }
        }

        private void btnEliminarRegla_Click(object sender, EventArgs e)
        {
            if (!ObtenerFilaSeleccionada(out FilaRegla fila))
            {
                return;
            }

            DialogResult confirmacion = UiDialogHelper.Confirmar(
                "FormCategorizationRules.Confirm.DeleteRuleMessage",
                "FormCategorizationRules.Confirm.DeleteRuleTitle",
                MessageBoxIcon.Warning,
                fila.Nombre);

            if (confirmacion != DialogResult.Yes)
            {
                return;
            }

            try
            {
                bool eliminado = reglaBLL.Eliminar(fila.Id);
                if (eliminado)
                {
                    LimpiarCampos();
                    CargarReglas(null);
                    UiDialogHelper.MostrarInfo("FormCategorizationRules.Success.RuleDeleted");
                }
            }
            catch (Exception ex)
            {
                UiExceptionHandler.Mostrar(ex, "Baja de regla de categorizacion");
            }
        }

        private void btnLimpiarRegla_Click(object sender, EventArgs e)
        {
            LimpiarCampos();
        }

        private void dgvReglas_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
            {
                return;
            }

            if (ObtenerFilaSeleccionada(out FilaRegla fila))
            {
                txtbNombreRegla.Text = fila.Nombre;
                txtbPalabrasClaveRegla.Text = fila.PalabrasClave;
                cmbCategoriaRegla.SelectedValue = fila.CategoriaId;

                HashSet<int> comercioIdsSet = new HashSet<int>(fila.ComercioIds ?? new List<int>());
                for (int i = 0; i < lstbComerciosRegla.Items.Count; i++)
                {
                    if (lstbComerciosRegla.Items[i] is Comercio c)
                    {
                        lstbComerciosRegla.SetItemChecked(i, comercioIdsSet.Contains(c.Id));
                    }
                }
            }
        }

        private void LimpiarCampos()
        {
            txtbNombreRegla.Clear();
            txtbPalabrasClaveRegla.Clear();
            if (cmbCategoriaRegla.Items.Count > 0)
            {
                cmbCategoriaRegla.SelectedIndex = 0;
            }
            for (int i = 0; i < lstbComerciosRegla.Items.Count; i++)
            {
                lstbComerciosRegla.SetItemChecked(i, false);
            }
            dgvReglas.ClearSelection();
        }

        private List<int> ObtenerComerciosSeleccionados()
        {
            List<int> ids = new List<int>();
            foreach (object item in lstbComerciosRegla.CheckedItems)
            {
                if (item is Comercio c)
                {
                    ids.Add(c.Id);
                }
            }
            return ids;
        }

        private bool ObtenerFilaSeleccionada(out FilaRegla fila)
        {
            fila = dgvReglas.CurrentRow?.DataBoundItem as FilaRegla;
            if (fila == null)
            {
                UiDialogHelper.MostrarWarning("FormCategorizationRules.Warning.SelectRule");
                return false;
            }

            return true;
        }

        private void SeleccionarRegla(int id)
        {
            DataGridViewRow fila = dgvReglas.Rows
                .Cast<DataGridViewRow>()
                .FirstOrDefault(item => item.DataBoundItem is FilaRegla filaRegla && filaRegla.Id == id);

            if (fila == null)
            {
                return;
            }

            dgvReglas.ClearSelection();
            fila.Selected = true;
            if (fila.Cells.Count > 0)
            {
                dgvReglas.CurrentCell = fila.Cells[0];
            }
        }
    }
}
