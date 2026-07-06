using System;
using System.Windows.Forms;
using SERVICIOS;

namespace TpIDS
{
    // Pantalla de contingencia que se muestra cuando la verificacion de integridad
    // (T07) detecta una alteracion de datos por fuera del sistema. Bloquea el acceso
    // al login e informa al administrador la tabla afectada y el tipo de falla.
    public partial class FormContingenciaIntegridad : FormTraducibleBase
    {
        private readonly ExcepcionIntegridad _falla;

        public FormContingenciaIntegridad(ExcepcionIntegridad falla)
        {
            _falla = falla;
            InitializeComponent();
            AplicarDetalleFalla();
        }

        public override void Actualizar(IdiomaService idiomaService)
        {
            base.Actualizar(idiomaService);
            // El mensaje depende del tipo de falla y de la tabla, por lo que se reaplica
            // luego de la traduccion automatica basada en Tag.
            AplicarDetalleFalla();
        }

        private void AplicarDetalleFalla()
        {
            if (_falla == null)
            {
                return;
            }

            string tabla = _falla.Tabla ?? string.Empty;
            string mensajeKey = _falla.Tipo == TipoFalloIntegridad.Vertical
                ? "FormContingencia.Message.Vertical"
                : "FormContingencia.Message.Horizontal";

            lblTablaValor.Text = tabla;
            lblMensaje.Text = UiDialogHelper.Traducir(mensajeKey, tabla);
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
