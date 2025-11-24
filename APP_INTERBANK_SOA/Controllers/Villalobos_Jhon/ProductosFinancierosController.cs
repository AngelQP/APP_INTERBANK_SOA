using APP_INTERBANK_SOA.DTO.Villalobos_Jhon;
using APP_INTERBANK_SOA.Servicios.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.JsonPatch;

namespace APP_INTERBANK_SOA.Controllers.Villalobos_Jhon
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductosFinancierosController : ControllerBase
    {
        private readonly ServicioProducto _svc;
        public ProductosFinancierosController(ServicioProducto svc) { _svc = svc; }

        // GET api/products/personalized/{idUsuario}
        [HttpGet("personalized/{idUsuario:int}")]
        public async Task<IActionResult> ListPersonalized(int idUsuario)
        {
            var list = (await _svc.ListPersonalizedProductsAsync(idUsuario)).ToList();
            if (!list.Any()) return NotFound(new { message = "La lista está vacía" });
            return Ok(list);
        }

        // GET api/products/{idProducto}
        [HttpGet("{idProducto:int}")]
        public async Task<IActionResult> GetProduct(int idProducto)
        {
            var p = await _svc.GetRecommendedProductAsync(idProducto);
            if (p == null) return NotFound(new { message = "No existe la información solicitada" });
            return Ok(p);
        }

        // POST api/products/favorite
        [HttpPost("favorite")]
        public async Task<IActionResult> AddFavorite([FromBody] CrearProductoFavoritoDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var ok = await _svc.AddFavoriteAsync(dto);
            if (!ok) return BadRequest(new { message = "No se pudo agregar a favoritos (puede ya existir)" });
            return Ok(new { message = "Producto agregado a favoritos" });
        }

        // PUT api/products/{idProducto}
        [HttpPut("{idProducto:int}")]
        public async Task<IActionResult> UpdateProduct(int idProducto, [FromBody] ProductoDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var updated = await _svc.UpdateProductAsync(idProducto, dto);
            if (!updated) return NotFound(new { message = "No existe el producto a actualizar" });
            return Ok(new { message = "Producto actualizado" });
        }

        // DELETE api/products/favorite?userId=1&productId=2
        [HttpDelete("favorite")]
        public async Task<IActionResult> RemoveFavorite([FromQuery] int userId, [FromQuery] int productId)
        {
            var ok = await _svc.RemoveFavoriteAsync(userId, productId);
            if (!ok) return NotFound(new { message = "Favorito no encontrado" });
            return Ok(new { message = "Producto removido de favoritos" });
        }
    }
}
