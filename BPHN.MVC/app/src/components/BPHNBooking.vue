<script setup>
import { ArrowLeft, ArrowRight, Search, Filter } from '@element-plus/icons-vue';
import BphnCollapse from '@/components/BPHNCollapse.vue';
import { onMounted, reactive, ref } from 'vue';
import { MonthEnum } from '@/const';

const stadiums = reactive([]);
const currentDate = ref(new Date());
const today = ref(new Date(currentDate.value.getFullYear(), currentDate.value.getMonth(), currentDate.value.getDate(), 0, 0, 0));
const year = ref(currentDate.value.getFullYear());
const month = ref(currentDate.value.getMonth());
const day = ref(currentDate.value.getDate());
const selectedDate = ref(new Date(year.value, month.value, day.value));

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

const handleMonthSelect = (item) => {
    month.value = item;
    selectedDate.value = new Date(year.value, month.value, day.value);
};

const handleChangeYear = (item) => {
    year.value += item;
    selectedDate.value = new Date(year.value, month.value, day.value);
};

const handleCalendarChange = (item) => {
    selectedDate.value = item;
    year.value = item.getFullYear();
    month.value = item.getMonth();
    day.value = item.getDate();
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
                    <el-button circle :icon="ArrowLeft" @click="handleChangeYear(-1)"></el-button>
                    <span class="booking__year fs-1 fw-bold">{{ year }}</span>
                    <el-button circle :icon="ArrowRight" @click="handleChangeYear(+1)"></el-button>
                </div>
                <div class="booking__months d-flex flex-column">
                    <div v-for="(item, index) in MonthEnum" :key="item"
                        class="booking__month-item pointer p-2 pl-5 mb-3 mr-4 fw-bold fs-4"
                        :class="{ 'booking__month-item--active': month === index }" @click="handleMonthSelect(index)">
                        {{ item }}
                    </div>
                </div>
            </div>
        </div>
        <div class="booking__calendar col-12 col-sm-12 col-md-2 col-lg-7 bg-light p-4">
            <el-calendar v-model="selectedDate" @update:model-value="handleCalendarChange">
                <template #header="{ date }">
                    <span class="fs-3 fw-bold">{{ date }}</span>
                </template>
                <template #date-cell="{ data }">
                    <div class="booking__calendar-cell">
                        <p class="booking__calendar-cell-day" >{{ data.day.split('-').slice(2).join() }}</p>
                        <div v-if="new Date(data.day) >= today" class="booking__calendar-cell-status booking__legend-dot booking__legend-dot--success"></div>
                    </div>
                </template>
            </el-calendar>
            <div class="booking__legend d-flex flex-row-reverse">
                <div class="booking__legend-item d-flex flex-row align-items-center">
                    <div class="booking__legend-dot booking__legend-dot--success mr-1"></div>
                    AVAILABILITY
                </div>
                <div class="booking__legend-item d-flex flex-row align-items-center mx-4">
                    <div class="booking__legend-dot booking__legend-dot--danger mr-1"></div>
                    FULLY
                </div>
            </div>
        </div>
        <div class="booking__stadiums col-12 col-sm-12 col-md-2 col-lg-3 bg-white">
            <div class="booking__search d-flex flex-column">
                <div class="mb-3 d-flex flex-row align-items-center">
                    <el-input placeholder="Tìm sân trống theo tên, địa chỉ" size="large"
                        :suffix-icon="Search"></el-input>
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

.booking__month-item--active {
    background: #EAEAEA;
    color: #555555;
;
}

.booking__legend-dot {
    width: 12px;
    height: 12px;
    border-radius: 50%;
    border: 0.5px solid #9D9D9D
}

.booking__legend-dot--success {
    background: #67c23a;
}

.booking__legend-dot--danger {
    background: #f56c6c;
}

.booking__search {
    padding: 40px;
}

.booking__stadiums-list {
    overflow-y: auto;
    height: calc(100vh - 196px);
}

.booking__calendar-cell {
    width: 100%;
    height: 100%;
    box-sizing: border-box;
    position: relative;
}

.booking__calendar-cell-day {
    font-size: 16px;
}

.booking__calendar-cell-status {
    position: absolute;
    bottom: 12px;
    right: 0;
}
</style>