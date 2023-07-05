import { createStore } from "vuex";
import stadium from "./modules/stadium";
import config from "./modules/config";
import bookingDetail from "./modules/bookingDetail";
import createPersistedState from "vuex-persistedstate";

export default createStore({
  modules: {
    stadium,
    config,
    bookingDetail
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