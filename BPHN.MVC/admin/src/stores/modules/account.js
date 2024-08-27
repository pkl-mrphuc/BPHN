import AccountAPI from "@/apis/AccountAPI";

const state = {
    context: null
};

const getters = {
    getFullName: (state) => {
        if (!state.context) return ""
        return state.context.fullName
    },

    getToken: (state) => {
        if (!state.context) return ""
        return state.context.token
    },

    getAccountId: (state) => {
        if (!state.context) return ""
        return state.context.id
    },

    getParentId: (state) => {
        if (!state.context) return ""
        return state.context.parentId
    },

    getUserName: (state) => {
        if (!state.context) return ""
        return state.context.userName;
    },

    getPhoneNumber: (state) => {
        if (!state.context) return ""
        return state.context.phoneNumber;
    },

    getEmail: (state) => {
        if (!state.context) return ""
        return state.context.email;
    },

    getRole: (state) => {
        if (!state.context) return "";
        return state.context.role;
    },

    getGender: (state) => {
        if (!state.context) return "";
        return state.context.gender;
    },

    getRelationIds: (state) => {
        if (!state.context) return "";
        return state.context.relationIds;
    },

    getAvatarUrl: (state) => {
        if (!state.context) return "";
        return state.context.avatarUrl;
    }
};

const mutations = {
    setContext: (state, payload) => {
        state.context = payload;
    },

    setAvatarUrl: (state, payload) => {
        state.context.avatarUrl = payload;
    }
};

const actions = {
    login: ((commit, account) => {
        return AccountAPI.login(account)
    }),

    forgot: ((commit, email) => {
        return AccountAPI.forgot(email)
    }),

    setPassword: ((commit, data) => {
        return AccountAPI.setPassword(data)
    }),

    changePassword: ((commit, data) => {
        return AccountAPI.changePassword(data);
    }),

    getPaging: ((commit, data) => {
        return AccountAPI.getPaging(data);
    }),

    getCountPaging: ((commit, data) => {
        return AccountAPI.getCountPaging(data);
    }),

    register: ((commit, data) => {
        return AccountAPI.register(data);
    }),

    getInstance: ((commit, id) => {
        return AccountAPI.getInstance(id);
    }),

    refresh: ((commit, id) => {
        return AccountAPI.refresh(id);
    })
};

export default {
    namespaced: true,
    state,
    getters,
    actions,
    mutations
};