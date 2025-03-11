import ItemAPI from "@/apis/ItemAPI";

const state = {

};

const getters = {

};

const mutations = {

};

const actions = {
    getAll: ((commit, data) => {
        return ItemAPI.getAll(data);
    }),

    getInstance: ((commit, id) => {
        return ItemAPI.getInstance(id);
    }),

    insert: ((commit, data) => {
        return ItemAPI.insert(data);
    }),

    update: ((commit, data) => {
        return ItemAPI.update(data);
    })
};

export default {
    namespaced: true,
    state, 
    getters,
    mutations,
    actions
}