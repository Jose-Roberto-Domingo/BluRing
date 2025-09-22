import api from "./api";
import type { EmpleadosHorario } from "../models/EmpleadosHorario";

const API_BASE = "/EmpleadosHorario";

export async function getHorarios(): Promise<EmpleadosHorario[]> {
  const { data } = await api.get(API_BASE);
  return data;
}

// En POST usamos PascalCase como lo espera el backend
export async function createHorario(horario: {
  IdEmpleado: number;
  HoraEntradaHorario: string;
  HoraSalidaHorario: string;
  Jornada: number;
}) {
  const { data } = await api.post(API_BASE, horario);
  return data;
}

export async function updateHorario(
  id: number,
  horario: {
    IdEmpleado: number;
    HoraEntradaHorario: string;
    HoraSalidaHorario: string;
    Jornada: number;
  }
) {
  const { data } = await api.put(`${API_BASE}/${id}`, horario);
  return data;
}

export async function deleteHorario(id: number) {
  const { data } = await api.delete(`${API_BASE}/${id}`);
  return data;
}
