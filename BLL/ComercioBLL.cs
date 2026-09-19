using System;
using System.Collections.Generic;
using BE;
using REPOSITORIO;
using SERVICIOS;

namespace BLL
{
    // Pantalla de operador (back-office): sin validacion de "dueno", Comercio/RubroComercio
    // no tienen Usuario asociado.
    public class ComercioBLL
    {
        private readonly IComercioRepositorio _comercioRepositorio;
        private readonly IUnidadDeTrabajoFactory _unidadDeTrabajoFactory;
        private readonly IBitacoraBLL _bitacoraBLL;

        internal ComercioBLL(
            IComercioRepositorio comercioRepositorio,
            IUnidadDeTrabajoFactory unidadDeTrabajoFactory,
            IBitacoraBLL bitacoraBLL)
        {
            _comercioRepositorio = comercioRepositorio;
            _unidadDeTrabajoFactory = unidadDeTrabajoFactory;
            _bitacoraBLL = bitacoraBLL;
        }

        public List<Comercio> ObtenerTodos()
        {
            return _comercioRepositorio.ObtenerTodos();
        }

        public Comercio Crear(string nombre, int rubroComercioId, string ubicacion)
        {
            string nombreNormalizado = NormalizarNombre(nombre);

            using (IUnidadDeTrabajo unidad = _unidadDeTrabajoFactory.Crear())
            {
                try
                {
                    RubroComercio rubro = unidad.RubrosComercio.ObtenerPorId(rubroComercioId);
                    if (rubro == null)
                    {
                        throw new ExcepcionNegocio("Error.MerchantManagement.RubroNotFound");
                    }

                    Comercio nuevoComercio = new Comercio
                    {
                        RubroComercio = rubro,
                        Nombre = nombreNormalizado,
                        Ubicacion = NormalizarUbicacion(ubicacion)
                    };

                    int id = unidad.Comercios.InsertarYObtenerId(nuevoComercio);
                    nuevoComercio.Id = id;

                    _bitacoraBLL.RegistrarOperacionInterno("FormMerchantManagement.Value.Activity.ComercioCreation", "FormMerchantManagement.Value.Info.ComercioCreation", "ComercioBLL", unidad);
                    unidad.Save();
                    return nuevoComercio;
                }
                catch (ExcepcionNegocio)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    throw new ExcepcionTecnica("Error.MerchantManagement.ComercioCreateFailed", ex);
                }
            }
        }

        public bool Actualizar(int id, string nombre, int rubroComercioId)
        {
            string nombreNormalizado = NormalizarNombre(nombre);

            using (IUnidadDeTrabajo unidad = _unidadDeTrabajoFactory.Crear())
            {
                try
                {
                    Comercio comercio = unidad.Comercios.ObtenerPorId(id);
                    if (comercio == null)
                    {
                        throw new ExcepcionNegocio("Error.MerchantManagement.ComercioNotFound");
                    }

                    RubroComercio rubro = unidad.RubrosComercio.ObtenerPorId(rubroComercioId);
                    if (rubro == null)
                    {
                        throw new ExcepcionNegocio("Error.MerchantManagement.RubroNotFound");
                    }

                    comercio.Nombre = nombreNormalizado;
                    comercio.RubroComercio = rubro;

                    bool actualizado = unidad.Comercios.Actualizar(comercio);
                    if (!actualizado)
                    {
                        throw new ExcepcionTecnica("Error.MerchantManagement.ComercioUpdateFailed");
                    }

                    _bitacoraBLL.RegistrarOperacionInterno("FormMerchantManagement.Value.Activity.ComercioUpdate", "FormMerchantManagement.Value.Info.ComercioUpdate", "ComercioBLL", unidad);
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
                    throw new ExcepcionTecnica("Error.MerchantManagement.ComercioUpdateFailed", ex);
                }
            }
        }

        public bool Eliminar(int id)
        {
            using (IUnidadDeTrabajo unidad = _unidadDeTrabajoFactory.Crear())
            {
                try
                {
                    Comercio comercio = unidad.Comercios.ObtenerPorId(id);
                    if (comercio == null)
                    {
                        throw new ExcepcionNegocio("Error.MerchantManagement.ComercioNotFound");
                    }

                    bool eliminado = unidad.Comercios.Eliminar(id);
                    if (!eliminado)
                    {
                        throw new ExcepcionTecnica("Error.MerchantManagement.ComercioDeleteFailed");
                    }

                    _bitacoraBLL.RegistrarOperacionInterno("FormMerchantManagement.Value.Activity.ComercioDeletion", "FormMerchantManagement.Value.Info.ComercioDeletion", "ComercioBLL", unidad);
                    unidad.Save();
                    return true;
                }
                catch (ExcepcionNegocio)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    // A diferencia de RubroComercio, aca no podemos validar de antemano:
                    // Producto (quien referencia a Comercio via FK) esta fuera de esta tanda
                    // y su repositorio todavia no forma parte de la Unidad de Trabajo. El
                    // SqlException original ya llega envuelto como ExcepcionTecnica desde
                    // AccesoDatosSqlServer.ExecuteInTransaction, por eso se revisa antes de
                    // decidir si hay que relanzarla tal cual o convertirla en negocio.
                    if (ViolacionIntegridadReferencialHelper.EsViolacionRestriccionForanea(ex))
                    {
                        throw new ExcepcionNegocio("Error.MerchantManagement.ComercioInUse");
                    }

                    if (ex is ExcepcionTecnica)
                    {
                        throw;
                    }

                    throw new ExcepcionTecnica("Error.MerchantManagement.ComercioDeleteFailed", ex);
                }
            }
        }

        private static string NormalizarNombre(string nombre)
        {
            string nombreNormalizado = (nombre ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(nombreNormalizado))
            {
                throw new ExcepcionNegocio("Error.MerchantManagement.ComercioNameRequired");
            }

            return nombreNormalizado;
        }

        private static string NormalizarUbicacion(string ubicacion)
        {
            string ubicacionNormalizada = (ubicacion ?? string.Empty).Trim();
            return string.IsNullOrWhiteSpace(ubicacionNormalizada) ? null : ubicacionNormalizada;
        }
    }
}
