using APP_INTERBANK_SOA.DTO.Ganoza_Sebastian;
using APP_INTERBANK_SOA.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace APP_INTERBANK_SOA.Servicios.Interfaces
{
    public interface Transferencia
    {
        Task<IEnumerable<Transferencium>> ListarHistorialAsync();
        Task<Transferencium?> ObtenerTransferenciaAsync(int idTransferencia);
        Task<(bool success, int? idTransferencia, string message)> CrearTransferenciaAsync(CrearTransferenciaDTO dto);
        Task<bool> ActualizarProgramadaAsync(int idTransferencia, ActualizarTransferenciaDTO dto);
        Task<bool> EliminarProgramadaAsync(int idTransferencia);
    }
}
