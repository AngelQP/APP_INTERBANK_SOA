namespace APP_INTERBANK_SOA.DTO.Quispe_Angel.Usuario
{
    public class UserLoginResponseDto
    {
        public string Token { get; set; } = null!;
        public DateTime ExpiraEn { get; set; }

        public int IdUsuario { get; set; }
        public string NombreCompleto { get; set; } = null!;
        public string NumeroDocumento { get; set; } = null!;
        public string TipoDocumento { get; set; } = null!;
        public string Correo { get; set; } = null!;
        public string Rol { get; set; } = null!;
        public string Estado { get; set; } = null!;
    }
}
