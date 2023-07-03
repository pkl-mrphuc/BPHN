import BookingDetailAPI from "@/apis/BookingDetailAPI";

const state = {

};

const getters = {

};

const mutations = {

};

const actions = {
    cancel: (( commit, id ) => {
        return BookingDetailAPI.cancel(id);
    }),

    getByDate: ((commit, date) => {
        return BookingDetailAPI.getByDate(date);
    }),

    updateMatch: ((commit, data) => {
        return BookingDetailAPI.updateMatch(data);
    })
};

export default {
    namespaced: true,
    state,
    getters,
    actions,
    mutations
};