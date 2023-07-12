<script setup>
import useToggleModal from "@/register-components/actionDialog";
import { ref, defineProps } from "vue";
const { toggleModel } = useToggleModal();
const show = ref(true);

const props = defineProps({
  title: String
});
</script>

<template>
  <el-dialog
    v-model="show"
    :title="props.title"
    :before-close="toggleModel"
    :close-on-click-modal="false"
    class="w-100 w-sm-75 w-md-75 w-xl-50"
  >
    <slot name="body"></slot>
    <template #footer>
      <slot name="foot"></slot>
    </template>
  </el-dialog>
</template>