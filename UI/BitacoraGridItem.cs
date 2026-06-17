using System;

namespace TpIDS
{
    internal sealed class BitacoraGridItem
    {
        [GridColumn("FormAuditLog.Grid.Id", Order = 0, Width = 60)]
        public int Id { get; set; }

        [GridColumn("FormAuditLog.Grid.DateTime", Order = 1)]
        public DateTime FechaHora { get; set; }

        [GridColumn("FormAuditLog.Grid.UserId", Order = 2)]
        public int? UsuarioId { get; set; }

        [GridColumn("FormAuditLog.Grid.UsernameSnapshot", Order = 3)]
        public string UsernameSnapshot { get; set; }

        [GridColumn("FormAuditLog.Grid.Activity", Order = 4, TraducirValor = true)]
        public string Actividad { get; set; }

        [GridColumn("FormAuditLog.Grid.Information", Order = 5, FillWeight = 180F, TraducirValor = true)]
        public string Informacion { get; set; }

        [GridColumn("FormAuditLog.Grid.Level", Order = 6, TraducirValor = true)]
        public string Nivel { get; set; }

        [GridColumn("FormAuditLog.Grid.Source", Order = 7)]
        public string Origen { get; set; }

        [GridColumn("FormAuditLog.Grid.IsTechnical", Order = 8)]
        public bool EsTecnica { get; set; }
    }
}