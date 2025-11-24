using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace APP_INTERBANK_SOA.Controllers.Quispe_Angel
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestAuthController : ControllerBase
    {
        // GET: api/testauth/ping
        [HttpGet("ping")]
        [Authorize]  // solo pide estar autenticado (cualquier rol)
        public IActionResult Ping()
        {
            return Ok(new
            {
                message = "Token válido, acceso concedido.",
                user = User.Identity?.Name,
                claims = User.Claims.Select(c => new { c.Type, c.Value })
            });
        }
    }
}
