<script setup>
import { useI18n } from "vue-i18n";
import {
  Ticket,
  Calendar,
  Tools,
  MapLocation,
  VideoCameraFilled,
  User,
  Histogram,
  Finished,
  Notebook
} from "@element-plus/icons-vue";
import { useStore } from "vuex";
import { computed } from "vue";
import { RoleEnum } from "@/const";
import useCommonFn from "@/commonFn";
import router from "@/routers";

const { t } = useI18n();
const store = useStore();
const { equals } = useCommonFn();

const role = computed(() => {
  return store.getters["account/getRole"];
});

const multiUser = computed(() => {
  return store.getters["config/getMultiUser"];
});

const goTo = (link) => {
  router.push(link);
  store.commit("account/setDrawer", false);
}
</script>

<template>
  <section>
    <el-menu class="el-menu-vertical-demo" default-active="1">
      <el-menu-item @click="goTo('')" index="1">
        <el-icon class="mr-3"><Histogram /></el-icon>
        <template #title>
          <router-link class="text-decoration-none" to="/">
            <h2>{{ t("Overview") }}</h2>
          </router-link>
        </template>
      </el-menu-item>

      <el-menu-item @click="goTo('bm')" index="2">
        <el-icon class="mr-3"><Ticket /></el-icon>
        <template #title>
          <router-link class="text-decoration-none" to="/bm">
            <h2>{{ t("BM") }}</h2>
          </router-link>
        </template>
      </el-menu-item>

      <el-menu-item @click="goTo('calendar')" index="3">
        <el-icon class="mr-3"><Calendar /></el-icon>
        <template #title>
          <router-link class="text-decoration-none" to="/calendar">
            <h2>{{ t("Calendar") }}</h2>
          </router-link>
        </template>
      </el-menu-item>

      <el-menu-item @click="goTo('my-football-fields')" index="4">
        <el-icon class="mr-3"><MapLocation /></el-icon>
        <template #title>
          <router-link class="text-decoration-none" to="/my-football-fields">
            <h2>{{ t("FootballField") }}</h2>
          </router-link>
        </template>
      </el-menu-item>

      <el-menu-item  @click="goTo('invoices')" index="5">
        <el-icon class="mr-3"><Finished /></el-icon>
        <template #title>
          <router-link class="text-decoration-none" to="/invoices">
            <h2>{{ t("Invoices") }}</h2>
          </router-link>
        </template>
      </el-menu-item>

      <el-menu-item @click="goTo('services')" index="6">
        <el-icon class="mr-3"><Notebook /></el-icon>
        <template #title>
          <router-link class="text-decoration-none" to="/services">
            <h2>{{ t("Services") }}</h2>
          </router-link>
        </template>
      </el-menu-item>

      <el-menu-item v-if="equals(role, RoleEnum.ADMIN) || multiUser" @click="goTo('tenants')">
        <el-icon class="mr-3"><User /></el-icon>
        <template #title>
          <router-link class="text-decoration-none" to="/tenants">
            <h2>{{ t("Accounts") }}</h2>
          </router-link>
        </template>
      </el-menu-item>

      <el-menu-item @click="goTo('configuartions')" index="7">
        <el-icon class="mr-3"><Tools /></el-icon>
        <template #title>
          <router-link class="text-decoration-none" to="/configuartions">
            <h2>{{ t("Configurations") }}</h2>
          </router-link>
        </template>
      </el-menu-item>

      <el-menu-item @click="goTo('history-logs')" index="8">
        <el-icon class="mr-3"><VideoCameraFilled /></el-icon>
        <template #title>
          <router-link class="text-decoration-none" to="/history-logs">
            <h2>{{ t("HistoryLog") }}</h2>
          </router-link>
        </template>
      </el-menu-item>
    </el-menu>
  </section>
</template>

<style scoped>
</style>