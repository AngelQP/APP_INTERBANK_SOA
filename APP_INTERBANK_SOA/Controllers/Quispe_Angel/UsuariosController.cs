using APP_INTERBANK_SOA.DTO.Quispe_Angel.Usuario;
using APP_INTERBANK_SOA.Models;
using APP_INTERBANK_SOA.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace APP_INTERBANK_SOA.Controllers.Quispe_Angel
{
    [Route("api/[controller]")]
    //[Authorize(Roles = "Admin")]
    //[Authorize]
    [ApiController]
    public class UsuariosController : ControllerBase
    {
        private readonly InterbankContext _context;
        private readonly IConfiguration _configuration;

        public UsuariosController(InterbankContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        // [AllowAnonymous] // Si se desea que el metodo quede publico sin restricciones
        [HttpPost("registrarCliente")]
        public async Task<ActionResult<RegisterUsuarioConCuentaDto>> Registrar([FromBody] RegisterUsuarioConCuentaDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // 1. Validar TipoDocumento
            var tipoDoc = dto.TipoDocumento.ToUpper().Trim();
            if (tipoDoc != "DNI" && tipoDoc != "CE")
            {
                return BadRequest(new
                {
                    message = "El tipoDocumento debe ser 'DNI' o 'CE'."
                });
            }

            // 1.1 Validaciones de cantidad de digitos segun tipoDocumento

            if (tipoDoc == "DNI" && dto.NumeroDocumento.Trim().Length != 8)
            {
                return BadRequest(new
                {
                    message = "El número de documento para DNI debe tener 8 dígitos."
                });
            }

            if (tipoDoc == "CE" && (dto.NumeroDocumento.Trim().Length < 9 || dto.NumeroDocumento.Trim().Length > 12))
            {
                return BadRequest(new
                {
                    message = "El número de documento para CE debe tener entre 9 y 12 dígitos."
                });
            }

            // 2. Validar unicidad de correo
            if (await _context.Usuarios.AnyAsync(u => u.Correo == dto.Correo))
            {
                return Conflict(new
                {
                    message = "Ya existe un usuario con ese correo."
                });
            }

            // 3. Validar documento unico
            if (await _context.Usuarios.AnyAsync(u =>
                    u.TipoDocumento == tipoDoc &&
                    u.NumeroDocumento == dto.NumeroDocumento))
            {
                return Conflict(new
                {
                    message = "Ya existe un usuario con ese tipo y número de documento."
                });
            }

            // 4  Validar número de cuenta tenga 14 digitos

            if (dto.NumeroCuenta.Trim().Length != 14)
            {
                return BadRequest(new
                {
                    message = "El número de cuenta debe tener 14 digítos"
                });
            }

            // 4.1 Validar número de cuenta unico
            if (await _context.Cuenta.AnyAsync(c => c.NumeroCuenta == dto.NumeroCuenta))
            {
                return Conflict(new
                {
                    message = "El número de cuenta ya está registrado."
                });
            }

            // Validar número de telefono tenga 9 digitos

            if (dto.Telefono.Trim().Length != 9)
            {
                return BadRequest(new
                {
                    message = "El número de telefono debe tener 9 digítos"
                });
            }

            // 5. Obtener rol "Cliente"
            var rolCliente = await _context.Rols
                .FirstOrDefaultAsync(r => r.Nombre == "Cliente");

            if (rolCliente == null)
            {
                return StatusCode(500, new
                {
                    message = "No se encontró el rol 'Cliente' en la base de datos."
                });
            }

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // 6. Crear Usuario
                var usuario = new Usuario
                {
                    Nombre = dto.Nombre,
                    Apellido = dto.Apellido,
                    Correo = dto.Correo,
                    ContrasenaHash = PasswordHasher.Hash(dto.Password),
                    TipoDocumento = tipoDoc,
                    NumeroDocumento = dto.NumeroDocumento,
                    Telefono = dto.Telefono,
                    Estado = "ACTIVO",
                    FechaRegistro = DateTime.UtcNow,
                    UltimoLogin = null,
                    IntentosFallidos = 0,
                    BloqueadoHasta = null,
                    IdRol = rolCliente.IdRol
                };

                _context.Usuarios.Add(usuario);
                await _context.SaveChangesAsync();

                // 7. Crear Cuenta principal de ahorro
                var cuenta = new Cuentum   // o Cuentum según tu entidad
                {
                    NumeroCuenta = dto.NumeroCuenta,
                    TipoCuenta = "AHORRO",      // Regla: cuenta principal de ahorro
                    SaldoDisponible = 0m,
                    Moneda = "PEN",
                    FechaCreacion = DateTime.UtcNow,
                    FechaCierre = null,
                    MotivoCierre = null,
                    Estado = "ACTIVA",
                    IdUsuario = usuario.IdUsuario,
                    IdCuentaPadre = null       // por tus reglas de monedero/ahorro
                };

                _context.Cuenta.Add(cuenta);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                var respuesta = new UsuarioConCuentaDto
                {
                    id = usuario.IdUsuario,
                    Nombre = usuario.Nombre,
                    Apellido = usuario.Apellido,
                    Correo = usuario.Correo,
                    TipoDocumento = usuario.TipoDocumento,
                    NumeroDocumento = usuario.NumeroDocumento,
                    Telefono = usuario.Telefono,
                    Estado = usuario.Estado,
                    FechaRegistro = usuario.FechaRegistro,
                    IdCuenta = cuenta.IdCuenta,
                    NumeroCuenta = cuenta.NumeroCuenta,
                    TipoCuenta = cuenta.TipoCuenta,
                    Moneda = cuenta.Moneda,
                    SaldoDisponible = cuenta.SaldoDisponible,
                    EstadoCuenta = cuenta.Estado
                };

                return CreatedAtAction(
                    nameof(ObtenerPorId),
                    new { id = usuario.IdUsuario },
                    respuesta
                );
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        [HttpGet("clientexId/{id:int}")]
        public async Task<ActionResult<UsuarioConCuentaDto>> ObtenerPorId(int id)
        {
            var usuario = await _context.Usuarios
                .Include(u => u.Cuenta)
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.IdUsuario == id);

            if (usuario == null)
                // Se retorna 404 NOT FOUND
                return StatusCode(404, new
                {
                    message = "ID de cliente no existe"
                });

            var cuentaPrincipal = usuario.Cuenta.FirstOrDefault();

            var respuesta = new UsuarioConCuentaDto
            {
                id = usuario.IdUsuario,
                Nombre = usuario.Nombre,
                Apellido = usuario.Apellido,
                Correo = usuario.Correo,
                TipoDocumento = usuario.TipoDocumento,
                NumeroDocumento = usuario.NumeroDocumento,
                Telefono = usuario.Telefono,
                Estado = usuario.Estado,
                FechaRegistro = usuario.FechaRegistro,
                IdCuenta = cuentaPrincipal?.IdCuenta ?? 0,
                NumeroCuenta = cuentaPrincipal?.NumeroCuenta ?? string.Empty,
                TipoCuenta = cuentaPrincipal?.TipoCuenta ?? string.Empty,
                Moneda = cuentaPrincipal?.Moneda ?? string.Empty,
                SaldoDisponible = cuentaPrincipal?.SaldoDisponible ?? 0m,
                EstadoCuenta = cuentaPrincipal?.Estado ?? string.Empty
            };

            return Ok(respuesta);
        }

        // GET: api/usuarios/listarClientes?cantidad=5
        [HttpGet("listarClientes")]
        public async Task<ActionResult<IEnumerable<UsuarioConCuentaDto>>> Listar([FromQuery] int? cantidad)
        {
            int take;

            if (cantidad == -1)
            {
                take = int.MaxValue;     // traer todos
            }
            else if (cantidad == null || cantidad == 0)
            {
                take = 5;                // default
            }
            else if (cantidad > 10)
            {
                take = 10;               // máximo
            }
            else
            {
                take = cantidad.Value;   // 1–10
            }

            // 2. Traer usuarios con rol "Cliente"
            var usuarios = await _context.Usuarios
                .Include(u => u.Cuenta)
                .Include(u => u.IdRolNavigation)
                .Where(u => u.IdRolNavigation.Nombre == "Cliente")
                .OrderBy(u => u.IdUsuario)   // para que el listado sea determinístico
                .Take(take)
                .ToListAsync();

            // 3. Mapear a DTO
            var resultado = usuarios.Select(u =>
            {
                var cuentaPrincipal = u.Cuenta.FirstOrDefault();

                return new UsuarioConCuentaDto
                {
                    id = u.IdUsuario,
                    Nombre = u.Nombre,
                    Apellido = u.Apellido,
                    Correo = u.Correo,
                    TipoDocumento = u.TipoDocumento,
                    NumeroDocumento = u.NumeroDocumento,
                    Telefono = u.Telefono,
                    Estado = u.Estado,
                    FechaRegistro = u.FechaRegistro,
                    IdCuenta = cuentaPrincipal?.IdCuenta ?? 0,
                    NumeroCuenta = cuentaPrincipal?.NumeroCuenta ?? string.Empty,
                    TipoCuenta = cuentaPrincipal?.TipoCuenta ?? string.Empty,
                    Moneda = cuentaPrincipal?.Moneda ?? string.Empty,
                    SaldoDisponible = cuentaPrincipal?.SaldoDisponible ?? 0m,
                    EstadoCuenta = cuentaPrincipal?.Estado ?? string.Empty
                };
            }).ToList();

            return Ok(resultado);
        }

        // PUT: api/usuarios/{id}
        [HttpPut("actualizarCliente/{id:int}")]
        public async Task<ActionResult<UsuarioConCuentaDto>> Actualizar(int id, [FromBody] UpdateUsuarioDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // 1. Buscar usuario con sus cuentas
            var usuario = await _context.Usuarios
                .Include(u => u.Cuenta)          // ajusta el nombre de la navegación si es necesario
                .FirstOrDefaultAsync(u => u.IdUsuario == id);

            if (usuario == null)
                return NotFound(new { message = "Usuario no encontrado." });

            // 2. Normalizar campos solo si vienen
            string tipoDocNuevo = usuario.TipoDocumento;        // valor actual por defecto
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

            string estadoNuevo = usuario.Estado;                // valor actual por defecto
            bool cambiarEstado = false;
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
                cambiarEstado = true;
            }

            string numeroDocNuevo = usuario.NumeroDocumento;
            if (!string.IsNullOrWhiteSpace(dto.NumeroDocumento))
            {
                numeroDocNuevo = dto.NumeroDocumento.Trim();
            }

            // 3. Validar unicidad del correo si se envía y cambia
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

                usuario.Correo = correoNormalizado;
            }

            // 4. Validar unicidad de (tipoDocumento, numeroDocumento) si cambia alguno
            bool cambiaTipoODoc =
                (!string.IsNullOrWhiteSpace(dto.TipoDocumento) && tipoDocNuevo != usuario.TipoDocumento) ||
                (!string.IsNullOrWhiteSpace(dto.NumeroDocumento) && numeroDocNuevo != usuario.NumeroDocumento);

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

                usuario.TipoDocumento = tipoDocNuevo;
                usuario.NumeroDocumento = numeroDocNuevo;
            }

            // 5. Actualizar otros campos solo si se enviaron
            if (!string.IsNullOrWhiteSpace(dto.Nombre))
                usuario.Nombre = dto.Nombre.Trim();

            if (!string.IsNullOrWhiteSpace(dto.Apellido))
                usuario.Apellido = dto.Apellido.Trim();

            if (!string.IsNullOrWhiteSpace(dto.Telefono))
                usuario.Telefono = dto.Telefono.Trim();

            // 6. Cambiar estado de usuario y propagar a cuentas si se envió Estado
            if (cambiarEstado)
            {
                usuario.Estado = estadoNuevo;

                if (estadoNuevo == "INACTIVO" || estadoNuevo == "ACTIVO")
                {
                    foreach (var cuenta in usuario.Cuenta)   // Cuentum / Cuenta según tu modelo
                    {
                        cuenta.Estado = estadoNuevo;
                    }
                }
            }

            await _context.SaveChangesAsync();

            // 7. Preparar respuesta
            var cuentaPrincipal = usuario.Cuenta.FirstOrDefault(); // ajusta nombre si es necesario

            var respuesta = new UsuarioConCuentaDto
            {
                id = usuario.IdUsuario,
                Nombre = usuario.Nombre,
                Apellido = usuario.Apellido,
                Correo = usuario.Correo,
                TipoDocumento = usuario.TipoDocumento,
                NumeroDocumento = usuario.NumeroDocumento,
                Telefono = usuario.Telefono,
                Estado = usuario.Estado,
                FechaRegistro = usuario.FechaRegistro,
                IdCuenta = cuentaPrincipal?.IdCuenta ?? 0,
                NumeroCuenta = cuentaPrincipal?.NumeroCuenta ?? string.Empty,
                TipoCuenta = cuentaPrincipal?.TipoCuenta ?? string.Empty,
                Moneda = cuentaPrincipal?.Moneda ?? string.Empty,
                SaldoDisponible = cuentaPrincipal?.SaldoDisponible ?? 0m,
                EstadoCuenta = cuentaPrincipal?.Estado ?? string.Empty
            };

            return Ok(respuesta);
        }

        // DELETE: api/usuarios/{id}
        [HttpDelete("eliminarCliente/{id:int}")]
        public async Task<IActionResult> Eliminar(int id)
        {
            // 1. Buscar usuario con sus cuentas
            var usuario = await _context.Usuarios
                .Include(u => u.Cuenta)              // Incluimos las cuentas asociadas
                .FirstOrDefaultAsync(u => u.IdUsuario == id);

            if (usuario == null)
                return NotFound(new { message = "Usuario no encontrado." });

            // 2. Marcar usuario como INACTIVO
            usuario.Estado = "INACTIVO";

            // 3. Marcar todas las cuentas asociadas como INACTIVO
            foreach (var cuenta in usuario.Cuenta)   // 'Cuenta' es la colección de Cuentum
            {
                cuenta.Estado = "INACTIVO";
            }

            await _context.SaveChangesAsync();

            // 4. Devolver 204 No Content (borrado lógico exitoso)
            return NoContent();
        }


        // POST: api/usuarios/login
        //[AllowAnonymous]  // público
        [HttpPost("login")]
        public async Task<ActionResult<UserLoginResponseDto>> Login([FromBody] UserLoginDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var numeroDoc = dto.NumeroDocumento.Trim();

            // 1. Buscar usuario por N° documento, aceptando DNI o Carnet de extranjería y rol Cliente
            var usuario = await _context.Usuarios
                .Include(u => u.IdRolNavigation)
                .FirstOrDefaultAsync(u =>
                    u.NumeroDocumento == numeroDoc &&
                    (
                        u.TipoDocumento.ToUpper() == "DNI" ||
                        u.TipoDocumento.ToUpper() == "CARNET DE EXTRANJERIA"
                    ) &&
                    u.IdRolNavigation.Nombre == "Cliente"
                );

            if (usuario == null)
            {
                return Unauthorized(new { message = "Credenciales inválidas." });
            }

            // 2. Validar estado INACTIVO
            var estado = (usuario.Estado ?? string.Empty).Trim().ToUpper();
            if (estado == "INACTIVO")
            {
                return StatusCode(StatusCodes.Status403Forbidden, new
                {
                    message = "La cuenta de usuario se encuentra INACTIVA. Contacte con el banco."
                });
            }

            // 3. Validar bloqueo por intentos fallidos
            if (usuario.BloqueadoHasta.HasValue && usuario.BloqueadoHasta > DateTime.UtcNow)
            {
                return StatusCode(StatusCodes.Status403Forbidden, new
                {
                    message = $"Cuenta bloqueada hasta {usuario.BloqueadoHasta.Value:yyyy-MM-dd HH:mm:ss} UTC."
                });
            }

            // 4. Verificar contraseña
            var passwordOk = PasswordHasher.Verify(dto.Password, usuario.ContrasenaHash);

            const int MAX_INTENTOS = 5;
            const int MINUTOS_BLOQUEO = 15;

            if (!passwordOk)
            {
                usuario.IntentosFallidos += 1;

                if (usuario.IntentosFallidos >= MAX_INTENTOS)
                {
                    usuario.BloqueadoHasta = DateTime.UtcNow.AddMinutes(MINUTOS_BLOQUEO);
                    usuario.IntentosFallidos = 0;
                }

                await _context.SaveChangesAsync();

                return Unauthorized(new { message = "Credenciales inválidas." });
            }

            // 5. Resetear intentos y actualizar último login
            usuario.IntentosFallidos = 0;
            usuario.BloqueadoHasta = null;
            usuario.UltimoLogin = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            // 6. Generar token JWT
            var jwtSettings = _configuration.GetSection("Jwt");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
        new Claim(JwtRegisteredClaimNames.Sub, usuario.IdUsuario.ToString()),
        new Claim(JwtRegisteredClaimNames.UniqueName, usuario.NumeroDocumento),
        new Claim(ClaimTypes.Name, $"{usuario.Nombre} {usuario.Apellido}"),
        new Claim(ClaimTypes.Role, usuario.IdRolNavigation.Nombre) // "Cliente"
    };

            var expires = DateTime.UtcNow.AddMinutes(
                double.Parse(jwtSettings["ExpiresMinutes"] ?? "60")
            );

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: expires,
                signingCredentials: creds
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            var response = new UserLoginResponseDto
            {
                Token = tokenString,
                ExpiraEn = expires,
                IdUsuario = usuario.IdUsuario,
                NombreCompleto = $"{usuario.Nombre} {usuario.Apellido}",
                NumeroDocumento = usuario.NumeroDocumento,
                TipoDocumento = usuario.TipoDocumento,
                Correo = usuario.Correo,
                Rol = usuario.IdRolNavigation.Nombre,
                Estado = usuario.Estado!
            };

            return Ok(response);
        }




    }
}
