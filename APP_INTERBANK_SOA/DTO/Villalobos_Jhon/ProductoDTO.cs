using System.ComponentModel.DataAnnotations;

namespace APP_INTERBANK_SOA.DTO.Villalobos_Jhon
{
    public class ProductoDto
    {
        public int IdProducto { get; set; }
        public string Nombre { get; set; } = null!;
        public string Tipo { get; set; } = null!;
        public decimal? TasaInteres { get; set; }
        public int? Plazo { get; set; }
        public string Estado { get; set; } = null!;
    }

    public class CrearProductoFavoritoDto
    {
        [Required]
        public int IdUsuario { get; set; }
        [Required]
        public int IdProducto { get; set; }
    }
}
