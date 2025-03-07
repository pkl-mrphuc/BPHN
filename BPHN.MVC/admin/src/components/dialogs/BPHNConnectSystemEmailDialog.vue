<script setup>
import { ConfigKeyEnum } from "@/const";
import useToggleModal from "@/register-components/actionDialog";
import { useI18n } from "vue-i18n";
import { useStore } from "vuex";
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

  setTimeout(() => {
    running.value = 0;
  }, 1000);
};
</script>

<template>
  <Dialog :title="t('ConnectForm')">
    <template #body>
      <div class="container">
        <div class="row">
          <div class="col-12 col-sm-12 col-md-9">
            <div class="row mb-3">
              <div class="col-6 fw-bold d-flex flex-row align-items-center justify-content-end">
                <div class="mx-3">
                  {{ t("Email") }}
                </div>
              </div>
              <div class="col-6">
                <div class="mx-3">
                  <el-input type="text" v-model="email" disabled />
                </div>
              </div>
            </div>
            <div class="row mb-3" >
              <div class="col-6 fw-bold d-flex flex-row align-items-center justify-content-end">
                <div class="mx-3">
                  {{ t("Password") }}
                </div>
              </div>
              <div class="col-6">
                <div class="mx-3">
                  <el-input type="password" v-model="password" />
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </template>
    <template #foot>
      <div class="d-flex flex-row-reverse">
        <el-button type="primary" @click="connect" class="ml-2">{{
          t("Connect")
        }}</el-button>
        <el-button @click="toggleModel">{{ t("Close") }}</el-button>
      </div>
    </template>
  </Dialog>
</template>
