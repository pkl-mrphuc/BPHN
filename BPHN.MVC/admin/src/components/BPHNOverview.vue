<script setup>
import { useI18n } from "vue-i18n";
import { CaretTop } from "@element-plus/icons-vue";
import { BookingStatusEnum } from "@/const";
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
                    <div class="mr-4">
                        <el-card class="mt-4 mb-4">
                            <div class="row">
                                <el-card class="col-12 col-sm-12 col-md-12 col-lg-4">
                                    <el-statistic :value="98500">
                                        <template #title>
                                            <div class="d-flex flex-row align-items-center">
                                                <h2 class="m-0">Doanh thu tháng 3</h2>
                                            </div>
                                        </template>
                                    </el-statistic>
                                    <div class="statistic-footer">
                                        <div class="footer-item">
                                            <span>than yesterday</span>
                                            <span class="green">24%
                                                <el-icon><CaretTop /></el-icon>
                                            </span>
                                        </div>
                                    </div>
                                </el-card>
                                <el-card class="col-12 col-sm-12 col-md-12 col-lg-6">
                                    <div class="d-flex flex-row align-items-center justify-content-between">
                                        <el-card>
                                            <el-statistic :value="98500">
                                                <template #title>
                                                    <div class="d-flex flex-row align-items-center">
                                                        <h2 class="m-0">{{ t(BookingStatusEnum.PENDING) }}</h2>
                                                    </div>
                                                </template>
                                            </el-statistic>
                                        </el-card>
                                        <el-card>
                                            <el-statistic :value="98500">
                                                <template #title>
                                                    <div class="d-flex flex-row align-items-center">
                                                        <h2 class="m-0">{{ t(BookingStatusEnum.SUCCESS) }}</h2>
                                                    </div>
                                                </template>
                                            </el-statistic>
                                        </el-card>
                                        <el-card>
                                            <el-statistic :value="98500">
                                                <template #title>
                                                    <div class="d-flex flex-row align-items-center">
                                                        <h2 class="m-0">{{ t(BookingStatusEnum.CANCEL) }}</h2>
                                                    </div>
                                                </template>
                                            </el-statistic>
                                        </el-card>
                                    </div>
                                </el-card>
                                <el-card class="col-12 col-sm-12 col-md-12 col-lg-2">
                                    <div class="d-flex flex-row justify-content-end">
                                        <div class="d-flex flex-column">
                                            <el-button text class="m-0">Chi tiết</el-button>
                                            <el-button class="m-0" type="danger">Từ chối</el-button>
                                            <el-button class="m-0" type="primary">Phê duyệt</el-button>
                                        </div>
                                    </div>
                                </el-card>
                            </div>
                        </el-card>
                        <el-card class="mt-4 mb-4">
                            <div class="d-flex flex-row align-items-center justify-content-between mb-3">
                                <div>
                                    <h2 class="m-0">Doanh thu của bạn</h2>
                                    <span>01/03/2025 - 31/03/2025</span>
                                </div>
                                <el-button text class="m-0">Chi tiết</el-button>
                            </div>
                            <div class="row">
                                <div class="col-12 col-sm-12 col-md-12 col-lg-6">
                                    <el-card></el-card>
                                </div>
                                <div class="col-12 col-sm-12 col-md-12 col-lg-6">
                                    <el-card></el-card>
                                </div>
                            </div>
                        </el-card>
                    </div>
                </div>
                <div class="col-12 col-sm-12 col-md-12 col-lg-4">
                    <div class="d-flex flex-column">
                        <div class="ml-4">
                            <el-card class="mt-4 mb-4">
                                <el-statistic :value="98500">
                                    <template #title>
                                        <div class="d-flex flex-row align-items-center">
                                            <h2 class="m-0">Doanh thu tháng 3</h2>
                                        </div>
                                    </template>
                                </el-statistic>
                                <div class="statistic-footer">
                                    <div class="footer-item">
                                        <span>than yesterday</span>
                                        <span class="green">24%
                                            <el-icon><CaretTop /></el-icon>
                                        </span>
                                    </div>
                                </div>
                            </el-card>
                            <el-card class="mt-4 mb-4">
                                <el-statistic :value="98500">
                                    <template #title>
                                        <div class="d-flex flex-row align-items-center">
                                            <h2 class="m-0">Tổng hóa đơn</h2>
                                        </div>
                                    </template>
                                </el-statistic>
                                <div class="statistic-footer">
                                    <div class="footer-item">
                                        <span>than yesterday</span>
                                        <span class="green">24%
                                            <el-icon><CaretTop /></el-icon>
                                        </span>
                                    </div>
                                </div>
                            </el-card>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </section>
</template>

<style scoped>
.el-statistic {
    --el-statistic-content-font-size: 28px;
}

.statistic-card {
    height: 100%;
    padding: 20px;
    border-radius: 4px;
    background-color: var(--el-bg-color-overlay);
}

.statistic-footer {
    display: flex;
    justify-content: space-between;
    align-items: center;
    flex-wrap: wrap;
    font-size: 12px;
    color: var(--el-text-color-regular);
    margin-top: 16px;
}

.statistic-footer .footer-item {
    display: flex;
    justify-content: space-between;
    align-items: center;
}

.statistic-footer .footer-item span:last-child {
    display: inline-flex;
    align-items: center;
    margin-left: 4px;
}

.green {
    color: var(--el-color-success);
}

.red {
    color: var(--el-color-error);
}
</style>
