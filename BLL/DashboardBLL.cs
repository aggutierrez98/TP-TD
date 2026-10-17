using System;
using System.Collections.Generic;
using System.Linq;
using ABSTRACCIONES;
using BE;
using REPOSITORIO;
using SERVICIOS;

namespace BLL
{
    public class DashboardBLL
    {
        private readonly IDashboardRepositorio _dashboardRepositorio;
        private readonly ITarjetaDashboardRepositorio _tarjetaDashboardRepositorio;
        private readonly IUnidadDeTrabajoFactory _unidadDeTrabajoFactory;
        private readonly ISesion _sesion;
        private readonly IBitacoraBLL _bitacoraBLL;

        internal DashboardBLL(
            IDashboardRepositorio dashboardRepositorio,
            ITarjetaDashboardRepositorio tarjetaDashboardRepositorio,
            IUnidadDeTrabajoFactory unidadDeTrabajoFactory,
            ISesion sesion,
            IBitacoraBLL bitacoraBLL)
        {
            _dashboardRepositorio = dashboardRepositorio;
            _tarjetaDashboardRepositorio = tarjetaDashboardRepositorio;
            _unidadDeTrabajoFactory = unidadDeTrabajoFactory;
            _sesion = sesion;
            _bitacoraBLL = bitacoraBLL;
        }

        // Devuelve el Dashboard del usuario actual; si todavia no tiene uno lo crea
        // vacio. Cada card del Dashboard vive en TarjetaDashboard (AgregarTarjeta).
        public Dashboard ObtenerOCrearDashboardActual()
        {
            int usuarioActualId = _sesion.GetUsuarioActualId().Value;

            Dashboard dashboard = _dashboardRepositorio.ObtenerPorUsuario(usuarioActualId);
            if (dashboard != null)
            {
                return dashboard;
            }

            using (IUnidadDeTrabajo unidad = _unidadDeTrabajoFactory.Crear())
            {
                try
                {
                    Dashboard nuevoDashboard = new Dashboard
                    {
                        Usuario = new Usuario { Id = usuarioActualId },
                        Tarjetas = new List<TarjetaDashboard>()
                    };

                    int id = unidad.Dashboards.InsertarYObtenerId(nuevoDashboard);
                    nuevoDashboard.Id = id;

                    _bitacoraBLL.RegistrarOperacionInterno(
                        "FormDashboard.Value.Activity.DashboardCreation",
                        "FormDashboard.Value.Info.DashboardCreation",
                        "DashboardBLL",
                        unidad);

                    unidad.Save();
                    return nuevoDashboard;
                }
                catch (ExcepcionNegocio)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    throw new ExcepcionTecnica("Error.Dashboard.CreateFailed", ex);
                }
            }
        }

        public TarjetaDashboard AgregarTarjeta(TipoTarjetaDashboard tipo)
        {
            Dashboard dashboard = ObtenerOCrearDashboardActual();
            List<TarjetaDashboard> tarjetasActuales = dashboard.Tarjetas ?? new List<TarjetaDashboard>();

            if (tarjetasActuales.Any(item => item.Tipo == tipo))
            {
                throw new ExcepcionNegocio("Error.Dashboard.TarjetaYaAgregada");
            }

            int orden = tarjetasActuales.Any() ? tarjetasActuales.Max(item => item.Orden) + 1 : 1;

            using (IUnidadDeTrabajo unidad = _unidadDeTrabajoFactory.Crear())
            {
                try
                {
                    TarjetaDashboard nuevaTarjeta = new TarjetaDashboard
                    {
                        Dashboard = dashboard,
                        Tipo = tipo,
                        Orden = orden
                    };

                    int id = unidad.TarjetasDashboard.InsertarYObtenerId(nuevaTarjeta);
                    nuevaTarjeta.Id = id;

                    _bitacoraBLL.RegistrarOperacionInterno(
                        "FormDashboard.Value.Activity.CardCreation",
                        "FormDashboard.Value.Info.CardCreation",
                        "DashboardBLL",
                        unidad);

                    unidad.Save();
                    return nuevaTarjeta;
                }
                catch (ExcepcionNegocio)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    throw new ExcepcionTecnica("Error.Dashboard.CardCreateFailed", ex);
                }
            }
        }

        public bool QuitarTarjeta(int tarjetaId)
        {
            int usuarioActualId = _sesion.GetUsuarioActualId().Value;

            using (IUnidadDeTrabajo unidad = _unidadDeTrabajoFactory.Crear())
            {
                try
                {
                    TarjetaDashboard tarjeta = unidad.TarjetasDashboard.ObtenerPorId(tarjetaId);
                    ValidarPropiedad(tarjeta, usuarioActualId);

                    bool eliminada = unidad.TarjetasDashboard.Eliminar(tarjetaId);
                    if (!eliminada)
                    {
                        throw new ExcepcionTecnica("Error.Dashboard.CardDeleteFailed");
                    }

                    _bitacoraBLL.RegistrarOperacionInterno(
                        "FormDashboard.Value.Activity.CardDeletion",
                        "FormDashboard.Value.Info.CardDeletion",
                        "DashboardBLL",
                        unidad);

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
                    throw new ExcepcionTecnica("Error.Dashboard.CardDeleteFailed", ex);
                }
            }
        }

        // Tipos de tarjeta que el usuario actual todavia no agrego a su Dashboard.
        public IReadOnlyList<TipoTarjetaDashboard> ObtenerTiposDisponibles()
        {
            Dashboard dashboard = ObtenerOCrearDashboardActual();
            HashSet<TipoTarjetaDashboard> tiposUsados = new HashSet<TipoTarjetaDashboard>(
                (dashboard.Tarjetas ?? new List<TarjetaDashboard>()).Select(item => item.Tipo));

            return Enum.GetValues(typeof(TipoTarjetaDashboard))
                .Cast<TipoTarjetaDashboard>()
                .Where(tipo => !tiposUsados.Contains(tipo))
                .ToList();
        }

        // Una tarjeta solo puede ser quitada por el dueno del Dashboard al que pertenece.
        private static void ValidarPropiedad(TarjetaDashboard tarjeta, int usuarioActualId)
        {
            if (tarjeta == null)
            {
                throw new ExcepcionNegocio("Error.Dashboard.TarjetaNotFound");
            }

            if (tarjeta.Dashboard == null || tarjeta.Dashboard.Usuario == null || tarjeta.Dashboard.Usuario.Id != usuarioActualId)
            {
                throw new ExcepcionNegocio("Error.Dashboard.TarjetaNotOwned");
            }
        }
    }
}
