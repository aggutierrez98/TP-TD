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
        private const string PermisoGestionPermisos = "gestionpermisos";

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
            if (!AutorizacionService.GetInstance().TienePermiso(PermisoGestionPermisos))
            {
                UiDialogHelper.MostrarWarning("FormUserManagement.Warning.NotPermitted");
                Close();
                return;
            }

            CargarCatalogo();
        }

        private void CargarCatalogo()
        {
            Dictionary<int, PermisoNodo> cache = new Dictionary<int, PermisoNodo>();
            catalogoRaiz = permisoBLL.ObtenerCatalogo()
                .Select(item => ClonarNodo(item, cache))
                .ToList();

            RefrescarArbol();
        }

        private PermisoNodo ClonarNodo(PermisoNodo origen, IDictionary<int, PermisoNodo> cache)
        {
            if (origen.Id > 0 && cache.ContainsKey(origen.Id))
            {
                return cache[origen.Id];
            }

            PermisoNodo copia = origen.EsGrupo ? (PermisoNodo)new PermisoGrupo() : new PermisoSimple();
            copia.Id = origen.Id;
            copia.Nombre = origen.Nombre;

            if (origen.Id > 0)
            {
                cache[origen.Id] = copia;
            }

            foreach (PermisoNodo hijo in origen.HijosNodos)
            {
                copia.AgregarHijo(ClonarNodo(hijo, cache));
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

            AsegurarTiposPermisoCargados();
            txtbNombrePermiso.Text = permiso.Nombre;
            int indice = permiso.EsGrupo ? 0 : 1;
            if (cmbTipoPermiso.Items.Count > indice)
            {
                cmbTipoPermiso.SelectedIndex = indice;
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

            AsegurarTiposPermisoCargados();
            bool esGrupo = cmbTipoPermiso.SelectedIndex == 0;
            string nombreNormalizado = nombre.ToLowerInvariant();

            PermisoNodo existente = BuscarNodoExistente(nombreNormalizado);
            if (existente != null)
            {
                if (existente.EsGrupo != esGrupo)
                {
                    throw new ExcepcionNegocio("Error.Permission.DuplicateNames");
                }

                return existente;
            }

            PermisoNodo nodo = esGrupo ? (PermisoNodo)new PermisoGrupo() : new PermisoSimple();
            nodo.Nombre = nombreNormalizado;
            return nodo;
        }

        private PermisoNodo BuscarNodoExistente(string nombreNormalizado)
        {
            return BuscarNodoExistente(catalogoRaiz, nombreNormalizado);
        }

        private PermisoNodo BuscarNodoExistente(IEnumerable<PermisoNodo> nodos, string nombreNormalizado)
        {
            if (nodos == null)
            {
                return null;
            }

            foreach (PermisoNodo nodo in nodos)
            {
                if (string.Equals(nodo.Nombre, nombreNormalizado, StringComparison.OrdinalIgnoreCase))
                {
                    return nodo;
                }

                PermisoNodo encontrado = BuscarNodoExistente(nodo.HijosNodos, nombreNormalizado);
                if (encontrado != null)
                {
                    return encontrado;
                }
            }

            return null;
        }

        protected override void OnIdiomaActualizado(Idioma idioma)
        {
            base.OnIdiomaActualizado(idioma);
            RecargarTiposPermiso(idioma);
        }

        private void RecargarTiposPermiso(Idioma idioma)
        {
            string textoGrupo = idioma == null ? "Grupo" : idioma.ObtenerTraduccion("FormPermissionManagement.TypeGroup");
            string textoPermiso = idioma == null ? "Permiso" : idioma.ObtenerTraduccion("FormPermissionManagement.TypeLeaf");
            int indiceActual = cmbTipoPermiso.SelectedIndex < 0 ? 0 : cmbTipoPermiso.SelectedIndex;

            cmbTipoPermiso.Items.Clear();
            cmbTipoPermiso.Items.Add(textoGrupo);
            cmbTipoPermiso.Items.Add(textoPermiso);
            if (cmbTipoPermiso.Items.Count > 0)
            {
                cmbTipoPermiso.SelectedIndex = indiceActual <= 1 && indiceActual < cmbTipoPermiso.Items.Count
                    ? indiceActual
                    : 0;
            }
        }

        private void AsegurarTiposPermisoCargados()
        {
            if (cmbTipoPermiso.Items.Count == 0)
            {
                RecargarTiposPermiso(IdiomaService.GetInstance().IdiomaActual);
            }
        }
    }
}
