using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using ABSTRACCIONES;
using BLL;
using SERVICIOS;

namespace TpIDS
{
    public partial class FormGestionIdiomas : FormTraducibleBase
    {
        private readonly IdiomaBLL idiomaBLL = CompositionRoot.Idioma;
        private bool cargandoIdiomas;

        public FormGestionIdiomas()
        {
            InitializeComponent();
            Load += FormGestionIdiomas_Load;
        }

        private void FormGestionIdiomas_Load(object sender, EventArgs e)
        {
            CargarIdiomas(null);
        }

        // View-model plano para la grilla
        private sealed class FilaTraduccion
        {
            public Traduccion Traduccion { get; set; }
            public int PalabraId { get; set; }
            public string Key { get; set; }
            public string Valor { get; set; }
        }

        private void btnAgregarIdioma_Click(object sender, EventArgs e)
        {
            try
            {
                Idioma idioma = idiomaBLL.CrearIdioma(new Idioma { Nombre = txtbNombreIdioma.Text });
                CargarIdiomas(idioma.Id);
                RefrescarIdiomaRuntimeSiCorresponde(idioma.Id);
                UiDialogHelper.MostrarInfo("FormLanguageManagement.Success.LanguageCreated");
            }
            catch (Exception ex)
            {
                UiExceptionHandler.Mostrar(ex, "Alta de idioma");
            }
        }

        private void btnActualizarIdioma_Click(object sender, EventArgs e)
        {
            if (!(cmbIdiomas.SelectedItem is Idioma idioma))
            {
                UiDialogHelper.MostrarWarning("FormLanguageManagement.Warning.SelectLanguage");
                return;
            }

            try
            {
                idioma.Nombre = txtbNombreIdioma.Text;
                bool actualizado = idiomaBLL.ActualizarIdioma(idioma);
                if (actualizado)
                {
                    CargarIdiomas(idioma.Id);
                    RefrescarIdiomaRuntimeSiCorresponde(idioma.Id);
                    UiDialogHelper.MostrarInfo("FormLanguageManagement.Success.LanguageUpdated");
                }
            }
            catch (Exception ex)
            {
                UiExceptionHandler.Mostrar(ex, "Edicion de idioma");
            }
        }

        private void btnEliminarIdioma_Click(object sender, EventArgs e)
        {
            if (!(cmbIdiomas.SelectedItem is Idioma idioma))
            {
                UiDialogHelper.MostrarWarning("FormLanguageManagement.Warning.SelectLanguage");
                return;
            }

            DialogResult confirmacion = UiDialogHelper.Confirmar(
                "FormLanguageManagement.Confirm.DeleteLanguageMessage",
                "FormLanguageManagement.Confirm.DeleteLanguageTitle",
                MessageBoxIcon.Warning,
                idioma.Nombre);

            if (confirmacion != DialogResult.Yes)
            {
                return;
            }

            try
            {
                bool eliminado = idiomaBLL.EliminarIdioma(idioma);
                if (eliminado)
                {
                    CargarIdiomas(null);
                    RefrescarIdiomaRuntimeSiCorresponde(idioma.Id);
                    UiDialogHelper.MostrarInfo("FormLanguageManagement.Success.LanguageDeleted");
                }
            }
            catch (Exception ex)
            {
                UiExceptionHandler.Mostrar(ex, "Baja de idioma");
            }
        }

        private void btnImportarJson_Click(object sender, EventArgs e)
        {
            if (!(cmbIdiomas.SelectedItem is Idioma idioma))
            {
                UiDialogHelper.MostrarWarning("FormLanguageManagement.Warning.SelectLanguage");
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
                    idiomaBLL.ImportarJson(idioma, openFileDialog.FileName);
                    RecargarIdiomaSeleccionado(idioma.Id);
                    RefrescarIdiomaRuntimeSiCorresponde(idioma.Id);
                    UiDialogHelper.MostrarInfo("FormLanguageManagement.Success.TranslationsImported");
                }
                catch (Exception ex)
                {
                    UiExceptionHandler.Mostrar(ex, "Importacion de traducciones");
                }
            }
        }

