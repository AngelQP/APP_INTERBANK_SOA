using APP_INTERBANK_SOA.DTO.Ganoza_Sebastian;
using APP_INTERBANK_SOA.Servicios.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace APP_INTERBANK_SOA.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransferenciaController : ControllerBase
    {
        private readonly Transferencia _svc;

        public TransferenciaController(Transferencia svc)
        {
            _svc = svc;
        }

        // GET api/transferencia
        [HttpGet]
        public async Task<IActionResult> Listar()
        {
            var list = (await _svc.ListarHistorialAsync()).ToList();
            if (!list.Any()) return NotFound(new { message = "No existen transferencias." });

            return Ok(list);
        }

        // GET api/transferencia/{id}
        [HttpGet("{id:int}")]
        public async Task<IActionResult> Obtener(int id)
        {
            var t = await _svc.ObtenerTransferenciaAsync(id);
            if (t is null) return NotFound(new { message = "Transferencia no encontrada." });

            return Ok(t);
        }

        // POST api/transferencia
        [HttpPost]
        public async Task<IActionResult> Crear([FromBody] CrearTransferenciaDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var (success, id, message) = await _svc.CrearTransferenciaAsync(dto);

            if (!success) return BadRequest(new { message });

            return CreatedAtAction(nameof(Obtener), new { id }, new { message, id });
        }

        // PUT api/transferencia/{id}
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Actualizar(int id, [FromBody] ActualizarTransferenciaDTO dto)
        {
            var ok = await _svc.ActualizarProgramadaAsync(id, dto);

            if (!ok) return NotFound(new { message = "Transferencia programada no existe." });

            return Ok(new { message = "Transferencia programada actualizada." });
        }

        // DELETE api/transferencia/{id}
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Eliminar(int id)
        {
            var ok = await _svc.EliminarProgramadaAsync(id);

            if (!ok) return NotFound(new { message = "Transferencia programada no existe." });

            return Ok(new { message = "Transferencia programada eliminada." });
        }
    }
}
