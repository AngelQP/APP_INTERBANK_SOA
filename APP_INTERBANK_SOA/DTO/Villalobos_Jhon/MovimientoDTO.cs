using System;
using System.ComponentModel.DataAnnotations;

namespace APP_INTERBANK_SOA.DTO.Villalobos_Jhon
{
    public class MovimientoDTO
    {
        public int IdMovimiento { get; set; }
        public DateTime Fecha { get; set; }
        public string Tipo { get; set; } = null!;
        public decimal Monto { get; set; }
        public string? Descripcion { get; set; }
        public int IdCuenta { get; set; }
    }

    public class MovimientoDataDTO
    {
        [Required]
        public int IdCuenta { get; set; }

        public DateTime? Desde { get; set; }
        public DateTime? Hasta { get; set; }
    }
}
