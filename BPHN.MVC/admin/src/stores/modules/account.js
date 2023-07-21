import AccountAPI from "@/apis/AccountAPI";
import i18n from "@/i18n/index.js";
import router from "@/routers/index.js";

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
    }
};

const mutations = {
    setContext: (state, payload) => {
        state.context = payload;
    }
};

const actions = {
    login: ({ commit }, account) => {
        AccountAPI.login(account).then((res) => {
            if (res?.data?.success) {
                let user = res.data.data;
                if (user) {
                    commit("setContext", user);
                    router.push("calendar");
                }
            }
            else {
                let msg = res?.data?.message;
                alert(msg ?? i18n.global.t("ErrorMesg"));
            }
        })
            .catch((error) => {
                console.log(error);
                alert(i18n.global.t("ErrorMesg"));
            })
    },

    forgot: ((commit, email) => {
        AccountAPI.forgot(email).then((res) => {
            if (res?.data?.success) {
                alert(i18n.global.t("ResetPasswordMesg"));
                router.push("login");
            }
            else {
                let msg = res?.data?.message;
                alert(msg ?? i18n.global.t("ErrorMesg"));
            }
        })
            .catch((error) => {
                console.log(error);
                alert(i18n.global.t("ErrorMesg"));
            })
    }),

    setPassword: ((commit, data) => {
        AccountAPI.setPassword(data).then((res) => {
            if (res?.data?.success) {
                alert(i18n.global.t("SaveSuccess"));
                router.push("login");
            }
            else {
                let msg = res?.data?.message
                alert(msg ?? i18n.global.t("ErrorMesg"))
            }
        })
            .catch((error) => {
                console.log(error);
                alert(i18n.global.t("ErrorMesg"));
            })
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
    })
};

export default {
    namespaced: true,
    state,
    getters,
    actions,
    mutations
};