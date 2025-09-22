using System;
using System.ComponentModel.DataAnnotations.Schema;
using Blu_Ring_Pro.Models;

namespace Blu_Ring_Pro.Models
{
    [Table("registro_asistencia")]
    public partial class RegistroAsistencia
    {
        [Column("Id_Registro")]
        public int IdRegistro { get; set; }

        [Column("Id_Empleado")]
        public int IdEmpleado { get; set; }

        [Column("Numero_Dispositivo")]
        public int? NumeroDispositivo { get; set; }

        [Column("Fecha")]
        public DateOnly Fecha { get; set; }

        [Column("Hora")]
        public TimeOnly Hora { get; set; }

        [Column("Dispositivo")]
        public string Dispositivo { get; set; } = null!;

        [Column("Tipo_Registro")]
        public int? TipoRegistro { get; set; }

        [Column("Created_At")]
        public DateTime CreatedAt { get; set; }

        [Column("Nombre_Empleado")]
        public string NombreEmpleado { get; set; } = null!; // ✅ nuevo campo

        // Relación con Empleados
        [ForeignKey("IdEmpleado")]
        [InverseProperty("RegistroAsistencia")]
        public virtual Empleados IdEmpleadoNavigation { get; set; } = null!;
    }
}
