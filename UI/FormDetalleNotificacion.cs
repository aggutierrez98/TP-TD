using System.Drawing;
using System.Windows.Forms;
using ABSTRACCIONES;
using BE;
using Krypton.Toolkit;

namespace TpIDS
{
    public sealed class FormDetalleNotificacion : FormTraducibleBase
    {
        private readonly Notificacion notificacion;
        private readonly KryptonLabel lblDatosNotificacion;

        public FormDetalleNotificacion(Notificacion notificacion)
        {
            this.notificacion = notificacion;
            Name = "FormDetalleNotificacion";
            Text = "Detalle de notificación";
            ClientSize = new Size(610, 330);
            StartPosition = FormStartPosition.CenterParent;
            lblDatosNotificacion = new KryptonLabel { Name = "lblDatosNotificacion", Location = new Point(18, 15), Size = new Size(574, 28), Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right };
            var txtbMensajeNotificacion = new KryptonTextBox { Name = "txtbMensajeNotificacion", Text = notificacion.Mensaje, ReadOnly = true, Multiline = true, ScrollBars = ScrollBars.Vertical, Location = new Point(18, 55), Size = new Size(574, 210), Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right };
            var btnCerrarDetalle = new KryptonButton { Name = "btnCerrarDetalle", Text = "Cerrar", Tag = "FormNotificationDetail.Close", ButtonStyle = ButtonStyle.Custom1, DialogResult = DialogResult.OK, Location = new Point(472, 283), Size = new Size(120, 30), Anchor = AnchorStyles.Bottom | AnchorStyles.Right };
            AcceptButton = btnCerrarDetalle;
            CancelButton = btnCerrarDetalle;
            Controls.AddRange(new Control[] { lblDatosNotificacion, txtbMensajeNotificacion, btnCerrarDetalle });
            ActualizarDatos();
        }

        private void ActualizarDatos()
        {
            Text = UiDialogHelper.Traducir("FormNotificationDetail.Title");
            lblDatosNotificacion.Text = notificacion.Fecha.ToString("g") + " · " + FormMisNotificaciones.TextoTipo(notificacion);
        }

        public override void Actualizar(IObservado observado)
        {
            base.Actualizar(observado);
            ActualizarDatos();
        }
    }
}
