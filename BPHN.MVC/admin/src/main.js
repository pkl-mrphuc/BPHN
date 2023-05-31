import { createApp } from 'vue'
import App from './App.vue'
import ElementPlus from 'element-plus'
import router from '@/routers/index.js'
import store from '@/stores/index.js'
import i18n from '@/i18n/index.js'
import 'element-plus/dist/index.css'
import 'element-plus/theme-chalk/dark/css-vars.css'
import '@/assets/css/index.css'
import { jwtInterceptor } from '@/interceptors.js'

jwtInterceptor()

createApp(App)
.use(ElementPlus)
.use(i18n)
.use(router)
.use(store)
.mount('#app')
