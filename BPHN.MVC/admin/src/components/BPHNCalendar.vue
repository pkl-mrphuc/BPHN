<script setup>
import { onMounted, ref } from "vue";
import { Calendar } from "@fullcalendar/core";
import resourceTimeGridPlugin from "@fullcalendar/resource-timegrid";
import allLocales from "@fullcalendar/core/locales-all";
import { useStore } from "vuex";
import useCommonFn from "@/commonFn";
import useToggleModal from "@/register-components/actionDialog";

const store = useStore();
const lstResource = ref([]);
const { dateToString } = useCommonFn();
const { hasRole, openModal } = useToggleModal();
const objMatch = ref(null);
const objEvent = ref(null);

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
        left: "title",
        right: "today prev,next",
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
  eventContainer.style.display = "flex";
  eventContainer.style.flexDirection = "column";
  eventContainer.style.height = "100%";
  eventContainer.style.padding = "5px";
  eventContainer.style.boxSizing = "border-box";
  let html = `<i>${timeText} | ${eventInfo.stadium}</i> 
              <ul>
                <li style="font-weight: bold; text-overflow: ellipsis; white-space: nowrap; overflow: hidden;" title="${
                  eventInfo.teamA
                }">- ${eventInfo.teamA}</li>
                <li style="font-weight: bold; text-overflow: ellipsis; white-space: nowrap; overflow: hidden;" title="${
                  eventInfo.teamB
                }">- ${!eventInfo.teamB ? "?" : eventInfo.teamB}</li>
              </ul>
              <div style="margin-top: auto; font-style: italic; overflow: hidden; text-overflow: ellipsis; white-space: nowrap;" title="${
                eventInfo.note
              }">Note: ${eventInfo.note}</div>`;

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
  height: calc(100vh - 100px);
}
</style>
