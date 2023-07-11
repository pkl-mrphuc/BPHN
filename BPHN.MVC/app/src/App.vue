<template>
  <div class="common-layout">
    <el-container>
      <el-header class="p-0 row" style="border-bottom: solid 1px var(--el-menu-border-color);">
        <div class="col-2"></div>
        <el-menu
          class="col-8 d-flex flex-row justify-content-between align-items-center border-0"
          mode="horizontal"
        >
          <h1 class="fs-1 m-0 pointer text-decoration-underline">BPHN</h1>

          <div class="d-flex flex-row justify-content-center align-items-center">
            <router-link to="/" class="text-decoration-none text-uppercase">
              <el-menu-item index="0">{{ t("Home") }}</el-menu-item>
            </router-link>
            <router-link to="/booking" class="text-decoration-none text-uppercase">
              <el-menu-item index="1">{{ t("Booking") }}</el-menu-item>
            </router-link>
            <el-sub-menu index="2" class="text-decoration-none text-uppercase">
              <template #title>{{ t("Service") }}</template>
              <router-link class="text-decoration-none text-uppercase" to="/partner-service">
                <el-menu-item index="2-1">{{ t("Partner") }}</el-menu-item>
              </router-link>
            </el-sub-menu>

            <router-link to="/contact-me" class="text-decoration-none text-uppercase">
              <el-menu-item index="3">{{ t("Contact") }}</el-menu-item>
            </router-link>
          </div>

          <div class="d-flex flex-row align-items-center justify-content-center">
            <el-switch
              v-model="darkMode"
              active-icon="Moon"
              inactive-icon="Sunny"
              class="mx-3"
              @change="changeDarkMode"
            />
            <el-dropdown @command="changeLanguage">
              <flag-icon :name="language"></flag-icon>
              <template #dropdown>
                <el-dropdown-menu>
                  <el-dropdown-item command="vi">
                    <flag-icon name="vi"></flag-icon>
                  </el-dropdown-item>
                  <el-dropdown-item command="en">
                    <flag-icon name="en"></flag-icon>
                  </el-dropdown-item>
                </el-dropdown-menu>
              </template>
            </el-dropdown>
          </div>
        </el-menu>
        <div class="col-2"></div>
      </el-header>
      <el-main class="row p-0">
        <section class="col-2"></section>
        <section class="col-8">
          <router-view />
        </section>
        <section class="col-2"></section>
      </el-main>
    </el-container>
  </div>
</template>


<script setup>
import FlagIcon from "@/components/FlagIcon.vue";
import { ref, computed, watch, onMounted } from "vue";
import { useStore } from "vuex";
import { useI18n } from "vue-i18n";

const { t } = useI18n();
const i18n = useI18n();
const store = useStore();
const darkMode = ref(store.getters["config/getDarkMode"]);
const language = ref(store.getters["config/getLanguage"]);

const getDarkMode = computed(() => {
  return store.getters["config/getDarkMode"];
});

const getLanguage = computed(() => {
  return store.getters["config/getLanguage"];
});

watch(getDarkMode, (newValue) => {
  darkMode.value = newValue;
});

watch(getLanguage, (newValue) => {
  language.value = newValue;
});

onMounted(() => {
  load(language.value, darkMode.value);
});

const changeLanguage = (command) => {
  store.commit("config/setLanguage", command);
  load(command, darkMode.value);
};

const changeDarkMode = () => {
  store.commit("config/setDarkMode", darkMode.value);
  load(language.value, darkMode.value);
};

const load = (language, darkMode) => {
  if (darkMode) document.documentElement.setAttribute("class", "dark");
  else document.documentElement.removeAttribute("class");
  i18n.locale.value = language;
};
</script>

<style scoped>
@import "@/assets/css/index.css";
</style>
