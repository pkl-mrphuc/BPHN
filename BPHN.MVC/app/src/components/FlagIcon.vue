<template>
  <div class="flag">
    <img :src="path" />
  </div>
</template>

<script setup>
import { computed, defineProps } from "vue";

const props = defineProps({
  name: String,
});

const path = computed(() => {
  return require(`../assets/images/flags/${props.name}.png`);
});
</script>

<style scoped>
.flag {
  width: 36px;
  height: 36px;
  border: none;
  outline: none;
  cursor: pointer;
}

.flag > img {
  background-position: center;
  background-repeat: no-repeat;
  background-size: cover;
  width: 100%;
  height: 100%;
}
</style>

