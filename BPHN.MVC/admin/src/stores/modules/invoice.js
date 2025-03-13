import InvoiceAPI from "@/apis/InvoiceAPI";

const state = {

};

const getters = {

};

const mutations = {

};

const actions = {
    getAll: ((commit, data) => {
        return InvoiceAPI.getAll(data);
    }),

    getInstance: ((commit, id) => {
        return InvoiceAPI.getInstance(id);
    }),

    insert: ((commit, data) => {
        return InvoiceAPI.insert(data);
    }),

    update: ((commit, data) => {
        return InvoiceAPI.update(data);
    })
};

export default {
    namespaced: true,
    state, 
    getters,
    mutations,
    actions
}