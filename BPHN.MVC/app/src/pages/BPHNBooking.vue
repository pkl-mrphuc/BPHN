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
      <el-steps :step="step" finish-status="success" simple>
        <el-step :title="t('Step1')" />
        <el-step :title="t('Step2')" />
        <el-step :title="t('Step3')" />
      </el-steps>
      <div class="p-12">
        <div class="content">
          <div v-if="step == 0">
            <el-alert
              type="warning"
              :closable="false"
              :description="t('OnlyPartner')"
              class="mb-8"
            />
            <el-autocomplete
              class="wp-100 mb-8"
              v-model="key"
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
          <div v-if="step == 1">
            <el-alert
              type="warning"
              :closable="false"
              :description="t('DragDropOnCalendar')"
              class="mb-8"
            />
            <div class="d-flex justify-content-between align-items-center">
              <span class="fs-36">{{ stadiumName }}</span>
              <div class="ml-auto"></div>
              <el-button @click="prevStep" type="primary" v-if="step != 0">{{
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
          <div v-if="step == 2">
            <div class="d-flex justify-content-between">
              <div class="booking-info">
                <h1
                  class="fs-20 d-flex justify-content-center align-items-center"
                >
                  {{ t("Booking") }}
                </h1>
                <el-form-item>
                  <el-col :span="7">
                    <b>{{ t("StadiumName") }}</b>
                  </el-col>
                  <el-col :span="17"> {{ stadiumName }} </el-col>
                </el-form-item>
                <el-form-item>
                  <el-col :span="7">
                    <b>{{ t("BookingDate") }}</b>
                  </el-col>
                  <el-col :span="17">
                    {{ bookingDate }}
                  </el-col>
                </el-form-item>
                <el-form-item>
                  <el-col :span="7">
                    <b>{{ t("MatchDate") }}</b>
                  </el-col>
                  <el-col :span="17">
                    {{ matchDate }}
                  </el-col>
                </el-form-item>
                <el-form-item>
                  <el-col :span="7">
                    <b>{{ t("TimeFrame") }}</b>
                  </el-col>
                  <el-col :span="17"> {{ timeFrameInfo }} </el-col>
                </el-form-item>
                <el-form-item>
                  <el-col :span="7">
                    <b>{{ t("Price") }}</b>
                  </el-col>
                  <el-col :span="17"> {{ price }} </el-col>
                </el-form-item>
              </div>
              <div class="user-info">
                <el-form-item>
                  <el-col>
                    <b>{{ t("PhoneNumber") }} <span class="red">(*)</span></b>
                  </el-col>
                  <el-col>
                    <el-input v-model="phoneNumber" maxlength="255" />
                  </el-col>
                </el-form-item>
                <el-form-item>
                  <el-col>
                    <b>{{ t("Email") }}<span class="red">(*)</span></b>
                  </el-col>
                  <el-col>
                    <el-input v-model="email" maxlength="255" />
                  </el-col>
                </el-form-item>
                <el-form-item>
                  <el-col>
                    <b>{{ t("FootballTeam") }}</b>
                  </el-col>
                  <el-col>
                    <el-input v-model="teamA" maxlength="255" />
                  </el-col>
                </el-form-item>
                <el-form-item>
                  <el-col>
                    <b>{{ t("Note") }}</b>
                  </el-col>
                  <el-col>
                    <el-input
                      v-model="note"
                      maxlength="500"
                      :rows="3"
                      type="textarea"
                    />
                  </el-col>
                </el-form-item>
                <el-form-item>
                  <div class="ml-auto"></div>
                  <el-button @click="prevStep">{{ t("Back") }}</el-button>
                  <el-button type="primary" @click="complete">{{
                    t("Complete")
                  }}</el-button>
                </el-form-item>
              </div>
            </div>
          </div>
          <div id="calendar" v-show="step == 1" class="wp-100"></div>
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

const key = ref("");
const store = useStore();
const language = ref(store.getters["config/getLanguage"]);
const objCalendar = ref(null);
const objStadium = ref(null);
const objBooking = ref(null);
const { t } = useI18n();
const step = ref(0);
const lstStadium = ref([]);
const pageIndex = ref(1);
const pageSize = ref(100);
const totalRecord = ref(0);
const running = ref(0);


const bookingId = ref(null);
const timeFrameInfoId = ref(null);
const stadiumName = ref(null);
const phoneNumber = ref(null);
const bookingDate = ref(null);
const matchDate = ref(null);
const matchDateReal = ref(null);
const email = ref(null);
const note = ref(null);
const teamA = ref(null);
const price = ref(null);
const timeFrameInfo = ref(null);
const pitchId = ref(null);
const weekdays = ref(null);
const accountId = ref(null);

const complete = () => {
  if(!phoneNumber.value) {
    alert(t(""));
    return;
  }
  if(!email.value) {
    alert(t(""));
    return;
  }

  store.dispatch("booking/insertBookingRequest", {
    id: bookingId.value,
    phoneNumber: phoneNumber.value,
    email: email.value,
    isRecurring: false,
    startDate: dateToString(matchDateReal.value, "yyyy-MM-dd"),
    endDate: dateToString(matchDateReal.value, "yyyy-MM-dd"),
    weekendays: weekdays.value,
    timeFrameInfoId: timeFrameInfoId.value,
    pitchId: pitchId.value,
    nameDetail: "Sân 1",
    teamA: teamA.value,
    note: note.value,
    accountId: accountId.value,
    bookingDate: dateToString(objBooking.value.bookingDate, "yyyy-MM-dd")
  }).then((res) => {
    console.log(res);
  })
};

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
              managerId: item.managerId,
              nameDetails: nameDetails(item.nameDetails),
              timeFrameName: propItem.name,
              timeFrameStart: propItem.timeBegin,
              timeFrameEnd: propItem.timeEnd,
              timeFramePrice: propItem.price,
              timeFrameId: propItem.id,
              timeFrameInfos: item.timeFrameInfos
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
  key.value = item.name;
  querySearch(key.value);
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
            stadiumData
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
        let result = validateSelectDateTimeOnCalendar(selectedInfo);
        if (result) {
          openConfirmDialog(selectedInfo, result);
        }
      },
    });
    objCalendar.value = calendar;
    objStadium.value = stadiumData;
    stadiumName.value = stadiumData.name;
    calendar.render();
  }
};

