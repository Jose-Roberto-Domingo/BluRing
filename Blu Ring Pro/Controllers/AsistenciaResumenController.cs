using Blu_Ring_Pro.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Route("api/[controller]")]
[ApiController]
public class AsistenciaResumenController : ControllerBase
{
    private readonly BluRingProContext _context;

    public AsistenciaResumenController(BluRingProContext context)
    {
        _context = context;
    }

    // Obtener todos los registros de resumen
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var resumen = await _context.AsistenciaResumen
            .Include(r => r.IdEmpleadoNavigation) // trae info de empleado
            .Select(r => new
            {
                r.IdResumen,
                r.IdEmpleado,
                r.NombreEmpleado,
                Departamento = r.IdEmpleadoNavigation.Departamento,
                Fecha = r.Fecha,
                HoraEntrada = r.HoraEntrada,
                HoraSalida = r.HoraSalida,
                HorasLaboradas = r.HorasLaboradas,
                HorasPermiso = r.HorasPermiso,
                r.CreatedAt
            })
            .ToListAsync();

        return Ok(resumen);
    }


    // Obtener un resumen por Id
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var resumen = await _context.AsistenciaResumen.FindAsync(id);
        if (resumen == null) return NotFound();
        return Ok(resumen);
    }

    // Insertar un resumen de asistencia manual
    [HttpPost]
    public async Task<IActionResult> Insert([FromBody] AsistenciaResumen model)
    {
        // 🔹 Obtener info del empleado
        var empleado = await _context.Empleados.FindAsync(model.IdEmpleado);
        if (empleado == null)
            return BadRequest("Empleado no encontrado");

        model.NombreEmpleado = $"{empleado.Nombre} {empleado.ApellidoPaterno} {empleado.ApellidoMaterno}".Trim();

        _context.AsistenciaResumen.Add(model);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = model.IdResumen }, model);
    }


    // Actualizar un resumen existente
    [HttpPut]
    public async Task<IActionResult> Update([FromBody] AsistenciaResumen model)
    {
        var registro = await _context.AsistenciaResumen.FindAsync(model.IdResumen);
        if (registro == null) return NotFound();

        // Actualizar todos los campos
        registro.IdEmpleado = model.IdEmpleado;
        registro.Fecha = model.Fecha;
        registro.HoraEntrada = model.HoraEntrada;
        registro.HoraSalida = model.HoraSalida;
        registro.HorasLaboradas = model.HorasLaboradas;
        registro.HorasPermiso = model.HorasPermiso;

        // 🔹 Actualizar nombre completo
        var empleado = await _context.Empleados.FindAsync(model.IdEmpleado);
        if (empleado != null)
            registro.NombreEmpleado = $"{empleado.Nombre} {empleado.ApellidoPaterno} {empleado.ApellidoMaterno}".Trim();

        await _context.SaveChangesAsync();
        return Ok(registro);
    }


    // Eliminar un resumen
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var resumen = await _context.AsistenciaResumen.FindAsync(id);
        if (resumen == null) return NotFound();

        _context.AsistenciaResumen.Remove(resumen);
        await _context.SaveChangesAsync();

        // Recalcular prenomina y resumen general
        await _context.Database.ExecuteSqlRawAsync(
            "CALL sp_generar_resumen_asistencia({0})", resumen.Fecha
        );
        await _context.Database.ExecuteSqlRawAsync(
            "CALL sp_actualizar_prenomina_semana_extra({0})", resumen.Fecha
        );

        return NoContent();
    }
}
