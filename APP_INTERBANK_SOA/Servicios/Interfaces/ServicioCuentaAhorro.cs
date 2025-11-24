using APP_INTERBANK_SOA.DTO.Villalobos_Jhon;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace APP_INTERBANK_SOA.Servicios.Interfaces
{
    public interface ServicioCuentaAhorro
    {
        Task<IEnumerable<TipoCuentaDTO>> ListAvailableSavingTypesAsync();
        Task<IEnumerable<RequisitoDto>> GetRequirementsForTypeAsync(int idTipoCuenta);
        Task<(bool Success, int? IdCuenta, string Message)> CreateSavingAccountAsync(CrearCuentaAhorroDto dto);
        Task<bool> UpdateAccountAsync(int idCuenta, GuardarDetalleProductoDTO updateDto);
        Task<bool> DeleteAccountAsync(int idCuenta);
    }
}
