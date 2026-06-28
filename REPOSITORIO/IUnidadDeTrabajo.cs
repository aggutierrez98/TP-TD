using System;

namespace REPOSITORIO
{
    public interface IUnidadDeTrabajo : IDisposable
    {
        IBitacoraRepositorio Bitacora { get; }

        IIdiomaRepositorio Idiomas { get; }

        IIntegridadRepositorio Integridad { get; }

        IPermisoRepositorio Permisos { get; }

        ITraduccionRepositorio Traducciones { get; }

        IUsuarioHistorialRepositorio UsuarioHistorial { get; }

        IUsuarioPermisoRepositorio UsuarioPermisos { get; }

        IUsuarioPreferenciaRepositorio UsuarioPreferencias { get; }

        IUsuarioRepositorio Usuarios { get; }

        void Save();
    }
}
