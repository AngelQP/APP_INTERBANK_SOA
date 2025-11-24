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
    public class ICuentaAhorro : ServicioCuentaAhorro
    {
        private readonly InterbankContext _ctx;
        public ICuentaAhorro(InterbankContext ctx) { _ctx = ctx; }

        public async Task<IEnumerable<TipoCuentaDTO>> ListAvailableSavingTypesAsync()
        {
            var tipos = await _ctx.TipoCuentaAhorros.Where(t => t.Estado == "ACTIVO").ToListAsync();
            if (!tipos.Any()) return Enumerable.Empty<TipoCuentaDTO>();
            return tipos.Select(t => new TipoCuentaDTO
            {
                IdTipoCuenta = t.IdTipoCuenta,
                Nombre = t.Nombre,
                Descripcion = t.Descripcion,
                TasaInteres = t.TasaInteres,
                Moneda = t.Moneda,
                Estado = t.Estado
            });
        }

        public async Task<IEnumerable<RequisitoDto>> GetRequirementsForTypeAsync(int idTipoCuenta)
        {
            var reqs = await _ctx.RequisitoCuentaAhorros.Where(r => r.IdTipoCuenta == idTipoCuenta).ToListAsync();
            if (!reqs.Any()) return Enumerable.Empty<RequisitoDto>();
            return reqs.Select(r => new RequisitoDto
            {
                IdRequisito = r.IdRequisito,
                IdTipoCuenta = r.IdTipoCuenta,
                Descripcion = r.Descripcion
            });
        }

        public async Task<(bool Success, int? IdCuenta, string Message)> CreateSavingAccountAsync(CrearCuentaAhorroDto dto)
        {
            // Validaciones básicas
            var tipo = await _ctx.TipoCuentaAhorros.FindAsync(dto.IdTipoCuenta);
            var usuario = await _ctx.Usuarios.FindAsync(dto.IdUsuario);
            if (tipo == null) return (false, null, "Tipo de cuenta no existe");
            if (usuario == null) return (false, null, "Usuario no existe");

            // Validar número único
            var exists = await _ctx.Cuenta.AnyAsync(c => c.NumeroCuenta == dto.NumeroCuenta);
            if (exists) return (false, null, "Número de cuenta ya existe");

            var cuenta = new Cuentum
            {
                NumeroCuenta = dto.NumeroCuenta,
                TipoCuenta = tipo.Nombre,
                SaldoDisponible = dto.SaldoInicial,
                Moneda = tipo.Moneda,
                Estado = "ACTIVO",
                IdUsuario = dto.IdUsuario
            };

            _ctx.Cuenta.Add(cuenta);
            await _ctx.SaveChangesAsync();
            return (true, cuenta.IdCuenta, "Cuenta creada");
        }

        public async Task<bool> UpdateAccountAsync(int idCuenta, GuardarDetalleProductoDTO updateDto)
        {
            var c = await _ctx.Cuenta.FindAsync(idCuenta);
            if (c == null) return false;
            // Actualizar campos permitidos
            c.NumeroCuenta = updateDto.NumeroCuenta;
            c.TipoCuenta = updateDto.TipoCuenta;
            c.SaldoDisponible = updateDto.SaldoDisponible;
            c.Moneda = updateDto.Moneda;
            _ctx.Cuenta.Update(c);
            var rows = await _ctx.SaveChangesAsync();
            return rows > 0;
        }

        public async Task<bool> DeleteAccountAsync(int idCuenta)
        {
            var c = await _ctx.Cuenta.FindAsync(idCuenta);
            if (c == null) return false;
            _ctx.Cuenta.Remove(c);
            var rows = await _ctx.SaveChangesAsync();
            return rows > 0;
        }
    }
}
