import AccountAPI from "@/apis/AccountAPI";
import i18n from "@/i18n/index.js";

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
    }
};

const mutations = {
    setContext: (state, payload) => {
        state.context = payload
    }
};

const actions = {
    login: ({ commit }, account) => {
        AccountAPI.login(account).then((res) => {
            if (res?.data?.success) {
                let user = res.data.data
                if (user) {
                    commit("setContext", user)
                    window.location = "/bm"
                }
            }
            else {
                let msg = res?.data?.message
                alert(msg ?? i18n.global.t("ErrorMesg"))
            }
        })
            .catch((error) => {
                console.log(error)
                alert(i18n.global.t("ErrorMesg"))
            })
    },

    forgot: ((commit, email) => {
        AccountAPI.forgot(email).then((res) => {
            if (res?.data?.success) {
                alert(i18n.global.t("ResetPasswordMesg"))
                window.location = "/login"
            }
            else {
                let msg = res?.data?.message
                alert(msg ?? i18n.global.t("ErrorMesg"))
            }
        })
            .catch((error) => {
                console.log(error)
                alert(i18n.global.t("ErrorMesg"))
            })
    }),

    resetPassword: ((commit, data) => {
        AccountAPI.resetPassword(data).then((res) => {
            if (res?.data?.success) {
                alert(i18n.global.t("SaveSuccess"))
            }
            else {
                let msg = res?.data?.message
                alert(msg ?? i18n.global.t("ErrorMesg"))
            }
        })
            .catch((error) => {
                console.log(error)
                alert(i18n.global.t("ErrorMesg"))
            })
    }),

    changePassword: ((commit, data) => {
        return AccountAPI.changePassword(data);
    })
};

export default {
    namespaced: true,
    state,
    getters,
    actions,
    mutations
};