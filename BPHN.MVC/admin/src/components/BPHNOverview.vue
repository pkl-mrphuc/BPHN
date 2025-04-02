<script setup>
import StatisticCard from "@/components/StatisticCard.vue";
import StatisticBookingCard from "@/components/StatisticBookingCard.vue";
import PieChart from "@/components/PieChartCard.vue";
import { BookingStatusEnum, StatisticTypeEnum } from "@/const";
import { useI18n } from "vue-i18n";
import { useStore } from "vuex";
import { onBeforeMount, ref, watchEffect } from "vue";
import router from "@/routers";
import useCommonFn from "@/commonFn";

const { t } = useI18n();
const { quarter } = useCommonFn();
const store = useStore();
const visible = ref(false);
const now = ref(new Date());
const isMobile = ref(store.getters["config/isMobile"]);
const totalBookingDay = ref({ value: 0, preValue: 0, parameter: now.value });
const totalBookingYear = ref({ value: 0, preValue: 0, parameter: now.value });
const revenueDay = ref({ value: 0, preValue: 0, parameter: now.value });
const revenueMonth = ref({ value: 0, preValue: 0, parameter: now.value });
const revenueYear = ref({ value: 0, preValue: 0, parameter: now.value });
const revenueQuarter = ref({ value: 0, preValue: 0, parameter: now.value });
const totalDetailBookingDay = ref({ value: 0, parameter: now.value });
const revenueServicesYear = ref({});
const checked1 = ref(true);
const checked2 = ref(true);
const checked3 = ref(true);
const checked4 = ref(true);
const checked5 = ref(true);

watchEffect(() => { checked1.value = store.getters["cache/getOverviewVariableCache"]?.checked1 ?? true; });
watchEffect(() => { checked2.value = store.getters["cache/getOverviewVariableCache"]?.checked2 ?? true; });
watchEffect(() => { checked3.value = store.getters["cache/getOverviewVariableCache"]?.checked3 ?? true; });
watchEffect(() => { checked4.value = store.getters["cache/getOverviewVariableCache"]?.checked4 ?? true; });
watchEffect(() => { checked5.value = store.getters["cache/getOverviewVariableCache"]?.checked5 ?? true; });

const onBack = () => {
    router.push("/");
};

const filter = () => {
    loadData();
    store.commit("cache/setOverviewVariableCache", {
        checked1: checked1.value,
        checked2: checked2.value,
        checked3: checked3.value,
        checked4: checked4.value,
        checked5: checked5.value
    });
};

const title = (type) => {
    switch (type) {
        case StatisticTypeEnum.REVENUEDAY: return `${t(StatisticTypeEnum.REVENUEDAY)}`;
        case StatisticTypeEnum.REVENUEMONTH: return `${t(StatisticTypeEnum.REVENUEMONTH)} ${now.value.getMonth() + 1}`;
        case StatisticTypeEnum.REVENUEYEAR: return `${t(StatisticTypeEnum.REVENUEYEAR)} ${now.value.getFullYear()}`;
        case StatisticTypeEnum.REVENUEQUARTER: return `${t(StatisticTypeEnum.REVENUEQUARTER)} ${quarter(now.value)}`;
        case StatisticTypeEnum.TOTALBOOKINGYEAR: return `${t(StatisticTypeEnum.TOTALBOOKINGYEAR)} ${now.value.getFullYear()}`;
        case StatisticTypeEnum.TOTALBOOKINGDAY: return `${t(StatisticTypeEnum.TOTALBOOKINGDAY)}`;
    }
};

