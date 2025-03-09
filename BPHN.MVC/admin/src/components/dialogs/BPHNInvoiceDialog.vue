<script setup>
import useToggleModal from "@/register-components/actionDialog";
import { defineProps, ref, inject } from "vue";
import { useI18n } from "vue-i18n";
import { ElLoading, ElNotification } from "element-plus";
import { useStore } from "vuex";

const { t } = useI18n();
const { toggleModel } = useToggleModal();
const store = useStore();
const props = defineProps({
  data: Array,
  accountId: String,
});

const lstPermission = ref(props.data);
const running = ref(0);
const loadingOptions = inject("loadingOptions");

const save = () => {
  if (running.value > 0) return;
  ++running.value;
  const loading = ElLoading.service(loadingOptions);
  store
    .dispatch("permission/save", {
      accountId: props.accountId,
      permissions: lstPermission.value,
    })
    .then((res) => {
      loading.close();
      if (res?.data?.success) {
        toggleModel();
        ElNotification({
          title: t("Notification"),
          message: t("SaveSuccess"),
          type: "success",
        });
      } else {
        ElNotification({
          title: t("Notification"),
          message: t("ErrorMesg"),
          type: "error",
        });
      }
      setTimeout(() => {
        running.value = 0;
      }, 1000);
    });
};
</script>

<template>
  <Dialog :title="t('Invoices')">
    <template #body>
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