using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using ABSTRACCIONES;
using BLL;
using Krypton.Toolkit;

namespace TpIDS
{
    public class FormTraducibleBase : KryptonForm, IObservador
    {
        private const int WM_SYSCOMMAND = 0x0112;
        private const int SC_MASCARA = 0xFFF0;
        private const int SC_SIZE = 0xF000;
        private const int SC_MOVE = 0xF010;
        private const int SC_MAXIMIZE = 0xF030;
        private const int SC_RESTORE = 0xF120;

        // Maximizado de un hijo MDI dentro del area de trabajo (ver MaximizarEnAreaMdi).
        private MdiClient areaMdi;
        private Rectangle limitesAntesDeMaximizar;

        /// <summary>True mientras el hijo MDI ocupa el area de trabajo con su propia barra de titulo.</summary>
        protected bool MaximizadoEnAreaMdi => areaMdi != null;

        protected override void OnLoad(EventArgs e)
        {
            // Antes de base.OnLoad: el tamaño de apertura es el de diseño (los handlers Load de
            // Balance y Dashboard todavia no agrandaron la ventana al area MDI).
            AsignarTamanioMinimo();

            // Antes de base.OnLoad: asi se tematiza antes de los handlers Load de los forms hijos.
            TemaFinanzapp.Tematizar(this);
            if (IsMdiContainer)
            {
                TemaFinanzapp.TematizarAreaMdi(this);
            }

            base.OnLoad(e);
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            RegistrarObservadorIdioma();
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            DejarDeSeguirAreaMdi();
            DesregistrarObservadorIdioma();
            base.OnFormClosed(e);
        }

        protected override void WndProc(ref Message m)
        {
            // El boton maximizar de KryptonForm, el doble clic en la barra y el menu de sistema
            // llegan como WM_SYSCOMMAND. En un hijo MDI el maximizado nativo fusiona la barra de
            // titulo con la ventana principal, asi que se reemplaza por MaximizarEnAreaMdi.
            if (m.Msg == WM_SYSCOMMAND && IsMdiChild)
            {
                int comando = m.WParam.ToInt32() & SC_MASCARA;
                if (comando == SC_MAXIMIZE)
                {
                    AlternarMaximizadoEnAreaMdi();
                    return;
                }

                // Como una ventana maximizada nativa: no se mueve ni se redimensiona por el borde.
                if (MaximizadoEnAreaMdi && (comando == SC_MOVE || comando == SC_SIZE))
                {
                    return;
                }

                if (MaximizadoEnAreaMdi && comando == SC_RESTORE)
                {
                    // Restaurada desde minimizada: el area de trabajo pudo cambiar mientras tanto.
                    base.WndProc(ref m);
                    AjustarAlAreaMdi();
                    return;
                }
            }

            base.WndProc(ref m);
        }

        /// <summary>
        /// Ocupa el area de trabajo de la ventana principal sin fusionar la barra de titulo
        /// (WindowState sigue en Normal) y la sigue si la ventana principal cambia de tamaño.
        /// </summary>
        protected void MaximizarEnAreaMdi()
        {
            if (MaximizadoEnAreaMdi)
            {
                return;
            }

            MdiClient mdiClient = MdiParent?.Controls.OfType<MdiClient>().FirstOrDefault();
            if (mdiClient == null)
            {
                return;
            }

            if (WindowState != FormWindowState.Normal)
            {
                WindowState = FormWindowState.Normal;
            }

            limitesAntesDeMaximizar = Bounds;
            areaMdi = mdiClient;
            areaMdi.Resize += areaMdi_Resize;
            AjustarAlAreaMdi();
            MostrarIconoRestaurar(true);
        }

        private void RestaurarDesdeAreaMdi()
        {
            DejarDeSeguirAreaMdi();
            Bounds = limitesAntesDeMaximizar;
            MostrarIconoRestaurar(false);
        }

        private void AlternarMaximizadoEnAreaMdi()
        {
            if (MaximizadoEnAreaMdi && WindowState == FormWindowState.Normal)
            {
                RestaurarDesdeAreaMdi();
                return;
            }

            if (MaximizadoEnAreaMdi)
            {
                // Minimizado mientras estaba maximizado: vuelve al area de trabajo completa.
                WindowState = FormWindowState.Normal;
                AjustarAlAreaMdi();
                return;
            }

            MaximizarEnAreaMdi();
        }

        private void areaMdi_Resize(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Normal)
            {
                AjustarAlAreaMdi();
            }
        }

        private void AjustarAlAreaMdi()
        {
            Bounds = new Rectangle(Point.Empty, areaMdi.ClientSize);
        }

        private void DejarDeSeguirAreaMdi()
        {
            if (areaMdi != null)
            {
                areaMdi.Resize -= areaMdi_Resize;
                areaMdi = null;
            }
        }

        /// <summary>
        /// KryptonForm elige el icono maximizar/restaurar segun WindowState, que aca queda en Normal:
        /// se fuerza con las imagenes FormRestore de la paleta mientras dura el maximizado.
        /// </summary>
        private void MostrarIconoRestaurar(bool restaurar)
        {
            PaletteBase paleta = GetResolvedPalette() ?? KryptonManager.CurrentGlobalPalette;
            ButtonImageStates imagenes = ButtonSpecMax.ImageStates;
            imagenes.ImageNormal = restaurar ? paleta.GetButtonSpecImage(PaletteButtonSpecStyle.FormRestore, PaletteState.Normal) : null;
            imagenes.ImageTracking = restaurar ? paleta.GetButtonSpecImage(PaletteButtonSpecStyle.FormRestore, PaletteState.Tracking) : null;
            imagenes.ImagePressed = restaurar ? paleta.GetButtonSpecImage(PaletteButtonSpecStyle.FormRestore, PaletteState.Pressed) : null;
            imagenes.ImageDisabled = restaurar ? paleta.GetButtonSpecImage(PaletteButtonSpecStyle.FormRestore, PaletteState.Disabled) : null;
        }

        /// <summary>
        /// El minimo de un form redimensionable es su tamaño de apertura: se asigna en runtime y no en el
        /// designer porque el tamaño exterior depende del marco que dibuja Krypton.
        /// </summary>
        private void AsignarTamanioMinimo()
        {
            bool redimensionable = FormBorderStyle == FormBorderStyle.Sizable
                || FormBorderStyle == FormBorderStyle.SizableToolWindow;
            if (redimensionable && !IsMdiContainer && MinimumSize.IsEmpty)
            {
                MinimumSize = Size;
            }
        }

        public virtual void Actualizar(IObservado observado)
        {
            IdiomaFormHelper.Aplicar(this, observado.IdiomaActual);
        }

        private void RegistrarObservadorIdioma()
        {
            CompositionRoot.ServicioIdioma.RegistrarObservador(this);
        }

        private void DesregistrarObservadorIdioma()
        {
            CompositionRoot.ServicioIdioma.DesregistrarObservador(this);
        }
    }
}
