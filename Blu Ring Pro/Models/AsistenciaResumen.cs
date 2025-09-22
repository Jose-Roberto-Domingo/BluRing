using System;
using System.Collections.Generic;

namespace Blu_Ring_Pro.Models;

public partial class AsistenciaResumen
{
    public int IdResumen { get; set; }

    public int IdEmpleado { get; set; }

    public string NombreEmpleado { get; set; } = null!;

    public DateOnly Fecha { get; set; }

    public TimeOnly? HoraEntrada { get; set; }

    public TimeOnly? HoraSalida { get; set; }

    public decimal? HorasLaboradas { get; set; }

    public decimal? HorasPermiso { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual Empleados IdEmpleadoNavigation { get; set; } = null!;
}
