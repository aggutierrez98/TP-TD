using System;
using System.Collections.Generic;
using System.Linq;
using ABSTRACCIONES;
using BE;
using REPOSITORIO;
using SERVICIOS;

namespace BLL
{
    // Agregado personal: cada CuentaFinanciera pertenece al usuario de la sesion y solo el
    // puede verla, editarla o eliminarla. Se crea o se edita (con su entidad y los medios
    // de pago contratados de su catalogo, en una sola transaccion) o se elimina.
    // CredencialesCifradas queda fuera de alcance: Crear persiste NULL y Actualizar la conserva.
    public class CuentaFinancieraBLL
    {
        private readonly ICuentaFinancieraRepositorio _cuentaFinancieraRepositorio;
        private readonly IUnidadDeTrabajoFactory _unidadDeTrabajoFactory;
        private readonly ISesion _sesion;
        private readonly IBitacoraBLL _bitacoraBLL;

        internal CuentaFinancieraBLL(
            ICuentaFinancieraRepositorio cuentaFinancieraRepositorio,
            IUnidadDeTrabajoFactory unidadDeTrabajoFactory,
            ISesion sesion,
            IBitacoraBLL bitacoraBLL)
        {
            _cuentaFinancieraRepositorio = cuentaFinancieraRepositorio;
            _unidadDeTrabajoFactory = unidadDeTrabajoFactory;
            _sesion = sesion;
            _bitacoraBLL = bitacoraBLL;
        }

        public List<CuentaFinanciera> ObtenerMisCuentas()
        {
            int usuarioActualId = _sesion.GetUsuarioActualId().Value;
            return _cuentaFinancieraRepositorio.ObtenerPorUsuario(usuarioActualId);
        }

        // medioPagoEntidadIds: productos del catalogo de la entidad que el usuario contrata
        // (null = ninguno; los repetidos se ignoran).
        public CuentaFinanciera Crear(int entidadFinancieraId, string alias, TipoCuentaFinanciera tipo, List<int> medioPagoEntidadIds)
        {
            string aliasNormalizado = NormalizarAlias(alias);
            ValidarTipo(tipo);
            List<int> idsDistintos = (medioPagoEntidadIds ?? new List<int>()).Distinct().ToList();

            int usuarioActualId = _sesion.GetUsuarioActualId().Value;

            using (IUnidadDeTrabajo unidad = _unidadDeTrabajoFactory.Crear())
            {
                try
                {
                    ValidarEntidadExiste(unidad, entidadFinancieraId);
                    ValidarMediosDelCatalogo(unidad, entidadFinancieraId, idsDistintos);

                    CuentaFinanciera nuevaCuenta = new CuentaFinanciera
                    {
                        Usuario = new Usuario { Id = usuarioActualId },
                        EntidadFinanciera = new EntidadFinanciera { Id = entidadFinancieraId },
                        Alias = aliasNormalizado,
                        Tipo = tipo,
                        CredencialesCifradas = null
                    };
                    nuevaCuenta.Id = unidad.CuentasFinancieras.InsertarYObtenerId(nuevaCuenta);

                    if (idsDistintos.Count > 0)
                    {
                        unidad.CuentasFinancieras.ReemplazarMediosPagoEntidadDeCuenta(nuevaCuenta.Id, idsDistintos);
                    }

                    _bitacoraBLL.RegistrarOperacionInterno("FormFinancialAccountManagement.Value.Activity.Creation", "FormFinancialAccountManagement.Value.Info.Creation", "CuentaFinancieraBLL", unidad);
                    unidad.Save();
                    return nuevaCuenta;
                }
                catch (ExcepcionNegocio)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    throw new ExcepcionTecnica("Error.FinancialAccountManagement.CreateFailed", ex);
                }
            }
        }

