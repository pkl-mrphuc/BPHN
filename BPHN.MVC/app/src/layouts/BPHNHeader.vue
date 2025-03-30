<script setup>
import { onMounted, ref } from "vue";
import { useStore } from "vuex";
import { Expand } from "@element-plus/icons-vue";
import { useRouter } from "vue-router";

const router = useRouter();
const store = useStore();
const drawer = ref(store.getters["account/getDrawer"]);

onMounted(() => {
  
});

const goToHome = () => {
  router.push("calendar");
};

const toggle = () => {
  store.commit("account/setDrawer", !drawer.value);
};

</script>

<template>
  <section class="h-100 d-flex flex-row align-items-center justify-content-between">
    <div class="pointer d-flex flex-row align-items-center justify-content-between">
      <el-button @click="toggle" class="mr-2" circle :icon="Expand" size="large"></el-button>
      <div @click="goToHome" class="d-flex flex-row align-items-center">
        <img height="50" class="mt-3" src="../assets/images/logo.png"/>
        <h4 class="fs-2 m-0 mt-1">BPHN</h4>
      </div>
    </div>
    <div class="d-flex flex-row align-items-center">
      
    </div>
  </section>
</template>
