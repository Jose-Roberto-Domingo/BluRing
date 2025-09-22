<template>
  <div class="table-container">
    <h2>Resumen de Asistencia</h2>

    <div class="export-buttons">
      <button class="btn btn-success" @click="exportExcel"><i class="bi bi-download"></i> Exportar a Excel</button>
      <button class="btn btn-info" @click="exportPDF"><i class="bi bi-download"></i> Exportar a PDF</button>
      <button class="btn btn-danger" @click="regresar">Regresar</button>
    </div>

    <input v-model="busqueda"
           type="text"
           placeholder="Buscar por nombre o departamento..."
           class="busqueda" />

    <div class="table-responsive">
      <table class="tabla">
        <thead>
          <tr>
            <th>Código de Empleado</th>
            <th>Nombre</th>
            <th>Departamento</th>
            <th>Fecha</th>
            <th>Hora Entrada</th>
            <th>Hora Salida</th>
            <th>Horas Laboradas</th>
            <th>Horas Permiso</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="asis in asistenciasFiltradas" :key="asis.idResumen">
            <td>{{ asis.idEmpleado }}</td>
            <td>{{ asis.nombreEmpleado }}</td>
            <td>{{ asis.departamento }}</td>
            <td>{{ asis.fecha }}</td>
            <td>{{ asis.horaEntrada ?? "-" }}</td>
            <td>{{ asis.horaSalida ?? "-" }}</td>
            <td>{{ asis.horasLaboradas ?? 0 }}</td>
            <td>{{ asis.horasPermiso ?? 0 }}</td>
          </tr>
          <tr v-if="asistenciasFiltradas.length === 0">
            <td colspan="8" style="text-align:center;">No hay registros de asistencia disponibles</td>
          </tr>
        </tbody>
      </table>
    </div>
  </div>
</template>

<script setup lang="ts">
  import { ref, computed, onMounted } from "vue";
  import type { AsistenciaResumen } from "../models/AsistenciaResumen";
  import { getAsistenciasResumen, exportResumenExcel, exportResumenPDF } from "../services/asistenciaResumenService";

  const asistencias = ref<AsistenciaResumen[]>([]);
  const busqueda = ref("");

  const asistenciasFiltradas = computed(() =>
    asistencias.value.filter(
      (a) =>
        a.nombreEmpleado.toLowerCase().includes(busqueda.value.toLowerCase()) ||
        a.departamento.toLowerCase().includes(busqueda.value.toLowerCase())
    )
  );

  onMounted(async () => {
    asistencias.value = await getAsistenciasResumen();
  });

  async function exportExcel() {
    await exportResumenExcel(busqueda.value);
  }

  async function exportPDF() {
    await exportResumenPDF(busqueda.value);
  }

  function regresar() {
    window.location.href = "/asistencia";
  }
</script>

<style scoped>
  .table-container {
    margin: 20px auto;
    max-width: 1200px;
    font-family: "Segoe UI", sans-serif;
    color: #333;
  }

  h2 {
    text-align: center;
    margin-bottom: 15px;
    color: #1e3a8a; /* Azul principal */
  }

  .export-buttons {
    display: flex;
    justify-content: flex-end;
    gap: 10px;
    margin-bottom: 15px;
  }

  .busqueda {
    display: block;
    width: 100%;
    max-width: 300px;
    margin: 0 auto 20px auto;
    padding: 8px 12px;
    border: 1px solid #ccc;
    border-radius: 6px;
  }

  .table-responsive {
    overflow-x: auto;
  }

  .tabla {
    width: 100%;
    border-collapse: collapse;
    border-radius: 8px;
    overflow: hidden;
    box-shadow: 0 0 10px #00000015;
  }

    .tabla th,
    .tabla td {
      border: 1px solid #ddd;
      padding: 10px;
      text-align: center;
    }

    .tabla th {
      background-color: #1e3a8a; /* Azul principal */
      color: white;
      font-weight: 600;
    }

    .tabla tr:nth-child(even) {
      background-color: #f9fafb;
    }

    .tabla tr:hover {
      background-color: #e0e7ff; /* azul claro */
    }

  .btn {
    padding: 6px 12px;
    border: none;
    border-radius: 6px;
    cursor: pointer;
    color: white;
    font-size: 0.9rem;
  }

  .btn-success {
    background-color: #16a34a;
  }

  .btn-info {
    background-color: #2563eb; /* azul */
  }

  .btn-danger {
    background-color: #dc2626;
  }
</style>
