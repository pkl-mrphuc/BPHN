<script setup>
import { onMounted, ref, defineProps } from 'vue';
import * as echarts from 'echarts';
import { StatisticTypeEnum } from "@/const";
import { useI18n } from "vue-i18n";
import { useStore } from 'vuex';

const { t } = useI18n();
const store = useStore();
const props = defineProps({
    type: String,
    data: Object
});
const chartRef = ref(null);
const time = ref(new Date());
const darkMode = ref(store.getters["config/getDarkMode"]);

const title = () => {
    switch (props.type) {
        case StatisticTypeEnum.REVENUESERVICEYEAR: return `${t(StatisticTypeEnum.REVENUESERVICEYEAR)} ${time.value.getFullYear()}`;
    }
};

const getDataSource = () => {
    return [
        { name: t("BM"), value: props.data?.detail1 ?? 0 },
        { name: t("Services"), value: (props?.data?.total ?? 0) - (props.data?.detail1 ?? 0) },
    ];
};

onMounted(() => {
    if (chartRef.value) {
        const myChart = echarts.init(chartRef.value, null, {
            renderer: 'canvas',
            useDirtyRect: false
        });

        const option = {
            tooltip: {
                trigger: 'item'
            },
            legend: {
                bottom: '5%',
                left: 'center',
                textStyle: {
                    color: darkMode.value ? "#f5f5f5f5" : "#555555",
                }
            },
            series: [
                {
                    type: 'pie',
                    radius: ['40%', '70%'],
                    avoidLabelOverlap: false,
                    padAngle: 5,
                    itemStyle: {
                        borderRadius: 10
                    },
                    label: {
                        show: false,
                        position: 'center'
                    },
                    emphasis: {
                        label: {
                            show: true,
                            fontSize: 40,
                            fontWeight: 'bold'
                        }
                    },
                    labelLine: {
                        show: false
                    },
                    data: getDataSource()
                }
            ]
        };

        myChart.setOption(option);
        window.addEventListener('resize', myChart.resize);
    }
});
</script>

<template>
    <div class="bphn-statistic3">
        <div class="d-flex flex-row align-items-center justify-content-between bphn-statistic3__title">
            <span>{{ title() }}</span>
        </div>
        <div ref="chartRef" class="bphn-statistic3__chart"></div>
    </div>
</template>

<style>
</style>