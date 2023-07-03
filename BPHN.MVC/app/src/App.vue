<template>
  <div class="common-layout">
    <el-container>
      <el-header class="p-0">
        <el-menu
          :default-active="activeIndex"
          class="px-200 d-flex justify-content-between align-items-center"
          mode="horizontal"
          @select="handleSelect"
        >
          <div>
            <div class="header_left">
              <h1 class="header_left__logo">BPHN</h1>
            </div>
          </div>

          <div class="d-flex justify-content-center align-items-center">
            <el-menu-item index="0">HOME</el-menu-item>
            <el-menu-item index="1">CALENDAR</el-menu-item>
            <el-menu-item index="2">REGISTER</el-menu-item>
            <el-menu-item index="3">CONTACT</el-menu-item>
          </div>

          <div class="d-flex align-items-center justify-content-center">
            <el-switch
              v-model="darkMode"
              active-icon="Moon"
              inactive-icon="Sunny"
              class="mx-12"
              @change="loadMode"
            />
            <el-dropdown @command="changeLanguage">
              <flag-icon :name="lang"></flag-icon>
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
      <el-main>
        <router-view />
      </el-main>
      <el-footer>Footer</el-footer>
    </el-container>
  </div>
</template>


<script setup>
import FlagIcon from "@/components/FlagIcon.vue";
import { ref } from "vue";

const lang = ref("vi");
const activeIndex = ref("0");
const darkMode = ref(false);
const changeLanguage = (command) => {
  lang.value = command;
};
const loadMode = () => {
  if (darkMode.value) document.documentElement.setAttribute("class", "dark");
  else document.documentElement.removeAttribute("class");
};
</script>

<style scoped>
@import "@/assets/css/index.css";

.el-menu-demo {
  width: 100%;
  margin: auto 20px;
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
