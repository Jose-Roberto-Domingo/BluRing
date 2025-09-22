import api from "./api";
import type { AsistenciaFalta } from "../models/AsistenciaFalta";

export const getAsistenciaFaltas = async (): Promise<AsistenciaFalta[]> => {
  try {
    const { data } = await api.get<AsistenciaFalta[]>("/AsistenciaFaltas");
    return data;
  } catch (error) {
    console.error('Error fetching faltas:', error);
    throw error;
  }
};

export const getAsistenciaFaltaById = async (id: number): Promise<AsistenciaFalta> => {
  try {
    const { data } = await api.get<AsistenciaFalta>(`/AsistenciaFaltas/${id}`);
    return data;
  } catch (error) {
    console.error('Error fetching falta:', error);
    throw error;
  }
};

export const updateAsistenciaFalta = async (id: number, falta: any): Promise<AsistenciaFalta> => {
  try {
    // Enviar solo los campos editables
    const payload = {
      lunes: falta.lunes,
      martes: falta.martes,
      miercoles: falta.miercoles,
      jueves: falta.jueves,
      viernes: falta.viernes,
      sabado: falta.sabado,
      domingo: falta.domingo
    };

    const { data } = await api.put(`/AsistenciaFaltas/${id}`, payload);
    return data;
  } catch (error) {
    console.error('Error updating falta:', error);
    throw error;
  }
};

export const deleteAsistenciaFalta = async (id: number): Promise<void> => {
  try {
    await api.delete(`/AsistenciaFaltas/${id}`);
  } catch (error) {
    console.error('Error deleting falta:', error);
    throw error;
  }
};

export const exportFaltasExcel = async (): Promise<void> => {
  try {
    const { data } = await api.get("/AsistenciaFaltas/export_excel", {
      responseType: "blob"
    });
    const url = window.URL.createObjectURL(new Blob([data]));
    const link = document.createElement("a");
    link.href = url;
    link.setAttribute("download", "faltas.xlsx");
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
    window.URL.revokeObjectURL(url);
  } catch (error) {
    console.error('Error exporting Excel:', error);
    throw error;
  }
};

export const exportFaltasPDF = async (): Promise<void> => {
  try {
    const { data } = await api.get("/AsistenciaFaltas/export_pdf", {
      responseType: "blob"
    });
    const url = window.URL.createObjectURL(new Blob([data]));
    const link = document.createElement("a");
    link.href = url;
    link.setAttribute("download", "faltas.pdf");
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
    window.URL.revokeObjectURL(url);
  } catch (error) {
    console.error('Error exporting PDF:', error);
    throw error;
  }
};
