using System;
using System.Collections.Generic;

namespace Blu_Ring_Pro.Models;

public partial class Empleados
{
    public int IdEmpleado { get; set; }

    public string? CodigoEmpleado { get; set; }

    public string Departamento { get; set; } = null!;

    public string Nombre { get; set; } = null!;

    public string ApellidoPaterno { get; set; } = null!;

    public string ApellidoMaterno { get; set; } = null!;

    public virtual ICollection<AsistenciaFalta> AsistenciaFalta { get; set; } = new List<AsistenciaFalta>();

    public virtual ICollection<AsistenciaResumen> AsistenciaResumen { get; set; } = new List<AsistenciaResumen>();

    public virtual ICollection<EmpleadosHorario> EmpleadosHorarios { get; set; } = new List<EmpleadosHorario>();

    public virtual ICollection<Prenomina> Prenominas { get; set; } = new List<Prenomina>();

    public virtual ICollection<RegistroAsistencia> RegistroAsistencia { get; set; } = new List<RegistroAsistencia>();
}
