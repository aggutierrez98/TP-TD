using System;
using System.Windows.Forms;
using ABSTRACCIONES;
using SERVICIOS;

namespace TpIDS
{
    // Pantalla de contingencia que se muestra cuando la verificacion de integridad
    // (T07) detecta una alteracion de datos por fuera del sistema. Bloquea el acceso
    // al login con un mensaje generico; el detalle tecnico (tabla, registro) queda
    // solo en bitacora, no se expone en esta pantalla pre-login.
    public partial class FormContingenciaIntegridad : FormTraducibleBase
    {
        private readonly ExcepcionIntegridad _falla;

        public FormContingenciaIntegridad(ExcepcionIntegridad falla)
        {
            _falla = falla;
            InitializeComponent();
            AplicarDetalleFalla();
        }

        public override void Actualizar(IObservado observado)
        {
            base.Actualizar(observado);
            // El mensaje depende del tipo de falla, por lo que se reaplica luego de
            // la traduccion automatica basada en Tag.
            AplicarDetalleFalla();
        }

        private void AplicarDetalleFalla()
        {
            if (_falla == null)
            {
                return;
            }

            string mensajeKey = _falla.Tipo == TipoFalloIntegridad.Vertical
                ? "FormContingencia.Message.Vertical"
                : "FormContingencia.Message.Horizontal";

            lblMensaje.Text = UiDialogHelper.Traducir(mensajeKey);
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
