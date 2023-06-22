<script setup>
import useToggleModal from "@/register-components/actionDialog";
import { useI18n } from "vue-i18n";
import { ref, inject, defineEmits, defineProps } from "vue";
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

  const loading = ElLoading.service(loadingOptions);
  let actionPath = "account/register";
  if(props.mode == "edit") actionPath = "account/update";
  store
    .dispatch(actionPath, {
      id: props.data?.id,
      userName: userName.value,
      fullName: fullName.value,
      email: email.value,
      phoneNumber: phoneNumber.value,
      gender: gender.value,
      status: status.value
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
      <div class="d-flex">
        <div class="w30">
          <el-avatar
            :size="120"
            src="https://cube.elemecdn.com/3/7c/3ea6beec64369c2642b92c6726f1epng.png"
          />
        </div>
        <div class="w70">
          <el-form-item>
            <el-col :span="7">
              <b>{{ t("Status") }}</b>
            </el-col>
            <el-col :span="17">
              <el-select v-model="status" style="width: 100%">
                <el-option :label="t('Active')" value="ACTIVE" />
                <el-option :label="t('Inactive')" value="INACTIVE" />
              </el-select>
            </el-col>
          </el-form-item>
          <el-form-item>
            <el-col :span="7">
              <b>{{ t("Username") }} <span class="red">(*)</span></b>
            </el-col>
            <el-col :span="17">
              <el-input v-model="userName" maxlength="255" />
            </el-col>
          </el-form-item>
          <el-form-item>
            <el-col :span="7">
              <b>{{ t("FullName") }}<span class="red">(*)</span></b>
            </el-col>
            <el-col :span="17">
              <el-input v-model="fullName" maxlength="255" />
            </el-col>
          </el-form-item>
          <el-form-item>
            <el-col :span="7">
              <b>{{ t("Gender") }}</b>
            </el-col>
            <el-col :span="17">
              <el-select v-model="gender" style="width: 100%">
                <el-option value="MALE" :label="t('Male')" />
                <el-option value="FEMALE" :label="t('Female')" />
                <el-option value="OTHER" :label="t('Other')" />
              </el-select>
            </el-col>
          </el-form-item>
          <el-form-item>
            <el-col :span="7">
              <b>{{ t("PhoneNumber") }}<span class="red">(*)</span></b>
            </el-col>
            <el-col :span="17">
              <el-input v-model="phoneNumber" maxlength="255" />
            </el-col>
          </el-form-item>
          <el-form-item>
            <el-col :span="7">
              <b>{{ t("Email") }}<span class="red">(*)</span></b>
            </el-col>
            <el-col :span="17">
              <el-input v-model="email" maxlength="255" />
            </el-col>
          </el-form-item>
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

<style scoped>
.d-flex {
  display: flex;
}

.flex-column {
  flex-direction: column;
}

.align-items-center {
  align-items: center;
}

.w30 {
  width: 30%;
}

.w70 {
  width: 70%;
}

.red {
  color: #f56c6c;
}
</style>