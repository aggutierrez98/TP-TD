using System;
using System.Windows.Forms;
using SERVICIOS;

namespace TpIDS
{
    public class FormTraducibleBase : Form
    {
        private readonly Action<Idioma> _observadorIdioma;
        private bool _idiomaRegistrado;

        public FormTraducibleBase()
        {
            _observadorIdioma = AplicarIdiomaInterno;
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            RegistrarObservadorIdioma();
            AplicarIdiomaActual();
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            DesregistrarObservadorIdioma();
            base.OnFormClosed(e);
        }

        protected virtual void OnIdiomaActualizado(Idioma idioma)
        {
            IdiomaFormHelper.Aplicar(this, idioma);
        }

        protected void AplicarIdiomaActual()
        {
            OnIdiomaActualizado(IdiomaService.GetInstance().IdiomaActual);
        }

        private void RegistrarObservadorIdioma()
        {
            if (_idiomaRegistrado)
            {
                return;
            }

            IdiomaService.GetInstance().RegistrarObservador(_observadorIdioma);
            _idiomaRegistrado = true;
        }

        private void DesregistrarObservadorIdioma()
        {
            if (!_idiomaRegistrado)
            {
                return;
            }

            IdiomaService.GetInstance().DesregistrarObservador(_observadorIdioma);
            _idiomaRegistrado = false;
        }

        private void AplicarIdiomaInterno(Idioma idioma)
        {
            OnIdiomaActualizado(idioma);
        }
    }
}
