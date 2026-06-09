using System;

namespace TpIDS
{
    internal sealed class BitacoraGridItem
    {
        [GridColumn("FormBitacora.Grid.Id", Order = 0, Width = 60)]
        public int Id { get; set; }

        [GridColumn("FormBitacora.Grid.FechaHora", Order = 1)]
        public DateTime FechaHora { get; set; }

        [GridColumn("FormBitacora.Grid.UsuarioId", Order = 2)]
        public int? UsuarioId { get; set; }

        [GridColumn("FormBitacora.Grid.UsernameSnapshot", Order = 3)]
        public string UsernameSnapshot { get; set; }

        [GridColumn("FormBitacora.Grid.Actividad", Order = 4)]
        public string Actividad { get; set; }

        [GridColumn("FormBitacora.Grid.Informacion", Order = 5, FillWeight = 180F)]
        public string Informacion { get; set; }

        [GridColumn("FormBitacora.Grid.Nivel", Order = 6)]
        public string Nivel { get; set; }

        [GridColumn("FormBitacora.Grid.Origen", Order = 7)]
        public string Origen { get; set; }

        [GridColumn("FormBitacora.Grid.EsTecnica", Order = 8)]
        public bool EsTecnica { get; set; }
    }
}
