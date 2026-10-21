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

        // Devuelve la misma instancia recibida, normalizada, con el rubro persistido y el Id cargado.
        public Comercio Crear(Comercio comercio)
        {
            string nombreNormalizado = NormalizarNombre(comercio?.Nombre);
            ValidarCoordenadas(comercio.Latitud, comercio.Longitud);

            using (IUnidadDeTrabajo unidad = _unidadDeTrabajoFactory.Crear())
            {
                try
                {
                    RubroComercio rubro = ObtenerRubroPersistido(unidad, comercio.RubroComercio);

                    comercio.RubroComercio = rubro;
                    comercio.Nombre = nombreNormalizado;
                    comercio.Direccion = NormalizarTexto(comercio.Direccion);
                    comercio.Localidad = NormalizarTexto(comercio.Localidad);
                    comercio.Provincia = NormalizarTexto(comercio.Provincia);

                    comercio.Id = unidad.Comercios.InsertarYObtenerId(comercio);

                    _bitacoraBLL.RegistrarOperacionInterno("FormMerchantManagement.Value.Activity.ComercioCreation", "FormMerchantManagement.Value.Info.ComercioCreation", "ComercioBLL", unidad);
                    unidad.Save();
                    return comercio;
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

        // Recibe el comercio con los valores nuevos y los copia sobre el persistido.
        public bool Actualizar(Comercio comercio)
        {
            string nombreNormalizado = NormalizarNombre(comercio?.Nombre);
            ValidarCoordenadas(comercio.Latitud, comercio.Longitud);

            using (IUnidadDeTrabajo unidad = _unidadDeTrabajoFactory.Crear())
            {
                try
                {
                    Comercio persistido = unidad.Comercios.ObtenerPorId(comercio.Id);
                    if (persistido == null)
                    {
                        throw new ExcepcionNegocio("Error.MerchantManagement.ComercioNotFound");
                    }

                    RubroComercio rubro = ObtenerRubroPersistido(unidad, comercio.RubroComercio);

                    persistido.Nombre = nombreNormalizado;
                    persistido.RubroComercio = rubro;
                    persistido.Direccion = NormalizarTexto(comercio.Direccion);
                    persistido.Localidad = NormalizarTexto(comercio.Localidad);
                    persistido.Provincia = NormalizarTexto(comercio.Provincia);
                    persistido.Latitud = comercio.Latitud;
                    persistido.Longitud = comercio.Longitud;

                    bool actualizado = unidad.Comercios.Actualizar(persistido);
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

        public bool Eliminar(Comercio comercio)
        {
            using (IUnidadDeTrabajo unidad = _unidadDeTrabajoFactory.Crear())
            {
                try
                {
                    Comercio persistido = comercio == null ? null : unidad.Comercios.ObtenerPorId(comercio.Id);
                    if (persistido == null)
                    {
                        throw new ExcepcionNegocio("Error.MerchantManagement.ComercioNotFound");
                    }

                    // Un comercio no se borra mientras algo lo referencie (hay que soltar esas
                    // referencias primero): productos asignados, movimientos, promociones y reglas.
                    if (unidad.ComercioProductos.ObtenerPorComercio(persistido).Count > 0
                        || unidad.Movimientos.ContarPorComercio(persistido) > 0
                        || unidad.PromocionComercio.ObtenerPromocionesPorComercio(persistido).Count > 0
                        || unidad.ComercioReglaCategorizacion.ContarPorComercio(persistido) > 0)
                    {
                        throw new ExcepcionNegocio("Error.MerchantManagement.ComercioInUse");
                    }

                    bool eliminado = unidad.Comercios.Eliminar(persistido);
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
                catch (ExcepcionTecnica)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    throw new ExcepcionTecnica("Error.MerchantManagement.ComercioDeleteFailed", ex);
                }
            }
        }

        // Relee el rubro recibido: el comercio queda con la entidad persistida, no con la recibida.
        private static RubroComercio ObtenerRubroPersistido(IUnidadDeTrabajo unidad, RubroComercio rubro)
        {
            RubroComercio persistido = rubro == null ? null : unidad.RubrosComercio.ObtenerPorId(rubro.Id);
            if (persistido == null)
            {
                throw new ExcepcionNegocio("Error.MerchantManagement.RubroNotFound");
            }

            return persistido;
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

        private static string NormalizarTexto(string valor)
        {
            string valorNormalizado = (valor ?? string.Empty).Trim();
            return string.IsNullOrWhiteSpace(valorNormalizado) ? null : valorNormalizado;
        }

        private static void ValidarCoordenadas(decimal? latitud, decimal? longitud)
        {
            if (latitud.HasValue != longitud.HasValue)
            {
                throw new ExcepcionNegocio("Error.MerchantManagement.CoordenadasIncompletas");
            }
        }
    }
}
