<script setup>
import { onMounted, ref, watchEffect } from "vue";
import { useStore } from "vuex";
import { useI18n } from "vue-i18n";
import { SwitchButton, Refresh, Avatar, Bell, Expand } from "@element-plus/icons-vue";
import useToggleModal from "@/register-components/actionDialog";
import { useRouter } from "vue-router";
import NotificationCard from "@/components/NotificationCard.vue";
import connection from "@/ws";
import { MaxNotification } from "@/const";

const router = useRouter();
const store = useStore();
const { t } = useI18n();
const { openModal, hasRole } = useToggleModal();

const lstNotification = ref([]);
const hasNewNoti = ref(false);
const isMobile = ref(store.getters["config/isMobile"]);
const fullname = ref(store.getters["account/getFullName"]);
const drawer = ref(store.getters["account/getDrawer"]);

watchEffect(() => { lstNotification.value = store.getters["cache/getHeaderVariableCache"]?.lstNotification ?? []; })

onMounted(() => {
  connection.on("PushNotification", function (type, model) {
    hasNewNoti.value = true;
    if (lstNotification.value.length > MaxNotification) {
      lstNotification.value = [];
    }
    if (model) {
      lstNotification.value.push(JSON.parse(model));
      store.commit("cache/setHeaderVariableCache", {
        lstNotification: lstNotification.value
      });
    }
  });
});

const showAccountInfo = () => {
  openModal("AccountInfoDialog");
};

const logout = () => {
  localStorage.clear();
  router.go();
};

const goToHome = () => {
  router.push("/");
};

const refresh = () => {
  localStorage.removeItem("config-key");
  store.dispatch("account/refresh").then((res) => {
    console.log(res);
    router.go();
  })
};

const markRead = () => {
  hasNewNoti.value = false;
};

const toggle = () => {
  store.commit("account/setDrawer", !drawer.value);
};
</script>

<template>
  <section class="h-100 d-flex flex-row align-items-center justify-content-between">
    <div class="pointer d-flex flex-row align-items-center justify-content-between">
      <el-button @click="toggle" class="mr-2" circle :icon="Expand" size="large"></el-button>
      <div @click="goToHome" class="d-flex flex-row align-items-center">
        <img height="50" class="mt-3" src="../assets/images/logo.png"/>
        <h4 class="fs-2 m-0 mt-1">BPHN</h4>
      </div>
    </div>
    <div class="d-flex flex-row align-items-center">
      <p v-if="!isMobile">{{ t("Hello") }}
        <span class="pointer fw-bold mx-1 text-decoration-underline" @click="showAccountInfo">{{ fullname }}</span>
      </p>
      <div class="mx-1 pointer" @click="showAccountInfo">
        <el-icon size="24"><Avatar /></el-icon>
      </div>
      <div class="mx-1 pointer">
        <el-popover trigger="click" :width="350">
          <template #reference>
            <el-badge is-dot :hidden="!hasNewNoti">
              <el-icon size="24" @click="markRead"><Bell /></el-icon>
            </el-badge>
          </template>
          <notification-card v-for="item in lstNotification" :key="item" :data="item" ></notification-card>
          <el-empty v-if="lstNotification.length == 0" :description="t('NoData')" />
        </el-popover>
      </div>
      <div v-if="false" class="mx-1 pointer" @click="refresh">
        <el-icon size="24"><Refresh /></el-icon>
      </div>
      <el-button class="mx-1 pointer" type="info" :icon="SwitchButton" circle @click="logout"></el-button>
    </div>
  </section>
  <AccountInfoDialog v-if="hasRole('AccountInfoDialog')"> </AccountInfoDialog>
</template>
