<script setup>
import { FunctionTypeEnum } from "@/const";
import useToggleModal from "@/register-components/actionDialog";
import { defineProps, ref, inject } from "vue";
import { useI18n } from "vue-i18n";
import { ElLoading } from "element-plus";
import { useStore } from "vuex";
import { NotificationTypeEnum } from "@/const";
import connection from "@/ws";

const { t } = useI18n();
const { toggleModel } = useToggleModal();
const store = useStore();
const props = defineProps({
  data: Array,
  accountId: String
});

const lstPermission = ref(props.data);
const running = ref(0);
const loadingOptions = inject("loadingOptions");

const functionName = (functionType) => {
  switch (functionType) {
    case FunctionTypeEnum.ADD_PITCH:
      return t("ADD_PITCH");
    case FunctionTypeEnum.EDIT_PITCH:
      return t("EDIT_PITCH");
    case FunctionTypeEnum.VIEW_LIST_PITCH:
      return t("VIEW_LIST_PITCH");
    case FunctionTypeEnum.ADD_BOOKING:
      return t("ADD_BOOKING");
    case FunctionTypeEnum.EDIT_BOOKING:
      return t("EDIT_BOOKING");
    case FunctionTypeEnum.VIEW_LIST_BOOKING:
      return t("VIEW_LIST_BOOKING");
    case FunctionTypeEnum.ADD_USER:
      return t("ADD_USER");
    case FunctionTypeEnum.EDIT_USER:
      return t("EDIT_USER");
    case FunctionTypeEnum.VIEW_LIST_USER:
      return t("VIEW_LIST_USER");
    case FunctionTypeEnum.VIEW_LIST_BOOKING_DETAIL:
      return t("VIEW_LIST_BOOKING_DETAIL");
    default:
      return "";
  }
};

const save = () => {
  if (running.value > 0) return;
  ++running.value;
  const loading = ElLoading.service(loadingOptions);
  store.dispatch("permission/save", {
    accountId: props.accountId,
    permissions: lstPermission.value
  }).then((res) => {
    loading.close();
    if (res?.data?.success) {
      toggleModel();
      connection.invoke(
          "PushNotification",
          store.getters["account/getRelationIds"],
          store.getters["account/getAccountId"],
          NotificationTypeEnum.EDIT_USER
        );
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
  <Dialog :title="t('Permission')">
    <template #body>
      <el-table :data="lstPermission">
        <el-table-column :label="t('FunctionName')">
          <template #default="scope">
            {{ functionName(scope.row.functionType) }}
          </template>
        </el-table-column>
        <el-table-column label="" width="50">
          <template #default="scope">
            <el-checkbox v-model="scope.row.allow" size="small" />
          </template>
        </el-table-column>
      </el-table>
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