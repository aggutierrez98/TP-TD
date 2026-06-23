using System;

namespace REPOSITORIO
{
    public interface IUnidadDeTrabajo : IDisposable
    {
        void Begin();

        void Commit();

        void Rollback();
    }
}
