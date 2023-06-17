<script setup>
import { useI18n } from "vue-i18n";
import {
  Ticket,
  Calendar,
  Setting,
  MapLocation,
  VideoCameraFilled
} from "@element-plus/icons-vue";

const { t } = useI18n();
</script>

<template>
  <section>
    <el-menu class="el-menu-vertical-demo bphn-menu">
      <router-link class="menu-item_link" to="/bm">
        <el-menu-item index="1" :title="t('BMTitle')">
          <el-icon><Ticket /></el-icon>
          <span>{{ t("BM") }}</span>
        </el-menu-item>
      </router-link>

      <router-link class="menu-item_link" to="/calendar">
        <el-menu-item index="2" :title="t('CalendarTitle')">
          <el-icon><Calendar /></el-icon>
          <span>{{ t("Calendar") }}</span>
        </el-menu-item>
      </router-link>

      <router-link class="menu-item_link" to="/my-football-fields">
        <el-menu-item index="3" :title="t('FootballFieldTitle')">
          <el-icon><MapLocation /></el-icon>
          <span>{{ t("FootballField") }}</span>
        </el-menu-item>
      </router-link>

      <router-link class="menu-item_link" to="/configuartions">
        <el-menu-item index="4" :title="t('ConfigurationsTitle')">
          <el-icon><Setting /></el-icon>
          <span>{{ t("Configurations") }}</span>
        </el-menu-item>
      </router-link>

      <router-link class="menu-item_link" to="/history-logs">
        <el-menu-item index="5" :title="t('HistoryLogsTitle')">
          <el-icon><VideoCameraFilled /></el-icon>
          <span>{{ t("HistoryLog") }}</span>
        </el-menu-item>
      </router-link>
    </el-menu>
  </section>
</template>

<style scoped>
@import "@/assets/css/BPHNMenu.css";
</style>