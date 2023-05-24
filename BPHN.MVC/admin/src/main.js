import { createApp } from 'vue'
import App from './App.vue'
import ElementPlus from 'element-plus'
import router from '@/routers/index.js'
import store from '@/stores/index.js'
import 'element-plus/dist/index.css'
import '@/assets/css/index.css'

createApp(App)
.use(ElementPlus)
.use(router)
.use(store)
.mount('#app')
