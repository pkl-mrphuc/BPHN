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
        return PitchAPI.insert(data)
    }),

    getPaging: ((commit, accountId) => {
        return PitchAPI.getPaging(accountId)
    }),

    update: ((commit, data) => {
        return PitchAPI.update(data)
    })
}

export default {
    namespaced: true,
    state, 
    getters,
    mutations,
    actions
}