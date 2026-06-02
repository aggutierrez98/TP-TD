using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using BLL;
using SERVICIOS;

namespace TpIDS
{
    public class FormGestionPermisos : Form
    {
        private readonly PermisoBLL permisoBLL = new PermisoBLL();
        private readonly Action<Idioma> observadorIdioma;

        private Label lblTituloGestionPermisos;
        private GroupBox groupBoxCatalogoPermisos;
        private GroupBox groupBoxEdicionPermiso;
        private TreeView tvwCatalogoPermisos;
        private Label lblNombrePermiso;
        private TextBox txtbNombrePermiso;
        private Label lblTipoPermiso;
        private ComboBox cmbTipoPermiso;
        private Button btnAgregarRaizPermiso;
        private Button btnAgregarHijoPermiso;
        private Button btnRenombrarPermiso;
        private Button btnEliminarPermiso;
        private Button btnGuardarCatalogoPermiso;

        private List<PermisoNodo> catalogoRaiz;

        public FormGestionPermisos()
        {
            observadorIdioma = Actualizar;
            catalogoRaiz = new List<PermisoNodo>();
            InitializeComponent();
            ConfigurarTags();
            Load += FormGestionPermisos_Load;
            FormClosed += FormGestionPermisos_FormClosed;
        }

        private void InitializeComponent()
        {
            lblTituloGestionPermisos = new Label();
            groupBoxCatalogoPermisos = new GroupBox();
            groupBoxEdicionPermiso = new GroupBox();
            tvwCatalogoPermisos = new TreeView();
            lblNombrePermiso = new Label();
            txtbNombrePermiso = new TextBox();
            lblTipoPermiso = new Label();
            cmbTipoPermiso = new ComboBox();
            btnAgregarRaizPermiso = new Button();
            btnAgregarHijoPermiso = new Button();
            btnRenombrarPermiso = new Button();
            btnEliminarPermiso = new Button();
            btnGuardarCatalogoPermiso = new Button();

            SuspendLayout();

            lblTituloGestionPermisos.AutoSize = true;
            lblTituloGestionPermisos.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
            lblTituloGestionPermisos.Location = new Point(12, 12);
            lblTituloGestionPermisos.Name = "lblTituloGestionPermisos";
            lblTituloGestionPermisos.Size = new Size(214, 25);

            groupBoxCatalogoPermisos.Location = new Point(15, 55);
            groupBoxCatalogoPermisos.Name = "groupBoxCatalogoPermisos";
            groupBoxCatalogoPermisos.Size = new Size(420, 480);

            tvwCatalogoPermisos.HideSelection = false;
            tvwCatalogoPermisos.Location = new Point(10, 25);
            tvwCatalogoPermisos.Name = "tvwCatalogoPermisos";
            tvwCatalogoPermisos.Size = new Size(400, 445);
            tvwCatalogoPermisos.AfterSelect += tvwCatalogoPermisos_AfterSelect;
            groupBoxCatalogoPermisos.Controls.Add(tvwCatalogoPermisos);

            groupBoxEdicionPermiso.Location = new Point(450, 55);
            groupBoxEdicionPermiso.Name = "groupBoxEdicionPermiso";
            groupBoxEdicionPermiso.Size = new Size(320, 480);

            lblNombrePermiso.AutoSize = true;
            lblNombrePermiso.Location = new Point(12, 35);
            lblNombrePermiso.Name = "lblNombrePermiso";

            txtbNombrePermiso.Location = new Point(15, 52);
            txtbNombrePermiso.Name = "txtbNombrePermiso";
            txtbNombrePermiso.Size = new Size(285, 20);

            lblTipoPermiso.AutoSize = true;
            lblTipoPermiso.Location = new Point(12, 92);
            lblTipoPermiso.Name = "lblTipoPermiso";

            cmbTipoPermiso.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbTipoPermiso.Location = new Point(15, 109);
            cmbTipoPermiso.Name = "cmbTipoPermiso";
            cmbTipoPermiso.Size = new Size(285, 21);

            btnAgregarRaizPermiso.Location = new Point(15, 155);
            btnAgregarRaizPermiso.Name = "btnAgregarRaizPermiso";
            btnAgregarRaizPermiso.Size = new Size(285, 30);
            btnAgregarRaizPermiso.Click += btnAgregarRaizPermiso_Click;

            btnAgregarHijoPermiso.Location = new Point(15, 195);
            btnAgregarHijoPermiso.Name = "btnAgregarHijoPermiso";
            btnAgregarHijoPermiso.Size = new Size(285, 30);
            btnAgregarHijoPermiso.Click += btnAgregarHijoPermiso_Click;

            btnRenombrarPermiso.Location = new Point(15, 235);
            btnRenombrarPermiso.Name = "btnRenombrarPermiso";
            btnRenombrarPermiso.Size = new Size(285, 30);
            btnRenombrarPermiso.Click += btnRenombrarPermiso_Click;

            btnEliminarPermiso.Location = new Point(15, 275);
            btnEliminarPermiso.Name = "btnEliminarPermiso";
            btnEliminarPermiso.Size = new Size(285, 30);
            btnEliminarPermiso.Click += btnEliminarPermiso_Click;

            btnGuardarCatalogoPermiso.Location = new Point(15, 430);
            btnGuardarCatalogoPermiso.Name = "btnGuardarCatalogoPermiso";
            btnGuardarCatalogoPermiso.Size = new Size(285, 30);
            btnGuardarCatalogoPermiso.Click += btnGuardarCatalogoPermiso_Click;

            groupBoxEdicionPermiso.Controls.Add(lblNombrePermiso);
            groupBoxEdicionPermiso.Controls.Add(txtbNombrePermiso);
            groupBoxEdicionPermiso.Controls.Add(lblTipoPermiso);
            groupBoxEdicionPermiso.Controls.Add(cmbTipoPermiso);
            groupBoxEdicionPermiso.Controls.Add(btnAgregarRaizPermiso);
            groupBoxEdicionPermiso.Controls.Add(btnAgregarHijoPermiso);
            groupBoxEdicionPermiso.Controls.Add(btnRenombrarPermiso);
            groupBoxEdicionPermiso.Controls.Add(btnEliminarPermiso);
            groupBoxEdicionPermiso.Controls.Add(btnGuardarCatalogoPermiso);

            AutoScaleDimensions = new SizeF(6F, 13F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(785, 550);
            Controls.Add(lblTituloGestionPermisos);
            Controls.Add(groupBoxCatalogoPermisos);
            Controls.Add(groupBoxEdicionPermiso);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            StartPosition = FormStartPosition.CenterParent;
            Name = "FormGestionPermisos";
            ResumeLayout(false);
            PerformLayout();
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
            PermisoNodo copia = origen.EsGrupo ? (PermisoNodo)new GrupoPermiso() : new PermisoSimple();
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
                MessageBox.Show("Debe seleccionar un grupo padre.", "Atencion", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!padre.EsGrupo)
            {
                MessageBox.Show("Solo se pueden agregar hijos dentro de un grupo.", "Atencion", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
                MessageBox.Show("Debe seleccionar un permiso o grupo.", "Atencion", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string nuevoNombre = txtbNombrePermiso.Text.Trim();
            if (string.IsNullOrWhiteSpace(nuevoNombre))
            {
                MessageBox.Show("Debe indicar un nombre.", "Atencion", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            permiso.Nombre = nuevoNombre;
            RefrescarArbol();
        }

        private void btnEliminarPermiso_Click(object sender, EventArgs e)
        {
            if (!(tvwCatalogoPermisos.SelectedNode?.Tag is PermisoNodo permiso))
            {
                MessageBox.Show("Debe seleccionar un permiso o grupo.", "Atencion", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult confirmacion = MessageBox.Show(
                "Se eliminara el nodo seleccionado y todos sus hijos. Tambien se eliminaran sus asignaciones a usuarios al guardar. Desea continuar?",
                "Confirmar eliminacion",
                MessageBoxButtons.YesNo,
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
                MessageBox.Show("Catalogo de permisos guardado correctamente.", "Exito", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
                throw new Exception("Debe indicar un nombre para el permiso o grupo.");
            }

            bool esGrupo = cmbTipoPermiso.SelectedIndex == 0;
            PermisoNodo nodo = esGrupo ? (PermisoNodo)new GrupoPermiso() : new PermisoSimple();
            nodo.Nombre = nombre;
            return nodo;
        }

        public void Actualizar(Idioma idioma)
        {
            IdiomaFormHelper.Aplicar(this, idioma);
            RecargarTiposPermiso(idioma);
        }

        private void ConfigurarTags()
        {
            Tag = "FormGestionPermisos.Title";
            lblTituloGestionPermisos.Tag = "FormGestionPermisos.TitleLabel";
            groupBoxCatalogoPermisos.Tag = "FormGestionPermisos.TreeGroup";
            groupBoxEdicionPermiso.Tag = "FormGestionPermisos.EditionGroup";
            lblNombrePermiso.Tag = "FormGestionPermisos.NameLabel";
            lblTipoPermiso.Tag = "FormGestionPermisos.TypeLabel";
            btnAgregarRaizPermiso.Tag = "FormGestionPermisos.RootButton";
            btnAgregarHijoPermiso.Tag = "FormGestionPermisos.ChildButton";
            btnRenombrarPermiso.Tag = "FormGestionPermisos.RenameButton";
            btnEliminarPermiso.Tag = "FormGestionPermisos.DeleteButton";
            btnGuardarCatalogoPermiso.Tag = "FormGestionPermisos.SaveButton";
        }

        private void RecargarTiposPermiso(Idioma idioma)
        {
            string textoGrupo = idioma == null ? "Grupo" : idioma.ObtenerTexto("FormGestionPermisos.TypeGroup");
            string textoPermiso = idioma == null ? "Permiso" : idioma.ObtenerTexto("FormGestionPermisos.TypeLeaf");
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
