using System;
using System.ComponentModel.DataAnnotations;

namespace APP_INTERBANK_SOA.DTO.Ganoza_Sebastian
{
    public class SaldoDTO
    {
        public int IdCuenta { get; set; }
        public string NumeroCuenta { get; set; } = string.Empty;
        public string TipoCuenta { get; set; } = string.Empty;
        public decimal SaldoDisponible { get; set; }
        public string Moneda { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
    }
}
