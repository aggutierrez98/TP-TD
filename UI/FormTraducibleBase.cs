using System;
using System.Windows.Forms;
using SERVICIOS;

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

        public virtual void Actualizar(IdiomaService idiomaService)
        {
            IdiomaFormHelper.Aplicar(this, idiomaService.IdiomaActual);
        }

        private void RegistrarObservadorIdioma()
        {
            IdiomaService.GetInstance().RegistrarObservador(this);
        }

        private void DesregistrarObservadorIdioma()
        {
            IdiomaService.GetInstance().DesregistrarObservador(this);
        }
    }
}
