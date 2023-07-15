import { createApp } from "vue";
import App from "./App.vue";
import ElementPlus from "element-plus";
import router from "@/routers/index.js";
import store from "@/stores/index.js";
import i18n from "@/i18n/index.js";
import "@/assets/css/index.css";
import "element-plus/theme-chalk/dark/css-vars.css";
import "element-plus/dist/index.css";
import { jwtInterceptor } from "@/interceptors.js";
import { startWs } from "@/ws.js";

// dialogs
import Dialog from "@/components/dialogs/BPHNDialog.vue";
import FootballFieldDialog from "@/components/dialogs/BPHNFootballFieldDialog.vue";
import BookingDialog from "@/components/dialogs/BPHNBookingDialog.vue";
import FindBlankDialog from "@/components/dialogs/BPHNFindBlankDialog.vue";
import AccountInfoDialog from "@/components/dialogs/BPHNAccountInfoDialog.vue";
import TenantDialog from "@/components/dialogs/BPHNTenantDialog.vue";
import MatchInfoDialog from "@/components/dialogs/BPHNMatchInfoDialog.vue";
import PermissionDialog from "@/components/dialogs/BPHNPermissionDialog.vue";

jwtInterceptor();
startWs();

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
    .component("MatchInfoDialog", MatchInfoDialog)
    .component("PermissionDialog", PermissionDialog)
    .mount("#app");
