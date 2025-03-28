const state = {
    drawer: false
};

const getters = {
    getDrawer: (state) => {
        return state.drawer;
    }
};

const mutations = {
    setDrawer: (state, payload) => {
        state.drawer = payload;
    }
};

const actions = {
    
};

export default {
    namespaced: true,
    state,
    getters,
    actions,
    mutations
};