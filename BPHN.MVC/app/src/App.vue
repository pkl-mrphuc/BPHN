<template>
  <div class="common-layout">
    <el-container>
      <el-header class="header p-0">
        <el-menu
          class="menu d-flex justify-content-between align-items-center"
          mode="horizontal"
        >
          <div>
            <div class="header_left">
              <h1 class="header_left__logo">BPHN</h1>
            </div>
          </div>

          <div class="d-flex justify-content-center align-items-center">
            <router-link to="/" class="menu_item">
              <el-menu-item index="0">{{ t('Home') }}</el-menu-item>
            </router-link>
            <router-link to="/booking" class="menu_item">
              <el-menu-item index="1">{{ t('Booking') }}</el-menu-item>
            </router-link>
            <el-sub-menu index="2" class="menu_item">
              <template #title>{{ t('Service') }}</template>
              <router-link class="menu_item" to="/partner-service">
                <el-menu-item index="2-1">{{ t('Partner') }}</el-menu-item>
              </router-link>
            </el-sub-menu>

            <router-link to="/contact-me" class="menu_item">
              <el-menu-item index="3">{{ t('Contact') }}</el-menu-item>
            </router-link>
          </div>

          <div class="d-flex align-items-center justify-content-center">
            <el-switch
              v-model="darkMode"
              active-icon="Moon"
              inactive-icon="Sunny"
              class="mx-12"
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
      </el-header>
      <el-main class="d-flex wp-100 p-0">
        <section class="wp-15 hp-100"></section>
        <section class="wp-70 hp-100">
          <router-view />
        </section>
        <section class="wp-15 hp-100"></section>
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
})

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

.menu {
  border-bottom: 0;
  width: 70%;
  margin: 0 auto;
}

.menu_item {
  text-decoration: none;
  text-transform: uppercase;
}

.header {
  border-bottom: solid 1px var(--el-menu-border-color);
}

.header_left {
  box-sizing: border-box;
  display: flex;
  align-items: center;
  justify-content: center;
  cursor: pointer;
}

.header_left__logo {
  font-size: 45px;
  margin: 0;
  text-decoration: underline;
}
</style>
