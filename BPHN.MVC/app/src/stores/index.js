import { createStore } from "vuex";
import stadium from "./modules/stadium";
import config from "./modules/config";
import bookingDetail from "./modules/bookingDetail";
import booking from "./modules/booking";
import createPersistedState from "vuex-persistedstate";

export default createStore({
  modules: {
    stadium,
    config,
    bookingDetail,
    booking
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