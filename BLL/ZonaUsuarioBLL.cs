using System;
using System.Collections.Generic;
using System.Linq;
using ABSTRACCIONES;
using BE;
using REPOSITORIO;
using SERVICIOS;

namespace BLL
{
    // Agregado personal: cada ZonaUsuario pertenece al usuario de la sesion y solo el puede
    // verla o quitarla. Solo alta y baja (no hay edicion). Expone ademas la distancia de un
    // punto a las zonas, que consume la recomendacion de compras.
    public class ZonaUsuarioBLL
    {
        public const int MaximoZonasPorUsuario = 10;
        public const int LongitudMaximaNombre = 50;
        public const int RadioMinimoMetros = 100;
        public const int RadioMaximoMetros = 20000;
        public const int RadioPorDefectoMetros = 1000;
        public const int PasoRadioMetros = 100;

        // Escala de las columnas DECIMAL(9,6) de ZonaUsuario.
        private const int DecimalesCoordenadas = 6;

        private readonly IZonaUsuarioRepositorio _zonaUsuarioRepositorio;
        private readonly IUnidadDeTrabajoFactory _unidadDeTrabajoFactory;
        private readonly ISesion _sesion;
        private readonly IBitacoraBLL _bitacoraBLL;

        internal ZonaUsuarioBLL(
            IZonaUsuarioRepositorio zonaUsuarioRepositorio,
            IUnidadDeTrabajoFactory unidadDeTrabajoFactory,
            ISesion sesion,
            IBitacoraBLL bitacoraBLL)
        {
            _zonaUsuarioRepositorio = zonaUsuarioRepositorio;
            _unidadDeTrabajoFactory = unidadDeTrabajoFactory;
            _sesion = sesion;
            _bitacoraBLL = bitacoraBLL;
        }

        public List<ZonaUsuario> ObtenerMisZonas()
        {
            int usuarioActualId = _sesion.GetUsuarioActualId().Value;
            return _zonaUsuarioRepositorio.ObtenerPorUsuario(usuarioActualId);
        }

        // latitud/longitud null = el usuario todavia no eligio un punto (busqueda o clic en el mapa).
        public ZonaUsuario Crear(string nombre, decimal? latitud, decimal? longitud, int radioMetros)
        {
            string nombreNormalizado = NormalizarNombre(nombre);
            ValidarPunto(latitud, longitud);
            ValidarRadio(radioMetros);

            int usuarioActualId = _sesion.GetUsuarioActualId().Value;

            using (IUnidadDeTrabajo unidad = _unidadDeTrabajoFactory.Crear())
            {
                try
                {
                    List<ZonaUsuario> zonasDelUsuario = unidad.ZonasUsuario.ObtenerPorUsuario(usuarioActualId);
                    if (zonasDelUsuario.Count >= MaximoZonasPorUsuario)
                    {
                        throw new ExcepcionNegocio("Error.MyZones.LimitReached");
                    }

                    if (zonasDelUsuario.Any(zona => string.Equals(zona.Nombre, nombreNormalizado, StringComparison.OrdinalIgnoreCase)))
                    {
                        throw new ExcepcionNegocio("Error.MyZones.DuplicateName");
                    }

                    ZonaUsuario nuevaZona = new ZonaUsuario
                    {
                        Usuario = new Usuario { Id = usuarioActualId },
                        Nombre = nombreNormalizado,
                        Latitud = Math.Round(latitud.Value, DecimalesCoordenadas, MidpointRounding.AwayFromZero),
                        Longitud = Math.Round(longitud.Value, DecimalesCoordenadas, MidpointRounding.AwayFromZero),
                        RadioMetros = radioMetros
                    };
                    nuevaZona.Id = unidad.ZonasUsuario.InsertarYObtenerId(nuevaZona);

                    _bitacoraBLL.RegistrarOperacionInterno("FormMyZones.Value.Activity.Creation", "FormMyZones.Value.Info.Creation", "ZonaUsuarioBLL", unidad);
                    unidad.Save();
                    return nuevaZona;
                }
                catch (ExcepcionNegocio)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    throw new ExcepcionTecnica("Error.MyZones.CreateFailed", ex);
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
                    ZonaUsuario zona = unidad.ZonasUsuario.ObtenerPorId(id);
                    ValidarPropiedad(zona, usuarioActualId);

                    if (!unidad.ZonasUsuario.Eliminar(id))
                    {
                        throw new ExcepcionTecnica("Error.MyZones.DeleteFailed");
                    }

                    _bitacoraBLL.RegistrarOperacionInterno("FormMyZones.Value.Activity.Deletion", "FormMyZones.Value.Info.Deletion", "ZonaUsuarioBLL", unidad);
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
                    throw new ExcepcionTecnica("Error.MyZones.DeleteFailed", ex);
                }
            }
        }

