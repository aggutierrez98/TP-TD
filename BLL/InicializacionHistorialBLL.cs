using DAL;
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

                UsuarioHistorialDAL usuarioHistorialDAL = new UsuarioHistorialDAL();
                HistorialEstadosService.GetInstance().Configurar(
                    (snapshot, contexto) => usuarioHistorialDAL.GuardarVersion(snapshot, contexto as AccesoDatos),
                    usuarioId => usuarioHistorialDAL.ObtenerVersiones(usuarioId),
                    historialId => usuarioHistorialDAL.ObtenerVersion(historialId));

                _inicializado = true;
            }
        }
    }
}
