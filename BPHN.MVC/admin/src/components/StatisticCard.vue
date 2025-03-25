<script setup>
import { StatisticTypeEnum } from "@/const";
import { useI18n } from "vue-i18n";
import { defineProps, ref } from "vue";
import useCommonFn from "@/commonFn";

const { t } = useI18n();
const { fakeNumber } = useCommonFn();
const props = defineProps({
  type: String,
  data: Object
});

const val = ref(props.data?.value ?? 0);
const preVal = ref(props.data?.preValue ?? 0);
const time = ref(props.data?.parameter);

const compareClass = () => {
    let result = val.value == preVal.value ? 0 : (val.value > preVal.value ? 1 : -1);
    switch (result) {
        case 1: return "statistic__footer--up";
        case -1: return "statistic__footer--down";
        default: return "statistic__footer--equal"
    }
};

const diff = () => {
    if (preVal.value == 0 && val.value > 0) return "+100%";
    if (preVal.value == 0 && val.value == 0) return "0%";
    
    let result = (((val.value - preVal.value) / preVal.value) * 100);
    if (result > 0) return `+${result.toFixed(0)}%`;
    else return `${result.toFixed(0)}%`;
};

const title = () => {
    switch (props.type) {
        case StatisticTypeEnum.REVENUEDAY: return `${t(StatisticTypeEnum.REVENUEDAY)}`;
        case StatisticTypeEnum.REVENUEMONTH: return `${t(StatisticTypeEnum.REVENUEMONTH)} ${time.value}`;
        case StatisticTypeEnum.REVENUEYEAR: return `${t(StatisticTypeEnum.REVENUEYEAR)} ${time.value}`;
        case StatisticTypeEnum.REVENUEQUARTER: return `${t(StatisticTypeEnum.REVENUEQUARTER)} ${time.value}`;
        case StatisticTypeEnum.TOTALBOOKINGYEAR: return `${t(StatisticTypeEnum.TOTALBOOKINGYEAR)} ${time.value}`;
        case StatisticTypeEnum.TOTALBOOKINGDAY: return `${t(StatisticTypeEnum.TOTALBOOKINGDAY)}`;
    }
};

const diffTo = () => {
    switch (props.type) {
        case StatisticTypeEnum.REVENUEDAY: 
        case StatisticTypeEnum.TOTALBOOKINGDAY: 
            return `${t("DiffToPreDay")}`;
        case StatisticTypeEnum.REVENUEMONTH: 
            return `${t("DiffToPreMonth")}`;
        case StatisticTypeEnum.REVENUEYEAR:
        case StatisticTypeEnum.TOTALBOOKINGYEAR:
            return `${t("DiffToPreYear")}`;
        case StatisticTypeEnum.REVENUEQUARTER: 
            return `${t("DiffToPreQuarter")}`;
    }
};

</script>
<template>
    <el-card :body-style="{ padding: '0px' }" class="bphn-statistic1">
        <el-statistic :value="val > 0 ? fakeNumber(val) : 0">
            <template #title>
                <div class="d-flex flex-row align-items-center">
                    <span class="statistic__title">{{ title() }}</span>
                </div>
            </template>
        </el-statistic>
        <div class="statistic__footer">
            <div class="d-flex flex-row align-items-center">
                <span :class="compareClass()">{{ diff() }}</span>
                <span>{{ diffTo() }}</span>
            </div>
        </div>
    </el-card>
</template>

<style scoped>
</style>