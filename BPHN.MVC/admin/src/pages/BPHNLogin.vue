<script setup>
import { ref, onMounted } from "vue";
import { useStore } from "vuex";
import { useI18n } from "vue-i18n";
import { useRouter } from "vue-router";
import { ElNotification } from "element-plus";

const { t } = useI18n();
const store = useStore();
const router = useRouter();
const username = ref("");
const password = ref("");
const inpUsername = ref(null);

onMounted(() => {
  inpUsername.value.focus();
});

const login = () => {
  if (!username.value) {
    ElNotification({
      title: t("Notification"),
      message: t("UsernameEmptyMesg"),
      type: "warning",
    });
    return;
  }
  if (!password.value) {
    ElNotification({
      title: t("Notification"),
      message: t("PasswordEmptyMesg"),
      type: "warning",
    });
    return;
  }

  store
    .dispatch("account/login", {
      username: username.value,
      password: password.value,
    })
    .then((res) => {
      if (res?.data?.success) {
        let user = res.data.data;
        if (user) {
          store.commit("account/setContext", user);
          router.push("calendar");
        }
      } else {
        ElNotification({
          title: t("Notification"),
          message: res?.data?.message ?? t("ErrorMesg"),
          type: "error",
        });
      }
    });
};

const goToForgot = () => {
  router.push("forgot");
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
        <h2 class="text-center">{{ t("LoginForm") }}</h2>
        <div class="w-100 p-3">
          <el-form class="box_right__form">
            <el-form-item>
              <el-input
                v-model="username"
                maxlength="255"
                :placeholder="t('Username')"
                tabindex="1"
                ref="inpUsername"
                @keyup.enter="login"
              />
            </el-form-item>
            <el-form-item>
              <el-input
                v-model="password"
                maxlength="500"
                show-password
                :placeholder="t('Password')"
                type="password"
                tabindex="2"
                @keyup.enter="login"
              />
            </el-form-item>
            <el-form-item>
              <el-button
                type="primary"
                @click="login"
                tabindex="3"
                >{{ t("Submit") }}</el-button
              >
              <el-button type="info" link @click="goToForgot()" tabindex="4">{{
                t("ForgotPasswordTitle")
              }}</el-button>
            </el-form-item>
          </el-form>
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped>
@import "@/assets/css/page.css";
</style>