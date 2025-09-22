import { createRouter, createWebHistory } from "vue-router";

// Vistas (puedes crearlas en src/views/)
import Home from "../views/Home.vue";
import AsistenciaFaltas from "../views/AsistenciaFaltas.vue";
import AsistenciaResumen from "../views/AsistenciaResumen.vue";
import Empleados from "../views/Empleados.vue";
import Horarios from "../views/EmpleadosHorario.vue";
import Prenomina from "../views/Prenomina.vue";
import RegistroAsistencia from "../views/RegistroAsistencia.vue";

const routes = [
  { path: "/", name: "Home", component: Home },
  { path: "/asistencia/faltas", name: "AsistenciaFaltas", component: AsistenciaFaltas },
  { path: "/asistencia/resumen", name: "AsistenciaResumen", component: AsistenciaResumen },
  { path: "/empleados", name: "Empleados", component: Empleados },
  { path: "/horarios", name: "Horarios", component: Horarios },
  { path: "/prenomina", name: "Prenomina", component: Prenomina },
  { path: "/asistencia", name: "RegistroAsistencia", component: RegistroAsistencia },
];

const router = createRouter({
  history: createWebHistory(),
  routes,
});

export default router;
