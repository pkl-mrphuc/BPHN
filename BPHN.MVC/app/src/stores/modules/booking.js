import BookingAPI from "@/apis/BookingAPI";

const state = {

};

const getters = {

};

const mutations = {

};

const actions = {
    getInstance: ((commit, id) => {
        return BookingAPI.getInstance(id);
    }),

    insertBookingRequest: ((commit, data) => {
        return BookingAPI.insertBookingRequest(data);
    })
};

export default {
    namespaced: true,
    state,
    getters,
    mutations,
    actions
};