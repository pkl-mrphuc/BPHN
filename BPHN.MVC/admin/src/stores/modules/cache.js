const state = {
    overviewVariable: null,
    bmVariable: null,
    calendarVariable: null,
    myFootbalFieldVariable: null,
    invoiceVariable: null,
    serviceVariable: null,
    configVariable: null,
    historyLogVariable: null,
};

const getters = {
    getOverviewVariableCache: (state) => {
        state.overviewVariable = JSON.parse(localStorage.getItem("overviewVariable"));
        return state.overviewVariable;
    },
 
    getBmVariableCache: (state) => {
        state.bmVariable = JSON.parse(localStorage.getItem("bmVariable"));
        return state.bmVariable;
    },

    getCalendarVariableCache: (state) => {
        state.calendarVariable = JSON.parse(localStorage.getItem("calendarVariable"));
        return state.calendarVariable;
    },

    getMyFootballFieldVariableCache: (state) => {
        state.myFootbalFieldVariable = JSON.parse(localStorage.getItem("myFootbalFieldVariable"));
        return state.myFootbalFieldVariable;
    },

    getInvoiceVariableCache: (state) => {
        state.invoiceVariable = JSON.parse(localStorage.getItem("invoiceVariable"));
        return state.invoiceVariable;
    },
 
    getServiceVariableCache: (state) => {
        state.serviceVariable = JSON.parse(localStorage.getItem("serviceVariable"));
        return state.serviceVariable;
    },

    getConfigVariableCache: (state) => {
        state.configVariable = JSON.parse(localStorage.getItem("configVariable"));
        return state.configVariable;
    },

    getHistoryLogVariableCache: (state) => {
        state.historyLogVariable = JSON.parse(localStorage.getItem("historyLogVariable"));
        return state.historyLogVariable;
    },
};

const mutations = {
    setOverviewVariableCache: (state, payload) => {
        state.overviewVariable = payload;
        localStorage.setItem("overviewVariable", JSON.stringify(payload));
    },
 
    setBmVariableCache: (state, payload) => {
        state.bmVariable = payload;
        localStorage.setItem("bmVariable", JSON.stringify(payload));
    },

    setCalendarVariableCache: (state, payload) => {
        state.calendarVariable = payload;
        localStorage.setItem("calendarVariable", JSON.stringify(payload));
    },

    setMyFootballFieldVariableCache: (state, payload) => {
        state.myFootbalFieldVariable = payload;
        localStorage.setItem("myFootbalFieldVariable", JSON.stringify(payload));
    },

    setInvoiceVariableCache: (state, payload) => {
        state.invoiceVariable = payload;
        localStorage.setItem("invoiceVariable", JSON.stringify(payload));
    },
 
    setServiceVariableCache: (state, payload) => {
        state.serviceVariable = payload;
        localStorage.setItem("serviceVariable", JSON.stringify(payload));
    },

    setConfigVariableCache: (state, payload) => {
        state.configVariable = payload;
        localStorage.setItem("configVariable", JSON.stringify(payload));
    },

    setHistoryLogVariableCache: (state, payload) => {
        state.historyLogVariable = payload;
        localStorage.setItem("historyLogVariable", JSON.stringify(payload));
    },
};

const actions = {
    
};

export default {
    namespaced: true,
    state, 
    getters,
    mutations,
    actions
}