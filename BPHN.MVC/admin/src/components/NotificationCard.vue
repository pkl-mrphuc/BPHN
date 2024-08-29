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
      <div class="text-truncate" :title="content">{{ content }}</div>
      <div class="fst-italic text-truncate" :title="author">{{ author }}</div>
    </div>
    <el-divider />
  </div>
</template>

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
const { dateToString, padToFive } = useCommonFn();
const store = useStore();
const formatDate = ref(store.getters["config/getFormatDate"]);
const { t } = useI18n();

const subject = ref(props.data?.subject);
const content = computed(() => {
  let model = JSON.parse(props.data?.content);
  switch (props.data?.notificationType) {
    case NotificationTypeEnum.CANCELBOOKINGDETAIL:
      return t("CANCELBOOKINGDETAIL", { code : `M${padToFive(model?.MatchCode)}` }) ;
    case NotificationTypeEnum.UPDATEMATCH:
      return t("UPDATEMATCH", { code : `M${padToFive(model?.MatchCode)}` });
    case NotificationTypeEnum.INSERTBOOKING:
      return t("INSERTBOOKING", { phone: model?.PhoneNumber, info: `${model?.PitchName}-${model?.NameDetail}`, timeFrame: model?.TimeFrameInfoName });
    case NotificationTypeEnum.DECLINEBOOKING:
      return t("DECLINEBOOKING", { phone: model?.PhoneNumber, info: `${model?.PitchName}-${model?.NameDetail}`, timeFrame: model?.TimeFrameInfoName });
    case NotificationTypeEnum.APPROVALBOOKING:
      return t("APPROVALBOOKING", { phone: model?.PhoneNumber, info: `${model?.PitchName}-${model?.NameDetail}`, timeFrame: model?.TimeFrameInfoName });
    case NotificationTypeEnum.CHANGEPERMISSION:
      return t("CHANGEPERMISSION", { name: model?.UserName });
    case NotificationTypeEnum.INSERTPITCH:
      return t("INSERTPITCH", { name: model?.Name });
    case NotificationTypeEnum.UPDATEPITCH:
      return t("UPDATEPITCH", { name: model?.Name });
    case NotificationTypeEnum.INSERTACCOUNT:
      return t("INSERTACCOUNT", { name: model?.UserName });
    case NotificationTypeEnum.UPDATEACCOUNT:
      return t("UPDATEACCOUNT", { name: model?.UserName });
    default:
      return "";
  }
});
const author = computed(() => {
  return `${props.data?.createdBy} - ${dateToString(
    props.data?.createdDate,
    formatDate.value,
    true
  )}`;
});
</script>
