<template>
  <section>
    <h1 class="fs-36">{{ t("Booking") }}</h1>
    <section>
      <el-alert
        type="warning"
        :closable="false"
        :description="t('BookingStep')"
        class="mb-8"
      />
      <el-alert
        type="warning"
        :closable="false"
        :description="t('OnlyPartner')"
        class="mb-8"
      />
      <el-steps :active="active" finish-status="success" simple>
        <el-step :title="t('Step1')" />
        <el-step :title="t('Step2')" />
        <el-step :title="t('Step3')" />
      </el-steps>
      <div class="p-12">
        <div class="content">
          <div v-if="active == 0">
            <el-autocomplete
              class="wp-100 mb-8"
              v-model="state"
              :fetch-suggestions="querySearch"
              popper-class="my-autocomplete"
              :placeholder="t('FindStadium')"
              @select="handleSelect"
            >
              <template #suffix>
                <el-icon class="el-input__icon">
                  <search />
                </el-icon>
              </template>
              <template #default="{ item }">
                <div class="value">{{ item.name }}</div>
              </template>
            </el-autocomplete>

            <el-table :data="lstStadium" height="250" style="width: 100%">
              <el-table-column prop="name" :label="t('Name')" width="180" />
              <el-table-column prop="address" :label="t('Address')" />
              <el-table-column fixed="right" label="" width="200">
                <template #default="scope">
                  <el-button
                    type="info"
                    size="small"
                    @click="view(scope.row)"
                    >{{ t("ViewDetail") }}</el-button
                  >
                  <el-button
                    type="primary"
                    size="small"
                    @click="choose(scope.row)"
                    >{{ t("Choose") }}</el-button
                  >
                </template>
              </el-table-column>
            </el-table>
            <el-pagination
              class="my-8"
              background
              layout="prev, pager, next"
              v-if="totalRecord != 0"
              v-model:current-page="pageIndex"
              v-model:page-size="pageSize"
              :total="totalRecord"
              @next-click="nextPage"
              @prev-click="prevPage"
              @current-change="currentPage"
            />
          </div>
          <div v-if="active == 1">
            <div class="d-flex justify-content-between align-items-center">
              <span class="fs-36">{{ stadiumName }}</span>
              <div class="ml-auto"></div>
              <el-button @click="prevStep" type="primary" v-if="active != 0">{{
                t("Back")
              }}</el-button>
              <el-button type="primary" @click="today">{{
                t("Today")
              }}</el-button>
              <el-button-group class="ml-8">
                <el-button type="primary" @click="prev">
                  <el-icon><ArrowLeft /></el-icon>
                </el-button>
                <el-button type="primary" @click="next">
                  <el-icon><ArrowRight /></el-icon>
                </el-button>
              </el-button-group>
            </div>
          </div>
          <div v-if="active == 2"></div>
          <div id="calendar" v-show="active == 1" class="wp-100"></div>
        </div>
      </div>
    </section>
  </section>
</template>

<script setup>
import { ref, watch, computed } from "vue";
import { useStore } from "vuex";
import { Calendar } from "@fullcalendar/core";
import timeGridPlugin from "@fullcalendar/timegrid";
import allLocales from "@fullcalendar/core/locales-all";
import { useI18n } from "vue-i18n";

const state = ref("");
const store = useStore();
const language = ref(store.getters["config/getLanguage"]);
const objCalendar = ref(null);
const { t } = useI18n();
const active = ref(0);
const lstStadium = ref([]);
const pageIndex = ref(1);
const pageSize = ref(100);
const totalRecord = ref(0);
const stadiumName = ref("");
const running = ref(0);

const getLanguage = computed(() => {
  return store.getters["config/getLanguage"];
});

watch(getLanguage, (newValue) => {
  language.value = newValue;
  renderCalendar(localStorage.getItem("stadium-id"));
});

const querySearch = (queryString, cb) => {
  store
    .dispatch("stadium/getPaging", {
      pageIndex: pageIndex.value,
      txtSearch: queryString,
    })
    .then((res) => {
      let data = res.data?.data ?? [];
      lstStadium.value = data;
      if (typeof cb === "function") cb(lstStadium.value);
    });
  store
    .dispatch("stadium/getCountPaging", {
      pageIndex: pageIndex.value,
      txtSearch: queryString,
    })
    .then((res) => {
      if (res.data?.data) {
        totalRecord.value = res.data.data.totalAllRecords;
      }
    });
};

