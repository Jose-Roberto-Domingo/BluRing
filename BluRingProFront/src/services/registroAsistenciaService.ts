import api from "./api";
import type { RegistroAsistencia } from "../models/RegistroAsistencia";

const API_BASE = "/asistencia";

export interface FiltroAsistencia {
  busqueda?: string;
  fecha?: string;
}

// 🔹 Obtener registros (con búsqueda y filtro por fecha/semana)
export const getAsistencias = async (
  filtro: FiltroAsistencia = {}
): Promise<RegistroAsistencia[]> => {
  const { data } = await api.get<RegistroAsistencia[]>(API_BASE, {
    params: filtro,
  });
  return data;
};

// 🔹 Obtener un registro por ID
export const getAsistenciaById = async (id: number): Promise<RegistroAsistencia> => {
  const { data } = await api.get<RegistroAsistencia>(`${API_BASE}/${id}`);
  return data;
};

// 🔹 Crear registro
export const createAsistencia = async (a: RegistroAsistencia) => {
  const { data } = await api.post(API_BASE, a);
  return data;
};

// 🔹 Actualizar registro
export const updateAsistencia = async (id: number, a: Partial<RegistroAsistencia>) => {
  const { data } = await api.put(`${API_BASE}/${id}`, a);
  return data;
};

// 🔹 Eliminar registro
export const deleteAsistencia = async (id: number) => {
  await api.delete(`${API_BASE}/${id}`);
};

// 🔹 Exportar registros a Excel
export const exportAsistenciasExcel = async (filtro: FiltroAsistencia = {}) => {
  const { data } = await api.get(`${API_BASE}/export_excel`, {
    params: filtro,
    responseType: "blob",
  });
  const url = window.URL.createObjectURL(new Blob([data]));
  const link = document.createElement("a");
  link.href = url;
  link.setAttribute("download", "asistencias.xlsx");
  document.body.appendChild(link);
  link.click();
  document.body.removeChild(link);
};

// 🔹 Exportar registros a PDF
export const exportAsistenciasPDF = async (filtro: FiltroAsistencia = {}) => {
  const { data } = await api.get(`${API_BASE}/export_pdf`, {
    params: filtro,
    responseType: "blob",
  });
  const url = window.URL.createObjectURL(new Blob([data]));
  const link = document.createElement("a");
  link.href = url;
  link.setAttribute("download", "asistencias.pdf");
  document.body.appendChild(link);
  link.click();
  document.body.removeChild(link);
};
