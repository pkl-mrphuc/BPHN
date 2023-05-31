<script setup>
import { ref } from 'vue'

const dialogVisible = ref(false)

</script>

<template>
  <el-dialog v-model="dialogVisible">
    <slot></slot>
  </el-dialog>
</template>