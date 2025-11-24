using System.ComponentModel.DataAnnotations;

namespace APP_INTERBANK_SOA.DTO.Villalobos_Jhon
{
    public class TipoCuentaDTO
    {
        public int IdTipoCuenta { get; set; }
        public string Nombre { get; set; } = null!;
        public string? Descripcion { get; set; }
        public decimal TasaInteres { get; set; }
        public string Moneda { get; set; } = null!;
        public string Estado { get; set; } = null!;
    }

    public class RequisitoDto
    {
        public int IdRequisito { get; set; }
        public int IdTipoCuenta { get; set; }
        public string Descripcion { get; set; } = null!;
    }

    public class CrearCuentaAhorroDto
    {
        [Required]
        public int IdUsuario { get; set; }
        [Required]
        public int IdTipoCuenta { get; set; }
        [Required, StringLength(20)]
        public string NumeroCuenta { get; set; } = null!;
        [Range(0, double.MaxValue)]
        public decimal SaldoInicial { get; set; } = 0m;
    }
}
