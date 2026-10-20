using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Krypton.Toolkit;

namespace TpIDS
{
    /// <summary>
    /// Punto unico de arranque del tema visual: crea el KryptonManager de la aplicacion y le aplica
    /// la paleta de FINAZAPP. Se llama una sola vez desde Program.Main, antes de crear cualquier form.
    /// Tambien tematiza los controles nativos de WinForms que conviven con los de Krypton.
    /// </summary>
    internal static class TemaFinanzapp
    {
        // Fuente base: Segoe UI viene con Windows, asi el proyecto se ve igual en cualquier maquina.
        private static readonly Font FuenteBase = new Font("Segoe UI", 9F);

        // Referencias estaticas: el manager y la paleta deben vivir lo mismo que la aplicacion.
        private static KryptonManager manager;
        private static KryptonCustomPaletteBase paleta;

        public static void Aplicar()
        {
            if (manager != null)
            {
                return;
            }

            manager = new KryptonManager();
            paleta = FinanzappPaleta.Crear();
            ThemeManager.ApplyTheme(paleta, manager);
            manager.BaseFont = FuenteBase;
            manager.GlobalUseThemeFormChromeBorderWidth = true;
            manager.GlobalApplyToolstrips = true;

            // La flecha de los combos y fechas se dibuja como poligono: en modo Unicode depende de una fuente
            // con el glifo y no se veia.
            manager.GlobalDropDownArrowRenderMode = DropDownArrowRenderMode.Polygon;
        }

        /// <summary>El area gris oscuro del contenedor MDI (MdiClient) se pinta con un fondo suave.</summary>
        public static void TematizarAreaMdi(Form contenedor)
        {
            foreach (Control control in contenedor.Controls)
            {
                if (control is MdiClient)
                {
                    control.BackColor = FinanzappPaleta.FondoMdi;
                }
            }
        }

        /// <summary>
        /// Estilo comun de las barras de progreso: pista verde clara, relleno verde, bordes redondeados y sin
        /// texto. En Krypton StateCommon.Back define el relleno y StateNormal.Back la pista completa.
        /// </summary>
        public static void TematizarBarraProgreso(KryptonProgressBar barra)
        {
            barra.UseValueAsText = false;
            barra.Text = string.Empty;
            barra.StateCommon.Back.Color1 = FinanzappPaleta.Verde;
            barra.StateCommon.Back.Color2 = FinanzappPaleta.Verde;
            barra.StateNormal.Back.Color1 = FinanzappPaleta.VerdeSuave;
            barra.StateNormal.Back.Color2 = FinanzappPaleta.VerdeSuave;
            barra.ValueBackColorStyle = PaletteColorStyle.Solid;
            barra.StateCommon.Border.Color1 = FinanzappPaleta.VerdeSuave;
            barra.StateCommon.Border.Rounding = 3;
        }

        /// <summary>Colores de la paleta para un DataGridView nativo (los tests UI lo necesitan nativo).</summary>
        public static void TematizarGrilla(DataGridView grilla)
        {
            grilla.BackgroundColor = FinanzappPaleta.Fondo;
            grilla.BorderStyle = BorderStyle.None;
            grilla.GridColor = FinanzappPaleta.Borde;
            grilla.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            grilla.EnableHeadersVisualStyles = false;

            grilla.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            grilla.ColumnHeadersDefaultCellStyle.BackColor = FinanzappPaleta.Verde;
            grilla.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            grilla.ColumnHeadersDefaultCellStyle.SelectionBackColor = FinanzappPaleta.Verde;
            grilla.ColumnHeadersDefaultCellStyle.SelectionForeColor = Color.White;
            grilla.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);

            grilla.DefaultCellStyle.BackColor = FinanzappPaleta.Fondo;
            grilla.DefaultCellStyle.ForeColor = FinanzappPaleta.Texto;
            grilla.DefaultCellStyle.SelectionBackColor = FinanzappPaleta.VerdeSuave;
            grilla.DefaultCellStyle.SelectionForeColor = FinanzappPaleta.Texto;
            grilla.AlternatingRowsDefaultCellStyle.BackColor = FinanzappPaleta.FilaAlterna;
        }

        /// <summary>
        /// KryptonForm ignora BackColor/ForeColor, por lo que los controles nativos hijos heredan el
        /// gris del sistema sobre el fondo blanco del form. Se les asigna explicitamente el color de
        /// la paleta. Los controles de Krypton no se tocan.
        /// </summary>
        public static void Tematizar(Control raiz)
        {
            // Copia: TematizarControl puede reubicar controles (RichTextBox dentro de un marco).
            foreach (Control control in raiz.Controls.Cast<Control>().ToArray())
            {
                TematizarControl(control);
                Tematizar(control);
            }
        }

        private static void TematizarControl(Control control)
        {
            if (control.GetType().Namespace.StartsWith("Krypton."))
            {
                return;
            }

            if (control is Label || control is CheckBox || control is RadioButton || control is GroupBox)
            {
                control.BackColor = FinanzappPaleta.Fondo;
                control.ForeColor = FinanzappPaleta.Texto;
            }
            else if (control is DataGridView grilla)
            {
                TematizarGrilla(grilla);
            }
            else if (control is TreeView || control is ListBox || control is NumericUpDown || control is RichTextBox)
            {
                // Excepciones nativas (ver design.md): TreeView, CheckedListBox, NumericUpDown y RichTextBox.
                control.BackColor = FinanzappPaleta.Fondo;
                control.ForeColor = FinanzappPaleta.Texto;
                if (control is RichTextBox texto)
                {
                    BordearRichTextBox(texto);
                }
            }
            else if (control is Panel || control is FlowLayoutPanel || control is TableLayoutPanel)
            {
                control.BackColor = FinanzappPaleta.Fondo;
            }
        }

        /// <summary>
        /// RichTextBox ignora BorderStyle.FixedSingle con estilos visuales y queda con el borde 3D del
        /// sistema, y KryptonForm no dispara Paint en su area cliente. Se quita el borde nativo y se lo
        /// envuelve en un panel verde con 1 px de padding, igual al borde de los KryptonTextBox.
        /// </summary>
        private static void BordearRichTextBox(RichTextBox texto)
        {
            Control contenedor = texto.Parent;
            if (contenedor == null || texto.BorderStyle == BorderStyle.None)
            {
                return;
            }

            var marco = new Panel
            {
                Bounds = texto.Bounds,
                Anchor = texto.Anchor,
                Dock = texto.Dock,
                Padding = new Padding(1),
                BackColor = FinanzappPaleta.Verde,
                TabStop = false
            };
            int indice = contenedor.Controls.GetChildIndex(texto);
            texto.BorderStyle = BorderStyle.None;
            texto.Dock = DockStyle.Fill;
            contenedor.Controls.Add(marco);
            contenedor.Controls.SetChildIndex(marco, indice);
            marco.Controls.Add(texto);
        }
    }
}
