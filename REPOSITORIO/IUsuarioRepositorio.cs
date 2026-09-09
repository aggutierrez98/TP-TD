using System;
using System.Collections.Generic;
using BE;

namespace REPOSITORIO
{
    public interface IUsuarioRepositorio
    {
        Usuario ObtenerPorId(int id);

        Usuario ObtenerPorUsername(string username);

        int InsertarYObtenerId(Usuario usuario);

        bool Actualizar(Usuario usuario);

        bool ActualizarDVH(int id, string dvh);

        bool ActualizarEstadoBloqueo(int id, int intentosFallidos, bool bloqueado, DateTime? bloqueadoHasta, string dvh);

        bool GuardarEstadoCompleto(Usuario usuario);

        bool EliminarFisicamente(int id);

        List<Usuario> ObtenerTodos();
    }
}
