<script setup>
import StatisticCard from "@/components/StatisticCard.vue";
import { BookingStatusEnum, StatisticTypeEnum } from "@/const";
import { useI18n } from "vue-i18n";
import useCommonFn from "@/commonFn";

const { t } = useI18n();
const { fakeNumber } = useCommonFn();
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
                            <statistic-card :type="StatisticTypeEnum.TOTALBOOKINGYEAR" :time="2025" :value="14999"
                                :pre-value="29343"></statistic-card>
                        </div>
                        <div class="col-12 col-sm-12 col-md-12 col-lg-6">
                            <div class="h-100 d-flex flex-row align-items-center justify-content-between">
                                <div class="d-flex flex-column align-items-center justify-content-center bcard">
                                    <span class="bcard__status">{{ t(BookingStatusEnum.PENDING) }}</span>
                                    <span class="bcard__data">{{ fakeNumber(1000) }}</span>
                                </div>
                                <div class="d-flex flex-column align-items-center justify-content-center bcard">
                                    <span class="bcard__status">{{ t(BookingStatusEnum.SUCCESS) }}</span>
                                    <span class="bcard__data">{{ fakeNumber(1000) }}</span>
                                </div>
                                <div class="d-flex flex-column align-items-center justify-content-center bcard">
                                    <span class="bcard__status">{{ t(BookingStatusEnum.CANCEL) }}</span>
                                    <span class="bcard__data">{{ fakeNumber(1000) }}</span>
                                </div>
                            </div>
                        </div>
                        <div class="col-12 col-sm-12 col-md-12 col-lg-2">
                            <div class="d-flex flex-row justify-content-end">
                                <div class="d-flex flex-column">
                                    <el-button text>Chi tiết</el-button>
                                    <el-button type="danger">Từ chối</el-button>
                                    <el-button type="primary">Phê duyệt</el-button>
                                </div>
                            </div>
                        </div>
                    </div>
                    <!-- <div>
                        <div class="d-flex flex-row align-items-center justify-content-between">
                            <div>
                                <h2 class="m-0">Doanh thu của bạn</h2>
                                <span>01/03/2025 - 31/03/2025</span>
                            </div>
                            <el-button text class="m-0">Chi tiết</el-button>
                        </div>
                        <div class="row">
                            <div class="col-12 col-sm-12 col-md-12 col-lg-6">

                            </div>
                            <div class="col-12 col-sm-12 col-md-12 col-lg-6">

                            </div>
                        </div>
                    </div> -->
                </div>
                <div class="col-12 col-sm-12 col-md-12 col-lg-4">
                    <div class="d-flex flex-column">
                        <statistic-card :type="StatisticTypeEnum.REVENUEMONTH" :time="3" :value="100"
                            :pre-value="1000"></statistic-card>
                        <statistic-card :type="StatisticTypeEnum.REVENUEYEAR" :time="2025" :value="10000"
                            :pre-value="1000"></statistic-card>
                    </div>
                </div>
            </div>
        </div>
    </section>
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
