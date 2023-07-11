<script setup>
import { useI18n } from "vue-i18n";
import {
  Ticket,
  Calendar,
  Setting,
  MapLocation,
  VideoCameraFilled,
  User,
} from "@element-plus/icons-vue";
import { useStore } from "vuex";
import { computed } from "vue";
import { RoleEnum } from "@/const";
import useCommonFn from "@/commonFn";

const { t } = useI18n();
const store = useStore();
const { equals } = useCommonFn();

const role = computed(() => {
  return store.getters["account/getRole"];
});

const multiUser = computed(() => {
  return store.getters["config/getMultiUser"];
});
</script>

<template>
  <section>
    <el-menu class="el-menu-vertical-demo">
      <router-link class="text-decoration-none" to="/bm">
        <el-menu-item index="1" :title="t('BMTitle')">
          <el-icon><Ticket /></el-icon>
          <span>{{ t("BM") }}</span>
        </el-menu-item>
      </router-link>

      <router-link class="text-decoration-none" to="/calendar">
        <el-menu-item index="2" :title="t('CalendarTitle')">
          <el-icon><Calendar /></el-icon>
          <span>{{ t("Calendar") }}</span>
        </el-menu-item>
      </router-link>

      <router-link class="text-decoration-none" to="/my-football-fields">
        <el-menu-item index="3" :title="t('FootballFieldTitle')">
          <el-icon><MapLocation /></el-icon>
          <span>{{ t("FootballField") }}</span>
        </el-menu-item>
      </router-link>

      <router-link
        class="text-decoration-none"
        to="/tenants"
        v-if="equals(role, RoleEnum.ADMIN) || multiUser"
      >
        <el-menu-item index="4" :title="t('AccountsTitle')">
          <el-icon><User /></el-icon>
          <span>{{ t("Accounts") }}</span>
        </el-menu-item>
      </router-link>

      <router-link
        class="text-decoration-none"
        to="/configuartions"
        v-if="equals(role, RoleEnum.ADMIN) || multiUser"
      >
        <el-menu-item index="5" :title="t('ConfigurationsTitle')">
          <el-icon><Setting /></el-icon>
          <span>{{ t("Configurations") }}</span>
        </el-menu-item>
      </router-link>

      <router-link class="text-decoration-none" to="/history-logs">
        <el-menu-item index="6" :title="t('HistoryLogsTitle')">
          <el-icon><VideoCameraFilled /></el-icon>
          <span>{{ t("HistoryLog") }}</span>
        </el-menu-item>
      </router-link>
    </el-menu>
  </section>
</template>

<style scoped>
.el-menu-vertical-demo {
  height: calc(100vh - 60px - 1px);
  width: 100%;
  box-sizing: border-box;
  overflow-y: scroll;
  border-right: 1px solid #cecece;
}
</style>>