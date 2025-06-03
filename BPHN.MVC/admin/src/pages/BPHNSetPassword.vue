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

const submit = () => {
  if (!password.value || !passwordAgain.value) {
    ElNotification({ title: t("Notification"), message: t("PasswordEmptyMesg"), type: "warning", });
    return;
  }
  if (password.value != passwordAgain.value) {
    ElNotification({ title: t("Notification"), message: t("NoMatchPasswordMesg"), type: "warning", });
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
      ElNotification({ title: t("Notification"), message: t("SaveSuccess"), type: "success", });
    } else {
      ElNotification({ title: t("Notification"), message: res?.data?.message ?? t("ErrorMesg"), type: "error", });
    }
  });
};
</script>

<template>
  <div class="set-password-page">
    <div class="set-password-page__container">
      <div class="set-password-page__form-wrapper">
        <h1 class="set-password-page__title">{{ t("SetPasswordTitle") }}</h1>
        <h3 class="set-password-page__username">
          <i>{{ userName }}</i>
        </h3>
        <div class="set-password-page__form-box">
          <el-form class="set-password-page__form">
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
                class="set-password-page__submit-btn"
                type="primary"
                @click="submit"
                tabindex="3"
              >{{ t("SendRequest") }}</el-button>
            </el-form-item>
          </el-form>
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped>
.set-password-page {
  width: 100vw;
  height: 100vh;
  display: flex;
  align-items: center;
  justify-content: center;
  padding: 1rem;
  background: #f0f2f5;
}
.set-password-page__container {
  width: 75%;
  display: flex;
  justify-content: center;
  align-items: center;
}
.set-password-page__form-wrapper {
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
.set-password-page__title {
  text-align: center;
  text-transform: uppercase;
  margin: 0;
  padding-top: 2rem;
}
.set-password-page__username {
  text-align: center;
  margin: 1rem 0 0 0;
  color: #666;
  font-weight: 400;
}
.set-password-page__form-box {
  width: 100%;
  padding: 2rem 1.5rem;
}
.set-password-page__form {
  width: 100%;
}
:deep(.el-form-item__content) {
  justify-content: space-between;
}
.set-password-page__submit-btn, .set-password-page__submit-btn:hover {
  width: 100%;
  background: #093D67;
  border: 0;
  font-weight: 700;
}
.set-password-page__submit-btn:hover {
  background: #0d5086;
}
.set-password-page__back-btn {
  width: 100%;
  display: inline-block;
  text-align: center;
  margin-top: 1rem;
  color: #409EFF;
  cursor: pointer;
  text-decoration: underline;
}
</style>