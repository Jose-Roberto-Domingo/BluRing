<template>
  <div class="asistencia-container">
    <h2>Registro de Asistencias</h2>

    <!-- 🔍 Barra de búsqueda -->
    <input v-model="busqueda"
           type="text"
           placeholder="Buscar asistencia..."
           class="busqueda" />

    <!-- 🔘 Botones de acción -->
    <div class="acciones-top">
      <button class="btn btn-azul" @click="abrirFormulario"><i class="bi bi-plus-circle"></i> Agregar</button>
      <router-link to="/asistencia/resumen" class="btn btn-verde"><i class="bi bi-journal-text"></i> Resumen</router-link>
      <router-link to="/asistencia/faltas" class="btn btn-naranja"><i class="bi bi-person-x-fill"></i> Faltas</router-link>
      <button class="btn btn-verde-oscuro" @click="exportExcel"><i class="bi bi-download"></i> Exportar a Excel</button>
      <button class="btn btn-rojo-oscuro" @click="exportPDF"><i class="bi bi-download"></i> Exportar a PDF</button>
    </div>

    <!-- 📋 Tabla -->
    <div class="tabla-responsive">
      <table class="tabla">
        <thead>
          <tr>
            <th>ID</th>
            <th>Código</th>
            <th>Nombre</th>
            <th>Fecha</th>
            <th>Hora</th>
            <th>Tipo</th>
            <th>Dispositivo</th>
            <th>Acciones</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="asis in asistenciasFiltradas" :key="asis.idRegistro">
            <td>{{ asis.idRegistro }}</td>
            <td>{{ asis.codigoEmpleado }}</td>
            <td>{{ asis.nombreCompleto }}</td>
            <td>{{ asis.fecha }}</td>
            <td>{{ asis.hora }}</td>
            <td>{{ asis.tipoRegistro === 0 ? "Entrada" : "Salida" }}</td>
            <td>{{ asis.dispositivo }}</td>
            <td>
              <button class="btn btn-amarillo" @click="editarAsistencia(asis)"><i class="bi bi-pencil-square"></i> Editar</button>
              <button class="btn btn-rojo" @click="eliminarAsistencia(asis.idRegistro)"><i class="bi bi-trash"></i> Eliminar</button>
            </td>
          </tr>
          <tr v-if="asistenciasFiltradas.length === 0">
            <td colspan="8" style="text-align:center;">Sin registros</td>
          </tr>
        </tbody>
      </table>
    </div>

    <!-- 📝 Modal formulario -->
    <transition name="modal-fade">
      <div v-if="mostrarFormulario" class="modal-overlay" @click.self="cerrarFormulario">
        <div class="modal-content">
          <h3>{{ asistenciaActual.idRegistro ? "Editar Asistencia" : "Agregar Asistencia" }}</h3>
          <form @submit.prevent="guardarAsistencia">
            <label>Empleado:</label>
            <select v-model="asistenciaActual.idEmpleado" required>
              <option value="">-- Seleccione empleado --</option>
              <option v-for="emp in empleados" :key="emp.idEmpleado" :value="emp.idEmpleado">
                {{ emp.codigoEmpleado }} - {{ emp.nombre }} {{ emp.apellidoPaterno }} {{ emp.apellidoMaterno }}
              </option>
            </select>

            <label>Fecha:</label>
            <input type="date" v-model="asistenciaActual.fecha" required />

            <label>Hora:</label>
            <input type="time" v-model="asistenciaActual.hora" required />

            <label>Tipo Registro:</label>
            <select v-model="asistenciaActual.tipoRegistro" required>
              <option :value="0">Entrada</option>
              <option :value="1">Salida</option>
            </select>

            <label>Dispositivo:</label>
            <input v-model="asistenciaActual.dispositivo" placeholder="Dispositivo" />

            <div class="acciones-modal">
              <button type="submit" class="btn btn-azul">
                {{ asistenciaActual.idRegistro ? "Actualizar" : "Guardar" }}
              </button>
              <button type="button" class="btn btn-rojo" @click="cerrarFormulario">Cancelar</button>
            </div>
          </form>
        </div>
      </div>
    </transition>
  </div>
</template>

