<template>
  <div class="empleados-container">
    <h2>Gestión de Empleados</h2>

    <!-- 🔍 Búsqueda -->
    <input v-model="busqueda"
           type="text"
           placeholder="Buscar empleado..."
           class="busqueda" />

    <!-- 🔘 Acciones principales -->
    <div class="acciones-top">
      <button class="btn btn-azul" @click="abrirFormulario"><i class="bi bi-person-plus"></i> Agregar empleado</button>
      <router-link to="/horarios" class="btn btn-verde"><i class="bi bi-clock"></i> Horario de Empleados</router-link>
    </div>

    <!-- 📋 Tabla -->
    <div class="tabla-responsive">
      <table class="tabla">
        <thead>
          <tr>
            <th>ID</th>
            <th>Código</th>
            <th>Departamento</th>
            <th>Nombre</th>
            <th>Apellido Paterno</th>
            <th>Apellido Materno</th>
            <th>Acciones</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="empleado in empleadosFiltrados" :key="empleado.idEmpleado">
            <td>{{ empleado.idEmpleado }}</td>
            <td>{{ empleado.codigoEmpleado }}</td>
            <td>{{ empleado.departamento }}</td>
            <td>{{ empleado.nombre }}</td>
            <td>{{ empleado.apellidoPaterno }}</td>
            <td>{{ empleado.apellidoMaterno }}</td>
            <td>
              <button class="btn btn-amarillo" @click="editarEmpleado(empleado)"><i class="bi bi-pencil-square"></i> Editar</button>
              <button class="btn btn-rojo" @click="eliminarEmpleado(empleado.idEmpleado)"><i class="bi bi-trash"></i> Eliminar</button>
            </td>
          </tr>
          <tr v-if="empleadosFiltrados.length === 0">
            <td colspan="7" style="text-align:center;">No hay empleados registrados</td>
          </tr>
        </tbody>
      </table>
    </div>

    <!-- 📝 Modal formulario -->
    <transition name="modal-fade">
      <div v-if="mostrarFormulario" class="modal-overlay" @click.self="cerrarFormulario">
        <div class="modal-content">
          <h3>{{ empleadoActual.idEmpleado ? "Editar Empleado" : "Agregar Empleado" }}</h3>

          <form @submit.prevent="guardarEmpleado">
            <input v-model="empleadoActual.departamento" placeholder="Departamento" required />
            <input v-model="empleadoActual.nombre" placeholder="Nombre" required />
            <input v-model="empleadoActual.apellidoPaterno" placeholder="Apellido Paterno" required />
            <input v-model="empleadoActual.apellidoMaterno" placeholder="Apellido Materno" required />

            <div class="acciones-modal">
              <button type="submit" class="btn btn-azul">
                {{ empleadoActual.idEmpleado ? "Actualizar" : "Guardar" }}
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
  import type { Empleado } from "../services/empleadosService";
  import { getEmpleados, createEmpleado, updateEmpleado, deleteEmpleado } from "../services/empleadosService";

  const empleados = ref<Empleado[]>([]);
  const busqueda = ref("");
  const mostrarFormulario = ref(false);

  const empleadoActual = ref<Empleado>({
    idEmpleado: 0,
    codigoEmpleado: "",
    departamento: "",
    nombre: "",
    apellidoPaterno: "",
    apellidoMaterno: "",
  });

  const empleadosFiltrados = computed(() => {
    const busquedaLower = busqueda.value.toLowerCase();
    return empleados.value.filter((e) =>
      `${e.nombre} ${e.apellidoPaterno} ${e.apellidoMaterno} ${e.departamento}`
        .toLowerCase()
        .includes(busquedaLower)
    );
  });

  onMounted(() => cargarEmpleados());

  async function cargarEmpleados() {
    try {
      empleados.value = await getEmpleados();
      console.log("✅ Empleados cargados:", empleados.value);
    } catch (error) {
      console.error("❌ Error al cargar empleados:", error);
    }
  }

  function abrirFormulario() {
    empleadoActual.value = {
      idEmpleado: 0,
      codigoEmpleado: "",
      departamento: "",
      nombre: "",
      apellidoPaterno: "",
      apellidoMaterno: "",
    };
    mostrarFormulario.value = true;
  }

  function editarEmpleado(emp: Empleado) {
    empleadoActual.value = { ...emp };
    mostrarFormulario.value = true;
  }

  async function guardarEmpleado() {
    try {
      const payload = {
        CodigoEmpleado: empleadoActual.value.codigoEmpleado || "", 
        Departamento: empleadoActual.value.departamento,
        Nombre: empleadoActual.value.nombre,
        ApellidoPaterno: empleadoActual.value.apellidoPaterno,
        ApellidoMaterno: empleadoActual.value.apellidoMaterno,
      };

      if (empleadoActual.value.idEmpleado) {
        await updateEmpleado(empleadoActual.value.idEmpleado, payload);
      } else {
        await createEmpleado(payload);
      }
      cerrarFormulario();
      await cargarEmpleados();
    } catch (error) {
      console.error("❌ Error al guardar empleado:", error);
    }
  }

  async function eliminarEmpleado(id: number) {
    if (confirm("¿Seguro que deseas eliminar este empleado?")) {
      try {
        await deleteEmpleado(id);
        await cargarEmpleados();
      } catch (error) {
        console.error("❌ Error al eliminar empleado:", error);
      }
    }
  }

  function cerrarFormulario() {
    mostrarFormulario.value = false;
  }
</script>


<style scoped>
  .empleados-container {
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

    .modal-content input {
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
