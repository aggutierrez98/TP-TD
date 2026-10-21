using System.Collections.Generic;
using ABSTRACCIONES;
using BE;
using REPOSITORIO;
using SERVICIOS;

namespace BLL
{
    internal interface IRecomendacionInversionesBLL
    {
        System.Threading.Tasks.Task<ResumenInversiones> CalcularAsync(Usuario usuario, decimal? porcentaje, System.Threading.CancellationToken cancellationToken, PerfilInversor perfil);

        // Solo el calculo economico local (sin fuentes externas): nivel, saldo y, si hay porcentaje, monto.
        ResumenInversiones CalcularSaldo(Usuario usuario, decimal? porcentaje);
    }

    internal interface IRecomendacionComprasBLL
    {
        System.Threading.Tasks.Task<ResumenCompras> CalcularAsync(Usuario usuario, string nombreLista, System.Threading.CancellationToken cancellationToken);
        IReadOnlyList<IReferenciaAsistente> ObtenerUniversoReferencias(Usuario usuario);
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
        void AplicarIdiomaSesionUsuario(Usuario usuario);

        Idioma ObtenerIdiomaParaUsuario(Usuario usuario);
    }

    internal interface IPermisoBLL
    {
        List<IPermiso> ObtenerPermisosDirectosUsuario(Usuario usuario);

        void AsignarPerfilBasicoInterno(Usuario usuario, IUnidadDeTrabajo unidad);

        void GuardarPermisosUsuarioInterno(Usuario usuario, List<IPermiso> permisos, IUnidadDeTrabajo unidad);

        void ValidarPuedeEliminarUsuario(Usuario usuario, IUnidadDeTrabajo unidad);
    }

    internal interface IUsuarioEstadoBLL
    {
        UsuarioHistorialSnapshot GuardarVersionActual(Usuario usuario, string accion, string observaciones);

        UsuarioHistorialSnapshot GuardarVersionActualInterno(Usuario usuario, string accion, string observaciones, IUnidadDeTrabajo unidad);

        List<UsuarioHistorialSnapshot> ObtenerHistorial(Usuario usuario);

        UsuarioHistorialSnapshot ObtenerVersion(IUsuarioSnapshot snapshot);

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
