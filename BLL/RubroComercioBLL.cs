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

        public RubroComercio Crear(string nombre)
        {
            string nombreNormalizado = NormalizarNombre(nombre);

            using (IUnidadDeTrabajo unidad = _unidadDeTrabajoFactory.Crear())
            {
                try
                {
                    RubroComercio nuevoRubro = new RubroComercio { Nombre = nombreNormalizado };

                    int id = unidad.RubrosComercio.InsertarYObtenerId(nuevoRubro);
                    nuevoRubro.Id = id;

                    _bitacoraBLL.RegistrarOperacionInterno("FormMerchantManagement.Value.Activity.RubroCreation", "FormMerchantManagement.Value.Info.RubroCreation", "RubroComercioBLL", unidad);
                    unidad.Save();
                    return nuevoRubro;
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

        public bool Actualizar(int id, string nombre)
        {
            string nombreNormalizado = NormalizarNombre(nombre);

            using (IUnidadDeTrabajo unidad = _unidadDeTrabajoFactory.Crear())
            {
                try
                {
                    RubroComercio rubro = unidad.RubrosComercio.ObtenerPorId(id);
                    if (rubro == null)
                    {
                        throw new ExcepcionNegocio("Error.MerchantManagement.RubroNotFound");
                    }

                    rubro.Nombre = nombreNormalizado;
                    bool actualizado = unidad.RubrosComercio.Actualizar(rubro);
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

        public bool Eliminar(int id)
        {
            using (IUnidadDeTrabajo unidad = _unidadDeTrabajoFactory.Crear())
            {
                try
                {
                    RubroComercio rubro = unidad.RubrosComercio.ObtenerPorId(id);
                    if (rubro == null)
                    {
                        throw new ExcepcionNegocio("Error.MerchantManagement.RubroNotFound");
                    }

                    // Validacion proactiva: un rubro con comercios o referenciado por una
                    // promocion no se da de baja.
                    if (unidad.Comercios.ObtenerPorRubro(id).Count > 0
                        || unidad.PromocionRubrosComercio.ContarPorRubroComercio(id) > 0)
                    {
                        throw new ExcepcionNegocio("Error.MerchantManagement.RubroInUse");
                    }

                    bool eliminado = unidad.RubrosComercio.Eliminar(id);
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
