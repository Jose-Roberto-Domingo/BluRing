using System.IO;
using Blu_Ring_Pro.Models;
using ClosedXML.Excel;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// Alias para evitar ambigüedad con Document
using iTextDocument = iTextSharp.text.Document;

[Route("api/[controller]")]
[ApiController]
public class AsistenciaFaltasController : ControllerBase
{
    private readonly BluRingProContext _context;

    public AsistenciaFaltasController(BluRingProContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<object>>> GetAll()
    {
        var datos = await _context.AsistenciaFaltas
            .Include(f => f.IdEmpleadoNavigation)
            .Select(f => new {
                f.IdFalta,
                f.IdEmpleado,
                CodigoEmpleado = f.IdEmpleadoNavigation.CodigoEmpleado,
                NombreEmpleado = f.IdEmpleadoNavigation.Nombre + " " +
                                f.IdEmpleadoNavigation.ApellidoPaterno + " " +
                                f.IdEmpleadoNavigation.ApellidoMaterno,
                Departamento = f.IdEmpleadoNavigation.Departamento,
                Semana = f.Semana.ToString("yyyy-MM-dd"),
                f.Lunes,
                f.Martes,
                f.Miercoles,
                f.Jueves,
                f.Viernes,
                f.Sabado,
                f.Domingo,
                f.TotalFaltas,
                f.TotalPermisos,
                CreatedAt = f.CreatedAt
            })
            .OrderByDescending(f => f.CreatedAt)
            .ToListAsync();

        // Convertir fechas a string después de la consulta
        var resultado = datos.Select(f => new {
            f.IdFalta,
            f.IdEmpleado,
            f.CodigoEmpleado,
            f.NombreEmpleado,
            f.Departamento,
            f.Semana,
            f.Lunes,
            f.Martes,
            f.Miercoles,
            f.Jueves,
            f.Viernes,
            f.Sabado,
            f.Domingo,
            f.TotalFaltas,
            f.TotalPermisos,
            CreatedAt = f.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss")
        }).ToList();

        return Ok(resultado);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<object>> GetById(int id)
    {
        var falta = await _context.AsistenciaFaltas
            .Include(f => f.IdEmpleadoNavigation)
            .Where(f => f.IdFalta == id)
            .Select(f => new {
                f.IdFalta,
                f.IdEmpleado,
                CodigoEmpleado = f.IdEmpleadoNavigation.CodigoEmpleado,
                NombreEmpleado = f.IdEmpleadoNavigation.Nombre + " " +
                                f.IdEmpleadoNavigation.ApellidoPaterno + " " +
                                f.IdEmpleadoNavigation.ApellidoMaterno,
                Departamento = f.IdEmpleadoNavigation.Departamento,
                Semana = f.Semana.ToString("yyyy-MM-dd"),
                f.Lunes,
                f.Martes,
                f.Miercoles,
                f.Jueves,
                f.Viernes,
                f.Sabado,
                f.Domingo,
                f.TotalFaltas,
                f.TotalPermisos,
                CreatedAt = f.CreatedAt
            })
            .FirstOrDefaultAsync();

        if (falta == null) return NotFound();

        // Convertir fecha a string después de la consulta
        var resultado = new
        {
            falta.IdFalta,
            falta.IdEmpleado,
            falta.CodigoEmpleado,
            falta.NombreEmpleado,
            falta.Departamento,
            falta.Semana,
            falta.Lunes,
            falta.Martes,
            falta.Miercoles,
            falta.Jueves,
            falta.Viernes,
            falta.Sabado,
            falta.Domingo,
            falta.TotalFaltas,
            falta.TotalPermisos,
            CreatedAt = falta.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss")
        };

        return resultado;
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] AsistenciaFaltaUpdateRequest request)
    {
        try
        {
            var faltaExistente = await _context.AsistenciaFaltas.FindAsync(id);
            if (faltaExistente == null)
                return NotFound("Registro no encontrado");

            // Solo permitir actualizar los días de la semana
            faltaExistente.Lunes = request.Lunes;
            faltaExistente.Martes = request.Martes;
            faltaExistente.Miercoles = request.Miercoles;
            faltaExistente.Jueves = request.Jueves;
            faltaExistente.Viernes = request.Viernes;
            faltaExistente.Sabado = request.Sabado;
            faltaExistente.Domingo = request.Domingo;

            // Recalcular totales automáticamente
            faltaExistente.TotalFaltas = CalcularTotales(faltaExistente, 'F');
            faltaExistente.TotalPermisos = CalcularTotales(faltaExistente, 'P');

            await _context.SaveChangesAsync();
            return NoContent();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await AsistenciaFaltaExists(id))
                return NotFound();
            else
                throw;
        }
        catch (Exception ex)
        {
            return BadRequest($"Error: {ex.Message}");
        }
    }

