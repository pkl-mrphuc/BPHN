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
  <div
    style="width: 100vw; height: 100vh"
    class="container p-3 d-flex flex-row align-items-center justify-content-center"
  >
    <div class="row w-75 w-sm-100 w-md-75 w-lg-50">
      <div
        class="col-12 col-sm-12 col-md-5 col-lg-6 d-flex flex-column align-items-center justify-content-center"
      >
        <h1 class="text-center text-uppercase">{{ t("BookingPitchHaNoi") }}</h1>
        <img class="img img-anime" src=".././assets/images/fb-anime.jpg" />
      </div>

      <div
        class="col-12 col-sm-12 col-md-7 col-lg-6 d-flex flex-column align-items-center justify-content-center"
      >
        <h2>{{ t("ForgotPasswordTitle") }}</h2>
        <div class="w-100 p-3">
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
            v-if="false"
            class="back-login-btn"
            @click="goToLogin"
            tabindex="3"
            href="javascript:void(0)"
            >{{ t("BackToLogin") }}</a
          >
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped>
@import "@/assets/css/page.css";
</style>