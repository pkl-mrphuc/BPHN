<script setup>
import { ArrowLeft, ArrowRight, Search, Filter } from '@element-plus/icons-vue';
import BphnCollapse from '@/components/BPHNCollapse.vue';
import { onMounted, reactive } from 'vue';

const stadiums = reactive([]);

const loadData = () => {
    Object.assign(stadiums, 
    [
        {
            name: 'Stadium A',
            details: [
                { title: 'Field 1', description: '7-a-side field' },
                { title: 'Field 2', description: '11-a-side field' }
            ]
        },
        {
            name: 'Stadium B',
            details: [
                { title: 'Field 1', description: '5-a-side field' },
                { title: 'Field 2', description: '7-a-side field' },
                { title: 'Field 3', description: '11-a-side field' }
            ]
        },
        {
            name: 'Stadium C',
            details: [
                { title: 'Field 1', description: 'Indoor field' },
                { title: 'Field 2', description: 'Outdoor field' }
            ]
        },
        {
            name: 'Stadium C',
            details: [
                { title: 'Field 1', description: 'Indoor field' },
                { title: 'Field 2', description: 'Outdoor field' }
            ]
        },
        {
            name: 'Stadium C',
            details: [
                { title: 'Field 1', description: 'Indoor field' },
                { title: 'Field 2', description: 'Outdoor field' }
            ]
        },
        {
            name: 'Stadium A',
            details: [
                { title: 'Field 1', description: '7-a-side field' },
                { title: 'Field 2', description: '11-a-side field' }
            ]
        },
        {
            name: 'Stadium B',
            details: [
                { title: 'Field 1', description: '5-a-side field' },
                { title: 'Field 2', description: '7-a-side field' },
                { title: 'Field 3', description: '11-a-side field' }
            ]
        },
        {
            name: 'Stadium C',
            details: [
                { title: 'Field 1', description: 'Indoor field' },
                { title: 'Field 2', description: 'Outdoor field' }
            ]
        },
        {
            name: 'Stadium C',
            details: [
                { title: 'Field 1', description: 'Indoor field' },
                { title: 'Field 2', description: 'Outdoor field' }
            ]
        },
        {
            name: 'Stadium C',
            details: [
                { title: 'Field 1', description: 'Indoor field' },
                { title: 'Field 2', description: 'Outdoor field' }
            ]
        },
        {
            name: 'Stadium A',
            details: [
                { title: 'Field 1', description: '7-a-side field' },
                { title: 'Field 2', description: '11-a-side field' }
            ]
        },
        {
            name: 'Stadium B',
            details: [
                { title: 'Field 1', description: '5-a-side field' },
                { title: 'Field 2', description: '7-a-side field' },
                { title: 'Field 3', description: '11-a-side field' }
            ]
        },
        {
            name: 'Stadium C',
            details: [
                { title: 'Field 1', description: 'Indoor field' },
                { title: 'Field 2', description: 'Outdoor field' }
            ]
        },
        {
            name: 'Stadium C',
            details: [
                { title: 'Field 1', description: 'Indoor field' },
                { title: 'Field 2', description: 'Outdoor field' }
            ]
        },
        {
            name: 'Stadium C',
            details: [
                { title: 'Field 1', description: 'Indoor field' },
                { title: 'Field 2', description: 'Outdoor field' }
            ]
        }
    ]);
};

onMounted(() => {
    loadData();
});
</script>

<template>
    <div class="booking row">
        <div class="booking__sidebar col-12 col-sm-12 col-md-2 col-lg-2">
            <div class="booking__sidebar-content d-flex flex-column text-white">
                <div class="booking__year-selector d-flex flex-row align-items-center justify-content-around p-4">
                    <el-button circle :icon="ArrowLeft"></el-button>
                    <span class="booking__year fs-1 fw-bold">2025</span>
                    <el-button circle :icon="ArrowRight"></el-button>
                </div>
                <div class="booking__months d-flex flex-column">
                    <div class="booking__month-item pointer p-2 pl-5 mb-3 mr-4 fw-bold fs-4">
                        January
                    </div>
                    <div class="booking__month-item pointer p-2 pl-5 mb-3 mr-4 fw-bold fs-4">
                        February
                    </div>
                    <div class="booking__month-item pointer p-2 pl-5 mb-3 mr-4 fw-bold fs-4">
                        March
                    </div>
                    <div class="booking__month-item pointer p-2 pl-5 mb-3 mr-4 fw-bold fs-4">
                        April
                    </div>
                    <div class="booking__month-item pointer p-2 pl-5 mb-3 mr-4 fw-bold fs-4">
                        May
                    </div>
                    <div class="booking__month-item pointer p-2 pl-5 mb-3 mr-4 fw-bold fs-4">
                        June
                    </div>
                    <div class="booking__month-item pointer p-2 pl-5 mb-3 mr-4 fw-bold fs-4">
                        July
                    </div>
                    <div class="booking__month-item pointer p-2 pl-5 mb-3 mr-4 fw-bold fs-4">
                        August
                    </div>
                    <div class="booking__month-item pointer p-2 pl-5 mb-3 mr-4 fw-bold fs-4">
                        September
                    </div>
                    <div class="booking__month-item pointer p-2 pl-5 mb-3 mr-4 fw-bold fs-4">
                        October
                    </div>
                    <div class="booking__month-item pointer p-2 pl-5 mb-3 mr-4 fw-bold fs-4">
                        November
                    </div>
                    <div class="booking__month-item pointer p-2 pl-5 mb-3 mr-4 fw-bold fs-4">
                        December
                    </div>
                </div>
            </div>
        </div>
        <div class="booking__calendar col-12 col-sm-12 col-md-2 col-lg-7 bg-light">
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
            <div class="booking__legend d-flex flex-row-reverse">
                <div class="booking__legend-item d-flex flex-row align-items-center">
                    <div class="booking__legend-dot bg-white mr-1"></div>
                    EMPTY
                </div>
                <div class="booking__legend-item d-flex flex-row align-items-center mx-4">
                    <div class="booking__legend-dot bg-success mr-1"></div>
                    MATCHED
                </div>
                <div class="booking__legend-item d-flex flex-row align-items-center">
                    <div class="booking__legend-dot bg-danger mr-1"></div>
                    FIND
                </div>
            </div>
        </div>
        <div class="booking__stadiums col-12 col-sm-12 col-md-2 col-lg-3 bg-white">
            <div class="booking__search d-flex flex-column">
                <div class="mb-3 d-flex flex-row align-items-center">
                    <el-input placeholder="Tìm sân trống theo tên, địa chỉ" size="large" :suffix-icon="Search"></el-input>
                    <el-button class="booking__filter-btn ml-1" size="large" :icon="Filter"></el-button>
                </div>
                <div class="booking__stadiums-list">
                    <div class="mb-2" v-for="item in stadiums" :key="item">
                        <bphn-collapse :title="item.name" :items="item.details"></bphn-collapse>
                    </div>
                </div>
            </div>
        </div>
    </div>

</template>

<style scoped>
.booking {
    min-height: calc(100vh - 60px);
}

.booking__sidebar {
    background: #093D67;
}

.booking__month-item {
    background: #EAEAEA33;
    border-top-right-radius: 50px;
    border-bottom-right-radius: 50px;
}

.booking__legend-dot {
    width: 12px;
    height: 12px;
    border-radius: 50%;
    border: 0.5px solid #9D9D9D
}

.booking__search {
    padding: 40px;;
}

.booking__stadiums-list {
    overflow-y: auto; 
    height: calc(100vh - 196px);
}
</style>