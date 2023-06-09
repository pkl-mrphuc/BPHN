<script setup>
</script>


<template>
  <div class="common-layout">
    <el-container>
      <el-header>
        <bphn-header></bphn-header>
      </el-header>
      <el-container>
        <el-aside width="200px" class="menu-layout">
          <bphn-menu></bphn-menu>
        </el-aside>
        <el-main class="main-layout">
          <router-view />
        </el-main>
      </el-container>
    </el-container>
  </div>
</template>

<script>
import BphnHeader from "@/layouts/BPHNHeader.vue";
import BphnMenu from "@/layouts/BPHNMenu.vue";
import AccountAPI from "@/apis/AccountAPI";
import { useStore } from "vuex";
import { useI18n } from "vue-i18n";
export default {
  name: "BPHNLayout",
  components: { BphnHeader, BphnMenu },
  async created() {
    const store = useStore();
    const t = useI18n();

    let validateResult = await AccountAPI.validateToken(
      store.getters["account/getToken"]
    );
    if (validateResult?.data?.success) {
      let configs = localStorage.getItem("config-key");
      let loadedConfig = store.getters["config/getLoadedConfig"];
      if (!configs || (!loadedConfig && configs))
        store.dispatch("config/loadConfig").then(() => {
          let lang = store.getters["config/getLanguage"];
          let darkMode = store.getters["config/getDarkMode"];
          t.locale.value = lang;
          if (darkMode) document.documentElement.setAttribute("class", "dark");
          else document.documentElement.removeAttribute("class");
        });
      else {
        let lang = store.getters["config/getLanguage"];
        let darkMode = store.getters["config/getDarkMode"];
        t.locale.value = lang;
        if (darkMode) document.documentElement.setAttribute("class", "dark");
        else document.documentElement.removeAttribute("class");
      }
    } else {
      window.location = "/login";
    }
  },
};
</script>