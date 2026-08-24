using System;
using SERVICIOS;

namespace BLL
{
    public static class ManejadorExcepcionesBLL
    {
        public static string ObtenerMensajeUsuarioKey(Exception exception, string actividad)
        {
            if (exception is ExcepcionNegocio)
            {
                return exception.Message;
            }

            if (exception is ExcepcionTecnica)
            {
                RegistrarExcepcionTecnica(exception, actividad, "UI");
                return exception.Message;
            }

            RegistrarExcepcionTecnica(exception, actividad, "UI");
            return "Error.Technical.Unexpected";
        }

        public static void RegistrarExcepcionTecnica(Exception exception, string actividad, string origen)
        {
            // Sin Inicializar() explicito: el getter CompositionRoot.Bitacora ya compone,
            // y dentro del try un fallo de composicion no tumba al manejador de excepciones.
            try
            {
                CompositionRoot.Bitacora.RegistrarExcepcionTecnica(actividad, exception, origen);
            }
            catch
            {
            }
        }
    }
}
