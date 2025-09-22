using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Blu_Ring_Pro.Models;
using Microsoft.EntityFrameworkCore;

namespace Blu_Ring_Pro.Models
{
    [Table("empleados_horario")]
    public partial class EmpleadosHorario
    {
        [Key]
        [Column("Id_Horario")]
        public int IdHorario { get; set; }

        [StringLength(50)]
        public string? NombreEmpleado { get; set; } // ✅ ahora es opcional

        [Column("Hora_Entrada_Horario")]
        public TimeOnly HoraEntradaHorario { get; set; }

        [Column("Hora_Salida_Horario")]
        public TimeOnly HoraSalidaHorario { get; set; }

        [Precision(4, 2)]
        public decimal Jornada { get; set; }

        [Column("Id_Empleado")]
        public int IdEmpleado { get; set; }

        [ForeignKey("IdEmpleado")]
        [InverseProperty("EmpleadosHorarios")]
        public virtual Empleados? IdEmpleadoNavigation { get; set; } // ✅ opcional
    }

}
