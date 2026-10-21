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

        public IReadOnlyList<PreguntaPerfilInversor> ObtenerPreguntas() => CalculadorPerfilInversor.Preguntas;

        public int ObtenerCantidadOpciones(PreguntaPerfilInversor pregunta)
        {
            if (pregunta == null || pregunta.Id < 1 || pregunta.Id > 10) throw new ArgumentOutOfRangeException(nameof(pregunta));
            return CalculadorPerfilInversor.CantidadesOpciones[pregunta.Id - 1];
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
            Usuario propietario = ObtenerPropietarioSesion();
            try { return _repositorio.ObtenerPorUsuario(propietario); }
            catch (Exception ex) { throw new ExcepcionTecnica("Error.InvestorProfile.ReadFailed", ex); }
        }

        public PerfilInversor GuardarMiPerfil(int version, IEnumerable<RespuestaPerfilInversor> respuestas)
        {
            Usuario propietario = ObtenerPropietarioSesion();
            return Guardar(propietario, version, respuestas);
        }

        public PerfilInversor GuardarPostRegistro(ContinuacionPerfilInversor continuacion, int version, IEnumerable<RespuestaPerfilInversor> respuestas)
        {
            if (continuacion == null) throw new ExcepcionNegocio("Error.InvestorProfile.ContinuationExpired");
            lock (continuacion.Sincronizacion)
            {
                if (continuacion.Finalizada || _ahoraUtc() >= continuacion.CreadaUtc.AddMinutes(30))
                    throw new ExcepcionNegocio("Error.InvestorProfile.ContinuationExpired");
                var perfil = Guardar(continuacion.Usuario, version, respuestas);
                continuacion.Finalizada = true;
                return perfil;
            }
        }

        public void CancelarPostRegistro(ContinuacionPerfilInversor continuacion)
        {
            if (continuacion == null) return;
            lock (continuacion.Sincronizacion) continuacion.Finalizada = true;
        }

        private PerfilInversor Guardar(Usuario propietario, int version, IEnumerable<RespuestaPerfilInversor> respuestas)
        {
            var perfil = CalculadorPerfilInversor.Evaluar(version, respuestas);
            perfil.Usuario = propietario;
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

        private Usuario ObtenerPropietarioSesion()
        {
            if (!_sesion.HaySesionActiva() || _sesion.ObtenerUsuarioActual().Id <= 0)
                throw new ExcepcionNegocio("Error.InvestorProfile.NoSession");
            return _sesion.ObtenerUsuarioActual();
        }
    }
}
