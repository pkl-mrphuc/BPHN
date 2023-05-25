import AccountAPI from '@/apis/AccountAPI'
import i18n from '@/i18n/index.js'

const state = {
    context: null
}

const getters = {
    getFullName: (state) => {
        if(!state.context) return ''
        return state.context.fullName
    }
}

const mutations = {
    setContext: (state, payload) => {
        state.context = payload
    }
}

const actions = {
    login:({ commit }, account) => {
        AccountAPI.login(account).then((res) => {
            if(res?.data?.success) {
                let user = res.data.data
                if(user) {
                    commit('setContext', user)
                    window.location = '/bm'
                }
            } 
            else {
                let msg = res?.data?.message
                alert(msg??i18n.global.t('ErrorMesg'))
            }
        })
        .catch((error) => {
            console.log(error)
            alert(i18n.global.t('ErrorMesg'))
        })
    },

    forgot:((commit, email) => {
        AccountAPI.forgot(email).then((res) => {
            if(res?.data?.success) {
                alert(i18n.global.t('ResetPasswordMesg'))
            }
            else {
                let msg = res?.data?.message
                alert(msg??i18n.global.t('ErrorMesg'))
            }
        })
        .catch((error) => {
            console.log(error)
            alert(i18n.global.t('ErrorMesg'))
        })
    })
}

export default {
    namespaced: true,
    state, 
    getters,
    actions,
    mutations
}