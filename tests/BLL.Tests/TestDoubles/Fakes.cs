using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ABSTRACCIONES;
using BE;
using BLL;
using REPOSITORIO;
using SERVICIOS;

namespace BLL.Tests.TestDoubles
{
    internal sealed class FakeUnidadDeTrabajoFactory : IUnidadDeTrabajoFactory
    {
        public FakeUnidadDeTrabajoFactory()
            : this(new FakeUnidadDeTrabajo())
        {
        }

        public FakeUnidadDeTrabajoFactory(FakeUnidadDeTrabajo unidad)
        {
            NextUnidad = unidad;
        }

        public FakeUnidadDeTrabajo NextUnidad { get; set; }

        public FakeUnidadDeTrabajo UltimaUnidadCreada { get; private set; }

        public int CrearCallCount { get; private set; }

        public IUnidadDeTrabajo Crear()
        {
            CrearCallCount++;
            UltimaUnidadCreada = NextUnidad ?? new FakeUnidadDeTrabajo();
            return UltimaUnidadCreada;
        }
    }

    internal sealed class FakeUnidadDeTrabajo : IUnidadDeTrabajo
    {
        public FakeUnidadDeTrabajo()
        {
            BitacoraRepo = new FakeBitacoraRepositorio();
            IdiomaRepo = new FakeIdiomaRepositorio();
            IntegridadRepo = new FakeIntegridadRepositorio();
            PermisoRepo = new FakePermisoRepositorio();
            TraduccionRepo = new FakeTraduccionRepositorio();
            UsuarioHistorialRepo = new FakeUsuarioHistorialRepositorio();
            UsuarioPermisoRepo = new FakeUsuarioPermisoRepositorio();
            UsuarioPreferenciaRepo = new FakeUsuarioPreferenciaRepositorio();
            UsuarioRepo = new FakeUsuarioRepositorio();
        }

        public FakeBitacoraRepositorio BitacoraRepo { get; set; }

        public FakeIdiomaRepositorio IdiomaRepo { get; set; }

        public FakeIntegridadRepositorio IntegridadRepo { get; set; }

        public FakePermisoRepositorio PermisoRepo { get; set; }

        public FakeTraduccionRepositorio TraduccionRepo { get; set; }

        public FakeUsuarioHistorialRepositorio UsuarioHistorialRepo { get; set; }

        public FakeUsuarioPermisoRepositorio UsuarioPermisoRepo { get; set; }

        public FakeUsuarioPreferenciaRepositorio UsuarioPreferenciaRepo { get; set; }

        public FakeUsuarioRepositorio UsuarioRepo { get; set; }

        public bool SaveCalled { get; private set; }

        public int SaveCallCount { get; private set; }

        public bool DisposeCalled { get; private set; }

        public bool ThrowOnSave { get; set; }

        public IBitacoraRepositorio Bitacora { get { return BitacoraRepo; } }

        public IIdiomaRepositorio Idiomas { get { return IdiomaRepo; } }

        public IIntegridadRepositorio Integridad { get { return IntegridadRepo; } }

        public IPermisoRepositorio Permisos { get { return PermisoRepo; } }

        public ITraduccionRepositorio Traducciones { get { return TraduccionRepo; } }

        public IUsuarioHistorialRepositorio UsuarioHistorial { get { return UsuarioHistorialRepo; } }

        public IUsuarioPermisoRepositorio UsuarioPermisos { get { return UsuarioPermisoRepo; } }

        public IUsuarioPreferenciaRepositorio UsuarioPreferencias { get { return UsuarioPreferenciaRepo; } }

        public IUsuarioRepositorio Usuarios { get { return UsuarioRepo; } }

        public void Save()
        {
            if (ThrowOnSave)
            {
                throw new InvalidOperationException("Save failed.");
            }

            SaveCalled = true;
            SaveCallCount++;
        }

        public void Dispose()
        {
            DisposeCalled = true;
        }
    }

    internal sealed class FakeUsuarioRepositorio : IUsuarioRepositorio
    {
        private int _nextId = 1;

        public Dictionary<int, Usuario> Usuarios { get; } = new Dictionary<int, Usuario>();

        public bool ActualizarResult { get; set; } = true;

        public bool ActualizarDvhResult { get; set; } = true;

        public bool GuardarEstadoCompletoResult { get; set; } = true;

        public bool EliminarResult { get; set; } = true;

