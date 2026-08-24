namespace REPOSITORIO
{
    public class UnidadDeTrabajoFactory : IUnidadDeTrabajoFactory
    {
        public IUnidadDeTrabajo Crear()
        {
            return new UnidadDeTrabajoSqlServer();
        }
    }
}
