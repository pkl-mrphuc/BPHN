import OverviewAPI from "@/apis/OverviewAPI";

const state = {

};

const getters = {

};

const mutations = {

};

const actions = {
    get: ((commit, data) => {
        return OverviewAPI.get(data);
    }),
};

export default {
    namespaced: true,
    state,
    getters,
    actions,
    mutations
};
