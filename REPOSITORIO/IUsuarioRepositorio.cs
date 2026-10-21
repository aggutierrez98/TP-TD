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

        bool ActualizarDVH(Usuario usuario, string dvh);

        bool ActualizarEstadoBloqueo(Usuario usuario, int intentosFallidos, bool bloqueado, DateTime? bloqueadoHasta, string dvh);

        bool GuardarEstadoCompleto(Usuario usuario);

        bool EliminarFisicamente(Usuario usuario);

        List<Usuario> ObtenerTodos();

        // Pagina de Ids de cuentas activas con Id mayor a ultimoId, en orden ascendente (paginacion por clave).
        List<Usuario> ObtenerActivosDespuesDe(Usuario ultimo, int cantidad);
    }
}
