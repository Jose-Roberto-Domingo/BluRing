import { defineConfig } from 'vite';
import plugin from '@vitejs/plugin-vue';

// https://vitejs.dev/config/
export default defineConfig({
    plugins: [plugin()],
  server: {
    host: true,
    port: 50035,
    strictPort: true,
    allowedHosts: ["f9f7d3cd7ed8.ngrok-free.app"],
    }
})
