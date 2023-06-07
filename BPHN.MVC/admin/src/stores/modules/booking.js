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

    insert: ((commit, data) => {
        return BookingAPI.insert(data);
    }),

    checkTimeFrame: ((commit, data) => {
        return BookingAPI.checkTimeFrame(data);
    })
};

export default {
    namespaced: true,
    state,
    getters,
    mutations, 
    actions
};