        // Mismas reglas que Crear sobre una cuenta propia. Los medios contratados se reemplazan
        // por completo y las credenciales se conservan (el UPDATE del repositorio las sobreescribe).
        public CuentaFinanciera Actualizar(int id, int entidadFinancieraId, string alias, TipoCuentaFinanciera tipo, List<int> medioPagoEntidadIds)
        {
            string aliasNormalizado = NormalizarAlias(alias);
            ValidarTipo(tipo);
            List<int> idsDistintos = (medioPagoEntidadIds ?? new List<int>()).Distinct().ToList();

            int usuarioActualId = _sesion.GetUsuarioActualId().Value;

            using (IUnidadDeTrabajo unidad = _unidadDeTrabajoFactory.Crear())
            {
                try
                {
                    CuentaFinanciera cuentaExistente = unidad.CuentasFinancieras.ObtenerPorId(id);
                    ValidarPropiedad(cuentaExistente, usuarioActualId);
                    ValidarEntidadExiste(unidad, entidadFinancieraId);
                    ValidarMediosDelCatalogo(unidad, entidadFinancieraId, idsDistintos);

                    CuentaFinanciera cuentaActualizada = new CuentaFinanciera
                    {
                        Id = id,
                        Usuario = new Usuario { Id = usuarioActualId },
                        EntidadFinanciera = new EntidadFinanciera { Id = entidadFinancieraId },
                        Alias = aliasNormalizado,
                        Tipo = tipo,
                        CredencialesCifradas = cuentaExistente.CredencialesCifradas
                    };
                    if (!unidad.CuentasFinancieras.Actualizar(cuentaActualizada))
                    {
                        throw new ExcepcionTecnica("Error.FinancialAccountManagement.UpdateFailed");
                    }

                    unidad.CuentasFinancieras.ReemplazarMediosPagoEntidadDeCuenta(id, idsDistintos);

                    _bitacoraBLL.RegistrarOperacionInterno("FormFinancialAccountManagement.Value.Activity.Update", "FormFinancialAccountManagement.Value.Info.Update", "CuentaFinancieraBLL", unidad);
                    unidad.Save();
                    return cuentaActualizada;
                }
                catch (ExcepcionNegocio)
                {
                    throw;
                }
                catch (ExcepcionTecnica)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    throw new ExcepcionTecnica("Error.FinancialAccountManagement.UpdateFailed", ex);
                }
            }
        }

        public bool Eliminar(int id)
        {
            int usuarioActualId = _sesion.GetUsuarioActualId().Value;

            using (IUnidadDeTrabajo unidad = _unidadDeTrabajoFactory.Crear())
            {
                try
                {
                    CuentaFinanciera cuenta = unidad.CuentasFinancieras.ObtenerPorId(id);
                    ValidarPropiedad(cuenta, usuarioActualId);

                    // Primero la tabla puente (FK hacia CuentaFinanciera), luego la cuenta.
                    unidad.CuentasFinancieras.ReemplazarMediosPagoEntidadDeCuenta(id, new List<int>());
                    if (!unidad.CuentasFinancieras.Eliminar(id))
                    {
                        throw new ExcepcionTecnica("Error.FinancialAccountManagement.DeleteFailed");
                    }

                    _bitacoraBLL.RegistrarOperacionInterno("FormFinancialAccountManagement.Value.Activity.Deletion", "FormFinancialAccountManagement.Value.Info.Deletion", "CuentaFinancieraBLL", unidad);
                    unidad.Save();
                    return true;
                }
                catch (ExcepcionNegocio)
                {
                    throw;
                }
                catch (ExcepcionTecnica)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    throw new ExcepcionTecnica("Error.FinancialAccountManagement.DeleteFailed", ex);
                }
            }
        }

        private static void ValidarEntidadExiste(IUnidadDeTrabajo unidad, int entidadFinancieraId)
        {
            if (unidad.EntidadesFinancieras.ObtenerPorId(entidadFinancieraId) == null)
            {
                throw new ExcepcionNegocio("Error.FinancialAccountManagement.EntityNotFound");
            }
        }

        // Solo se pueden contratar productos del catalogo de la entidad de la cuenta.
        private static void ValidarMediosDelCatalogo(IUnidadDeTrabajo unidad, int entidadFinancieraId, List<int> medioPagoEntidadIds)
        {
            if (medioPagoEntidadIds.Count == 0)
            {
                return;
            }

            HashSet<int> idsDelCatalogo = new HashSet<int>(
                unidad.MediosPagoEntidad.ObtenerPorEntidadFinanciera(entidadFinancieraId).Select(item => item.Id));
            if (medioPagoEntidadIds.Any(medioPagoEntidadId => !idsDelCatalogo.Contains(medioPagoEntidadId)))
            {
                throw new ExcepcionNegocio("Error.FinancialAccountManagement.PaymentMethodNotFromEntity");
            }
        }

        // Una cuenta solo puede ser vista o eliminada por su dueno.
        private static void ValidarPropiedad(CuentaFinanciera cuenta, int usuarioActualId)
        {
            if (cuenta == null)
            {
                throw new ExcepcionNegocio("Error.FinancialAccountManagement.NotFound");
            }

            if (cuenta.Usuario == null || cuenta.Usuario.Id != usuarioActualId)
            {
                throw new ExcepcionNegocio("Error.FinancialAccountManagement.NotOwned");
            }
        }

        private static void ValidarTipo(TipoCuentaFinanciera tipo)
        {
            if (!Enum.IsDefined(typeof(TipoCuentaFinanciera), tipo))
            {
                throw new ExcepcionNegocio("Error.FinancialAccountManagement.InvalidType");
            }
        }

        private static string NormalizarAlias(string alias)
        {
            string aliasNormalizado = (alias ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(aliasNormalizado))
            {
                throw new ExcepcionNegocio("Error.FinancialAccountManagement.AliasRequired");
            }

            return aliasNormalizado;
        }
    }
}
