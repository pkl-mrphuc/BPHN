<script setup>
import { computed } from "vue";
import { useStore } from "vuex";

const store = useStore();
const drawer = computed(() => {
  return store.getters["account/getDrawer"];
});

const handleClose = () => {
  store.commit("account/setDrawer", false);
};
</script>

<template>
  <div>
    <el-container>
      <el-header>
        <bphn-header></bphn-header>
      </el-header>
      <el-container>
        <el-drawer v-model="drawer" :before-close="handleClose" direction="ltr" :withHeader="false">
          <bphn-menu></bphn-menu>
        </el-drawer>
        <el-main>
          <router-view />
        </el-main>
      </el-container>
    </el-container>
  </div>
</template>

<script>
import BphnHeader from "@/layouts/BPHNHeader.vue";
import BphnMenu from "@/layouts/BPHNMenu.vue";

export default {
  name: "BPHNLayout",
  components: { BphnHeader, BphnMenu }
};
</script>

<style scoped>

</style>