const loadData = () => {
    let types = [
        { name: StatisticTypeEnum.TOTALBOOKINGDAY, parameter: totalBookingDay.value.parameter },
        { name: StatisticTypeEnum.TOTALDETAILBOOKINGDAY, parameter: totalDetailBookingDay.value.parameter },
        { name: StatisticTypeEnum.REVENUESERVICEYEAR, parameter: revenueServicesYear.value.parameter },
    ];
    if (checked1.value) types.push({ name: StatisticTypeEnum.REVENUEDAY, parameter: revenueDay.value.parameter });
    if (checked2.value) types.push({ name: StatisticTypeEnum.REVENUEMONTH, parameter: revenueMonth.value.parameter });
    if (checked3.value) types.push({ name: StatisticTypeEnum.REVENUEQUARTER, parameter: revenueQuarter.value.parameter });
    if (checked4.value) types.push({ name: StatisticTypeEnum.REVENUEYEAR, parameter: revenueYear.value.parameter });
    if (checked5.value) types.push({ name: StatisticTypeEnum.TOTALBOOKINGYEAR, parameter: totalBookingYear.value.parameter });
    store.dispatch("overview/get", { types: types })
    .then((res) => {
        if (res?.data?.data) {
            let result = res.data.data;
            if (result.TOTALBOOKINGDAY) {
                totalBookingDay.value = result.TOTALBOOKINGDAY;
            }
            if (result.REVENUEDAY) {
                revenueDay.value = result.REVENUEDAY;
            }
            if (result.REVENUEMONTH) {
                revenueMonth.value = result.REVENUEMONTH;
            }
            if (result.REVENUEYEAR) {
                revenueYear.value = result.REVENUEYEAR;
            }
            if (result.REVENUEQUARTER) {
                revenueQuarter.value = result.REVENUEQUARTER;
            }
            if (result.TOTALBOOKINGYEAR) {
                totalBookingYear.value = result.TOTALBOOKINGYEAR;
            }
            if (result.TOTALDETAILBOOKINGDAY) {
                totalDetailBookingDay.value = result.TOTALDETAILBOOKINGDAY;
            }
            if (result.REVENUESERVICEYEAR) {
                revenueServicesYear.value = result.REVENUESERVICEYEAR;
            }
        }
    });
};

onBeforeMount(() => {
    loadData();
});
</script>

