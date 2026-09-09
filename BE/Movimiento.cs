using System;
using ABSTRACCIONES;

namespace BE
{
    public class Movimiento
    {
        public int Id { get; set; }

        public int UsuarioId { get; set; }

        public int CategoriaId { get; set; }

        public TipoMovimiento Tipo { get; set; }

        public decimal Monto { get; set; }

        public DateTime Fecha { get; set; }

        public string Descripcion { get; set; }

        public DateTime FechaCreacion { get; set; }
    }
}
