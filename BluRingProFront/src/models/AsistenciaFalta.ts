export interface AsistenciaFalta {
  idFalta: number;
  idEmpleado: number;
  nombreEmpleado?: string;
  codigoEmpleado?: string;
  departamento?: string;
  semana: string;
  lunes?: string;
  martes?: string;
  miercoles?: string;
  jueves?: string;
  viernes?: string;
  sabado?: string;
  domingo?: string;
  totalFaltas?: number;
  totalPermisos?: number;
  createdAt: string;
}
