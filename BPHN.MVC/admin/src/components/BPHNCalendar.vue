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
import { InfoFilled } from "@element-plus/icons-vue";

const store = useStore();
const lstResource = ref([]);
const { dateToString, equals } = useCommonFn();
const { hasRole, openModal } = useToggleModal();
const objMatch = ref(null);
const objEvent = ref(null);
const objCalendar = ref(null);
const { t } = useI18n();
const formatDate = ref(store.getters["config/getFormatDate"]);
const selectedDate = ref(null);
const loadingOptions = inject("loadingOptions");
const pageSize = ref(2);
const pageIndex = ref(1);
const currentDate = ref(null);

onMounted(() => {
  const { offsetWidth } = document.getElementById("app");
  if (offsetWidth >= 768) {
    pageSize.value = -1;
  }
  if (offsetWidth >= 992) {
    pageSize.value = -1;
  }
  if (offsetWidth >= 1200) {
    pageSize.value = -1;
  }

  const loading = ElLoading.service(loadingOptions);
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
});

watch(currentDate, (newValue) => {
  if (objCalendar.value) {
    objCalendar.value.gotoDate(newValue);
    selectedDate.value = getSelectedDate();
  }
});

const getSelectedDate = () => {
  return objCalendar.value
    ? dateToString(objCalendar.value.getDate(), formatDate.value)
    : "";
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
    currentDate.value = objCalendar.value.getDate();
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
          status: item.status ?? "",
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
  }
};
</script>

<template>
  <section>
    <div class="container">
      <div class="d-flex flex-row align-items-center justify-content-between">
        <h3 class="fs-3">{{ t("Calendar") }}</h3>
        <div>
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
        </div>
      </div>
      <div class="row" style="height: calc(100vh - 190px); overflow: scroll">
        <div class="col-12 col-sm-12 col-md-12 col-lg-12 col-xl-4">
          <el-calendar v-model="currentDate">
            <template #header="{}">
              <span></span>
            </template>
          </el-calendar>
        </div>
        <div class="col-12 col-sm-12 col-md-12 col-lg-12 col-xl-8">
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
