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
    public class IProducto : ServicioProducto
    {
        private readonly InterbankContext _ctx;
        public IProducto(InterbankContext ctx) { _ctx = ctx; }

        public async Task<IEnumerable<ProductoDto>> ListPersonalizedProductsAsync(int idUsuario)
        {
            // ejemplo simple: listar productos activos y que no sean creados por el usuario (o usar preferencias)
            var prods = await _ctx.ProductoFinancieros
                .Where(p => p.Estado == "ACTIVO")
                .ToListAsync();

            if (!prods.Any()) return Enumerable.Empty<ProductoDto>();

            return prods.Select(p => new ProductoDto
            {
                IdProducto = p.IdProducto,
                Nombre = p.Nombre,
                Tipo = p.Tipo,
                TasaInteres = p.TasaInteres,
                Plazo = p.Plazo,
                Estado = p.Estado
            });
        }

        public async Task<ProductoDto?> GetRecommendedProductAsync(int idProducto)
        {
            var p = await _ctx.ProductoFinancieros.FindAsync(idProducto);
            if (p == null) return null;

            return new ProductoDto
            {
                IdProducto = p.IdProducto,
                Nombre = p.Nombre,
                Tipo = p.Tipo,
                TasaInteres = p.TasaInteres,
                Plazo = p.Plazo,
                Estado = p.Estado
            };
        }

        public async Task<bool> AddFavoriteAsync(CrearProductoFavoritoDto favorite)
        {
            // Validar que no exista ya
            var exists = await _ctx.ProductoFavoritos.AnyAsync(f => f.IdUsuario == favorite.IdUsuario && f.IdProducto == favorite.IdProducto);
            if (exists) return false;

            var pf = new ProductoFavorito
            {
                IdUsuario = favorite.IdUsuario,
                IdProducto = favorite.IdProducto
                // FechaAgregado por defecto en BD
            };

            _ctx.ProductoFavoritos.Add(pf);
            var rows = await _ctx.SaveChangesAsync();
            return rows > 0;
        }

        public async Task<bool> UpdateProductAsync(int idProducto, ProductoDto updateDto)
        {
            var p = await _ctx.ProductoFinancieros.FindAsync(idProducto);
            if (p == null) return false;

            p.Nombre = updateDto.Nombre;
            p.Tipo = updateDto.Tipo;
            p.TasaInteres = updateDto.TasaInteres;
            p.Plazo = updateDto.Plazo;
            p.Estado = updateDto.Estado;
            _ctx.ProductoFinancieros.Update(p);
            var rows = await _ctx.SaveChangesAsync();
            return rows > 0;
        }

        public async Task<bool> RemoveFavoriteAsync(int idUsuario, int idProducto)
        {
            var fav = await _ctx.ProductoFavoritos.FirstOrDefaultAsync(f => f.IdUsuario == idUsuario && f.IdProducto == idProducto);
            if (fav == null) return false;
            _ctx.ProductoFavoritos.Remove(fav);
            var rows = await _ctx.SaveChangesAsync();
            return rows > 0;
        }
    }
}
