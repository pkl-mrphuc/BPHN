import { createRouter, createWebHistory } from 'vue-router'
import Layout from '@/layouts/BPHNLayout.vue'
import HelloWorld from '@/components/HelloWorld.vue'
import LoginPage from '@/pages/BPHNLogin.vue'
import ForgotPage from '@/pages/BPHNForgot.vue'
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
          title: i18n.global.t('CalendarTitle')
        }
      },
      {
        path: '/bm',
        component: HelloWorld,
        meta: {
          title: i18n.global.t('BMTitle')
        }
      },
      {
        path: '/my-grounds',
        component: HelloWorld,
        meta: {
          title: i18n.global.t('MyGroundsTitle')
        }
      },
      {
        path: '/configuartions',
        component: HelloWorld,
        meta: {
          title: i18n.global.t('ConfiguarationsTitle')
        }
      }
    ]
  },
  {
    path: '/login',
    name: 'login',
    component: LoginPage,
    meta: {
      title: i18n.global.t('LoginForm')
    }
  },
  {
    path: '/forgot',
    name: 'forgot',
    component: ForgotPage,
    meta: {
      title: i18n.global.t('ForgotPasswordTitle')
    }
  },
]

const router = createRouter({
  history: createWebHistory(),
  routes
})

router.beforeEach(async (to) => {

  const publicPages = ['/login', '/forgot']

  const authRequired = !publicPages.includes(to.path)
  const authKey = JSON.parse(localStorage.getItem('admin-auth-key'))
  if (authRequired && !authKey?.account?.context) {
      return '/login'
  }

});

router.afterEach((to) => {
  document.title = to.meta.title ? `[${to.meta.title}] | ${i18n.global.t('BPHNHaNoi')}` : `${i18n.global.t('BPHNHaNoi')}`
})

export default router