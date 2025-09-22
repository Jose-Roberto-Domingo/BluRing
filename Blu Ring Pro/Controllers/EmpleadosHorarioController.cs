using Blu_Ring_Pro.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Blu_Ring_Pro.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmpleadosHorarioController : ControllerBase
    {
        private readonly BluRingProContext _context;

        public EmpleadosHorarioController(BluRingProContext context)
        {
            _context = context;
        }

        // GET: api/EmpleadosHorario
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            // Proyectar los datos a un objeto que tenga nombre completo y propiedades consistentes
            var horarios = await _context.EmpleadosHorarios
                .Include(h => h.IdEmpleadoNavigation) // traer datos del empleado
                .Select(h => new
                {
                    h.IdHorario,
                    h.IdEmpleado,
                    NombreEmpleado = h.IdEmpleadoNavigation != null
                        ? $"{h.IdEmpleadoNavigation.Nombre} {h.IdEmpleadoNavigation.ApellidoPaterno} {h.IdEmpleadoNavigation.ApellidoMaterno}"
                        : string.Empty,
                    h.HoraEntradaHorario,
                    h.HoraSalidaHorario,
                    h.Jornada
                })
                .ToListAsync();

            return Ok(horarios);
        }

        // GET: api/EmpleadosHorario/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var h = await _context.EmpleadosHorarios
                .Include(hh => hh.IdEmpleadoNavigation)
                .Where(hh => hh.IdHorario == id)
                .Select(hh => new
                {
                    hh.IdHorario,
                    hh.IdEmpleado,
                    NombreEmpleado = hh.IdEmpleadoNavigation != null
                        ? $"{hh.IdEmpleadoNavigation.Nombre} {hh.IdEmpleadoNavigation.ApellidoPaterno} {hh.IdEmpleadoNavigation.ApellidoMaterno}"
                        : string.Empty,
                    hh.HoraEntradaHorario,
                    hh.HoraSalidaHorario,
                    hh.Jornada
                })
                .FirstOrDefaultAsync();

            if (h == null) return NotFound();
            return Ok(h);
        }

        // POST: api/EmpleadosHorario
        [HttpPost]
        public async Task<IActionResult> Insert([FromBody] EmpleadosHorario horario)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var empleado = await _context.Empleados.AsNoTracking()
                .FirstOrDefaultAsync(e => e.IdEmpleado == horario.IdEmpleado);
            if (empleado == null)
                return BadRequest(new { error = "Empleado no encontrado" });

            horario.NombreEmpleado = $"{empleado.Nombre} {empleado.ApellidoPaterno} {empleado.ApellidoMaterno}";
            horario.IdEmpleadoNavigation = null; // 🛑 evitar tracking doble

            _context.EmpleadosHorarios.Add(horario);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = horario.IdHorario }, horario);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] EmpleadosHorario horario)
        {
            var h = await _context.EmpleadosHorarios.FindAsync(id);
            if (h == null) return NotFound();

            var empleado = await _context.Empleados.AsNoTracking()
                .FirstOrDefaultAsync(e => e.IdEmpleado == horario.IdEmpleado);
            if (empleado == null)
                return BadRequest(new { error = "Empleado no encontrado" });

            h.IdEmpleado = horario.IdEmpleado;
            h.HoraEntradaHorario = horario.HoraEntradaHorario;
            h.HoraSalidaHorario = horario.HoraSalidaHorario;
            h.Jornada = horario.Jornada;
            h.NombreEmpleado = $"{empleado.Nombre} {empleado.ApellidoPaterno} {empleado.ApellidoMaterno}";
            h.IdEmpleadoNavigation = null; // 🛑 evitar tracking doble

            await _context.SaveChangesAsync();
            return NoContent();
        }


        // DELETE: api/EmpleadosHorario/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var h = await _context.EmpleadosHorarios.FindAsync(id);
            if (h == null) return NotFound();
            _context.EmpleadosHorarios.Remove(h);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