        public bool ThrowOnInsert { get; set; }

        public Usuario LastInserted { get; private set; }

        public Usuario LastUpdated { get; private set; }

        public int? LastDeletedId { get; private set; }

        public Usuario ObtenerPorId(int id)
        {
            Usuario usuario;
            return Usuarios.TryGetValue(id, out usuario) ? usuario : null;
        }

        public Usuario ObtenerPorUsername(string username)
        {
            return Usuarios.Values.FirstOrDefault(item => string.Equals(item.Username, username, StringComparison.OrdinalIgnoreCase));
        }

        public int InsertarYObtenerId(Usuario usuario)
        {
            if (ThrowOnInsert)
            {
                throw new InvalidOperationException("Insert failed.");
            }

            if (usuario.Id <= 0)
            {
                usuario.Id = _nextId++;
            }

            Usuarios[usuario.Id] = usuario;
            LastInserted = usuario;
            return usuario.Id;
        }

        public bool Actualizar(Usuario usuario)
        {
            LastUpdated = usuario;
            if (!ActualizarResult)
            {
                return false;
            }

            Usuarios[usuario.Id] = usuario;
            return true;
        }

        public bool ActualizarDVH(int id, string dvh)
        {
            if (!ActualizarDvhResult)
            {
                return false;
            }

            Usuario usuario = ObtenerPorId(id);
            if (usuario != null)
            {
                usuario.DVH = dvh;
            }

            return true;
        }

        public bool GuardarEstadoCompleto(Usuario usuario)
        {
            if (!GuardarEstadoCompletoResult)
            {
                return false;
            }

            Usuarios[usuario.Id] = usuario;
            LastUpdated = usuario;
            return true;
        }

        public bool EliminarFisicamente(int id)
        {
            LastDeletedId = id;
            return EliminarResult && Usuarios.Remove(id);
        }

        public List<Usuario> ObtenerTodos()
        {
            return Usuarios.Values.ToList();
        }
    }

    internal sealed class FakePermisoRepositorio : IPermisoRepositorio
    {
        private int _nextId = 100;

        public List<IPermiso> Nodos { get; } = new List<IPermiso>();

        public List<Tuple<int, int>> Relaciones { get; } = new List<Tuple<int, int>>();

        public Dictionary<string, int> IdsPorNombre { get; } = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

        public List<IPermiso> Guardados { get; } = new List<IPermiso>();

        public List<int> IdsEliminados { get; private set; } = new List<int>();

        public List<Tuple<int, int>> RelacionesReemplazadas { get; private set; } = new List<Tuple<int, int>>();

        public bool ThrowOnGuardar { get; set; }

        public List<IPermiso> ObtenerNodos()
        {
            return Nodos.ToList();
        }

        public List<Tuple<int, int>> ObtenerRelaciones()
        {
            return Relaciones.ToList();
        }

        public int Guardar(IPermiso permiso)
        {
            if (ThrowOnGuardar)
            {
                throw new InvalidOperationException("Permission save failed.");
            }

            if (permiso.Id <= 0)
            {
                permiso.Id = _nextId++;
            }

            Guardados.Add(permiso);
            if (Nodos.All(item => item.Id != permiso.Id))
            {
                Nodos.Add(permiso);
            }

            return permiso.Id;
        }

        public bool ReemplazarRelaciones(List<Tuple<int, int>> relaciones)
        {
            RelacionesReemplazadas = relaciones ?? new List<Tuple<int, int>>();
            Relaciones.Clear();
            Relaciones.AddRange(RelacionesReemplazadas);
            return true;
        }

        public bool EliminarPermisos(List<int> idsPermiso)
        {
            IdsEliminados = idsPermiso ?? new List<int>();
            Nodos.RemoveAll(item => IdsEliminados.Contains(item.Id));
            return true;
        }

        public int ObtenerIdPorNombre(string nombre)
        {
            int id;
            return IdsPorNombre.TryGetValue(nombre, out id) ? id : 0;
        }
    }

    internal sealed class FakeUsuarioPermisoRepositorio : IUsuarioPermisoRepositorio
    {
        public Dictionary<int, List<int>> PermisosPorUsuario { get; } = new Dictionary<int, List<int>>();

        public int? LastUsuarioId { get; private set; }

        public List<int> LastPermisos { get; private set; }

        public List<int> ObtenerPermisoIdsPorUsuario(int usuarioId)
        {
            List<int> permisos;
            return PermisosPorUsuario.TryGetValue(usuarioId, out permisos) ? permisos.ToList() : new List<int>();
        }

