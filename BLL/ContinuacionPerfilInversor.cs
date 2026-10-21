using System;
using BE;

namespace BLL
{
    public sealed class ContinuacionPerfilInversor
    {
        internal Usuario Usuario { get; }
        internal DateTime CreadaUtc { get; }
        internal readonly object Sincronizacion = new object();
        internal bool Finalizada;
        internal ContinuacionPerfilInversor(Usuario usuario, DateTime creadaUtc)
        {
            Usuario = usuario;
            CreadaUtc = creadaUtc;
        }
    }
}
