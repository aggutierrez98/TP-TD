namespace SERVICIOS
{
    public class PermisoGrupo : PermisoNodo
    {
        public override bool EsGrupo
        {
            get { return true; }
        }
    }
}
