using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using BLL;
using SERVICIOS;

namespace TpIDS
{
    public partial class FormGestionPermisos : FormTraducibleBase
    {
        private readonly PermisoBLL permisoBLL = new PermisoBLL();
        private readonly PermisoService permisoService = new PermisoService();

        private List<object> catalogoRaiz;
        private object familiaActiva;

        public FormGestionPermisos()
        {
            catalogoRaiz = new List<object>();
            InitializeComponent();
            Load += FormGestionPermisos_Load;
        }

        private void FormGestionPermisos_Load(object sender, EventArgs e)
        {
            CargarCatalogo();
        }

        private void CargarCatalogo()
        {
            Dictionary<int, object> cache = new Dictionary<int, object>();
            catalogoRaiz = permisoBLL.ObtenerCatalogo()
                .Select(item => ClonarNodo(item, cache))
                .ToList();

            familiaActiva = null;
            txtbNombreFamilia.Clear();
            Refrescar();
        }

        private object ClonarNodo(object origen, IDictionary<int, object> cache)
        {
            int id = permisoService.ObtenerId(origen);
            if (id > 0 && cache.ContainsKey(id))
            {
                return cache[id];
            }

            object copia = permisoService.EsGrupo(origen)
                ? (object)new PermisoGrupo()
                : new PermisoSimple();
            permisoService.EstablecerId(copia, id);
            permisoService.EstablecerNombre(copia, permisoService.ObtenerNombre(origen));

            if (id > 0)
            {
                cache[id] = copia;
            }

            foreach (object hijo in permisoService.ObtenerHijos(origen))
            {
                permisoService.AgregarHijo(copia, ClonarNodo(hijo, cache));
            }

            return copia;
        }

        // Refresca arbol del catalogo, listas de disponibles, preview y estado de botones.
        private void Refrescar()
        {
            RefrescarArbol();
            RefrescarListas();
            RefrescarPreview();
            ActualizarEstadoControles();
        }

        private void RefrescarArbol()
        {
            tvwCatalogoPermisos.AfterSelect -= tvwCatalogoPermisos_AfterSelect;

            tvwCatalogoPermisos.Nodes.Clear();
            TreeNode raiz = new TreeNode("Raiz") { Tag = null };
            foreach (object familia in catalogoRaiz)
            {
                raiz.Nodes.Add(CrearNodoTreeView(familia));
            }

            tvwCatalogoPermisos.Nodes.Add(raiz);
            tvwCatalogoPermisos.ExpandAll();
            SeleccionarNodoFamiliaActiva(tvwCatalogoPermisos.Nodes);

            tvwCatalogoPermisos.AfterSelect += tvwCatalogoPermisos_AfterSelect;
        }

        private void RefrescarListas()
        {
            lstbFamiliasDisponibles.BeginUpdate();
            lstbFamiliasDisponibles.Items.Clear();
            foreach (object familia in permisoService.FiltrarFamilias(catalogoRaiz))
            {
                if (!ReferenceEquals(familia, familiaActiva))
                {
                    lstbFamiliasDisponibles.Items.Add(familia);
                }
            }

            lstbFamiliasDisponibles.EndUpdate();

            lstbPatentesDisponibles.BeginUpdate();
            lstbPatentesDisponibles.Items.Clear();
            foreach (object patente in permisoService.FiltrarPatentes(catalogoRaiz))
            {
                lstbPatentesDisponibles.Items.Add(patente);
            }

            lstbPatentesDisponibles.EndUpdate();
        }

        private void RefrescarPreview()
        {
            tvwPreviewFamilia.Nodes.Clear();
            if (familiaActiva == null)
            {
                return;
            }

            tvwPreviewFamilia.Nodes.Add(CrearNodoTreeView(familiaActiva));
            tvwPreviewFamilia.ExpandAll();
        }

        private void ActualizarEstadoControles()
        {
            bool hayFamiliaActiva = familiaActiva != null;

            btnCrearFamilia.Enabled = !hayFamiliaActiva;
            btnAgregarFamiliaHija.Enabled = hayFamiliaActiva;
            btnAgregarPatenteHija.Enabled = hayFamiliaActiva;
            btnEliminarSeleccionado.Enabled = hayFamiliaActiva;
            btnGuardarFamilia.Enabled = hayFamiliaActiva;

            object seleccionCatalogo = tvwCatalogoPermisos.SelectedNode?.Tag;
            btnEliminarFamilia.Enabled = seleccionCatalogo != null && permisoService.EsGrupo(seleccionCatalogo);
        }

        private TreeNode CrearNodoTreeView(object permiso)
        {
            TreeNode nodo = new TreeNode(permisoService.ObtenerNombre(permiso))
            {
                Tag = permiso
            };

            foreach (object hijo in permisoService.ObtenerHijos(permiso))
            {
                nodo.Nodes.Add(CrearNodoTreeView(hijo));
            }

            return nodo;
        }

        private void SeleccionarNodoFamiliaActiva(TreeNodeCollection nodos)
        {
            if (familiaActiva == null)
            {
                return;
            }

            foreach (TreeNode nodo in nodos)
            {
                if (ReferenceEquals(nodo.Tag, familiaActiva))
                {
                    tvwCatalogoPermisos.SelectedNode = nodo;
                    return;
                }

                SeleccionarNodoFamiliaActiva(nodo.Nodes);
            }
        }

