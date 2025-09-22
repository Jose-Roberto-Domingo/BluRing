using System;
using System.Collections.Generic;

namespace Blu_Ring_Pro.Models;

public partial class Prenomina
{
    public int IdPrenomina { get; set; }

    public int IdEmpleado { get; set; }

    public DateOnly Semana { get; set; }

    public decimal? Lunes { get; set; }

    public decimal? Martes { get; set; }

    public decimal? Miercoles { get; set; }

    public decimal? Jueves { get; set; }

    public decimal? Viernes { get; set; }

    public decimal? Sabado { get; set; }

    public decimal? Domingo { get; set; }

    public decimal? DiasTrabajados { get; set; }

    public decimal? HorasExtras { get; set; }

    public virtual Empleados IdEmpleadoNavigation { get; set; } = null!;
}
