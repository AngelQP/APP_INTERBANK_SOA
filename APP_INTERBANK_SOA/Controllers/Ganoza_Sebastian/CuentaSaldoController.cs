using APP_INTERBANK_SOA.DTO.Ganoza_Sebastian;
using APP_INTERBANK_SOA.Servicios.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace APP_INTERBANK_SOA.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CuentaSaldoController : ControllerBase
    {
        private readonly CuentaSaldo _svc;

        public CuentaSaldoController(CuentaSaldo svc)
        {
            _svc = svc;
        }

        // GET api/cuentasaldo/user/5
        [HttpGet("user/{idUsuario:int}")]
        public async Task<IActionResult> ListarSaldos(int idUsuario)
        {
            var list = (await _svc.ListarSaldosPorUsuarioAsync(idUsuario)).ToList();

            if (!list.Any())
                return NotFound(new { message = "El usuario no tiene cuentas o no existen saldos." });

            return Ok(list);
        }

        // GET api/cuentasaldo/10
        [HttpGet("{idCuenta:int}")]
        public async Task<IActionResult> ObtenerSaldo(int idCuenta)
        {
            var saldo = await _svc.ObtenerSaldoCuentaAsync(idCuenta);

            if (saldo is null)
                return NotFound(new { message = "La cuenta no existe." });

            return Ok(saldo);
        }
    }
}
