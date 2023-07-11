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
  <section class="dot">
    <div class="active dot_icon" v-if="equals(props.status, StatusEnum.ACTIVE)"></div>
    <div class="inactive dot_icon" v-else></div>
    <p>{{ props.message }}</p>
  </section>
</template>

<style scoped>
.dot {
  display: flex;
  align-items: center;
}

.dot_icon {
  width: 10px;
  height: 10px;
  border-radius: 50%;
  margin-right: 5px;
}

.dot_icon.active {
  background-color: #529b2e;
}

.dot_icon.inactive {
  background-color: #73767a;
}
</style>