<template>
  <div class="common-layout">
    <el-container>
      <el-header
        class="p-0 row d-flex flex-row align-items-center"
        style="border-bottom: solid 1px var(--el-menu-border-color)"
      >
        <div class="col-1 col-sm-1 col-md-1 col-lg-2"></div>
        <div
          class="col-10 col-sm-10 col-md-10 col-lg-8 d-flex flex-row align-items-center"
        >
          <h1 class="fs-1 m-0 pointer text-decoration-underline">BPHN</h1>
          <div class="ml-auto"></div>
          <el-menu
            class="menu w-100 d-flex flex-row align-items-center justify-content-end"
            mode="horizontal"
            :ellipsis="false"
          >
            <el-menu-item
              index="1"
              @click="goTo('')"
              class="text-decoration-none text-uppercase"
              >{{ t("Home") }}</el-menu-item
            >
            <el-menu-item
              index="2"
              @click="goTo('booking')"
              class="text-decoration-none text-uppercase"
              >{{ t("Booking") }}</el-menu-item
            >
            <el-sub-menu index="3" class="text-decoration-none text-uppercase">
              <template #title>{{ t("Service") }}</template>
              <el-menu-item index="3_1" @click="goTo('partner-service')">{{
                t("Partner")
              }}</el-menu-item>
            </el-sub-menu>
            <el-menu-item
              index="4"
              @click="goTo('contact-me')"
              class="text-decoration-none text-uppercase"
              >{{ t("Contact") }}</el-menu-item
            >
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
          </el-menu>
          <div class="menu-toggle pointer" @click="toggleMenu">
            <el-icon size="24" v-if="toggle"><Fold /></el-icon>
            <el-icon size="24" v-else><Expand /></el-icon>

            <el-drawer v-model="draw" :direction="direction" class="w-75">
              <el-menu style="border: 0">
                <el-menu-item
                  index="1"
                  @click="goTo('')"
                  class="text-decoration-none text-uppercase"
                  >{{ t("Home") }}</el-menu-item
                >
                <el-menu-item
                  index="2"
                  @click="goTo('booking')"
                  class="text-decoration-none text-uppercase"
                  >{{ t("Booking") }}</el-menu-item
                >
                <el-sub-menu
                  index="3"
                  class="text-decoration-none text-uppercase"
                >
                  <template #title>{{ t("Service") }}</template>
                  <el-menu-item index="3_1" @click="goTo('partner-service')">{{
                    t("Partner")
                  }}</el-menu-item>
                </el-sub-menu>
                <el-menu-item
                  index="4"
                  @click="goTo('contact-me')"
                  class="text-decoration-none text-uppercase"
                  >{{ t("Contact") }}</el-menu-item
                >
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
              </el-menu>
            </el-drawer>
          </div>
        </div>
        <div class="col-1 col-sm-1 col-md-1 col-lg-2"></div>
      </el-header>
      <el-main class="p-0 row d-flex flex-row align-items-center">
        <section class="col-1 col-sm-1 col-md-1 col-lg-2"></section>
        <section
          class="col-10 col-sm-10 col-md-10 col-lg-8 d-flex flex-row align-items-center"
        >
          <router-view />
        </section>
        <section class="col-1 col-sm-1 col-md-1 col-lg-2"></section>
      </el-main>
    </el-container>
  </div>
</template>


<script setup>
import FlagIcon from "@/components/FlagIcon.vue";
import { ref, computed, watch, onMounted } from "vue";
import { useStore } from "vuex";
import { useI18n } from "vue-i18n";
import router from "./routers";

const { t } = useI18n();
const i18n = useI18n();
const store = useStore();
const darkMode = ref(store.getters["config/getDarkMode"]);
const language = ref(store.getters["config/getLanguage"]);
const toggle = ref(true);
const draw = ref(false);
const direction = ref("rtl");

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

const toggleMenu = () => {
  toggle.value = !toggle.value;
  draw.value = !draw.value;
};

const goTo = (route) => {
  router.push(route);
};
</script>

<style scoped>
@import "@/assets/css/index.css";
.menu {
  display: none;
}
.menu-toggle {
  display: block;
}
@media (min-width: 576px) {
}
@media (min-width: 768px) {
}
@media (min-width: 992px) {
}
@media (min-width: 1200px) {
  .menu {
    display: flex;
  }

  .menu-toggle {
    display: none;
  }
}
@media (min-width: 1400px) {
}
</style>
