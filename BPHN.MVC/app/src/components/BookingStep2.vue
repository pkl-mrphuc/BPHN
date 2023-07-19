<template>
  <div>
    <el-alert
      type="warning"
      :closable="true"
      :description="t('DragDropOnCalendar')"
      class="mb-3"
      v-if="!getLocalStorage('note_3')"
      @close="saveLocalStorage('note_3', '1')"
    />
    <div
      class="mb-3 d-flex flex-row justify-content-between align-items-center"
    >
      <span class="fs-2">{{ stadiumName }}</span>
      <el-dropdown @command="chooseNameDetail">
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
    </div>
    <div class="d-flex flex-row justify-content-between align-items-center">
      <el-button @click="prevStep">{{ t("Back") }}</el-button>
      <div>
        <el-popover
          placement="top-start"
          :title="t('Note')"
          width="250"
          trigger="click"
        >
          <template #reference>
            <el-button class="mx-1" type="warning" :icon="InfoFilled" circle />
          </template>
          <div class="row mb-3 d-flex flex-row align-items-center">
            <div class="col-3 square bg-danger"></div>
            <div class="col-9">
              <div class="mx-3">
                {{ t("HasCompetitor") }}
              </div>
            </div>
          </div>

          <div class="row mb-3 d-flex flex-row align-items-center">
            <div class="col-3 square bg-primary"></div>
            <div class="col-9">
              <div class="mx-3">
                {{ t("HasNotCompetitor") }}
              </div>
            </div>
          </div>

          <div class="row mb-3 d-flex flex-row fst-italic">
            {{ t("ClickTimeFrameToChooseBooking") }}
          </div>

        </el-popover>
        <el-button type="primary" @click="today" class="ml-2">{{
          t("Today")
        }}</el-button>
        <el-button-group class="ml-2">
          <el-button type="primary" @click="prev">
            <el-icon><ArrowLeft /></el-icon>
          </el-button>
          <el-button type="primary" @click="next">
            <el-icon><ArrowRight /></el-icon>
          </el-button>
        </el-button-group>
      </div>
    </div>
    <div id="calendar" class="w-100"></div>
    <el-dialog v-model="showChooseNameDetail" :show-close="false">
      <div class="mb-3 fs-5">{{ t("ChooseNameDetailMesg") }}</div>
      <el-radio-group v-model="nameDetail" size="large">
        <el-radio-button :label="item" v-for="item in options" :key="item" />
      </el-radio-group>
      <template #footer>
        <span class="dialog-footer">
          <el-button type="primary" @click="chooseNameDetail(nameDetail)">
            {{ t("OK") }}
          </el-button>
        </span>
      </template>
    </el-dialog>
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
import { InfoFilled } from "@element-plus/icons-vue";

const nameDetail = ref("");
const { dateToString, getLocalStorage, saveLocalStorage } = useCommonFn();
const store = useStore();
const { t } = useI18n();
const objCalendar = ref(null);
const running = ref(0);
const language = ref(store.getters["config/getLanguage"]);
const duration = ref(3);
const showChooseNameDetail = ref(true);
const props = defineProps({
  data: Object,
});
const emit = defineEmits(["agree", "back"]);

const options = computed(() => {
  return [...props.data.nameDetails.split("/")];
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
  nameDetail.value = options.value?.length > 0 ? options.value[0] : "";
  const { offsetWidth } = document.getElementById("app");
  if (offsetWidth >= 576) {
    duration.value = 4;
  }
  if (offsetWidth >= 768) {
    duration.value = 5;
  }
  if (offsetWidth >= 992) {
    duration.value = 6;
  }
  if (offsetWidth >= 1200) {
    duration.value = 7;
  }
  renderCalendar(props.data);
});

const chooseNameDetail = (command) => {
  if (nameDetail.value != command || showChooseNameDetail.value) {
    nameDetail.value = command;
    renderCalendar(props.data);
  }
  showChooseNameDetail.value = false;
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
      initialView: "timeGridFourDay",
      views: {
        timeGridFourDay: {
          type: "timeGrid",
          duration: { days: duration.value },
        },
      },
      locales: allLocales,
      locale: language.value,
      headerToolbar: {
        left: "",
        right: "",
      },
      height: "1785px",
      events: async (data, callback) => {
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
      eventContent: (arg) => {
        let eventInfo = arg.event.extendedProps;
        return { domNodes: buildEventInfoHtml(arg.timeText, eventInfo) };
      },
      dateClick: (dateClickInfo) => {
        let selectedStart = dateClickInfo.date;
        let selectedEnd = dateClickInfo.date;
        let result = validateSelectDateTimeOnCalendar(
          selectedStart,
          selectedEnd
        );
        if (result) {
          openConfirmDialog(selectedEnd, result);
        }
      },
    });
    objCalendar.value = calendar;
    calendar.render();
  }
};

const openConfirmDialog = (selectedDateEnd, timeFrame) => {
  let selectedDate = dateToString(selectedDateEnd, "dd/MM/yyyy", false, true);
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
            matchDateReal: selectedDateEnd,
            price: timeFrame.price,
            timeFrameInfo: selectedTime,
            pitchId: props.data.id,
            weekdays: selectedDateEnd.getDay(),
            accountId: props.data.managerId,
            nameDetail: nameDetail.value,
          });
        }
      });
    })
    .catch(() => {});
};

const validateSelectDateTimeOnCalendar = (
  selectedStartReal,
  selectedEndReal
) => {
  let selectedStart = selectedStartReal;
  let selectedEnd = selectedEndReal;
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
        selectedStartReal.getHours(),
        selectedStartReal.getMinutes(),
        selectedStartReal.getSeconds()
      );
      selectedEnd = new Date(
        now.getFullYear(),
        now.getMonth(),
        now.getDate(),
        selectedEndReal.getHours(),
        selectedEndReal.getMinutes(),
        selectedEndReal.getSeconds()
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
  if (eventInfo?.teamB) {
    eventContainer.className = "p-2 w-100 h-100 bg-danger";
  } else {
    eventContainer.className = "p-2 w-100 h-100 bg-primary";
  }
  eventContainer.title = t("ClickHereCopyPhoneNumber");
  eventContainer.addEventListener("click", () => {
    if(!eventInfo.teamB) {
      alert(eventInfo.teamA);
    }
    else {
      alert(t('HasCompetitor'));
    }
  });
  let html = `<div class="fs-3 fw-bold">${eventInfo.stadium}</div>`;
  if (eventInfo?.note) {
    html += `<div class="fs-6 fst-italic">${eventInfo.note}</div>`;
  }

  eventContainer.innerHTML = html;
  let arrayOfDomNodes = [eventContainer];
  return arrayOfDomNodes;
};

const prevStep = () => {
  emit("back");
};
</script>

<style scoped>
.square {
  width: 15px;
  height: 15px;
  border-radius: 3px;
}
</style>