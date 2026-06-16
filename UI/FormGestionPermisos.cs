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

        private List<PermisoNodo> catalogoRaiz;

        public FormGestionPermisos()
        {
            catalogoRaiz = new List<PermisoNodo>();
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
            catalogoRaiz = permisoBLL.ObtenerCatalogo()
                .Select(ClonarNodo)
                .ToList();

            RefrescarArbol();
        }

        private PermisoNodo ClonarNodo(PermisoNodo origen)
        {
            PermisoNodo copia = origen.EsGrupo ? (PermisoNodo)new PermisoGrupo() : new PermisoSimple();
            copia.Id = origen.Id;
            copia.Nombre = origen.Nombre;

            foreach (PermisoNodo hijo in origen.HijosNodos)
            {
                copia.AgregarHijo(ClonarNodo(hijo));
            }

            return copia;
        }

        private void RefrescarArbol()
        {
            tvwCatalogoPermisos.Nodes.Clear();

            foreach (PermisoNodo raiz in catalogoRaiz)
            {
                tvwCatalogoPermisos.Nodes.Add(CrearNodoTreeView(raiz));
            }

            tvwCatalogoPermisos.ExpandAll();
        }

        private TreeNode CrearNodoTreeView(PermisoNodo permiso)
        {
            TreeNode nodo = new TreeNode(permiso.Nombre)
            {
                Tag = permiso
            };

            foreach (PermisoNodo hijo in permiso.HijosNodos)
            {
                nodo.Nodes.Add(CrearNodoTreeView(hijo));
            }

            return nodo;
        }

        private void tvwCatalogoPermisos_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (!(e.Node.Tag is PermisoNodo permiso))
            {
                return;
            }

            txtbNombrePermiso.Text = permiso.Nombre;
            SeleccionarTipoPermiso(permiso.EsGrupo);
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
                PermisoNodo nodo = CrearNodoDesdeFormulario();
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
            if (!(tvwCatalogoPermisos.SelectedNode?.Tag is PermisoNodo padre))
            {
                UiDialogHelper.MostrarWarning("FormPermissionManagement.Warning.SelectParentGroup");
                return;
            }

            if (!padre.EsGrupo)
            {
                UiDialogHelper.MostrarWarning("FormPermissionManagement.Warning.OnlyGroupsCanContainChildren");
                return;
            }

            try
            {
                padre.AgregarHijo(CrearNodoDesdeFormulario());
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
            if (!(tvwCatalogoPermisos.SelectedNode?.Tag is PermisoNodo permiso))
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

            permiso.Nombre = nuevoNombre;
            RefrescarArbol();
        }

        private void btnEliminarPermiso_Click(object sender, EventArgs e)
        {
            if (!(tvwCatalogoPermisos.SelectedNode?.Tag is PermisoNodo permiso))
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
                foreach (PermisoNodo raiz in catalogoRaiz)
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

        private bool QuitarNodo(PermisoNodo padre, PermisoNodo objetivo)
        {
            foreach (PermisoNodo hijo in padre.HijosNodos.ToList())
            {
                if (ReferenceEquals(hijo, objetivo))
                {
                    padre.QuitarHijo(hijo);
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

        private PermisoNodo CrearNodoDesdeFormulario()
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
            PermisoNodo nodo = esGrupo ? (PermisoNodo)new PermisoGrupo() : new PermisoSimple();
            nodo.Nombre = nombre;
            return nodo;
        }
    }
}
