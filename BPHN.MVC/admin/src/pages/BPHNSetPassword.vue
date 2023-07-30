<script setup>
import { computed, ref, onMounted } from "vue";
import { useStore } from "vuex";
import { useI18n } from "vue-i18n";
import { useRoute, useRouter } from "vue-router";
import { ElNotification } from "element-plus";

const { t } = useI18n();
const store = useStore();
const route = useRoute();
const router = useRouter();

const password = ref("");
const passwordAgain = ref("");
const inpPassword = ref(null);

onMounted(() => {
  inpPassword.value.focus();
});

const userName = computed(() => {
  return getQueryStringByKey("userName");
});

const getQueryStringByKey = (key) => {
  return route.query[key];
};

const goToLogin = () => {
  router.push("login");
};

const submit = () => {
  if (!password.value || !passwordAgain.value) {
    ElNotification({
      title: t("Notification"),
      message: t("PasswordEmptyMesg"),
      type: "warning",
    });
    return;
  }
  if (password.value != passwordAgain.value) {
    ElNotification({
      title: t("Notification"),
      message: t("NoMatchPasswordMesg"),
      type: "warning",
    });
    return;
  }

  let data = {
    userName: userName.value,
    code: getQueryStringByKey("code"),
    password: password.value,
  };
  store.dispatch("account/setPassword", data).then((res) => {
    if (res?.data?.success) {
      router.push("login");
      ElNotification({
        title: t("Notification"),
        message: t("SaveSuccess"),
        type: "success",
      });
    } else {
      ElNotification({
        title: t("Notification"),
        message: res?.data?.message ?? t("ErrorMesg"),
        type: "error",
      });
    }
  });
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
        <h2 style="margin-bottom: 0">{{ t("SetPasswordTitle") }}</h2>
        <h3>
          <i>{{ userName }}</i>
        </h3>
        <div class="w-100 p-3">
          <el-form class="box_right__form">
            <el-form-item>
              <el-input
                v-model="password"
                maxlength="500"
                show-password
                :placeholder="t('Password')"
                type="password"
                tabindex="1"
                ref="inpPassword"
              />
            </el-form-item>
            <el-form-item>
              <el-input
                v-model="passwordAgain"
                maxlength="500"
                show-password
                :placeholder="t('PasswordAgain')"
                type="password"
                tabindex="2"
                @keyup.enter="submit"
              />
            </el-form-item>
            <el-form-item>
              <el-button
                class="w-100"
                type="primary"
                @click="submit"
                tabindex="3"
                >{{ t("Submit") }}</el-button
              >
            </el-form-item>
          </el-form>
        </div>

        <a
          v-if="false"
          class="back-login-btn"
          @click="goToLogin"
          href="javascript:void(0)"
          tabindex="4"
          >{{ t("BackToLogin") }}</a
        >
      </div>
    </div>
  </div>
</template>

<style scoped>
@import "@/assets/css/page.css";
</style>