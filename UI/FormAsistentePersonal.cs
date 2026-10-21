using System;
using System.Windows.Forms;
using BE;
using BLL;

namespace TpIDS
{
    // Chat con el asistente financiero (Gemini), limitado a temas financieros: el BLL
    // aplica los guardrails y devuelve la respuesta o un rechazo con texto fijo traducible.
    // Los rechazos se ven solo en la sesion abierta del chat; no quedan en el historial.
    public partial class FormAsistentePersonal : FormTraducibleBase
    {
        private readonly AsistenteFinancieroBLL asistenteBLL = CompositionRoot.AsistenteFinanciero;

        // Un solo envio activo por formulario: se controla con este indicador ademas del
        // estado de los controles, porque Enter y el boton pueden dispararse en el mismo
        // ciclo de mensajes antes de que se deshabiliten.
        private bool envioEnCurso;
        private bool formularioCerrado;

        public FormAsistentePersonal()
        {
            InitializeComponent();
            Load += FormAsistentePersonal_Load;
            FormClosed += FormAsistentePersonal_FormClosed;
        }

        private void FormAsistentePersonal_Load(object sender, EventArgs e)
        {
            CargarHistorial();
        }

        private void FormAsistentePersonal_FormClosed(object sender, FormClosedEventArgs e)
        {
            formularioCerrado = true;
        }

        // El envio no se cancela al cerrar: la respuesta puede llegar despues y ya no hay
        // controles que actualizar ni formulario al cual asociar un dialogo.
        private bool FormularioDisponible
        {
            get { return !formularioCerrado && !IsDisposed && !Disposing; }
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
            if (envioEnCurso)
            {
                return;
            }

            string pregunta = txtbMensajeAsistente.Text?.Trim();
            if (string.IsNullOrWhiteSpace(pregunta))
            {
                UiDialogHelper.MostrarWarning("FormPersonalAssistant.Warning.QuestionRequired");
                return;
            }

            envioEnCurso = true;
            CambiarEstadoEnvio(true);
            try
            {
                DateTime fecha = DateTime.Now;
                RespuestaAsistente respuesta = await asistenteBLL.EnviarMensajeAsync(pregunta);
              
                if (!FormularioDisponible)
                {
                    return;
                }

                // Casos de rechazo si el usuario no cumple las condiciones del rag
                string textoRespuesta = respuesta.EsRechazo
                    ? UiDialogHelper.Traducir(respuesta.ClaveRechazo)
                    : respuesta.Texto;

                AgregarIntercambioAlHistorial(pregunta, textoRespuesta, fecha);
                txtbMensajeAsistente.Clear();
            }
            catch (Exception ex)
            {
                if (FormularioDisponible)
                {
                    UiExceptionHandler.Mostrar(ex, "Envio de mensaje al asistente financiero");
                }
            }
            finally
            {
                envioEnCurso = false;
                if (FormularioDisponible)
                {
                    CambiarEstadoEnvio(false);
                    txtbMensajeAsistente.Focus();
                }
            }
        }

        private void txtbMensajeAsistente_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter || e.Shift)
            {
                return;
            }

            e.SuppressKeyPress = true;
            if (!envioEnCurso && btnEnviarMensajeAsistente.Enabled)
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
