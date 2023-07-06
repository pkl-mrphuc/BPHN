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
      <el-steps :active="active" finish-status="success" simple>
        <el-step :title="t('Step1')" />
        <el-step :title="t('Step2')" />
        <el-step :title="t('Step3')" />
      </el-steps>
      <div class="p-12">
        <div class="content">
          <div v-if="active == 0">
            <el-alert
              type="warning"
              :closable="false"
              :description="t('OnlyPartner')"
              class="mb-8"
            />
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

            <el-table
              :data="lstStadium"
              height="250"
              style="width: 100%"
              :span-method="objectSpanMethod"
            >
              <el-table-column prop="name" :label="t('Name')" />
              <el-table-column prop="address" :label="t('Address')" />
              <el-table-column :label="t('NameDetails')">
                <template #default="scope">
                  {{ nameDetails(scope.row.nameDetails) }}
                </template>
              </el-table-column>
              <el-table-column
                prop="timeFrameName"
                :label="t('TimeFrameName')"
              />
              <el-table-column :label="t('TimeFrameStart')">
                <template #default="scope">
                  {{
                    dateToString(
                      scope.row.timeFrameStart,
                      "dd/MM/yyyy",
                      true,
                      false
                    )
                  }}
                </template>
              </el-table-column>
              <el-table-column :label="t('TimeFrameEnd')">
                <template #default="scope">
                  {{
                    dateToString(
                      scope.row.timeFrameEnd,
                      "dd/MM/yyyy",
                      true,
                      false
                    )
                  }}
                </template>
              </el-table-column>
              <el-table-column
                prop="timeFramePrice"
                :label="t('TimeFramePrice')"
              />
              <el-table-column fixed="right" label="">
                <template #default="scope">
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
            <el-alert
              type="warning"
              :closable="false"
              :description="t('DragDropOnCalendar')"
              class="mb-8"
            />
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
import interactionPlugin from "@fullcalendar/interaction";
import allLocales from "@fullcalendar/core/locales-all";
import { useI18n } from "vue-i18n";
import { ElMessageBox } from "element-plus";

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

const objectSpanMethod = ({ row, column, rowIndex, columnIndex }) => {
  console.log(column ? "" : "");
  let lstRow = lstStadium.value.filter((item) => item.id == row.id);
  if (lstRow.length > 1) {
    let curRow = lstRow.filter((item) => item.timeFrameId == row.timeFrameId);
    let curObj = Array.isArray(curRow) && curRow.length > 0 ? curRow[0] : null;
    if (curObj) {
      if (curObj && curObj["firstIndex"] == undefined) {
        for (let i = 0; i < lstRow.length; i++) {
          const item = lstRow[i];
          item["firstIndex"] = rowIndex;
        }
      }
      if (rowIndex < lstRow.length + curObj["firstIndex"]) {
        if (
          columnIndex == 0 ||
          columnIndex == 1 ||
          columnIndex == 2 ||
          columnIndex == 7
        ) {
          if (rowIndex == curObj["firstIndex"]) {
            return {
              rowspan: lstRow.length,
              colspan: 1,
            };
          } else {
            return {
              rowspan: 0,
              colspan: 0,
            };
          }
        }
      }
    }
  }
};

const getLanguage = computed(() => {
  return store.getters["config/getLanguage"];
});
const nameDetails = (nameDetails) => {
  return !nameDetails ? "" : nameDetails.split(";").join("/");
};

watch(getLanguage, (newValue) => {
  language.value = newValue;
  let stadiumJSON = localStorage.getItem("stadium-data");
  if (stadiumJSON) {
    renderCalendar(JSON.parse(stadiumJSON));
  }
});

