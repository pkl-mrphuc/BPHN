import ConfigAPI from "@/apis/ConfigAPI";
import i18n from "@/i18n/index.js";

const state = {
    language: process.env.VUE_APP_I18N_LOCALE,
    darkMode: true,
    loadedConfig: false,
    formatDate: process.env.VUE_APP_FORMAT_DATE
};

const getters = {
    getDarkMode: (state) => {
        return state.darkMode
    },

    getLanguage: (state) => {
        return state.language
    },

    getLoadedConfig: (state) => {
        return state.loadedConfig
    },

    getFormatDate: (state) => {
        return state.formatDate;
    }
};

const mutations = {
    setLanguage: (state, payload) => {
        state.language = payload
    },

    setDarkMode: (state, payload) => {
        state.darkMode = payload
    },

    setLoadedConfig: (state, payload) => {
        state.loadedConfig = payload
    },

    setFormatDate: (state, payload) => {
        state.formatDate = payload;
    }
};

const actions = {
    loadConfig: (({ commit }) => {
        ConfigAPI.getConfigs("").then((res) => {
            if (res?.data?.success && res?.data?.data) {
                let lstConfig = res?.data?.data
                let map = new Map()
                if (Array.isArray(lstConfig)) {
                    for (let i = 0; i < lstConfig.length; i++) {
                        const config = lstConfig[i]
                        if (!map.has(config.key)) {
                            map.set(config.key, config.value)
                        }
                    }
                }

                if (map.has("Language")) {
                    let lang = map.get("Language");
                    commit("setLanguage", lang);
                    i18n.global.locale.value = lang;
                }

                if (map.has("DarkMode")) {
                    let darkMode = map.get("DarkMode") == "true";
                    commit("setDarkMode", darkMode);
                    if (darkMode) document.documentElement.setAttribute("class", "dark");
                    else document.documentElement.removeAttribute("class");
                }

                if(map.has("FormatDate")) {
                    commit("setFormatDate", map.get("FormatDate"));
                }
                commit("setLoadedConfig", true)
            }

        })
            .catch((error) => {
                console.log(error)
                alert(i18n.global.t("ErrorMesg"))
            })
    }),

    save: ((commit, configs) => {
        ConfigAPI.save(configs).then((res) => {
            if (res?.data?.success) {
                alert(i18n.global.t("SaveSuccess"))
                localStorage.removeItem("config-key")
                window.location.reload()
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
    })

};

export default {
    namespaced: true,
    state,
    getters,
    mutations,
    actions
};