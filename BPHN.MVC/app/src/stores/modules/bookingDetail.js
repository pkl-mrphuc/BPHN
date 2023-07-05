import BookingDetailAPI from "@/apis/BookingDetailAPI"

const state = {

}

const getters = {

}

const mutations = {

}

const actions = {
    getByDate: ((commit, data) => {
        return BookingDetailAPI.getByDate(data);
    })
}

export default {
    namespaced: true,
    state,
    getters,
    mutations,
    actions
}