<script setup>
import { useI18n } from "vue-i18n";
import { defineProps } from "vue";
import useCommonFn from "@/commonFn";
import router from "@/routers";
import { useStore } from "vuex";

const { t } = useI18n();
const { fakeNumber } = useCommonFn();
const store = useStore();
const props = defineProps({
  type: String,
  value: Number
});

const goToBm = () => {
  store.commit("cache/setBmVariableCache", {
    checked1: true,
    checked4: true,
    status: props.type,
    matchDate: new Date()
  });
  router.push("bm");
};

</script>
<template>
    <div @click="goToBm" class="d-flex flex-column align-items-center justify-content-center bphn-statistic2">
        <span class="bphn-statistic2__status">{{ t(props.type) }}</span>
        <span class="bphn-statistic2__data">{{ props.value > 0 ? fakeNumber(props.value) : 0 }}</span>
    </div>
</template>

<style scoped>
</style>