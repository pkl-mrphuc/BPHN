const state = {
    language: process.env.VUE_APP_I18N_LOCALE,
    darkMode: true
}

const getters = {
    getLanguage: (state) => {
        return state.language
    },

    getDarkMode: (state) => {
        return state.darkMode
    }
}

const mutations = {
    setLanguage: (state, payload) => {
        state.language = payload;
    },

    setDarkMode: (state, payload) => {
        state.darkMode = payload;
    }
}

const actions = {

}

export default {
    namespaced: true,
    state,
    getters,
    mutations,
    actions
}