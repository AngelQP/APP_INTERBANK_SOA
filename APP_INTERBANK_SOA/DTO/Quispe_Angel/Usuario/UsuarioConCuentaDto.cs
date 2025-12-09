namespace APP_INTERBANK_SOA.DTO.Quispe_Angel.Usuario
{
    public class UsuarioConCuentaDto
    {
        public int id { get; set; }
        public string Nombre { get; set; } = null!;
        public string Apellido { get; set; } = null!;
        public string Correo { get; set; } = null!;
        public string TipoDocumento { get; set; } = null!;
        public string NumeroDocumento { get; set; } = null!;
        public string? Telefono { get; set; }
        public string Estado{ get; set; } = null!;
        public DateTime FechaRegistro { get; set; }

        // Datos de la cuenta creada
        public int IdCuenta { get; set; }
        public string NumeroCuenta { get; set; } = null!;
        public string TipoCuenta { get; set; } = null!;
        public string Moneda { get; set; } = null!;
        public decimal SaldoDisponible { get; set; }
        public string EstadoCuenta { get; set; } = null!;
    }
}
