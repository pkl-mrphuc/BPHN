<script setup>
import { onMounted, ref, inject, watch } from "vue";
import { Calendar } from "@fullcalendar/core";
import resourceTimeGridPlugin from "@fullcalendar/resource-timegrid";
import allLocales from "@fullcalendar/core/locales-all";
import { useStore } from "vuex";
import useCommonFn from "@/commonFn";
import useToggleModal from "@/register-components/actionDialog";
import { useI18n } from "vue-i18n";
import { ElLoading } from "element-plus";
import { BookingStatusEnum } from "@/const";
import {
  InfoFilled,
  ArrowLeft,
  ArrowRight,
  FullScreen,
} from "@element-plus/icons-vue";

const store = useStore();
const lstResource = ref([]);
const { dateToString, equals } = useCommonFn();
const { hasRole, openModal } = useToggleModal();
const objMatch = ref(null);
const objEvent = ref(null);
const objCalendar = ref(null);
const { t } = useI18n();
const loadingOptions = inject("loadingOptions");
const pageSize = ref(1);
const pageIndex = ref(1);
const currentDate = ref(new Date());
const totalPage = ref(0);
const expandMode = ref(false);
const selectedDateDisplay = ref("");

onMounted(() => {
  const { offsetWidth } = document.getElementById("app");
  if (offsetWidth >= 768) {
    pageSize.value = 2;
  }
  let expandModeCache = localStorage.getItem("expand-mode-calendar-key");
  if(expandModeCache === "1") {
    expandMode.value = true;
  }
  loadResource();
});

watch(currentDate, (newValue) => {
  if (
    objCalendar.value &&
    !equals(dateToString(newValue), dateToString(objCalendar.value.getDate()))
  ) {
    objCalendar.value.gotoDate(newValue);
    selectedDateDisplay.value = currentDate.value
      ? dateToString(currentDate.value, store.getters["config/getFormatDate"])
      : "";
  }
});

const loadResource = () => {
  const loading = ElLoading.service(loadingOptions);

  store
    .dispatch("pitch/getCountPaging", {
      accountId: store.getters["account/getAccountId"],
      hasInactive: false,
      pageSize: pageSize.value,
      pageIndex: pageIndex.value,
    })
    .then((res) => {
      if (res?.data?.data) {
        totalPage.value = res.data.data.totalPage;
      }
    });

  store
    .dispatch("pitch/getPaging", {
      accountId: store.getters["account/getAccountId"],
      hasDetail: false,
      hasInactive: false,
      pageSize: pageSize.value,
      pageIndex: pageIndex.value,
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
      loading.close();
    });
};

const renderCalendar = async (lstResource) => {
  if (lstResource?.length > 0) {
    let calendarEl = document.getElementById("calendarTimeGrid");
    if (!calendarEl) return;
    let calendar = new Calendar(calendarEl, {
      plugins: [resourceTimeGridPlugin],
      initialView: "resourceTimeGridDay",
      resources: lstResource,
      locales: allLocales,
      locale: store.getters["config/getLanguage"],
      height: "1785px",
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
        } else {
          callback([]);
        }
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
    if (currentDate?.value) {
      objCalendar.value.gotoDate(currentDate.value);
      selectedDateDisplay.value = currentDate.value
        ? dateToString(currentDate.value, store.getters["config/getFormatDate"])
        : "";
    }
    handleAfterRenderCalendar(calendarEl);
  }
};

const handleAfterRenderCalendar = (calendar) => {
  let licenseElement = calendar.getElementsByClassName("fc-license-message")[0];
  if (licenseElement) {
    licenseElement.style.display = "none";
  }

  let prevElement = document.createElement("div");
  prevElement.setAttribute("id", "prev_icon");
  prevElement.addEventListener("click", () => {
    pageIndex.value--;
    loadResource();
  });
  let nextElement = document.createElement("div");
  nextElement.setAttribute("id", "next_icon");
  nextElement.addEventListener("click", () => {
    pageIndex.value++;
    loadResource();
  });
  let lstNameStadiumElement = calendar.getElementsByClassName("fc-col-header");

  document.getElementById("prev_icon")?.remove();
  document.getElementById("next_icon")?.remove();

  if (pageIndex.value == 1) {
    prevElement.style.display = "none";
  }

  if (pageIndex.value == totalPage.value) {
    nextElement.style.display = "none";
  }

  lstNameStadiumElement?.[0].appendChild(prevElement);
  lstNameStadiumElement?.[0].appendChild(nextElement);
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
          bookingId: item.bookingId,
          teamA: !item.teamA ? item.phoneNumber : item.teamA,
          teamB: item.teamB ?? "",
          stadium: item.stadium,
          note: item.note ?? "",
          status: item.status ?? "",
          deposite: item.deposite ?? 0
        },
      });
    }
    return lstResult;
  }
};

