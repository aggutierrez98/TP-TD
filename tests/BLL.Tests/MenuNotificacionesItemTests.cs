using System.Drawing;
using System.Windows.Forms;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TpIDS;

namespace BLL.Tests
{
    [TestClass]
    public sealed class MenuNotificacionesItemTests
    {
        [STATestMethod]
        [DataRow("Mis notificaciones", 1)]
        [DataRow("My notifications", 25)]
        [DataRow("Mis notificaciones", 1000)]
        public void Dibujar_ContadorVisible_ReservaAreaSeparadaDelTexto(string texto, int cantidad)
        {
            using (var menu = new MenuStrip { Size = new Size(600, 28) })
            using (var item = new MenuNotificacionesItem { Text = texto })
            using (var imagen = new Bitmap(600, 28))
            {
                var renderer = new RendererCaptura();
                menu.Renderer = renderer;
                menu.Items.Add(item);
                item.ActualizarCantidad(cantidad);
                menu.PerformLayout();
                menu.DrawToBitmap(imagen, new Rectangle(Point.Empty, imagen.Size));
                int anchoBadge = TextRenderer.MeasureText(cantidad.ToString(), item.Font).Width + 8;
                int izquierdaBadge = item.Width - anchoBadge - 7;
                Assert.IsTrue(renderer.TextoDibujado, "El renderer debe dibujar el texto del menú.");
                Assert.AreEqual(3, izquierdaBadge - renderer.AreaTexto.Right,
                    "El área de texto invade el espacio reservado al badge.");
                Assert.AreEqual(TextRenderer.MeasureText(texto, item.Font).Width, renderer.AreaTexto.Width,
                    "El espacio reservado debe permitir leer el título completo.");
            }
        }

        private sealed class RendererCaptura : ToolStripProfessionalRenderer
        {
            public bool TextoDibujado { get; private set; }
            public Rectangle AreaTexto { get; private set; }
            protected override void OnRenderItemText(ToolStripItemTextRenderEventArgs e)
            {
                TextoDibujado = true;
                AreaTexto = e.TextRectangle;
                base.OnRenderItemText(e);
            }
        }
    }
}
