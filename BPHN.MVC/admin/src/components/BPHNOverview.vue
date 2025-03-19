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
                <h3 class="col-12 col-sm-12 col-md-12 col-lg-8 fs-3 mt-1 mb-1">{{ t("Overview") }}</h3>
                <div class="col-12 col-sm-12 col-md-12 col-lg-4 d-flex flex-row-reverse">
                    <el-button type="primary">Tùy chỉnh</el-button>
                </div>
            </div>
            <div class="row">
                <div class="col-12 col-sm-12 col-md-12 col-lg-9">
                    <div class="mb-5 mr-5" id="booking">
                        <div class="row">
                            <div class="col-12 col-sm-12 col-md-12 col-lg-3">
                                <statistic-card :type="StatisticTypeEnum.TOTALBOOKINGYEAR" :time="2025" :value="14999" :pre-value="29343"></statistic-card>
                            </div>
                            <div class="col-12 col-sm-12 col-md-12 col-lg-5 d-flex flex-row align-items-center justify-content-around">
                                <div>
                                    <statistic-booking-card :type="BookingStatusEnum.PENDING" :value="1000"></statistic-booking-card>
                                </div>
                                <div>
                                    <statistic-booking-card :type="BookingStatusEnum.SUCCESS" :value="1000"></statistic-booking-card>
                                </div>
                                <div>
                                    <statistic-booking-card :type="BookingStatusEnum.CANCEL" :value="1000"></statistic-booking-card>
                                </div>
                            </div>
                            <div class="col-12 col-sm-12 col-md-12 col-lg-4">

                            </div>
                        </div>
                    </div>
                    <div>
                        <div class="mb-5 mr-5" style="height: 350px; background-color: #121212; border-radius: 20px;"></div>
                    </div>
                </div>
                <div class="col-12 col-sm-12 col-md-12 col-lg-3">
                    <div class="mb-5">
                        <statistic-card :type="StatisticTypeEnum.REVENUEMONTH" :time="3" :value="100" :pre-value="1000"></statistic-card>
                    </div>
                    <div class="mb-5">
                        <statistic-card :type="StatisticTypeEnum.REVENUEYEAR" :time="2025" :value="10000" :pre-value="1000"></statistic-card>
                    </div>
                    <div class="mb-5">
                        <statistic-card :type="StatisticTypeEnum.REVENUEYEAR" :time="2025" :value="10000" :pre-value="1000"></statistic-card>
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
