using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using APP_INTERBANK_SOA.DTO.Villalobos_Jhon;
using APP_INTERBANK_SOA.Models;
using APP_INTERBANK_SOA.Servicios.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace APP_INTERBANK_SOA.Servicios.Implementaciones
{
    public class IGuardar : ServicioGuardar
    {
        private readonly InterbankContext _ctx;
        public IGuardar(InterbankContext ctx) { _ctx = ctx; }

        public async Task<IEnumerable<GuardarProductoDTO>> ListSavingsByUserAsync(int idUsuario)
        {
            var cuentas = await _ctx.Cuenta
                .Where(c => c.IdUsuario == idUsuario)
                .Include(c => c.DepositoPlazos)
                .ToListAsync();

            var depositos = cuentas.SelectMany(c => c.DepositoPlazos).ToList();

            if (depositos.Count == 0) return Enumerable.Empty<GuardarProductoDTO>();

            return depositos.Select(d => new GuardarProductoDTO
            {
                IdDeposito = d.IdDeposito,
                Monto = d.Monto,
                PlazoDias = d.PlazoDias,
                TasaAnual = d.TasaAnual,
                FechaApertura = d.FechaApertura,
                Estado = d.Estado,
                IdCuenta = d.IdCuenta
            });
        }

        public async Task<GuardarProductoDTO?> GetSavingDetailAsync(int idDeposito)
        {
            var d = await _ctx.DepositoPlazos.FindAsync(idDeposito);
            if (d == null) return null;

            return new GuardarProductoDTO
            {
                IdDeposito = d.IdDeposito,
                Monto = d.Monto,
                PlazoDias = d.PlazoDias,
                TasaAnual = d.TasaAnual,
                FechaApertura = d.FechaApertura,
                Estado = d.Estado,
                IdCuenta = d.IdCuenta
            };
        }
    }
}
