import api from "./api";
import type { AsistenciaResumen } from "../models/AsistenciaResumen";

const API_BASE = "/AsistenciaResumen";

export async function getAsistenciasResumen(): Promise<AsistenciaResumen[]> {
  const { data } = await api.get(API_BASE);
  return data;
}

export async function exportResumenExcel(busqueda: string) {
  const { data } = await api.get(`${API_BASE}/export_excel`, {
    params: { busqueda },
    responseType: "blob",
  });
  const url = window.URL.createObjectURL(new Blob([data]));
  const link = document.createElement("a");
  link.href = url;
  link.setAttribute("download", "Resumen_Asistencias.xlsx");
  document.body.appendChild(link);
  link.click();
}

export async function exportResumenPDF(busqueda: string) {
  const { data } = await api.get(`${API_BASE}/export_pdf`, {
    params: { busqueda },
    responseType: "blob",
  });
  const url = window.URL.createObjectURL(new Blob([data]));
  const link = document.createElement("a");
  link.href = url;
  link.setAttribute("download", "Resumen_Asistencias.pdf");
  document.body.appendChild(link);
  link.click();
}