        public bool ReemplazarPermisosUsuario(int usuarioId, List<int> permisosId)
        {
            LastUsuarioId = usuarioId;
            LastPermisos = permisosId == null ? new List<int>() : permisosId.ToList();
            PermisosPorUsuario[usuarioId] = LastPermisos.ToList();
            return true;
        }
    }

    internal sealed class FakeUsuarioHistorialRepositorio : IUsuarioHistorialRepositorio
    {
        private int _nextId = 1;

        public List<UsuarioHistorialSnapshot> Snapshots { get; } = new List<UsuarioHistorialSnapshot>();

        public UsuarioHistorialSnapshot LastSaved { get; private set; }

        public int GuardarVersion(UsuarioHistorialSnapshot snapshot)
        {
            if (snapshot.HistorialId <= 0)
            {
                snapshot.HistorialId = _nextId++;
            }

            LastSaved = snapshot;
            Snapshots.Add(snapshot);
            return snapshot.HistorialId;
        }

        public List<UsuarioHistorialSnapshot> ObtenerVersiones(int usuarioId)
        {
            return Snapshots.Where(item => item.UsuarioId == usuarioId).ToList();
        }

        public UsuarioHistorialSnapshot ObtenerVersion(int historialId)
        {
            return Snapshots.FirstOrDefault(item => item.HistorialId == historialId);
        }
    }

    internal sealed class FakeUsuarioPreferenciaRepositorio : IUsuarioPreferenciaRepositorio
    {
        public Dictionary<int, UsuarioPreferencia> Preferencias { get; } = new Dictionary<int, UsuarioPreferencia>();

        public int? LastUsuarioId { get; private set; }

        public int? LastIdiomaId { get; private set; }

        public UsuarioPreferencia ObtenerPorUsuarioId(int usuarioId)
        {
            UsuarioPreferencia preferencia;
            return Preferencias.TryGetValue(usuarioId, out preferencia) ? preferencia : null;
        }

        public bool Guardar(UsuarioPreferencia preferencia)
        {
            Preferencias[preferencia.UsuarioId] = preferencia;
            LastUsuarioId = preferencia.UsuarioId;
            LastIdiomaId = preferencia.IdiomaId;
            return true;
        }

        public bool Guardar(int usuarioId, int? idiomaId)
        {
            LastUsuarioId = usuarioId;
            LastIdiomaId = idiomaId;
            if (idiomaId.HasValue)
            {
                Preferencias[usuarioId] = new UsuarioPreferencia { UsuarioId = usuarioId, IdiomaId = idiomaId.Value };
            }
            else
            {
                Preferencias.Remove(usuarioId);
            }

            return true;
        }
    }

    internal sealed class FakeIdiomaRepositorio : IIdiomaRepositorio
    {
        private int _nextId = 1;

        public List<Idioma> Idiomas { get; } = new List<Idioma>();

        public List<Idioma> Insertados { get; } = new List<Idioma>();

        public Idioma LastUpdated { get; private set; }

        public int? LastDeletedId { get; private set; }

        public List<Idioma> ObtenerIdiomas()
        {
            return Idiomas.ToList();
        }

        public Idioma ObtenerIdiomaCompleto(int idiomaId)
        {
            return Idiomas.FirstOrDefault(item => item.Id == idiomaId);
        }

        public Idioma ObtenerIdiomaDefault()
        {
            return Idiomas.FirstOrDefault(item => item.Default);
        }

        public int InsertarIdioma(Idioma idioma)
        {
            if (idioma.Id <= 0)
            {
                idioma.Id = _nextId++;
            }

            Insertados.Add(idioma);
            Idiomas.Add(idioma);
            return idioma.Id;
        }

        public bool ActualizarIdioma(Idioma idioma)
        {
            LastUpdated = idioma;
            Idioma existing = ObtenerIdiomaCompleto(idioma.Id);
            if (existing == null)
            {
                return false;
            }

            existing.Nombre = idioma.Nombre;
            return true;
        }

        public bool EliminarIdioma(int idiomaId)
        {
            LastDeletedId = idiomaId;
            return Idiomas.RemoveAll(item => item.Id == idiomaId) > 0;
        }

        public bool ConsolidarIdioma(int idiomaOrigenId, int idiomaDestinoId)
        {
            return true;
        }

