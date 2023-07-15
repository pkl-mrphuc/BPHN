import PermissionAPI from "@/apis/PermissionAPI";

const state = {

};

const getters = {

};

const mutations = {

};

const actions = {
    get: ((commit, accountId) => {
        return PermissionAPI.getPermissions(accountId);
    }),

    save: ((commit, data) => {
        return PermissionAPI.save(data)
    })
};

export default {
    namespaced: true,
    state,
    getters,
    actions,
    mutations
};
