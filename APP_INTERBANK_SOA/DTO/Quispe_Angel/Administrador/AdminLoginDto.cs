using System.ComponentModel.DataAnnotations;

namespace APP_INTERBANK_SOA.DTO.Quispe_Angel.Administrador
{
    public class AdminLoginDto
    {
        [Required]
        [EmailAddress]
        public string Correo { get; set; } = null!;

        [Required]
        public string Password { get; set; } = null!;
    }
}
