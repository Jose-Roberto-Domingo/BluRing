<template>
  <div class="container">
    <h2>Gestión de Horarios</h2>

    <div class="top-buttons">
      <button class="btn btn-success" @click="abrirFormulario"><i class="bi bi-plus-circle"></i> Agregar horario</button>
      <router-link to="/empleados" class="btn btn-primary">Volver a Empleados</router-link>
    </div>

    <div class="table-responsive">
      <table class="tabla">
        <thead>
          <tr>
            <th>ID</th>
            <th>Empleado</th>
            <th>Hora Entrada</th>
            <th>Hora Salida</th>
            <th>Jornada</th>
            <th>Acciones</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="horario in horarios" :key="horario.idHorario">
            <td>{{ horario.idHorario }}</td>
            <td>{{ horario.nombreEmpleado }}</td>
            <td>{{ horario.horaEntradaHorario }}</td>
            <td>{{ horario.horaSalidaHorario }}</td>
            <td>{{ horario.jornada }}</td>
            <td>
              <button class="btn btn-warning" @click="editarHorario(horario)"><i class="bi bi-pencil-square"></i> Editar</button>
              <button class="btn btn-danger" @click="eliminarHorario(horario.idHorario)"><i class="bi bi-trash"></i> Eliminar</button>
            </td>
          </tr>
        </tbody>
      </table>
    </div>

    <transition name="modal-fade">
      <div v-if="mostrarFormulario" class="modal-overlay" @click.self="cerrarFormulario">
        <div class="modal-content">
          <h3>{{ horarioActual.idHorario ? "Editar Horario" : "Agregar Horario" }}</h3>

          <div v-if="Object.keys(errores).length" class="errores">
            <ul>
              <li v-for="(msgs, field) in errores" :key="field">
                <strong>{{ field }}:</strong>
                <span v-for="msg in msgs" :key="msg">{{ msg }}</span>
              </li>
            </ul>
          </div>

          <form @submit.prevent="guardarHorario">
            <label>Empleado</label>
            <select v-model="horarioActual.idEmpleado" required>
              <option v-for="emp in empleados" :key="emp.idEmpleado" :value="emp.idEmpleado">
                {{ emp.nombre }} {{ emp.apellidoPaterno }} {{ emp.apellidoMaterno }}
              </option>
            </select>

            <label>Hora Entrada</label>
            <input type="time" v-model="horarioActual.horaEntradaHorario" required />

            <label>Hora Salida</label>
            <input type="time" v-model="horarioActual.horaSalidaHorario" required />

            <label>Jornada</label>
            <input type="text" v-model="horarioActual.jornada" placeholder="Jornada (horas)" required />

            <div class="acciones">
              <button type="submit" class="btn btn-success">
                {{ horarioActual.idHorario ? "Actualizar" : "Guardar" }}
              </button>
              <button type="button" class="btn btn-danger" @click="cerrarFormulario">Cancelar</button>
            </div>
          </form>
        </div>
      </div>
    </transition>
  </div>
</template>

