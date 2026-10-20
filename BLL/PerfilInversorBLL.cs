using System;
using System.Collections.Generic;
using ABSTRACCIONES;
using BE;
using REPOSITORIO;
using SERVICIOS;

namespace BLL
{
    public sealed class PerfilInversorBLL
    {
        public const int VersionCuestionario = CalculadorPerfilInversor.Version;

        public int ObtenerCantidadOpciones(int preguntaId)
        {
            if (preguntaId < 1 || preguntaId > 10) throw new ArgumentOutOfRangeException(nameof(preguntaId));
            return CalculadorPerfilInversor.CantidadesOpciones[preguntaId - 1];
        }

        public PerfilInversor EvaluarCuestionario(int version, IEnumerable<RespuestaPerfilInversor> respuestas) =>
            CalculadorPerfilInversor.Evaluar(version, respuestas);

        private readonly IPerfilInversorRepositorio _repositorio;
        private readonly IUnidadDeTrabajoFactory _unidades;
        private readonly ISesion _sesion;
        private readonly IBitacoraBLL _bitacora;
        private readonly Func<DateTime> _ahoraUtc;
        internal PerfilInversorBLL(IPerfilInversorRepositorio repositorio, IUnidadDeTrabajoFactory unidades,
            ISesion sesion, IBitacoraBLL bitacora, Func<DateTime> ahoraUtc)
        {
            _repositorio = repositorio;
            _unidades = unidades;
            _sesion = sesion;
            _bitacora = bitacora;
            _ahoraUtc = ahoraUtc;
        }

        public PerfilInversor ObtenerMiPerfil()
        {
            int propietario = ObtenerPropietarioSesion();
            try { return _repositorio.ObtenerPorUsuario(propietario); }
            catch (Exception ex) { throw new ExcepcionTecnica("Error.InvestorProfile.ReadFailed", ex); }
        }

        public PerfilInversor GuardarMiPerfil(int version, IEnumerable<RespuestaPerfilInversor> respuestas)
        {
            int propietario = ObtenerPropietarioSesion();
            return Guardar(propietario, version, respuestas);
        }

        public PerfilInversor GuardarPostRegistro(ContinuacionPerfilInversor continuacion, int version, IEnumerable<RespuestaPerfilInversor> respuestas)
        {
            if (continuacion == null) throw new ExcepcionNegocio("Error.InvestorProfile.ContinuationExpired");
            lock (continuacion.Sincronizacion)
            {
                if (continuacion.Finalizada || _ahoraUtc() >= continuacion.CreadaUtc.AddMinutes(30))
                    throw new ExcepcionNegocio("Error.InvestorProfile.ContinuationExpired");
                var perfil = Guardar(continuacion.UsuarioId, version, respuestas);
                continuacion.Finalizada = true;
                return perfil;
            }
        }

        public void CancelarPostRegistro(ContinuacionPerfilInversor continuacion)
        {
            if (continuacion == null) return;
            lock (continuacion.Sincronizacion) continuacion.Finalizada = true;
        }

        private PerfilInversor Guardar(int propietario, int version, IEnumerable<RespuestaPerfilInversor> respuestas)
        {
            var perfil = CalculadorPerfilInversor.Evaluar(version, respuestas);
            perfil.UsuarioId = propietario;
            perfil.FechaEvaluacionUtc = _ahoraUtc();
            try
            {
                using (var unidad = _unidades.Crear())
                {
                    unidad.PerfilesInversor.Guardar(perfil);
                    _bitacora.RegistrarOperacionInterno("FormInvestorProfile.Value.Activity.Saved",
                        "FormInvestorProfile.Value.Info.Saved", nameof(PerfilInversorBLL), unidad);
                    unidad.Save();
                }
                return perfil;
            }
            catch (Exception ex) { throw new ExcepcionTecnica("Error.InvestorProfile.SaveFailed", ex); }
        }

        private int ObtenerPropietarioSesion()
        {
            int? propietario = _sesion.GetUsuarioActualId();
            if (!propietario.HasValue || propietario.Value <= 0)
                throw new ExcepcionNegocio("Error.InvestorProfile.NoSession");
            return propietario.Value;
        }
    }
}
