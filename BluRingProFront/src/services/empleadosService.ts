// src/services/empleadosService.ts
import api from "./api";

export interface Empleado {
  idEmpleado: number;
  codigoEmpleado?: string;
  departamento: string;
  nombre: string;
  apellidoPaterno: string;
  apellidoMaterno: string;
}

// 🔹 Obtener empleados
export const getEmpleados = async (): Promise<Empleado[]> => {
  const { data } = await api.get<Empleado[]>("/Empleados");
  return data; // Ya está en camelCase
};

// 🔹 Obtener un empleado por ID
export const getEmpleadoById = async (id: number): Promise<Empleado> => {
  const { data } = await api.get<Empleado>(`/Empleados/${id}`);
  return data;
};

// 🔹 Crear empleado
export const createEmpleado = async (empleado: Omit<Empleado, "idEmpleado">) => {
  const { data } = await api.post("/Empleados", empleado);
  return data;
};

// 🔹 Actualizar empleado
export const updateEmpleado = async (id: number, empleado: Partial<Empleado>) => {
  const { data } = await api.put(`/Empleados/${id}`, empleado);
  return data;
};

// 🔹 Eliminar empleado
export const deleteEmpleado = async (id: number) => {
  await api.delete(`/Empleados/${id}`);
};
