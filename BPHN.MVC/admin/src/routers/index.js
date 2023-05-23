import { createRouter, createWebHistory } from 'vue-router'
import Layout from '@/layouts/BPHNLayout.vue'
import HelloWorld from '@/components/HelloWorld.vue'
import LoginPage from '@/pages/BPHNLogin.vue'

const routes = [
  {
    path: '/',
    component: Layout,
    children: [
      {
        path: '/calendar',
        component: HelloWorld,
        meta: {
          title: 'Calendar'
        }
      },
      {
        path: '/bm',
        component: HelloWorld,
        meta: {
          title: 'Booking manager'
        }
      },
      {
        path: '/my-grounds',
        component: HelloWorld,
        meta: {
          title: 'My grounds'
        }
      },
      {
        path: '/configuartions',
        component: HelloWorld,
        meta: {
          title: 'Configuarations'
        }
      }
    ]
  },
  {
    path: '/login',
    name: 'login',
    component: LoginPage,
    meta: {
      title: 'Login'
    }
  },
]

const router = createRouter({
  history: createWebHistory(),
  routes
})

router.beforeEach(async (to) => {

  const publicPages = ['/login'];

  const authRequired = !publicPages.includes(to.path);
  if (authRequired) {
      return '/login';
  }

});

router.afterEach((to) => {
  document.title = to.meta.title ? `[${to.meta.title}] | BPHN Hà Nội` : 'BPHN Hà Nội'
})

export default router