        private void tvwCatalogoPermisos_AfterSelect(object sender, TreeViewEventArgs e)
        {
            object permiso = e.Node.Tag;

            if (permiso != null && permisoService.EsGrupo(permiso))
            {
                familiaActiva = permiso;
                txtbNombreFamilia.Text = permisoService.ObtenerNombre(permiso);
            }
            else
            {
                // Nodo Raiz o patente: no se edita. Vuelve al modo "nueva familia".
                familiaActiva = null;
                txtbNombreFamilia.Clear();
            }

            RefrescarListas();
            RefrescarPreview();
            ActualizarEstadoControles();
        }

        private void txtbNombreFamilia_TextChanged(object sender, EventArgs e)
        {
            if (familiaActiva == null)
            {
                return;
            }

            permisoService.EstablecerNombre(familiaActiva, txtbNombreFamilia.Text);

            if (tvwPreviewFamilia.Nodes.Count > 0)
            {
                tvwPreviewFamilia.Nodes[0].Text = txtbNombreFamilia.Text;
            }

            if (tvwCatalogoPermisos.SelectedNode != null &&
                ReferenceEquals(tvwCatalogoPermisos.SelectedNode.Tag, familiaActiva))
            {
                tvwCatalogoPermisos.SelectedNode.Text = txtbNombreFamilia.Text;
            }
        }

        private void btnCrearFamilia_Click(object sender, EventArgs e)
        {
            string nombre = txtbNombreFamilia.Text.Trim();
            if (!ValidarNombreFamilia(nombre))
            {
                return;
            }

            PermisoGrupo nuevaFamilia = new PermisoGrupo { Nombre = nombre };
            catalogoRaiz.Add(nuevaFamilia);
            familiaActiva = nuevaFamilia;

            Refrescar();
        }

        private void btnAgregarFamiliaHija_Click(object sender, EventArgs e)
        {
            if (familiaActiva == null)
            {
                UiDialogHelper.MostrarWarning("FormPermissionManagement.Warning.SelectFamilyToEdit");
                return;
            }

            object familia = lstbFamiliasDisponibles.SelectedItem;
            if (familia == null)
            {
                UiDialogHelper.MostrarWarning("FormPermissionManagement.Warning.SelectFamilyFromList");
                return;
            }

            permisoService.AgregarHijo(familiaActiva, familia);
            Refrescar();
        }

        private void btnAgregarPatenteHija_Click(object sender, EventArgs e)
        {
            if (familiaActiva == null)
            {
                UiDialogHelper.MostrarWarning("FormPermissionManagement.Warning.SelectFamilyToEdit");
                return;
            }

            object patente = lstbPatentesDisponibles.SelectedItem;
            if (patente == null)
            {
                UiDialogHelper.MostrarWarning("FormPermissionManagement.Warning.SelectPatentFromList");
                return;
            }

            permisoService.AgregarHijo(familiaActiva, patente);
            Refrescar();
        }

        private void btnEliminarSeleccionado_Click(object sender, EventArgs e)
        {
            TreeNode seleccionado = tvwPreviewFamilia.SelectedNode;
            if (familiaActiva == null || seleccionado == null || seleccionado.Parent == null)
            {
                UiDialogHelper.MostrarWarning("FormPermissionManagement.Warning.SelectChildToRemove");
                return;
            }

            object hijo = seleccionado.Tag;
            object padre = seleccionado.Parent.Tag;
            if (hijo == null || padre == null)
            {
                return;
            }

            permisoService.QuitarHijo(padre, hijo);
            Refrescar();
        }

        private void btnEliminarFamilia_Click(object sender, EventArgs e)
        {
            object permiso = tvwCatalogoPermisos.SelectedNode?.Tag;
            if (permiso == null || !permisoService.EsGrupo(permiso))
            {
                UiDialogHelper.MostrarWarning("FormPermissionManagement.Warning.SelectFamilyToDelete");
                return;
            }

            DialogResult confirmacion = UiDialogHelper.Confirmar(
                "FormPermissionManagement.Confirm.DeleteFamilyMessage",
                "FormPermissionManagement.Confirm.DeleteFamilyTitle",
                MessageBoxIcon.Warning);

            if (confirmacion != DialogResult.Yes)
            {
                return;
            }

            if (!catalogoRaiz.Remove(permiso))
            {
                foreach (object raiz in catalogoRaiz)
                {
                    if (QuitarNodo(raiz, permiso))
                    {
                        break;
                    }
                }
            }

            if (ReferenceEquals(permiso, familiaActiva))
            {
                familiaActiva = null;
                txtbNombreFamilia.Clear();
            }

            Refrescar();
        }

        private bool QuitarNodo(object padre, object objetivo)
        {
            foreach (object hijo in permisoService.ObtenerHijos(padre))
            {
                if (ReferenceEquals(hijo, objetivo))
                {
                    permisoService.QuitarHijo(padre, hijo);
                    return true;
                }

                if (QuitarNodo(hijo, objetivo))
                {
                    return true;
                }
            }

            return false;
        }

        private void btnGuardarFamilia_Click(object sender, EventArgs e)
        {
            try
            {
                permisoBLL.GuardarCatalogo(catalogoRaiz);
                CargarCatalogo();
                UiDialogHelper.MostrarInfo("FormPermissionManagement.Success.CatalogSaved");
            }
            catch (Exception ex)
            {
                UiExceptionHandler.Mostrar(ex, "Guardado de familia de permisos");
            }
        }

        private bool ValidarNombreFamilia(string nombre)
        {
            if (string.IsNullOrWhiteSpace(nombre))
            {
                UiDialogHelper.MostrarWarning("FormPermissionManagement.Warning.NameRequired");
                return false;
            }

            if (nombre.Contains("."))
            {
                UiDialogHelper.MostrarWarning("FormPermissionManagement.Warning.NameContainsDot");
                return false;
            }

            return true;
        }
    }
}
