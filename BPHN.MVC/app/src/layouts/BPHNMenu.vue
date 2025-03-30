<script setup>
import { useI18n } from "vue-i18n";
import {
  Ticket,
  Calendar,
  MapLocation,
  Histogram,
} from "@element-plus/icons-vue";
import { useStore } from "vuex";
import router from "@/routers";

const { t } = useI18n();
const store = useStore();

const goTo = (link) => {
  router.push(link);
  store.commit("account/setDrawer", false);
}
</script>

<template>
  <section>
    <el-menu class="el-menu-vertical-demo">
      <el-menu-item @click="goTo('/')">
        <el-icon class="mr-3"><Histogram /></el-icon>
        <template #title>
          <router-link class="text-decoration-none" to="/">
            <h2>{{ t("Home") }}</h2>
          </router-link>
        </template>
      </el-menu-item>

      <el-menu-item @click="goTo('booking')">
        <el-icon class="mr-3"><Ticket /></el-icon>
        <template #title>
          <router-link class="text-decoration-none" to="/booking">
            <h2>{{ t("Booking") }}</h2>
          </router-link>
        </template>
      </el-menu-item>

      <el-menu-item @click="goTo('about-us')">
        <el-icon class="mr-3"><Calendar /></el-icon>
        <template #title>
          <router-link class="text-decoration-none" to="/about">
            <h2>{{ t("AboutUs") }}</h2>  
          </router-link>
        </template>
      </el-menu-item>

      <el-menu-item @click="goTo('stadiums')">
        <el-icon class="mr-3"><MapLocation /></el-icon>
        <template #title>
          <router-link class="text-decoration-none" to="/stadiums">
            <h2>{{ t("Stadiums") }}</h2>
          </router-link>
        </template>
      </el-menu-item>  
    </el-menu>
  </section>
</template>

<style scoped>
</style>