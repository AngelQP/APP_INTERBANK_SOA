using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using APP_INTERBANK_SOA.Models;

namespace APP_INTERBANK_SOA.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class PagoAutomaticoController : ControllerBase
	{
		private readonly InterbankContext _context;

		public PagoAutomaticoController(InterbankContext context)
		{
			_context = context;
		}

		// ===========================================================
		// GET: api/PagoAutomatico
		// Listar todos los pagos automáticos
		// ===========================================================
		[HttpGet]
		public async Task<ActionResult<IEnumerable<Pago>>> GetPagosAutomaticos()
		{
			return await _context.Pagos
				.Include(p => p.IdCuentaNavigation)
				.ToListAsync();
		}

		// ===========================================================
		// GET: api/PagoAutomatico/5
		// Obtener un pago automático específico
		// ===========================================================
		[HttpGet("{id}")]
		public async Task<ActionResult<Pago>> GetPagoAutomatico(int id)
		{
			var pago = await _context.Pagos
				.Include(p => p.IdCuentaNavigation)
				.FirstOrDefaultAsync(p => p.IdPago == id);

			if (pago == null)
				return NotFound(new { mensaje = "El pago automático no existe." });

			return pago;
		}

		// ===========================================================
		// POST: api/PagoAutomatico
		// Registrar un nuevo pago automático
		// ===========================================================
		[HttpPost]
		public async Task<ActionResult<Pago>> CrearPagoAutomatico(Pago pago)
		{
			// ---------- VALIDACIONES ----------
			if (string.IsNullOrEmpty(pago.TipoPago))
				return BadRequest(new { mensaje = "El tipo de pago es obligatorio." });

			if (pago.Monto <= 0)
				return BadRequest(new { mensaje = "El monto debe ser mayor a cero." });

			if (!_context.Cuenta.Any(c => c.IdCuenta == pago.IdCuenta))
				return BadRequest(new { mensaje = "La cuenta asociada no existe." });

			if (pago.Fecha < DateTime.Now)
				return BadRequest(new { mensaje = "La fecha del pago debe ser futura." });

			if (string.IsNullOrEmpty(pago.Estado))
				pago.Estado = "Pendiente"; // Estado por defecto de un pago automático

			// Guardar
			_context.Pagos.Add(pago);
			await _context.SaveChangesAsync();

			return CreatedAtAction(nameof(GetPagoAutomatico), new { id = pago.IdPago }, pago);
		}

		// ===========================================================
		// PUT: api/PagoAutomatico/5
		// Actualizar un pago automático
		// ===========================================================
		[HttpPut("{id}")]
		public async Task<IActionResult> ActualizarPagoAutomatico(int id, Pago pago)
		{
			if (id != pago.IdPago)
				return BadRequest(new { mensaje = "El ID del pago no coincide." });

			var pagoDB = await _context.Pagos.FindAsync(id);

			if (pagoDB == null)
				return NotFound(new { mensaje = "El pago automático no existe." });

			// Validaciones
			if (pago.Monto <= 0)
				return BadRequest(new { mensaje = "El monto debe ser mayor a cero." });

			if (pago.Fecha < DateTime.Now)
				return BadRequest(new { mensaje = "La nueva fecha debe ser futura." });

			// Campos actualizables
			pagoDB.TipoPago = pago.TipoPago;
			pagoDB.Monto = pago.Monto;
			pagoDB.Fecha = pago.Fecha;
			pagoDB.Estado = pago.Estado;

			await _context.SaveChangesAsync();

			return Ok(new { mensaje = "Pago automático actualizado correctamente." });
		}

		// ===========================================================
		// DELETE: api/PagoAutomatico/5
		// Eliminar un pago automático
		// ===========================================================
		[HttpDelete("{id}")]
		public async Task<IActionResult> EliminarPagoAutomatico(int id)
		{
			var pago = await _context.Pagos.FindAsync(id);

			if (pago == null)
				return NotFound(new { mensaje = "El pago automático no existe." });

			// Validación: solo pagos pendientes pueden eliminarse
			if (pago.Estado != "Pendiente")
				return BadRequest(new { mensaje = "Solo se pueden eliminar pagos pendientes o no ejecutados." });

			_context.Pagos.Remove(pago);
			await _context.SaveChangesAsync();

			return Ok(new { mensaje = "Pago automático eliminado correctamente." });
		}
	}
}