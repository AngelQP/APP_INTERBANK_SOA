using APP_INTERBANK_SOA.DTO.Ganoza_Sebastian;
using APP_INTERBANK_SOA.Servicios.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace APP_INTERBANK_SOA.Controllers.Ganoza_Sebastian
    {
    [Route("api/[controller]")]
    [ApiController]
    public class PagoController : ControllerBase
    {
        private readonly Pagos _svc;

        public PagoController(Pagos svc)
        {
            _svc = svc;
        }

        // LISTAR HISTORIAL
        [HttpGet]
        public async Task<IActionResult> Listar()
        {
            var list = (await _svc.ListarHistorialAsync()).ToList();
            if (!list.Any()) return NotFound(new { message = "No hay pagos registrados." });

            return Ok(list);
        }

        // OBTENER DETALLE
        [HttpGet("{idPago:int}")]
        public async Task<IActionResult> Obtener(int idPago)
        {
            var pago = await _svc.ObtenerDetalleAsync(idPago);

            if (pago is null)
                return NotFound(new { message = "Pago no encontrado." });

            return Ok(pago);
        }

        // CREAR PAGO O RECARGA
        [HttpPost]
        public async Task<IActionResult> Crear([FromBody] CrearPagoDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var (success, idPago, message) = await _svc.CrearPagoAsync(dto);

            if (!success) return BadRequest(new { message });

            return CreatedAtAction(nameof(Obtener), new { idPago }, new { message, idPago });
        }

        // ACTUALIZAR PAGO PROGRAMADO
        [HttpPut("{idPago:int}")]
        public async Task<IActionResult> Actualizar(int idPago, [FromBody] ActualizarPagoProgramadoDTO dto)
        {
            var ok = await _svc.ActualizarPagoProgramadoAsync(idPago, dto);

            if (!ok)
                return NotFound(new { message = "Pago programado no encontrado." });

            return Ok(new { message = "Pago programado actualizado." });
        }

        // ELIMINAR PAGO PROGRAMADO
        [HttpDelete("{idPago:int}")]
        public async Task<IActionResult> Eliminar(int idPago)
        {
            var ok = await _svc.EliminarPagoProgramadoAsync(idPago);

            if (!ok)
                return NotFound(new { message = "Pago programado no encontrado." });

            return Ok(new { message = "Pago programado eliminado." });
        }
    }
}
