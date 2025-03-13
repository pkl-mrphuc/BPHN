<script setup>
import { ref } from "vue";
const drawer = ref(true);
</script>

<template>
  <div class="common-layout">
    <el-container>
      <el-header>
        <bphn-header></bphn-header>
      </el-header>
      <el-container>
        <el-drawer v-model="drawer" direction="ltr" :withHeader="false">
          <bphn-menu></bphn-menu>
        </el-drawer>
        <el-main class="main-layout">
          <router-view />
        </el-main>
      </el-container>
    </el-container>
    <div style="position: absolute;left: 0;bottom: 0;z-index: 100;">
      <div style="width: 20px;height: 20px;background-color: red;" @click="drawer = true"></div>
    </div>
  </div>
</template>

<script>
import BphnHeader from "@/layouts/BPHNHeader.vue";
import BphnMenu from "@/layouts/BPHNMenu.vue";
import AccountAPI from "@/apis/AccountAPI";
import { useStore } from "vuex";
import { useI18n } from "vue-i18n";
import { useRouter } from "vue-router";
import connection from "@/ws.js";

export default {
  name: "BPHNLayout",
  components: { BphnHeader, BphnMenu },
  async created() {
    const store = useStore();
    const t = useI18n();
    const router = useRouter();

    let validateResult = await AccountAPI.validateToken(
      store.getters["account/getToken"]
    );
    if (validateResult?.data?.success) {
      connection.start().then(() => {
        connection
          .invoke("AddConnection", store.getters["account/getAccountId"])
          .catch(function (err) {
            console.error(err.toString());
          });
      });

      let configs = localStorage.getItem("config-key");
      let loadedConfig = store.getters["config/getLoadedConfig"];
      if (!configs || (!loadedConfig && configs))
        store.dispatch("config/loadConfig");
      let lang = store.getters["config/getLanguage"];
      let darkMode = store.getters["config/getDarkMode"];
      t.locale.value = lang;
      if (darkMode) document.documentElement.setAttribute("class", "dark");
        else document.documentElement.removeAttribute("class");

    } else {
      router.push("login");
    }
  },
};
</script>

<style scoped>
.common-layout .el-header {
  position: relative;
  background-color: var(--el-color-primary-light-7);
  color: var(--el-text-color-primary);
}

.common-layout :deep(.el-drawer) {
  color: var(--el-text-color-primary);
  background: var(--el-color-primary-light-8);
  width: 64px !important;
  height: calc(100vh - 60px - 1px);
  left: 0;
  bottom: 0;
  top: unset;
  padding: 0;
}
.common-layout :deep(.el-drawer__body) {
  padding: 0;
}
</style>