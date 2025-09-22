<template>
  <div class="faltas-container">
    <h2>Registro de Faltas</h2>

    <!-- 🔍 Barra de búsqueda -->
    <input v-model="busqueda"
           type="text"
           placeholder="Buscar por nombre, departamento o código..."
           class="busqueda" />

    <!-- 🔘 Botones de acción -->
    <div class="acciones-top">
      <button class="btn btn-verde-oscuro" @click="exportExcel">
        <i class="bi bi-download"></i> Exportar Excel
      </button>
      <button class="btn btn-rojo-oscuro" @click="exportPDF">
        <i class="bi bi-download"></i> Exportar PDF
      </button>
      <router-link to="/asistencia" class="btn btn-primary">Volver a Registros</router-link>
    </div>

    <!-- 📋 Tabla -->
    <div class="tabla-responsive">
      <table class="tabla">
        <thead>
          <tr>
            <th>Código</th>
            <th>Empleado</th>
            <th>Departamento</th>
            <th>Semana</th>
            <th>Lun</th>
            <th>Mar</th>
            <th>Mié</th>
            <th>Jue</th>
            <th>Vie</th>
            <th>Sáb</th>
            <th>Dom</th>
            <th>Total Faltas</th>
            <th>Total Permisos</th>
            <th>Acciones</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="f in faltasFiltradas" :key="f.idFalta">
            <td>{{ f.codigoEmpleado ?? '-' }}</td>
            <td>{{ f.nombreEmpleado }}</td>
            <td>{{ f.departamento }}</td>
            <td>{{ formatSemana(f.semana) }}</td>
            <td>{{ f.lunes ?? "-" }}</td>
            <td>{{ f.martes ?? "-" }}</td>
            <td>{{ f.miercoles ?? "-" }}</td>
            <td>{{ f.jueves ?? "-" }}</td>
            <td>{{ f.viernes ?? "-" }}</td>
            <td>{{ f.sabado ?? "-" }}</td>
            <td>{{ f.domingo ?? "-" }}</td>
            <td>{{ f.totalFaltas ?? 0 }}</td>
            <td>{{ f.totalPermisos ?? 0 }}</td>
            <td>
              <button class="btn btn-amarillo" @click="editarFalta(f)">
                <i class="bi bi-pencil-square"></i> Editar
              </button>
            </td>
          </tr>
          <tr v-if="faltasFiltradas.length === 0">
            <td colspan="15" style="text-align:center;">Sin registros</td>
          </tr>
        </tbody>
      </table>
    </div>

    <!-- 📝 Modal formulario -->
    <transition name="modal-fade">
      <div v-if="mostrarFormulario" class="modal-overlay" @click.self="cerrarFormulario">
        <div class="modal-content">
          <h3>Editar Falta</h3>

          <!-- Información del empleado (solo lectura) -->
          <div class="info-empleado">
            <p><strong>Empleado:</strong> {{ faltaActual.nombreEmpleado }}</p>
            <p><strong>Código:</strong> {{ faltaActual.codigoEmpleado }}</p>
            <p><strong>Departamento:</strong> {{ faltaActual.departamento }}</p>
            <p><strong>Semana:</strong> {{ formatSemana(faltaActual.semana) }}</p>
          </div>

          <form @submit.prevent="guardarFalta">
            <div class="dias-semana">
              <label>Días de la semana (F=Falta, P=Permiso):</label>
              <div class="dias-grid">
                <div>
                  <label>Lunes:</label>
                  <input v-model="faltaActual.lunes" placeholder="F/P" maxlength="1" />
                </div>
                <div>
                  <label>Martes:</label>
                  <input v-model="faltaActual.martes" placeholder="F/P" maxlength="1" />
                </div>
                <div>
                  <label>Miércoles:</label>
                  <input v-model="faltaActual.miercoles" placeholder="F/P" maxlength="1" />
                </div>
                <div>
                  <label>Jueves:</label>
                  <input v-model="faltaActual.jueves" placeholder="F/P" maxlength="1" />
                </div>
                <div>
                  <label>Viernes:</label>
                  <input v-model="faltaActual.viernes" placeholder="F/P" maxlength="1" />
                </div>
                <div>
                  <label>Sábado:</label>
                  <input v-model="faltaActual.sabado" placeholder="F/P" maxlength="1" />
                </div>
                <div>
                  <label>Domingo:</label>
                  <input v-model="faltaActual.domingo" placeholder="F/P" maxlength="1" />
                </div>
              </div>
            </div>

            <!-- Totales calculados -->
            <div class="totales">
              <p><strong>Total Faltas:</strong> {{ calcularTotalFaltas() }}</p>
              <p><strong>Total Permisos:</strong> {{ calcularTotalPermisos() }}</p>
            </div>

            <div class="acciones-modal">
              <button type="submit" class="btn btn-azul">Actualizar</button>
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
  import type { AsistenciaFalta } from "../models/AsistenciaFalta";
  import {
    getAsistenciaFaltas,
    updateAsistenciaFalta,
    deleteAsistenciaFalta,
    exportFaltasExcel,
    exportFaltasPDF
  } from "../services/asistenciaFaltaService";

  const faltas = ref<any[]>([]);
  const busqueda = ref("");
  const mostrarFormulario = ref(false);
  const loading = ref(false);

  const faltaActual = ref<Partial<AsistenciaFalta>>({
    idFalta: 0,
    idEmpleado: 0,
    nombreEmpleado: "",
    codigoEmpleado: "",
    departamento: "",
    semana: "",
    lunes: "",
    martes: "",
    miercoles: "",
    jueves: "",
    viernes: "",
    sabado: "",
    domingo: "",
    totalFaltas: 0,
    totalPermisos: 0,
    createdAt: ""
  });

  function formatSemana(semana: string): string {
    if (!semana) return '-';
    return semana;
  }

  function formatFecha(fecha: string): string {
    if (!fecha) return '-';
    return new Date(fecha).toLocaleDateString();
  }

  const faltasFiltradas = computed(() =>
    faltas.value.filter(f =>
      `${f.nombreEmpleado} ${f.departamento} ${f.codigoEmpleado}`.toLowerCase()
        .includes(busqueda.value.toLowerCase())
    )
  );

  onMounted(async () => {
    await cargarDatos();
  });

  async function cargarDatos() {
    try {
      faltas.value = await getAsistenciaFaltas();
    } catch (error) {
      console.error('Error cargando datos:', error);
      alert('Error al cargar los datos');
    }
  }

  function editarFalta(f: AsistenciaFalta) {
    faltaActual.value = { ...f };
    mostrarFormulario.value = true;
  }

  function calcularTotalFaltas(): number {
    const dias = [
      faltaActual.value.lunes,
      faltaActual.value.martes,
      faltaActual.value.miercoles,
      faltaActual.value.jueves,
      faltaActual.value.viernes,
      faltaActual.value.sabado,
      faltaActual.value.domingo
    ];
    return dias.filter(d => d && d.toUpperCase() === 'F').length;
  }

  function calcularTotalPermisos(): number {
    const dias = [
      faltaActual.value.lunes,
      faltaActual.value.martes,
      faltaActual.value.miercoles,
      faltaActual.value.jueves,
      faltaActual.value.viernes,
      faltaActual.value.sabado,
      faltaActual.value.domingo
    ];
    return dias.filter(d => d && d.toUpperCase() === 'P').length;
  }

  async function guardarFalta() {
    try {
      loading.value = true;

      // Preparar payload para el backend (solo campos editables)
      const payload = {
        lunes: faltaActual.value.lunes?.toUpperCase() || null,
        martes: faltaActual.value.martes?.toUpperCase() || null,
        miercoles: faltaActual.value.miercoles?.toUpperCase() || null,
        jueves: faltaActual.value.jueves?.toUpperCase() || null,
        viernes: faltaActual.value.viernes?.toUpperCase() || null,
        sabado: faltaActual.value.sabado?.toUpperCase() || null,
        domingo: faltaActual.value.domingo?.toUpperCase() || null
      };

      await updateAsistenciaFalta(faltaActual.value.idFalta!, payload);

      cerrarFormulario();
      await cargarDatos();
      alert('Registro actualizado exitosamente');

    } catch (error: any) {
      console.error('Error guardando falta:', error);

      if (error.response?.data) {
        alert(`Error: ${error.response.data}`);
      } else {
        alert('Error al guardar los datos. Verifique la consola para más detalles.');
      }
    } finally {
      loading.value = false;
    }
  }

  function cerrarFormulario() {
    mostrarFormulario.value = false;
    faltaActual.value = {
      idFalta: 0,
      idEmpleado: 0,
      nombreEmpleado: "",
      codigoEmpleado: "",
      departamento: "",
      semana: "",
      lunes: "",
      martes: "",
      miercoles: "",
      jueves: "",
      viernes: "",
      sabado: "",
      domingo: "",
      totalFaltas: 0,
      totalPermisos: 0,
      createdAt: ""
    };
  }

  async function exportExcel() {
    try {
      await exportFaltasExcel();
    } catch (error) {
      console.error('Error exportando Excel:', error);
      alert('Error al exportar Excel');
    }
  }

  async function exportPDF() {
    try {
      await exportFaltasPDF();
    } catch (error) {
      console.error('Error exportando PDF:', error);
      alert('Error al exportar PDF');
    }
  }