const buildEventInfoHtml = (timeText, eventInfo) => {
  let eventContainer = document.createElement("div");
  if (equals(eventInfo?.status, BookingStatusEnum.PENDING)) {
    eventContainer.className = "p-2 w-100 h-100 bg-info";
  } else {
    if (eventInfo?.teamB) {
      eventContainer.className = "p-2 w-100 h-100 bg-danger";
    } else {
      eventContainer.className = "p-2 w-100 h-100 bg-primary";
    }
  }

  let html = `<div class="fs-3 fw-bold">${eventInfo.stadium}</div>`;
  if (eventInfo?.note) {
    html += `<div class="fs-6 fst-italic">${eventInfo.note}</div>`;
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
    objEvent.value.event.setExtendedProp("deposite", data.deposite);
    objEvent.value.event.setExtendedProp("status", data.status);
  }
};

const today = () => {
  if (objCalendar.value) {
    objCalendar.value.today();
    currentDate.value = objCalendar.value.getDate();
    selectedDateDisplay.value = currentDate.value
      ? dateToString(currentDate.value, store.getters["config/getFormatDate"])
      : "";
  }
};

const prev = () => {
  if (objCalendar.value) {
    objCalendar.value.prev();
    currentDate.value = objCalendar.value.getDate();
    selectedDateDisplay.value = currentDate.value
      ? dateToString(currentDate.value, store.getters["config/getFormatDate"])
      : "";
  }
};

const next = () => {
  if (objCalendar.value) {
    objCalendar.value.next();
    currentDate.value = objCalendar.value.getDate();
    selectedDateDisplay.value = currentDate.value
      ? dateToString(currentDate.value, store.getters["config/getFormatDate"])
      : "";
  }
};

const expandModeClick = () => {
  expandMode.value = !expandMode.value;
  renderCalendar(lstResource.value);
  localStorage.removeItem("expand-mode-calendar-key");
  localStorage.setItem("expand-mode-calendar-key", expandMode.value ? "1" : "0");
};
</script>

<template>
  <section>
    <div class="container">
      <div class="d-flex flex-row align-items-center justify-content-between">
        <h3 class="fs-3">{{ t("Calendar") }} / {{ selectedDateDisplay }}</h3>
        <div class="d-flex flex-row">
          <el-popover
            placement="top-start"
            :title="t('Note')"
            width="250"
            trigger="click"
          >
            <template #reference>
              <el-button
                class="mx-1"
                type="warning"
                :icon="InfoFilled"
                circle
              />
            </template>

            <div class="row mb-3 d-flex flex-row align-items-center">
              <div class="col-3 square bg-info"></div>
              <div class="col-9">
                <div class="mx-3">
                  {{ t("PENDING") }}
                </div>
              </div>
            </div>

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
          </el-popover>
          <el-button
            class="mx-1"
            @click="expandModeClick"
            :icon="FullScreen"
            circle
          ></el-button>
          <div v-if="expandMode">
            <el-button-group class="mx-1">
              <el-button type="primary" @click="prev">
                <el-icon><ArrowLeft /></el-icon>
              </el-button>
              <el-button type="primary" @click="next">
                <el-icon><ArrowRight /></el-icon>
              </el-button>
            </el-button-group>
            <el-button class="mx-1" type="primary" @click="today">{{
              t("Today")
            }}</el-button>
          </div>
        </div>
      </div>
      <div class="row" style="height: calc(100vh - 190px); overflow: scroll">
        <div
          v-if="!expandMode"
          class="col-12 col-sm-12 col-md-12 col-lg-12 col-xl-4"
        >
          <el-calendar v-model="currentDate">
            <template #header="{}">
              <span></span>
            </template>
          </el-calendar>
        </div>
        <div
          class="col-12 col-sm-12 col-md-12 col-lg-12"
          :class="!expandMode ? 'col-xl-8' : 'col-xl-12'"
        >
          <div class="mx-2">
            <div id="calendarTimeGrid"></div>
          </div>
        </div>
      </div>
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
.square {
  width: 15px;
  height: 15px;
  border-radius: 3px;
}
</style>
