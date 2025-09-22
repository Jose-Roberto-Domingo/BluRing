<template>
  <div class="prenominas-container">
    <h2>Registro de Prenóminas</h2>

    <!-- 🔍 Barra de búsqueda -->
    <input v-model="busqueda"
           type="text"
           placeholder="Buscar por código, nombre o departamento..."
           class="busqueda" />

    <!-- 🔘 Botones de acción -->
    <div class="acciones-top">
      <button class="btn btn-verde-oscuro" @click="exportExcel">
        <i class="bi bi-download"></i> Exportar Excel
      </button>
      <button class="btn btn-rojo-oscuro" @click="exportPDF">
        <i class="bi bi-download"></i> Exportar PDF
      </button>
      <router-link to="/dashboard" class="btn btn-primary">Volver a Dashboard</router-link>
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
            <th>Días Trabajados</th>
            <th>Horas Extras</th>
            <th>Acciones</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="p in prenominasFiltradas" :key="p.idPrenomina">
            <td>{{ p.codigoEmpleado ?? '-' }}</td>
            <td>{{ p.nombreCompleto }}</td>
            <td>{{ p.departamento }}</td>
            <td>{{ formatSemana(p.semana) }}</td>
            <td>{{ p.lunes ?? 0 }}</td>
            <td>{{ p.martes ?? 0 }}</td>
            <td>{{ p.miercoles ?? 0 }}</td>
            <td>{{ p.jueves ?? 0 }}</td>
            <td>{{ p.viernes ?? 0 }}</td>
            <td>{{ p.sabado ?? 0 }}</td>
            <td>{{ p.domingo ?? 0 }}</td>
            <td>{{ p.diasTrabajados ?? 0 }}</td>
            <td>{{ p.horasExtras ?? 0 }}</td>
            <td>
              <button class="btn btn-amarillo" @click="editarPrenomina(p)">
                <i class="bi bi-pencil-square"></i> Editar
              </button>
            </td>
          </tr>
          <tr v-if="prenominasFiltradas.length === 0">
            <td colspan="14" style="text-align:center;">Sin registros</td>
          </tr>
        </tbody>
      </table>
    </div>

    <!-- 📝 Modal formulario -->
    <transition name="modal-fade">
      <div v-if="mostrarFormulario" class="modal-overlay" @click.self="cerrarFormulario">
        <div class="modal-content">
          <h3>Editar Prenómina</h3>

          <!-- Información del empleado (solo lectura) -->
          <div class="info-empleado">
            <p><strong>Empleado:</strong> {{ prenominaActual.nombreCompleto }}</p>
            <p><strong>Código:</strong> {{ prenominaActual.codigoEmpleado }}</p>
            <p><strong>Departamento:</strong> {{ prenominaActual.departamento }}</p>
            <p><strong>Semana:</strong> {{ formatSemana(prenominaActual.semana) }}</p>
          </div>

          <form @submit.prevent="guardarPrenomina">
            <div class="dias-semana">
              <label>Horas trabajadas por día:</label>
              <div class="dias-grid">
                <div>
                  <label>Lunes:</label>
                  <input v-model="prenominaActual.lunes" type="number" min="0" max="24" step="0.5" />
                </div>
                <div>
                  <label>Martes:</label>
                  <input v-model="prenominaActual.martes" type="number" min="0" max="24" step="0.5" />
                </div>
                <div>
                  <label>Miércoles:</label>
                  <input v-model="prenominaActual.miercoles" type="number" min="0" max="24" step="0.5" />
                </div>
                <div>
                  <label>Jueves:</label>
                  <input v-model="prenominaActual.jueves" type="number" min="0" max="24" step="0.5" />
                </div>
                <div>
                  <label>Viernes:</label>
                  <input v-model="prenominaActual.viernes" type="number" min="0" max="24" step="0.5" />
                </div>
                <div>
                  <label>Sábado:</label>
                  <input v-model="prenominaActual.sabado" type="number" min="0" max="24" step="0.5" />
                </div>
                <div>
                  <label>Domingo:</label>
                  <input v-model="prenominaActual.domingo" type="number" min="0" max="24" step="0.5" />
                </div>
              </div>
            </div>

            <!-- Totales calculados -->
            <div class="totales">
              <p><strong>Días Trabajados:</strong> {{ calcularDiasTrabajados() }}</p>
              <p><strong>Horas Extras:</strong> {{ calcularHorasExtras() }}</p>
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
  import type { Prenomina } from "../models/Prenomina";
  import type { Empleado } from "../services/empleadosService";
  import { getEmpleados } from "../services/empleadosService";
  import {
    getPrenominas,
    updatePrenomina,
    exportPrenominasExcel,
    exportPrenominasPDF
  } from "../services/prenominaService";

  const prenominas = ref<any[]>([]);
  const empleados = ref<Empleado[]>([]);
  const busqueda = ref("");
  const mostrarFormulario = ref(false);
  const loading = ref(false);

  const prenominaActual = ref<Partial<Prenomina>>({
    idPrenomina: 0,
    idEmpleado: 0,
    codigoEmpleado: "",
    nombreCompleto: "",
    departamento: "",
    semana: "",
    lunes: 0,
    martes: 0,
    miercoles: 0,
    jueves: 0,
    viernes: 0,
    sabado: 0,
    domingo: 0,
    diasTrabajados: 0,
    horasExtras: 0
  });

  function formatSemana(semana: string): string {
    if (!semana) return '-';
    return semana;
  }

  const prenominasFiltradas = computed(() =>
    prenominas.value.filter(p =>
      `${p.codigoEmpleado} ${p.nombreCompleto} ${p.departamento} ${p.semana}`.toLowerCase()
        .includes(busqueda.value.toLowerCase())
    )
  );

  onMounted(async () => {
    await cargarDatos();
  });

  async function cargarDatos() {
    try {
      // Cargar empleados y prenóminas en paralelo
      const [empleadosData, prenominasData] = await Promise.all([
        getEmpleados(),
        getPrenominas()
      ]);

      empleados.value = empleadosData;

      // Combinar datos de prenóminas con datos de empleados
      prenominas.value = prenominasData.map(prenomina => {
        const empleado = empleadosData.find(e => e.idEmpleado === prenomina.idEmpleado);

        return {
          ...prenomina,
          codigoEmpleado: empleado?.codigoEmpleado || `EMP${prenomina.idEmpleado.toString().padStart(4, '0')}`,
          nombreCompleto: empleado ? `${empleado.nombre} ${empleado.apellidoPaterno} ${empleado.apellidoMaterno}` : `Empleado ${prenomina.idEmpleado}`,
          departamento: empleado?.departamento || 'Sin departamento'
        };
      });
    } catch (error) {
      console.error('Error cargando datos:', error);
      alert('Error al cargar los datos');
    }
  }

  function editarPrenomina(p: Prenomina) {
    prenominaActual.value = { ...p };
    mostrarFormulario.value = true;
  }

  function calcularDiasTrabajados(): number {
    const dias = [
      prenominaActual.value.lunes,
      prenominaActual.value.martes,
      prenominaActual.value.miercoles,
      prenominaActual.value.jueves,
      prenominaActual.value.viernes,
      prenominaActual.value.sabado,
      prenominaActual.value.domingo
    ];
    return dias.filter(d => d && d > 0).length;
  }

  function calcularHorasExtras(): number {
    const dias = [
      prenominaActual.value.lunes,
      prenominaActual.value.martes,
      prenominaActual.value.miercoles,
      prenominaActual.value.jueves,
      prenominaActual.value.viernes,
      prenominaActual.value.sabado,
      prenominaActual.value.domingo
    ];

    // Suponiendo que la jornada normal es de 8 horas
    const horasNormales = 8;
    let horasExtras = 0;

    dias.forEach(horas => {
      if (horas && horas > horasNormales) {
        horasExtras += horas - horasNormales;
      }
    });

    return horasExtras;
  }

  async function guardarPrenomina() {
    try {
      loading.value = true;

      // Preparar payload para el backend
      const payload = {
        lunes: prenominaActual.value.lunes || 0,
        martes: prenominaActual.value.martes || 0,
        miercoles: prenominaActual.value.miercoles || 0,
        jueves: prenominaActual.value.jueves || 0,
        viernes: prenominaActual.value.viernes || 0,
        sabado: prenominaActual.value.sabado || 0,
        domingo: prenominaActual.value.domingo || 0
      };

      await updatePrenomina(prenominaActual.value.idPrenomina!, payload);

      cerrarFormulario();
      await cargarDatos();
      alert('Registro actualizado exitosamente');

    } catch (error: any) {
      console.error('Error guardando prenomina:', error);

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
    prenominaActual.value = {
      idPrenomina: 0,
      idEmpleado: 0,
      codigoEmpleado: "",
      nombreCompleto: "",
      departamento: "",
      semana: "",
      lunes: 0,
      martes: 0,
      miercoles: 0,
      jueves: 0,
      viernes: 0,
      sabado: 0,
      domingo: 0,
      diasTrabajados: 0,
      horasExtras: 0
    };
  }

  async function exportExcel() {
    try {
      await exportPrenominasExcel();
    } catch (error) {
      console.error('Error exportando Excel:', error);
      alert('Error al exportar Excel');
    }
  }

  async function exportPDF() {
    try {
      await exportPrenominasPDF();
    } catch (error) {
      console.error('Error exportando PDF:', error);
      alert('Error al exportar PDF');
    }
  }
</script>

<style scoped>
  .prenominas-container {
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
    margin-bottom: 15px;
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
