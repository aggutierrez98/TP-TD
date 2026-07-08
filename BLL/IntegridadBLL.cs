using System;
using System.Collections.Generic;
using System.Linq;
using BE;
using DAL;
using SERVICIOS;

namespace BLL
{
    // Orquesta el control de integridad por digitos verificadores (T07).
    // La verificacion global corre al arranque de la aplicacion; las escrituras
    // de usuarios mantienen el DVV al dia via ActualizarDVV dentro de la misma
    // transaccion que la operacion de negocio.
    public class IntegridadBLL
    {
        // Nombre logico de la tabla bajo analisis (entidad mas sensible del sistema).
        private const string TablaUsuarios = "Usuarios";

        private readonly IntegridadService _integridadService;
        private readonly IntegridadDAL _integridadDAL;
        private readonly UsuarioDAL _usuarioDAL;

        public IntegridadBLL()
        {
            _integridadService = new IntegridadService();
            _integridadDAL = new IntegridadDAL();
            _usuarioDAL = new UsuarioDAL();
        }

        // Calcula el DVH de una entidad integrable.
        public string CalcularDVH<T>(T entidad) where T : class
        {
            return _integridadService.CalcularDVH(entidad);
        }

        // Camino incremental: las escrituras de usuario no vuelven a leer la
        // tabla completa. Parten del DVV persistido y aplican solo la diferencia
        // entre el DVH anterior y el nuevo, dentro de la transaccion en curso.
        public void ActualizarDVV(string tabla, string dvhAnterior, string dvhNuevo, AccesoDatos accesoDatos)
        {
            if (accesoDatos == null)
        {
                throw new ArgumentNullException("accesoDatos");
            }

            string dvvActual = _integridadDAL.ObtenerDVV(tabla, accesoDatos);
            string dvv = _integridadService.CalcularDVVIncremental(dvvActual, dvhAnterior, dvhNuevo);
            _integridadDAL.GuardarDVV(tabla, dvv, accesoDatos);
        }

        // Verificacion global al arranque: si la tabla nunca fue firmada se
        // inicializan las firmas; si ya lo fue, cualquier mismatch bloquea el
        // acceso lanzando ExcepcionIntegridad.
        public void ValidarIntegridadGlobal()
        {
            using (AccesoDatos accesoDatos = new AccesoDatos())
            {
                accesoDatos.BeginTransactionScope();

                try
                {
                    // Las filas se leen ordenadas por Id para que el DVV sea deterministico
                    // y consistente con el calculo de las operaciones de escritura.
                    List<Usuario> usuarios = _usuarioDAL.ObtenerTodos(accesoDatos);
                    string dvvAlmacenado = _integridadDAL.ObtenerDVV(TablaUsuarios, accesoDatos);

                    if (EstaSinInicializar(usuarios, dvvAlmacenado))
                    {
                        InicializarFirmas(usuarios, accesoDatos);
                        accesoDatos.CommitTransactionScope();
                        return;
                    }

                    VerificarHorizontal(usuarios);
                    VerificarVertical(usuarios, dvvAlmacenado);

                    accesoDatos.CommitTransactionScope();
                }
                catch (Exception)
                {
                    accesoDatos.RollbackTransactionScope();
                    throw;
                }
            }
        }

        // La tabla nunca fue firmada: no existe DVV y ninguna fila tiene DVH.
        private static bool EstaSinInicializar(IEnumerable<Usuario> usuarios, string dvvAlmacenado)
        {
            return dvvAlmacenado == null && usuarios.All(usuario => string.IsNullOrEmpty(usuario.DVH));
        }

        private void InicializarFirmas(List<Usuario> usuarios, AccesoDatos accesoDatos)
        {
            accesoDatos.BeginTransactionScope();

            try
            {
            foreach (Usuario usuario in usuarios)
            {
                usuario.DVH = _integridadService.CalcularDVH(usuario);
                _usuarioDAL.ActualizarDVH(usuario.Id, usuario.DVH, accesoDatos);
            }

                string dvv = _integridadService.CalcularDVV(usuarios.Select(usuario => usuario.DVH));
                _integridadDAL.GuardarDVV(TablaUsuarios, dvv, accesoDatos);

            BitacoraService.GetInstance().RegistrarOperacion(
                "FormAuditLog.Value.Activity.IntegritySignaturesInitialized",
                "FormAuditLog.Value.Info.IntegritySignaturesInitialized",
                "IntegridadBLL",
                accesoDatos);

                accesoDatos.CommitTransactionScope();
        }
            catch
            {
                accesoDatos.RollbackTransactionScope();
                throw;
            }
        }

        private void VerificarHorizontal(IEnumerable<Usuario> usuarios)
        {
            foreach (Usuario usuario in usuarios)
            {
                string dvhCalculado = _integridadService.CalcularDVH(usuario);
                if (!string.Equals(dvhCalculado, usuario.DVH, StringComparison.Ordinal))
                {
                    // El fallo se registra sin contexto transaccional para que persista
                    // aunque la verificacion haga rollback.
                    BitacoraService.GetInstance().RegistrarFalloNegocio(
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
                BitacoraService.GetInstance().RegistrarFalloNegocio(
                    "FormAuditLog.Value.Activity.IntegrityCheckFailed",
                    "FormAuditLog.Value.Info.IntegrityVerticalCorruption",
                    "IntegridadBLL");
                throw new ExcepcionIntegridad(TipoFalloIntegridad.Vertical, TablaUsuarios, null, "Error.Integridad.Vertical");
            }
        }
    }
}