const handleSelect = (item) => {
  state.value = item.name;
  lstStadium.value = [item];
};

const today = () => {
  if (running.value > 0) return;
  ++running.value;

  if (objCalendar.value) {
    objCalendar.value.today();
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
  }

  setTimeout(() => {
    running.value = 0;
  }, 1000);
};

const renderCalendar = (stadiumId) => {
  let calendarElement = document.getElementById("calendar");
  if (calendarElement) {
    const calendar = new Calendar(calendarElement, {
      plugins: [timeGridPlugin],
      initialView: "timeGridWeek",
      locales: allLocales,
      locale: language.value,
      headerToolbar: {
        left: "",
        right: "",
      },
      height: "1785px",
      events: async function (data, callback) {
        if (data) {
          let events = await getEventByDate(
            dateToString(data.start, "yyyy-MM-dd"),
            dateToString(data.end, "yyyy-MM-dd"),
            stadiumId
          );
          callback(events);
        }
        callback([]);
      },
      eventContent: function (arg) {
        let eventInfo = arg.event.extendedProps;
        return { domNodes: buildEventInfoHtml(arg.timeText, eventInfo) };
      },
    });
    objCalendar.value = calendar;
    calendar.render();
  }
};

const dateToString = (date, formatDate, hasTime = false) => {
  if (typeof date == "string") {
    date = new Date(date);
  }
  let fullYear = date.getFullYear();
  let month =
    date.getMonth() + 1 < 10 ? `0${date.getMonth() + 1}` : date.getMonth() + 1;
  let day = date.getDate() < 10 ? `0${date.getDate()}` : date.getDate();

  let result = "";
  switch (formatDate) {
    case "yyyy-MM-dd":
      result = `${fullYear}-${month}-${day}`;
      break;
    case "dd-MM-yyyy":
      result = `${day}-${month}-${fullYear}`;
      break;
    default:
      result = `${day}/${month}/${fullYear}`;
      break;
  }

  if (hasTime) {
    let hours = date.getHours() < 10 ? `0${date.getHours()}` : date.getHours();
    let minutes =
      date.getMinutes() < 10 ? `0${date.getMinutes()}` : date.getMinutes();
    let seconds =
      date.getSeconds() < 10 ? `0${date.getSeconds()}` : date.getSeconds();
    return `${result} ${hours}:${minutes}:${seconds}`;
  }
  return result;
};

const getEventByDate = async (start, end, stadiumId) => {
  let result = await store.dispatch("bookingDetail/getByDate", {
    startDate: start,
    endDate: end,
    pitchId: stadiumId,
  });
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
          pitchId: item.pitchId,
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
  eventContainer.style.display = "flex";
  eventContainer.style.flexDirection = "column";
  eventContainer.style.height = "100%";
  eventContainer.style.padding = "5px";
  eventContainer.style.boxSizing = "border-box";
  let html = `<i>${eventInfo.stadium}</i> 
              <div>
                <div style="font-weight: bold; text-overflow: ellipsis; white-space: nowrap; overflow: hidden;" title="${
                  eventInfo.teamA
                }">- ${eventInfo.teamA}</div>
                <div style="font-weight: bold; text-overflow: ellipsis; white-space: nowrap; overflow: hidden;" title="${
                  eventInfo.teamB
                }">- ${!eventInfo.teamB ? "?" : eventInfo.teamB}</div>
              </div>
              <div style="margin-top: auto; font-style: italic; overflow: hidden; text-overflow: ellipsis; white-space: nowrap;" title="${
                eventInfo.note
              }">${eventInfo.note}</div>`;

  eventContainer.innerHTML = html;
  let arrayOfDomNodes = [eventContainer];
  return arrayOfDomNodes;
};

const nextStep = () => {
  active.value++;
};

const prevStep = () => {
  active.value--;
  if (active.value < 0) active.value = 0;
};

const nextPage = () => {
  querySearch(state.value);
};

const prevPage = () => {
  querySearch(state.value);
};

const currentPage = () => {
  querySearch(state.value);
};

const view = (stadium) => {
  alert(stadium);
};

const choose = (stadium) => {
  if (stadium) {
    let stadiumId = localStorage.getItem("stadium-id");
    if (stadiumId) {
      localStorage.removeItem("stadium-id");
    }
    localStorage.setItem("stadium-id", stadium.id);
    stadiumName.value = stadium.name;
    nextStep();
    renderCalendar(stadium.id);
  }
};
</script>
