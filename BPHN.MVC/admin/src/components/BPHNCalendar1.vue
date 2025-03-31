<script setup>
import { onMounted, ref, watch, watchEffect } from "vue";
import { useI18n } from "vue-i18n";
import { useStore } from "vuex";
import { Calendar } from "@fullcalendar/core";
import timeGridPlugin from "@fullcalendar/timegrid";
import useCommonFn from "@/commonFn";
import allLocales from "@fullcalendar/core/locales-all";
import useToggleModal from "@/register-components/actionDialog";
import {
  InfoFilled,
  ArrowLeft,
  ArrowRight,
  Calendar as DatePick
} from "@element-plus/icons-vue";

const { t } = useI18n();
const { time, dateToString, equals } = useCommonFn();
const { hasRole, openModal } = useToggleModal();
const store = useStore();
const isMobile = ref(store.getters["config/isMobile"]);

const currentDate = ref(new Date());
const objEvent = ref(null);
const objMatch = ref(null);
const pitchId = ref(null);
const lstPitch = ref([]);
const lstFrameInfo = ref([]);
const lstNameDetail = ref([]);
const lstCurrentFrame = ref([]);
const calendarManager = ref([]);

watchEffect(() => { pitchId.value = store.getters["cache/getCalendarVariableCache"]?.pitchId ?? null; });

const loadData = () => {
    store.dispatch("pitch/getAll", { onlyActive: true })
    .then((res) => {
        if (res?.data?.data) {
            let result = res.data.data;
            lstPitch.value = (result.lstPitch ?? []).map(function(x) { return { id: x.id, name: x.name, minutesPerMatch: x.minutesPerMatch, nameDetails: (x.nameDetails ?? "").split(';') } });
            lstFrameInfo.value = (result.lstFrameInfo ?? []).map(function(x) { return { id: x.id, name: `${time(new Date(x.timeBegin))} - ${time(new Date(x.timeEnd))}`, pitchId: x.pitchId, timeBegin: new Date(x.timeBegin), timeEnd: new Date(x.timeEnd) } });
            if (lstPitch.value.length > 0) {
                pitchId.value = lstPitch.value[0].id;
                handleSelect();
            }
        }
    });
};

const handleSelect = () => {
    store.commit("cache/setCalendarVariableCache", 
    { 
        pitchId: pitchId.value 
    });
    lstNameDetail.value = lstPitch.value.find(x => x.id == pitchId.value)?.nameDetails ?? [];
    lstCurrentFrame.value = lstFrameInfo.value.filter(x => x.pitchId == pitchId.value);
    calendarManager.value = [];
    setTimeout(function() {
        for (let i = 0; i < lstNameDetail.value.length; i++) {
            calendarManager.value[lstNameDetail.value[i]] = null;
            renderCalendar(i);
        }
    }, 100);
};

const renderCalendar = (index) => {
    let calendarEl = document.getElementById(`calendarTimeGrid${index}`);
    if (!calendarEl) return;

    let minTime = Math.min(...lstCurrentFrame.value.map(x => { return x.timeBegin }));
    let maxTime = Math.max(...lstCurrentFrame.value.map(x => { return x.timeEnd }));

    let options = {
        plugins: [timeGridPlugin],
        initialView: "timeGridWeek",
        slotMinTime: time(new Date(minTime)),
        slotMaxTime: time(new Date(maxTime)),
        slotDuration: "00:30:00",
        height: "auto",
        headerToolbar: false,
        locales: allLocales,
        locale: store.getters["config/getLanguage"],
        events: async function (data, callback) {
            if (data) {
                let events = await getEventByDate(lstNameDetail.value[index], dateToString(data.start, "yyyy-MM-dd"), dateToString(data.end, "yyyy-MM-dd"));
                callback(events);
            } else {
                callback([]);
            }
        },
        eventClick: function (calEvent) {
            openForm(calEvent);
        },
        eventContent: function (arg) {
            let info = arg.event.extendedProps;
            
            let bgClass = 'border-info';
            if (info.teamA && info.teamB) bgClass = 'border-danger';
            else if (info.teamA) bgClass = 'border-primary';

            let eventHtml = `
                <div class="bphn-event ${bgClass}">
                    <div class="bphn-event__name" title="${info.teamA}">${info.teamA}</div>
                </div>
            `;

            return { domNodes: [createElementFromHTML(eventHtml)] };
        }
    };
    if (isMobile.value) {
        options.initialView = "timeGridThreeDay";
        options.views = {
            timeGridThreeDay: {
                type: 'timeGrid',
                duration: { days: 3 }
            }
        }
    }
    let calendar = new Calendar(calendarEl, options);
    calendarManager.value[lstNameDetail.value[index]] = calendar;
    calendar.render();
};