        public int ObtenerPalabraIdPorKey(string key)
        {
            return 0;
        }

        public bool GuardarTraduccion(int idiomaId, int palabraId, string valor)
        {
            return true;
        }

        public bool EliminarTraduccion(int idiomaId, int palabraId)
        {
            return true;
        }

        public bool SwitchearIdiomaDefault(int idiomaId)
        {
            Idioma idioma = ObtenerIdiomaCompleto(idiomaId);
            if (idioma == null)
            {
                throw new InvalidOperationException("Language not found.");
            }

            idioma.Default = !idioma.Default;
            return true;
        }
    }

    internal sealed class FakeTraduccionRepositorio : ITraduccionRepositorio
    {
        public Dictionary<string, int> PalabraIdsPorKey { get; } = new Dictionary<string, int>(StringComparer.Ordinal);

        public List<Tuple<int, int, string>> TraduccionesGuardadas { get; } = new List<Tuple<int, int, string>>();

        public bool ThrowOnGuardar { get; set; }

        public int ObtenerPalabraIdPorKey(string key)
        {
            int id;
            return PalabraIdsPorKey.TryGetValue(key, out id) ? id : 0;
        }

        public bool GuardarTraduccion(int idiomaId, int palabraId, string valor)
        {
            if (ThrowOnGuardar)
            {
                throw new InvalidOperationException("Translation save failed.");
            }

            TraduccionesGuardadas.Add(Tuple.Create(idiomaId, palabraId, valor));
            return true;
        }

        public bool EliminarTraduccion(int idiomaId, int palabraId)
        {
            return true;
        }
    }

    internal sealed class FakeIntegridadRepositorio : IIntegridadRepositorio
    {
        public Dictionary<string, string> DvvPorTabla { get; } = new Dictionary<string, string>(StringComparer.Ordinal);

        public string LastTablaGuardada { get; private set; }

        public string LastValorGuardado { get; private set; }

        public string ObtenerDVV(string nombreTabla)
        {
            string value;
            return DvvPorTabla.TryGetValue(nombreTabla, out value) ? value : null;
        }

        public void GuardarDVV(string nombreTabla, string valor)
        {
            LastTablaGuardada = nombreTabla;
            LastValorGuardado = valor;
            DvvPorTabla[nombreTabla] = valor;
        }
    }

    internal sealed class FakeBitacoraRepositorio : IBitacoraRepositorio
    {
        public List<EventoBitacora> Eventos { get; } = new List<EventoBitacora>();

        public FiltroBitacora LastFiltro { get; private set; }

        public int Guardar(EventoBitacora evento)
        {
            evento.Id = Eventos.Count + 1;
            Eventos.Add(evento);
            return evento.Id;
        }

        public List<EventoBitacora> Buscar(FiltroBitacora filtro)
        {
            LastFiltro = filtro;
            return Eventos.ToList();
        }
    }

    internal sealed class FakeSesion : ISesion
    {
        private IUsuarioAutenticado _usuario;

        public int ActualizarNombreCallCount { get; private set; }

        public void IniciarSesion(IUsuarioAutenticado usuario)
        {
            _usuario = usuario;
        }

        public void CerrarSesion()
        {
            _usuario = null;
        }

        public IUsuarioAutenticado GetUsuarioActual()
        {
            return _usuario;
        }

        public int? GetUsuarioActualId()
        {
            return _usuario == null ? (int?)null : _usuario.Id;
        }

        public string GetUsuarioActualNombre()
        {
            return _usuario == null ? null : _usuario.Username;
        }

        public void ActualizarUsuarioActualNombre(string username)
        {
            ActualizarNombreCallCount++;
            if (_usuario != null)
            {
                _usuario.Username = username;
            }
        }

        public bool HaySesionActiva()
        {
            return _usuario != null;
        }
    }

    internal sealed class FakeAutorizacion : IAutorizacion
    {
        public IList<IPermiso> ActualizarPermisosArgument { get; private set; }

        public int ActualizarPermisosCallCount { get; private set; }

        public bool LimpiarCalled { get; private set; }

        public void ActualizarPermisosUsuarioActual(IEnumerable<IPermiso> permisos)
        {
            ActualizarPermisosCallCount++;
            ActualizarPermisosArgument = permisos == null
                ? new List<IPermiso>()
                : permisos.ToList();
        }

        public void Limpiar()
        {
            LimpiarCalled = true;
        }