    // Método para calcular totales automáticamente
    private int CalcularTotales(AsistenciaFalta falta, char tipo)
    {
        int total = 0;
        if (falta.Lunes == tipo.ToString()) total++;
        if (falta.Martes == tipo.ToString()) total++;
        if (falta.Miercoles == tipo.ToString()) total++;
        if (falta.Jueves == tipo.ToString()) total++;
        if (falta.Viernes == tipo.ToString()) total++;
        if (falta.Sabado == tipo.ToString()) total++;
        if (falta.Domingo == tipo.ToString()) total++;
        return total;
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var falta = await _context.AsistenciaFaltas.FindAsync(id);
            if (falta == null) return NotFound();

            _context.AsistenciaFaltas.Remove(falta);
            await _context.SaveChangesAsync();

            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest($"Error: {ex.Message}");
        }
    }

    private async Task<bool> AsistenciaFaltaExists(int id)
    {
        return await _context.AsistenciaFaltas.AnyAsync(e => e.IdFalta == id);
    }

    [HttpGet("export_excel")]
    public async Task<IActionResult> ExportExcel()
    {
        try
        {
            var datos = await _context.AsistenciaFaltas
                .Include(f => f.IdEmpleadoNavigation)
                .OrderByDescending(f => f.CreatedAt)
                .ToListAsync();

            using var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add("Faltas");

            // Encabezados
            ws.Cell(1, 1).Value = "Código";
            ws.Cell(1, 2).Value = "Empleado";
            ws.Cell(1, 3).Value = "Departamento";
            ws.Cell(1, 4).Value = "Semana";
            ws.Cell(1, 5).Value = "Lunes";
            ws.Cell(1, 6).Value = "Martes";
            ws.Cell(1, 7).Value = "Miércoles";
            ws.Cell(1, 8).Value = "Jueves";
            ws.Cell(1, 9).Value = "Viernes";
            ws.Cell(1, 10).Value = "Sábado";
            ws.Cell(1, 11).Value = "Domingo";
            ws.Cell(1, 12).Value = "Total Faltas";
            ws.Cell(1, 13).Value = "Total Permisos";
            ws.Cell(1, 14).Value = "Fecha Registro";

            // Formato de encabezados
            var headerRange = ws.Range(1, 1, 1, 14);
            headerRange.Style.Fill.BackgroundColor = XLColor.LightBlue;
            headerRange.Style.Font.Bold = true;
            headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            int fila = 2;
            foreach (var f in datos)
            {
                ws.Cell(fila, 1).Value = f.IdEmpleadoNavigation?.CodigoEmpleado ?? "";
                ws.Cell(fila, 2).Value = $"{f.IdEmpleadoNavigation?.Nombre} {f.IdEmpleadoNavigation?.ApellidoPaterno} {f.IdEmpleadoNavigation?.ApellidoMaterno}";
                ws.Cell(fila, 3).Value = f.IdEmpleadoNavigation?.Departamento ?? "";
                ws.Cell(fila, 4).Value = f.Semana.ToString("yyyy-MM-dd");
                ws.Cell(fila, 5).Value = f.Lunes ?? "-";
                ws.Cell(fila, 6).Value = f.Martes ?? "-";
                ws.Cell(fila, 7).Value = f.Miercoles ?? "-";
                ws.Cell(fila, 8).Value = f.Jueves ?? "-";
                ws.Cell(fila, 9).Value = f.Viernes ?? "-";
                ws.Cell(fila, 10).Value = f.Sabado ?? "-";
                ws.Cell(fila, 11).Value = f.Domingo ?? "-";
                ws.Cell(fila, 12).Value = f.TotalFaltas ?? 0;
                ws.Cell(fila, 13).Value = f.TotalPermisos ?? 0;
                ws.Cell(fila, 14).Value = f.CreatedAt.ToString("yyyy-MM-dd HH:mm");
                fila++;
            }

            // Autoajustar columnas
            ws.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            wb.SaveAs(stream);
            return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "faltas.xlsx");
        }
        catch (Exception ex)
        {
            return BadRequest($"Error al exportar Excel: {ex.Message}");
        }
    }

    [HttpGet("export_pdf")]
    public async Task<IActionResult> ExportPDF()
    {
        try
        {
            var datos = await _context.AsistenciaFaltas
                .Include(f => f.IdEmpleadoNavigation)
                .OrderByDescending(f => f.CreatedAt)
                .ToListAsync();

            using var stream = new MemoryStream();

            iTextDocument doc = new iTextDocument(PageSize.A4.Rotate());
            PdfWriter.GetInstance(doc, stream);
            doc.Open();

            // Título
            var title = new Paragraph("Reporte de Faltas",
                FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 16));
            title.Alignment = Element.ALIGN_CENTER;
            doc.Add(title);
            doc.Add(new Paragraph(" "));

            var table = new PdfPTable(14);
            table.WidthPercentage = 100;
            table.SetWidths(new float[] { 1, 2, 1.5f, 1, 0.5f, 0.5f, 0.5f, 0.5f, 0.5f, 0.5f, 0.5f, 0.5f, 0.5f, 1.5f });

            // Encabezados
            string[] headers = {
                "Código", "Empleado", "Departamento", "Semana",
                "Lun", "Mar", "Mié", "Jue", "Vie", "Sáb", "Dom",
                "Faltas", "Permisos", "Fecha Registro"
            };

            foreach (var h in headers)
            {
                var cell = new PdfPCell(new Phrase(h))
                {
                    BackgroundColor = BaseColor.LIGHT_GRAY,
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    Padding = 5
                };
                table.AddCell(cell);
            }

            // Datos
            foreach (var f in datos)
            {
                table.AddCell(f.IdEmpleadoNavigation?.CodigoEmpleado ?? "-");
                table.AddCell($"{f.IdEmpleadoNavigation?.Nombre} {f.IdEmpleadoNavigation?.ApellidoPaterno} {f.IdEmpleadoNavigation?.ApellidoMaterno}");
                table.AddCell(f.IdEmpleadoNavigation?.Departamento ?? "-");
                table.AddCell(f.Semana.ToString("yyyy-MM-dd"));
                table.AddCell(f.Lunes ?? "-");
                table.AddCell(f.Martes ?? "-");
                table.AddCell(f.Miercoles ?? "-");
                table.AddCell(f.Jueves ?? "-");
                table.AddCell(f.Viernes ?? "-");
                table.AddCell(f.Sabado ?? "-");
                table.AddCell(f.Domingo ?? "-");
                table.AddCell((f.TotalFaltas ?? 0).ToString());
                table.AddCell((f.TotalPermisos ?? 0).ToString());
                table.AddCell(f.CreatedAt.ToString("yyyy-MM-dd HH:mm"));
            }

            doc.Add(table);
            doc.Close();

            return File(stream.ToArray(), "application/pdf", "faltas.pdf");
        }
        catch (Exception ex)
        {
            return BadRequest($"Error al exportar PDF: {ex.Message}");
        }
    }
}

// DTO para actualización (solo campos editables)
public class AsistenciaFaltaUpdateRequest
{
    public string? Lunes { get; set; }
    public string? Martes { get; set; }
    public string? Miercoles { get; set; }
    public string? Jueves { get; set; }
    public string? Viernes { get; set; }
    public string? Sabado { get; set; }
    public string? Domingo { get; set; }
}