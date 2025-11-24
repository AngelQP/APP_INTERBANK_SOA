using System;
using System.ComponentModel.DataAnnotations;

namespace APP_INTERBANK_SOA.DTO.Ganoza_Sebastian

{
    public class CrearTransferenciaDTO
    {
        [Required]
        public int IdCuentaOrigen { get; set; }

        public int? IdCuentaDestino { get; set; }
        public string? CuentaDestinoExterna { get; set; }
        public string? BancoDestino { get; set; }
        public string? NombreDestinatario { get; set; }

        [Required]
        public decimal Monto { get; set; }

        [Required]
        public string TipoTransferencia { get; set; } = string.Empty; // INMEDIATA / PROGRAMADA

        public DateTime? FechaProgramada { get; set; }
        public string? Concepto { get; set; }
    }
}

