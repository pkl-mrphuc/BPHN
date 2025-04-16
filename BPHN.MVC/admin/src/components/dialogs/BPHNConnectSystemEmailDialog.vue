<script setup>
import { ConfigKeyEnum } from "@/const";
import useToggleModal from "@/register-components/actionDialog";
import { useI18n } from "vue-i18n";
import { useStore } from "vuex";
import { ElNotification } from "element-plus";
import {
  ref,
  defineProps,
} from "vue";

const { toggleModel } = useToggleModal();
const { t } = useI18n();
const store = useStore();
const props = defineProps({
  data: Object
});

const running = ref(0);
const email = ref(props.data ?? "");
const password = ref("");

const connect = () => {
  if (running.value > 0) return;
  ++running.value;
  setTimeout(() => {
    running.value = 0;
  }, 1000);

  if (!email.value) {
    ElNotification({ title: t("Notification"), message: t("EmailEmptyMesg"), type: "warning", });
    return;
  }

  if (!password.value) {
    ElNotification({ title: t("Notification"), message: t("PasswordEmptyMesg"), type: "warning", });
    return;
  }

  let configs = [
    {
      Key: ConfigKeyEnum.SYSTEMEMAIL,
      Value: `${email.value}`,
    },
    {
      Key: ConfigKeyEnum.SECRETEMAIL,
      Value: `${password.value}`,
    }
  ];
  store.dispatch("config/save", configs);
};
</script>

<template>
  <Dialog :title="t('ConnectForm')" :className="'w-xl-25'">
    <template #body>
      <div class="container">
        <div class="row">
          <div class="col-12 col-sm-12 col-md-12">
              <div class="row d-flex flex-row align-items-center">
                  <div class="mb-2 col-12 col-sm-12 col-md-4 fw-bold">{{ t("Email") }}</div>
                  <div class="col-12 col-sm-12 col-md-8">
                    <el-input class="mb-2" type="text" v-model="email" disabled />
                  </div>
              </div>
              <div class="row d-flex flex-row align-items-center">
                  <div class="mb-2 col-12 col-sm-12 col-md-4 fw-bold">{{ t("Password") }}</div>
                  <div class="col-12 col-sm-12 col-md-8">
                    <el-input class="mb-2" type="password" v-model="password" />
                  </div>
              </div>
          </div>
        </div>
      </div>
    </template>
    <template #foot>
      <div class="d-flex flex-row-reverse">
        <el-button type="primary" @click="connect" class="ml-2">{{ t("Connect") }}</el-button>
        <el-button @click="toggleModel">{{ t("Close") }}</el-button>
      </div>
    </template>
  </Dialog>
</template>
