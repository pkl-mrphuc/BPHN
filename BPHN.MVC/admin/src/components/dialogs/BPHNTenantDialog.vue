<script setup>
import useToggleModal from "@/register-components/actionDialog";
import { useI18n } from "vue-i18n";
import { ref, inject, defineEmits, defineProps, computed } from "vue";
import { useStore } from "vuex";
import { ElLoading } from "element-plus";

const props = defineProps({
  data: Object,
  mode: String,
});
const { toggleModel } = useToggleModal();
const { t } = useI18n();
const store = useStore();
const emits = defineEmits(["callback"]);
const fullName = ref(props.data?.fullName ?? "");
const userName = ref(props.data?.userName ?? "");
const email = ref(props.data?.email ?? "");
const phoneNumber = ref(props.data?.phoneNumber ?? "");
const gender = ref(props.data?.gender ?? "MALE");
const status = ref(props.data?.status ?? "ACTIVE");
const loadingOptions = inject("loadingOptions");
const isDisabled = computed(() => {
  return props.mode == "edit" ? true : false;
});

const save = () => {
  if (!userName.value) {
    alert(t("UsernameEmptyMesg"));
    return;
  }
  if (!fullName.value) {
    alert(t("FullNameEmptyMesg"));
    return;
  }
  if (!phoneNumber.value) {
    alert(t("PhoneNumberEmptyMesg"));
    return;
  }
  if (!email.value) {
    alert(t("EmailEmptyMesg"));
    return;
  }

  let actionPath = "account/register";
  if (props.mode == "edit") {
    actionPath = "account/update";
    alert(t("FeatureIsDeveloping"));
    return;
  }

  const loading = ElLoading.service(loadingOptions);
  store
    .dispatch(actionPath, {
      id: props.data?.id,
      userName: userName.value,
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
      } else {
        let msg = res?.data?.message;
        alert(msg ?? t("ErrorMesg"));
      }
      loading.close();
    });
};
</script>

<template>
  <Dialog :title="t('TenantForm')">
    <template #body>
      <div class="container">
        <div class="row">
          <div class="col-4">
            <el-avatar
              :size="120"
              src="https://cube.elemecdn.com/3/7c/3ea6beec64369c2642b92c6726f1epng.png"
            />
          </div>
          <div class="col-8">
            <el-form-item>
              <el-col :span="7" class="fw-bold">
                {{ t("Status") }}
              </el-col>
              <el-col :span="17">
                <el-select v-model="status" class="w-100">
                  <el-option :label="t('Active')" value="ACTIVE" />
                  <el-option :label="t('Inactive')" value="INACTIVE" />
                </el-select>
              </el-col>
            </el-form-item>
            <el-form-item>
              <el-col :span="7" class="fw-bold">
                {{ t("Username") }} <span class="text-danger">(*)</span>
              </el-col>
              <el-col :span="17">
                <el-input
                  v-model="userName"
                  :disabled="isDisabled"
                  maxlength="255"
                />
              </el-col>
            </el-form-item>
            <el-form-item>
              <el-col :span="7" class="fw-bold">
                {{ t("FullName") }}<span class="text-danger">(*)</span>
              </el-col>
              <el-col :span="17">
                <el-input v-model="fullName" maxlength="255" />
              </el-col>
            </el-form-item>
            <el-form-item>
              <el-col :span="7" class="fw-bold">
                {{ t("Gender") }}
              </el-col>
              <el-col :span="17">
                <el-select v-model="gender" class="w-100">
                  <el-option value="MALE" :label="t('Male')" />
                  <el-option value="FEMALE" :label="t('Female')" />
                  <el-option value="OTHER" :label="t('Other')" />
                </el-select>
              </el-col>
            </el-form-item>
            <el-form-item>
              <el-col :span="7" class="fw-bold">
                {{ t("PhoneNumber") }}<span class="text-danger">(*)</span>
              </el-col>
              <el-col :span="17">
                <el-input v-model="phoneNumber" maxlength="255" />
              </el-col>
            </el-form-item>
            <el-form-item>
              <el-col :span="7" class="fw-bold">
                {{ t("Email") }}<span class="text-danger">(*)</span>
              </el-col>
              <el-col :span="17">
                <el-input
                  v-model="email"
                  :disabled="isDisabled"
                  maxlength="255"
                />
              </el-col>
            </el-form-item>
          </div>
        </div>
      </div>
    </template>
    <template #foot>
      <span class="dialog-footer">
        <el-button @click="toggleModel">{{ t("Close") }}</el-button>
        <el-button type="primary" @click="save">{{ t("Save") }}</el-button>
      </span>
    </template>
  </Dialog>
</template>