        private void btnExportarJson_Click(object sender, EventArgs e)
        {
            if (!(cmbIdiomas.SelectedItem is Idioma idioma))
            {
                UiDialogHelper.MostrarWarning("FormLanguageManagement.Warning.SelectLanguage");
                return;
            }

            DialogResult confirmacion = UiDialogHelper.Confirmar(
                "FormLanguageManagement.ExportConfirmMessage",
                "FormLanguageManagement.ExportConfirmTitle",
                MessageBoxIcon.Warning,
                idioma.Nombre);

            if (confirmacion != DialogResult.Yes)
            {
                return;
            }

            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter = "Archivos JSON (*.json)|*.json";
                saveFileDialog.Title = UiDialogHelper.Traducir("FormLanguageManagement.ExportDialogTitle");
                saveFileDialog.FileName = GenerarNombreArchivoIdioma(idioma.Nombre);
                saveFileDialog.DefaultExt = "json";
                saveFileDialog.AddExtension = true;
                saveFileDialog.OverwritePrompt = true;

                if (saveFileDialog.ShowDialog(this) != DialogResult.OK)
                {
                    return;
                }

                try
                {
                    idiomaBLL.ExportarJson(idioma, saveFileDialog.FileName);
                    UiDialogHelper.MostrarInfo(
                        "FormLanguageManagement.ExportSuccessMessage",
                        new object[] { Path.GetFileName(saveFileDialog.FileName) });
                }
                catch (Exception ex)
                {
                    UiExceptionHandler.Mostrar(ex, "Exportacion de traducciones");
                }
            }
        }

        private void btnGuardarTraduccion_Click(object sender, EventArgs e)
        {
            if (!(cmbIdiomas.SelectedItem is Idioma idioma))
            {
                UiDialogHelper.MostrarWarning("FormLanguageManagement.Warning.SelectLanguage");
                return;
            }

            try
            {
                string keySeleccionada = txtbKeyManual.Text;
                idiomaBLL.GuardarTraduccion(new Traduccion { Idioma = idioma, Palabra = new Palabra { Key = keySeleccionada }, Valor = txtbValorManual.Text });
                RecargarIdiomaSeleccionado(idioma.Id, keySeleccionada);
                RefrescarIdiomaRuntimeSiCorresponde(idioma.Id);
                UiDialogHelper.MostrarInfo("FormLanguageManagement.Success.TranslationSaved");
            }
            catch (Exception ex)
            {
                UiExceptionHandler.Mostrar(ex, "Guardado de traduccion");
            }
        }

        private void btnEliminarTraduccion_Click(object sender, EventArgs e)
        {
            if (!(cmbIdiomas.SelectedItem is Idioma idioma))
            {
                UiDialogHelper.MostrarWarning("FormLanguageManagement.Warning.SelectLanguage");
                return;
            }

            if (!(dgvTraducciones.CurrentRow?.DataBoundItem is FilaTraduccion item))
            {
                UiDialogHelper.MostrarWarning("FormLanguageManagement.Warning.SelectTranslation");
                return;
            }

            try
            {
                bool eliminado = idiomaBLL.EliminarTraduccion(item.Traduccion);
                if (eliminado)
                {
                    LimpiarEdicionManual();
                    RecargarIdiomaSeleccionado(idioma.Id);
                    RefrescarIdiomaRuntimeSiCorresponde(idioma.Id);
                }
            }
            catch (Exception ex)
            {
                UiExceptionHandler.Mostrar(ex, "Baja de traduccion");
            }
        }

        private void cmbIdiomas_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cargandoIdiomas || !(cmbIdiomas.SelectedItem is Idioma idioma))
            {
                return;
            }

            LimpiarEdicionManual();
            RefrescarDatos(idioma.Id, null);
        }

        private void dgvTraducciones_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
            {
                return;
            }

            CargarTraduccionSeleccionada();
        }

        private void CargarIdiomas(int? idiomaSeleccionadoId)
        {
            cargandoIdiomas = true;

            List<Idioma> idiomas = idiomaBLL.ObtenerIdiomas();
            cmbIdiomas.DataSource = null;
            cmbIdiomas.DisplayMember = "Nombre";
            cmbIdiomas.ValueMember = "Id";
            cmbIdiomas.DataSource = idiomas;

            // Sin seleccion previa se abre en el idioma default (la lista viene ordenada por nombre).
            Idioma idiomaSeleccionado = idiomaSeleccionadoId.HasValue
                ? idiomas.FirstOrDefault(item => item.Id == idiomaSeleccionadoId.Value)
                : idiomas.FirstOrDefault(item => item.Default);
            if (idiomaSeleccionado != null)
            {
                cmbIdiomas.SelectedItem = idiomaSeleccionado;
            }

            cargandoIdiomas = false;

            if (cmbIdiomas.SelectedItem is Idioma idioma)
            {
                RefrescarDatos(idioma.Id, null);
            }
        }

        private void RefrescarDatos(int idiomaId, string keySeleccionada)
        {
            Idioma idioma = idiomaBLL.ObtenerIdioma(cmbIdiomas.SelectedItem as Idioma);
            if (idioma == null)
            {
                return;
            }

            txtbNombreIdioma.Text = idioma.Nombre;
            chkbIdiomaDefault.Checked = idioma.Default;

            dgvTraducciones.DataSource = null;
            dgvTraducciones.DataSource = idioma.Traducciones
                .OfType<Traduccion>()
                .OrderBy(item => item.Palabra.Key)
                .Select(ProyectarTraduccionFila)
                .ToList();
            SeleccionarTraduccion(keySeleccionada);
        }

        private void RecargarIdiomaSeleccionado(int idiomaId)
        {
            RecargarIdiomaSeleccionado(idiomaId, null);
        }

        private void RecargarIdiomaSeleccionado(int idiomaId, string keySeleccionada)
        {
            CargarIdiomas(idiomaId);
            SeleccionarTraduccion(keySeleccionada);
        }

        private void RefrescarIdiomaRuntimeSiCorresponde(int idiomaIdAfectado)
        {
            IIdioma idiomaActual = IdiomaService.GetInstance().IdiomaActual;
            Idioma idiomaDefault = idiomaBLL.ObtenerIdiomaDefault();

            if (idiomaActual != null && idiomaActual.Id == idiomaIdAfectado)
            {
                Idioma idiomaRecargado = idiomaBLL.ObtenerIdioma(idiomaActual as Idioma);
                IdiomaService.GetInstance().CambiarIdioma(idiomaRecargado ?? idiomaDefault);
                return;
            }

            if (idiomaDefault != null && idiomaActual != null && idiomaDefault.Id == idiomaActual.Id)
            {
                IdiomaService.GetInstance().CambiarIdioma(idiomaDefault);
            }
        }

        private void CargarTraduccionSeleccionada()
        {
            if (!(dgvTraducciones.CurrentRow?.DataBoundItem is FilaTraduccion item))
            {
                LimpiarEdicionManual();
                return;
            }

            txtbKeyManual.Text = item.Key;
            txtbValorManual.Text = item.Valor;
        }

        private void SeleccionarTraduccion(string keySeleccionada)
        {
            if (dgvTraducciones.Rows.Count == 0)
            {
                LimpiarEdicionManual();
                return;
            }

            DataGridViewRow fila = null;

            if (!string.IsNullOrWhiteSpace(keySeleccionada))
            {
                fila = dgvTraducciones.Rows
                    .Cast<DataGridViewRow>()
                    .FirstOrDefault(item =>
                    {
                        FilaTraduccion traduccion = item.DataBoundItem as FilaTraduccion;
                        return traduccion != null &&
                               string.Equals(traduccion.Key, keySeleccionada, StringComparison.Ordinal);
                    });
            }

            if (fila == null)
            {
                fila = dgvTraducciones.Rows[0];
            }

            dgvTraducciones.ClearSelection();
            fila.Selected = true;

            if (fila.Cells.Count > 0)
            {
                dgvTraducciones.CurrentCell = fila.Cells[0];
            }

            CargarTraduccionSeleccionada();
        }

        private void LimpiarEdicionManual()
        {
            txtbKeyManual.Clear();
            txtbValorManual.Clear();
        }
        private string ObtenerTextoActual(string key)
        {
            IIdioma idioma = IdiomaService.GetInstance().IdiomaActual;
            return idioma == null ? key : idioma.ObtenerTraduccion(key);
        }

        private static string GenerarNombreArchivoIdioma(string nombreIdioma)
        {
            string nombreBase = string.IsNullOrWhiteSpace(nombreIdioma) ? "idioma" : nombreIdioma.Trim();

            foreach (char caracterInvalido in Path.GetInvalidFileNameChars())
            {
                nombreBase = nombreBase.Replace(caracterInvalido, '_');
            }

            return string.Format("traducciones-{0}.json", nombreBase);
        }

        private FilaTraduccion ProyectarTraduccionFila(Traduccion traduccion)
        {
            return new FilaTraduccion
            {
                Traduccion = traduccion,
                PalabraId = traduccion.Palabra.Id,
                Key = traduccion.Palabra.Key,
                Valor = traduccion.Valor
            };
        }

        private void btnIdiomaDefault_Click(object sender, EventArgs e)
        {

            if (!(cmbIdiomas.SelectedItem is Idioma idioma))
            {
                UiDialogHelper.MostrarWarning("FormLanguageManagement.Warning.SelectLanguage");
                return;
            }

            try
            {
                bool actualizado = idiomaBLL.HacerIdiomaDefault(idioma);
                if (actualizado)
                {
                    CargarIdiomas(idioma.Id);
                    RefrescarIdiomaRuntimeSiCorresponde(idioma.Id);
                    UiDialogHelper.MostrarInfo("FormLanguageManagement.Success.LanguageUpdated");
                }
            }
            catch (Exception ex)
            {
                UiExceptionHandler.Mostrar(ex, "Edicion de idioma");
            }

        }
    }
}
