<script setup>
import { onMounted, ref, computed } from "vue";
import { useI18n } from "vue-i18n";
import { useStore } from "vuex";
import { Calendar } from "@fullcalendar/core";
import timeGridPlugin from "@fullcalendar/timegrid";
import useCommonFn from "@/commonFn";
import allLocales from "@fullcalendar/core/locales-all";
import {
  InfoFilled,
  ArrowLeft,
  ArrowRight,
  Calendar as DatePick
} from "@element-plus/icons-vue";

const { t } = useI18n();
const store = useStore();
const { time } = useCommonFn();

const pitchId = ref(null);
const lstPitch = ref([]);
const lstFrameInfo = ref([]);
const lstNameDetail = ref([]);
const lstCurrentFrame = ref([]);

const isMobile = computed(() => {
    return store.getters["config/isMobile"];
});

const loadData = () => {
    store.dispatch("pitch/getAll", null)
    .then((res) => {
        if (res?.data?.data) {
            let result = res.data.data;
            lstPitch.value = (result.lstPitch ?? []).map(function(x) { return { id: x.id, name: x.name, minutesPerMatch: x.minutesPerMatch, nameDetails: (x.nameDetails ?? "").split(';') } });
            lstFrameInfo.value = (result.lstFrameInfo ?? []).map(function(x) { return { id: x.id, name: x.name, pitchId: x.pitchId, timeBegin: new Date(x.timeBegin), timeEnd: new Date(x.timeEnd) } });
            if (lstPitch.value.length > 0) {
                pitchId.value = lstPitch.value[0].id;
                handleSelect();
            }
        }
    });
};

const handleSelect = () => {
    lstNameDetail.value = lstPitch.value.find(x => x.id == pitchId.value)?.nameDetails ?? [];
    lstCurrentFrame.value = lstFrameInfo.value.filter(x => x.pitchId == pitchId.value);
    setTimeout(function() {
        for (let i = 0; i < lstNameDetail.value.length; i++) {
            renderCalendar(i);
        }
    }, 100);
};

const renderCalendar = (index) => {
    let calendarEl = document.getElementById(`calendarTimeGrid${index}`);
    if (!calendarEl) return;

    let minTime = Math.min(...lstCurrentFrame.value.map(x => { return x.timeBegin }));
    let maxTime = Math.max(...lstCurrentFrame.value.map(x => { return x.timeEnd }));
    let calendar = new Calendar(calendarEl, {
        plugins: [timeGridPlugin],
        initialView: "timeGridWeek",
        slotMinTime: time(new Date(minTime)),
        slotMaxTime: time(new Date(maxTime)),
        slotDuration: "00:30:00",
        height: "700px",
        headerToolbar: false,
        locales: allLocales,
        locale: store.getters["config/getLanguage"],
    });
    calendar.render();
};

onMounted(() => {
    loadData();
});
</script>

<template>
    <section>
        <div class="container">
            <el-page-header v-if="isMobile" class="mb-3" @back="onBack">
                <template #content>
                    <span class="text-large font-600 mr-3">{{ t("MyCalendar") }}</span>
                </template>
                <template #extra>
                    <div class="flex items-center">
                        
                    </div>
                </template>
            </el-page-header>
            <div v-else class="row mb-3 d-flex flex-row align-items-center justify-content-between">
                <div class="col-12 col-sm-12 col-md-12 col-lg-8">
                    <div class="d-flex flex-row align-items-center">
                        <h3 class="fs-3 mr-3 mb-1 mt-1">{{ t("MyCalendar") }}</h3>
                        <el-select style="width: 150px;" :no-data-text="t('NoData')" :placeholder="t('Infrastructure')"
                            v-model="pitchId" @change="handleSelect">
                            <el-option v-for="item in lstPitch" :key="item.id" :label="item.name" :value="item.id" />
                        </el-select>
                    </div>
                </div>
                <div class="col-12 col-sm-12 col-md-12 col-lg-4 d-flex flex-row-reverse">
                    <el-button-group class="mr-1">
                        <el-button type="primary" @click="prev">
                            <el-icon>
                                <ArrowLeft />
                            </el-icon>
                        </el-button>
                        <el-button type="primary" @click="next">
                            <el-icon>
                                <ArrowRight />
                            </el-icon>
                        </el-button>
                    </el-button-group>
                    <el-button class="mr-1" type="primary" @click="today">{{ t("Today") }}</el-button>
                    <div>
                        <el-popover placement="top-start" width="600" trigger="click">
                            <template #reference>
                                <el-button class="mr-1" type="secondary" :icon="DatePick" circle />
                            </template>
                            <div class="row">
                                <div class="col-12 col-sm-12 col-md-12 col-lg-12 col-xl-12">
                                    <el-calendar v-model="currentDate">
                                        <template #header="{ }">
                                            <span></span>
                                        </template>
                                    </el-calendar>
                                </div>
                            </div>
                        </el-popover>
                    </div>
                    <div>
                        <el-popover placement="top-start" :title="t('Note')" width="250" trigger="click">
                            <template #reference>
                                <el-button class="mr-1" type="warning" :icon="InfoFilled" circle />
                            </template>
                            <div class="row mb-3 d-flex flex-row align-items-center">
                                <div class="col-3 square bg-info"></div>
                                <div class="col-9">
                                    <div class="mx-3">{{ t("PENDING") }}</div>
                                </div>
                            </div>
                            <div class="row mb-3 d-flex flex-row align-items-center">
                                <div class="col-3 square bg-danger"></div>
                                <div class="col-9">
                                    <div class="mx-3">{{ t("HasCompetitor") }}</div>
                                </div>
                            </div>
                            <div class="row mb-3 d-flex flex-row align-items-center">
                                <div class="col-3 square bg-primary"></div>
                                <div class="col-9">
                                    <div class="mx-3">{{ t("HasNotCompetitor") }}</div>
                                </div>
                            </div>
                        </el-popover>
                    </div>
                </div>
            </div>
            <div class="row">
                <div class="col-12 col-sm-12 col-md-12 col-lg-12 col-xl-12">
                    <div class="row">
                        <div class="col-12 col-sm-12 col-md-12 col-lg-6" v-for="(item, index) in lstNameDetail"
                            :key="item" :label="item" :value="item">
                            <h2 class="m-0 mb-2">{{ item }}</h2>
                            <section :class="(!isMobile && index % 2 == 0 ? 'mb-5 mr-5' : 'mb-5')"
                                :id="`calendarTimeGrid${index}`"></section>
                        </div>
                    </div>
                </div>
            </div>
        </div>
        <el-empty :description="t('NoData')" v-if="lstPitch.length == 0" />
    </section>
</template>

<style scoped>
.square {
  width: 15px;
  height: 15px;
  border-radius: 3px;
}
</style>
