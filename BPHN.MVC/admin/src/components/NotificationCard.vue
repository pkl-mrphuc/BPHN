<template>
  <div class="row d-flex flex-row align-items-center">
    <div class="col-2">
      <div class="mx-2">
        <el-icon size="24"><Check /></el-icon>
      </div>
    </div>
    <div class="col-10">
      <div class="fw-bold">
        <span class="text-truncate">{{ subject }}</span>
      </div>
      <div class="text-truncate">{{ content }}</div>
      <div class="fst-italic">{{ author }}</div>
    </div>
  </div>
</template>

<script setup>
import { defineProps, ref, computed } from "vue";
import useCommonFn from "@/commonFn";
import { useStore } from "vuex";
import { Check } from "@element-plus/icons-vue";

const props = defineProps({
  data: Object,
});
const { dateToString } = useCommonFn();
const store = useStore();
const formatDate = ref(store.getters["config/getFormatDate"]);

const subject = ref(props.data?.subject);
const content = ref(props.data?.content);
const author = computed(() => {
  return `${props.data?.createdBy} - ${dateToString(
    props.data?.createdDate,
    formatDate.value
  )}`;
});
</script>
