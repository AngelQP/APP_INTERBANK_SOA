using APP_INTERBANK_SOA.DTO.Quispe_Angel.Reclamo;
using APP_INTERBANK_SOA.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace APP_INTERBANK_SOA.Controllers.Quispe_Angel
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReclamosController : ControllerBase
    {
        private readonly InterbankContext _context;

        public ReclamosController(InterbankContext context)
        {
            _context = context;
        }

        // POST: api/reclamos
        /* TipoReclamo: Consulta, sugerencia, reclamo */
        [HttpPost("crearReclamo")]
        [Authorize(Roles = "Cliente")]
        public async Task<ActionResult<ReclamoResponseDto>> CrearReclamo([FromBody] CrearReclamoDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var idUsuario = GetUserIdFromToken();
            if (idUsuario == null)
                return Unauthorized(new { message = "No se pudo identificar al usuario autenticado." });

            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.IdUsuario == idUsuario.Value);
            if (usuario == null)
                return Unauthorized(new { message = "El usuario ya no existe en el sistema." });

            var estadoUsuario = (usuario.Estado ?? string.Empty).Trim().ToUpper();
            if (estadoUsuario == "INACTIVO")
                return StatusCode(StatusCodes.Status403Forbidden,
                    new { message = "El usuario se encuentra INACTIVO. No puede registrar reclamos." });

            var tipo = (dto.TipoReclamo ?? string.Empty).Trim().ToUpper();
            if (tipo != "CONSULTA" && tipo != "SUGERENCIA" && tipo != "RECLAMO")
            {
                return BadRequest(new
                {
                    message = "El tipoReclamo debe ser CONSULTA, SUGERENCIA o RECLAMO."
                });
            }

            var reclamo = new Reclamo
            {
                TipoReclamo = tipo,
                Descripcion = dto.Descripcion,
                Estado = "PENDIENTE",
                FechaCreacion = DateTime.UtcNow,
                IdUsuario = idUsuario.Value,
                Respuesta = null,
                FechaRespuesta = null
            };

            _context.Reclamos.Add(reclamo);
            await _context.SaveChangesAsync();

            var response = new ReclamoResponseDto
            {
                IdReclamo = reclamo.IdReclamo,
                TipoReclamo = reclamo.TipoReclamo,
                Descripcion = reclamo.Descripcion,
                Estado = reclamo.Estado,
                FechaCreacion = reclamo.FechaCreacion
            };

            return CreatedAtAction(nameof(ObtenerReclamoPorId), new { id = reclamo.IdReclamo }, response);
        }


        // GET: api/reclamos/{id}
        [HttpGet("reclamoXId/{id:int}")]
        [Authorize(Roles = "Cliente")]
        public async Task<ActionResult<ReclamoResponseDto>> ObtenerReclamoPorId(int id)
        {
            var idUsuario = GetUserIdFromToken();
            if (idUsuario == null)
                return Unauthorized(new { message = "No se pudo identificar al usuario autenticado." });

            // Solo busca reclamos del propio usuario
            var reclamo = await _context.Reclamos
                .FirstOrDefaultAsync(r => r.IdReclamo == id && r.IdUsuario == idUsuario.Value);

            if (reclamo == null)
                return NotFound(new { message = "Reclamo no encontrado para este usuario." });

            var response = new ReclamoResponseDto
            {
                IdReclamo = reclamo.IdReclamo,
                TipoReclamo = reclamo.TipoReclamo,
                Descripcion = reclamo.Descripcion,
                Estado = reclamo.Estado,
                FechaCreacion = reclamo.FechaCreacion
            };

            return Ok(response);
        }

        
        // GET: api/reclamos
        [HttpGet("listarReclamos")]
        [Authorize(Roles = "Cliente")]
        public async Task<ActionResult<IEnumerable<ReclamoResponseDto>>> ListarMisReclamos()
        {
            var idUsuario = GetUserIdFromToken();
            if (idUsuario == null)
                return Unauthorized(new { message = "No se pudo identificar al usuario autenticado." });

            var reclamos = await _context.Reclamos
                .Where(r => r.IdUsuario == idUsuario.Value)
                .OrderByDescending(r => r.FechaCreacion)
                .ToListAsync();

            var response = reclamos.Select(r => new ReclamoResponseDto
            {
                IdReclamo = r.IdReclamo,
                TipoReclamo = r.TipoReclamo,
                Descripcion = r.Descripcion,
                Estado = r.Estado,
                FechaCreacion = r.FechaCreacion
            });

            return Ok(response);
        }

        
        [HttpPut("actualizarReclamo/{id:int}")]
        [Authorize(Roles = "Cliente")] 
        public async Task<ActionResult<ReclamoResponseDto>> ActualizarReclamo(
            int id,
            [FromBody] ActualizarReclamoDto dto )
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // 1. Obtener idUsuario desde el token
            var idUsuario = GetUserIdFromToken();
            if (idUsuario == null)
                return Unauthorized(new { message = "No se pudo identificar al usuario autenticado." });

            // 2. Buscar el reclamo que PERTENEZCA a este usuario
            var reclamo = await _context.Reclamos
                .FirstOrDefaultAsync(r => r.IdReclamo == id && r.IdUsuario == idUsuario.Value);

            if (reclamo == null)
                return NotFound(new { message = "Reclamo no encontrado para este usuario." });

            // 3. Aplicar cambios solo si los campos vienen informados

            // TipoReclamo opcional
            if (dto.TipoReclamo != null)
            {
                var tipo = dto.TipoReclamo.Trim().ToUpper();

                if (tipo != "CONSULTA" && tipo != "SUGERENCIA" && tipo != "RECLAMO")
                {
                    return BadRequest(new
                    {
                        message = "El tipoReclamo debe ser CONSULTA, SUGERENCIA o RECLAMO."
                    });
                }

                reclamo.TipoReclamo = tipo;
            }

            // Descripcion opcional
            if (dto.Descripcion != null)
            {
                if (string.IsNullOrWhiteSpace(dto.Descripcion))
                {
                    return BadRequest(new { message = "La descripción no puede ser vacía si se envía." });
                }

                reclamo.Descripcion = dto.Descripcion;
            }

            // 4. Guardar cambios
            await _context.SaveChangesAsync();

            // 5. Devolver DTO actualizado
            var response = new ReclamoResponseDto
            {
                IdReclamo = reclamo.IdReclamo,
                TipoReclamo = reclamo.TipoReclamo,
                Descripcion = reclamo.Descripcion,
                Estado = reclamo.Estado,
                FechaCreacion = reclamo.FechaCreacion
            };

            return Ok(response);
        }


        [HttpDelete("eliminarReclamo/{id:int}")]
        [Authorize(Roles = "Cliente")] // por si el controlador no lo tiene a nivel de clase
        public async Task<IActionResult> EliminarReclamo(int id)
        {
            // 1. Obtener idUsuario desde el token
            var idUsuario = GetUserIdFromToken();
            if (idUsuario == null)
                return Unauthorized(new { message = "No se pudo identificar al usuario autenticado." });

            // 2. Buscar reclamo que PERTENEZCA a este usuario
            var reclamo = await _context.Reclamos
                .FirstOrDefaultAsync(r => r.IdReclamo == id && r.IdUsuario == idUsuario.Value);

            if (reclamo == null)
                return NotFound(new { message = "Reclamo no encontrado para este usuario." });

            // 3. Si ya está INACTIVO, simplemente responder
            var estadoActual = (reclamo.Estado ?? string.Empty).Trim().ToUpper();
            if (estadoActual == "INACTIVO")
            {
                return Ok(new { message = "El reclamo ya se encuentra INACTIVO." });
            }

            // 4. Marcar como INACTIVO (soft delete)
            reclamo.Estado = "INACTIVO";

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Reclamo marcado como INACTIVO correctamente.",
                idReclamo = reclamo.IdReclamo,
                estado = reclamo.Estado
            });
        }




        private int? GetUserIdFromToken()
        {
            var userIdClaim =
                User.FindFirst(JwtRegisteredClaimNames.Sub) ??
                User.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
                return null;

            if (!int.TryParse(userIdClaim.Value, out var idUsuario))
                return null;

            return idUsuario;
        }

    }
}
