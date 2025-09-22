export interface RegistroAsistencia {
  IdRegistro?: number; // opcional porque puede no existir antes de crear
  IdEmpleado: number;
  CodigoEmpleado?: string;
  Nombre?: string;
  ApellidoPaterno?: string;
  ApellidoMaterno?: string;
  Fecha: string; // YYYY-MM-DD
  Hora: string;  // HH:mm:ss
  TipoRegistro: string;
  Dispositivo: string;
}
