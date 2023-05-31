import ConfigAPI from '@/apis/ConfigAPI'
import i18n from '@/i18n/index.js'

const state = {
    language: 'vn',
    darkMode: true
}

const getters = {
    getDarkMode: (state) => {
        return state.darkMode
    },

    getLanguage: (state) => {
        return state.language
    }
}

const mutations = {
    setLanguage: (state, payload) => {
        state.language = payload
    },

    setDarkMode: (state, payload) => {
        state.darkMode = payload
    }
}

const actions = {
    loadConfig: (({ commit }) => {
        ConfigAPI.getConfigs('').then((res) => {
            if (res?.data?.success && res?.data?.data) {
                let lstConfig = res?.data?.data
                let map = new Map()
                if (Array.isArray(lstConfig)) {
                    for (let i = 0; i < lstConfig.length; i++) {
                        const config = lstConfig[i]
                        if(!map.has(config.key)) {
                            map.set(config.key, config.value)
                        }
                    }
                }

                if(map.has('Language')) {
                    commit('setLanguage', map.get('Language'))
                }

                if(map.has('DarkMode')) {
                    commit('setDarkMode', map.get('DarkMode') == "true")
                }

                window['loadedConfig'] = true
            }
        })
        .catch((error) => {
            console.log(error)
            alert(i18n.global.t('ErrorMesg'))
        })
    }),

    save: ((commit, configs) => {
        ConfigAPI.save(configs).then((res) => {
            if(res?.data?.success) {
                alert(i18n.global.t('SaveSuccess'))
                localStorage.removeItem('config-key')
                window.location.reload()
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
    mutations,
    actions
}