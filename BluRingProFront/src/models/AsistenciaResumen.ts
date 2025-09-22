export interface AsistenciaResumen {
  idResumen: number;
  idEmpleado: number;
  nombreEmpleado: string;
  departamento: string; // ✅ agregado
  fecha: string;        // DateOnly -> string
  horaEntrada?: string; // TimeOnly -> string "HH:mm:ss"
  horaSalida?: string;
  horasLaboradas?: number;
  horasPermiso?: number;
  createdAt: string;
}
