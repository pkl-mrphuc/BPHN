<script setup>
import { ref } from "vue";
import { useStore } from "vuex";
import { useI18n } from "vue-i18n";
import { useRouter } from "vue-router";

const { t } = useI18n();
const store = useStore();
const router = useRouter();
const username = ref("");
const password = ref("");
const isLoading = ref(false);

const login = () => {
  if (!username.value) {
    alert(t("UsernameEmptyMesg"));
    return;
  }
  if (!password.value) {
    alert(t("PasswordEmptyMesg"));
    return;
  }

  isLoading.value = true;
  store.dispatch("account/login", {
    username: username.value,
    password: password.value,
  }).then(() => {
    isLoading.value = false;
  });
};

const goToForgot = () => {
  router.push("forgot");
};
</script>

<template>
  <section class="pbhn-login">
    <div class="box">
      <div class="box_left">
        <h1 class="box_left__title">{{ t("BookingPitchHaNoi") }}</h1>
        <img class="img img-anime" src=".././assets/images/fb-anime.jpg" />
      </div>

      <div class="box_right">
        <h2>{{ t("LoginForm") }}</h2>
        <el-form class="box_right__form">
          <el-form-item>
            <el-input
              v-model="username"
              maxlength="255"
              :placeholder="t('Username')"
            />
          </el-form-item>
          <el-form-item>
            <el-input
              v-model="password"
              maxlength="500"
              show-password
              :placeholder="t('Password')"
              type="password"
            />
          </el-form-item>
          <el-form-item>
            <el-button type="primary" :loading="isLoading" @click="login">{{
              t("Submit")
            }}</el-button>
            <a
              class="forgot-btn"
              href="javascript:void(0)"
              @click="goToForgot()"
              >{{ t("ForgotPasswordTitle") }}</a
            >
          </el-form-item>
        </el-form>
      </div>
    </div>
  </section>
</template>

<style scoped>
@import "@/assets/css/BPHNLogin.css";
</style>