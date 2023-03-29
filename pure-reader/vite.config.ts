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
    },
  }
  return config
})
