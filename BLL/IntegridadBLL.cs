using System;
using System.Collections.Generic;
using System.Linq;
using BE;
using DAL;
using SERVICIOS;

namespace BLL
{
    // Orquesta el control de integridad de datos mediante digitos verificadores (T07).
    public class IntegridadBLL
    {
        // Nombre logico de la tabla bajo analisis (entidad mas sensible del sistema).
        private const string TablaUsuarios = "Usuarios";

        private readonly IIntegridadService _integridadService;
        private readonly IntegridadDAL _integridadDAL;
        private readonly UsuarioDAL _usuarioDAL;

        public IntegridadBLL()
        {
            BootstrapAplicacionBLL.Inicializar();
            _integridadService = new IntegridadService();
            _integridadDAL = new IntegridadDAL();
            _usuarioDAL = new UsuarioDAL();
        }

        // Calcula el DVH de una entidad integrable.
        public string CalcularDVH<T>(T entidad) where T : class
        {
            return _integridadService.CalcularDVH(entidad);
        }

        // Recalcula y persiste el DVV de una tabla a partir de los DVH actuales (en orden).
        public void RecalcularDVV(string tabla, IEnumerable<string> listaDVH, AccesoDatos accesoDatos)
        {
            string dvv = _integridadService.CalcularDVV(listaDVH);
            _integridadDAL.GuardarDVV(tabla, dvv, accesoDatos);
        }

        // Verificacion de integridad ejecutada al arrancar la aplicacion, antes del login.
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
            foreach (Usuario usuario in usuarios)
            {
                usuario.DVH = _integridadService.CalcularDVH(usuario);
                _usuarioDAL.ActualizarDVH(usuario.Id, usuario.DVH, accesoDatos);
            }

            RecalcularDVV(TablaUsuarios, usuarios.Select(usuario => usuario.DVH), accesoDatos);
            BitacoraService.GetInstance().RegistrarOperacion(
                "FormAuditLog.Value.Activity.IntegritySignaturesInitialized",
                "FormAuditLog.Value.Info.IntegritySignaturesInitialized",
                "IntegridadBLL",
                accesoDatos);
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
