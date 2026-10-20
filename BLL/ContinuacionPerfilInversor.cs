using System;

namespace BLL
{
    public sealed class ContinuacionPerfilInversor
    {
        internal int UsuarioId { get; }
        internal DateTime CreadaUtc { get; }
        internal readonly object Sincronizacion = new object();
        internal bool Finalizada;
        internal ContinuacionPerfilInversor(int usuarioId, DateTime creadaUtc)
        {
            UsuarioId = usuarioId;
            CreadaUtc = creadaUtc;
        }
    }
}
