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
    }),

    getPaging: ((commit, data) => {
        return BookingAPI.getPaging(data);
    }),

    getCountPaging: ((commit, data) => {
        return BookingAPI.getCountPaging(data);
    }),

    findBlank: ((commit, data) => {
        return BookingAPI.findBlank(data);
    }),

    approval: ((commit, data) => {
        return BookingAPI.approval(data);
    }),

    decline: ((commit, data) => {
        return BookingAPI.decline(data);
    })
};

export default {
    namespaced: true,
    state,
    getters,
    mutations,
    actions
};