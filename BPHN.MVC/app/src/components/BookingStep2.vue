<template>
  <div>
    <el-alert
      type="warning"
      :closable="false"
      :description="t('DragDropOnCalendar')"
      class="mb-2"
    />
    <div class="d-flex flex-row justify-content-between align-items-center">
      <span class="fs-2">{{ stadiumName }}</span>
      <el-dropdown
        @command="chooseNameDetail"
        class="ml-2"
        style="margin-bottom: -20px"
      >
        <span class="el-dropdown-link d-flex flex-row align-items-center">
          {{ !nameDetail ? t("All") : nameDetail }}
          <el-icon class="el-icon--right">
            <arrow-down />
          </el-icon>
        </span>
        <template #dropdown>
          <el-dropdown-menu>
            <el-dropdown-item
              v-for="item in options"
              :key="item"
              :command="item"
              >{{ !item ? t("All") : item }}</el-dropdown-item
            >
          </el-dropdown-menu>
        </template>
      </el-dropdown>
      <div class="ml-auto"></div>
      <el-button @click="prevStep">{{ t("Back") }}</el-button>
      <el-button type="primary" @click="today" class="ml-2">{{ t("Today") }}</el-button>
      <el-button-group class="ml-2">
        <el-button type="primary" @click="prev">
          <el-icon><ArrowLeft /></el-icon>
        </el-button>
        <el-button type="primary" @click="next">
          <el-icon><ArrowRight /></el-icon>
        </el-button>
      </el-button-group>
    </div>
    <div id="calendar" class="w-100"></div>
  </div>
</template>

<script setup>
import { useI18n } from "vue-i18n";
import { useStore } from "vuex";
import {
  ref,
  defineProps,
  watch,
  computed,
  onMounted,
  defineEmits,
  h,
} from "vue";
import { Calendar } from "@fullcalendar/core";
import timeGridPlugin from "@fullcalendar/timegrid";
import interactionPlugin from "@fullcalendar/interaction";
import allLocales from "@fullcalendar/core/locales-all";
import { ElMessageBox } from "element-plus";
import useCommonFn from "@/commonFn";

const nameDetail = ref("");
const { dateToString } = useCommonFn();
const store = useStore();
const { t } = useI18n();
const objCalendar = ref(null);
const running = ref(0);
const language = ref(store.getters["config/getLanguage"]);
const props = defineProps({
  data: Object,
});
const emit = defineEmits(["agree", "back"]);

const options = computed(() => {
  return ["", ...props.data.nameDetails.split(",")];
});

const stadiumName = computed(() => {
  return props.data.name;
});

const getLanguage = computed(() => {
  return store.getters["config/getLanguage"];
});

watch(getLanguage, (newValue) => {
  language.value = newValue;
  renderCalendar(props.data);
});

onMounted(() => {
  renderCalendar(props.data);
});

const chooseNameDetail = (command) => {
  if (nameDetail.value != command) {
    nameDetail.value = command;
    renderCalendar(props.data);
  }
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
    calendar.render();
  }
};

const openConfirmDialog = (selectedInfo, timeFrame) => {
  if (!nameDetail.value) {
    alert(t("ChooseNameDetailMesg"));
    return;
  }
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
  ElMessageBox({
    title: t("Confirm"),
    message: h("div", null, [
      h(
        "div",
        null,
        t("ConfirmBooking", {
          name: props.data.name,
          time: selectedTime,
          date: selectedDate,
          detail: nameDetail.value,
        })
      ),
    ]),
    confirmButtonText: t("OK"),
    cancelButtonText: t("Cancel"),
    type: "info",
  })
    .then(() => {
      store.dispatch("booking/getInstance", "").then((res) => {
        if (res?.data?.data) {
          let objBooking = res.data.data;
          emit("agree", {
            bookingId: objBooking.id,
            timeFrameInfoId: timeFrame.id,
            stadiumName: props.data.name,
            bookingDate: dateToString(objBooking.bookingDate, "dd/MM/yyyy"),
            bookingDateReal: objBooking.bookingDate,
            matchDate: selectedDate,
            matchDateReal: selectedInfo.end,
            price: timeFrame.price,
            timeFrameInfo: selectedTime,
            pitchId: props.data.id,
            weekdays: selectedInfo.end.getDay(),
            accountId: props.data.managerId,
            nameDetail: nameDetail.value,
          });
        }
      });
    })
    .catch(() => {});
};

const validateSelectDateTimeOnCalendar = (selectedInfo) => {
  let selectedStart = selectedInfo.start;
  let selectedEnd = selectedInfo.end;
  for (let i = 0; i < props.data.timeFrameInfos.length; i++) {
    const items = props.data.timeFrameInfos[i];
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

const getEventByDate = async (start, end, stadiumData) => {
  let result = await store.dispatch("bookingDetail/getByDate", {
    startDate: start,
    endDate: end,
    pitchId: stadiumData.id,
    nameDetail: nameDetail.value,
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

const prevStep = () => {
  emit("back");
};
</script>
