using APP_INTERBANK_SOA.DTO.Villalobos_Jhon;
using APP_INTERBANK_SOA.Servicios.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace APP_INTERBANK_SOA.Controllers.Villalobos_Jhon
{
    [Route("api/[controller]")]
    [ApiController]
    public class CuentaAhorroController : ControllerBase
    {
        private readonly ServicioCuentaAhorro _svc;
        public CuentaAhorroController(ServicioCuentaAhorro svc) { _svc = svc; }

        // GET api/savingaccounts/types
        [HttpGet("types")]
        public async Task<IActionResult> ListTypes()
        {
            var list = (await _svc.ListAvailableSavingTypesAsync()).ToList();
            if (!list.Any()) return NotFound(new { message = "No hay tipos de cuenta disponibles" });
            return Ok(list);
        }

        // GET api/savingaccounts/requirements/{idTipo}
        [HttpGet("requirements/{idTipo:int}")]
        public async Task<IActionResult> GetRequirements(int idTipo)
        {
            var reqs = (await _svc.GetRequirementsForTypeAsync(idTipo)).ToList();
            if (!reqs.Any()) return NotFound(new { message = "No existen requisitos para este tipo" });
            return Ok(reqs);
        }

        // POST api/savingaccounts
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CrearCuentaAhorroDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var (success, idCuenta, message) = await _svc.CreateSavingAccountAsync(dto);
            if (!success) return BadRequest(new { message });
            return CreatedAtAction(nameof(GetAccount), new { idCuenta = idCuenta }, new { message, idCuenta });
        }

        // GET api/savingaccounts/{idCuenta}
        [HttpGet("{idCuenta:int}")]
        public async Task<IActionResult> GetAccount(int idCuenta)
        {
            var c = await _svc.ListAvailableSavingTypesAsync(); // solo para ejemplo: normalmente tendrías método para obtener cuenta por id
            var cuenta = await Task.Run(() => _svc); // placeholder: si deseas get por id, implementa método en servicio
                                                     // Para no romper la consistencia del ejemplo, responderemos con NotFound (o implementar método real)
            return NotFound(new { message = "Implementa Get por id si lo requieres" });
        }

        // PUT api/savingaccounts/{idCuenta}
        [HttpPut("{idCuenta:int}")]
        public async Task<IActionResult> UpdateAccount(int idCuenta, [FromBody] GuardarDetalleProductoDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var ok = await _svc.UpdateAccountAsync(idCuenta, dto);
            if (!ok) return NotFound(new { message = "No existe la cuenta a actualizar" });
            return Ok(new { message = "Cuenta actualizada" });
        }

        // DELETE api/savingaccounts/{idCuenta}
        [HttpDelete("{idCuenta:int}")]
        public async Task<IActionResult> DeleteAccount(int idCuenta)
        {
            var ok = await _svc.DeleteAccountAsync(idCuenta);
            if (!ok) return NotFound(new { message = "No existe la cuenta a eliminar" });
            return Ok(new { message = "Cuenta eliminada" });
        }
    }
}
