using Blu_Ring_Pro.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Blu_Ring_Pro.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmpleadosController : ControllerBase
    {
        private readonly BluRingProContext _context;

        public EmpleadosController(BluRingProContext context)
        {
            _context = context;
        }

        // ?? Obtener todos los empleados
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var empleados = await _context.Empleados
                .Select(e => new
                {
                    e.IdEmpleado,
                    e.CodigoEmpleado,
                    e.Departamento,
                    e.Nombre,
                    e.ApellidoPaterno,
                    e.ApellidoMaterno
                })
                .ToListAsync();
            return Ok(empleados);
        }

        // ?? Obtener empleado por ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var empleado = await _context.Empleados.FindAsync(id);
            if (empleado == null) return NotFound();
            return Ok(empleado);
        }

        // ?? Crear empleado
        [HttpPost]
        public async Task<IActionResult> Insert([FromBody] Empleados nuevoEmpleado)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            _context.Empleados.Add(nuevoEmpleado);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = nuevoEmpleado.IdEmpleado }, nuevoEmpleado);
        }

        // ?? Actualizar empleado
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Empleados empleado)
        {
            var reg = await _context.Empleados.FindAsync(id);
            if (reg == null) return NotFound();

            reg.CodigoEmpleado = empleado.CodigoEmpleado;
            reg.Departamento = empleado.Departamento;
            reg.Nombre = empleado.Nombre;
            reg.ApellidoPaterno = empleado.ApellidoPaterno;
            reg.ApellidoMaterno = empleado.ApellidoMaterno;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // ?? Eliminar empleado
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var reg = await _context.Empleados.FindAsync(id);
            if (reg == null) return NotFound();

            _context.Empleados.Remove(reg);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
