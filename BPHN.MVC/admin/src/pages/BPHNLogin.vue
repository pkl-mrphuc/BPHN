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
    ElNotification({ title: t("Notification"), message: t("UsernameEmptyMesg"), type: "warning", });
    return;
  }
  if (!password.value) {
    ElNotification({ title: t("Notification"), message: t("PasswordEmptyMesg"), type: "warning", });
    return;
  }

  store
    .dispatch("account/login", 
    {
      username: username.value,
      password: password.value,
    })
    .then((res) => {
      if (res?.data?.success) {
        let user = res.data.data;
        if (user) {
          store.commit("account/setContext", user);
          router.push("/");
        }
      } else {
        ElNotification({ title: t("Notification"), message: res?.data?.message ?? t("ErrorMesg"), type: "error", });
      }
    });
};

const goToForgot = () => {
  router.push("forgot");
};
</script>

<template>
  <div class="login-page">
    <div class="login-page__container">
      <div class="login-page__form-wrapper">
        <h1 class="login-page__title">{{ t("BookingPitchHaNoi") }}</h1>
        <div class="login-page__form-box">
          <el-form class="login-page__form">
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
                class="login-page__submit-btn"
                type="primary"
                @click="login"
                tabindex="3"
              >{{ t("Submit") }}</el-button>
            </el-form-item>
            <el-form-item>
              <el-button
                class="login-page__forgot-btn"
                type="info"
                link
                @click="goToForgot()"
                tabindex="4"
              >{{ t("ForgotPasswordTitle") }}</el-button>
            </el-form-item>
          </el-form>
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped>
.login-page {
  width: 100vw;
  height: 100vh;
  display: flex;
  align-items: center;
  justify-content: center;
  padding: 1rem;
  background: #f0f2f5;
}
.login-page__container {
  width: 75%;
  display: flex;
  justify-content: center;
  align-items: center;
}
.login-page__form-wrapper {
  width: 100%;
  max-width: 450px;
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  background: #fff;
  box-shadow: 0 2px 4px #0000001a,0 8px 16px #0000001a;
  border-radius: 8px;
}
.login-page__title {
  text-align: center;
  text-transform: uppercase;
  margin: 0;
  padding-top: 2rem;
}
.login-page__form-box {
  width: 100%;
  padding: 2rem 1.5rem;
}
.login-page__form {
  width: 100%;
}
:deep(.el-form-item__content) {
  justify-content: space-between;
}
.login-page__submit-btn, .login-page__submit-btn:hover {
  width: 100%;
  background: #093D67;
  border: 0;
  font-weight: 700;
}
.login-page__submit-btn:hover {
  background: #0d5086;
}
.login-page__forgot-btn {
  width: 100%;
}
</style>