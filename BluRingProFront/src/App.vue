<template>
  <div id="app">
    <!-- NAVBAR -->
    <nav class="navbar navbar-expand-lg" id="main-navbar">
      <div class="container-fluid">
        <!-- LOGO -->
        <router-link to="/">
          <img src="/Logo2.png" width="77" height="43" alt="Logo">
        </router-link>

        <button class="navbar-toggler" type="button" data-bs-toggle="collapse" data-bs-target="#navbarNav"
                aria-controls="navbarNav" aria-expanded="false" aria-label="Toggle navigation">
          <span class="navbar-toggler-icon"></span>
        </button>

        <!-- MENÚ CENTRADO -->
        <div class="collapse navbar-collapse" id="navbarNav">
          <ul class="navbar-nav mx-auto">
            <li class="nav-item">
              <router-link to="/empleados" class="nav-link"><i class="bi bi-person-vcard"></i> Empleados</router-link>
            </li>
            <li class="nav-item">
              <router-link to="/asistencia" class="nav-link"><i class="bi bi-clock"></i> Asistencias</router-link>
            </li>
            <li class="nav-item">
              <router-link to="/prenomina" class="nav-link"><i class="bi bi-journal-text"></i> Prenómina</router-link>
            </li>
          </ul>
        </div>

        <!-- USUARIO + BOTÓN MODO -->
        <ul class="navbar-nav ms-auto">
          <li class="nav-item">
            <span class="navbar-brand">{{ nombreUsuario }}</span>
          </li>
          <li class="nav-item">
            <button id="theme-toggle" class="btn btn-outline-light me-2">
              <i class="bi bi-moon-fill" id="theme-icon"></i>
            </button>
          </li>
          <li class="nav-item">
            <button @click="cerrarSesion" class="btn btn-outline-danger logout-button">Cerrar sesión</button>
          </li>
        </ul>
      </div>
    </nav>

    <!-- CONTENIDO DINÁMICO -->
    <main class="py-4">
      <router-view />
    </main>
  </div>
</template>

<script setup lang="ts">
  import { ref, onMounted } from "vue";
  import { useRouter } from "vue-router";

  const nombreUsuario = ref("Usuario"); // Cambiar por lógica real de sesión
  const router = useRouter();

  function cerrarSesion() {
    // Lógica para cerrar sesión
    localStorage.removeItem("token");
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

<style>
  /* Navbar */
  .navbar {
    background-color: #1e3a8a; /* Azul principal más moderno */
    box-shadow: 0 4px 10px rgba(0,0,0,0.15);
    padding: 0.5rem 1rem;
    font-size: 14px;
  }

  .navbar-brand img {
    border-radius: 6px;
    transition: transform 0.3s ease;
  }

    .navbar-brand img:hover {
      transform: scale(1.05);
    }

  .navbar-nav .nav-link {
    color: #ffffff !important;
    margin: 0 8px;
    transition: all 0.3s ease;
    font-weight: 500;
  }

    .navbar-nav .nav-link:hover {
      color: #0ccc28ff !important; /* Verde suave al pasar el mouse */
      transform: scale(1.05);
    }

    .navbar-nav .nav-link.active {
      border-bottom: 2px solid #0ccc28ff;
      color: #0ccc28ff !important;
    }

  #theme-toggle {
    border: none;
    background: transparent;
    color: #ffffff;
    font-size: 18px;
    transition: transform 0.3s ease;
  }

    #theme-toggle:hover {
      transform: rotate(20deg);
    }

  .logout-button {
    background-color: transparent;
    border: 1px solid #ef4444;
    color: #ef4444;
    padding: 6px 14px;
    border-radius: 6px;
    font-weight: 500;
    transition: all 0.3s ease;
  }

    .logout-button:hover {
      background-color: #ef4444;
      color: white;
    }

  /* Centrado del menú */
  .navbar-nav.mx-auto {
    text-align: center;
  }

  /* Responsive */
  @media (max-width: 992px) {
    .navbar-nav {
      text-align: center;
      margin-top: 10px;
    }

      .navbar-nav .nav-link {
        margin: 5px 0;
      }
  }

  /* General */
  body {
    font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
  }

  main {
    margin: 20px;
  }

</style>
