namespace APP_INTERBANK_SOA.DTO.Quispe_Angel.Administrador
{
    public class AdminLoginResponseDto
    {
        public string Token { get; set; } = null!;
        public DateTime ExpiraEn { get; set; }

        public int IdUsuario { get; set; }
        public string NombreCompleto { get; set; } = null!;
        public string Correo { get; set; } = null!;
        public string Rol { get; set; } = null!;
    }
}
