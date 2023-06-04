import FileAPI from "@/apis/FileAPI";

const state = {

};

const getters = {

};

const mutations = {

};

const actions = {
    upload: ((commit, data) => {
        return FileAPI.upload(data);
    })
};

export default {
    namespaced: true,
    state, 
    getters,
    mutations,
    actions
};