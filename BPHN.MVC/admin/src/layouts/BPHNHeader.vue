<script setup>
import { computed } from "vue";
import { useStore } from "vuex";
import { useI18n } from "vue-i18n";
import { SwitchButton } from "@element-plus/icons-vue";
import useToggleModal from "@/register-components/actionDialog";

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
  window.location = "/login";
};

const goToHome = () => {
  window.location = "/";
};
</script>

<template>
  <section class="pbhn-header">
    <div class="header_left">
      <h1 class="header_left__logo" @click="goToHome">BPHN</h1>
    </div>
    <div class="header_right">
      <p class="header_right__fullname">
        {{ t("Hello") }}
        <span class="header_right__fullname--underline" @click="showAccountInfo">{{ fullname }}</span>
      </p>
      <div class="header_right__logout" @click="logout">
        <el-icon size="24"><SwitchButton /></el-icon>
      </div>
    </div>
  </section>
  <AccountInfoDialog v-if="hasRole('AccountInfoDialog')">
  </AccountInfoDialog>
</template>

<style scoped>
@import "@/assets/css/BPHNHeader.css";
</style>