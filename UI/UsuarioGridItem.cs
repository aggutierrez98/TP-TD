namespace TpIDS
{
    internal sealed class UsuarioGridItem
    {
        [GridColumn("FormUserManagement.Grid.Id", Order = 0, Width = 50)]
        public int Id { get; set; }

        [GridColumn("FormUserManagement.Grid.Username", Order = 1)]
        public string Username { get; set; }

        [GridColumn("FormUserManagement.Grid.Active", Order = 2, Width = 65)]
        public bool Activo { get; set; }
    }
}
