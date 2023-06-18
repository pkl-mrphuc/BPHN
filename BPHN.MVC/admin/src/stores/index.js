import { createStore } from "vuex";
import createPersistedState from "vuex-persistedstate";
import account from "@/stores/modules/account";
import config from "@/stores/modules/config";
import pitch from "@/stores/modules/pitch";
import file from "@/stores/modules/file";
import booking from "@/stores/modules/booking";
import bookingDetail from "@/stores/modules/bookingDetail";
import historyLog from "@/stores/modules/historyLog";

export default createStore({
  modules: {
    account,
    config,
    pitch,
    file,
    booking,
    bookingDetail,
    historyLog
  },
  plugins: [
    createPersistedState({
      key: "admin-auth-key",
      paths: ["account"]
    }),
    createPersistedState(
      {
        key: "config-key",
        paths: ["config"]
      }
    )
  ]
});