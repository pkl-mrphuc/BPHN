import StadiumAPI from "@/apis/StadiumAPI"

const state = {

}

const getters = {

}

const mutations = {

}

const actions = {
    getPaging: ((commit, data) => {
        return StadiumAPI.getPaging(data)
    }),

    getCountPaging: ((commit, data) => {
        return StadiumAPI.getCountPaging(data)
    })
}

export default {
    namespaced: true,
    state,
    getters,
    mutations,
    actions
}