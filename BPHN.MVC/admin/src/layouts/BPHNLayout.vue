<script setup>
import { computed } from "vue";

const store = useStore();
const drawer = computed(() => {
  return store.getters["account/getDrawer"];
});

const handleClose = () => {
  store.commit("account/setDrawer", false);
};
</script>

<template>
  <div class="common-layout">
    <el-container>
      <el-header>
        <bphn-header></bphn-header>
      </el-header>
      <el-container>
        <el-drawer v-model="drawer" :before-close="handleClose" direction="ltr" :withHeader="false">
          <bphn-menu></bphn-menu>
        </el-drawer>
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
  position: fixed;
  top: 0;
  left: 0;
  z-index: 2019;
  width: 100%;
}

.main-layout {
  padding-top: calc(var(--el-main-padding) + 60px);
}

.common-layout :deep(.el-drawer) {
  width: 300px !important;
}
</style>