        // Distancia del punto a cada zona del usuario de la sesion, de la mas cercana a la mas lejana.
        public List<DistanciaZonaUsuario> CalcularDistanciasAMisZonas(decimal latitud, decimal longitud)
        {
            return CalcularDistancias(ObtenerMisZonas(), latitud, longitud);
        }

        // Zona del usuario cuyo centro esta mas cerca del punto (null si no tiene zonas),
        // con su distancia y si el punto cae dentro de su radio.
        public DistanciaZonaUsuario ObtenerZonaMasCercana(decimal latitud, decimal longitud)
        {
            return CalcularDistanciasAMisZonas(latitud, longitud).FirstOrDefault();
        }

        // Zona mas cercana entre las que contienen al punto dentro de su radio (null si ninguna).
        public DistanciaZonaUsuario ObtenerZonaQueContiene(decimal latitud, decimal longitud)
        {
            return CalcularDistanciasAMisZonas(latitud, longitud).FirstOrDefault(distancia => distancia.EstaDentroDelRadio);
        }

        // Calculo puro sobre zonas ya leidas: permite evaluar muchos puntos (por ejemplo,
        // comercios) con una sola lectura de ObtenerMisZonas.
        public static List<DistanciaZonaUsuario> CalcularDistancias(IEnumerable<ZonaUsuario> zonas, decimal latitud, decimal longitud)
        {
            return (zonas ?? Enumerable.Empty<ZonaUsuario>())
                .Select(zona => new DistanciaZonaUsuario(
                    zona,
                    DistanciaGeografica.CalcularMetros(latitud, longitud, zona.Latitud, zona.Longitud)))
                .OrderBy(distancia => distancia.DistanciaMetros)
                .ToList();
        }

        private static string NormalizarNombre(string nombre)
        {
            string nombreNormalizado = (nombre ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(nombreNormalizado))
            {
                throw new ExcepcionNegocio("Error.MyZones.NameRequired");
            }

            if (nombreNormalizado.Length > LongitudMaximaNombre)
            {
                throw new ExcepcionNegocio("Error.MyZones.NameTooLong");
            }

            return nombreNormalizado;
        }

        private static void ValidarPunto(decimal? latitud, decimal? longitud)
        {
            if (!latitud.HasValue || !longitud.HasValue)
            {
                throw new ExcepcionNegocio("Error.MyZones.PointRequired");
            }

            if (latitud.Value < -90m || latitud.Value > 90m || longitud.Value < -180m || longitud.Value > 180m)
            {
                throw new ExcepcionNegocio("Error.MyZones.InvalidCoordinates");
            }
        }

        private static void ValidarRadio(int radioMetros)
        {
            if (radioMetros < RadioMinimoMetros || radioMetros > RadioMaximoMetros)
            {
                throw new ExcepcionNegocio("Error.MyZones.InvalidRadius");
            }
        }

        // Una zona solo puede ser quitada por su dueno.
        private static void ValidarPropiedad(ZonaUsuario zona, int usuarioActualId)
        {
            if (zona == null)
            {
                throw new ExcepcionNegocio("Error.MyZones.NotFound");
            }

            if (zona.Usuario == null || zona.Usuario.Id != usuarioActualId)
            {
                throw new ExcepcionNegocio("Error.MyZones.NotOwned");
            }
        }
    }
}
