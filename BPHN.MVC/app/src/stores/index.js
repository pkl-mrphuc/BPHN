import { createStore } from "vuex";
import stadium from "./modules/stadium";
import config from "./modules/config";
import createPersistedState from "vuex-persistedstate";

export default createStore({
  modules: {
    stadium,
    config
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