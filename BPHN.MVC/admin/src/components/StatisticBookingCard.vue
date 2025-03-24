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
.bphn-statistic2 {
    width: 114px;
    height: 114px;
    border-radius: 18px;
    cursor: pointer;
}
.bphn-statistic2__status {
    font-size: 16px;
    margin-bottom: 16px;
}
.bphn-statistic2__data {
    font-size: 26px;
    font-weight: 700;
}
</style>