const createElementFromHTML = (htmlString) => {
    let div = document.createElement("div");
    div.innerHTML = htmlString.trim();
    return div.firstChild;
}

const getEventByDate = async (nameDetail, start, end) => {
    let result = await store.dispatch("bookingDetail/getByRangeDate", { startDate: start, endDate: end, nameDetail: nameDetail, pitchId: pitchId.value });
    if (result?.data?.data) {
        let data = result.data.data;
        let lstResult = [];
        for (let i = 0; i < data.length; i++) {
            let item = data[i];
            lstResult.push({
                start: item.start,
                end: item.end,
                extendedProps: {
                    bookingDetailId: item.id,
                    bookingId: item.bookingId,
                    teamA: !item.teamA ? item.phoneNumber : item.teamA,
                    teamB: item.teamB ?? "",
                    stadium: item.stadium,
                    note: item.note ?? "",
                    status: item.status ?? "",
                    deposit: item.deposit ?? 0
                },
            });
        }
        return lstResult;
    }
};

const openForm = (calEvent) => {
    objEvent.value = calEvent;
    objMatch.value = calEvent.event.extendedProps;
    openModal("MatchInfoDialog");
};

const loadEvent = (data) => {
    if (objEvent.value && data) {
        objEvent.value.event.setExtendedProp("teamA", data.teamA);
        objEvent.value.event.setExtendedProp("teamB", data.teamB);
        objEvent.value.event.setExtendedProp("note", data.note);
        objEvent.value.event.setExtendedProp("deposit", data.deposit);
        objEvent.value.event.setExtendedProp("status", data.status);
    }
};

const today = () => {
    for (let i = 0; i < lstNameDetail.value.length; i++) {
        if (calendarManager.value[lstNameDetail.value[i]] && !equals(dateToString(new Date()), dateToString(calendarManager.value[lstNameDetail.value[i]].getDate()))) {
            calendarManager.value[lstNameDetail.value[i]].today();
            currentDate.value = calendarManager.value[lstNameDetail.value[i]].getDate();
        }
    }
};

const prev = () => {
    for (let i = 0; i < lstNameDetail.value.length; i++) {
        if (calendarManager.value[lstNameDetail.value[i]]) {
            calendarManager.value[lstNameDetail.value[i]].prev();
            currentDate.value = calendarManager.value[lstNameDetail.value[i]].getDate();
        }
    }
};

const next = () => {
    for (let i = 0; i < lstNameDetail.value.length; i++) {
        if (calendarManager.value[lstNameDetail.value[i]]) {
            calendarManager.value[lstNameDetail.value[i]].next();
            currentDate.value = calendarManager.value[lstNameDetail.value[i]].getDate();
        }
    }
};

watch(currentDate, (newValue) => {
    for (let i = 0; i < lstNameDetail.value.length; i++) {
        if (calendarManager.value[lstNameDetail.value[i]] && !equals(dateToString(newValue), dateToString(calendarManager.value[lstNameDetail.value[i]].getDate()))) {
            calendarManager.value[lstNameDetail.value[i]].gotoDate(newValue);
        }
    }
});

onMounted(() => {
    loadData();
});
</script>

<template>
    <section>
        <div class="container">
            <el-page-header icon="" v-if="isMobile" class="mb-3" @back="onBack">
                <template #content>
                    <span class="text-large font-600 mr-3">{{ t("MyCalendar") }}</span>
                </template>
                <template #extra>
                    <div class="d-flex flex-row">
                        <el-select style="width: 150px;" :no-data-text="t('NoData')" :placeholder="t('Infrastructure')" v-model="pitchId" @change="handleSelect">
                            <el-option v-for="item in lstPitch" :key="item.id" :label="item.name" :value="item.id" />
                        </el-select>
                    </div>
                </template>
            </el-page-header>
            <div v-else class="row mb-3 d-flex flex-row align-items-center justify-content-between">
                <div class="col-12 col-sm-12 col-md-12 col-lg-8">
                    <div class="d-flex flex-row align-items-center">
                        <h3 class="fs-3 mr-3 mb-1 mt-1">{{ t("MyCalendar") }}</h3>
                        <el-select style="width: 150px;" :no-data-text="t('NoData')" :placeholder="t('Infrastructure')" v-model="pitchId" @change="handleSelect">
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
    <MatchInfoDialog v-if="hasRole('MatchInfoDialog')" :data="objMatch" @callback="loadEvent"></MatchInfoDialog>
</template>

<style scoped>
</style>
