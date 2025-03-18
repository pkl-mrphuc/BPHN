<script setup>
import StatisticCard from "@/components/StatisticCard.vue";
import StatisticBookingCard from "@/components/StatisticBookingCard.vue";
import { BookingStatusEnum, StatisticTypeEnum } from "@/const";
import { useI18n } from "vue-i18n";

const { t } = useI18n();
</script>

<template>
    <section>
        <div class="container">
            <div class="row mb-3 d-flex flex-row align-items-center justify-content-between">
                <h3 class="fs-3 col-12 col-sm-12 col-md-12 col-lg-8">{{ t("Overview") }}</h3>
                <div class="col-12 col-sm-12 col-md-12 col-lg-4 d-flex flex-row-reverse">
                    <el-button type="primary">Tùy chỉnh</el-button>
                </div>
            </div>
            <div class="row">
                <div class="col-12 col-sm-12 col-md-12 col-lg-8">
                    <div class="row">
                        <div class="col-12 col-sm-12 col-md-12 col-lg-4">
                            <statistic-card :type="StatisticTypeEnum.TOTALBOOKINGYEAR" :time="2025" :value="14999" :pre-value="29343"></statistic-card>
                        </div>
                        <div class="col-12 col-sm-12 col-md-12 col-lg-8">
                            <statistic-booking-card :type="BookingStatusEnum.PENDING" :value="1000"></statistic-booking-card>
                            <statistic-booking-card :type="BookingStatusEnum.SUCCESS" :value="1000"></statistic-booking-card>
                            <statistic-booking-card :type="BookingStatusEnum.CANCEL" :value="1000"></statistic-booking-card>
                        </div>
                    </div>
                </div>
                <div class="col-12 col-sm-12 col-md-12 col-lg-4">
                    <statistic-card :type="StatisticTypeEnum.REVENUEMONTH" :time="3" :value="100" :pre-value="1000"></statistic-card>
                    <statistic-card :type="StatisticTypeEnum.REVENUEYEAR" :time="2025" :value="10000" :pre-value="1000"></statistic-card>
                </div>
            </div>
        </div>
    </section>
</template>

<style scoped>
</style>
