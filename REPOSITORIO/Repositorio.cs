using System.Collections.Generic;

namespace REPOSITORIO
{
    public abstract class Repositorio<T>
    {
        public abstract IList<T> ObtenerTodos(IUnidadDeTrabajo unidad = null);

        public abstract T Guardar(T entidad, IUnidadDeTrabajo unidad = null);

        public abstract T Actualizar(T entidad, IUnidadDeTrabajo unidad = null);

        public abstract void Eliminar(T entidad, IUnidadDeTrabajo unidad = null);
    }
}
