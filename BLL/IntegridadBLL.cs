using System;
using System.Collections.Generic;
using System.Linq;
using ABSTRACCIONES;
using BE;
using REPOSITORIO;
using SERVICIOS;

namespace BLL
{
    public class IntegridadBLL : IIntegridadBLL
    {
        private const string TablaUsuarios = "Usuarios";

        private readonly IIntegridadService _integridadService;
        private readonly IIntegridadRepositorio _integridadRepositorio;
        private readonly IUsuarioRepositorio _usuarioRepositorio;
        private readonly IBitacoraBLL _bitacoraBLL;
        private readonly IUnidadDeTrabajoFactory _unidadDeTrabajoFactory;

        public IntegridadBLL()
            : this(
                CompositionRoot.IntegridadService,
                new IntegridadRepositorio(),
                new UsuarioRepositorio(),
                new BitacoraBLL(),
                new UnidadDeTrabajoFactory())
        {
        }

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
            string dvv = _integridadService.CalcularDVV(listaDVH);
            _integridadRepositorio.GuardarDVV(tabla, dvv, unidad);
        }

        void IIntegridadBLL.RecalcularDVVInterno(string tabla, IEnumerable<string> listaDVH, IUnidadDeTrabajo unidad)
        {
            RecalcularDVVInterno(tabla, listaDVH, unidad);
        }

        public void ValidarIntegridadGlobal()
        {
            using (IUnidadDeTrabajo unidad = _unidadDeTrabajoFactory.Crear())
            {
                unidad.Begin();

                try
                {
                    List<Usuario> usuarios = _usuarioRepositorio.ObtenerTodos(unidad);
                    string dvvAlmacenado = _integridadRepositorio.ObtenerDVV(TablaUsuarios, unidad);

                    if (EstaSinInicializar(usuarios, dvvAlmacenado))
                    {
                        InicializarFirmas(usuarios, unidad);
                        unidad.Commit();
                        return;
                    }

                    VerificarHorizontal(usuarios);
                    VerificarVertical(usuarios, dvvAlmacenado);

                    unidad.Commit();
                }
                catch (Exception)
                {
                    unidad.Rollback();
                    throw;
                }
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
                _usuarioRepositorio.ActualizarDVH(usuario.Id, usuario.DVH, unidad);
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
                    throw new ExcepcionIntegridad(TipoFalloIntegridad.Horizontal, TablaUsuarios, usuario.Id, "Error.Integridad.Horizontal");
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
