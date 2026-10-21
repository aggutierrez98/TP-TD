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
            return _cuentaFinancieraRepositorio.ObtenerPorUsuario(_sesion.ObtenerUsuarioActual());
        }

        // La cuenta conserva los productos contratados (null = ninguno; repetidos se ignoran).
        public CuentaFinanciera Crear(CuentaFinanciera cuenta)
        {
            string aliasNormalizado = NormalizarAlias(cuenta?.Alias);
            ValidarTipo(cuenta.Tipo);

            Usuario usuarioActual = _sesion.ObtenerUsuarioActual();

            using (IUnidadDeTrabajo unidad = _unidadDeTrabajoFactory.Crear())
            {
                try
                {
                    EntidadFinanciera entidad = ValidarEntidadExiste(unidad, cuenta.EntidadFinanciera);
                    List<MedioPagoEntidad> medios = ValidarMediosDelCatalogo(unidad, entidad, cuenta.MediosPagoEntidad);
                    cuenta.Usuario = usuarioActual;
                    cuenta.EntidadFinanciera = entidad;
                    cuenta.Alias = aliasNormalizado;
                    cuenta.CredencialesCifradas = null;
                    cuenta.MediosPagoEntidad = medios;
                    cuenta.Id = unidad.CuentasFinancieras.InsertarYObtenerId(cuenta);

                    if (medios.Count > 0)
                    {
                        unidad.CuentasFinancieras.ReemplazarMediosPagoEntidadDeCuenta(cuenta, medios);
                    }

                    _bitacoraBLL.RegistrarOperacionInterno("FormFinancialAccountManagement.Value.Activity.Creation", "FormFinancialAccountManagement.Value.Info.Creation", "CuentaFinancieraBLL", unidad);
                    unidad.Save();
                    return cuenta;
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
        public CuentaFinanciera Actualizar(CuentaFinanciera cuenta)
        {
            string aliasNormalizado = NormalizarAlias(cuenta?.Alias);
            ValidarTipo(cuenta.Tipo);

            Usuario usuarioActual = _sesion.ObtenerUsuarioActual();

            using (IUnidadDeTrabajo unidad = _unidadDeTrabajoFactory.Crear())
            {
                try
                {
                    CuentaFinanciera cuentaActualizada = unidad.CuentasFinancieras.ObtenerPorId(cuenta.Id);
                    ValidarPropiedad(cuentaActualizada, usuarioActual);
                    EntidadFinanciera entidad = ValidarEntidadExiste(unidad, cuenta.EntidadFinanciera);
                    List<MedioPagoEntidad> medios = ValidarMediosDelCatalogo(unidad, entidad, cuenta.MediosPagoEntidad);
                    cuentaActualizada.EntidadFinanciera = entidad;
                    cuentaActualizada.Alias = aliasNormalizado;
                    cuentaActualizada.Tipo = cuenta.Tipo;
                    cuentaActualizada.MediosPagoEntidad = medios;
                    if (!unidad.CuentasFinancieras.Actualizar(cuentaActualizada))
                    {
                        throw new ExcepcionTecnica("Error.FinancialAccountManagement.UpdateFailed");
                    }

                    unidad.CuentasFinancieras.ReemplazarMediosPagoEntidadDeCuenta(cuentaActualizada, medios);

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

        public bool Eliminar(CuentaFinanciera cuenta)
        {
            Usuario usuarioActual = _sesion.ObtenerUsuarioActual();

            using (IUnidadDeTrabajo unidad = _unidadDeTrabajoFactory.Crear())
            {
                try
                {
                    CuentaFinanciera persistida = cuenta == null ? null : unidad.CuentasFinancieras.ObtenerPorId(cuenta.Id);
                    ValidarPropiedad(persistida, usuarioActual);

                    // Primero la tabla puente (FK hacia CuentaFinanciera), luego la cuenta.
                    unidad.CuentasFinancieras.ReemplazarMediosPagoEntidadDeCuenta(persistida, new List<MedioPagoEntidad>());
                    if (!unidad.CuentasFinancieras.Eliminar(persistida))
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

        private static EntidadFinanciera ValidarEntidadExiste(IUnidadDeTrabajo unidad, EntidadFinanciera recibida)
        {
            EntidadFinanciera entidad = recibida == null ? null : unidad.EntidadesFinancieras.ObtenerPorId(recibida.Id);
            if (entidad == null)
            {
                throw new ExcepcionNegocio("Error.FinancialAccountManagement.EntityNotFound");
            }
            return entidad;
        }

        // Solo se pueden contratar productos del catalogo de la entidad de la cuenta.
        private static List<MedioPagoEntidad> ValidarMediosDelCatalogo(IUnidadDeTrabajo unidad, EntidadFinanciera entidad, IEnumerable<MedioPagoEntidad> medios)
        {
            List<MedioPagoEntidad> seleccionados = (medios ?? Enumerable.Empty<MedioPagoEntidad>()).ToList();
            if (seleccionados.Count == 0) return seleccionados;
            List<MedioPagoEntidad> catalogo = unidad.MediosPagoEntidad.ObtenerPorEntidadFinanciera(entidad);
            if (seleccionados.Any(medio => medio == null || !catalogo.Any(item => item.Id == medio.Id)))
            {
                throw new ExcepcionNegocio("Error.FinancialAccountManagement.PaymentMethodNotFromEntity");
            }
            return seleccionados.GroupBy(item => item.Id).Select(grupo => catalogo.First(item => item.Id == grupo.Key)).ToList();
        }

        // Una cuenta solo puede ser vista o eliminada por su dueno.
        private static void ValidarPropiedad(CuentaFinanciera cuenta, Usuario usuarioActual)
        {
            if (cuenta == null)
            {
                throw new ExcepcionNegocio("Error.FinancialAccountManagement.NotFound");
            }

            if (cuenta.Usuario == null || cuenta.Usuario.Id != usuarioActual.Id)
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
