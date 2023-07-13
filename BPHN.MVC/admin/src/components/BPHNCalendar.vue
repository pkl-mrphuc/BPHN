<script setup>
import {  onMounted, ref } from "vue";
import { Calendar } from "@fullcalendar/core";
import resourceTimeGridPlugin from "@fullcalendar/resource-timegrid";
import allLocales from "@fullcalendar/core/locales-all";
import { useStore } from "vuex";
import useCommonFn from "@/commonFn";
import useToggleModal from "@/register-components/actionDialog";
import { ArrowLeft, ArrowRight } from "@element-plus/icons-vue";
import { useI18n } from "vue-i18n";

const store = useStore();
const lstResource = ref([]);
const { dateToString } = useCommonFn();
const { hasRole, openModal } = useToggleModal();
const objMatch = ref(null);
const objEvent = ref(null);
const objCalendar = ref(null);
const { t } = useI18n();
const running = ref(0);
const formatDate = ref(store.getters["config/getFormatDate"]);
const selectedDate = ref(null);

onMounted(() => {
  store
    .dispatch("pitch/getPaging", {
      accountId: store.getters["account/getAccountId"],
      hasDetail: false,
      hasInactive: false,
    })
    .then(async (res) => {
      let lstPitch = res?.data?.data ?? [];
      lstResource.value = [];
      for (let i = 0; i < lstPitch.length; i++) {
        const item = lstPitch[i];
        lstResource.value.push({
          title: item.name,
          id: item.id,
        });
      }
      await renderCalendar(lstResource.value);
    });
});

const getSelectedDate = () => {
  return objCalendar.value ? dateToString(objCalendar.value.currentData.currentDate, formatDate.value) : "";
};

const renderCalendar = async (lstResource) => {
  if (lstResource?.length > 0) {
    let calendarEl = document.getElementById("calendarTimeGrid");
    let calendar = new Calendar(calendarEl, {
      plugins: [resourceTimeGridPlugin],
      initialView: "resourceTimeGridDay",
      resources: lstResource,
      locales: allLocales,
      locale: store.getters["config/getLanguage"],
      headerToolbar: {
        left: "",
        right: "",
      },
      events: async function (data, callback) {
        if (data) {
          let events = await getEventByDate(
            dateToString(data.start, "yyyy-MM-dd")
          );
          callback(events);
        }
        callback([]);
        handleAfterRenderCalendar(calendarEl);
      },
      eventContent: function (arg) {
        let eventInfo = arg.event.extendedProps;
        return { domNodes: buildEventInfoHtml(arg.timeText, eventInfo) };
      },
      eventClick: function (calEvent) {
        openForm(calEvent);
      },
    });
    calendar.render();
    objCalendar.value = calendar;
    selectedDate.value = getSelectedDate(calendar);
    handleAfterRenderCalendar(calendarEl);
  }
};

const handleAfterRenderCalendar = (calendar) => {
  let licenseElement = calendar.getElementsByClassName("fc-license-message")[0];
  if (licenseElement) {
    licenseElement.style.display = "none";
  }
};

const getEventByDate = async (date) => {
  let result = await store.dispatch("bookingDetail/getByDate", date);
  if (result?.data?.data) {
    let data = result.data.data;
    let lstResult = [];
    for (let i = 0; i < data.length; i++) {
      let item = data[i];
      lstResult.push({
        resourceId: item.pitchId,
        start: item.start,
        end: item.end,
        extendedProps: {
          bookingDetailId: item.id,
          teamA: !item.teamA ? item.phoneNumber : item.teamA,
          teamB: item.teamB ?? "",
          stadium: item.stadium,
          note: item.note ?? "",
        },
      });
    }
    return lstResult;
  }
};

const buildEventInfoHtml = (timeText, eventInfo) => {
  let eventContainer = document.createElement("div");
  if(eventInfo?.teamB) {
    eventContainer.className = "p-2 w-100 h-100 bg-danger";
  }
  else {
    eventContainer.className = "p-2 w-100 h-100 bg-primary";
  }
  let html = `<div class="fs-3 fw-bold">${eventInfo.stadium}</div>`;
  if(eventInfo?.note) {
    html += `<div class="fs-6 fst-italic">${eventInfo.note}</div>`
  }

  eventContainer.innerHTML = html;
  let arrayOfDomNodes = [eventContainer];
  return arrayOfDomNodes;
};

const openForm = (calEvent) => {
  openModal("MatchInfoDialog");
  objEvent.value = calEvent;
  objMatch.value = calEvent.event.extendedProps;
};

const loadEvent = (data) => {
  if (objEvent.value && data) {
    objEvent.value.event.setExtendedProp("teamA", data.teamA);
    objEvent.value.event.setExtendedProp("teamB", data.teamB);
    objEvent.value.event.setExtendedProp("note", data.note);
  }
};

const today = () => {
  if (running.value > 0) return;
  ++running.value;

  if (objCalendar.value) {
    objCalendar.value.today();
    selectedDate.value = getSelectedDate();
  }

  setTimeout(() => {
    running.value = 0;
  }, 1000);
};

const prev = () => {
  if (running.value > 0) return;
  ++running.value;

  if (objCalendar.value) {
    objCalendar.value.prev();
    selectedDate.value = getSelectedDate();
  }

  setTimeout(() => {
    running.value = 0;
  }, 1000);
};

const next = () => {
  if (running.value > 0) return;
  ++running.value;

  if (objCalendar.value) {
    objCalendar.value.next();
    selectedDate.value = getSelectedDate();
  }

  setTimeout(() => {
    running.value = 0;
  }, 1000);
};
</script>

<template>
  <section>
    <div class="container">
      <div class="d-flex flex-row align-items-center justify-content-between">
        <h3 class="fs-3 m-0">{{ selectedDate }}</h3>
        <div>
          <el-button class="mx-1" type="primary" @click="today">{{ t("Today") }}</el-button>
          <el-button-group class="mx-1">
            <el-button type="primary" @click="prev">
              <el-icon><ArrowLeft /></el-icon>
            </el-button>
            <el-button type="primary" @click="next">
              <el-icon><ArrowRight /></el-icon>
            </el-button>
          </el-button-group>
        </div>
      </div>

      <div id="calendarTimeGrid"></div>
    </div>
  </section>
  <MatchInfoDialog
    v-if="hasRole('MatchInfoDialog')"
    :data="objMatch"
    @callback="loadEvent"
  >
  </MatchInfoDialog>
</template>

<style scoped>
#calendarTimeGrid {
  height: calc(100vh - 170px);
}
</style>
