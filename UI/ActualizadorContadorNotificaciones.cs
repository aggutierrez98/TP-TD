using System;
using System.Threading.Tasks;

namespace TpIDS
{
    internal sealed class ActualizadorContadorNotificaciones
    {
        private readonly Func<int?> sesion;
        private readonly Func<Task<int>> consultar;
        private readonly Action<int> publicar;
        private readonly Action<Exception> fallo;
        private bool enCurso, pendiente, detenido;
        private int version;

        public ActualizadorContadorNotificaciones(Func<int?> sesion, Func<Task<int>> consultar, Action<int> publicar, Action<Exception> fallo)
        {
            this.sesion = sesion;
            this.consultar = consultar;
            this.publicar = publicar;
            this.fallo = fallo;
        }

        // Se invoca exclusivamente desde el hilo UI; el await conserva ese contexto.
        public async Task ActualizarAsync(bool trasLectura = false)
        {
            if (detenido) return;
            if (trasLectura) version++;
            if (enCurso)
            {
                if (trasLectura) pendiente = true;
                return;
            }
            enCurso = true;
            try
            {
                do
                {
                    pendiente = false;
                    int? usuario = sesion();
                    if (!usuario.HasValue || detenido) return;
                    int iniciada = version;
                    try
                    {
                        int cantidad = await consultar();
                        if (!detenido && iniciada == version && usuario == sesion()) publicar(cantidad);
                    }
                    catch (Exception ex)
                    {
                        if (!detenido && iniciada == version && usuario == sesion()) fallo(ex);
                    }
                } while (pendiente && !detenido);
            }
            finally { enCurso = false; }
        }

        public void Detener()
        {
            detenido = true;
            pendiente = false;
            version++;
        }
    }
}
