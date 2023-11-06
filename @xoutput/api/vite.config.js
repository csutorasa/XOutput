import { resolve } from 'path';
import { defineConfig } from 'vite';
import dts from 'vite-plugin-dts';

export default defineConfig({
  build: {
    outDir: resolve(__dirname, 'lib'),
    lib: {
      entry: resolve(__dirname, 'src/index.ts'),
      name: '@xoutput/api',
      fileName: 'index',
    },
    rollupOptions: {
      output: {
        sourcemapExcludeSources: true,
      },
    },
    sourcemap: true,
    target: 'es2021',
    minify: false,
  },
  plugins: [dts({
    rollupTypes: true,
  })],
});