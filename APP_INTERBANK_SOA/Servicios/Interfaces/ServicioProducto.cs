using APP_INTERBANK_SOA.DTO.Villalobos_Jhon;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace APP_INTERBANK_SOA.Servicios.Interfaces
{
    public interface ServicioProducto
    {
        Task<IEnumerable<ProductoDto>> ListPersonalizedProductsAsync(int idUsuario);
        Task<ProductoDto?> GetRecommendedProductAsync(int idProducto);
        Task<bool> AddFavoriteAsync(CrearProductoFavoritoDto favorite);
        Task<bool> UpdateProductAsync(int idProducto, ProductoDto updateDto);
        Task<bool> RemoveFavoriteAsync(int idUsuario, int idProducto);
    }
}
