import PitchAPI from '@/apis/PitchAPI'

const state = {

}

const getters = {

}

const mutations = {

}

const actions = {
    getInstance: ((commit, id) => {
        return PitchAPI.getInstance(id)
    }),

    insert: ((commit, data) => {
        debugger; // eslint-disable-line no-debugger
        return PitchAPI.insert(data)
    })
}

export default {
    namespaced: true,
    state, 
    getters,
    mutations,
    actions
}