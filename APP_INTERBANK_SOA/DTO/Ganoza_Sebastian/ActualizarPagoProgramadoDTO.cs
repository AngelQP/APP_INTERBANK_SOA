using System;
using System.ComponentModel.DataAnnotations;

namespace APP_INTERBANK_SOA.DTO.Ganoza_Sebastian
{
    public class ActualizarPagoProgramadoDTO
    {
        public decimal? Monto { get; set; }
        public DateTime? FechaProgramada { get; set; }
        public string? Descripcion { get; set; }
    }
}
