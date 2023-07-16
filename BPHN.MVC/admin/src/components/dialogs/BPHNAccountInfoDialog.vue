<script setup>
import { useI18n } from "vue-i18n";
import useToggleModal from "@/register-components/actionDialog";
import { computed, ref, inject } from "vue";
import { useStore } from "vuex";
import { useRouter } from "vue-router";
import { GenderEnum } from "@/const";
import { ElLoading } from "element-plus";

const running = ref(0);
const loadingOptions = inject("loadingOptions");
const router = useRouter();
const store = useStore();
const { t } = useI18n();
const { toggleModel } = useToggleModal();
const showResetPassword = ref(false);
const password = ref("");
const passwordAgain = ref("");

const fullName = computed(() => {
  return store.getters["account/getFullName"];
});

const userName = computed(() => {
  return store.getters["account/getUserName"];
});

const email = computed(() => {
  return store.getters["account/getEmail"];
});

const phoneNumber = computed(() => {
  return store.getters["account/getPhoneNumber"];
});

const gender = computed(() => {
  let gender = store.getters["account/getGender"];
  switch (gender) {
    case GenderEnum.MALE:
      return t("Male");
    case GenderEnum.FEMALE:
      return t("Female");
    default:
      return t("Other");
  }
});

const changePassword = () => {
  if (running.value > 0) return;
  ++running.value;
  const loading = ElLoading.service(loadingOptions);

  if (!password.value || !passwordAgain.value) {
    alert(t("PasswordEmptyMesg"));
    return;
  }
  if (password.value != passwordAgain.value) {
    alert(t("NoMatchPasswordMesg"));
    return;
  }

  let data = {
    id: store.getters["account/getAccountId"],
    password: password.value,
  };
  store.dispatch("account/changePassword", data).then((res) => {
    loading.close();
    if (res?.data?.success) {
      alert(t("SaveSuccess"));
      router.push("login");
    } else {
      let msg = res?.data?.message;
      alert(msg ?? t("ErrorMesg"));
    }

    setTimeout(() => {
      running.value = 0;
    }, 1000);
  });
};
</script>


<template>
  <Dialog :title="t('AccountInfoForm')">
    <template #body>
      <div class="container">
        <div class="row">
          <div
            class="d-flex flex-row justify-content-center mb-3 col-12 col-sm-12 col-md-3"
          >
            <el-avatar
              :size="120"
              src="https://cube.elemecdn.com/3/7c/3ea6beec64369c2642b92c6726f1epng.png"
            />
          </div>
          <div class="col-12 col-sm-12 col-md-9">
            <div class="row mb-3">
              <div class="col-6 fw-bold d-flex flex-row justify-content-end">
                <div class="mx-3">{{ t("Username") }}</div>
              </div>
              <div class="col-6">
                <div class="mx-3">
                  {{ userName }}
                </div>
              </div>
            </div>
            <div class="row mb-3">
              <div class="col-6 fw-bold d-flex flex-row justify-content-end">
                <div class="mx-3">{{ t("FullName") }}</div>
              </div>
              <div class="col-6">
                <div class="mx-3">
                  {{ fullName }}
                </div>
              </div>
            </div>
            <div class="row mb-3">
              <div class="col-6 fw-bold d-flex flex-row justify-content-end">
                <div class="mx-3">{{ t("Gender") }}</div>
              </div>
              <div class="col-6">
                <div class="mx-3">
                  {{ gender }}
                </div>
              </div>
            </div>
            <div class="row mb-3">
              <div class="col-6 fw-bold d-flex flex-row justify-content-end">
                <div class="mx-3">{{ t("PhoneNumber") }}</div>
              </div>
              <div class="col-6">
                <div class="mx-3">
                  {{ phoneNumber }}
                </div>
              </div>
            </div>
            <div class="row mb-3">
              <div class="col-6 fw-bold d-flex flex-row justify-content-end">
                <div class="mx-3">{{ t("Email") }}</div>
              </div>
              <div class="col-6">
                <div class="mx-3">
                  {{ email }}
                </div>
              </div>
            </div>
            <div class="row mb-3">
              <div class="col-6 fw-bold"></div>
              <div class="col-6">
                <div class="mx-3">
                  <el-checkbox
                    v-model="showResetPassword"
                    :label="t('ResetPasswordTitle')"
                  />
                </div>
              </div>
            </div>
            <div class="row mb-3" v-if="showResetPassword">
              <div
                class="col-6 fw-bold d-flex flex-row align-items-center justify-content-end"
              >
                <div class="mx-3">
                  {{ t("Password") }}
                </div>
              </div>
              <div class="col-6">
                <div class="mx-3">
                  <el-input
                    type="password"
                    v-model="password"
                    :placeholder="t('Password')"
                  />
                </div>
              </div>
            </div>
            <div class="row mb-3" v-if="showResetPassword">
              <div
                class="col-6 fw-bold d-flex flex-row align-items-center justify-content-end"
              >
                <div class="mx-3">
                  {{ t("PasswordAgain") }}
                </div>
              </div>
              <div class="col-6">
                <div class="mx-3">
                  <el-input
                    type="password"
                    v-model="passwordAgain"
                    :placeholder="t('PasswordAgain')"
                  />
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </template>
    <template #foot>
      <el-button
        type="primary"
        @click="changePassword"
        v-if="showResetPassword"
        >{{ t("Submit") }}</el-button
      >
      <el-button @click="toggleModel">{{ t("Close") }}</el-button>
    </template>
  </Dialog>
</template>