using System.ComponentModel.DataAnnotations;

namespace APP_INTERBANK_SOA.DTO.Quispe_Angel.Administrador
{
        public class UpdateAdminDto
        {
            [StringLength(100)]
            public string? Nombre { get; set; }

            [StringLength(100)]
            public string? Apellido { get; set; }

            [EmailAddress]
            [StringLength(255)]
            public string? Correo { get; set; }

            [StringLength(20)]
            public string? TipoDocumento { get; set; }   

            [StringLength(20)]
            public string? NumeroDocumento { get; set; }

            [Phone]
            [StringLength(20)]
            public string? Telefono { get; set; }

            [StringLength(20)]
            public string? Estado { get; set; }
     
        }
}
