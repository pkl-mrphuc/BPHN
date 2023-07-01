<script setup>
import { useI18n } from "vue-i18n";
import useToggleModal from "@/register-components/actionDialog";
import { computed, ref } from "vue";
import { useStore } from "vuex";
import { useRouter } from "vue-router";

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

const changePassword = () => {
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
    if (res?.data?.success) {
      alert(t("SaveSuccess"));
      router.push("login");
    } else {
      let msg = res?.data?.message;
      alert(msg ?? t("ErrorMesg"));
    }
  });
};
</script>


<template>
  <Dialog :title="t('AccountInfoForm')">
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
              <b>{{ t("Username") }}:</b>
            </el-col>
            <el-col :span="17"> {{ userName }} </el-col>
          </el-form-item>
          <el-form-item>
            <el-col :span="7">
              <b>{{ t("FullName") }}:</b>
            </el-col>
            <el-col :span="17"> {{ fullName }} </el-col>
          </el-form-item>
          <el-form-item>
            <el-col :span="7">
              <b>{{ t("Gender") }}:</b>
            </el-col>
            <el-col :span="17"> Nam </el-col>
          </el-form-item>
          <el-form-item>
            <el-col :span="7">
              <b>{{ t("PhoneNumber") }}:</b>
            </el-col>
            <el-col :span="17"> {{ phoneNumber }} </el-col>
          </el-form-item>
          <el-form-item>
            <el-col :span="7">
              <b>{{ t("Email") }}:</b>
            </el-col>
            <el-col :span="17"> {{ email }} </el-col>
          </el-form-item>
          <el-form-item>
            <el-col :span="7"> </el-col>
            <el-col :span="17">
              <el-checkbox
                v-model="showResetPassword"
                :label="t('ResetPasswordTitle')"
              />
            </el-col>
          </el-form-item>
          <el-form-item v-if="showResetPassword">
            <el-col :span="7">
              <b>{{ t("Password") }}</b>
            </el-col>
            <el-col :span="17">
              <el-input type="password" v-model="password" :placeholder="t('Password')" />
            </el-col>
          </el-form-item>
          <el-form-item v-if="showResetPassword">
            <el-col :span="7">
              <b>{{ t("PasswordAgain") }}</b>
            </el-col>
            <el-col :span="17">
              <el-input type="password" v-model="passwordAgain" :placeholder="t('PasswordAgain')" />
            </el-col>
          </el-form-item>
        </div>
      </div>
    </template>
    <template #foot>
      <span class="dialog-footer">
        <el-button
          type="primary"
          @click="changePassword"
          v-if="showResetPassword"
          >{{ t("Submit") }}</el-button
        >
        <el-button @click="toggleModel">{{ t("Close") }}</el-button>
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
</style>