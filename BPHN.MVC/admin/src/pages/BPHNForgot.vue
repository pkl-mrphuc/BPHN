<script setup>
import { ref, onMounted } from "vue";
import { useStore } from "vuex";
import { useI18n } from "vue-i18n";
import { useRouter } from "vue-router";
import { ElNotification } from "element-plus";

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
    ElNotification({ title: t("Notification"), message: t("EmailEmptyMesg"), type: "warning", });
    return;
  }
  store.dispatch("account/forgot", email.value).then((res) => {
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
  <div class="forgot-page">
    <div class="forgot-page__container">
      <div class="forgot-page__form-wrapper">
        <h1 class="forgot-page__title">{{ t("ForgotPasswordTitle") }}</h1>
        <div class="forgot-page__form-box">
          <el-form class="forgot-page__form">
            <el-form-item>
              <el-input
                v-model="email"
                maxlength="255"
                :placeholder="t('Email')"
                ref="inpEmail"
                tabindex="1"
              />
            </el-form-item>
            <el-form-item>
              <el-button
                class="forgot-page__submit-btn"
                type="primary"
                @click="forgot"
                tabindex="2"
              >{{ t("SendRequest") }}</el-button>
            </el-form-item>
          </el-form>
          <a
            v-if="false"
            class="forgot-page__back-btn"
            @click="goToLogin"
            tabindex="3"
            href="javascript:void(0)"
          >{{ t("BackToLogin") }}</a>
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped>
.forgot-page {
  width: 100vw;
  height: 100vh;
  display: flex;
  align-items: center;
  justify-content: center;
  padding: 1rem;
  background: #f0f2f5;
}
.forgot-page__container {
  width: 75%;
  display: flex;
  justify-content: center;
  align-items: center;
}
.forgot-page__form-wrapper {
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
.forgot-page__title {
  text-align: center;
  text-transform: uppercase;
  margin: 0;
  padding-top: 2rem;
}
.forgot-page__form-box {
  width: 100%;
  padding: 2rem 1.5rem;
}
.forgot-page__form {
  width: 100%;
}
:deep(.el-form-item__content) {
  justify-content: space-between;
}
.forgot-page__submit-btn, .forgot-page__submit-btn:hover {
  width: 100%;
  background: #093D67;
  border: 0;
  font-weight: 700;
}
.forgot-page__submit-btn:hover {
  background: #0d5086;
}
.forgot-page__back-btn {
  width: 100%;
  display: inline-block;
  text-align: center;
  margin-top: 1rem;
  color: #409EFF;
  cursor: pointer;
  text-decoration: underline;
}
</style>