<template>
    <section>
        <div class="container">
            <el-page-header icon="" v-if="isMobile" class="mb-3" @back="onBack">
                <template #content>
                    <span class="text-large font-600 mr-3">{{ t("Overview") }}</span>
                </template>
                <template #extra>
                    <div class="d-flex flex-row">
                        <el-popover :visible="visible" placement="bottom" :width="300">
                            <div class="d-flex flex-column mb-3">
                                <el-checkbox v-model="checked1" :label="`${title(StatisticTypeEnum.REVENUEDAY)}`" size="large" />
                                <el-checkbox v-model="checked2" :label="`${title(StatisticTypeEnum.REVENUEMONTH)}`" size="large" />
                                <el-checkbox v-model="checked3" :label="`${title(StatisticTypeEnum.REVENUEQUARTER)}`" size="large" />
                                <el-checkbox v-model="checked4" :label="`${title(StatisticTypeEnum.REVENUEYEAR)}`" size="large" />
                                <el-checkbox v-model="checked5" :label="`${title(StatisticTypeEnum.TOTALBOOKINGYEAR)}`" size="large" />
                            </div>
                            <div class="d-flex flex-row align-items-center justify-content-end">
                                <el-button size="small" text @click="visible = false">{{ t('Cancel') }}</el-button>
                                <el-button size="small" type="primary" @click="filter">{{ t('Filter') }}</el-button>
                            </div>
                            <template #reference>
                                <el-button @click="visible = true" type="primary">Tùy chỉnh</el-button>
                            </template>
                        </el-popover>
                    </div>
                </template>
            </el-page-header>
            <div v-else class="row mb-3 d-flex flex-row align-items-center justify-content-between">
                <h3 class="col-12 col-sm-12 col-md-12 col-lg-8 fs-3 mt-1 mb-1">{{ t("Overview") }}</h3>
                <div class="col-12 col-sm-12 col-md-12 col-lg-4 d-flex flex-row-reverse">
                    <el-popover :visible="visible" placement="bottom" :width="300">
                        <div class="d-flex flex-column mb-3">
                            <el-checkbox v-model="checked1" :label="`${title(StatisticTypeEnum.REVENUEDAY)}`" size="large" />
                            <el-checkbox v-model="checked2" :label="`${title(StatisticTypeEnum.REVENUEMONTH)}`" size="large" />
                            <el-checkbox v-model="checked3" :label="`${title(StatisticTypeEnum.REVENUEQUARTER)}`" size="large" />
                            <el-checkbox v-model="checked4" :label="`${title(StatisticTypeEnum.REVENUEYEAR)}`" size="large" />
                            <el-checkbox v-model="checked5" :label="`${title(StatisticTypeEnum.TOTALBOOKINGYEAR)}`" size="large" />
                        </div>
                        <div class="d-flex flex-row align-items-center justify-content-end">
                            <el-button size="small" text @click="visible = false">{{ t('Cancel') }}</el-button>
                            <el-button size="small" type="primary" @click="filter">{{ t('Filter') }}</el-button>
                        </div>
                        <template #reference>
                            <el-button @click="visible = true" type="primary">{{ t('Customize') }}</el-button>
                        </template>
                    </el-popover>
                </div>
            </div>
            <div class="row">
                <div class="col-12 col-sm-12 col-md-12 col-lg-9">
                    <div :class="isMobile ? '' : 'mb-4 mr-4'" :id="isMobile ? '' : 'booking'">
                        <div class="row">
                            <div class="col-12 col-sm-12 col-md-12 col-lg-4 d-flex flex-row">
                                <div class="w-100">
                                    <statistic-card :key="totalBookingDay" :type="StatisticTypeEnum.TOTALBOOKINGDAY" :data="totalBookingDay"></statistic-card>
                                </div>
                                <div class="h-100" id="border" v-if="!isMobile">
                                    <div class="h-100" id="border_content"></div>
                                </div>
                            </div>
                            <div class="col-12 col-sm-12 col-md-12 col-lg-5 d-flex flex-row align-items-center justify-content-around">
                                <div>
                                    <statistic-booking-card :key="totalDetailBookingDay" :type="BookingStatusEnum.PENDING" :value="totalDetailBookingDay.pending"></statistic-booking-card>
                                </div>
                                <div>
                                    <statistic-booking-card :key="totalDetailBookingDay" :type="BookingStatusEnum.SUCCESS" :value="totalDetailBookingDay.success"></statistic-booking-card>
                                </div>
                                <div>
                                    <statistic-booking-card :key="totalDetailBookingDay" :type="BookingStatusEnum.CANCEL" :value="totalDetailBookingDay.cancel"></statistic-booking-card>
                                </div>
                            </div>
                            <div class="col-12 col-sm-12 col-md-12 col-lg-3 d-flex flex-row align-items-center justify-content-end">
                                <div class="h-100 d-flex flex-column justify-content-between" style="padding: 30px;">
                                    <div class="d-flex flex-row justify-content-end">
                                        <span>Chi tiết</span>
                                    </div>
                                    <div class="d-flex flex-column">
                                        <el-button class="mb-2" type="primary">Phê duyệt</el-button>
                                        <el-button class="m-0" type="danger">Từ chối</el-button>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div :class="isMobile ? 'mb-4 mt-4' : 'mb-4 mr-4'" id="booking">
                        <pie-chart :key="revenueServicesYear" :type="StatisticTypeEnum.REVENUESERVICEYEAR" :data="revenueServicesYear"></pie-chart>
                    </div>
                </div>
                <div class="col-12 col-sm-12 col-md-12 col-lg-3">
                    <div class="mb-4" v-if="checked1">
                        <statistic-card :key="revenueDay" :type="StatisticTypeEnum.REVENUEDAY" :data="revenueDay"></statistic-card>
                    </div>
                    <div class="mb-4" v-if="checked2">
                        <statistic-card :key="revenueMonth" :type="StatisticTypeEnum.REVENUEMONTH" :data="revenueMonth"></statistic-card>
                    </div>
                    <div class="mb-4" v-if="checked3">
                        <statistic-card :key="revenueQuarter" :type="StatisticTypeEnum.REVENUEQUARTER" :data="revenueQuarter"></statistic-card>
                    </div>
                    <div class="mb-4" v-if="checked4">
                        <statistic-card :key="revenueYear" :type="StatisticTypeEnum.REVENUEYEAR" :data="revenueYear"></statistic-card>
                    </div>
                    <div class="mb-4" v-if="checked5">
                        <statistic-card :key="totalBookingYear" :type="StatisticTypeEnum.TOTALBOOKINGYEAR" :data="totalBookingYear"></statistic-card>
                    </div>
                </div>
            </div>
        </div>
    </section>
</template>

<style scoped>
</style>
