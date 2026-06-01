using System;
using System.Windows.Forms;
using BLL;
using SERVICIOS;

namespace TpIDS
{
    internal static class UiExceptionHandler
    {
        public static void Mostrar(Exception exception, string actividad, string titulo = "Error")
        {
            string mensaje = ManejadorExcepcionesBLL.ObtenerMensajeUsuario(exception, actividad);
            MessageBoxIcon icono = exception is ExcepcionNegocio
                ? MessageBoxIcon.Warning
                : MessageBoxIcon.Error;

            MessageBox.Show(mensaje, titulo, MessageBoxButtons.OK, icono);
        }
    }
}
