using System;

namespace TpIDS
{
    internal sealed class HistorialUsuarioGridItem
    {
        [GridColumn("FormUserHistory.Grid.HistoryId", Order = 0)]
        public int HistorialId { get; set; }

        [GridColumn("FormUserHistory.Grid.UserId", Order = 1)]
        public int UsuarioId { get; set; }

        [GridColumn("FormUserHistory.Grid.DateTime", Order = 2)]
        public DateTime FechaHora { get; set; }

        [GridColumn("FormUserHistory.Grid.ActorUserId", Order = 3)]
        public int? ActorUsuarioId { get; set; }

        [GridColumn("FormUserHistory.Grid.ActorUsernameSnapshot", Order = 4)]
        public string ActorUsernameSnapshot { get; set; }

        [GridColumn("FormUserHistory.Grid.Action", Order = 5, TraducirValor = true)]
        public string Accion { get; set; }

        [GridColumn("FormUserHistory.Grid.Observations", Order = 6, TraducirValor = true)]
        public string Observaciones { get; set; }

        [GridColumn("FormUserHistory.Grid.Username", Order = 7)]
        public string Username { get; set; }

        [GridColumn("FormUserHistory.Grid.Active", Order = 8)]
        public bool Activo { get; set; }

        [GridColumn("FormUserHistory.Grid.LanguageId", Order = 9)]
        public int? IdiomaId { get; set; }
    }
}

