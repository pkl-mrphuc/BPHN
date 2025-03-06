<script setup>
import { useI18n } from "vue-i18n";

const { t } = useI18n();
</script>

<template>
    <section>
        <div class="container">
            <div class="row mb-3 d-flex flex-row align-items-center justify-content-between">
                <h3 class="fs-3 col-12 col-sm-12 col-md-12 col-lg-8">
                    {{ t("Overview") }}
                </h3>
                <div class="col-12 col-sm-12 col-md-12 col-lg-4 d-flex flex-row">
                </div>
            </div>
            <div>
                <el-row :gutter="16">
                    <el-col :span="8">
                        <div class="statistic-card">
                            <el-statistic :value="98500">
                                <template #title>
                                    <div style="display: inline-flex; align-items: center">
                                        Booking chờ duyệt
                                        <el-tooltip effect="dark"
                                            content=""
                                            placement="top">
                                            <el-icon style="margin-left: 4px" :size="12">
                                                <Warning />
                                            </el-icon>
                                        </el-tooltip>
                                    </div>
                                </template>
                            </el-statistic>
                            <div class="statistic-footer">
                                <div class="footer-item">
                                    <span></span>
                                    <span class="green">
                                        <el-icon>
                                            <CaretTop />
                                        </el-icon>
                                    </span>
                                </div>
                            </div>
                        </div>
                    </el-col>
                    <el-col :span="8">
                        <div class="statistic-card">
                            <el-statistic :value="693700">
                                <template #title>
                                    <div style="display: inline-flex; align-items: center">
                                        Doanh thu hôm nay
                                        <el-tooltip effect="dark"
                                            content=""
                                            placement="top">
                                            <el-icon style="margin-left: 4px" :size="12">
                                                <Warning />
                                            </el-icon>
                                        </el-tooltip>
                                    </div>
                                </template>
                            </el-statistic>
                            <div class="statistic-footer">
                                <div class="footer-item">
                                    <span></span>
                                    <span class="red">
                                        <el-icon>
                                            <CaretBottom />
                                        </el-icon>
                                    </span>
                                </div>
                            </div>
                        </div>
                    </el-col>
                    <el-col :span="8">
                        <div class="statistic-card">
                            <el-statistic :value="72000" title="New transactions today">
                                <template #title>
                                    <div style="display: inline-flex; align-items: center">
                                        Doanh thu tháng 3
                                    </div>
                                </template>
                            </el-statistic>
                            <div class="statistic-footer">
                                <div class="footer-item">
                                    <span></span>
                                    <span class="green">
                                        <el-icon>
                                            <CaretTop />
                                        </el-icon>
                                    </span>
                                </div>
                                <div class="footer-item">
                                    <el-icon :size="14">
                                        <ArrowRight />
                                    </el-icon>
                                </div>
                            </div>
                        </div>
                    </el-col>
                </el-row>
            </div>
        </div>
    </section>
</template>

<style scoped>
:global(h2#card-usage ~ .example .example-showcase) {
    background-color: var(--el-fill-color) !important;
}

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
