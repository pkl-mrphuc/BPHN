import AccountAPI from '@/apis/AccountAPI'

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
                    window.location = '/'
                }
            } 
            else {
                let msg = res?.data?.message
                alert(msg??'Đã có lỗi xảy ra. Vui lòng liên hệ đến nhà cung cấp')
            }
        })
    },

    forgot:((commit, email) => {
        AccountAPI.forgot(email).then((res) => {
            if(res?.data?.success) {
                alert('Vui lòng kiểm tra email để lấy lại mật khẩu')
            }
            else {
                let msg = res?.data?.message
                alert(msg??'Đã có lỗi xảy ra. Vui lòng liên hệ đến nhà cung cấp')
            }
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