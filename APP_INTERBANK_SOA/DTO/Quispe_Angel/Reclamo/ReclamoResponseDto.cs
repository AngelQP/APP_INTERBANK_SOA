namespace APP_INTERBANK_SOA.DTO.Quispe_Angel.Reclamo
{
    public class ReclamoResponseDto
    {
        public int IdReclamo { get; set; }
        public string TipoReclamo { get; set; } = null!;
        public string Descripcion { get; set; } = null!;
        public string Estado { get; set; } = null!;
        public DateTime FechaCreacion { get; set; }
    }
}
