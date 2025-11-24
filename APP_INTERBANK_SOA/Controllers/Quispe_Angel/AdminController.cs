using APP_INTERBANK_SOA.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using APP_INTERBANK_SOA.Utils;
using APP_INTERBANK_SOA.DTO.Quispe_Angel.Administrador;

namespace APP_INTERBANK_SOA.Controllers.Quispe_Angel
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {

        private readonly InterbankContext _context;
        private readonly IConfiguration _configuration;

        public AdminController(InterbankContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        // POST: api/usuarios/registrar-admin
        [HttpPost("registrarAdmin")]
        public async Task<ActionResult<AdminDto>> RegistrarAdmin([FromBody] RegisterAdminDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // 1. Validar tipo de documento
            var tipoDoc = dto.TipoDocumento.Trim().ToUpper();
            if (tipoDoc != "DNI" && tipoDoc != "CE")
            {
                return BadRequest(new
                {
                    message = "El tipoDocumento debe ser 'DNI' o 'CE'."
                });
            }

            // 2. Verificar unicidad de correo
            var correoEnUso = await _context.Usuarios
                .AnyAsync(u => u.Correo == dto.Correo);

            if (correoEnUso)
            {
                return Conflict(new
                {
                    message = "Ya existe un usuario registrado con ese correo."
                });
            }

            // 3. Verificar unicidad de documento
            var docEnUso = await _context.Usuarios
                .AnyAsync(u =>
                    u.TipoDocumento == tipoDoc &&
                    u.NumeroDocumento == dto.NumeroDocumento);

            if (docEnUso)
            {
                return Conflict(new
                {
                    message = "Ya existe un usuario registrado con ese tipo y número de documento."
                });
            }

            // 4. Buscar rol Admin
            var rolAdmin = await _context.Rols
                .FirstOrDefaultAsync(r => r.Nombre == "Admin");

            if (rolAdmin == null)
            {
                return StatusCode(500, new
                {
                    message = "No se encontró el rol 'Admin' en la base de datos."
                });
            }

            // 5. Crear usuario administrador (sin cuenta asociada)
            var admin = new Usuario
            {
                Nombre = dto.Nombre.Trim(),
                Apellido = dto.Apellido.Trim(),
                Correo = dto.Correo.Trim(),
                ContrasenaHash = PasswordHasher.Hash(dto.Password),
                TipoDocumento = tipoDoc,
                NumeroDocumento = dto.NumeroDocumento.Trim(),
                Telefono = string.IsNullOrWhiteSpace(dto.Telefono) ? null : dto.Telefono.Trim(),
                Estado = "ACTIVO",                 // los admins se crean activos
                FechaRegistro = DateTime.UtcNow,
                UltimoLogin = null,
                IntentosFallidos = 0,
                BloqueadoHasta = null,
                IdRol = rolAdmin.IdRol
            };

            _context.Usuarios.Add(admin);
            await _context.SaveChangesAsync();

            var respuesta = new AdminDto
            {
                IdUsuario = admin.IdUsuario,
                NombreCompleto = $"{admin.Nombre} {admin.Apellido}",
                Correo = admin.Correo,
                TipoDocumento = admin.TipoDocumento,
                NumeroDocumento = admin.NumeroDocumento,
                Telefono = admin.Telefono,
                Estado = admin.Estado,
                Rol = rolAdmin.Nombre,
                FechaRegistro = admin.FechaRegistro
            };

            return CreatedAtAction(
                nameof(ObtenerPorId),
                new { id = admin.IdUsuario },
                respuesta
            );
        }

        // GET: api/administradores/{id}
        [HttpGet("adminxId/{id:int}")]
        public async Task<ActionResult<AdminDto>> ObtenerPorId(int id)
        {
            var admin = await _context.Usuarios
                .Include(u => u.IdRolNavigation)
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.IdUsuario == id && u.IdRolNavigation.Nombre == "Admin");

            if (admin == null)
                return NotFound(new { message = "Administrador no encontrado." });

            var respuesta = new AdminDto
            {
                IdUsuario = admin.IdUsuario,
                NombreCompleto = $"{admin.Nombre} {admin.Apellido}",
                Correo = admin.Correo,
                TipoDocumento = admin.TipoDocumento,
                NumeroDocumento = admin.NumeroDocumento,
                Telefono = admin.Telefono,
                Estado = admin.Estado,
                Rol = admin.IdRolNavigation.Nombre,
                FechaRegistro = admin.FechaRegistro
            };

            return Ok(respuesta);
        }

        // GET: api/administradores/listar?cantidad=5
        [HttpGet("listarAdmin")]
        public async Task<ActionResult<IEnumerable<AdminDto>>> Listar([FromQuery] int? cantidad)
        {
            // 1. Regla: por defecto 5, máximo 10
            int take = cantidad ?? 5;
            if (take <= 0) take = 5;
            if (take > 10) take = 10;

            // 2. Traer solo usuarios con rol "Admin"
            var admins = await _context.Usuarios
                .Include(u => u.IdRolNavigation)
                .AsNoTracking()
                .Where(u => u.IdRolNavigation.Nombre == "Admin")
                .OrderBy(u => u.IdUsuario)
                .Take(take)
                .ToListAsync();

            // 3. Mapear a AdminDto
            var resultado = admins.Select(a => new AdminDto
            {
                IdUsuario = a.IdUsuario,
                NombreCompleto = $"{a.Nombre} {a.Apellido}",
                Correo = a.Correo,
                TipoDocumento = a.TipoDocumento,
                NumeroDocumento = a.NumeroDocumento,
                Telefono = a.Telefono,
                Estado = a.Estado,
                Rol = a.IdRolNavigation.Nombre,
                FechaRegistro = a.FechaRegistro
            }).ToList();

            return Ok(resultado);
        }

        // PUT: api/administradores/{id}
        [HttpPut("actualizarAdmin/{id:int}")]
        public async Task<ActionResult<AdminDto>> Actualizar(int id, [FromBody] UpdateAdminDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // 1. Buscar admin (solo rol Admin)
            var admin = await _context.Usuarios
                .Include(u => u.IdRolNavigation)
                .FirstOrDefaultAsync(u => u.IdUsuario == id && u.IdRolNavigation.Nombre == "Admin");

            if (admin == null)
                return NotFound(new { message = "Administrador no encontrado." });

            // 2. Normalizar y validar tipoDocumento si viene
            string tipoDocNuevo = admin.TipoDocumento;
            if (!string.IsNullOrWhiteSpace(dto.TipoDocumento))
            {
                var tipoDoc = dto.TipoDocumento.Trim().ToUpper();
                if (tipoDoc != "DNI" && tipoDoc != "CE")
                {
                    return BadRequest(new
                    {
                        message = "El tipoDocumento debe ser 'DNI' o 'CE' si se envía."
                    });
                }
                tipoDocNuevo = tipoDoc;
            }

            // 3. Normalizar y validar estado si viene
            string estadoNuevo = admin.Estado;
            if (!string.IsNullOrWhiteSpace(dto.Estado))
            {
                var estado = dto.Estado.Trim().ToUpper();
                if (estado != "ACTIVO" && estado != "INACTIVO")
                {
                    return BadRequest(new
                    {
                        message = "El estado debe ser 'ACTIVO' o 'INACTIVO' si se envía."
                    });
                }
                estadoNuevo = estado;
            }

            string numeroDocNuevo = admin.NumeroDocumento;
            if (!string.IsNullOrWhiteSpace(dto.NumeroDocumento))
            {
                numeroDocNuevo = dto.NumeroDocumento.Trim();
            }

            // 4. Validar unicidad de correo si se envía
            if (!string.IsNullOrWhiteSpace(dto.Correo))
            {
                var correoNormalizado = dto.Correo.Trim();

                var correoEnUso = await _context.Usuarios
                    .AnyAsync(u => u.Correo == correoNormalizado && u.IdUsuario != id);

                if (correoEnUso)
                {
                    return Conflict(new
                    {
                        message = "Ya existe otro usuario con ese correo."
                    });
                }

                admin.Correo = correoNormalizado;
            }

            // 5. Validar unicidad de (tipoDocumento, numeroDocumento) si cambia alguno
            bool cambiaTipoODoc =
                (!string.IsNullOrWhiteSpace(dto.TipoDocumento) && tipoDocNuevo != admin.TipoDocumento) ||
                (!string.IsNullOrWhiteSpace(dto.NumeroDocumento) && numeroDocNuevo != admin.NumeroDocumento);

            if (cambiaTipoODoc)
            {
                var docEnUso = await _context.Usuarios
                    .AnyAsync(u =>
                        u.TipoDocumento == tipoDocNuevo &&
                        u.NumeroDocumento == numeroDocNuevo &&
                        u.IdUsuario != id);

                if (docEnUso)
                {
                    return Conflict(new
                    {
                        message = "Ya existe otro usuario con ese tipo y número de documento."
                    });
                }

                admin.TipoDocumento = tipoDocNuevo;
                admin.NumeroDocumento = numeroDocNuevo;
            }

            // 6. Actualizar otros campos solo si vienen
            if (!string.IsNullOrWhiteSpace(dto.Nombre))
                admin.Nombre = dto.Nombre.Trim();

            if (!string.IsNullOrWhiteSpace(dto.Apellido))
                admin.Apellido = dto.Apellido.Trim();

            if (!string.IsNullOrWhiteSpace(dto.Telefono))
                admin.Telefono = dto.Telefono.Trim();

            // Estado (solo usuario, no toca cuentas porque es admin)
            admin.Estado = estadoNuevo;

            await _context.SaveChangesAsync();

            // 7. Armar respuesta
            var respuesta = new AdminDto
            {
                IdUsuario = admin.IdUsuario,
                NombreCompleto = $"{admin.Nombre} {admin.Apellido}",
                Correo = admin.Correo,
                TipoDocumento = admin.TipoDocumento,
                NumeroDocumento = admin.NumeroDocumento,
                Telefono = admin.Telefono,
                Estado = admin.Estado,
                Rol = admin.IdRolNavigation.Nombre,
                FechaRegistro = admin.FechaRegistro
            };

            return Ok(respuesta);
        }

        // DELETE: api/administradores/{id}
        [HttpDelete("eliminarAdmin/{id:int}")]
        public async Task<IActionResult> Eliminar(int id)
        {
            // 1. Buscar solo admins
            var admin = await _context.Usuarios
                .Include(u => u.IdRolNavigation)
                .FirstOrDefaultAsync(u => u.IdUsuario == id && u.IdRolNavigation.Nombre == "Admin");

            if (admin == null)
                return NotFound(new { message = "Administrador no encontrado." });

            // 2. Si ya está INACTIVO, se devuelve 204
            if (admin.Estado == "INACTIVO")
                return NoContent();

            // 3. Marcar como INACTIVO
            admin.Estado = "INACTIVO";

            await _context.SaveChangesAsync();

            // 4. Sin contenido, borrado lógico OK
            return NoContent();
        }


        // POST: api/administradores/login
        [HttpPost("login")]
        public async Task<ActionResult<AdminLoginResponseDto>> Login([FromBody] AdminLoginDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // 1. Buscar admin por correo
            var admin = await _context.Usuarios
                .Include(u => u.IdRolNavigation)
                .FirstOrDefaultAsync(u => u.Correo == dto.Correo && u.IdRolNavigation.Nombre == "Admin");

            if (admin == null)
            {
                return Unauthorized(new { message = "Credenciales inválidas." });
            }

            // 2. NO permitir login si está INACTIVO
            var estado = (admin.Estado ?? string.Empty).Trim().ToUpper();
            if (estado == "INACTIVO")
            {
                return StatusCode(StatusCodes.Status403Forbidden, new
                {
                    message = "La cuenta de administrador se encuentra INACTIVA. Contacte con el administrador del sistema."
                });
            }

            // 3. Validar bloqueo por intentos fallidos
            if (admin.BloqueadoHasta.HasValue && admin.BloqueadoHasta > DateTime.UtcNow)
            {
                return StatusCode(StatusCodes.Status403Forbidden, new
                {
                    message = $"Cuenta bloqueada hasta {admin.BloqueadoHasta.Value:yyyy-MM-dd HH:mm:ss} UTC."
                });
            }

            // 4. Verificar contraseña con nuestro helper
            var passwordOk = PasswordHasher.Verify(dto.Password, admin.ContrasenaHash);

            const int MAX_INTENTOS = 5;
            const int MINUTOS_BLOQUEO = 15;

            if (!passwordOk)
            {
                admin.IntentosFallidos += 1;

                if (admin.IntentosFallidos >= MAX_INTENTOS)
                {
                    admin.BloqueadoHasta = DateTime.UtcNow.AddMinutes(MINUTOS_BLOQUEO);
                    admin.IntentosFallidos = 0;
                }

                await _context.SaveChangesAsync();

                return Unauthorized(new { message = "Credenciales inválidas." });
            }

            // 5. Resetear intentos y actualizar último login
            admin.IntentosFallidos = 0;
            admin.BloqueadoHasta = null;
            admin.UltimoLogin = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            // 6. Generar token JWT
            var jwtSettings = _configuration.GetSection("Jwt");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, admin.IdUsuario.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, admin.Correo),
                new Claim(ClaimTypes.Name, $"{admin.Nombre} {admin.Apellido}"),
                new Claim(ClaimTypes.Role, "Admin")
            };

            //var expires = DateTime.UtcNow.AddMinutes(
            //    double.Parse(jwtSettings["ExpiresMinutes"] ?? "60")
            //);

            var expires = DateTime.UtcNow.AddHours(1);

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: expires,
                signingCredentials: creds
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            var response = new AdminLoginResponseDto
            {
                Token = tokenString,
                ExpiraEn = expires,
                IdUsuario = admin.IdUsuario,
                NombreCompleto = $"{admin.Nombre} {admin.Apellido}",
                Correo = admin.Correo,
                Rol = "Admin"
            };

            return Ok(response);
        }



    }
}
