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
}
</script>

<template>
  <section>
    <el-menu class="el-menu-vertical-demo">
      <el-menu-item @click="goTo('overview')">
        <el-icon class="mr-3"><Histogram /></el-icon>
        <template #title>
          <router-link class="text-decoration-none" to="/overview">
            <h2>{{ t("Overview") }}</h2>
          </router-link>
        </template>
      </el-menu-item>

      <el-menu-item @click="goTo('bm')">
        <el-icon class="mr-3"><Ticket /></el-icon>
        <template #title>
          <router-link class="text-decoration-none" to="/bm">
            <h2>{{ t("BM") }}</h2>
          </router-link>
        </template>
      </el-menu-item>

      <el-menu-item @click="goTo('calendar')">
        <el-icon class="mr-3"><Calendar /></el-icon>
        <template #title>
          <router-link class="text-decoration-none" to="/calendar">
            <h2>{{ t("Calendar") }}</h2>
          </router-link>
        </template>
      </el-menu-item>

      <el-menu-item @click="goTo('my-football-fields')">
        <el-icon class="mr-3"><MapLocation /></el-icon>
        <template #title>
          <router-link class="text-decoration-none" to="/my-football-fields">
            <h2>{{ t("FootballField") }}</h2>
          </router-link>
        </template>
      </el-menu-item>

      <el-menu-item  @click="goTo('invoices')">
        <el-icon class="mr-3"><Finished /></el-icon>
        <template #title>
          <router-link class="text-decoration-none" to="/invoices">
            <h2>{{ t("Invoices") }}</h2>
          </router-link>
        </template>
      </el-menu-item>

      <el-menu-item @click="goTo('services')">
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

      <el-menu-item @click="goTo('configuartions')">
        <el-icon class="mr-3"><Tools /></el-icon>
        <template #title>
          <router-link class="text-decoration-none" to="/configuartions">
            <h2>{{ t("Configurations") }}</h2>
          </router-link>
        </template>
      </el-menu-item>

      <el-menu-item @click="goTo('history-logs')">
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