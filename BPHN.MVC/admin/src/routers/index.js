import { createRouter, createWebHistory } from 'vue-router'
import Layout from '@/layouts/BPHNLayout.vue'
import HelloWorld from '@/components/HelloWorld.vue'
import LoginPage from '@/pages/BPHNLogin.vue'
import ForgotPage from '@/pages/BPHNForgot.vue'
import ResetPasswordPage from '@/pages/BPHNResetPassword.vue'
import Configurations from '@/components/BPHNConfigurations.vue'
import i18n from '@/i18n/index.js'

const routes = [
  {
    path: '/',
    component: Layout,
    children: [
      {
        path: '/calendar',
        component: HelloWorld,
        meta: {
          title: 'CalendarTitle'
        }
      },
      {
        path: '/bm',
        component: HelloWorld,
        meta: {
          title: 'BMTitle'
        }
      },
      {
        path: '/my-grounds',
        component: HelloWorld,
        meta: {
          title: 'MyGroundsTitle'
        }
      },
      {
        path: '/configuartions',
        component: Configurations,
        meta: {
          title: 'ConfigurationsTitle'
        }
      }
    ]
  },
  {
    path: '/login',
    name: 'login',
    component: LoginPage,
    meta: {
      title: 'LoginForm'
    }
  },
  {
    path: '/forgot',
    name: 'forgot',
    component: ForgotPage,
    meta: {
      title: 'ForgotPasswordTitle'
    }
  },
  {
    path: '/reset-password',
    name: 'reset-password',
    component: ResetPasswordPage,
    meta: {
      title: 'ResetPasswordTitle'
    }
  },
]

const router = createRouter({
  history: createWebHistory(),
  routes
})

router.beforeEach(async (to) => {

  const publicPages = ['/login', '/forgot', '/reset-password']

  const authRequired = !publicPages.includes(to.path)

  if(!authRequired) {
    localStorage.clear()
  }

  const authKey = JSON.parse(localStorage.getItem('admin-auth-key'))
  if (authRequired && !authKey?.account?.context?.token) {
    return '/login'
  }

  document.title = to.meta.title ? `[${i18n.global.t(to.meta.title)}] | ${i18n.global.t('BPHNHaNoi')}` : `${i18n.global.t('BPHNHaNoi')}`
})

export default router