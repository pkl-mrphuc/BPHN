<script setup>
import StatisticCard from "@/components/StatisticCard.vue";
import StatisticBookingCard from "@/components/StatisticBookingCard.vue";
import { BookingStatusEnum, StatisticTypeEnum } from "@/const";
import { useI18n } from "vue-i18n";
import { useStore } from "vuex";
import { computed, onBeforeMount, ref } from "vue";
import router from "@/routers";
import useCommonFn from "@/commonFn";

const { t } = useI18n();
const store = useStore();
const { dateToString, quarter } = useCommonFn();
const visible = ref(false);
const now = ref(new Date());
const totalBookingDay = ref({ value: 0, preValue: 0, parameter: now.value });
const totalBookingYear = ref({ value: 0, preValue: 0, parameter: now.value.getFullYear() });
const revenueDay = ref({ value: 0, preValue: 0, parameter: now.value });
const revenueMonth = ref({ value: 0, preValue: 0, parameter: now.value.getMonth() + 1 });
const revenueYear = ref({ value: 0, preValue: 0, parameter: now.value.getFullYear() });
const revenueQuarter = ref({ value: 0, preValue: 0, parameter: quarter(now.value) });
const checked1 = ref(true);
const checked2 = ref(true);
const checked3 = ref(true);
const checked4 = ref(true);
const checked5 = ref(true);

const formatDate = computed(() => {
  return store.getters["config/getFormatDate"];
});

const isMobile = computed(() => {
    return store.getters["config/isMobile"];
});

const onBack = () => {
  router.push("overview");
};

const filter = () => {
    loadData();
};

const loadData = () => {
    let types = [
        {
            name: StatisticTypeEnum.TOTALBOOKINGDAY,
            parameter: totalBookingDay.value.parameter
        }
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
                                <el-checkbox v-model="checked1" :label="`${t(StatisticTypeEnum.REVENUEDAY)} ${dateToString(revenueDay.parameter, formatDate)}`" size="large" />
                                <el-checkbox v-model="checked2" :label="`${t(StatisticTypeEnum.REVENUEMONTH)} ${revenueMonth.parameter}`" size="large" />
                                <el-checkbox v-model="checked3" :label="`${t(StatisticTypeEnum.REVENUEQUARTER)} ${revenueQuarter.parameter}`" size="large" />
                                <el-checkbox v-model="checked4" :label="`${t(StatisticTypeEnum.REVENUEYEAR)} ${revenueYear.parameter}`" size="large" />
                                <el-checkbox v-model="checked5" :label="`${t(StatisticTypeEnum.TOTALBOOKINGYEAR)} ${totalBookingYear.parameter}`" size="large" />
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
                            <el-checkbox v-model="checked1" :label="`${t(StatisticTypeEnum.REVENUEDAY)} ${dateToString(revenueDay.parameter, formatDate)}`" size="large" />
                            <el-checkbox v-model="checked2" :label="`${t(StatisticTypeEnum.REVENUEMONTH)} ${revenueMonth.parameter}`" size="large" />
                            <el-checkbox v-model="checked3" :label="`${t(StatisticTypeEnum.REVENUEQUARTER)} ${revenueQuarter.parameter}`" size="large" />
                            <el-checkbox v-model="checked4" :label="`${t(StatisticTypeEnum.REVENUEYEAR)} ${revenueYear.parameter}`" size="large" />
                            <el-checkbox v-model="checked5" :label="`${t(StatisticTypeEnum.TOTALBOOKINGYEAR)} ${totalBookingYear.parameter}`" size="large" />
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
            </div>
            <div class="row">
                <div class="col-12 col-sm-12 col-md-12 col-lg-9">
                    <div :class="isMobile ? '' : 'mb-4 mr-4'" id="booking">
                        <div class="row">
                            <div class="col-12 col-sm-12 col-md-12 col-lg-3">
                                <statistic-card :key="totalBookingDay" :type="StatisticTypeEnum.TOTALBOOKINGDAY" :data="totalBookingDay"></statistic-card>
                            </div>
                            <div
                                class="col-12 col-sm-12 col-md-12 col-lg-5 d-flex flex-row align-items-center justify-content-around">
                                <div>
                                    <statistic-booking-card :type="BookingStatusEnum.PENDING"
                                        :value="1000"></statistic-booking-card>
                                </div>
                                <div>
                                    <statistic-booking-card :type="BookingStatusEnum.SUCCESS"
                                        :value="1000"></statistic-booking-card>
                                </div>
                                <div>
                                    <statistic-booking-card :type="BookingStatusEnum.CANCEL"
                                        :value="1000"></statistic-booking-card>
                                </div>
                            </div>
                            <div class="col-12 col-sm-12 col-md-12 col-lg-4">

                            </div>
                        </div>
                    </div>
                    <div>
                        <div :class="isMobile ? 'mb-4 mt-4' : 'mb-4 mr-4'"
                            style="height: 360px; background-color: #121212; border-radius: 20px;"></div>
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
html.dark #booking {
    background-color: #121212; 
    border-radius: 20px;
}
html #booking {
    background-color: #f5f5f5; 
    border-radius: 20px;
}
</style>
