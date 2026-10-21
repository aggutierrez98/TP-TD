using REPOSITORIO;
using SERVICIOS;

namespace BLL
{
    public static class InicializacionHistorialBLL
    {
        private static bool _inicializado;

        public static void Inicializar()
        {
            if (_inicializado)
            {
                return;
            }

            UsuarioHistorialRepositorio usuarioHistorialRepositorio = new UsuarioHistorialRepositorio();
            HistorialEstadosService.GetInstance().Configurar(
                usuarioId => usuarioHistorialRepositorio.ObtenerVersiones(usuarioId),
                snapshot => usuarioHistorialRepositorio.ObtenerPorId(snapshot.HistorialId));

            _inicializado = true;
        }
    }
}
