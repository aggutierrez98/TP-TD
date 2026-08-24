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
        private readonly PermisoBLL permisoBLL = CompositionRoot.Permiso;
        private readonly PermisoService permisoService = new PermisoService();

        private List<IPermiso> catalogoRaiz;
        private IPermiso grupoActivo;

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

            grupoActivo = null;
            txtbNombreGrupo.Clear();
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
            foreach (IPermiso grupo in catalogoRaiz)
            {
                raiz.Nodes.Add(CrearNodoTreeView(grupo));
            }

            tvwCatalogoPermisos.Nodes.Add(raiz);
            tvwCatalogoPermisos.ExpandAll();
            SeleccionarNodoGrupoActivo(tvwCatalogoPermisos.Nodes);

            tvwCatalogoPermisos.AfterSelect += tvwCatalogoPermisos_AfterSelect;
        }

        private void RefrescarListas()
        {
            lstbGruposDisponibles.BeginUpdate();
            lstbGruposDisponibles.Items.Clear();
            foreach (IPermiso grupo in permisoService.FiltrarGrupos(catalogoRaiz))
            {
                if (!ReferenceEquals(grupo, grupoActivo))
                {
                    lstbGruposDisponibles.Items.Add(grupo);
                }
            }

            lstbGruposDisponibles.EndUpdate();

            lstbPermisosDisponibles.BeginUpdate();
            lstbPermisosDisponibles.Items.Clear();
            foreach (IPermiso permiso in permisoService.FiltrarPermisos(catalogoRaiz))
            {
                lstbPermisosDisponibles.Items.Add(permiso);
            }

            lstbPermisosDisponibles.EndUpdate();
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
            tvwPreviewGrupo.Nodes.Clear();
            if (grupoActivo == null)
            {
                return;
            }

            tvwPreviewGrupo.Nodes.Add(CrearNodoTreeView(grupoActivo));
            tvwPreviewGrupo.ExpandAll();
        }

        private void ActualizarEstadoControles()
        {
            bool hayGrupoActivo = grupoActivo != null;

            btnCrearGrupo.Enabled = !hayGrupoActivo;
            btnAgregarGrupoHijo.Enabled = hayGrupoActivo;
            btnAgregarPermisoHijo.Enabled = hayGrupoActivo;
            btnEliminarSeleccionado.Enabled = hayGrupoActivo;
            btnGuardarGrupo.Enabled = hayGrupoActivo;

            IPermiso seleccionCatalogo = tvwCatalogoPermisos.SelectedNode?.Tag as IPermiso;
            btnEliminarGrupo.Enabled = seleccionCatalogo != null && seleccionCatalogo.EsGrupo;
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

        private void SeleccionarNodoGrupoActivo(TreeNodeCollection nodos)
        {
            if (grupoActivo == null)
            {
                return;
            }

            foreach (TreeNode nodo in nodos)
            {
                if (ReferenceEquals(nodo.Tag, grupoActivo))
                {
                    tvwCatalogoPermisos.SelectedNode = nodo;
                    return;
                }

                SeleccionarNodoGrupoActivo(nodo.Nodes);
            }
        }

        private void tvwCatalogoPermisos_AfterSelect(object sender, TreeViewEventArgs e)
        {
            IPermiso permiso = e.Node.Tag as IPermiso;

            if (permiso != null && permiso.EsGrupo)
            {
                grupoActivo = permiso;
                txtbNombreGrupo.Text = permiso.Nombre;
            }
            else
            {
                // Nodo Raiz o permiso hoja: no se edita. Vuelve al modo "nuevo grupo".
                grupoActivo = null;
                txtbNombreGrupo.Clear();
            }

            RefrescarListas();
            RefrescarPreview();
            ActualizarEstadoControles();
        }

        private void txtbNombreGrupo_TextChanged(object sender, EventArgs e)
        {
            if (grupoActivo == null)
            {
                return;
            }

            grupoActivo.Nombre = txtbNombreGrupo.Text;

            if (tvwPreviewGrupo.Nodes.Count > 0)
            {
                tvwPreviewGrupo.Nodes[0].Text = txtbNombreGrupo.Text;
            }

            if (tvwCatalogoPermisos.SelectedNode != null &&
                ReferenceEquals(tvwCatalogoPermisos.SelectedNode.Tag, grupoActivo))
            {
                tvwCatalogoPermisos.SelectedNode.Text = txtbNombreGrupo.Text;
            }

            RefrescarListas();
            RefrescarPreview();
            ActualizarEstadoControles();
        }

        private void btnCrearGrupo_Click(object sender, EventArgs e)
        {
            string nombre = txtbNombreGrupo.Text.Trim();
            if (!ValidarNombreGrupo(nombre))
            {
                return;
            }

            PermisoGrupo nuevoGrupo = new PermisoGrupo { Nombre = nombre };
            catalogoRaiz.Add(nuevoGrupo);
            grupoActivo = nuevoGrupo;

            Refrescar();
        }

        private void btnAgregarGrupoHijo_Click(object sender, EventArgs e)
        {
            if (grupoActivo == null)
            {
                UiDialogHelper.MostrarWarning("FormPermissionManagement.Warning.SelectGroupToEdit");
                return;
            }

            IPermiso grupo = ObtenerSeleccion(lstbGruposDisponibles);
            if (grupo == null)
            {
                UiDialogHelper.MostrarWarning("FormPermissionManagement.Warning.SelectGroupFromList");
                return;
            }

            // CU01 6a: un ciclo en memoria rompe los recorridos recursivos del arbol,
            // asi que se rechaza aca y no recien al guardar.
            if (permisoService.GeneraCiclo(grupoActivo, grupo))
            {
                UiDialogHelper.MostrarWarning("Error.Permission.CycleDetected");
                return;
            }

            grupoActivo.Agregar(grupo);
            Refrescar();
        }

        private void btnAgregarPermisoHijo_Click(object sender, EventArgs e)
        {
            if (grupoActivo == null)
            {
                UiDialogHelper.MostrarWarning("FormPermissionManagement.Warning.SelectGroupToEdit");
                return;
            }

            IPermiso permiso = ObtenerSeleccion(lstbPermisosDisponibles);
            if (permiso == null)
            {
                UiDialogHelper.MostrarWarning("FormPermissionManagement.Warning.SelectPermissionFromList");
                return;
            }

            grupoActivo.Agregar(permiso);
            Refrescar();
        }

        private void btnEliminarSeleccionado_Click(object sender, EventArgs e)
        {
            TreeNode seleccionado = tvwPreviewGrupo.SelectedNode;
            if (grupoActivo == null || seleccionado == null || seleccionado.Parent == null)
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

        private void btnEliminarGrupo_Click(object sender, EventArgs e)
        {
            IPermiso permiso = tvwCatalogoPermisos.SelectedNode?.Tag as IPermiso;
            if (permiso == null || !permiso.EsGrupo)
            {
                UiDialogHelper.MostrarWarning("FormPermissionManagement.Warning.SelectGroupToDelete");
                return;
            }

            DialogResult confirmacion = UiDialogHelper.Confirmar(
                "FormPermissionManagement.Confirm.DeleteGroupMessage",
                "FormPermissionManagement.Confirm.DeleteGroupTitle",
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

            if (ReferenceEquals(permiso, grupoActivo))
            {
                grupoActivo = null;
                txtbNombreGrupo.Clear();
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

        private void btnGuardarGrupo_Click(object sender, EventArgs e)
        {
            try
            {
                permisoBLL.GuardarCatalogo(catalogoRaiz);
                CargarCatalogo();
                UiDialogHelper.MostrarInfo("FormPermissionManagement.Success.CatalogSaved");
            }
            catch (Exception ex)
            {
                UiExceptionHandler.Mostrar(ex, "Guardado de grupos y permisos");
            }
        }

        private bool ValidarNombreGrupo(string nombre)
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
