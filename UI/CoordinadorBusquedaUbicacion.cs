using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ABSTRACCIONES;

namespace TpIDS
{
    // Resultado de una consulta ya filtrado por vigencia: el formulario solo debe
    // reflejar los que no son Descartada. No depende de WinForms ni de GMap.NET
    // para poder verificarse con un servicio falso.
    internal enum TipoResultadoBusqueda
    {
        // La consulta no se envio (ya habia una en curso o el coordinador esta detenido).
        Ignorada,
        // Edicion, cambio de idioma o cierre invalidaron la consulta: no tocar controles.
        Descartada,
        // El servicio cancelo por su cuenta sin que el usuario invalidara: sin mensaje de error.
        Cancelada,
        Coincidencias,
        SinCoincidencias,
        Error
    }

    internal sealed class ResultadoCoordinado
    {
        private ResultadoCoordinado(TipoResultadoBusqueda tipo, IReadOnlyList<ResultadoBusquedaUbicacion> coincidencias, EstadoBusquedaUbicacion? estado)
        {
            Tipo = tipo;
            Coincidencias = coincidencias;
            Estado = estado;
        }

        public TipoResultadoBusqueda Tipo { get; }

        public IReadOnlyList<ResultadoBusquedaUbicacion> Coincidencias { get; }

        public EstadoBusquedaUbicacion? Estado { get; }

        public static ResultadoCoordinado De(TipoResultadoBusqueda tipo)
        {
            return new ResultadoCoordinado(tipo, null, null);
        }

        public static ResultadoCoordinado ConCoincidencias(IReadOnlyList<ResultadoBusquedaUbicacion> coincidencias)
        {
            return new ResultadoCoordinado(TipoResultadoBusqueda.Coincidencias, coincidencias, null);
        }

        public static ResultadoCoordinado ConError(EstadoBusquedaUbicacion estado)
        {
            return new ResultadoCoordinado(TipoResultadoBusqueda.Error, null, estado);
        }
    }

    // Administra el ciclo de vida de la consulta del selector: un CancellationTokenSource
    // y una version por consulta, descarte de respuestas obsoletas e impedimento de envios
    // dobles. Se usa solo desde el hilo UI; el await conserva su contexto, asi que el
    // formulario puede actualizar controles con el resultado devuelto.
    internal sealed class CoordinadorBusquedaUbicacion : IDisposable
    {
        private readonly IBusquedaUbicacionesService _servicio;
        private CancellationTokenSource _cancelacion;
        private int _version;
        private bool _detenido;

        public CoordinadorBusquedaUbicacion(IBusquedaUbicacionesService servicio)
        {
            _servicio = servicio;
        }

        // True mientras la consulta vigente no termino. Invalidar() lo apaga de inmediato.
        public bool EnCurso { get; private set; }

        public int Version => _version;

        public async Task<ResultadoCoordinado> BuscarAsync(ConsultaUbicacion consulta)
        {
            if (_detenido || EnCurso || _servicio == null)
            {
                return ResultadoCoordinado.De(TipoResultadoBusqueda.Ignorada);
            }

            int version = ++_version;
            CancellationTokenSource cancelacion = new CancellationTokenSource();
            _cancelacion = cancelacion;
            EnCurso = true;
            try
            {
                IReadOnlyList<ResultadoBusquedaUbicacion> lista = await _servicio.BuscarAsync(consulta, cancelacion.Token);
                if (version != _version)
                {
                    return ResultadoCoordinado.De(TipoResultadoBusqueda.Descartada);
                }

                return lista == null || lista.Count == 0
                    ? ResultadoCoordinado.De(TipoResultadoBusqueda.SinCoincidencias)
                    : ResultadoCoordinado.ConCoincidencias(lista);
            }
            catch (OperationCanceledException)
            {
                return ResultadoCoordinado.De(version != _version ? TipoResultadoBusqueda.Descartada : TipoResultadoBusqueda.Cancelada);
            }
            catch (BusquedaUbicacionException ex)
            {
                return version != _version
                    ? ResultadoCoordinado.De(TipoResultadoBusqueda.Descartada)
                    : ResultadoCoordinado.ConError(ex.Estado);
            }
            catch (Exception)
            {
                // Un fallo no previsto no debe escapar de un handler async: se informa como falla de conexion.
                return version != _version
                    ? ResultadoCoordinado.De(TipoResultadoBusqueda.Descartada)
                    : ResultadoCoordinado.ConError(EstadoBusquedaUbicacion.ConexionFallida);
            }
            finally
            {
                // Solo la consulta vigente restaura el estado; una invalidada ya lo hizo Invalidar().
                if (version == _version)
                {
                    EnCurso = false;
                    _cancelacion = null;
                }

                cancelacion.Dispose();
            }
        }

        // Edicion de texto, cambio de idioma, aceptar, cancelar o cierre: cancela la consulta
        // pendiente (si hay) y descarta su respuesta aunque el transporte no observe la cancelacion.
        public void Invalidar()
        {
            _version++;
            EnCurso = false;
            CancellationTokenSource cancelacion = _cancelacion;
            _cancelacion = null;
            if (cancelacion != null)
            {
                try
                {
                    cancelacion.Cancel();
                }
                catch (ObjectDisposedException)
                {
                }
            }
        }

        public void Detener()
        {
            _detenido = true;
            Invalidar();
        }

        public void Dispose()
        {
            Detener();
        }
    }
}
