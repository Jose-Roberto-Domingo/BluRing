using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Blu_Ring_Pro.Models;

public partial class AsistenciaFalta
{
    public int IdFalta { get; set; }

    [Required]
    public int IdEmpleado { get; set; }

    [Required]
    public DateOnly Semana { get; set; }

    [StringLength(1)]
    public string? Lunes { get; set; }

    [StringLength(1)]
    public string? Martes { get; set; }

    [StringLength(1)]
    public string? Miercoles { get; set; }

    [StringLength(1)]
    public string? Jueves { get; set; }

    [StringLength(1)]
    public string? Viernes { get; set; }

    [StringLength(1)]
    public string? Sabado { get; set; }

    [StringLength(1)]
    public string? Domingo { get; set; }

    public int? TotalFaltas { get; set; }

    public int? TotalPermisos { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual Empleados IdEmpleadoNavigation { get; set; } = null!;
}