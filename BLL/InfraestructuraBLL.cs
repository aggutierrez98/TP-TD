using System.Collections.Generic;
using DAL;
using SERVICIOS;

namespace BLL
{
    public static class InfraestructuraBLL
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

                InicializadorEsquemaBLL.AsegurarEsquema();

                BitacoraDAL bitacoraDAL = new BitacoraDAL();
                UsuarioHistorialDAL usuarioHistorialDAL = new UsuarioHistorialDAL();

                Sesion.GetInstance().ConfigurarBitacora(
                    (evento, contexto) => bitacoraDAL.Guardar(evento, contexto as AccesoDatos),
                    filtro => bitacoraDAL.Buscar(filtro));

                HistorialEstadosService.GetInstance().Configurar(
                    (snapshot, contexto) => usuarioHistorialDAL.GuardarVersion(snapshot, contexto as AccesoDatos),
                    usuarioId => usuarioHistorialDAL.ObtenerVersiones(usuarioId),
                    historialId => usuarioHistorialDAL.ObtenerVersion(historialId));

                _inicializado = true;
            }
        }

    }
}
