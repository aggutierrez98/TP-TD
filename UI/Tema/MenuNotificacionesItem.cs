using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace TpIDS
{
    internal sealed class MenuNotificacionesItem : ToolStripMenuItem
    {
        private int cantidad;
        public void ActualizarCantidad(int valor)
        {
            cantidad = valor;
            Owner?.PerformLayout();
            Invalidate();
        }

        public override Size GetPreferredSize(Size constrainingSize)
        {
            Size normal = base.GetPreferredSize(constrainingSize);
            if (cantidad > 0)
            {
                int anchoTexto = TextRenderer.MeasureText(Text, Font).Width;
                int anchoBadge = TextRenderer.MeasureText(cantidad.ToString(), Font).Width + 8;
                normal.Width = anchoTexto + anchoBadge + 14;
            }
            return normal;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (cantidad <= 0 || Owner == null)
            {
                base.OnPaint(e);
                return;
            }
            string texto = cantidad.ToString();
            int ancho = TextRenderer.MeasureText(texto, Font).Width + 8;
            int alto = System.Math.Min(Height - 4, Font.Height + 4);
            var rect = new Rectangle(Width - ancho - 7, (Height - alto) / 2, ancho, alto);
            // Padding no limita el rectángulo de texto que usa ToolStripMenuItem.
            // El renderer recibe un área explícita y separada del contador.
            Owner.Renderer.DrawMenuItemBackground(new ToolStripItemRenderEventArgs(e.Graphics, this));
            var areaTexto = new Rectangle(4, 0, System.Math.Max(0, rect.Left - 7), Height);
            Owner.Renderer.DrawItemText(new ToolStripItemTextRenderEventArgs(e.Graphics, this, Text, areaTexto,
                ForeColor, Font, TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.SingleLine));
            SmoothingMode anterior = e.Graphics.SmoothingMode;
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            using (var fondo = new SolidBrush(Color.FromArgb(48, 137, 74))) e.Graphics.FillEllipse(fondo, rect);
            TextRenderer.DrawText(e.Graphics, texto, Font, rect, Color.White, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPadding);
            e.Graphics.SmoothingMode = anterior;
        }
    }
}
