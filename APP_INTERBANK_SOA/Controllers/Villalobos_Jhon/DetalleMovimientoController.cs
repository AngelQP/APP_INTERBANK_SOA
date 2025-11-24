using APP_INTERBANK_SOA.Servicios.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace APP_INTERBANK_SOA.Controllers.Villalobos_Jhon
{
    [Route("api/[controller]")]
    [ApiController]
    public class DetalleMovimientoController : ControllerBase
    {
        private readonly ServicioMovimiento _svc;
        public DetalleMovimientoController(ServicioMovimiento svc) { _svc = svc; }

        // GET api/movements/{idCuenta}
        [HttpGet("{idCuenta:int}")]
        public async Task<IActionResult> GetMovements(int idCuenta)
        {
            var list = (await _svc.ListMovementsAsync(idCuenta)).ToList();
            if (!list.Any()) return NotFound(new { message = "La lista está vacía" });
            return Ok(list);
        }

        // GET api/movements/{idCuenta}/range?desde=2025-01-01&hasta=2025-01-31
        [HttpGet("{idCuenta:int}/range")]
        public async Task<IActionResult> GetMovementsByRange(int idCuenta, [FromQuery] DateTime desde, [FromQuery] DateTime hasta)
        {
            if (desde > hasta) return BadRequest(new { message = "Fecha 'desde' no puede ser mayor que 'hasta'" });
            var list = (await _svc.ListMovementsByDateRangeAsync(idCuenta, desde, hasta)).ToList();
            if (!list.Any()) return NotFound(new { message = "No hay datos en ese rango" });
            return Ok(list);
        }
    }
}
