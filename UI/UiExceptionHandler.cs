using System;
using BLL;
using SERVICIOS;

namespace TpIDS
{
    internal static class UiExceptionHandler
    {
        public static void Mostrar(Exception exception, string actividad)
        {
            string messageKey = ManejadorExcepcionesBLL.ObtenerMensajeUsuarioKey(exception, actividad);
            if (exception is ExcepcionNegocio)
            {
                UiDialogHelper.MostrarWarning(messageKey);
                return;
            }

            UiDialogHelper.MostrarError(messageKey);
        }
    }
}
