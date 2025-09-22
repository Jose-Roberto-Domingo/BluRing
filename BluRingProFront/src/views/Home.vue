<template>
  <div>

    <!-- HOME CONTENT -->
    <div class="container">
      <div class="logo-section">
        <img src="/Logo2.png" alt="Logo" class="logo-img">
      </div>

      <div class="welcome-section">
        <h2 class="welcome-title">Bienvenido a Bluring Pro</h2>
        <p>Selecciona una de las opciones para continuar</p>
      </div>

      <div class="row justify-content-center">
        <!-- Empleados -->
        <div class="col-md-3 col-sm-6 mb-4">
          <router-link to="/empleados" class="card-link">
            <div class="card-menu">
              <img src="/empleados.png" alt="Empleados">
              <h5 class="card-title">Empleados</h5>
            </div>
          </router-link>
        </div>

        <!-- Registro de llegadas -->
        <div class="col-md-3 col-sm-6 mb-4">
          <router-link to="/asistencia" class="card-link">
            <div class="card-menu">
              <img src="/asistencia.png" alt="Registro de llegadas">
              <h5 class="card-title">Registro de llegadas</h5>
            </div>
          </router-link>
        </div>

        <!-- Prenómina -->
        <div class="col-md-3 col-sm-6 mb-4">
          <router-link to="/prenomina" class="card-link">
            <div class="card-menu">
              <img src="/prenomina.png" alt="Prenómina">
              <h5 class="card-title">Prenómina</h5>
            </div>
          </router-link>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from "vue";
import { useRouter } from "vue-router";

const nombreUsuario = ref("Usuario"); // Reemplaza con lógica real de sesión
const router = useRouter();

function cerrarSesion() {
  // Lógica para cerrar sesión
  localStorage.removeItem("token"); // ejemplo
  router.push("/login");
}

// Tema oscuro/claro
function applyTheme(theme: string) {
  const body = document.body;
  const navbar = document.getElementById("main-navbar") as HTMLElement;
  const icon = document.getElementById("theme-icon") as HTMLElement;

  if (theme === "light") {
    body.setAttribute("data-bs-theme", "light");
    navbar?.setAttribute("data-bs-theme", "light");
    icon?.classList.replace("bi-moon-fill", "bi-sun-fill");
  } else {
    body.setAttribute("data-bs-theme", "dark");
    navbar?.setAttribute("data-bs-theme", "dark");
    icon?.classList.replace("bi-sun-fill", "bi-moon-fill");
  }
}

let savedTheme = localStorage.getItem("theme") || "dark";
applyTheme(savedTheme);

onMounted(() => {
  const btn = document.getElementById("theme-toggle");
  btn?.addEventListener("click", () => {
    savedTheme = savedTheme === "dark" ? "light" : "dark";
    localStorage.setItem("theme", savedTheme);
    applyTheme(savedTheme);
  });
});
</script>

<style scoped>
  /* Navbar */
  .navbar {
    background-color: #004080 !important;
    box-shadow: 0 10px 20px rgba(0,0,0,0.19), 0 6px 6px rgba(0,0,0,0.23);
  }

  .navbar-brand, .navbar-nav .nav-link {
    color: #ffffff !important;
    text-shadow: 1px 1px 2px rgba(0,0,0,0.5);
    transition: all 0.3s ease;
  }

    .navbar-brand:hover, .navbar-nav .nav-link:hover {
      text-shadow: 3px 3px 3px rgba(0,0,0,0.5);
      transform: scale(1.05);
      color: #0ccc28ff !important;
    }

    .navbar-nav .nav-link.active {
      background-color: #007bff;
      border-radius: 5px;
    }

  .logout-button {
    padding: 5px 20px;
    margin: 5px;
    cursor: pointer;
    border-radius: 5px;
  }

  .navbar-nav {
    margin: 0 auto;
  }

  /* Home */
  .logo-section {
    text-align: center;
    margin: 20px 0;
  }

  .logo-img {
    max-width: 400px;
    height: auto;
  }

  .welcome-section {
    text-align: center;
    padding: 20px 0;
    margin-bottom: 40px;
  }

  .welcome-title {
    color: #007bff;
    font-size: 2em;
    font-weight: bold;
  }

  .card-menu {
    border: 1px solid #e0e0e0;
    border-radius: 12px;
    box-shadow: 0 4px 10px rgba(0,0,0,0.1);
    transition: transform 0.3s, box-shadow 0.3s;
    text-align: center;
    padding: 20px;
  }

    .card-menu:hover {
      transform: translateY(-8px);
      box-shadow: 0 8px 20px rgba(0,0,0,0.15);
    }

    .card-menu img {
      max-width: 100px;
      height: auto;
      margin-bottom: 15px;
    }

  .card-link {
    text-decoration: none;
  }

    .card-link:hover {
      text-decoration: underline;
    }

  .card-title {
    font-size: 1.2em;
    color: #343a40;
    font-weight: bold;
  }
</style>
