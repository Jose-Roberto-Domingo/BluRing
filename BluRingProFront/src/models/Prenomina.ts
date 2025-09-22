export interface Prenomina {
  idPrenomina: number;
  idEmpleado: number;
  codigoEmpleado: string;
  nombreCompleto: string;
  departamento: string;
  semana: string;
  lunes?: number;
  martes?: number;
  miercoles?: number;
  jueves?: number;
  viernes?: number;
  sabado?: number;
  domingo?: number;
  diasTrabajados?: number;
  horasExtras?: number;
}
