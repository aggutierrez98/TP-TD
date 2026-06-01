using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using BLL;
using SERVICIOS;

namespace TpIDS
{
    public class FormBitacora : Form
    {
        private readonly BitacoraBLL _bitacoraBLL;
        private readonly Action<Idioma> _observadorIdioma;

        private Label lblTituloBitacora;
        private GroupBox groupBoxFiltrosBitacora;
        private CheckBox chkbFechaDesdeBitacora;
        private DateTimePicker dtpFechaDesdeBitacora;
        private CheckBox chkbFechaHastaBitacora;
        private DateTimePicker dtpFechaHastaBitacora;
        private Label lblUsuarioBitacora;
        private TextBox txtbUsuarioBitacora;
        private Label lblActividadBitacora;
        private TextBox txtbActividadBitacora;
        private Label lblNivelBitacora;
        private ComboBox cmbNivelBitacora;
        private CheckBox chkbSoloTecnicaBitacora;
        private Button btnBuscarBitacora;
        private DataGridView dgvBitacora;

        public FormBitacora()
        {
            _bitacoraBLL = new BitacoraBLL();
            _observadorIdioma = Actualizar;
            InitializeComponent();
            ConfigurarTags();
            Load += FormBitacora_Load;
            FormClosed += FormBitacora_FormClosed;
        }

