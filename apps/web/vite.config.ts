import path from 'node:path';
import { defineConfig } from 'vite';
import react from '@vitejs/plugin-react';
import tailwindcss from '@tailwindcss/vite';

export default defineConfig({
  plugins: [react(), tailwindcss()],
  resolve: {
    alias: {
      '@': path.resolve(__dirname, './src'),
      '@naswood/ui': path.resolve(__dirname, '../../packages/ui/src'),
    },
  },
  server: {
    host: true,
    port: 5173,
    // Cursor / cloud port-forward previews send a non-localhost Host header.
    // Without this, Vite returns 403 and login shows a connection error.
    allowedHosts: true,
    proxy: {
      '/api': {
        target: 'http://127.0.0.1:5080',
        changeOrigin: true,
      },
      '/health': {
        target: 'http://127.0.0.1:5080',
        changeOrigin: true,
      },
    },
  },
});

