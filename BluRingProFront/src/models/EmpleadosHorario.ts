export interface EmpleadosHorario {
  IdHorario: number;
  NombreEmpleado?: string;
  HoraEntradaHorario: string; // TimeOnly -> string
  HoraSalidaHorario: string;
  Jornada: number;
  IdEmpleado: number;
}
