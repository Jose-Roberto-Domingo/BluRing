import api from "./api";
import type { Prenomina } from "../models/Prenomina";

export const getPrenominas = async (): Promise<Prenomina[]> => {
  try {
    const { data } = await api.get<Prenomina[]>("/Prenomina");
    return data;
  } catch (error) {
    console.error('Error fetching prenominas:', error);
    throw error;
  }
};

export const getPrenominaById = async (id: number): Promise<Prenomina> => {
  try {
    const { data } = await api.get<Prenomina>(`/Prenomina/${id}`);
    return data;
  } catch (error) {
    console.error('Error fetching prenomina:', error);
    throw error;
  }
};

export const updatePrenomina = async (id: number, prenomina: any): Promise<Prenomina> => {
  try {
    const { data } = await api.put(`/Prenomina/${id}`, prenomina);
    return data;
  } catch (error) {
    console.error('Error updating prenomina:', error);
    throw error;
  }
};

export const deletePrenomina = async (id: number): Promise<void> => {
  try {
    await api.delete(`/Prenomina/${id}`);
  } catch (error) {
    console.error('Error deleting prenomina:', error);
    throw error;
  }
};

export const exportPrenominasExcel = async (): Promise<void> => {
  try {
    const { data } = await api.get("/Prenomina/export_excel", {
      responseType: "blob"
    });
    const url = window.URL.createObjectURL(new Blob([data]));
    const link = document.createElement("a");
    link.href = url;
    link.setAttribute("download", "prenominas.xlsx");
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
    window.URL.revokeObjectURL(url);
  } catch (error) {
    console.error('Error exporting Excel:', error);
    throw error;
  }
};

export const exportPrenominasPDF = async (): Promise<void> => {
  try {
    const { data } = await api.get("/Prenomina/export_pdf", {
      responseType: "blob"
    });
    const url = window.URL.createObjectURL(new Blob([data]));
    const link = document.createElement("a");
    link.href = url;
    link.setAttribute("download", "prenominas.pdf");
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
    window.URL.revokeObjectURL(url);
  } catch (error) {
    console.error('Error exporting PDF:', error);
    throw error;
  }
};
