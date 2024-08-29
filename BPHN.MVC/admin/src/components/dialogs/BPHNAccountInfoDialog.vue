<script setup>
import { useI18n } from "vue-i18n";
import useToggleModal from "@/register-components/actionDialog";
import { computed, ref, inject } from "vue";
import { useStore } from "vuex";
import { useRouter } from "vue-router";
import { GenderEnum } from "@/const";
import { ElLoading, ElNotification } from "element-plus";

const running = ref(0);
const loadingOptions = inject("loadingOptions");
const router = useRouter();
const store = useStore();
const { t } = useI18n();
const { toggleModel } = useToggleModal();
const showSetPassword = ref(false);
const password = ref("");
const passwordAgain = ref("");
const hdfFile = ref(null);
const imgAvatar = ref(null);

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

const avatarUrl = computed(() => {
  return store.getters["account/getAvatarUrl"];
});

const changePassword = () => {
  if (running.value > 0) return;
  ++running.value;
  const loading = ElLoading.service(loadingOptions);

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
    id: store.getters["account/getAccountId"],
    password: password.value,
  };
  store.dispatch("account/changePassword", data).then((res) => {
    loading.close();
    if (res?.data?.success) {
      ElNotification({
        title: t("Notification"),
        message: t("SaveSuccess"),
        type: "success",
      });
      router.push("login");
    } else {
      ElNotification({
        title: t("Notification"),
        message: res?.data?.message ?? t("ErrorMesg"),
        type: "error",
      });
    }

    setTimeout(() => {
      running.value = 0;
    }, 1000);
  });
};

const upload = () => {
  hdfFile.value.click();
};

const changeHdfFile = (event) => {
  let files = event.target.files;
  if (files?.length > 0) {
    readImageFile(files[0]);
    store.dispatch("file/upload", {
      file: files[0],
      id: store.getters["account/getAccountId"],
    }).then((res) => {
      if (res?.data?.success) {
        fetch(res.data.data).then(() => {
          store.commit("account/setAvatarUrl", res.data.data);
        });
      } else {
        ElNotification({
          title: t("Notification"),
          message: res?.data?.message ?? t("ErrorMesg"),
          type: "error",
        });
      }
    });
  }
};

const readImageFile = (file) => {
  let reader = new FileReader();
  reader.onload = (e) => {
    imgAvatar.value.src = e.target.result;
  };
  reader.readAsDataURL(file);
};
</script>


<template>
  <Dialog :title="t('AccountInfoForm')">
    <template #body>
      <div class="container">
        <div class="row">
          <div class="d-flex flex-row justify-content-center mb-3 col-12 col-sm-12 col-md-3 pointer" @click="upload">
            <img v-if="avatarUrl" ref="imgAvatar" :src="avatarUrl" height="120" width="120" class="image" />
            <img v-else height="120" width="120" ref="imgAvatar" src="../../assets/images/avatar-default.png"
              class="image" />

            <!-- hdf = hidden field -->
            <input type="file" hidden accept="image/*" @change="changeHdfFile" name="hdfFile" ref="hdfFile" />
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
                  <el-checkbox v-model="showSetPassword" :label="t('SetPasswordTitle')" />
                </div>
              </div>
            </div>
            <div class="row mb-3" v-if="showSetPassword">
              <div class="col-6 fw-bold d-flex flex-row align-items-center justify-content-end">
                <div class="mx-3">
                  {{ t("Password") }}
                </div>
              </div>
              <div class="col-6">
                <div class="mx-3">
                  <el-input type="password" v-model="password" :placeholder="t('Password')" />
                </div>
              </div>
            </div>
            <div class="row mb-3" v-if="showSetPassword">
              <div class="col-6 fw-bold d-flex flex-row align-items-center justify-content-end">
                <div class="mx-3">
                  {{ t("PasswordAgain") }}
                </div>
              </div>
              <div class="col-6">
                <div class="mx-3">
                  <el-input type="password" v-model="passwordAgain" :placeholder="t('PasswordAgain')" />
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </template>
    <template #foot>
      <el-button type="primary" @click="changePassword" v-if="showSetPassword">{{ t("Submit") }}</el-button>
      <el-button @click="toggleModel">{{ t("Close") }}</el-button>
    </template>
  </Dialog>
</template>
<style scoped>
.image {
  display: block;
  object-fit: cover;
  border-radius: 50%;
}
</style>