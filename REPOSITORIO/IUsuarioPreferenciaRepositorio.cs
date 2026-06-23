using SERVICIOS;

namespace REPOSITORIO
{
    public interface IUsuarioPreferenciaRepositorio
    {
        UsuarioPreferencia ObtenerPorUsuarioId(int usuarioId, IUnidadDeTrabajo unidad = null);

        bool Guardar(UsuarioPreferencia preferencia, IUnidadDeTrabajo unidad = null);

        bool Guardar(int usuarioId, int? idiomaId, IUnidadDeTrabajo unidad = null);
    }
}
