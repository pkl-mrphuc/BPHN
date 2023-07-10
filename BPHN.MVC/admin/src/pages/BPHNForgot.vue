<script setup>
import { ref } from "vue";
import { useStore } from "vuex";
import { useI18n } from "vue-i18n";
import { useRouter } from "vue-router";

const router = useRouter();
const { t } = useI18n();
const store = useStore();
const email = ref("");

const goToLogin = () => {
  router.push("login");
};

const forgot = () => {
  if (!email.value) {
    alert(t("EmailEmptyMesg"));
    return;
  }
  store.dispatch("account/forgot", email.value);
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
        <h2>{{ t("ForgotPasswordTitle") }}</h2>
        <el-form class="box_right__form">
          <el-form-item>
            <el-input v-model="email" maxlength="255" placeholder="Email" />
          </el-form-item>
          <el-form-item>
            <el-button style="width: 100%" type="primary" @click="forgot()">{{
              t("SendRequest")
            }}</el-button>
          </el-form-item>
        </el-form>
        <a
          class="back-login-btn"
          @click="goToLogin()"
          href="javascript:void(0)"
          >{{ t("BackToLogin") }}</a
        >
      </div>
    </div>
  </section>
</template>

<style scoped>
@import "@/assets/css/page.css";
</style>