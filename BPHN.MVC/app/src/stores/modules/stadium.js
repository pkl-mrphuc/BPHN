import StadiumAPI from "@/apis/StadiumAPI"

const state = {

}

const getters = {

}

const mutations = {

}

const actions = {
    fetchStadium: ((commit, key) => {
        return StadiumAPI.fetchStadium(key)
    })
}

export default {
    namespaced: true,
    state,
    getters,
    mutations,
    actions
}