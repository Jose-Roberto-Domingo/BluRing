using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Scaffolding.Internal;

namespace Blu_Ring_Pro.Models;

public partial class BluRingProContext : DbContext
{
    public BluRingProContext()
    {
    }

    public BluRingProContext(DbContextOptions<BluRingProContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AsistenciaFalta> AsistenciaFaltas { get; set; }

    public virtual DbSet<AsistenciaResumen> AsistenciaResumen { get; set; }

    public virtual DbSet<Empleados> Empleados { get; set; }

    public virtual DbSet<EmpleadosHorario> EmpleadosHorarios { get; set; }

    public virtual DbSet<Prenomina> Prenominas { get; set; }

    public virtual DbSet<RegistroAsistencia> RegistroAsistencia { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseMySql("server=localhost;port=3306;database=BluRingPro;user=root;charset=utf8mb4;sslmode=None", Microsoft.EntityFrameworkCore.ServerVersion.Parse("10.4.32-mariadb"));

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_general_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<AsistenciaFalta>(entity =>
        {
            entity.HasKey(e => e.IdFalta).HasName("PRIMARY");

            entity.ToTable("asistencia_faltas");

            entity.HasIndex(e => new { e.IdEmpleado, e.Semana }, "unique_falta").IsUnique();

            entity.Property(e => e.IdFalta)
                .HasColumnType("int(11)")
                .HasColumnName("Id_Falta");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("current_timestamp()")
                .HasColumnType("timestamp")
                .HasColumnName("Created_At");
            entity.Property(e => e.Domingo)
                .HasMaxLength(1)
                .IsFixedLength();
            entity.Property(e => e.IdEmpleado)
                .HasColumnType("int(11)")
                .HasColumnName("Id_Empleado");
            entity.Property(e => e.Jueves)
                .HasMaxLength(1)
                .IsFixedLength();
            entity.Property(e => e.Lunes)
                .HasMaxLength(1)
                .IsFixedLength();
            entity.Property(e => e.Martes)
                .HasMaxLength(1)
                .IsFixedLength();
            entity.Property(e => e.Miercoles)
                .HasMaxLength(1)
                .IsFixedLength();
            entity.Property(e => e.Sabado)
                .HasMaxLength(1)
                .IsFixedLength();
            entity.Property(e => e.TotalFaltas)
                .HasDefaultValueSql("'0'")
                .HasColumnType("int(11)")
                .HasColumnName("Total_Faltas");
            entity.Property(e => e.TotalPermisos)
                .HasDefaultValueSql("'0'")
                .HasColumnType("int(11)")
                .HasColumnName("Total_Permisos");
            entity.Property(e => e.Viernes)
                .HasMaxLength(1)
                .IsFixedLength();

            entity.HasOne(d => d.IdEmpleadoNavigation).WithMany(p => p.AsistenciaFalta)
                .HasForeignKey(d => d.IdEmpleado)
                .HasConstraintName("fk_faltas_empleado");
        });

        modelBuilder.Entity<AsistenciaResumen>(entity =>
        {
            entity.HasKey(e => e.IdResumen).HasName("PRIMARY");

            entity.ToTable("asistencia_resumen");

            entity.HasIndex(e => new { e.IdEmpleado, e.Fecha }, "unique_resumen").IsUnique();

            entity.Property(e => e.IdResumen)
                .HasColumnType("int(11)")
                .HasColumnName("Id_Resumen");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("current_timestamp()")
                .HasColumnType("timestamp")
                .HasColumnName("Created_At");
            entity.Property(e => e.HoraEntrada)
                .HasColumnType("time")
                .HasColumnName("Hora_Entrada");
            entity.Property(e => e.HoraSalida)
                .HasColumnType("time")
                .HasColumnName("Hora_Salida");
            entity.Property(e => e.HorasLaboradas)
                .HasPrecision(5, 2)
                .HasDefaultValueSql("'0.00'")
                .HasColumnName("Horas_Laboradas");
            entity.Property(e => e.HorasPermiso)
                .HasPrecision(5, 2)
                .HasDefaultValueSql("'0.00'")
                .HasColumnName("Horas_Permiso");
            entity.Property(e => e.IdEmpleado)
                .HasColumnType("int(11)")
                .HasColumnName("Id_Empleado");
            entity.Property(e => e.NombreEmpleado).HasMaxLength(50);

            entity.HasOne(d => d.IdEmpleadoNavigation).WithMany(p => p.AsistenciaResumen)
                .HasForeignKey(d => d.IdEmpleado)
                .HasConstraintName("asistencia_resumen_ibfk_1");
        });

        modelBuilder.Entity<Empleados>(entity =>
        {
            entity.HasKey(e => e.IdEmpleado).HasName("PRIMARY");

            entity.ToTable("empleados");

            entity.HasIndex(e => e.CodigoEmpleado, "Codigo_Empleado").IsUnique();

            entity.Property(e => e.IdEmpleado)
                .HasColumnType("int(11)")
                .HasColumnName("Id_Empleado");
            entity.Property(e => e.ApellidoMaterno)
                .HasMaxLength(50)
                .HasColumnName("Apellido_Materno");
            entity.Property(e => e.ApellidoPaterno)
                .HasMaxLength(50)
                .HasColumnName("Apellido_Paterno");
            entity.Property(e => e.CodigoEmpleado)
                .HasMaxLength(10)
                .HasColumnName("Codigo_Empleado");
            entity.Property(e => e.Departamento).HasMaxLength(50);
            entity.Property(e => e.Nombre).HasMaxLength(50);
        });

        modelBuilder.Entity<EmpleadosHorario>(entity =>
        {
            entity.HasKey(e => e.IdHorario).HasName("PRIMARY");

            entity.ToTable("empleados_horario");

            entity.HasIndex(e => e.IdEmpleado, "fk_empleado_horario");

            entity.Property(e => e.IdHorario)
                .HasColumnType("int(11)")
                .HasColumnName("Id_Horario");
            entity.Property(e => e.HoraEntradaHorario)
                .HasColumnType("time")
                .HasColumnName("Hora_Entrada_Horario");
            entity.Property(e => e.HoraSalidaHorario)
                .HasColumnType("time")
                .HasColumnName("Hora_Salida_Horario");
            entity.Property(e => e.IdEmpleado)
                .HasColumnType("int(11)")
                .HasColumnName("Id_Empleado");
            entity.Property(e => e.Jornada)
                .HasPrecision(4, 2)
                .HasDefaultValueSql("'8.00'");
            entity.Property(e => e.NombreEmpleado).HasMaxLength(50);

            entity.HasOne(d => d.IdEmpleadoNavigation).WithMany(p => p.EmpleadosHorarios)
                .HasForeignKey(d => d.IdEmpleado)
                .HasConstraintName("fk_empleado_horario");
        });

        modelBuilder.Entity<Prenomina>(entity =>
        {
            entity.HasKey(e => e.IdPrenomina).HasName("PRIMARY");

            entity.ToTable("prenomina");

            entity.HasIndex(e => new { e.IdEmpleado, e.Semana }, "unique_prenomina").IsUnique();

            entity.Property(e => e.IdPrenomina)
                .HasColumnType("int(11)")
                .HasColumnName("Id_Prenomina");
            entity.Property(e => e.DiasTrabajados)
                .HasPrecision(4, 2)
                .HasDefaultValueSql("'0.00'")
                .HasColumnName("Dias_Trabajados");
            entity.Property(e => e.Domingo)
                .HasPrecision(4, 2)
                .HasDefaultValueSql("'0.00'");
            entity.Property(e => e.HorasExtras)
                .HasPrecision(4, 2)
                .HasDefaultValueSql("'0.00'")
                .HasColumnName("Horas_Extras");
            entity.Property(e => e.IdEmpleado)
                .HasColumnType("int(11)")
                .HasColumnName("Id_Empleado");
            entity.Property(e => e.Jueves)
                .HasPrecision(4, 2)
                .HasDefaultValueSql("'0.00'");
            entity.Property(e => e.Lunes)
                .HasPrecision(4, 2)
                .HasDefaultValueSql("'0.00'");
            entity.Property(e => e.Martes)
                .HasPrecision(4, 2)
                .HasDefaultValueSql("'0.00'");
            entity.Property(e => e.Miercoles)
                .HasPrecision(4, 2)
                .HasDefaultValueSql("'0.00'");
            entity.Property(e => e.Sabado)
                .HasPrecision(4, 2)
                .HasDefaultValueSql("'0.00'");
            entity.Property(e => e.Viernes)
                .HasPrecision(4, 2)
                .HasDefaultValueSql("'0.00'");

            entity.HasOne(d => d.IdEmpleadoNavigation).WithMany(p => p.Prenominas)
                .HasForeignKey(d => d.IdEmpleado)
                .HasConstraintName("prenomina_ibfk_1");
        });

        modelBuilder.Entity<RegistroAsistencia>(entity =>
        {
            entity.HasKey(e => e.IdRegistro).HasName("PRIMARY");

            entity.ToTable("registro_asistencia");

            entity.HasIndex(e => e.IdEmpleado, "Id_Empleado");

            entity.Property(e => e.IdRegistro)
                .HasColumnType("int(11)")
                .HasColumnName("Id_Registro");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("current_timestamp()")
                .HasColumnType("timestamp")
                .HasColumnName("Created_At");
            entity.Property(e => e.Dispositivo)
                .HasMaxLength(50)
                .HasDefaultValueSql("'Desconocido'");
            entity.Property(e => e.Hora).HasColumnType("time");
            entity.Property(e => e.IdEmpleado)
                .HasColumnType("int(11)")
                .HasColumnName("Id_Empleado");
            entity.Property(e => e.NumeroDispositivo)
                .HasColumnType("int(11)")
                .HasColumnName("Numero_Dispositivo");
            entity.Property(e => e.TipoRegistro)
                .HasColumnType("int(11)")
                .HasColumnName("Tipo_Registro");

            entity.HasOne(d => d.IdEmpleadoNavigation).WithMany(p => p.RegistroAsistencia)
                .HasForeignKey(d => d.IdEmpleado)
                .HasConstraintName("registro_asistencia_ibfk_1");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
