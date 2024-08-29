<script setup>
import { computed, onMounted, ref } from "vue";
import { useStore } from "vuex";
import { useI18n } from "vue-i18n";
import { SwitchButton, Refresh, Avatar, Bell } from "@element-plus/icons-vue";
import useToggleModal from "@/register-components/actionDialog";
import { useRouter } from "vue-router";
import NotificationCard from "@/components/NotificationCard.vue";
import connection from "@/ws";
import { ElNotification } from "element-plus";
import { NotificationTypeEnum } from "@/const";
import useCommonFn from "@/commonFn";

const router = useRouter();
const store = useStore();
const { t } = useI18n();
const { openModal, hasRole } = useToggleModal();
const { padToFive } = useCommonFn();
const lstNotification = ref([]);
const hasNewNoti = ref(false);

onMounted(() => {
  store.dispatch("notification/get").then((res) => {
    lstNotification.value = res.data?.data ?? [];
  });

  connection.on("PushNotification", function (type, model) {
    hasNewNoti.value = true;
    ElNotification({
      title: t("Notification"),
      message: getMessage(type, model),
      duration: 0,
    });
  });
});

const getMessage = (type, model) => {
  model = JSON.parse(model);
  switch (type) {
    case NotificationTypeEnum.CANCELBOOKINGDETAIL:
      return t("CANCELBOOKINGDETAIL", { code : `M${padToFive(model?.MatchCode)}` }) ;
    case NotificationTypeEnum.UPDATEMATCH:
      return t("UPDATEMATCH", { code : `M${padToFive(model?.MatchCode)}` });
    case NotificationTypeEnum.INSERTBOOKING:
      return t("INSERTBOOKING", { info: `${model?.PhoneNumber}/${model?.PitchName}-${model?.NameDetail}/${model?.TimeFrameInfoName}` });
    case NotificationTypeEnum.DECLINEBOOKING:
      return t("DECLINEBOOKING", { info: `${model?.PhoneNumber}/${model?.PitchName}-${model?.NameDetail}/${model?.TimeFrameInfoName}` });
    case NotificationTypeEnum.APPROVALBOOKING:
      return t("APPROVALBOOKING", { info: `${model?.PhoneNumber}/${model?.PitchName}-${model?.NameDetail}/${model?.TimeFrameInfoName}` });
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
};

const fullname = computed(() => {
  return store.getters["account/getFullName"];
});

const showAccountInfo = () => {
  openModal("AccountInfoDialog");
};

const logout = () => {
  localStorage.clear();
  router.go();
};

const goToHome = () => {
  router.push("calendar");
};

const refresh = () => {
  localStorage.removeItem("config-key");
  store.dispatch("account/refresh").then((res) => {
    console.log(res);
    router.go();
  })
};

const markRead = () => {
  if (hasNewNoti.value) {
    store.dispatch("notification/get").then((res) => {
      lstNotification.value = res.data?.data ?? [];
    });
  }
  hasNewNoti.value = false;
};
</script>

<template>
  <section
    class="h-100 d-flex flex-row align-items-center justify-content-between"
  >
    <h1 class="fs-1 m-0 pointer text-decoration-underline" @click="goToHome">
      BPHN
    </h1>
    <div class="d-flex flex-row align-items-center">
      <p class="account pointer">
        {{ t("Hello") }}
        <span class="mx-1 text-decoration-underline" @click="showAccountInfo">{{
          fullname
        }}</span>
      </p>
      <div class="mx-1 pointer account-sm" @click="showAccountInfo">
        <el-icon size="24"><Avatar /></el-icon>
      </div>
      <div class="mx-1 pointer">
        <el-popover trigger="click" :width="350">
          <template #reference>
            <el-badge is-dot :hidden="!hasNewNoti">
              <el-icon size="24" @click="markRead"><Bell /></el-icon>
            </el-badge>
          </template>
          <notification-card
            v-for="item in lstNotification"
            :key="item"
            :data="item"
          ></notification-card>
          <el-empty
            v-if="lstNotification.length == 0"
            :description="t('NoData')"
          />
        </el-popover>
      </div>
      <div class="mx-1 pointer" @click="refresh">
        <el-icon size="24"><Refresh /></el-icon>
      </div>
      <el-button
        class="mx-1 pointer"
        type="info"
        :icon="SwitchButton"
        circle
        @click="logout"
      >
      </el-button>
    </div>
  </section>
  <AccountInfoDialog v-if="hasRole('AccountInfoDialog')"> </AccountInfoDialog>
</template>

<style scoped>
.account {
  display: none;
}

.account-sm {
  display: block;
}

@media (min-width: 576px) {
  .account-sm {
    display: none;
  }

  .account {
    display: block;
  }
}
</style>