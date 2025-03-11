<script setup>
import { FunctionTypeEnum } from "@/const";
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

const functionName = (functionType) => {
  switch (functionType) {
    case FunctionTypeEnum.ADDPITCH:
      return t("ADDPITCH");
    case FunctionTypeEnum.EDITPITCH:
      return t("EDITPITCH");
    case FunctionTypeEnum.VIEWLISTPITCH:
      return t("VIEWLISTPITCH");
    case FunctionTypeEnum.ADDBOOKING:
      return t("ADDBOOKING");
    case FunctionTypeEnum.EDITBOOKING:
      return t("EDITBOOKING");
    case FunctionTypeEnum.VIEWLISTBOOKING:
      return t("VIEWLISTBOOKING");
    case FunctionTypeEnum.ADDUSER:
      return t("ADDUSER");
    case FunctionTypeEnum.EDITUSER:
      return t("EDITUSER");
    case FunctionTypeEnum.VIEWLISTUSER:
      return t("VIEWLISTUSER");
    case FunctionTypeEnum.VIEWLISTBOOKINGDETAIL:
      return t("VIEWLISTBOOKINGDETAIL");
    case FunctionTypeEnum.ADDINVOICE:
      return t("ADDINVOICE");
    case FunctionTypeEnum.EDITINVOICE:
      return t("EDITINVOICE");
    case FunctionTypeEnum.VIEWLISTINVOICE:
      return t("VIEWLISTINVOICE");
      case FunctionTypeEnum.ADDSERVICE:
      return t("ADDSERVICE");
    case FunctionTypeEnum.EDITSERVICE:
      return t("EDITSERVICE");
    case FunctionTypeEnum.VIEWLISTSERVICE:
      return t("VIEWLISTSERVICE");
    default:
      return "";
  }
};

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