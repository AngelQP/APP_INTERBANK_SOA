using APP_INTERBANK_SOA.Servicios.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace APP_INTERBANK_SOA.Controllers.Villalobos_Jhon
{
    [Route("api/[controller]")]
    [ApiController]
    public class DetalleAhorroController : ControllerBase
    {
        private readonly ServicioGuardar _svc;
        public DetalleAhorroController(ServicioGuardar svc) { _svc = svc; }

        // GET api/savings/user/{idUsuario}
        [HttpGet("user/{idUsuario:int}")]
        public async Task<IActionResult> ListUserSavings(int idUsuario)
        {
            var list = (await _svc.ListSavingsByUserAsync(idUsuario)).ToList();
            if (!list.Any()) return NotFound(new { message = "No hay productos de ahorro" });
            return Ok(list);
        }

        // GET api/savings/{idDeposito}
        [HttpGet("{idDeposito:int}")]
        public async Task<IActionResult> GetSavingDetail(int idDeposito)
        {
            var detail = await _svc.GetSavingDetailAsync(idDeposito);
            if (detail == null) return NotFound(new { message = "No existe el depósito solicitado" });
            return Ok(detail);
        }
    }
}
