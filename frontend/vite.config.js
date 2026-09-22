import react from '@vitejs/plugin-react'
import { defineConfig } from 'vite'

export default defineConfig({
  plugins: [react()],
  server: {
    proxy: {
      target: process.env.services__api__http__0 ?? 'http://localhost:7071',
    },
  },
})
