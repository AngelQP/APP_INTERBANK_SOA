using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using APP_INTERBANK_SOA.DTO.Ganoza_Sebastian;
using APP_INTERBANK_SOA.Models;
using APP_INTERBANK_SOA.Servicios.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace APP_INTERBANK_SOA.Servicios.Implementaciones
{
    public class ICuentaSaldo : CuentaSaldo
    {
        private readonly InterbankContext _ctx;

        public ICuentaSaldo(InterbankContext ctx)
        {
            _ctx = ctx;
        }

        // LISTAR SALDOS DEL USUARIO
        public async Task<IEnumerable<SaldoDTO>> ListarSaldosPorUsuarioAsync(int idUsuario)
        {
            return await _ctx.Cuenta
                .Where(c => c.IdUsuario == idUsuario && c.Estado == "ACTIVO")
                .Select(c => new SaldoDTO
                {
                    IdCuenta = c.IdCuenta,
                    NumeroCuenta = c.NumeroCuenta,
                    TipoCuenta = c.TipoCuenta,
                    SaldoDisponible = c.SaldoDisponible,
                    Moneda = c.Moneda,
                    Estado = c.Estado
                })
                .ToListAsync();
        }

        // OBTENER SALDO DE UNA CUENTA ESPECÍFICA
        public async Task<SaldoDTO?> ObtenerSaldoCuentaAsync(int idCuenta)
        {
            return await _ctx.Cuenta
                .Where(c => c.IdCuenta == idCuenta)
                .Select(c => new SaldoDTO
                {
                    IdCuenta = c.IdCuenta,
                    NumeroCuenta = c.NumeroCuenta,
                    TipoCuenta = c.TipoCuenta,
                    SaldoDisponible = c.SaldoDisponible,
                    Moneda = c.Moneda,
                    Estado = c.Estado
                })
                .FirstOrDefaultAsync();
        }
    }
}