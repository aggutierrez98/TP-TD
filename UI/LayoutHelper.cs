using System;
using System.Windows.Forms;

namespace TpIDS
{
    /// <summary>Ayudas de layout para lo que Anchor/Dock no resuelven solos.</summary>
    internal static class LayoutHelper
    {
        /// <summary>
        /// El bloque se ensancha con su contenedor hasta <paramref name="anchoMaximo"/> y despues queda fijo y
        /// centrado; en altura conserva su alto y reparte el espacio sobrante arriba y abajo. Los margenes que
        /// tiene el bloque al llamar al metodo son los minimos: llamarlo cuando el contenedor ya tiene su tamaño
        /// de apertura (por ejemplo, en el Load del formulario).
        /// </summary>
        public static void CentrarConAnchoMaximo(Control bloque, int anchoMaximo)
        {
            Control contenedor = bloque.Parent ?? throw new ArgumentException("El bloque no tiene contenedor.", nameof(bloque));
            int margenIzquierdo = bloque.Left;
            int margenesHorizontales = contenedor.ClientSize.Width - bloque.Width;
            int margenSuperior = bloque.Top;
            int margenesVerticales = contenedor.ClientSize.Height - bloque.Height;

            // Anchor se desactiva: la posicion y el ancho los decide este helper.
            bloque.Anchor = AnchorStyles.Top | AnchorStyles.Left;

            void Reubicar()
            {
                int anchoDisponible = contenedor.ClientSize.Width - margenesHorizontales;
                int ancho = Math.Max(bloque.MinimumSize.Width, Math.Min(anchoDisponible, anchoMaximo));
                int sobranteAncho = Math.Max(0, anchoDisponible - ancho);
                int sobranteAlto = Math.Max(0, contenedor.ClientSize.Height - margenesVerticales - bloque.Height);

                bloque.SetBounds(margenIzquierdo + sobranteAncho / 2, margenSuperior + sobranteAlto / 2, ancho, bloque.Height);
            }

            contenedor.Resize += (sender, e) => Reubicar();
            Reubicar();
        }
    }
}
