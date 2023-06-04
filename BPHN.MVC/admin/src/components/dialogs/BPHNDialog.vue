<script setup>
import useToggleModal from "@/register-components/actionDialog";
import { ref, defineProps } from "vue";
const { toggleModel } = useToggleModal();
const show = ref(true);

const props = defineProps({
  title: String,
  width: Number,
});
</script>

<template>
  <el-dialog
    v-model="show"
    :title="props.title"
    :width="props.width ?? 700"
    :before-close="toggleModel"
  >
    <slot name="body"></slot>
    <template #footer>
      <slot name="foot"></slot>
    </template>
  </el-dialog>
</template>