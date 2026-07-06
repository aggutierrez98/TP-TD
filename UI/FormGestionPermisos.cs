using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using ABSTRACCIONES;
using BLL;
using SERVICIOS;

namespace TpIDS
{
    public partial class FormGestionPermisos : FormTraducibleBase
    {
        private readonly PermisoBLL permisoBLL = new PermisoBLL();
        private readonly PermisoService permisoService = new PermisoService();

        private List<IPermiso> catalogoRaiz;
        private IPermiso familiaActiva;

        public FormGestionPermisos()
        {
            catalogoRaiz = new List<IPermiso>();
            InitializeComponent();
            Load += FormGestionPermisos_Load;
        }

        private void FormGestionPermisos_Load(object sender, EventArgs e)
        {
            CargarCatalogo();
        }

        private void CargarCatalogo()
        {
            Dictionary<int, IPermiso> cache = new Dictionary<int, IPermiso>();
            catalogoRaiz = permisoBLL.ObtenerCatalogo()
                .Select(item => ClonarNodo(item, cache))
                .ToList();

            familiaActiva = null;
            txtbNombreFamilia.Clear();
            Refrescar();
        }

        private IPermiso ClonarNodo(IPermiso origen, IDictionary<int, IPermiso> cache)
        {
            int id = origen.Id;
            if (id > 0 && cache.ContainsKey(id))
            {
                return cache[id];
            }

            IPermiso copia = origen.EsGrupo ? (IPermiso)new PermisoGrupo() : new PermisoSimple();
            copia.Id = id;
            copia.Nombre = origen.Nombre;

            if (id > 0)
            {
                cache[id] = copia;
            }

            foreach (IPermiso hijo in origen.Hijos)
            {
                copia.Agregar(ClonarNodo(hijo, cache));
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
            foreach (IPermiso familia in catalogoRaiz)
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
            foreach (IPermiso familia in permisoService.FiltrarFamilias(catalogoRaiz))
            {
                if (!ReferenceEquals(familia, familiaActiva))
                {
                    lstbFamiliasDisponibles.Items.Add(familia);
                }
            }

            lstbFamiliasDisponibles.EndUpdate();

            lstbPatentesDisponibles.BeginUpdate();
            lstbPatentesDisponibles.Items.Clear();
            foreach (IPermiso patente in permisoService.FiltrarPatentes(catalogoRaiz))
            {
                lstbPatentesDisponibles.Items.Add(patente);
            }

            lstbPatentesDisponibles.EndUpdate();
        }

        // Lee la seleccion por indice contra Items en vez de SelectedItem: tras
        // reconstruir la lista (Clear + Add en RefrescarListas) WinForms deja una
        // SelectedObjectCollection cacheada e inconsistente, y SelectedItem lanza
        // IndexOutOfRangeException. SelectedIndex + Items evita ese camino.
        private static IPermiso ObtenerSeleccion(ListBox lista)
        {
            int indice = lista.SelectedIndex;
            if (indice < 0 || indice >= lista.Items.Count)
            {
                return null;
            }

            return lista.Items[indice] as IPermiso;
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

            IPermiso seleccionCatalogo = tvwCatalogoPermisos.SelectedNode?.Tag as IPermiso;
            btnEliminarFamilia.Enabled = seleccionCatalogo != null && seleccionCatalogo.EsGrupo;
        }

        private TreeNode CrearNodoTreeView(IPermiso permiso)
        {
            TreeNode nodo = new TreeNode(permiso.Nombre)
            {
                Tag = permiso
            };

            foreach (IPermiso hijo in permiso.Hijos)
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
            IPermiso permiso = e.Node.Tag as IPermiso;

            if (permiso != null && permiso.EsGrupo)
            {
                familiaActiva = permiso;
                txtbNombreFamilia.Text = permiso.Nombre;
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

            familiaActiva.Nombre = txtbNombreFamilia.Text;

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

            IPermiso familia = ObtenerSeleccion(lstbFamiliasDisponibles);
            if (familia == null)
            {
                UiDialogHelper.MostrarWarning("FormPermissionManagement.Warning.SelectFamilyFromList");
                return;
            }

            // CU01 6a: un ciclo en memoria rompe los recorridos recursivos del arbol,
            // asi que se rechaza aca y no recien al guardar.
            if (permisoService.GeneraCiclo(familiaActiva, familia))
            {
                UiDialogHelper.MostrarWarning("Error.Permission.CycleDetected");
                return;
            }

            familiaActiva.Agregar(familia);
            Refrescar();
        }

        private void btnAgregarPatenteHija_Click(object sender, EventArgs e)
        {
            if (familiaActiva == null)
            {
                UiDialogHelper.MostrarWarning("FormPermissionManagement.Warning.SelectFamilyToEdit");
                return;
            }

            IPermiso patente = ObtenerSeleccion(lstbPatentesDisponibles);
            if (patente == null)
            {
                UiDialogHelper.MostrarWarning("FormPermissionManagement.Warning.SelectPatentFromList");
                return;
            }

            familiaActiva.Agregar(patente);
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

            IPermiso hijo = seleccionado.Tag as IPermiso;
            IPermiso padre = seleccionado.Parent.Tag as IPermiso;
            if (hijo == null || padre == null)
            {
                return;
            }

            padre.Quitar(hijo);
            Refrescar();
        }

        private void btnEliminarFamilia_Click(object sender, EventArgs e)
        {
            IPermiso permiso = tvwCatalogoPermisos.SelectedNode?.Tag as IPermiso;
            if (permiso == null || !permiso.EsGrupo)
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
                foreach (IPermiso raiz in catalogoRaiz)
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

        private bool QuitarNodo(IPermiso padre, IPermiso objetivo)
        {
            foreach (IPermiso hijo in padre.Hijos)
            {
                if (ReferenceEquals(hijo, objetivo))
                {
                    padre.Quitar(hijo);
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
