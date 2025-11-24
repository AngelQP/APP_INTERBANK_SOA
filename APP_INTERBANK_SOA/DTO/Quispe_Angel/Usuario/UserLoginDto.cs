using System.ComponentModel.DataAnnotations;

namespace APP_INTERBANK_SOA.DTO.Quispe_Angel.Usuario
{
    public class UserLoginDto
    {
        [Required]
        [StringLength(20)]
        public string NumeroDocumento { get; set; } = null!;  // DNI o carnet

        [Required]
        public string Password { get; set; } = null!;
    }
}
