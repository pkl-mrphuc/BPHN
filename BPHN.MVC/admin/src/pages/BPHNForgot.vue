<script setup>
import { ref, onMounted } from "vue";
import { useStore } from "vuex";
import { useI18n } from "vue-i18n";
import { useRouter } from "vue-router";

const router = useRouter();
const { t } = useI18n();
const store = useStore();
const email = ref("");
const inpEmail = ref(null);

onMounted(() => {
  inpEmail.value.focus();
});

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
            <el-input
              v-model="email"
              maxlength="255"
              placeholder="Email"
              ref="inpEmail"
              tabindex="1"
            />
          </el-form-item>
          <el-form-item>
            <el-button
              class="w-100"
              type="primary"
              @keyup.enter="forgot"
              @click="forgot"
              tabindex="2"
              >{{ t("SendRequest") }}</el-button
            >
          </el-form-item>
        </el-form>
        <a
          class="back-login-btn"
          @click="goToLogin"
          tabindex="3"
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