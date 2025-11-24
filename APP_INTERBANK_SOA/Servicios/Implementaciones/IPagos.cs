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
    public class IPagos : Pagos
    {
        private readonly InterbankContext _ctx;

        public IPagos(InterbankContext ctx)
        {
            _ctx = ctx;
        }

        // LISTAR HISTORIAL
        public async Task<IEnumerable<Pago>> ListarHistorialAsync()
        {
            return await _ctx.Pagos
                .OrderByDescending(p => p.Fecha)
                .ToListAsync();
        }

        // OBTENER DETALLE
        public async Task<Pago?> ObtenerDetalleAsync(int idPago)
        {
            return await _ctx.Pagos
                .FirstOrDefaultAsync(p => p.IdPago == idPago);
        }

        // CREAR PAGO O RECARGA
        public async Task<(bool success, int? idPago, string message)> CrearPagoAsync(CrearPagoDTO dto)
        {
            using var tx = await _ctx.Database.BeginTransactionAsync();

            try
            {
                var cuenta = await _ctx.Cuenta.FirstOrDefaultAsync(c => c.IdCuenta == dto.IdCuenta);

                if (cuenta == null)
                    return (false, null, "La cuenta no existe.");

                if (cuenta.SaldoDisponible < dto.Monto)
                    return (false, null, "Saldo insuficiente.");

                cuenta.SaldoDisponible -= dto.Monto;
                _ctx.Cuenta.Update(cuenta);

                var pago = new Pago
                {
                    TipoPago = dto.TipoPago,
                    Monto = dto.Monto,
                    Fecha = dto.FechaProgramada ?? DateTime.Now,
                    Estado = dto.FechaProgramada.HasValue ? "PENDIENTE" : "COMPLETADO",
                    IdCuenta = dto.IdCuenta
                };

                _ctx.Pagos.Add(pago);
                await _ctx.SaveChangesAsync();

                await tx.CommitAsync();
                return (true, pago.IdPago, "Pago creado correctamente.");
            }
            catch (Exception ex)
            {
                await tx.RollbackAsync();
                return (false, null, $"Error: {ex.Message}");
            }
        }

        // ACTUALIZAR
        public async Task<bool> ActualizarPagoProgramadoAsync(int idPago, ActualizarPagoProgramadoDTO dto)
        {
            var pago = await _ctx.Pagos
                .FirstOrDefaultAsync(p => p.IdPago == idPago && p.Estado == "PENDIENTE");

            if (pago == null)
                return false;

            if (dto.Monto.HasValue) pago.Monto = dto.Monto.Value;
            if (dto.FechaProgramada.HasValue) pago.Fecha = dto.FechaProgramada.Value;
            if (!string.IsNullOrEmpty(dto.Descripcion)) pago.TipoPago = dto.Descripcion;

            _ctx.Pagos.Update(pago);
            await _ctx.SaveChangesAsync();
            return true;
        }

        // ELIMINAR
        public async Task<bool> EliminarPagoProgramadoAsync(int idPago)
        {
            var pago = await _ctx.Pagos
                .FirstOrDefaultAsync(p => p.IdPago == idPago && p.Estado == "PENDIENTE");

            if (pago == null)
                return false;

            _ctx.Pagos.Remove(pago);
            await _ctx.SaveChangesAsync();
            return true;
        }
    }
}
