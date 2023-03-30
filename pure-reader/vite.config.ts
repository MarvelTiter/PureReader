import { defineConfig } from 'vite'
import { internalIpV4 } from 'internal-ip'
import vue from '@vitejs/plugin-vue'

// https://vitejs.dev/config/
export default defineConfig(async () => {
  const host = await internalIpV4()
  const config = {
    plugins: [vue()],
    server: {
      host: '0.0.0.0', // listen on all addresses
      port: 8080,
      strictPort: true,
      hmr: {
        protocol: 'ws',
        host,
        port: 5183,
      },
      envPrefix: ['VITE_', 'TAURI_'],
      build: {
        // Tauri supports es2021
        target: ['es2021', 'chrome100', 'safari13'],
        // don't minify for debug builds
        minify: !process.env.TAURI_DEBUG ? 'esbuild' : false,
        // produce sourcemaps for debug builds
        sourcemap: !!process.env.TAURI_DEBUG,
      },
    },
  }
  return config
})
