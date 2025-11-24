using System;
using System.ComponentModel.DataAnnotations;

namespace APP_INTERBANK_SOA.DTO.Ganoza_Sebastian

{
    public class ActualizarTransferenciaDTO
    {
        public DateTime? FechaProgramada { get; set; }
        public decimal? Monto { get; set; }
        public string? Concepto { get; set; }
    }
}
