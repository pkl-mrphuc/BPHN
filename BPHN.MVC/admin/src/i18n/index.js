import { createI18n } from "vue-i18n";
import vi from "@/i18n/vi/index.js";
import en from "@/i18n/en/index.js";

const i18n = createI18n({
    legacy: false,
    locale: "en",
    messages: {
        en: en,
        vi: vi
    }
});

export default i18n;