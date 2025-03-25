<script setup>
import { useI18n } from "vue-i18n";
import { defineProps } from "vue";
import useCommonFn from "@/commonFn";

const { t } = useI18n();
const { fakeNumber } = useCommonFn();
const props = defineProps({
  type: String,
  value: Number
});

</script>
<template>
    <div class="d-flex flex-column align-items-center justify-content-center bphn-statistic2">
        <span class="bphn-statistic2__status">{{ t(props.type) }}</span>
        <span class="bphn-statistic2__data">{{ props.value > 0 ? fakeNumber(props.value) : 0 }}</span>
    </div>
</template>

<style scoped>
</style>