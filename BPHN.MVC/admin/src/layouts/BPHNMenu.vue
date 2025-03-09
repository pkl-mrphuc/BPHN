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
import { computed, ref } from "vue";
import { RoleEnum } from "@/const";
import useCommonFn from "@/commonFn";
import router from "@/routers";

const { t } = useI18n();
const store = useStore();
const { equals } = useCommonFn();
const toggle = ref(true);

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
    <el-menu class="el-menu-vertical-demo" :collapse="toggle">
      <el-menu-item>
        <el-icon @click="goTo('overview')"><Histogram /></el-icon>
        <template #title>
          <router-link class="text-decoration-none" to="/overview">
            <span>{{ t("Overview") }}</span>
          </router-link>
        </template>
      </el-menu-item>

      <el-menu-item>
        <el-icon @click="goTo('bm')"><Ticket /></el-icon>
        <template #title>
          <router-link class="text-decoration-none" to="/bm">
            <span>{{ t("BM") }}</span>
          </router-link>
        </template>
      </el-menu-item>

      <el-menu-item>
        <el-icon @click="goTo('calendar')"><Calendar /></el-icon>
        <template #title>
          <router-link class="text-decoration-none" to="/calendar">
            <span>{{ t("Calendar") }}</span>
          </router-link>
        </template>
      </el-menu-item>

      <el-menu-item>
        <el-icon @click="goTo('my-football-fields')"><MapLocation /></el-icon>
        <template #title>
          <router-link class="text-decoration-none" to="/my-football-fields">
            <span>{{ t("FootballField") }}</span>
          </router-link>
        </template>
      </el-menu-item>

      <el-menu-item>
        <el-icon @click="goTo('invoices')"><Finished /></el-icon>
        <template #title>
          <router-link class="text-decoration-none" to="/invoices">
            <span>{{ t("Invoices") }}</span>
          </router-link>
        </template>
      </el-menu-item>

      <el-menu-item>
        <el-icon @click="goTo('services')"><Notebook /></el-icon>
        <template #title>
          <router-link class="text-decoration-none" to="/services">
            <span>{{ t("Services") }}</span>
          </router-link>
        </template>
      </el-menu-item>

      <el-menu-item v-if="equals(role, RoleEnum.ADMIN) || multiUser">
        <el-icon @click="goTo('tenants')"><User /></el-icon>
        <template #title>
          <router-link
            class="text-decoration-none"
            to="/tenants"
          >
            <span>{{ t("Accounts") }}</span>
          </router-link>
        </template>
      </el-menu-item>

      <el-menu-item>
        <el-icon @click="goTo('configuartions')"><Tools /></el-icon>
        <template #title>
          <router-link class="text-decoration-none" to="/configuartions">
            <span>{{ t("Configurations") }}</span>
          </router-link>
        </template>
      </el-menu-item>

      <el-menu-item>
        <el-icon @click="goTo('history-logs')"><VideoCameraFilled /></el-icon>
        <template #title>
          <router-link class="text-decoration-none" to="/history-logs">
            <span>{{ t("HistoryLog") }}</span>
          </router-link>
        </template>
      </el-menu-item>
    </el-menu>
  </section>
</template>

<style scoped>
.el-menu-vertical-demo {
  height: calc(100vh - 60px - 1px);
  overflow-y: scroll;
}
</style>