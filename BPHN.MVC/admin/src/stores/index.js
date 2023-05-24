import { createStore } from 'vuex'
import account from '@/stores/modules/account'

export default createStore({
    modules: {
      account
    }
  })