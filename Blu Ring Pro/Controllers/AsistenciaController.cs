using Blu_Ring_Pro.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Blu_Ring_Pro.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AsistenciaController : ControllerBase
    {
        private readonly BluRingProContext _context;

        public AsistenciaController(BluRingProContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var asistencias = await _context.RegistroAsistencia
                .Include(r => r.IdEmpleadoNavigation)
                .Select(r => new
                {
                    r.IdRegistro,
                    r.IdEmpleado,
                    r.IdEmpleadoNavigation.CodigoEmpleado,
                    r.IdEmpleadoNavigation.Nombre,
                    r.IdEmpleadoNavigation.ApellidoPaterno,
                    r.IdEmpleadoNavigation.ApellidoMaterno,
                    r.Fecha,
                    r.Hora,
                    r.TipoRegistro,
                    r.Dispositivo
                })
                .ToListAsync();

            return Ok(asistencias);
        }

        [HttpPost]
        public async Task<IActionResult> Insert([FromBody] RegistroAsistencia asistencia)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            // Obtener empleado completo
            var empleado = await _context.Empleados.AsNoTracking()
                .FirstOrDefaultAsync(e => e.IdEmpleado == asistencia.IdEmpleado);
            if (empleado == null)
                return BadRequest(new { error = "Empleado no encontrado" });

            // Asignar NombreEmpleado y evitar tracking doble
            asistencia.NombreEmpleado = $"{empleado.Nombre} {empleado.ApellidoPaterno} {empleado.ApellidoMaterno}";
            asistencia.IdEmpleadoNavigation = null;

            _context.RegistroAsistencia.Add(asistencia);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetAll), new { id = asistencia.IdRegistro }, asistencia);
        }

        // PUT: api/Asistencia/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] RegistroAsistencia asistencia)
        {
            var a = await _context.RegistroAsistencia.FindAsync(id);
            if (a == null) return NotFound();

            var empleado = await _context.Empleados.AsNoTracking()
                .FirstOrDefaultAsync(e => e.IdEmpleado == asistencia.IdEmpleado);
            if (empleado == null)
                return BadRequest(new { error = "Empleado no encontrado" });

            // Actualizar campos
            a.IdEmpleado = asistencia.IdEmpleado;
            a.Fecha = asistencia.Fecha;
            a.Hora = asistencia.Hora;
            a.TipoRegistro = asistencia.TipoRegistro;
            a.Dispositivo = asistencia.Dispositivo;
            a.NombreEmpleado = $"{empleado.Nombre} {empleado.ApellidoPaterno} {empleado.ApellidoMaterno}";
            a.IdEmpleadoNavigation = null; // evitar tracking doble

            await _context.SaveChangesAsync();
            return NoContent();
        }



        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var registro = await _context.RegistroAsistencia.FindAsync(id);
            if (registro == null) return NotFound();

            _context.RegistroAsistencia.Remove(registro);
            await _context.SaveChangesAsync();

            await _context.Database.ExecuteSqlRawAsync("CALL sp_generar_resumen_asistencia({0})", registro.Fecha);
            await _context.Database.ExecuteSqlRawAsync("CALL sp_actualizar_prenomina_semana_extra({0})", registro.Fecha);

            return NoContent();
        }
    }
}
