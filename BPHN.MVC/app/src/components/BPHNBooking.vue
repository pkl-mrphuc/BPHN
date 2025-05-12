<script setup>
import { ArrowLeft, ArrowRight, Search, Filter } from '@element-plus/icons-vue';
import BphnCollapse from '@/components/BPHNCollapse.vue';
</script>

<template>
    <div class="row" style="min-height: 100vh;">
        <div class="col-12 col-sm-12 col-md-2 col-lg-2" style="background: #093D67;">
            <div class="d-flex flex-column text-white">
                <div class="d-flex flex-row align-items-center justify-content-around p-4">
                    <el-button circle :icon="ArrowLeft"></el-button>
                    <span class="fs-1 fw-bold">2025</span>
                    <el-button circle :icon="ArrowRight"></el-button>
                </div>
                <div class="d-flex flex-column">
                    <div class="pointer p-2 pl-5 mb-3 mr-4 fw-bold fs-4 month">
                        January
                    </div>
                    <div class="pointer p-2 pl-5 mb-3 mr-4 fw-bold fs-4 month">
                        February
                    </div>
                    <div class="pointer p-2 pl-5 mb-3 mr-4 fw-bold fs-4 month">
                        March
                    </div>
                    <div class="pointer p-2 pl-5 mb-3 mr-4 fw-bold fs-4 month">
                        April
                    </div>
                    <div class="pointer p-2 pl-5 mb-3 mr-4 fw-bold fs-4 month">
                        May
                    </div>
                    <div class="pointer p-2 pl-5 mb-3 mr-4 fw-bold fs-4 month">
                        June
                    </div>
                    <div class="pointer p-2 pl-5 mb-3 mr-4 fw-bold fs-4 month">
                        July
                    </div>
                    <div class="pointer p-2 pl-5 mb-3 mr-4 fw-bold fs-4 month">
                        August
                    </div>
                    <div class="pointer p-2 pl-5 mb-3 mr-4 fw-bold fs-4 month">
                        September
                    </div>
                    <div class="pointer p-2 pl-5 mb-3 mr-4 fw-bold fs-4 month">
                        October
                    </div>
                    <div class="pointer p-2 pl-5 mb-3 mr-4 fw-bold fs-4 month">
                        November
                    </div>
                    <div class="pointer p-2 pl-5 mb-3 mr-4 fw-bold fs-4 month">
                        December
                    </div>
                </div>
            </div>
        </div>
        <div class="col-12 col-sm-12 col-md-2 col-lg-7 bg-light">
            <el-calendar>
                <template #header="{ date }">
                    <span>{{ date }}</span>
                </template>
                <template #date-cell="{ data }">
                    <p>
                        {{ data.day.split('-').slice(2).join() }}
                    </p>
                </template>
            </el-calendar>
            <div class="d-flex flex-row-reverse">
                <div class="d-flex flex-row align-items-center">
                    <div class="dot bg-white mr-1"></div>
                    EMPTY
                </div>
                <div class="d-flex flex-row align-items-center mx-4">
                    <div class="dot bg-success mr-1"></div>
                    MATCHED
                </div>
                <div class="d-flex flex-row align-items-center">
                    <div class="dot bg-danger mr-1"></div>
                    FIND
                </div>
            </div>
        </div>
        <div class="col-12 col-sm-12 col-md-2 col-lg-3 bg-white">
            <div class="d-flex flex-column p-5">
                <div class="mb-3 d-flex flex-row align-items-center">
                    <el-input placeholder="Search here" size="large" :suffix-icon="Search"></el-input>
                    <el-button class="ml-1" size="large" :icon="Filter"></el-button>
                </div>
                <div>
                    <div class="mb-2">
                        <bphn-collapse></bphn-collapse>
                    </div>
                    <div class="mb-2">
                        <bphn-collapse></bphn-collapse>
                    </div>
                    <div class="mb-2">
                        <bphn-collapse></bphn-collapse>
                    </div>
                </div>
            </div>
        </div>
    </div>

</template>

<style scoped>
.month {
    background: #EAEAEA33;
    border-top-right-radius: 50px;
    border-bottom-right-radius: 50px;
}

.dot {
    width: 12px;
    height: 12px;
    border-radius: 50%;
    border: 0.5px solid #9D9D9D
}

.is-selected {
  color: #1989fa;
}
</style>