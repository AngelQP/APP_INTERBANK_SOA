using System.ComponentModel.DataAnnotations;

namespace APP_INTERBANK_SOA.DTO.Quispe_Angel.Usuario
{
    public class RegisterUsuarioConCuentaDto
    {
        [Required]
        [StringLength(100)]
        public string Nombre { get; set; } = null!;

        [Required]
        [StringLength(100)]
        public string Apellido { get; set; } = null!;

        [Required]
        [EmailAddress]
        [StringLength(255)]
        public string Correo { get; set; } = null!;

        [Required]
        [StringLength(100, MinimumLength = 6)]
        public string Password { get; set; } = null!;

        [Required]
        [StringLength(20)]
        public string TipoDocumento { get; set; } = null!;   // "DNI" o "CE"

        [Required]
        [StringLength(20)]
        public string NumeroDocumento { get; set; } = null!;

        [Phone]
        [StringLength(20)]
        public string Telefono { get; set; }

        // 🔹 Regla: número de cuenta obligatorio
        [Required]
        [StringLength(20)]
        public string NumeroCuenta { get; set; } = null!;
    }
}
