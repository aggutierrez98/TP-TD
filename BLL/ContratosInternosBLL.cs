using System.Collections.Generic;
using ABSTRACCIONES;
using BE;
using REPOSITORIO;
using SERVICIOS;

namespace BLL
{
    internal interface IRecomendacionInversionesBLL
    {
        System.Threading.Tasks.Task<ResumenInversiones> CalcularAsync(int usuarioActualId, decimal? porcentaje, System.Threading.CancellationToken cancellationToken, PerfilInversor perfil);

        // Solo el calculo economico local (sin fuentes externas): nivel, saldo y, si hay porcentaje, monto.
        ResumenInversiones CalcularSaldo(int usuarioActualId, decimal? porcentaje);
    }

    internal interface IRecomendacionComprasBLL
    {
        System.Threading.Tasks.Task<ResumenCompras> CalcularAsync(int usuarioActualId, string nombreLista, System.Threading.CancellationToken cancellationToken);
        IReadOnlyList<ReferenciaAsistente> ObtenerUniversoReferencias(int usuarioActualId);
    }

    internal interface IBitacoraBLL
    {
        List<EventoBitacora> Buscar(FiltroBitacora filtro);

        void RegistrarOperacion(string actividad, string informacion, string origen);

        void RegistrarFalloNegocio(string actividad, string informacion, string origen);

        void RegistrarOperacionInterno(string actividad, string informacion, string origen, IUnidadDeTrabajo unidad);

        void RegistrarFalloNegocioInterno(string actividad, string informacion, string origen, IUnidadDeTrabajo unidad);
    }

    internal interface IIdiomaBLL
    {
        void AplicarIdiomaSesionUsuario(int usuarioId);

        Idioma ObtenerIdiomaParaUsuario(int usuarioId);
    }

    internal interface IPermisoBLL
    {
        List<IPermiso> ObtenerPermisosDirectosUsuario(int usuarioId);

        void AsignarPerfilBasicoInterno(int usuarioId, IUnidadDeTrabajo unidad);

        void GuardarPermisosUsuarioInterno(int usuarioId, List<int> permisosId, IUnidadDeTrabajo unidad);

        void ValidarPuedeEliminarUsuario(int usuarioId, IUnidadDeTrabajo unidad);
    }

    internal interface IUsuarioEstadoBLL
    {
        int GuardarVersionActual(int usuarioId, string accion, string observaciones);

        int GuardarVersionActualInterno(int usuarioId, string accion, string observaciones, IUnidadDeTrabajo unidad);

        List<UsuarioHistorialSnapshot> ObtenerHistorial(int usuarioId);

        UsuarioHistorialSnapshot ObtenerVersion(int historialId);

        List<ResultadoComparacionEstado> CompararVersiones(UsuarioHistorialSnapshot versionActual, UsuarioHistorialSnapshot versionAnterior);

        void AplicarSnapshotInterno(UsuarioHistorialSnapshot snapshot, IUnidadDeTrabajo unidad);
    }

    internal interface IIntegridadBLL
    {
        string CalcularDVH<T>(T entidad) where T : class;

        void RecalcularDVVInterno(string tabla, IEnumerable<string> listaDVH, IUnidadDeTrabajo unidad);

        void ActualizarDVVInterno(string tabla, string dvhAnterior, string dvhNuevo, IUnidadDeTrabajo unidad);

        void ValidarIntegridadGlobal();
    }

    internal interface IPasswordHashService
    {
        string HashPassword(string passwordRaw);

        bool VerifyPassword(string passwordRaw, string passwordHash);
    }
}
