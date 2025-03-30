import { createStore } from "vuex";
import stadium from "@/stores/modules/stadium";
import config from "@/stores/modules/config";
import bookingDetail from "@/stores/modules/bookingDetail";
import booking from "@/stores/modules/booking";
import account from "@/stores/modules/account";
import createPersistedState from "vuex-persistedstate";

export default createStore({
  modules: {
    stadium,
    config,
    bookingDetail,
    booking,
    account
  },
  plugins: [
    createPersistedState(
      {
        key: "config-key",
        paths: ["config"]
      }
    )
  ]
});