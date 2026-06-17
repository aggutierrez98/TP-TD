using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using BE;
using BLL;
using SERVICIOS;

namespace TpIDS
{
    public partial class FormGestionPermisos : FormTraducibleBase
    {
        private const string PermisoGestionPermisos = "gestionpermisos";

        private readonly PermisoBLL permisoBLL = new PermisoBLL();
        private readonly PermisoService permisoService = new PermisoService();

        private List<object> catalogoRaiz;

        public FormGestionPermisos()
        {
            catalogoRaiz = new List<object>();
            InitializeComponent();
            Load += FormGestionPermisos_Load;
        }

        private void FormGestionPermisos_Load(object sender, EventArgs e)
        {
            CargarTiposPermiso();
            CargarCatalogo();
        }

        // El combo se puebla una sola vez con los valores de dominio; su texto lo traduce
        // IdiomaFormHelper al cambiar el idioma (ver OpcionTraducible), sin hooks por-form.
        private void CargarTiposPermiso()
        {
            cmbTipoPermiso.DisplayMember = "Texto";
            cmbTipoPermiso.Items.Clear();
            cmbTipoPermiso.Items.Add(new OpcionTraducible("FormPermissionManagement.TypeGroup", true));
            cmbTipoPermiso.Items.Add(new OpcionTraducible("FormPermissionManagement.TypeLeaf", false));
            cmbTipoPermiso.SelectedIndex = 0;
        }

        private void CargarCatalogo()
        {
            Dictionary<int, object> cache = new Dictionary<int, object>();
            catalogoRaiz = permisoBLL.ObtenerCatalogo()
                .Select(item => ClonarNodo(item, cache))
                .ToList();

            RefrescarArbol();
        }

        private object ClonarNodo(object origen, IDictionary<int, object> cache)
        {
            dynamic permisoOrigen = origen;

            if ((int)permisoOrigen.Id > 0 && cache.ContainsKey((int)permisoOrigen.Id))
            {
                return cache[(int)permisoOrigen.Id];
            }

            dynamic copia = permisoOrigen.EsGrupo ? (object)new PermisoGrupo() : new PermisoSimple();
            copia.Id = permisoOrigen.Id;
            copia.Nombre = permisoOrigen.Nombre;

            if ((int)permisoOrigen.Id > 0)
            {
                cache[(int)permisoOrigen.Id] = copia;
            }

            foreach (object hijo in permisoOrigen.Hijos)
            {
                permisoService.AgregarHijo(copia, ClonarNodo(hijo, cache));
            }

            return copia;
        }

        private void RefrescarArbol()
        {
            tvwCatalogoPermisos.Nodes.Clear();

            foreach (object raiz in catalogoRaiz)
            {
                tvwCatalogoPermisos.Nodes.Add(CrearNodoTreeView(raiz));
            }

            tvwCatalogoPermisos.ExpandAll();
        }

        private TreeNode CrearNodoTreeView(object permiso)
        {
            dynamic permisoActual = permiso;
            TreeNode nodo = new TreeNode((string)permisoActual.Nombre)
            {
                Tag = permiso
            };

            foreach (object hijo in permisoActual.Hijos)
            {
                nodo.Nodes.Add(CrearNodoTreeView(hijo));
            }

            return nodo;
        }

        private void tvwCatalogoPermisos_AfterSelect(object sender, TreeViewEventArgs e)
        {
            object permiso = e.Node.Tag;
            if (permiso == null)
            {
                return;
            }

            dynamic permisoActual = permiso;
            txtbNombrePermiso.Text = (string)permisoActual.Nombre;
            SeleccionarTipoPermiso((bool)permisoActual.EsGrupo);
        }

        private void SeleccionarTipoPermiso(bool esGrupo)
        {
            foreach (object item in cmbTipoPermiso.Items)
            {
                if (item is OpcionTraducible opcion && opcion.Valor is bool valor && valor == esGrupo)
                {
                    cmbTipoPermiso.SelectedItem = item;
                    return;
                }
            }
        }

        private void btnAgregarRaizPermiso_Click(object sender, EventArgs e)
        {
            try
            {
                object nodo = CrearNodoDesdeFormulario();
                catalogoRaiz.Add(nodo);
                RefrescarArbol();
                txtbNombrePermiso.Clear();
            }
            catch (Exception ex)
            {
                UiExceptionHandler.Mostrar(ex, "Alta de permiso");
            }
        }

