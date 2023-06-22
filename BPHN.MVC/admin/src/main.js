import { createApp } from "vue";
import App from "./App.vue";
import ElementPlus from "element-plus";
import router from "@/routers/index.js";
import store from "@/stores/index.js";
import i18n from "@/i18n/index.js";
import "element-plus/dist/index.css";
import "element-plus/theme-chalk/dark/css-vars.css";
import "@/assets/css/index.css";
import { jwtInterceptor } from "@/interceptors.js";

// dialogs
import Dialog from "@/components/dialogs/BPHNDialog.vue";
import FootballFieldDialog from "@/components/dialogs/BPHNFootballFieldDialog.vue";
import BookingDialog from "@/components/dialogs/BPHNBookingDialog.vue";
import FindBlankDialog from "@/components/dialogs/BPHNFindBlankDialog.vue";
import AccountInfoDialog from "@/components/dialogs/BPHNAccountInfoDialog.vue";
import TenantDialog from "@/components/dialogs/BPHNTenantDialog.vue";

jwtInterceptor();

createApp(App)
    .use(ElementPlus)
    .use(i18n)
    .use(router)
    .use(store)
    .provide("loadingOptions", {
        lock: true,
        text: "Loading",
        background: "rgba(0, 0, 0, 0.7)",
    })
    .component("Dialog", Dialog)
    .component("FootballFieldDialog", FootballFieldDialog)
    .component("BookingDialog", BookingDialog)
    .component("FindBlankDialog", FindBlankDialog)
    .component("AccountInfoDialog", AccountInfoDialog)
    .component("TenantDialog", TenantDialog)
    .mount("#app");
