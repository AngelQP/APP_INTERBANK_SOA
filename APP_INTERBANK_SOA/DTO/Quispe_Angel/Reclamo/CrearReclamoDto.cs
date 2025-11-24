using System.ComponentModel.DataAnnotations;

namespace APP_INTERBANK_SOA.DTO.Quispe_Angel.Reclamo
{
    public class CrearReclamoDto
    {
        [Required]
        [MaxLength(50)]
        public string TipoReclamo { get; set; } = null!;   // CONSULTA, SUGERENCIA, RECLAMO

        [Required]
        public string Descripcion { get; set; } = null!;
    }
}
