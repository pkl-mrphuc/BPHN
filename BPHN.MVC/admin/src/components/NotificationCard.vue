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
const subject = ref(props.data?.Subject);

const content = computed(() => {
  switch (props.data?.NotificationType) {
    case NotificationTypeEnum.CANCELBOOKINGDETAIL: return `${t("CANCELBOOKINGDETAIL")} ${props.data?.Content}`;
    case NotificationTypeEnum.UPDATEMATCH: return `${t("UPDATEMATCH")} ${props.data?.Content}`;
    case NotificationTypeEnum.INSERTBOOKING: return `${t("INSERTBOOKING")} ${props.data?.Content}`;
    case NotificationTypeEnum.DECLINEBOOKING: return `${t("DECLINEBOOKING")} ${props.data?.Content}`;
    case NotificationTypeEnum.APPROVALBOOKING: return `${t("APPROVALBOOKING")} ${props.data?.Content}`;
    case NotificationTypeEnum.CHANGEPERMISSION: return `${t("CHANGEPERMISSION")} ${props.data?.Content}`;
    case NotificationTypeEnum.INSERTPITCH: return `${t("INSERTPITCH")} ${props.data?.Content}`;
    case NotificationTypeEnum.UPDATEPITCH: return `${t("UPDATEPITCH")} ${props.data?.Content}`;
    case NotificationTypeEnum.INSERTACCOUNT: return `${t("INSERTACCOUNT")} ${props.data?.Content}`;
    case NotificationTypeEnum.UPDATEACCOUNT: return `${t("UPDATEACCOUNT")} ${props.data?.Content}`;
    case NotificationTypeEnum.INSERTSERVICE: return `${t("INSERTSERVICE")} ${props.data?.Content}`;
    case NotificationTypeEnum.UPDATESERVICE: return `${t("UPDATESERVICE")} ${props.data?.Content}`;
    case NotificationTypeEnum.INSERTINVOICE: return `${t("INSERTINVOICE")} ${props.data?.Content}`;
    case NotificationTypeEnum.UPDATEINVOICE: return `${t("UPDATEINVOICE")} ${props.data?.Content}`;
    default: return "";
  }
});

const author = computed(() => {
  return `${props.data?.CreatedBy} ${dateToString(props.data?.CreatedDate, formatDate.value, true)}`;
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