        public bool TienePermiso(string rutaPermiso)
        {
            return false;
        }
    }

    internal sealed class FakePasswordHashService : IPasswordHashService
    {
        public bool VerifyResult { get; set; } = true;

        public string LastHashInput { get; private set; }

        public string LastVerifyPassword { get; private set; }

        public string LastVerifyHash { get; private set; }

        public string HashPassword(string passwordRaw)
        {
            LastHashInput = passwordRaw;
            return "HASH:" + passwordRaw;
        }

        public bool VerifyPassword(string passwordRaw, string passwordHash)
        {
            LastVerifyPassword = passwordRaw;
            LastVerifyHash = passwordHash;
            return VerifyResult;
        }
    }

    internal sealed class SpyBitacoraBLL : IBitacoraBLL
    {
        public List<BitacoraCall> Calls { get; } = new List<BitacoraCall>();

        public List<EventoBitacora> BuscarResult { get; set; } = new List<EventoBitacora>();

        public List<EventoBitacora> Buscar(FiltroBitacora filtro)
        {
            return BuscarResult;
        }

        public void RegistrarOperacion(string actividad, string informacion, string origen)
        {
            Calls.Add(new BitacoraCall("Operacion", actividad, informacion, origen, null));
        }

        public void RegistrarFalloNegocio(string actividad, string informacion, string origen)
        {
            Calls.Add(new BitacoraCall("FalloNegocio", actividad, informacion, origen, null));
        }

        public void RegistrarOperacionInterno(string actividad, string informacion, string origen, IUnidadDeTrabajo unidad)
        {
            Calls.Add(new BitacoraCall("OperacionInterno", actividad, informacion, origen, unidad));
        }

        public void RegistrarFalloNegocioInterno(string actividad, string informacion, string origen, IUnidadDeTrabajo unidad)
        {
            Calls.Add(new BitacoraCall("FalloNegocioInterno", actividad, informacion, origen, unidad));
        }
    }

    internal sealed class SpyIdiomaBLL : IIdiomaBLL
    {
        public int? AplicarIdiomaSesionUsuarioId { get; private set; }

        public void AplicarIdiomaSesionUsuario(int usuarioId)
        {
            AplicarIdiomaSesionUsuarioId = usuarioId;
        }
    }

    internal sealed class SpyPermisoBLL : IPermisoBLL
    {
        public List<IPermiso> PermisosDirectos { get; set; } = new List<IPermiso>();

        public int? ObtenerPermisosUsuarioId { get; private set; }

        public int? AsignarPerfilUsuarioId { get; private set; }

        public int? GuardarPermisosUsuarioId { get; private set; }

        public List<int> GuardarPermisosIds { get; private set; }

        public IUnidadDeTrabajo LastUnidad { get; private set; }

        public List<IPermiso> ObtenerPermisosDirectosUsuario(int usuarioId)
        {
            ObtenerPermisosUsuarioId = usuarioId;
            return PermisosDirectos;
        }

        public void AsignarPerfilBasicoInterno(int usuarioId, IUnidadDeTrabajo unidad)
        {
            AsignarPerfilUsuarioId = usuarioId;
            LastUnidad = unidad;
        }

        public void GuardarPermisosUsuarioInterno(int usuarioId, List<int> permisosId, IUnidadDeTrabajo unidad)
        {
            GuardarPermisosUsuarioId = usuarioId;
            GuardarPermisosIds = permisosId;
            LastUnidad = unidad;
        }
    }

    internal sealed class SpyUsuarioEstadoBLL : IUsuarioEstadoBLL
    {
        public Dictionary<int, UsuarioHistorialSnapshot> Versiones { get; } = new Dictionary<int, UsuarioHistorialSnapshot>();

        public int? GuardarUsuarioId { get; private set; }

        public string GuardarAccion { get; private set; }

        public string GuardarObservaciones { get; private set; }

        public int? AplicarSnapshotUsuarioId { get; private set; }

        public IUnidadDeTrabajo LastUnidad { get; private set; }

        public int GuardarVersionActual(int usuarioId, string accion, string observaciones)
        {
            GuardarUsuarioId = usuarioId;
            GuardarAccion = accion;
            GuardarObservaciones = observaciones;
            return 1;
        }

        public int GuardarVersionActualInterno(int usuarioId, string accion, string observaciones, IUnidadDeTrabajo unidad)
        {
            GuardarUsuarioId = usuarioId;
            GuardarAccion = accion;
            GuardarObservaciones = observaciones;
            LastUnidad = unidad;
            return 1;
        }

