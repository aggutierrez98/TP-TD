using System;

namespace TpIDS
{
    internal sealed class HistorialUsuarioGridItem
    {
        [GridColumn("FormHistorialUsuario.Grid.HistorialId", Order = 0)]
        public int HistorialId { get; set; }

        [GridColumn("FormHistorialUsuario.Grid.UsuarioId", Order = 1)]
        public int UsuarioId { get; set; }

        [GridColumn("FormHistorialUsuario.Grid.FechaHora", Order = 2)]
        public DateTime FechaHora { get; set; }

        [GridColumn("FormHistorialUsuario.Grid.ActorUsuarioId", Order = 3)]
        public int? ActorUsuarioId { get; set; }

        [GridColumn("FormHistorialUsuario.Grid.ActorUsernameSnapshot", Order = 4)]
        public string ActorUsernameSnapshot { get; set; }

        [GridColumn("FormHistorialUsuario.Grid.Accion", Order = 5)]
        public string Accion { get; set; }

        [GridColumn("FormHistorialUsuario.Grid.Observaciones", Order = 6)]
        public string Observaciones { get; set; }

        [GridColumn("FormHistorialUsuario.Grid.Username", Order = 7)]
        public string Username { get; set; }

        [GridColumn("FormHistorialUsuario.Grid.Activo", Order = 8)]
        public bool Activo { get; set; }

        [GridColumn("FormHistorialUsuario.Grid.IdiomaId", Order = 9)]
        public int? IdiomaId { get; set; }
    }
}
