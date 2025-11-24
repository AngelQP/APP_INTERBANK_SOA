using APP_INTERBANK_SOA.DTO.Ganoza_Sebastian;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace APP_INTERBANK_SOA.Servicios.Interfaces
{
    public interface CuentaSaldo
    {
        Task<IEnumerable<SaldoDTO>> ListarSaldosPorUsuarioAsync(int idUsuario);
        Task<SaldoDTO?> ObtenerSaldoCuentaAsync(int idCuenta);
    }
}