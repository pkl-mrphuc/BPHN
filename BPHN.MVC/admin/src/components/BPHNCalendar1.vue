<script setup>
import { onMounted, ref } from "vue";
import { useI18n } from "vue-i18n";
import { useStore } from "vuex";
import { Calendar } from "@fullcalendar/core";
import timeGridPlugin from "@fullcalendar/timegrid";
import allLocales from "@fullcalendar/core/locales-all";
import useCommonFn from "@/commonFn";

const { t } = useI18n();
const store = useStore();
const { time } = useCommonFn();

const pitchId = ref(null);
const lstPitch = ref([]);
const lstFrameInfo = ref([]);
const lstNameDetail = ref([]);
const lstCurrentFrame = ref([]);
const minutesPerMatch = ref(30);

const loadData = () => {
    store.dispatch("pitch/getAll", null)
    .then((res) => {
        if (res?.data?.data) {
            let result = res.data.data;
            lstPitch.value = (result.lstPitch ?? []).map(function(x) { return { id: x.id, name: x.name, minutesPerMatch: x.minutesPerMatch, nameDetails: (x.nameDetails ?? "").split(';') } });
            lstFrameInfo.value = (result.lstFrameInfo ?? []).map(function(x) { return { id: x.id, name: x.name, pitchId: x.pitchId, timeBegin: new Date(x.timeBegin), timeEnd: new Date(x.timeEnd) } });
            if (lstPitch.value.length > 0) {
                pitchId.value = lstPitch.value[0].id;
                minutesPerMatch.value = lstPitch.value[0].minutesPerMatch;
                handleSelect();
            }
        }
    });
};

const handleSelect = () => {
    minutesPerMatch.value = lstPitch.value.find(x => x.id == pitchId.value)?.minutesPerMatch ?? 30;
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
        height: "500px",
        locales: allLocales,
        locale: store.getters["config/getLanguage"],
        headerToolbar: {
            left: "",
            right: "",
        }
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
      <div class="row mb-3 d-flex flex-row align-items-center justify-content-between">
        <h3 class="col-12 col-sm-12 col-md-12 col-lg-8 fs-3 mt-1 mb-1">{{ t("CalendarForDate") }}</h3>
        <div class="col-12 col-sm-12 col-md-12 col-lg-4 d-flex flex-row-reverse">
            <el-select class="w-100" v-model="pitchId" @change="handleSelect">
                <el-option v-for="item in lstPitch" :key="item.id" :label="item.name" :value="item.id" />
            </el-select>
        </div>
      </div>
      <div class="row">
        <div class="col-12 col-sm-12 col-md-12 col-lg-12 col-xl-12">
            <div :id="`calendarTimeGrid${index}`" v-for="(item, index) in lstNameDetail" :key="item" :label="item" :value="item"></div>
        </div>
      </div>
    </div>
  </section>
</template>

<style scoped>
</style>