<script setup lang="ts">
  import { ref, computed, onMounted } from "vue";
  import type { RegistroAsistencia } from "../models/RegistroAsistencia";
  import {
    getAsistencias,
    createAsistencia,
    updateAsistencia,
    deleteAsistencia,
    exportAsistenciasExcel,
    exportAsistenciasPDF
  } from "../services/registroAsistenciaService";
  import { getEmpleados } from "../services/empleadosService";

  const asistencias = ref<any[]>([]);
  const empleados = ref<any[]>([]);
  const busqueda = ref("");
  const mostrarFormulario = ref(false);

  const asistenciaActual = ref<Partial<RegistroAsistencia>>({
    idEmpleado: 0,
    fecha: new Date().toISOString().substr(0, 10),
    hora: new Date().toTimeString().substr(0, 5),
    tipoRegistro: 0,
    dispositivo: ""
  });

  const asistenciasFiltradas = computed(() =>
    asistencias.value.filter(a =>
      `${a.idRegistro} ${a.codigoEmpleado} ${a.nombreCompleto} ${a.fecha} ${a.hora}`
        .toLowerCase()
        .includes(busqueda.value.toLowerCase())
    )
  );

  onMounted(async () => {
    await cargarDatos();
  });

  async function cargarDatos() {
    asistencias.value = await getAsistencias();
    empleados.value = await getEmpleados();

    asistencias.value = asistencias.value.map(a => ({
      ...a,
      nombreCompleto: `${a.nombre} ${a.apellidoPaterno} ${a.apellidoMaterno}`
    }));
  }

  function abrirFormulario() {
    asistenciaActual.value = {
      idEmpleado: 0,
      fecha: new Date().toISOString().substr(0, 10),
      hora: new Date().toTimeString().substr(0, 5),
      tipoRegistro: 0,
      dispositivo: ""
    };
    mostrarFormulario.value = true;
  }

  function editarAsistencia(a: RegistroAsistencia) {
    const empleadoSeleccionado = empleados.value.find((e: any) => e.idEmpleado === a.idEmpleado);
    if (!empleadoSeleccionado) {
      alert("Empleado no encontrado.");
      return;
    }

    asistenciaActual.value = {
      idRegistro: a.idRegistro,
      idEmpleado: a.idEmpleado,
      fecha: a.fecha,
      hora: a.hora.length === 5 ? a.hora + ":00" : a.hora,
      tipoRegistro: a.tipoRegistro ?? 0,
      dispositivo: a.dispositivo && a.dispositivo !== "Manual" ? a.dispositivo : ""
    };

    mostrarFormulario.value = true;
  }

  async function guardarAsistencia() {
  try {
    // Construir payload limpio
    const payload = {
      idEmpleado: asistenciaActual.value.idEmpleado!,
      fecha: asistenciaActual.value.fecha!,
      hora: asistenciaActual.value.hora!,
      tipoRegistro: asistenciaActual.value.tipoRegistro ?? 0,
      dispositivo: asistenciaActual.value.dispositivo || ""
    };

    if (asistenciaActual.value.idRegistro) {
      await updateAsistencia(asistenciaActual.value.idRegistro, payload);
    } else {
      await createAsistencia(payload);
    }

    cerrarFormulario();
    await cargarDatos();
  } catch (err: any) {
    console.error("Error guardando asistencia:", err.response?.data || err.message);
    alert("Error guardando asistencia. Revisa la consola.");
  }
}


  async function eliminarAsistencia(id: number) {
    if (confirm("¿Eliminar este registro?")) {
      await deleteAsistencia(id);
      await cargarDatos();
    }
  }

  function cerrarFormulario() {
    mostrarFormulario.value = false;
  }

  async function exportExcel() {
    await exportAsistenciasExcel(busqueda.value, "");
  }

  async function exportPDF() {
    await exportAsistenciasPDF(busqueda.value, "");
  }
</script>


<style scoped>
  .asistencia-container {
    padding: 20px;
    font-family: "Segoe UI", sans-serif;
  }

  h2 {
    color: #1e3a8a;
    margin-bottom: 15px;
  }

  .busqueda {
    width: 100%;
    padding: 10px;
    border: 1px solid #cbd5e1;
    border-radius: 8px;
    margin-bottom: 15px;
  }

  .acciones-top {
    display: flex;
    flex-wrap: wrap;
    gap: 10px;
    margin-bottom: 15px;
  }

  .tabla-responsive {
    overflow-x: auto;
  }

  .tabla {
    width: 100%;
    border-collapse: collapse;
    border-radius: 10px;
    overflow: hidden;
    box-shadow: 0 2px 6px rgba(0,0,0,0.05);
  }

    .tabla thead {
      background-color: #1e3a8a;
      color: white;
    }

    .tabla th,
    .tabla td {
      padding: 12px;
      text-align: center;
      border-bottom: 1px solid #e2e8f0;
      font-size: 14px;
    }

    .tabla tbody tr:nth-child(even) {
      background-color: #f8fafc;
    }

    .tabla tbody tr:hover {
      background-color: #f1f5f9;
    }

  .btn {
    padding: 6px 12px;
    border: none;
    border-radius: 6px;
    cursor: pointer;
    font-size: 14px;
    transition: background-color 0.2s ease;
    color: white;
  }

  .btn-azul {
    background: #2563eb;
  }

  .btn-verde {
    background: #22c55e;
  }

  .btn-naranja {
    background: #f97316;
  }

  .btn-verde-oscuro {
    background: #15803d;
  }

  .btn-rojo-oscuro {
    background: #dc2626;
  }

  .btn-amarillo {
    background: #facc15;
    color: #1e293b;
  }

  .btn-rojo {
    background: #ef4444;
  }

  .modal-overlay {
    position: fixed;
    inset: 0;
    background: rgba(0, 0, 0, 0.4);
    display: flex;
    justify-content: center;
    align-items: center;
  }

  .modal-content {
    background: white;
    padding: 20px;
    border-radius: 12px;
    width: 400px;
    max-width: 90%;
  }

    .modal-content label {
      font-weight: 600;
      margin-top: 8px;
      display: block;
    }

    .modal-content input,
    .modal-content select {
      width: 100%;
      padding: 8px;
      margin-top: 4px;
      border: 1px solid #cbd5e1;
      border-radius: 6px;
    }

  .acciones-modal {
    display: flex;
    justify-content: space-between;
    margin-top: 15px;
  }

  .modal-fade-enter-active,
  .modal-fade-leave-active {
    transition: all 0.3s ease;
  }

  .modal-fade-enter-from,
  .modal-fade-leave-to {
    opacity: 0;
    transform: scale(0.9);
  }
</style>