</script>

<style scoped>
  .faltas-container {
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
    margin-bottom: 20px;
  }

  .tabla {
    width: 100%;
    border-collapse: collapse;
    border-radius: 10px;
    overflow: hidden;
    box-shadow: 0 2px 6px rgba(0,0,0,0.05);
    font-size: 12px;
  }

    .tabla thead {
      background-color: #1e3a8a;
      color: white;
    }

    .tabla th, .tabla td {
      padding: 8px;
      text-align: center;
      border-bottom: 1px solid #e2e8f0;
    }

    .tabla tbody tr:nth-child(even) {
      background-color: #f8fafc;
    }

    .tabla tbody tr:hover {
      background-color: #f1f5f9;
    }

  .btn {
    padding: 4px 8px;
    border: none;
    border-radius: 4px;
    cursor: pointer;
    font-size: 12px;
    transition: background-color 0.2s ease;
    color: white;
    margin: 2px;
  }

  .btn-primary {
    background: #2563eb;
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

  .btn-azul {
    background: #2563eb;
  }

  .modal-overlay {
    position: fixed;
    inset: 0;
    background: rgba(0, 0, 0, 0.4);
    display: flex;
    justify-content: center;
    align-items: center;
    z-index: 1000;
  }

  .modal-content {
    background: white;
    padding: 20px;
    border-radius: 12px;
    width: 500px;
    max-width: 90%;
    max-height: 90vh;
    overflow-y: auto;
  }

  .info-empleado {
    background-color: #f8fafc;
    padding: 15px;
    border-radius: 8px;
    margin-bottom: 15px;
  }

    .info-empleado p {
      margin: 5px 0;
    }

  .modal-content label {
    font-weight: 600;
    margin-top: 8px;
    display: block;
  }

  .modal-content input, .modal-content select {
    width: 100%;
    padding: 8px;
    margin-top: 4px;
    border: 1px solid #cbd5e1;
    border-radius: 6px;
  }

  .dias-semana {
    margin: 15px 0;
  }

  .dias-grid {
    display: grid;
    grid-template-columns: repeat(2, 1fr);
    gap: 10px;
    margin-top: 10px;
  }

    .dias-grid div {
      display: flex;
      flex-direction: column;
    }

    .dias-grid label {
      font-size: 12px;
      margin-bottom: 4px;
    }

  .totales {
    background-color: #e0f2fe;
    padding: 10px;
    border-radius: 6px;
    margin: 15px 0;
  }

    .totales p {
      margin: 5px 0;
      font-weight: 600;
    }

  .acciones-modal {
    display: flex;
    justify-content: space-between;
    margin-top: 15px;
  }

  .modal-fade-enter-active, .modal-fade-leave-active {
    transition: all 0.3s ease;
  }

  .modal-fade-enter-from, .modal-fade-leave-to {
    opacity: 0;
    transform: scale(0.9);
  }

  @media (max-width: 768px) {
    .dias-grid {
      grid-template-columns: 1fr;
    }

    .tabla {
      font-size: 11px;
    }

      .tabla th, .tabla td {
        padding: 4px;
      }
  }
</style>
