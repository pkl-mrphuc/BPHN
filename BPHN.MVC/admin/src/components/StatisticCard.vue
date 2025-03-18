<script setup>
import { StatisticTypeEnum } from "@/const";
import { useI18n } from "vue-i18n";
import { defineProps, ref } from "vue";
import useCommonFn from "@/commonFn";

const { t } = useI18n();
const { fakeNumber } = useCommonFn();
const props = defineProps({
  type: String,
  time: String,
  value: String,
  preValue: String,
  border: Boolean
});

const val = ref(props.value ?? 0);
const preVal = ref(props.preValue ?? 0);
const time = ref(props.time ?? "");
const isDown = ref(false);

const diff = () => {
    if (preVal.value == 0) return "0%";
    let result = (((val.value - preVal.value) / preVal.value) * 100);
    if (result > 0) {
        isDown.value = false;
        return `+${result.toFixed(0)}%`;
    }
    else {
        isDown.value = true;
        return `${result.toFixed(0)}%`;
    }
};

const title = () => {
    switch (props.type) {
        case StatisticTypeEnum.REVENUEDAY: return `${t(StatisticTypeEnum.REVENUEDAY)} ${time.value}`;
        case StatisticTypeEnum.REVENUEMONTH: return `${t(StatisticTypeEnum.REVENUEMONTH)} ${time.value}`;
        case StatisticTypeEnum.REVENUEYEAR: return `${t(StatisticTypeEnum.REVENUEYEAR)} ${time.value}`;
        case StatisticTypeEnum.REVENUEQUARTER: return `${t(StatisticTypeEnum.REVENUEQUARTER)} ${time.value}`;
        case StatisticTypeEnum.TOTALBOOKINGYEAR: return `${t(StatisticTypeEnum.TOTALBOOKINGYEAR)} ${time.value}`;
        default: return "";
    }
};

const diffTo = () => {
    switch (props.type) {
        case StatisticTypeEnum.REVENUEDAY: 
            return `${t("DiffToPreDay")}`;
        case StatisticTypeEnum.REVENUEMONTH: 
            return `${t("DiffToPreMonth")}`;
        case StatisticTypeEnum.REVENUEYEAR:
        case StatisticTypeEnum.TOTALBOOKINGYEAR:
            return `${t("DiffToPreYear")}`;
        case StatisticTypeEnum.REVENUEQUARTER: 
            return `${t("DiffToPreQuarter")}`;
        default: 
            return "";
    }
};

</script>
<template>
    <el-card :body-style="{ padding: '0px', 'border-right': border ? '1px solid #f5f5f5' : 'none' }">
        <el-statistic :value="fakeNumber(val)">
            <template #title>
                <div class="d-flex flex-row align-items-center">
                    <span class="statistic__title">{{ title() }}</span>
                </div>
            </template>
        </el-statistic>
        <div class="statistic__footer">
            <div class="d-flex flex-row align-items-center">
                <span :class="(isDown ? 'statistic__footer--down' : 'statistic__footer--up')">{{ diff() }}</span>
                <span>{{ diffTo() }}</span>
            </div>
        </div>
    </el-card>
</template>

<style scoped>
.el-card {
    border: 0;
    border-radius: 15px;
    padding: 20px;
}
.el-card.is-always-shadow {
    box-shadow: none;
}
.statistic__title{
    font-size: 24px;
    margin-bottom: 7px;
}
.statistic__footer {
    margin-top: 20px;
    font-size: 10px;
}
.statistic__footer--up {
    background-color: #55be24;
    border-radius: 5px;
    padding: 4px 8px;
    margin-right: 6px;
    font-weight: 700;
}
.statistic__footer--down {
    background-color: #f51a1a;
    border-radius: 5px;
    padding: 4px 8px;
    margin-right: 6px;
    font-weight: 700;
}
.el-statistic {
  --el-statistic-content-font-size: 35px;
  --el-statistic-content-font-weight: 700;
}
</style>