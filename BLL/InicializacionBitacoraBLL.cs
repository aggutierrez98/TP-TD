using REPOSITORIO;
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

                BitacoraRepositorio bitacoraRepositorio = new BitacoraRepositorio();
                Sesion sesion = Sesion.GetInstance();

                BitacoraService.GetInstance().Configurar(
                    (evento, contexto) => bitacoraRepositorio.Guardar(evento, contexto as IUnidadDeTrabajo),
                    filtro => bitacoraRepositorio.Buscar(filtro),
                    sesion.GetUsuarioActualId,
                    sesion.GetUsuarioActualNombre);

                _inicializado = true;
            }
        }
    }
}
