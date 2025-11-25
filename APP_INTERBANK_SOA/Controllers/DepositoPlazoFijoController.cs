using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using APP_INTERBANK_SOA.Models;

namespace APP_INTERBANK_SOA.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class DepositoPlazoController : ControllerBase
	{
		private readonly InterbankContext _context;

		public DepositoPlazoController(InterbankContext context)
		{
			_context = context;
		}

		// ===========================================================
		// GET: api/DepositoPlazo
		// Listar todos los depósitos a plazo
		// ===========================================================
		[HttpGet]
		public async Task<ActionResult<IEnumerable<DepositoPlazo>>> GetDepositos()
		{
			return await _context.DepositoPlazos
				.Include(dp => dp.IdCuentaNavigation)
				.ToListAsync();
		}

		// ===========================================================
		// GET: api/DepositoPlazo/5
		// Obtener un depósito a plazo específico
		// ===========================================================
		[HttpGet("{id}")]
		public async Task<ActionResult<DepositoPlazo>> GetDeposito(int id)
		{
			var deposito = await _context.DepositoPlazos
				.Include(dp => dp.IdCuentaNavigation)
				.FirstOrDefaultAsync(dp => dp.IdDeposito == id);

			if (deposito == null)
				return NotFound(new { mensaje = "El depósito a plazo no existe." });

			return deposito;
		}

		// ===========================================================
		// POST: api/DepositoPlazo
		// Crear (Aperturar) un depósito a plazo fijo
		// ===========================================================
		[HttpPost]
		public async Task<ActionResult<DepositoPlazo>> CrearDeposito(DepositoPlazo deposito)
		{
			// ---------- VALIDACIONES INTERBANK ----------

			// 1. Crear solo con cuentas existentes
			if (!_context.Cuenta.Any(c => c.IdCuenta == deposito.IdCuenta))
				return BadRequest(new { mensaje = "La cuenta asociada no existe." });

			// 2. Validación de montos Interbank
			// Soles: 2000 - 500000
			// Dólares: 1000 - 300000
			var cuenta = await _context.Cuenta.FindAsync(deposito.IdCuenta);

			bool esSoles = cuenta.Moneda == "SOLES" || cuenta.Moneda == "S";
			bool esDolares = cuenta.Moneda == "DOLARES" || cuenta.Moneda == "USD";

			if (esSoles)
			{
				if (deposito.Monto < 2000 || deposito.Monto > 500000)
					return BadRequest(new { mensaje = "En soles, el monto debe ser entre 2,000 y 500,000." });
			}
			else if (esDolares)
			{
				if (deposito.Monto < 1000 || deposito.Monto > 300000)
					return BadRequest(new { mensaje = "En dólares, el monto debe ser entre 1,000 y 300,000." });
			}
			else
			{
				return BadRequest(new { mensaje = "La moneda de la cuenta no es válida para depósitos a plazo." });
			}

			// 3. Plazo mínimo Interbank: 30 días
			if (deposito.PlazoDias < 30)
				return BadRequest(new { mensaje = "El plazo mínimo permitido es 30 días." });

			// 4. Tasa anual debe ser positiva
			if (deposito.TasaAnual <= 0)
				return BadRequest(new { mensaje = "La tasa anual debe ser mayor a cero." });

			// 5. Fecha de apertura no puede ser antigua
			if (deposito.FechaApertura.Date < DateTime.Today)
				return BadRequest(new { mensaje = "La fecha de apertura debe ser hoy o una fecha futura." });

			// 6. Estado inicial por defecto
			deposito.Estado = "ACTIVO";

			// Guardar registro
			_context.DepositoPlazos.Add(deposito);
			await _context.SaveChangesAsync();

			return CreatedAtAction(nameof(GetDeposito), new { id = deposito.IdDeposito }, deposito);
		}

		// ===========================================================
		// PUT: api/DepositoPlazo/5
		// Modificar un depósito a plazo (solo si no está vencido)
		// ===========================================================
		[HttpPut("{id}")]
		public async Task<IActionResult> ActualizarDeposito(int id, DepositoPlazo deposito)
		{
			if (id != deposito.IdDeposito)
				return BadRequest(new { mensaje = "El ID no coincide con el del depósito." });

			var depositoDB = await _context.DepositoPlazos.FindAsync(id);

			if (depositoDB == null)
				return NotFound(new { mensaje = "El depósito no existe." });

			// No se puede modificar un depósito vencido
			if (depositoDB.Estado == "VENCIDO")
				return BadRequest(new { mensaje = "Los depósitos vencidos no pueden modificarse." });

			// Validaciones mínimas
			if (deposito.TasaAnual <= 0)
				return BadRequest(new { mensaje = "La tasa anual debe ser mayor a cero." });

			if (deposito.PlazoDias < 30)
				return BadRequest(new { mensaje = "El plazo mínimo es 30 días." });

			// Actualizar campos permitidos
			depositoDB.TasaAnual = deposito.TasaAnual;
			depositoDB.PlazoDias = deposito.PlazoDias;
			depositoDB.RenovacionAutomatica = deposito.RenovacionAutomatica;
			depositoDB.Estado = deposito.Estado;

			await _context.SaveChangesAsync();

			return Ok(new { mensaje = "Depósito actualizado correctamente." });
		}

		// ===========================================================
		// DELETE: api/DepositoPlazo/5
		// Cancelar o eliminar un depósito a plazo
		// ===========================================================
		[HttpDelete("{id}")]
		public async Task<IActionResult> EliminarDeposito(int id)
		{
			var deposito = await _context.DepositoPlazos.FindAsync(id);

			if (deposito == null)
				return NotFound(new { mensaje = "El depósito no existe." });

			// INTERBANK: solo se puede cancelar si está activo y aún no vence
			if (deposito.Estado != "ACTIVO")
				return BadRequest(new { mensaje = "Solo se pueden eliminar depósitos activos no vencidos." });

			_context.DepositoPlazos.Remove(deposito);
			await _context.SaveChangesAsync();

			return Ok(new { mensaje = "Depósito eliminado correctamente." });
		}
	}
}