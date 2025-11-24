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
    public class ITransferencia : Transferencia
    {
        private readonly InterbankContext _ctx;

        public ITransferencia(InterbankContext ctx)
        {
            _ctx = ctx;
        }

        // LISTAR HISTORIAL
        public async Task<IEnumerable<Transferencium>> ListarHistorialAsync()
        {
            return await _ctx.Transferencia
                .OrderByDescending(t => t.Fecha)
                .ToListAsync();
        }

        // OBTENER DETALLE
        public async Task<Transferencium?> ObtenerTransferenciaAsync(int idTransferencia)
        {
            return await _ctx.Transferencia
                .FirstOrDefaultAsync(t => t.IdTransferencia == idTransferencia);
        }

        // CREAR TRANSFERENCIA (CON TRANSACCIÓN)
        public async Task<(bool success, int? idTransferencia, string message)> CrearTransferenciaAsync(CrearTransferenciaDTO dto)
        {
            using var tx = await _ctx.Database.BeginTransactionAsync();

            try
            {
                // Validar cuenta origen
                var origen = await _ctx.Cuenta
                    .FirstOrDefaultAsync(c => c.IdCuenta == dto.IdCuentaOrigen);

                if (origen == null)
                    return (false, null, "La cuenta origen no existe.");

                if (origen.SaldoDisponible < dto.Monto)
                    return (false, null, "Saldo insuficiente en la cuenta origen.");

                // Restar saldo origen
                origen.SaldoDisponible -= dto.Monto;
                _ctx.Cuenta.Update(origen);

                // Si es destino interno → sumar saldo
                if (dto.IdCuentaDestino.HasValue)
                {
                    var destino = await _ctx.Cuenta
                        .FirstOrDefaultAsync(c => c.IdCuenta == dto.IdCuentaDestino);

                    if (destino == null)
                        return (false, null, "La cuenta destino no existe.");

                    destino.SaldoDisponible += dto.Monto;
                    _ctx.Cuenta.Update(destino);
                }

                // Crear transferencia
                var transferencia = new Transferencium
                {
                    Monto = dto.Monto,
                    Fecha = dto.FechaProgramada ?? DateTime.Now,
                    TipoTransferencia = dto.TipoTransferencia,
                    IdCuentaOrigen = dto.IdCuentaOrigen,
                    IdCuentaDestino = dto.IdCuentaDestino,
                    CuentaDestinoExterna = dto.CuentaDestinoExterna,
                    BancoDestino = dto.BancoDestino,
                    NombreDestinatario = dto.NombreDestinatario,
                    Concepto = dto.Concepto,
                    Comision = 0,
                    Estado = "COMPLETADO"
                };

                _ctx.Transferencia.Add(transferencia);
                await _ctx.SaveChangesAsync();

                await tx.CommitAsync();
                return (true, transferencia.IdTransferencia, "Transferencia realizada correctamente.");
            }
            catch (Exception ex)
            {
                await tx.RollbackAsync();
                return (false, null, $"Error en transferencia: {ex.Message}");
            }
        }

        // ACTUALIZAR TRANSFERENCIA PROGRAMADA
        public async Task<bool> ActualizarProgramadaAsync(int idTransferencia, ActualizarTransferenciaDTO dto)
        {
            var t = await _ctx.Transferencia
                .FirstOrDefaultAsync(x => x.IdTransferencia == idTransferencia &&
                                          x.TipoTransferencia == "PROGRAMADA");

            if (t == null) return false;

            if (dto.FechaProgramada.HasValue) t.Fecha = dto.FechaProgramada.Value;
            if (dto.Monto.HasValue) t.Monto = dto.Monto.Value;
            if (!string.IsNullOrEmpty(dto.Concepto)) t.Concepto = dto.Concepto;

            _ctx.Transferencia.Update(t);
            await _ctx.SaveChangesAsync();
            return true;
        }

        // ELIMINAR TRANSFERENCIA PROGRAMADA
        public async Task<bool> EliminarProgramadaAsync(int idTransferencia)
        {
            var t = await _ctx.Transferencia
                .FirstOrDefaultAsync(x => x.IdTransferencia == idTransferencia &&
                                          x.TipoTransferencia == "PROGRAMADA");

            if (t == null) return false;

            _ctx.Transferencia.Remove(t);
            await _ctx.SaveChangesAsync();
            return true;
        }
    }
}
