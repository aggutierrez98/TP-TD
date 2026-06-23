using REPOSITORIO;
using SERVICIOS;

namespace BLL
{
    public static class InicializacionHistorialBLL
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

                UsuarioHistorialRepositorio usuarioHistorialRepositorio = new UsuarioHistorialRepositorio();
                HistorialEstadosService.GetInstance().Configurar(
                    (snapshot, contexto) => usuarioHistorialRepositorio.GuardarVersion(snapshot, contexto as IUnidadDeTrabajo),
                    usuarioId => usuarioHistorialRepositorio.ObtenerVersiones(usuarioId),
                    historialId => usuarioHistorialRepositorio.ObtenerVersion(historialId));

                _inicializado = true;
            }
        }
    }
}
