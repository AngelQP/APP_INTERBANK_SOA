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
    public class IMovimiento : ServicioMovimiento
    {
        private readonly InterbankContext _ctx;
        public IMovimiento(InterbankContext ctx) { _ctx = ctx; }

        public async Task<IEnumerable<MovimientoDTO>> ListMovementsAsync(int idCuenta)
        {
            var movs = await _ctx.Movimientos
                .Where(m => m.IdCuenta == idCuenta)
                .OrderByDescending(m => m.Fecha)
                .ToListAsync();

            if (movs == null || movs.Count == 0) return Enumerable.Empty<MovimientoDTO>();

            return movs.Select(m => new MovimientoDTO
            {
                IdMovimiento = m.IdMovimiento,
                Fecha = m.Fecha,
                Tipo = m.Tipo,
                Monto = m.Monto,
                Descripcion = m.Descripcion,
                IdCuenta = m.IdCuenta
            });
        }

        public async Task<IEnumerable<MovimientoDTO>> ListMovementsByDateRangeAsync(int idCuenta, DateTime desde, DateTime hasta)
        {
            if (desde > hasta) return Enumerable.Empty<MovimientoDTO>();

            var movs = await _ctx.Movimientos
                .Where(m => m.IdCuenta == idCuenta && m.Fecha >= desde && m.Fecha <= hasta)
                .OrderByDescending(m => m.Fecha)
                .ToListAsync();

            if (movs == null || movs.Count == 0) return Enumerable.Empty<MovimientoDTO>();

            return movs.Select(m => new MovimientoDTO
            {
                IdMovimiento = m.IdMovimiento,
                Fecha = m.Fecha,
                Tipo = m.Tipo,
                Monto = m.Monto,
                Descripcion = m.Descripcion,
                IdCuenta = m.IdCuenta
            });
        }
    }
}
