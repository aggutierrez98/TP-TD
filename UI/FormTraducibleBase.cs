using System;
using System.Windows.Forms;
using ABSTRACCIONES;
using BLL;

namespace TpIDS
{
    public class FormTraducibleBase : Form, IObservador
    {
        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            RegistrarObservadorIdioma();
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            DesregistrarObservadorIdioma();
            base.OnFormClosed(e);
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
