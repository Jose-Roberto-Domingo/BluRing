using Blu_Ring_Pro.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Route("api/[controller]")]
[ApiController]
public class PrenominaController : ControllerBase
{
    private readonly BluRingProContext _context;

    public PrenominaController(BluRingProContext context)
    {
        _context = context;
    }

    // GET: api/Prenomina
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Prenomina>>> GetAll()
    {
        return await _context.Prenominas.ToListAsync();
    }

    // GET: api/Prenomina/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Prenomina>> GetById(int id)
    {
        var pre = await _context.Prenominas.FindAsync(id);
        if (pre == null) return NotFound();
        return pre;
    }

    // POST: api/Prenomina
    [HttpPost]
    public async Task<ActionResult<Prenomina>> Insert([FromBody] Prenomina nuevaPrenomina)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        _context.Prenominas.Add(nuevaPrenomina);
        await _context.SaveChangesAsync();

        // Recalcular prenomina automáticamente para la semana
        await _context.Database.ExecuteSqlRawAsync("CALL sp_actualizar_prenomina_semana_extra({0})", nuevaPrenomina.Semana);

        return CreatedAtAction(nameof(GetById), new { id = nuevaPrenomina.IdPrenomina }, nuevaPrenomina);
    }

    // PUT: api/Prenomina/5
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] Prenomina updatedPrenomina)
    {
        if (id != updatedPrenomina.IdPrenomina) return BadRequest();

        _context.Entry(updatedPrenomina).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
            // Recalcular prenomina después de cambios
            await _context.Database.ExecuteSqlRawAsync("CALL sp_actualizar_prenomina_semana_extra({0})", updatedPrenomina.Semana);
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!_context.Prenominas.Any(e => e.IdPrenomina == id)) return NotFound();
            throw;
        }

        return NoContent();
    }

    // DELETE: api/Prenomina/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var pre = await _context.Prenominas.FindAsync(id);
        if (pre == null) return NotFound();

        _context.Prenominas.Remove(pre);
        await _context.SaveChangesAsync();

        // Recalcular prenomina después de eliminación
        await _context.Database.ExecuteSqlRawAsync("CALL sp_actualizar_prenomina_semana_extra({0})", pre.Semana);

        return NoContent();
    }
}
