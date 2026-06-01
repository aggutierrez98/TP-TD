using System;
using SERVICIOS;

namespace BLL
{
    public static class ManejadorExcepcionesBLL
    {
        public static string ObtenerMensajeUsuario(Exception exception, string actividad)
        {
            if (exception is ExcepcionNegocio)
            {
                return exception.Message;
            }

            RegistrarExcepcionTecnica(exception, actividad, "UI");
            return "Ocurrio un error tecnico inesperado.";
        }

        public static void RegistrarExcepcionTecnica(Exception exception, string actividad, string origen)
        {
            InfraestructuraBLL.Inicializar();

            try
            {
                Sesion.GetInstance().RegistrarExcepcionTecnica(actividad, exception, origen);
            }
            catch
            {
            }
        }
    }
}
