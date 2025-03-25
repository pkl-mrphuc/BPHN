<script setup>
import useCommonFn from "@/commonFn";
import { StatusEnum } from "@/const";
import { defineProps } from "vue";

const props = defineProps({
  status: String,
  message: String,
});
const { equals } = useCommonFn();
</script>

<template>
  <section class="d-flex flex-row align-items-center">
    <div class="active dot_icon" v-if="equals(props.status, StatusEnum.ACTIVE)"></div>
    <div class="inactive dot_icon" v-else></div>
    <p>{{ props.message }}</p>
  </section>
</template>

<style scoped>
</style>