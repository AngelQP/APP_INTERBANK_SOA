using APP_INTERBANK_SOA.DTO.Villalobos_Jhon;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace APP_INTERBANK_SOA.Servicios.Interfaces
{
    public interface ServicioGuardar
    {
        Task<IEnumerable<GuardarProductoDTO>> ListSavingsByUserAsync(int idUsuario);
        Task<GuardarProductoDTO?> GetSavingDetailAsync(int idDeposito);
    }
}
