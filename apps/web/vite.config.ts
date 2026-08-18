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
    host: '0.0.0.0',
    port: 5173,
    strictPort: true,
    // Cursor / cloud port-forward previews send a non-localhost Host header.
    // Without this, Vite returns 403 and the preview stays blank/black.
    allowedHosts: true,
    // HMR websocket through Cursor port proxy can fail and leave a blank frame.
    // Disable overlay-blocking; client still loads the app.
    hmr: {
      overlay: true,
      clientPort: 5173,
    },
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
  preview: {
    host: '0.0.0.0',
    port: 4173,
    allowedHosts: true,
  },
});

