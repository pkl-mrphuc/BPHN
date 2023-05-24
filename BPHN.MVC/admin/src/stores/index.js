import { createStore } from 'vuex'
import createPersistedState from 'vuex-persistedstate'
import account from '@/stores/modules/account'

export default createStore({
    modules: {
      account
    },
    plugins: [
      createPersistedState({
        key: 'admin-auth-key',
        paths: ['account']
      })
    ]
  })