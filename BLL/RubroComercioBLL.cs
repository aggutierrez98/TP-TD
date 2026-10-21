using System;
using System.Collections.Generic;
using BE;
using REPOSITORIO;
using SERVICIOS;

namespace BLL
{
    // Pantalla de operador (back-office): sin validacion de "dueno", Comercio/RubroComercio
    // no tienen Usuario asociado.
    public class RubroComercioBLL
    {
        private readonly IRubroComercioRepositorio _rubroComercioRepositorio;
        private readonly IUnidadDeTrabajoFactory _unidadDeTrabajoFactory;
        private readonly IBitacoraBLL _bitacoraBLL;

        internal RubroComercioBLL(
            IRubroComercioRepositorio rubroComercioRepositorio,
            IUnidadDeTrabajoFactory unidadDeTrabajoFactory,
            IBitacoraBLL bitacoraBLL)
        {
            _rubroComercioRepositorio = rubroComercioRepositorio;
            _unidadDeTrabajoFactory = unidadDeTrabajoFactory;
            _bitacoraBLL = bitacoraBLL;
        }

        public List<RubroComercio> ObtenerTodas()
        {
            return _rubroComercioRepositorio.ObtenerTodas();
        }

        // Devuelve la misma instancia recibida, con el nombre normalizado y el Id cargado.
        public RubroComercio Crear(RubroComercio rubro)
        {
            string nombreNormalizado = NormalizarNombre(rubro?.Nombre);

            using (IUnidadDeTrabajo unidad = _unidadDeTrabajoFactory.Crear())
            {
                try
                {
                    rubro.Nombre = nombreNormalizado;
                    rubro.Id = unidad.RubrosComercio.InsertarYObtenerId(rubro);

                    _bitacoraBLL.RegistrarOperacionInterno("FormMerchantManagement.Value.Activity.RubroCreation", "FormMerchantManagement.Value.Info.RubroCreation", "RubroComercioBLL", unidad);
                    unidad.Save();
                    return rubro;
                }
                catch (ExcepcionNegocio)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    throw new ExcepcionTecnica("Error.MerchantManagement.RubroCreateFailed", ex);
                }
            }
        }

        // Recibe el rubro con el nombre nuevo y lo copia sobre el persistido.
        public bool Actualizar(RubroComercio rubro)
        {
            string nombreNormalizado = NormalizarNombre(rubro?.Nombre);

            using (IUnidadDeTrabajo unidad = _unidadDeTrabajoFactory.Crear())
            {
                try
                {
                    RubroComercio persistido = unidad.RubrosComercio.ObtenerPorId(rubro.Id);
                    if (persistido == null)
                    {
                        throw new ExcepcionNegocio("Error.MerchantManagement.RubroNotFound");
                    }

                    persistido.Nombre = nombreNormalizado;
                    bool actualizado = unidad.RubrosComercio.Actualizar(persistido);
                    if (!actualizado)
                    {
                        throw new ExcepcionTecnica("Error.MerchantManagement.RubroUpdateFailed");
                    }

                    _bitacoraBLL.RegistrarOperacionInterno("FormMerchantManagement.Value.Activity.RubroUpdate", "FormMerchantManagement.Value.Info.RubroUpdate", "RubroComercioBLL", unidad);
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
                    throw new ExcepcionTecnica("Error.MerchantManagement.RubroUpdateFailed", ex);
                }
            }
        }

        public bool Eliminar(RubroComercio rubro)
        {
            using (IUnidadDeTrabajo unidad = _unidadDeTrabajoFactory.Crear())
            {
                try
                {
                    RubroComercio persistido = rubro == null ? null : unidad.RubrosComercio.ObtenerPorId(rubro.Id);
                    if (persistido == null)
                    {
                        throw new ExcepcionNegocio("Error.MerchantManagement.RubroNotFound");
                    }

                    // Validacion proactiva: un rubro con comercios o referenciado por una
                    // promocion no se da de baja.
                    if (unidad.Comercios.ObtenerPorRubro(persistido).Count > 0
                        || unidad.PromocionRubrosComercio.ContarPorRubroComercio(persistido) > 0)
                    {
                        throw new ExcepcionNegocio("Error.MerchantManagement.RubroInUse");
                    }

                    bool eliminado = unidad.RubrosComercio.Eliminar(persistido);
                    if (!eliminado)
                    {
                        throw new ExcepcionTecnica("Error.MerchantManagement.RubroDeleteFailed");
                    }

                    _bitacoraBLL.RegistrarOperacionInterno("FormMerchantManagement.Value.Activity.RubroDeletion", "FormMerchantManagement.Value.Info.RubroDeletion", "RubroComercioBLL", unidad);
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
                    throw new ExcepcionTecnica("Error.MerchantManagement.RubroDeleteFailed", ex);
                }
            }
        }

        private static string NormalizarNombre(string nombre)
        {
            string nombreNormalizado = (nombre ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(nombreNormalizado))
            {
                throw new ExcepcionNegocio("Error.MerchantManagement.RubroNameRequired");
            }

            return nombreNormalizado;
        }
    }
}
