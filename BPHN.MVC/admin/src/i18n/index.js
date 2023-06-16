import { createI18n } from "vue-i18n"
import vn from "@/i18n/vn/index.js"
import en from "@/i18n/en/index.js"

const i18n = createI18n({
    legacy: false,
    locale: "en",
    messages: {
        en: en,
        vn: vn
    }
})

export default i18n;