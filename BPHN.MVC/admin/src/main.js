import { createApp } from 'vue'
import App from './App.vue'
import ElementPlus from 'element-plus'
import router from '@/routers/index.js'
import store from '@/stores/index.js'
import i18n from '@/i18n/index.js'
import 'element-plus/dist/index.css'
import '@/assets/css/index.css'

createApp(App)
.use(ElementPlus)
.use(router)
.use(i18n)
.use(store)
.mount('#app')
