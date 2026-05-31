using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using BLL;
using SERVICIOS;

namespace TpIDS
{
    public partial class FormGestionIdiomas : Form
    {
        private readonly IdiomaBLL idiomaBLL = new IdiomaBLL();
        private readonly Action<Idioma> observadorIdioma;
        private bool cargandoIdiomas;

        public FormGestionIdiomas()
        {
            InitializeComponent();
            observadorIdioma = Actualizar;
            ConfigurarTags();
            Load += FormGestionIdiomas_Load;
            FormClosed += FormGestionIdiomas_FormClosed;
        }

        private void FormGestionIdiomas_Load(object sender, EventArgs e)
        {
            Sesion.GetInstance().RegistrarObservador(observadorIdioma);
            CargarIdiomas(null);
            Actualizar(Sesion.GetInstance().IdiomaActual);
        }

        private void btnAgregarIdioma_Click(object sender, EventArgs e)
        {
            try
            {
                int idiomaId = idiomaBLL.CrearIdioma(txtbNombreIdioma.Text, chkbIdiomaDefault.Checked);
                CargarIdiomas(idiomaId);
                RefrescarSesionSiCorresponde(idiomaId);
                MessageBox.Show("Idioma creado correctamente.", "Exito", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnActualizarIdioma_Click(object sender, EventArgs e)
        {
            if (!(cmbIdiomas.SelectedItem is Idioma idioma))
            {
                MessageBox.Show("Debe seleccionar un idioma.", "Atencion", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                bool actualizado = idiomaBLL.ActualizarIdioma(idioma.Id, txtbNombreIdioma.Text, chkbIdiomaDefault.Checked);
                if (actualizado)
                {
                    CargarIdiomas(idioma.Id);
                    RefrescarSesionSiCorresponde(idioma.Id);
                    MessageBox.Show("Idioma actualizado correctamente.", "Exito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnEliminarIdioma_Click(object sender, EventArgs e)
        {
            if (!(cmbIdiomas.SelectedItem is Idioma idioma))
            {
                MessageBox.Show("Debe seleccionar un idioma.", "Atencion", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult confirmacion = MessageBox.Show(
                string.Format("Estas seguro de que queres eliminar el idioma '{0}'?", idioma.Nombre),
                "Confirmar eliminacion",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (confirmacion != DialogResult.Yes)
            {
                return;
            }

            try
            {
                bool eliminado = idiomaBLL.EliminarIdioma(idioma.Id);
                if (eliminado)
                {
                    CargarIdiomas(null);
                    RefrescarSesionSiCorresponde(idioma.Id);
                    MessageBox.Show("Idioma eliminado correctamente.", "Exito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnImportarJson_Click(object sender, EventArgs e)
        {
            if (!(cmbIdiomas.SelectedItem is Idioma idioma))
            {
                MessageBox.Show("Debe seleccionar un idioma.", "Atencion", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Archivos JSON (*.json)|*.json";
                openFileDialog.Title = "Seleccionar archivo JSON";

                if (openFileDialog.ShowDialog(this) != DialogResult.OK)
                {
                    return;
                }

                try
                {
                    idiomaBLL.ImportarJson(idioma.Id, openFileDialog.FileName);
                    RecargarIdiomaSeleccionado(idioma.Id);
                    RefrescarSesionSiCorresponde(idioma.Id);
                    MessageBox.Show("Traducciones importadas correctamente.", "Exito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnGuardarTraduccion_Click(object sender, EventArgs e)
        {
            if (!(cmbIdiomas.SelectedItem is Idioma idioma))
            {
                MessageBox.Show("Debe seleccionar un idioma.", "Atencion", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                idiomaBLL.GuardarTraduccion(idioma.Id, txtbKeyManual.Text, txtbValorManual.Text);
                txtbKeyManual.Clear();
                txtbValorManual.Clear();
                RecargarIdiomaSeleccionado(idioma.Id);
                RefrescarSesionSiCorresponde(idioma.Id);
                MessageBox.Show("Traduccion guardada correctamente.", "Exito", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnEliminarTraduccion_Click(object sender, EventArgs e)
        {
            if (!(cmbIdiomas.SelectedItem is Idioma idioma))
            {
                MessageBox.Show("Debe seleccionar un idioma.", "Atencion", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!(dgvTraducciones.CurrentRow?.DataBoundItem is TraduccionGridItem item))
            {
                MessageBox.Show("Debe seleccionar una traduccion.", "Atencion", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (item.EsKeyBase)
            {
                MessageBox.Show(
                    "La traduccion seleccionada corresponde a una key base del sistema y no puede eliminarse.",
                    "Operacion no permitida",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            try
            {
                bool eliminado = idiomaBLL.EliminarTraduccion(idioma.Id, item.PalabraId);
                if (eliminado)
                {
                    LimpiarEdicionManual();
                    RecargarIdiomaSeleccionado(idioma.Id);
                    RefrescarSesionSiCorresponde(idioma.Id);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void cmbIdiomas_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cargandoIdiomas || !(cmbIdiomas.SelectedItem is Idioma idioma))
            {
                return;
            }

            LimpiarEdicionManual();
            CargarDetalleIdioma(idioma.Id);
        }

        private void dgvTraducciones_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0)
            {
                return;
            }

            if (!(cmbIdiomas.SelectedItem is Idioma idioma))
            {
                return;
            }

            if (!(dgvTraducciones.Rows[e.RowIndex].DataBoundItem is TraduccionGridItem item))
            {
                return;
            }

            if (e.ColumnIndex != dgvTraducciones.Columns["Valor"].Index)
            {
                return;
            }

            string valorEditado = dgvTraducciones.Rows[e.RowIndex].Cells["Valor"].Value == null
                ? string.Empty
                : dgvTraducciones.Rows[e.RowIndex].Cells["Valor"].Value.ToString();

            try
            {
                idiomaBLL.GuardarTraduccion(idioma.Id, item.Key, valorEditado);
                txtbKeyManual.Text = item.Key;
                txtbValorManual.Text = valorEditado;
                RefrescarSesionSiCorresponde(idioma.Id);

                BeginInvoke(new Action(() =>
                {
                    if (!IsDisposed)
                    {
                        RecargarIdiomaSeleccionado(idioma.Id);
                    }
                }));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                BeginInvoke(new Action(() =>
                {
                    if (!IsDisposed)
                    {
                        RecargarIdiomaSeleccionado(idioma.Id);
                    }
                }));
            }
        }

        private void dgvTraducciones_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
            {
                return;
            }

            CargarTraduccionSeleccionada();
        }

        public void Actualizar(Idioma idioma)
        {
            IdiomaFormHelper.Aplicar(this, idioma);
            ActualizarEncabezadosGrilla(idioma);
        }

        private void ConfigurarTags()
        {
            Tag = "FormGestionIdiomas.Title";
            lblTituloGestionIdiomas.Tag = "FormGestionIdiomas.TitleLabel";
            groupBoxIdioma.Tag = "FormGestionIdiomas.LanguageGroup";
            groupBoxTraducciones.Tag = "FormGestionIdiomas.TranslationsGroup";
            lblNombreIdioma.Tag = "FormGestionIdiomas.NameLabel";
            chkbIdiomaDefault.Tag = "FormGestionIdiomas.DefaultCheck";
            btnAgregarIdioma.Tag = "FormGestionIdiomas.AddButton";
            btnActualizarIdioma.Tag = "FormGestionIdiomas.UpdateButton";
            btnEliminarIdioma.Tag = "FormGestionIdiomas.DeleteButton";
            lblIdiomaSeleccionado.Tag = "FormGestionIdiomas.SelectLanguageLabel";
            btnImportarJson.Tag = "FormGestionIdiomas.ImportButton";
            lblKeyManual.Tag = "FormGestionIdiomas.ManualKeyLabel";
            lblValorManual.Tag = "FormGestionIdiomas.ManualValueLabel";
            btnGuardarTraduccion.Tag = "FormGestionIdiomas.SaveTranslationButton";
            btnEliminarTraduccion.Tag = "FormGestionIdiomas.DeleteTranslationButton";
        }

        private void CargarIdiomas(int? idiomaSeleccionadoId)
        {
            cargandoIdiomas = true;

            List<Idioma> idiomas = idiomaBLL.ObtenerIdiomas();
            cmbIdiomas.DataSource = null;
            cmbIdiomas.DisplayMember = "Nombre";
            cmbIdiomas.ValueMember = "Id";
            cmbIdiomas.DataSource = idiomas;

            if (idiomaSeleccionadoId.HasValue)
            {
                Idioma idiomaSeleccionado = idiomas.FirstOrDefault(item => item.Id == idiomaSeleccionadoId.Value);
                if (idiomaSeleccionado != null)
                {
                    cmbIdiomas.SelectedItem = idiomaSeleccionado;
                }
            }

            cargandoIdiomas = false;

            if (cmbIdiomas.SelectedItem is Idioma idioma)
            {
                CargarDetalleIdioma(idioma.Id);
            }
        }

        private void CargarDetalleIdioma(int idiomaId)
        {
            Idioma idioma = idiomaBLL.ObtenerIdioma(idiomaId);
            if (idioma == null)
            {
                return;
            }

            txtbNombreIdioma.Text = idioma.Nombre;
            chkbIdiomaDefault.Checked = idioma.Default;

            dgvTraducciones.AutoGenerateColumns = false;
            if (dgvTraducciones.Columns.Count == 0)
            {
                dgvTraducciones.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "Key",
                    DataPropertyName = "Key",
                    ReadOnly = true,
                    AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
                });
                dgvTraducciones.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "Valor",
                    DataPropertyName = "Valor",
                    ReadOnly = false,
                    AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
                });
            }

            dgvTraducciones.DataSource = idioma.Traducciones
                .OfType<Traduccion>()
                .OrderBy(item => item.Palabra.Key)
                .Select(item => new TraduccionGridItem
                {
                    PalabraId = item.PalabraId,
                    Key = item.Palabra.Key,
                    Valor = item.Valor,
                    EsKeyBase = EsKeyBase(item.Palabra.Key)
                })
                .ToList();

            ActualizarEncabezadosGrilla(Sesion.GetInstance().IdiomaActual);
            CargarTraduccionSeleccionada();
        }

        private void RecargarIdiomaSeleccionado(int idiomaId)
        {
            CargarIdiomas(idiomaId);
        }

        private void RefrescarSesionSiCorresponde(int idiomaIdAfectado)
        {
            Idioma idiomaActual = Sesion.GetInstance().IdiomaActual;
            Idioma idiomaDefault = idiomaBLL.ObtenerIdiomaDefault();

            if (idiomaActual != null && idiomaActual.Id == idiomaIdAfectado)
            {
                Idioma idiomaRecargado = idiomaBLL.ObtenerIdioma(idiomaIdAfectado);
                Sesion.GetInstance().CambiarIdioma(idiomaRecargado ?? idiomaDefault);
                return;
            }

            if (idiomaDefault != null && idiomaActual != null && idiomaDefault.Id == idiomaActual.Id)
            {
                Sesion.GetInstance().CambiarIdioma(idiomaDefault);
            }
        }

        private void ActualizarEncabezadosGrilla(Idioma idioma)
        {
            if (idioma == null || dgvTraducciones.Columns.Count == 0)
            {
                return;
            }

            dgvTraducciones.Columns["Key"].HeaderText = idioma.ObtenerTexto("FormGestionIdiomas.Grid.Key");
            dgvTraducciones.Columns["Valor"].HeaderText = idioma.ObtenerTexto("FormGestionIdiomas.Grid.Value");
        }

        private void CargarTraduccionSeleccionada()
        {
            if (!(dgvTraducciones.CurrentRow?.DataBoundItem is TraduccionGridItem item))
            {
                LimpiarEdicionManual();
                return;
            }

            txtbKeyManual.Text = item.Key;
            txtbValorManual.Text = item.Valor;
        }

        private void LimpiarEdicionManual()
        {
            txtbKeyManual.Clear();
            txtbValorManual.Clear();
        }

        private static bool EsKeyBase(string key)
        {
            return !string.IsNullOrWhiteSpace(key) &&
                   (key.StartsWith("FormLogin.", StringComparison.Ordinal) ||
                    key.StartsWith("FormRegistro.", StringComparison.Ordinal) ||
                    key.StartsWith("FormGestionUsuarios.", StringComparison.Ordinal) ||
                    key.StartsWith("FormConfiguracionUsuario.", StringComparison.Ordinal) ||
                    key.StartsWith("FormGestionIdiomas.", StringComparison.Ordinal));
        }

        private void FormGestionIdiomas_FormClosed(object sender, FormClosedEventArgs e)
        {
            Sesion.GetInstance().DesregistrarObservador(observadorIdioma);
        }

        private class TraduccionGridItem
        {
            public int PalabraId { get; set; }

            public string Key { get; set; }

            public string Valor { get; set; }

            public bool EsKeyBase { get; set; }
        }
    }
}
