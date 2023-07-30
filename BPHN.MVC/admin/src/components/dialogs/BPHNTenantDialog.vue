<script setup>
import useToggleModal from "@/register-components/actionDialog";
import { useI18n } from "vue-i18n";
import {
  ref,
  inject,
  defineEmits,
  defineProps,
  computed,
  onMounted,
  nextTick,
} from "vue";
import { useStore } from "vuex";
import { ElLoading, ElNotification } from "element-plus";
import { GenderEnum, StatusEnum } from "@/const";
import useCommonFn from "@/commonFn";

const props = defineProps({
  data: Object,
  mode: String,
});
const { toggleModel } = useToggleModal();
const { t } = useI18n();
const store = useStore();
const { equals, isEmail } = useCommonFn();
const emits = defineEmits(["callback"]);
const fullName = ref(props.data?.fullName);
const email = ref(props.data?.email);
const phoneNumber = ref(props.data?.phoneNumber);
const gender = ref(props.data?.gender ?? GenderEnum.MALE);
const status = ref(props.data?.status ?? StatusEnum.ACTIVE);
const loadingOptions = inject("loadingOptions");
const inpEmail = ref(null);
const inpFullName = ref(null);
const running = ref(0);
const isDisabled = computed(() => {
  return equals(props.mode, "edit");
});

onMounted(() => {
  nextTick(() => {
    if (equals(props.mode, "edit")) {
      inpFullName.value.focus();
    } else {
      inpEmail.value.focus();
    }
  });
});

const save = () => {
  if (running.value > 0) return;
  ++running.value;

  if (!email.value) {
    ElNotification({
      title: t("Notification"),
      message: t("EmailEmptyMesg"),
      type: "warning",
    });
    return;
  }
  if (!fullName.value) {
    ElNotification({
      title: t("Notification"),
      message: t("FullNameEmptyMesg"),
      type: "warning",
    });
    return;
  }
  if (!phoneNumber.value) {
    ElNotification({
      title: t("Notification"),
      message: t("PhoneNumberEmptyMesg"),
      type: "warning",
    });
    return;
  }
  if (!isEmail(email.value)) {
    ElNotification({
      title: t("Notification"),
      message: t("InvalidEmail"),
      type: "warning",
    });
    return;
  }

  let actionPath = "account/register";
  if (props.mode == "edit") {
    actionPath = "account/update";
    ElNotification({
      title: t("Notification"),
      message: t("FeatureIsDeveloping"),
      type: "info",
    });
    return;
  }

  const loading = ElLoading.service(loadingOptions);
  store
    .dispatch(actionPath, {
      id: props.data?.id,
      userName: email.value,
      fullName: fullName.value,
      email: email.value,
      phoneNumber: phoneNumber.value,
      gender: gender.value,
      status: status.value,
    })
    .then((res) => {
      if (res?.data?.success) {
        emits("callback");
        toggleModel();
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
      loading.close();

      setTimeout(() => {
        running.value = 0;
      }, 1000);
    });
};
</script>

<template>
  <Dialog :title="t('TenantForm')">
    <template #body>
      <div class="container">
        <div class="row">
          <div
            class="d-flex flex-row justify-content-center mb-3 col-12 col-sm-12 col-md-4"
          >
            <el-avatar
              :size="120"
              src="https://cube.elemecdn.com/3/7c/3ea6beec64369c2642b92c6726f1epng.png"
            />
          </div>
          <div class="col-12 col-sm-12 col-md-8">
            <div class="row">
              <div class="mb-2 col-12 col-sm-12 col-md-4 fw-bold">
                {{ t("Status") }}
              </div>
              <div class="col-12 col-sm-12 col-md-8">
                <el-select v-model="status" class="w-100 mb-2">
                  <el-option :label="t('Active')" :value="StatusEnum.ACTIVE" />
                  <el-option
                    :label="t('Inactive')"
                    :value="StatusEnum.INACTIVE"
                  />
                </el-select>
              </div>
            </div>
            <div class="row">
              <div class="mb-2 col-12 col-sm-12 col-md-4 fw-bold">
                {{ t("Email") }}<span class="text-danger">(*)</span>
              </div>
              <div class="mb-2 col-12 col-sm-12 col-md-8">
                <el-input
                  v-model="email"
                  :disabled="isDisabled"
                  maxlength="255"
                  ref="inpEmail"
                />
              </div>
            </div>
            <div class="row">
              <div class="mb-2 col-12 col-sm-12 col-md-4 fw-bold">
                {{ t("FullName") }}<span class="text-danger">(*)</span>
              </div>
              <div class="mb-2 col-12 col-sm-12 col-md-8">
                <el-input
                  v-model="fullName"
                  maxlength="255"
                  ref="inpFullName"
                />
              </div>
            </div>
            <div class="row">
              <div class="mb-2 col-12 col-sm-12 col-md-4 fw-bold">
                {{ t("Gender") }}
              </div>
              <div class="mb-2 col-12 col-sm-12 col-md-8">
                <el-select v-model="gender" class="w-100">
                  <el-option :value="GenderEnum.MALE" :label="t('Male')" />
                  <el-option :value="GenderEnum.FEMALE" :label="t('Female')" />
                  <el-option :value="GenderEnum.OTHER" :label="t('Other')" />
                </el-select>
              </div>
            </div>
            <div class="row">
              <div class="mb-2 col-12 col-sm-12 col-md-4 fw-bold">
                {{ t("PhoneNumber") }}<span class="text-danger">(*)</span>
              </div>
              <div class="mb-2 col-12 col-sm-12 col-md-8">
                <el-input v-model="phoneNumber" maxlength="255" />
              </div>
            </div>
          </div>
        </div>
      </div>
    </template>
    <template #foot>
      <div class="d-flex flex-row-reverse">
        <el-button type="primary" @click="save" class="ml-2">{{
          t("Save")
        }}</el-button>
        <el-button @click="toggleModel">{{ t("Close") }}</el-button>
      </div>
    </template>
  </Dialog>
</template>
