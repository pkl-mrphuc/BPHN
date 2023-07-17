import NotificationAPI from "@/apis/NotificationAPI";

const state = {

};

const getters = {

};

const mutations = {

};

const actions = {
    get: ((commit, data) => {
        return NotificationAPI.getNotifications(data);
    })
};

export default {
    namespaced: true,
    state,
    getters,
    actions,
    mutations
};
