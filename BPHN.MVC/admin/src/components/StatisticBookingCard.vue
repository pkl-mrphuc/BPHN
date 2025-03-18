<script setup>
import { useI18n } from "vue-i18n";
import { defineProps } from "vue";
import useCommonFn from "@/commonFn";

const { t } = useI18n();
const { fakeNumber } = useCommonFn();
const props = defineProps({
  type: String,
  value: String
});

</script>
<template>
    <div class="d-flex flex-column align-items-center justify-content-center bcard">
        <span class="bcard__status">{{ t(props.type) }}</span>
        <span class="bcard__data">{{ fakeNumber(props.value ?? 0) }}</span>
    </div>
</template>

<style scoped>
.bcard {
    background-color: #555555;
    width: 114px;
    height: 114px;
    border-radius: 18px;
}
.bcard__status {
    font-size: 16px;
    margin-bottom: 16px;
}
.bcard__data {
    font-size: 26px;
    font-weight: 700;
}
</style>