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
    })
};

export default {
    namespaced: true,
    state,
    getters,
    actions,
    mutations
};