import HistoryLogAPI from "@/apis/HistoryLogAPI";

const state = {

};

const getters = {

};

const mutations = {

};

const actions = {
    getPaging: ((commit, data) => {
        return HistoryLogAPI.getPaging(data);
    }),

    getCountPaging: ((commit, data) => {
        return HistoryLogAPI.getCountPaging(data);
    }),
};

export default {
    namespaced: true,
    state, 
    getters,
    mutations,
    actions
}