<script setup>
import { computed } from "vue";
import { useStore } from "vuex";
import { useI18n } from "vue-i18n";
import { SwitchButton, Refresh, Avatar } from "@element-plus/icons-vue";
import useToggleModal from "@/register-components/actionDialog";
import { useRouter } from "vue-router";

const router = useRouter();
const store = useStore();
const { t } = useI18n();
const { openModal, hasRole } = useToggleModal();

const fullname = computed(() => {
  return store.getters["account/getFullName"];
});

const showAccountInfo = () => {
  openModal("AccountInfoDialog");
};

const logout = () => {
  localStorage.clear();
  router.go();
};

const goToHome = () => {
  router.push("calendar");
};

const refresh = () => {
  localStorage.removeItem("config-key");
  router.go();
};
</script>

<template>
  <section
    class="h-100 d-flex flex-row align-items-center justify-content-between"
  >
    <h1 class="fs-1 m-0 pointer text-decoration-underline" @click="goToHome">
      BPHN
    </h1>
    <div class="d-flex flex-row align-items-center">
      <p class="account pointer">
        {{ t("Hello") }}
        <span class="mx-1 text-decoration-underline" @click="showAccountInfo">{{
          fullname
        }}</span>
      </p>
      <div class="mx-1 pointer account-sm" @click="showAccountInfo">
        <el-icon size="24"><Avatar /></el-icon>
      </div>
      <div class="mx-1 pointer" @click="refresh">
        <el-icon size="24"><Refresh /></el-icon>
      </div>
      <div class="mx-1 pointer" @click="logout">
        <el-icon size="24"><SwitchButton /></el-icon>
      </div>
    </div>
  </section>
  <AccountInfoDialog v-if="hasRole('AccountInfoDialog')"> </AccountInfoDialog>
</template>

<style scoped>
.account {
  display: none;
}

.account-sm {
  display: block;
}

@media (min-width: 576px) {
  .account-sm {
    display: none;
  }

  .account {
    display: block;
  }
}
</style>