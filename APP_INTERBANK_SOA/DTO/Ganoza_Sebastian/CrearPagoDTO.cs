using System;
using System.ComponentModel.DataAnnotations;

namespace APP_INTERBANK_SOA.DTO.Ganoza_Sebastian
{
    public class CrearPagoDTO
    {
        [Required]
        public int IdCuenta { get; set; }

        [Required]
        public string TipoPago { get; set; } = string.Empty; // SERVICIO / RECARGA

        [Required]
        public decimal Monto { get; set; }

        public DateTime? FechaProgramada { get; set; }

        public string? Descripcion { get; set; }
    }
}

