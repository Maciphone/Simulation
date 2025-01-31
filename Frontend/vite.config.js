import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react-swc'

// https://vite.dev/config/
export default defineConfig({
  plugins: [react()],
  server: {
    proxy: {
      "/simulationHub": {
        target: "http://localhost:5050", // A simulattion címe
        changeOrigin: true,
        secure: false,
        ws: true, // WebSocket támogatás
      },
      "/api": {
        target: "http://localhost:5000", // A backendn címe
        changeOrigin: true,
        secure: false,
      },
    },
  },
});