        private void btnAgregarHijoPermiso_Click(object sender, EventArgs e)
        {
            object padre = tvwCatalogoPermisos.SelectedNode?.Tag;
            if (padre == null)
            {
                UiDialogHelper.MostrarWarning("FormPermissionManagement.Warning.SelectParentGroup");
                return;
            }

            dynamic grupoPadre = padre;
            if (!grupoPadre.EsGrupo)
            {
                UiDialogHelper.MostrarWarning("FormPermissionManagement.Warning.OnlyGroupsCanContainChildren");
                return;
            }

            try
            {
                permisoService.AgregarHijo(grupoPadre, CrearNodoDesdeFormulario());
                RefrescarArbol();
                txtbNombrePermiso.Clear();
            }
            catch (Exception ex)
            {
                UiExceptionHandler.Mostrar(ex, "Alta de permiso hijo");
            }
        }

        private void btnRenombrarPermiso_Click(object sender, EventArgs e)
        {
            object permiso = tvwCatalogoPermisos.SelectedNode?.Tag;
            if (permiso == null)
            {
                UiDialogHelper.MostrarWarning("FormPermissionManagement.Warning.SelectPermission");
                return;
            }

            string nuevoNombre = txtbNombrePermiso.Text.Trim();
            if (string.IsNullOrWhiteSpace(nuevoNombre))
            {
                UiDialogHelper.MostrarWarning("FormPermissionManagement.Warning.NameRequired");
                return;
            }

            if (nuevoNombre.Contains("."))
            {
                UiDialogHelper.MostrarWarning("FormPermissionManagement.Warning.NameContainsDot");
                return;
            }

            ((dynamic)permiso).Nombre = nuevoNombre;
            RefrescarArbol();
        }

        private void btnEliminarPermiso_Click(object sender, EventArgs e)
        {
            object permiso = tvwCatalogoPermisos.SelectedNode?.Tag;
            if (permiso == null)
            {
                UiDialogHelper.MostrarWarning("FormPermissionManagement.Warning.SelectPermission");
                return;
            }

            DialogResult confirmacion = UiDialogHelper.Confirmar(
                "FormPermissionManagement.Confirm.DeleteNodeMessage",
                "FormPermissionManagement.Confirm.DeleteNodeTitle",
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

            RefrescarArbol();
            txtbNombrePermiso.Clear();
        }

        private bool QuitarNodo(object padre, object objetivo)
        {
            dynamic padreActual = padre;
            foreach (object hijo in ((System.Collections.IEnumerable)padreActual.Hijos).Cast<object>().ToList())
            {
                if (ReferenceEquals(hijo, objetivo))
                {
                    permisoService.QuitarHijo(padreActual, hijo);
                    return true;
                }

                if (QuitarNodo(hijo, objetivo))
                {
                    return true;
                }
            }

            return false;
        }

        private void btnGuardarCatalogoPermiso_Click(object sender, EventArgs e)
        {
            try
            {
                permisoBLL.GuardarCatalogo(catalogoRaiz);
                CargarCatalogo();
                UiDialogHelper.MostrarInfo("FormPermissionManagement.Success.CatalogSaved");
            }
            catch (Exception ex)
            {
                UiExceptionHandler.Mostrar(ex, "Guardado de catalogo de permisos");
            }
        }

        private object CrearNodoDesdeFormulario()
        {
            string nombre = txtbNombrePermiso.Text.Trim();
            if (string.IsNullOrWhiteSpace(nombre))
            {
                throw new ExcepcionNegocio("FormPermissionManagement.Warning.NodeNameRequired");
            }

            if (nombre.Contains("."))
            {
                throw new ExcepcionNegocio("FormPermissionManagement.Warning.NameContainsDot");
            }

            OpcionTraducible opcion = cmbTipoPermiso.SelectedItem as OpcionTraducible;
            bool esGrupo = opcion != null && opcion.Valor is bool valorGrupo && valorGrupo;
            object nodo = esGrupo ? (object)new PermisoGrupo() : new PermisoSimple();
            ((dynamic)nodo).Nombre = nombre;
            return nodo;
        }
    }
}
