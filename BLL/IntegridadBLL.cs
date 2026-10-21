using System;
using System.Collections.Generic;
using System.Linq;
using ABSTRACCIONES;
using BE;
using REPOSITORIO;
using SERVICIOS;

namespace BLL
{
    // Orquesta el control de integridad por digitos verificadores (T07).
    // La verificacion global corre al arranque de la aplicacion; las escrituras
    // de usuarios mantienen el DVV al dia via ActualizarDVVInterno dentro de la
    // misma unidad de trabajo que la operacion de negocio.
    public class IntegridadBLL : IIntegridadBLL
    {
        private const string TablaUsuarios = "Usuarios";

        private readonly IIntegridadService _integridadService;
        private readonly IIntegridadRepositorio _integridadRepositorio;
        private readonly IUsuarioRepositorio _usuarioRepositorio;
        private readonly IBitacoraBLL _bitacoraBLL;
        private readonly IUnidadDeTrabajoFactory _unidadDeTrabajoFactory;

        internal IntegridadBLL(
            IIntegridadService integridadService,
            IIntegridadRepositorio integridadRepositorio,
            IUsuarioRepositorio usuarioRepositorio,
            IBitacoraBLL bitacoraBLL,
            IUnidadDeTrabajoFactory unidadDeTrabajoFactory)
        {
            _integridadService = integridadService;
            _integridadRepositorio = integridadRepositorio;
            _usuarioRepositorio = usuarioRepositorio;
            _bitacoraBLL = bitacoraBLL;
            _unidadDeTrabajoFactory = unidadDeTrabajoFactory;
        }

        public string CalcularDVH<T>(T entidad) where T : class
        {
            return _integridadService.CalcularDVH(entidad);
        }

        internal void RecalcularDVVInterno(string tabla, IEnumerable<string> listaDVH, IUnidadDeTrabajo unidad)
        {
            if (unidad == null)
            {
                throw new ArgumentNullException("unidad");
            }

            // Camino completo: se usa al inicializar firmas o al validar toda la
            // tabla. Aca si se dispone de todos los DVH y se reconstruye el DVV
            // esperado desde cero.
            string dvv = _integridadService.CalcularDVV(listaDVH);
            unidad.Integridad.GuardarDVV(tabla, dvv);
        }

        internal void ActualizarDVVInterno(string tabla, string dvhAnterior, string dvhNuevo, IUnidadDeTrabajo unidad)
        {
            if (unidad == null)
            {
                throw new ArgumentNullException("unidad");
            }

            // Camino incremental: las escrituras de usuario no vuelven a leer la
            // tabla completa. Parten del DVV persistido y aplican solo la
            // diferencia entre el DVH anterior y el nuevo dentro de la misma UoW.
            string dvvActual = unidad.Integridad.ObtenerDVV(tabla);
            string dvv = _integridadService.CalcularDVVIncremental(dvvActual, dvhAnterior, dvhNuevo);
            unidad.Integridad.GuardarDVV(tabla, dvv);
        }

        void IIntegridadBLL.RecalcularDVVInterno(string tabla, IEnumerable<string> listaDVH, IUnidadDeTrabajo unidad)
        {
            RecalcularDVVInterno(tabla, listaDVH, unidad);
        }

        void IIntegridadBLL.ActualizarDVVInterno(string tabla, string dvhAnterior, string dvhNuevo, IUnidadDeTrabajo unidad)
        {
            ActualizarDVVInterno(tabla, dvhAnterior, dvhNuevo, unidad);
        }

        public void ValidarIntegridadGlobal()
        {
            using (IUnidadDeTrabajo unidad = _unidadDeTrabajoFactory.Crear())
            {
                List<Usuario> usuarios = unidad.Usuarios.ObtenerTodos();
                string dvvAlmacenado = unidad.Integridad.ObtenerDVV(TablaUsuarios);

                if (EstaSinInicializar(usuarios, dvvAlmacenado))
                {
                    InicializarFirmas(usuarios, unidad);
                    unidad.Save();
                    return;
                }

                VerificarHorizontal(usuarios);
                VerificarVertical(usuarios, dvvAlmacenado);

                unidad.Save();
            }
        }

        private static bool EstaSinInicializar(IEnumerable<Usuario> usuarios, string dvvAlmacenado)
        {
            return dvvAlmacenado == null && usuarios.All(usuario => string.IsNullOrEmpty(usuario.DVH));
        }

        private void InicializarFirmas(List<Usuario> usuarios, IUnidadDeTrabajo unidad)
        {
            foreach (Usuario usuario in usuarios)
            {
                usuario.DVH = _integridadService.CalcularDVH(usuario);
                unidad.Usuarios.ActualizarDVH(usuario, usuario.DVH);
            }

            RecalcularDVVInterno(TablaUsuarios, usuarios.Select(usuario => usuario.DVH), unidad);
            _bitacoraBLL.RegistrarOperacionInterno(
                "FormAuditLog.Value.Activity.IntegritySignaturesInitialized",
                "FormAuditLog.Value.Info.IntegritySignaturesInitialized",
                "IntegridadBLL",
                unidad);
        }

        private void VerificarHorizontal(IEnumerable<Usuario> usuarios)
        {
            foreach (Usuario usuario in usuarios)
            {
                string dvhCalculado = _integridadService.CalcularDVH(usuario);
                if (!string.Equals(dvhCalculado, usuario.DVH, StringComparison.Ordinal))
                {
                    _bitacoraBLL.RegistrarFalloNegocio(
                        "FormAuditLog.Value.Activity.IntegrityCheckFailed",
                        "FormAuditLog.Value.Info.IntegrityHorizontalCorruption",
                        "IntegridadBLL");
                    throw new ExcepcionIntegridad(TipoFalloIntegridad.Horizontal, TablaUsuarios, usuario, "Error.Integridad.Horizontal");
                }
            }
        }

        private void VerificarVertical(List<Usuario> usuarios, string dvvAlmacenado)
        {
            string dvvCalculado = _integridadService.CalcularDVV(usuarios.Select(usuario => usuario.DVH));
            if (!string.Equals(dvvCalculado, dvvAlmacenado, StringComparison.Ordinal))
            {
                _bitacoraBLL.RegistrarFalloNegocio(
                    "FormAuditLog.Value.Activity.IntegrityCheckFailed",
                    "FormAuditLog.Value.Info.IntegrityVerticalCorruption",
                    "IntegridadBLL");
                throw new ExcepcionIntegridad(TipoFalloIntegridad.Vertical, TablaUsuarios, null, "Error.Integridad.Vertical");
            }
        }
    }
}
