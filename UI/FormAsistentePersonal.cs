using System;
using System.Windows.Forms;
using BE;
using BLL;

namespace TpIDS
{
    // Chat libre con el asistente financiero (Gemini): por ahora sin recomendaciones
    // financieras ni inyeccion de datos del sistema (Movimientos, Categorias, etc.),
    // eso queda para una iteracion futura. Solo pregunta/respuesta con historial.
    public partial class FormAsistentePersonal : FormTraducibleBase
    {
        private readonly AsistenteFinancieroBLL asistenteBLL = CompositionRoot.AsistenteFinanciero;

        public FormAsistentePersonal()
        {
            InitializeComponent();
            Load += FormAsistentePersonal_Load;
        }

        private void FormAsistentePersonal_Load(object sender, EventArgs e)
        {
            CargarHistorial();
        }

        private void CargarHistorial()
        {
            try
            {
                rtbHistorialAsistente.Clear();
                foreach (ConsultaChatbot consulta in asistenteBLL.ObtenerHistorial())
                {
                    AgregarIntercambioAlHistorial(consulta.Pregunta, consulta.Respuesta, consulta.Fecha);
                }
            }
            catch (Exception ex)
            {
                UiExceptionHandler.Mostrar(ex, "Consulta de historial del asistente personal");
            }
        }

        private async void btnEnviarMensajeAsistente_Click(object sender, EventArgs e)
        {
            string pregunta = txtbMensajeAsistente.Text?.Trim();
            if (string.IsNullOrWhiteSpace(pregunta))
            {
                UiDialogHelper.MostrarWarning("FormPersonalAssistant.Warning.QuestionRequired");
                return;
            }

            CambiarEstadoEnvio(true);
            try
            {
                DateTime fecha = DateTime.Now;
                string respuesta = await asistenteBLL.EnviarMensajeAsync(pregunta);
                AgregarIntercambioAlHistorial(pregunta, respuesta, fecha);
                txtbMensajeAsistente.Clear();
            }
            catch (Exception ex)
            {
                UiExceptionHandler.Mostrar(ex, "Envio de mensaje al asistente financiero");
            }
            finally
            {
                CambiarEstadoEnvio(false);
                txtbMensajeAsistente.Focus();
            }
        }

        private void txtbMensajeAsistente_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter || e.Shift)
            {
                return;
            }

            e.SuppressKeyPress = true;
            if (btnEnviarMensajeAsistente.Enabled)
            {
                btnEnviarMensajeAsistente_Click(sender, EventArgs.Empty);
            }
        }

        private void CambiarEstadoEnvio(bool enCurso)
        {
            btnEnviarMensajeAsistente.Enabled = !enCurso;
            txtbMensajeAsistente.Enabled = !enCurso;
            UseWaitCursor = enCurso;
        }

        private void AgregarIntercambioAlHistorial(string pregunta, string respuesta, DateTime fecha)
        {
            string etiquetaUsuario = UiDialogHelper.Traducir("FormPersonalAssistant.Value.You");
            string etiquetaAsistente = UiDialogHelper.Traducir("FormPersonalAssistant.Value.Assistant");

            rtbHistorialAsistente.AppendText(string.Format("[{0:HH:mm}] {1}: {2}{3}", fecha, etiquetaUsuario, pregunta, Environment.NewLine));
            rtbHistorialAsistente.AppendText(string.Format("[{0:HH:mm}] {1}: {2}{3}{3}", fecha, etiquetaAsistente, respuesta, Environment.NewLine));
            rtbHistorialAsistente.SelectionStart = rtbHistorialAsistente.TextLength;
            rtbHistorialAsistente.ScrollToCaret();
        }
    }
}