const openConfirmDialog = (selectedInfo, timeFrame) => {
  let selectedDate = dateToString(selectedInfo.end, "dd/MM/yyyy", false, true);
  let selectedStartTime = dateToString(
    timeFrame.timeBegin,
    "dd/MM/yyyy",
    true,
    false
  );
  let selectedEndTime = dateToString(
    timeFrame.timeEnd,
    "dd/MM/yyyy",
    true,
    false
  );
  let selectedTime = `${selectedStartTime} - ${selectedEndTime}`;
  ElMessageBox.confirm(
    t("ConfirmBooking", {
      name: objStadium.value.name,
      time: selectedTime,
      date: selectedDate,
    }),
    "",
    {
      confirmButtonText: t("OK"),
      cancelButtonText: t("Cancel"),
      type: "info",
    }
  )
    .then(() => {
      nextStep();
      store.dispatch("booking/getInstance", "").then((res) => {
        if (res?.data?.data) {
          objBooking.value = res.data.data;
          bookingId.value = objBooking.value.id;
          timeFrameInfoId.value = timeFrame.id;
          stadiumName.value = objStadium.value.name;
          bookingDate.value = dateToString(objBooking.value.bookingDate, "dd/MM/yyyy");
          matchDate.value = selectedDate;
          matchDateReal.value = selectedInfo.end
          price.value = timeFrame.price;
          timeFrameInfo.value = selectedTime;
          pitchId.value = objStadium.value.id;
          weekdays.value = selectedInfo.end.getDay(),
          accountId.value = objStadium.value.managerId
        }
      });
    })
    .catch(() => {});
};

const validateSelectDateTimeOnCalendar = (selectedInfo) => {
  let selectedStart = selectedInfo.start;
  let selectedEnd = selectedInfo.end;
  for (let i = 0; i < objStadium.value.timeFrameInfos.length; i++) {
    const items = objStadium.value.timeFrameInfos[i];
    let startTime = new Date(items.timeBegin);
    let endTime = new Date(items.timeEnd);
    if (selectedStart > endTime) {
      let now = new Date();
      selectedStart = new Date(
        now.getFullYear(),
        now.getMonth(),
        now.getDate(),
        selectedInfo.start.getHours(),
        selectedInfo.start.getMinutes(),
        selectedInfo.start.getSeconds()
      );
      selectedEnd = new Date(
        now.getFullYear(),
        now.getMonth(),
        now.getDate(),
        selectedInfo.start.getHours(),
        selectedInfo.end.getMinutes(),
        selectedInfo.end.getSeconds()
      );
    }

    if (
      (selectedStart >= startTime && selectedStart <= endTime) ||
      (selectedEnd >= startTime && selectedEnd <= endTime) ||
      (selectedStart <= startTime && selectedEnd >= endTime)
    ) {
      return items;
    }
  }
  return null;
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

const getEventByDate = async (start, end, stadiumData) => {
  let result = await store.dispatch("bookingDetail/getByDate", {
    startDate: start,
    endDate: end,
    pitchId: stadiumData.id,
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
  step.value++;
};

const prevStep = () => {
  step.value--;
  if (step.value < 0) step.value = 0;
};

const nextPage = () => {
  querySearch(key.value);
};

const prevPage = () => {
  querySearch(key.value);
};

const currentPage = () => {
  querySearch(key.value);
};

const choose = (stadium) => {
  if (stadium) {
    let stadiumData = localStorage.getItem("stadium-data");
    if (stadiumData) {
      localStorage.removeItem("stadium-data");
    }
    localStorage.setItem("stadium-data", JSON.stringify(stadium));
    nextStep();
    renderCalendar(stadium);
  }
};
</script>

<style scoped>
.booking-info {
  width: 40%;
  border: var(--el-border);
  padding: 30px;
}

.user-info {
  width: 60%;
  padding: 30px;
}
</style>>