<script setup lang="ts">
  import { ref, onMounted } from "vue";
  import type { EmpleadoHorario } from "../services/empleadosHorarioService";
  import type { Empleado } from "../services/empleadosService";
  import { getHorarios, createHorario, updateHorario, deleteHorario } from "../services/empleadosHorarioService";
  import { getEmpleados } from "../services/empleadosService";

  const horarios = ref<EmpleadoHorario[]>([]);
  const empleados = ref<Empleado[]>([]);
  const mostrarFormulario = ref(false);
  const errores = ref<Record<string, string[]>>({}); // 🔹 Errores de validación

  const horarioActual = ref<EmpleadoHorario>({
    idHorario: 0,
    idEmpleado: 0,
    horaEntradaHorario: "",
    horaSalidaHorario: "",
    jornada: 0,
    nombreEmpleado: "",
  });

  onMounted(async () => {
    await cargarHorarios();
    await cargarEmpleados();
  });

  async function cargarHorarios() {
    horarios.value = await getHorarios();
  }

  async function cargarEmpleados() {
    empleados.value = await getEmpleados();
  }

  function abrirFormulario() {
    horarioActual.value = { idHorario: 0, idEmpleado: 0, horaEntradaHorario: "", horaSalidaHorario: "", jornada: 0, nombreEmpleado: "" };
    mostrarFormulario.value = true;
    errores.value = {}; // limpiar errores al abrir modal
  }

  function editarHorario(h: EmpleadoHorario) {
    // 🔹 Obtener datos completos del empleado
    const empleadoSeleccionado = empleados.value.find(
      (e: any) => e.idEmpleado === h.idEmpleado
    );

    if (!empleadoSeleccionado) {
      alert("Empleado no encontrado.");
      return;
    }

    // 🔹 Construir horario actual incluyendo nombreEmpleado
    horarioActual.value = {
      idHorario: h.idHorario,
      idEmpleado: h.idEmpleado,
      horaEntradaHorario: h.horaEntradaHorario.length === 5
        ? h.horaEntradaHorario + ":00"
        : h.horaEntradaHorario,
      horaSalidaHorario: h.horaSalidaHorario.length === 5
        ? h.horaSalidaHorario + ":00"
        : h.horaSalidaHorario,
      jornada: h.jornada ?? 0,
      nombreEmpleado: `${empleadoSeleccionado.nombre} ${empleadoSeleccionado.apellidoPaterno} ${empleadoSeleccionado.apellidoMaterno}`,
      idEmpleadoNavigation: {
        idEmpleado: empleadoSeleccionado.idEmpleado,
        nombre: empleadoSeleccionado.nombre,
        apellidoPaterno: empleadoSeleccionado.apellidoPaterno,
        apellidoMaterno: empleadoSeleccionado.apellidoMaterno,
        departamento: empleadoSeleccionado.departamento,
        codigoEmpleado: empleadoSeleccionado.codigoEmpleado,
      },
    };

    mostrarFormulario.value = true;
  }

  async function guardarHorario() {
    try {
      const h = horarioActual.value;

      // 🔹 Validaciones locales
      if (!h.idEmpleado || h.idEmpleado === 0) {
        alert("Debe seleccionar un empleado.");
        return;
      }
      if (!h.horaEntradaHorario) {
        alert("Debe ingresar hora de entrada.");
        return;
      }
      if (!h.horaSalidaHorario) {
        alert("Debe ingresar hora de salida.");
        return;
      }

      // Formatear horas a HH:mm:ss
      if (h.horaEntradaHorario.length === 5) {
        h.horaEntradaHorario = h.horaEntradaHorario + ":00";
      }
      if (h.horaSalidaHorario.length === 5) {
        h.horaSalidaHorario = h.horaSalidaHorario + ":00";
      }

      // 🔹 Obtener datos del empleado
      const empleadoSeleccionado = empleados.value.find(
        (e: any) => e.idEmpleado === h.idEmpleado
      );
      if (!empleadoSeleccionado) {
        alert("Empleado no encontrado.");
        return;
      }

      // 🔹 Construir payload con nombreEmpleado
      const payload = {
        horaEntradaHorario: h.horaEntradaHorario,
        horaSalidaHorario: h.horaSalidaHorario,
        jornada: Number(h.jornada),
        idEmpleado: h.idEmpleado,
        nombreEmpleado: `${empleadoSeleccionado.nombre} ${empleadoSeleccionado.apellidoPaterno} ${empleadoSeleccionado.apellidoMaterno}`,
        idEmpleadoNavigation: {
          idEmpleado: empleadoSeleccionado.idEmpleado,
          nombre: empleadoSeleccionado.nombre,
          apellidoPaterno: empleadoSeleccionado.apellidoPaterno,
          apellidoMaterno: empleadoSeleccionado.apellidoMaterno,
          departamento: empleadoSeleccionado.departamento,
          codigoEmpleado: empleadoSeleccionado.codigoEmpleado,
        },
      };

      if (h.idHorario && h.idHorario > 0) {
        await updateHorario(h.idHorario, payload);
      } else {
        await createHorario(payload);
      }

      // 🔹 Refrescar tabla
      horarios.value = await getHorarios();

      // 🔹 Cerrar modal
      cerrarFormulario();

    } catch (error: any) {
      console.error("Error guardando horario:", error.response?.data || error.message);

      if (error.response?.data?.errors) {
        const errores = error.response.data.errors;
        let mensaje = "Errores de validación:\n";
        for (const campo in errores) {
          mensaje += `${campo}: ${errores[campo].join(", ")}\n`;
        }
        alert(mensaje);
      } else {
        alert("Error al guardar horario. Revisa los datos e inténtalo de nuevo.");
      }
    }
  }

  async function eliminarHorario(id: number) {
    if (confirm("¿Seguro que deseas eliminar este horario?")) {
      await deleteHorario(id);
      await cargarHorarios();
    }
  }

  function cerrarFormulario() {
    mostrarFormulario.value = false;
    errores.value = {}; // limpiar errores al cerrar
  }
</script>

<style scoped>
  .container {
    padding: 20px;
    font-family: "Segoe UI", sans-serif;
  }

  h2 {
    color: #1e3a8a;
    margin-bottom: 15px;
  }

  .top-buttons {
    display: flex;
    flex-wrap: wrap;
    gap: 10px;
    margin-bottom: 15px;
  }

  .table-responsive {
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

  .btn-success {
    background-color: #28a745;
  }

  .btn-warning {
    background-color: #facc15;
    color: #1e293b;
  }

  .btn-danger {
    background-color: #ef4444;
  }

  .btn-primary {
    background-color: #2563eb;
  }

  /* Modal igual que empleados */
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

    .modal-content input,
    .modal-content select {
      width: 100%;
      padding: 8px;
      margin-top: 4px;
      border: 1px solid #cbd5e1;
      border-radius: 6px;
    }

  .acciones {
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

  .errores {
    background: #ffe0e0;
    color: #b30000;
    border: 1px solid #ff8080;
    padding: 10px;
    margin-bottom: 10px;
    border-radius: 5px;
  }
</style>
