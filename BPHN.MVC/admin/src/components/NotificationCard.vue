<script setup>
import { defineProps, ref, computed } from "vue";
import useCommonFn from "@/commonFn";
import { useStore } from "vuex";
import { Check } from "@element-plus/icons-vue";
import { useI18n } from "vue-i18n";
import { NotificationTypeEnum } from "@/const";

const props = defineProps({
  data: Object,
});
const { dateToString } = useCommonFn();
const { t } = useI18n();
const store = useStore();

const formatDate = ref(store.getters["config/getFormatDate"]);
const subject = ref(props.data?.subject);

const content = computed(() => {
  let model = JSON.parse(props.data?.content);
  console.log(model);
  switch (props.data?.notificationType) {
    case NotificationTypeEnum.CANCELBOOKINGDETAIL: return t("CANCELBOOKINGDETAIL") ;
    case NotificationTypeEnum.UPDATEMATCH: return t("UPDATEMATCH");
    case NotificationTypeEnum.INSERTBOOKING: return t("INSERTBOOKING");
    case NotificationTypeEnum.DECLINEBOOKING: return t("DECLINEBOOKING");
    case NotificationTypeEnum.APPROVALBOOKING: return t("APPROVALBOOKING");
    case NotificationTypeEnum.CHANGEPERMISSION: return t("CHANGEPERMISSION");
    case NotificationTypeEnum.INSERTPITCH: return t("INSERTPITCH");
    case NotificationTypeEnum.UPDATEPITCH: return t("UPDATEPITCH");
    case NotificationTypeEnum.INSERTACCOUNT: return t("INSERTACCOUNT");
    case NotificationTypeEnum.UPDATEACCOUNT: return t("UPDATEACCOUNT");
    case NotificationTypeEnum.INSERTSERVICIE: return t("INSERTSERVICIE");
    case NotificationTypeEnum.UPDATESERVICE: return t("UPDATESERVICE");
    case NotificationTypeEnum.INSERTINVOICE: return t("INSERTINVOICE");
    case NotificationTypeEnum.UPDATEINVOICE: return t("UPDATEINVOICE");
    default: return "";
  }
});

const author = computed(() => {
  return `${props.data?.createdBy} ${dateToString(props.data?.createdDate, formatDate.value, true)}`;
});
</script>

<template>
  <div class="row d-flex flex-row align-items-center">
    <div class="col-2">
      <div class="mx-2">
        <el-icon size="24"><Check /></el-icon>
      </div>
    </div>
    <div class="col-10">
      <div class="fw-bold mb-2">
        <span class="text-truncate">{{ t(subject) }}</span>
      </div>
      <div class="text-truncate fw-bold fs-5" :title="content">{{ content }}</div>
      <div class="fst-italic text-truncate" :title="author">{{ author }}</div>
    </div>
    <el-divider />
  </div>
</template>
