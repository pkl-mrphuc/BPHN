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
const fullname = ref(store.getters["account/getFullName"]);
const drawer = ref(store.getters["account/getDrawer"]);

watchEffect(() => { lstNotification.value = store.getters["cache/getHeaderVariableCache"]?.lstNotification ?? []; })
watchEffect(() => { drawer.value = store.getters["account/getDrawer"]; })

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
  console.log(drawer.value);
  if (drawer.value) {
    store.commit("account/setDrawer", false);
  }
  else {
    store.commit("account/setDrawer", true);
  }
};
</script>

<template>
  <section class="header">
    <div class="header__left">
      <el-button @click="toggle" class="header__toggle-btn" circle :icon="Expand" size="large"></el-button>
      <div @click="goToHome" class="header__logo pointer">
        <img height="50" class="header__logo-img" src="../assets/images/logo.png"/>
        <h4 class="header__logo-title">BPHN</h4>
      </div>
    </div>
    <div class="header__right">
      <p class="header__greeting">{{ t("Hello") }}
        <span class="header__fullname pointer fw-bold mx-1 text-decoration-underline" @click="showAccountInfo">{{ fullname }}</span>
      </p>
      <div class="header__avatar pointer" @click="showAccountInfo">
        <el-icon size="24"><Avatar /></el-icon>
      </div>
      <div class="header__notification pointer">
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
      <div v-if="false" class="header__refresh pointer" @click="refresh">
        <el-icon size="24"><Refresh /></el-icon>
      </div>
      <el-button class="header__logout-btn pointer" type="info" :icon="SwitchButton" circle @click="logout"></el-button>
    </div>
  </section>
  <AccountInfoDialog v-if="hasRole('AccountInfoDialog')"> </AccountInfoDialog>
</template>

<style scoped>
.header {
  height: 100%;
  display: flex;
  flex-direction: row;
  align-items: center;
  justify-content: space-between;
  background: #252728;
}
.header__left {
  display: flex;
  flex-direction: row;
  align-items: center;
  justify-content: space-between;
}
.header__toggle-btn {
  margin-right: 0.5rem;
}
.header__logo {
  display: flex;
  flex-direction: row;
  align-items: center;
  cursor: pointer;
}
.header__logo-img {
  height: 50px;
  margin-top: 1rem;
}
.header__logo-title {
  font-size: 2rem;
  margin: 0;
  margin-top: 0.25rem;
}
.header__right {
  display: flex;
  flex-direction: row;
  align-items: center;
}
.header__greeting {
  margin: 0;
}
.header__fullname {
  font-weight: bold;
  margin: 0 0.25rem;
  text-decoration: underline;
}
.header__avatar,
.header__notification,
.header__refresh {
  margin: 0 0.25rem;
  cursor: pointer;
}
.header__logout-btn {
  margin-left: 0.25rem;
  cursor: pointer;
}
</style>