        private void InitializeComponent()
        {
            lblTituloBitacora = new Label();
            groupBoxFiltrosBitacora = new GroupBox();
            chkbFechaDesdeBitacora = new CheckBox();
            dtpFechaDesdeBitacora = new DateTimePicker();
            chkbFechaHastaBitacora = new CheckBox();
            dtpFechaHastaBitacora = new DateTimePicker();
            lblUsuarioBitacora = new Label();
            txtbUsuarioBitacora = new TextBox();
            lblActividadBitacora = new Label();
            txtbActividadBitacora = new TextBox();
            lblNivelBitacora = new Label();
            cmbNivelBitacora = new ComboBox();
            chkbSoloTecnicaBitacora = new CheckBox();
            btnBuscarBitacora = new Button();
            dgvBitacora = new DataGridView();

            SuspendLayout();

            lblTituloBitacora.AutoSize = true;
            lblTituloBitacora.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
            lblTituloBitacora.Location = new Point(15, 15);
            lblTituloBitacora.Name = "lblTituloBitacora";

            groupBoxFiltrosBitacora.Location = new Point(18, 50);
            groupBoxFiltrosBitacora.Name = "groupBoxFiltrosBitacora";
            groupBoxFiltrosBitacora.Size = new Size(944, 110);

            chkbFechaDesdeBitacora.Location = new Point(15, 28);
            chkbFechaDesdeBitacora.Name = "chkbFechaDesdeBitacora";
            chkbFechaDesdeBitacora.Size = new Size(110, 20);

            dtpFechaDesdeBitacora.CustomFormat = "dd/MM/yyyy HH:mm";
            dtpFechaDesdeBitacora.Format = DateTimePickerFormat.Custom;
            dtpFechaDesdeBitacora.Location = new Point(130, 26);
            dtpFechaDesdeBitacora.Name = "dtpFechaDesdeBitacora";
            dtpFechaDesdeBitacora.Size = new Size(185, 20);

            chkbFechaHastaBitacora.Location = new Point(340, 28);
            chkbFechaHastaBitacora.Name = "chkbFechaHastaBitacora";
            chkbFechaHastaBitacora.Size = new Size(110, 20);

            dtpFechaHastaBitacora.CustomFormat = "dd/MM/yyyy HH:mm";
            dtpFechaHastaBitacora.Format = DateTimePickerFormat.Custom;
            dtpFechaHastaBitacora.Location = new Point(455, 26);
            dtpFechaHastaBitacora.Name = "dtpFechaHastaBitacora";
            dtpFechaHastaBitacora.Size = new Size(185, 20);

            lblUsuarioBitacora.AutoSize = true;
            lblUsuarioBitacora.Location = new Point(15, 63);
            lblUsuarioBitacora.Name = "lblUsuarioBitacora";

            txtbUsuarioBitacora.Location = new Point(18, 79);
            txtbUsuarioBitacora.Name = "txtbUsuarioBitacora";
            txtbUsuarioBitacora.Size = new Size(180, 20);

            lblActividadBitacora.AutoSize = true;
            lblActividadBitacora.Location = new Point(220, 63);
            lblActividadBitacora.Name = "lblActividadBitacora";

            txtbActividadBitacora.Location = new Point(223, 79);
            txtbActividadBitacora.Name = "txtbActividadBitacora";
            txtbActividadBitacora.Size = new Size(220, 20);

            lblNivelBitacora.AutoSize = true;
            lblNivelBitacora.Location = new Point(465, 63);
            lblNivelBitacora.Name = "lblNivelBitacora";

            cmbNivelBitacora.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbNivelBitacora.Location = new Point(468, 79);
            cmbNivelBitacora.Name = "cmbNivelBitacora";
            cmbNivelBitacora.Size = new Size(160, 21);

            chkbSoloTecnicaBitacora.Location = new Point(650, 79);
            chkbSoloTecnicaBitacora.Name = "chkbSoloTecnicaBitacora";
            chkbSoloTecnicaBitacora.Size = new Size(140, 20);

            btnBuscarBitacora.Location = new Point(800, 72);
            btnBuscarBitacora.Name = "btnBuscarBitacora";
            btnBuscarBitacora.Size = new Size(125, 30);
            btnBuscarBitacora.Click += btnBuscarBitacora_Click;

            groupBoxFiltrosBitacora.Controls.Add(chkbFechaDesdeBitacora);
            groupBoxFiltrosBitacora.Controls.Add(dtpFechaDesdeBitacora);
            groupBoxFiltrosBitacora.Controls.Add(chkbFechaHastaBitacora);
            groupBoxFiltrosBitacora.Controls.Add(dtpFechaHastaBitacora);
            groupBoxFiltrosBitacora.Controls.Add(lblUsuarioBitacora);
            groupBoxFiltrosBitacora.Controls.Add(txtbUsuarioBitacora);
            groupBoxFiltrosBitacora.Controls.Add(lblActividadBitacora);
            groupBoxFiltrosBitacora.Controls.Add(txtbActividadBitacora);
            groupBoxFiltrosBitacora.Controls.Add(lblNivelBitacora);
            groupBoxFiltrosBitacora.Controls.Add(cmbNivelBitacora);
            groupBoxFiltrosBitacora.Controls.Add(chkbSoloTecnicaBitacora);
            groupBoxFiltrosBitacora.Controls.Add(btnBuscarBitacora);

            dgvBitacora.AllowUserToAddRows = false;
            dgvBitacora.AllowUserToDeleteRows = false;
            dgvBitacora.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvBitacora.Location = new Point(18, 175);
            dgvBitacora.MultiSelect = false;
            dgvBitacora.Name = "dgvBitacora";
            dgvBitacora.ReadOnly = true;
            dgvBitacora.RowHeadersVisible = false;
            dgvBitacora.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvBitacora.Size = new Size(944, 360);

            AutoScaleDimensions = new SizeF(6F, 13F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(980, 555);
            Controls.Add(lblTituloBitacora);
            Controls.Add(groupBoxFiltrosBitacora);
            Controls.Add(dgvBitacora);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            Name = "FormBitacora";
            StartPosition = FormStartPosition.CenterParent;
            ResumeLayout(false);
            PerformLayout();
        }

        private void FormBitacora_Load(object sender, EventArgs e)
        {
            Sesion.GetInstance().RegistrarObservador(_observadorIdioma);
            CargarNiveles();
            Buscar();
            Actualizar(Sesion.GetInstance().IdiomaActual);
        }

        private void CargarNiveles()
        {
            cmbNivelBitacora.Items.Clear();
            cmbNivelBitacora.Items.Add(string.Empty);
            cmbNivelBitacora.Items.Add("Informacion");
            cmbNivelBitacora.Items.Add("Advertencia");
            cmbNivelBitacora.Items.Add("Error");
            cmbNivelBitacora.SelectedIndex = 0;
        }

        private void btnBuscarBitacora_Click(object sender, EventArgs e)
        {
            Buscar();
        }

        private void Buscar()
        {
            try
            {
                FiltroBitacora filtro = new FiltroBitacora
                {
                    FechaDesde = chkbFechaDesdeBitacora.Checked ? (DateTime?)dtpFechaDesdeBitacora.Value : null,
                    FechaHasta = chkbFechaHastaBitacora.Checked ? (DateTime?)dtpFechaHastaBitacora.Value : null,
                    Usuario = txtbUsuarioBitacora.Text.Trim(),
                    Actividad = txtbActividadBitacora.Text.Trim(),
                    Nivel = cmbNivelBitacora.SelectedItem == null ? null : cmbNivelBitacora.SelectedItem.ToString(),
                    EsTecnica = chkbSoloTecnicaBitacora.Checked ? (bool?)true : null
                };

                List<EventoBitacora> registros = _bitacoraBLL.Buscar(filtro);
                dgvBitacora.DataSource = null;
                dgvBitacora.DataSource = registros;
                RecargarEncabezados();
            }
            catch (Exception ex)
            {
                UiExceptionHandler.Mostrar(ex, "Consulta de bitacora");
            }
        }

        private void RecargarEncabezados()
        {
            if (dgvBitacora.Columns.Count == 0)
            {
                return;
            }

            if (dgvBitacora.Columns.Contains("Id"))
            {
                dgvBitacora.Columns["Id"].Width = 60;
            }

            if (dgvBitacora.Columns.Contains("Informacion"))
            {
                dgvBitacora.Columns["Informacion"].FillWeight = 180;
            }
        }

        public void Actualizar(Idioma idioma)
        {
            IdiomaFormHelper.Aplicar(this, idioma);
        }

        private void ConfigurarTags()
        {
            Tag = "FormBitacora.Title";
            lblTituloBitacora.Tag = "FormBitacora.TitleLabel";
            groupBoxFiltrosBitacora.Tag = "FormBitacora.FiltersGroup";
            chkbFechaDesdeBitacora.Tag = "FormBitacora.FromCheck";
            chkbFechaHastaBitacora.Tag = "FormBitacora.ToCheck";
            lblUsuarioBitacora.Tag = "FormBitacora.UserLabel";
            lblActividadBitacora.Tag = "FormBitacora.ActivityLabel";
            lblNivelBitacora.Tag = "FormBitacora.LevelLabel";
            chkbSoloTecnicaBitacora.Tag = "FormBitacora.OnlyTechnicalCheck";
            btnBuscarBitacora.Tag = "FormBitacora.SearchButton";
        }

        private void FormBitacora_FormClosed(object sender, FormClosedEventArgs e)
        {
            Sesion.GetInstance().DesregistrarObservador(_observadorIdioma);
        }
    }
}
