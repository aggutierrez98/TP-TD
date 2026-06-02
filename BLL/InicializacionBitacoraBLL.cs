using DAL;
using SERVICIOS;

namespace BLL
{
    public static class InicializacionBitacoraBLL
    {
        private static readonly object Sync = new object();
        private static bool _inicializado;

        public static void Inicializar()
        {
            if (_inicializado)
            {
                return;
            }

            lock (Sync)
            {
                if (_inicializado)
                {
                    return;
                }

                BitacoraDAL bitacoraDAL = new BitacoraDAL();
                Sesion sesion = Sesion.GetInstance();

                BitacoraService.GetInstance().Configurar(
                    (evento, contexto) => bitacoraDAL.Guardar(evento, contexto as AccesoDatos),
                    filtro => bitacoraDAL.Buscar(filtro),
                    sesion.GetUsuarioActualId,
                    sesion.GetUsuarioActualNombre);

                _inicializado = true;
            }
        }
    }
}