const querySearch = (queryString, cb) => {
  store
    .dispatch("stadium/getPaging", {
      pageIndex: pageIndex.value,
      txtSearch: queryString,
    })
    .then((res) => {
      lstStadium.value = [];
      let data = res.data?.data ?? [];
      if (typeof cb === "function") cb(data);
      for (let i = 0; i < data.length; i++) {
        const item = data[i];
        if (Array.isArray(item?.timeFrameInfos)) {
          for (let j = 0; j < item.timeFrameInfos.length; j++) {
            const propItem = item.timeFrameInfos[j];
            lstStadium.value.push({
              id: item.id,
              name: item.name,
              address: item.address,
              nameDetails: nameDetails(item.nameDetails),
              timeFrameName: propItem.name,
              timeFrameStart: propItem.timeBegin,
              timeFrameEnd: propItem.timeEnd,
              timeFramePrice: propItem.price,
              timeFrameId: propItem.id,
            });
          }
        }
      }
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
  querySearch(state.value);
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

const renderCalendar = (stadiumData) => {
  let calendarElement = document.getElementById("calendar");
  if (calendarElement) {
    const calendar = new Calendar(calendarElement, {
      selectable: true,
      plugins: [timeGridPlugin, interactionPlugin],
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
            stadiumData.id
          );
          callback(events);
        }
        callback([]);
      },
      eventContent: function (arg) {
        let eventInfo = arg.event.extendedProps;
        return { domNodes: buildEventInfoHtml(arg.timeText, eventInfo) };
      },
      select: function (selectedInfo) {
        if (validateSelectDateTimeOnCalendar(selectedInfo, stadiumData)) {
          openConfirmDialog(selectedInfo, stadiumData);
        }
      },
    });
    objCalendar.value = calendar;
    calendar.render();
  }
};

const openConfirmDialog = (selectedInfo, stadiumData) => {
  let startTime = dateToString(selectedInfo.start, "dd/MM/yyyy", true, false);
  let endTime = dateToString(selectedInfo.end, "dd/MM/yyyy", true, false);
  let selectedTime = `${startTime}-${endTime}`;
  ElMessageBox.confirm(t('ConfirmBooking', { name: stadiumData.name, time: selectedTime }), "", {
    confirmButtonText: t("OK"),
    cancelButtonText: t("Cancel"),
    type: "info",
  })
    .then(() => {
      nextStep();
    })
    .catch(() => {});
};

const validateSelectDateTimeOnCalendar = (selectedInfo, stadiumData) => {
  let startTime = dateToString(selectedInfo.start, "dd/MM/yyyy", true, false);
  let endTime = dateToString(selectedInfo.end, "dd/MM/yyyy", true, false);
  let selectedTime = `${startTime}-${endTime}`;
  if (
    stadiumData &&
    stadiumData.timeFrames.includes(selectedTime) &&
    selectedInfo.start >= new Date()
  ) {
    return true;
  }
  return false;
};

const dateToString = (date, formatDate, hasTime = false, hasDate = true) => {
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
    if (hasDate) {
      return `${result} ${hours}:${minutes}:${seconds}`;
    } else {
      return `${hours}:${minutes}:${seconds}`;
    }
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

const choose = (stadium) => {
  if (stadium) {
    let stadiumData = localStorage.getItem("stadium-data");
    if (stadiumData) {
      localStorage.removeItem("stadium-data");
    }
    let lstTimeFrame = lstStadium.value.filter((item) => item.id == stadium.id);
    let timeFrames = [];
    for (let i = 0; i < lstTimeFrame.length; i++) {
      const item = lstTimeFrame[i];
      let startTime = dateToString(
        item.timeFrameStart,
        "dd/MM/yyyy",
        true,
        false
      );
      let endTime = dateToString(item.timeFrameEnd, "dd/MM/yyyy", true, false);
      let timeFrame = `${startTime}-${endTime}`;
      timeFrames.push(timeFrame);
    }
    stadium.timeFrames = timeFrames;
    localStorage.setItem("stadium-data", JSON.stringify(stadium));
    stadiumName.value = stadium.name;
    nextStep();
    renderCalendar(stadium);
  }
};
</script>
