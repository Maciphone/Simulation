import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react-swc'

// https://vite.dev/config/
export default defineConfig({
  plugins: [react()],
  server: {
    proxy: {
      "/simulationHub": {
        target: "http://localhost:5213", // A backend címe
        changeOrigin: true,
        secure: false,
        ws: true, // WebSocket támogatás
      },
    },
  },
});
