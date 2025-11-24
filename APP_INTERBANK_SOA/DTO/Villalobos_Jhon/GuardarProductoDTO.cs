using System;
using System.ComponentModel.DataAnnotations;

namespace APP_INTERBANK_SOA.DTO.Villalobos_Jhon
{
    public class GuardarProductoDTO
    {
        public int IdDeposito { get; set; }
        public decimal Monto { get; set; }
        public int PlazoDias { get; set; }
        public decimal TasaAnual { get; set; }
        public DateTime FechaApertura { get; set; }
        public string Estado { get; set; } = null!;
        public int IdCuenta { get; set; }
    }

    public class GuardarDetalleProductoDTO
    {
        public int IdCuenta { get; set; }
        public string NumeroCuenta { get; set; } = null!;
        public string TipoCuenta { get; set; } = null!;
        public decimal SaldoDisponible { get; set; }
        public string Moneda { get; set; } = null!;
        public DateTime FechaCreacion { get; set; }
    }
}