        public List<UsuarioHistorialSnapshot> ObtenerHistorial(int usuarioId)
        {
            return Versiones.Values.Where(item => item.UsuarioId == usuarioId).ToList();
        }

        public UsuarioHistorialSnapshot ObtenerVersion(int historialId)
        {
            UsuarioHistorialSnapshot snapshot;
            return Versiones.TryGetValue(historialId, out snapshot) ? snapshot : null;
        }

        public List<ResultadoComparacionEstado> CompararVersiones(UsuarioHistorialSnapshot versionActual, UsuarioHistorialSnapshot versionAnterior)
        {
            return new List<ResultadoComparacionEstado>();
        }

        public void AplicarSnapshotInterno(UsuarioHistorialSnapshot snapshot, IUnidadDeTrabajo unidad)
        {
            AplicarSnapshotUsuarioId = snapshot == null ? (int?)null : snapshot.UsuarioId;
            LastUnidad = unidad;
        }
    }

    internal sealed class SpyIntegridadBLL : IIntegridadBLL
    {
        public string NextDvh { get; set; } = "DVH";

        public List<Tuple<string, string, string, IUnidadDeTrabajo>> ActualizarDvvCalls { get; } = new List<Tuple<string, string, string, IUnidadDeTrabajo>>();

        public string CalcularDVH<T>(T entidad) where T : class
        {
            Usuario usuario = entidad as Usuario;
            return usuario == null ? NextDvh : "DVH:" + usuario.Username + ":" + usuario.PasswordHash + ":" + usuario.Activo;
        }

        public void RecalcularDVVInterno(string tabla, IEnumerable<string> listaDVH, IUnidadDeTrabajo unidad)
        {
        }

        public void ActualizarDVVInterno(string tabla, string dvhAnterior, string dvhNuevo, IUnidadDeTrabajo unidad)
        {
            ActualizarDvvCalls.Add(Tuple.Create(tabla, dvhAnterior, dvhNuevo, unidad));
        }

        public void ValidarIntegridadGlobal()
        {
        }
    }

    internal sealed class FakeIntegridadService : IIntegridadService
    {
        public string NextDvh { get; set; } = "DVH";

        public string NextDvv { get; set; } = "DVV";

        public string NextIncrementalDvv { get; set; } = "DVV-INCREMENTAL";

        public object LastEntity { get; private set; }

        public IEnumerable<string> LastDvhList { get; private set; }

        public Tuple<string, string, string> LastIncrementalArgs { get; private set; }

        public string CalcularDVH<T>(T entidad) where T : class
        {
            LastEntity = entidad;
            Usuario usuario = entidad as Usuario;
            return usuario == null ? NextDvh : "DVH:" + usuario.Username + ":" + usuario.PasswordHash + ":" + usuario.Activo;
        }

        public string CalcularDVV(IEnumerable<string> listaDVH)
        {
            LastDvhList = listaDVH == null ? null : listaDVH.ToList();
            return NextDvv;
        }

        public string CalcularDVVIncremental(string dvvActual, string dvhAnterior, string dvhNuevo)
        {
            LastIncrementalArgs = Tuple.Create(dvvActual, dvhAnterior, dvhNuevo);
            return NextIncrementalDvv;
        }
    }

    internal sealed class FakeObservado : IObservado
    {
        public IIdioma IdiomaActual { get; private set; }

        public int CambiarIdiomaCallCount { get; private set; }

        public void RegistrarObservador(IObservador observador)
        {
        }

        public void DesregistrarObservador(IObservador observador)
        {
        }

        public void Notificar()
        {
        }

        public void CambiarIdioma(IIdioma idioma)
        {
            CambiarIdiomaCallCount++;
            IdiomaActual = idioma;
        }
    }

    internal sealed class BitacoraCall
    {
        public BitacoraCall(string kind, string actividad, string informacion, string origen, IUnidadDeTrabajo unidad)
        {
            Kind = kind;
            Actividad = actividad;
            Informacion = informacion;
            Origen = origen;
            Unidad = unidad;
        }

        public string Kind { get; private set; }

        public string Actividad { get; private set; }

        public string Informacion { get; private set; }

        public string Origen { get; private set; }

        public IUnidadDeTrabajo Unidad { get; private set; }
    }
}
