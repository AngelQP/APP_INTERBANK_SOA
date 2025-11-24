using APP_INTERBANK_SOA.DTO.Villalobos_Jhon;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace APP_INTERBANK_SOA.Servicios.Interfaces
{
    public interface ServicioMovimiento
    {
        Task<IEnumerable<MovimientoDTO>> ListMovementsAsync(int idCuenta);
        Task<IEnumerable<MovimientoDTO>> ListMovementsByDateRangeAsync(int idCuenta, DateTime desde, DateTime hasta);
    }
}
