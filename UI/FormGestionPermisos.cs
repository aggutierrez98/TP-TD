using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using BLL;
using SERVICIOS;

namespace TpIDS
{
    public partial class FormGestionPermisos : Form
    {
        private readonly PermisoBLL permisoBLL = new PermisoBLL();
        private readonly Action<Idioma> observadorIdioma;

        private List<PermisoNodo> catalogoRaiz;

        public FormGestionPermisos()
        {
            observadorIdioma = Actualizar;
            catalogoRaiz = new List<PermisoNodo>();
            InitializeComponent();
            Load += FormGestionPermisos_Load;
            FormClosed += FormGestionPermisos_FormClosed;
        }

        private void FormGestionPermisos_Load(object sender, EventArgs e)
        {
            IdiomaService.GetInstance().RegistrarObservador(observadorIdioma);
            RecargarTiposPermiso(IdiomaService.GetInstance().IdiomaActual);
            CargarCatalogo();
            Actualizar(IdiomaService.GetInstance().IdiomaActual);
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
            cmbTipoPermiso.SelectedIndex = permiso.EsGrupo ? 0 : 1;
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
                UiDialogHelper.MostrarWarning("FormGestionPermisos.Warning.SelectParentGroup");
                return;
            }

            if (!padre.EsGrupo)
            {
                UiDialogHelper.MostrarWarning("FormGestionPermisos.Warning.OnlyGroupsCanContainChildren");
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
                UiDialogHelper.MostrarWarning("FormGestionPermisos.Warning.SelectPermission");
                return;
            }

            string nuevoNombre = txtbNombrePermiso.Text.Trim();
            if (string.IsNullOrWhiteSpace(nuevoNombre))
            {
                UiDialogHelper.MostrarWarning("FormGestionPermisos.Warning.NameRequired");
                return;
            }

            permiso.Nombre = nuevoNombre;
            RefrescarArbol();
        }

        private void btnEliminarPermiso_Click(object sender, EventArgs e)
        {
            if (!(tvwCatalogoPermisos.SelectedNode?.Tag is PermisoNodo permiso))
            {
                UiDialogHelper.MostrarWarning("FormGestionPermisos.Warning.SelectPermission");
                return;
            }

            DialogResult confirmacion = UiDialogHelper.Confirmar(
                "FormGestionPermisos.Confirm.DeleteNodeMessage",
                "FormGestionPermisos.Confirm.DeleteNodeTitle",
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
                UiDialogHelper.MostrarInfo("FormGestionPermisos.Success.CatalogSaved");
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
                throw new ExcepcionNegocio("FormGestionPermisos.Warning.NodeNameRequired");
            }

            bool esGrupo = cmbTipoPermiso.SelectedIndex == 0;
            PermisoNodo nodo = esGrupo ? (PermisoNodo)new PermisoGrupo() : new PermisoSimple();
            nodo.Nombre = nombre;
            return nodo;
        }

        public void Actualizar(Idioma idioma)
        {
            IdiomaFormHelper.Aplicar(this, idioma);
            RecargarTiposPermiso(idioma);
        }

        private void RecargarTiposPermiso(Idioma idioma)
        {
            string textoGrupo = idioma == null ? "Grupo" : idioma.ObtenerTraduccion("FormGestionPermisos.TypeGroup");
            string textoPermiso = idioma == null ? "Permiso" : idioma.ObtenerTraduccion("FormGestionPermisos.TypeLeaf");
            int indiceActual = cmbTipoPermiso.SelectedIndex < 0 ? 0 : cmbTipoPermiso.SelectedIndex;

            cmbTipoPermiso.Items.Clear();
            cmbTipoPermiso.Items.Add(textoGrupo);
            cmbTipoPermiso.Items.Add(textoPermiso);
            cmbTipoPermiso.SelectedIndex = indiceActual <= 1 ? indiceActual : 0;
        }

        private void FormGestionPermisos_FormClosed(object sender, FormClosedEventArgs e)
        {
            IdiomaService.GetInstance().DesregistrarObservador(observadorIdioma);
        }
    }
}
