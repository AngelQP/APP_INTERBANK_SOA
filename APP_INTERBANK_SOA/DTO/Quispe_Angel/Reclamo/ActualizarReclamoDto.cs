namespace APP_INTERBANK_SOA.DTO.Quispe_Angel.Reclamo
{
    public class ActualizarReclamoDto
    {
        // Opcionales: si vienen null, no se modifican
        public string? TipoReclamo { get; set; }      // CONSULTA, SUGERENCIA, RECLAMO
        public string? Descripcion { get; set; }
    }
}
