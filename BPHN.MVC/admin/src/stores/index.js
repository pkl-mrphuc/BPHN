import { createStore } from 'vuex'
import createPersistedState from 'vuex-persistedstate'
import account from '@/stores/modules/account'
import config from '@/stores/modules/config'
import pitch from '@/stores/modules/pitch'

export default createStore({
    modules: {
      account,
      config,
      pitch
    },
    plugins: [
      createPersistedState({
        key: 'admin-auth-key',
        paths: ['account']
      }),
      createPersistedState(
        {
          key: 'config-key',
          paths: ['config']
        }
      )
    ]
  })