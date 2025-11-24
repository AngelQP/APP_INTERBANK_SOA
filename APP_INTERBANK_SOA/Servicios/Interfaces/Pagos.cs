using APP_INTERBANK_SOA.Models;
using APP_INTERBANK_SOA.DTO.Ganoza_Sebastian;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace APP_INTERBANK_SOA.Servicios.Interfaces
{
    public interface Pagos
    {
        Task<IEnumerable<Pago>> ListarHistorialAsync();
        Task<Pago?> ObtenerDetalleAsync(int idPago);
        Task<(bool success, int? idPago, string message)> CrearPagoAsync(CrearPagoDTO dto);
        Task<bool> ActualizarPagoProgramadoAsync(int idPago, ActualizarPagoProgramadoDTO dto);
        Task<bool> EliminarPagoProgramadoAsync(int idPago);
    }
}
