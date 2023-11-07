import { defineConfig } from 'vite';
import { resolve, join } from 'path';
import react from '@vitejs/plugin-react';

export default defineConfig({
  root: join(__dirname, 'src'),
  build: {
    outDir: resolve(__dirname, 'webapp'),
    target: 'es2021',
    sourcemap: true,
  },
  plugins: [react()],
  server: {
    port: 8080,
    proxy: {
      '/api': 'http://localhost:8000',
      '/websocket': {
        target: 'ws://localhost:8000',
        ws: true,
      },
    },
  }
});