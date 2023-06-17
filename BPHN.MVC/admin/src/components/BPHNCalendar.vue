<script setup>
import { onMounted, ref } from "vue";
import { Calendar } from "@fullcalendar/core";
import resourceTimeGridPlugin from "@fullcalendar/resource-timegrid";
import allLocales from "@fullcalendar/core/locales-all";
import { useStore } from "vuex";
import useCommonFn from "@/commonFn";

const store = useStore();
const resources = ref([]);
const { dateToString } = useCommonFn();

onMounted(() => {
  store
    .dispatch("pitch/getPaging", {
      accountId: store.getters["account/getAccountId"],
      hasDetail: false,
      hasInactive: false,
    })
    .then(async (res) => {
      let lstPitch = res?.data?.data ?? [];
      resources.value = [];
      for (let i = 0; i < lstPitch.length; i++) {
        const item = lstPitch[i];
        resources.value.push({
          title: item.name,
          id: item.id,
        });
      }
      await renderCalendar(resources.value);
    });
});

const renderCalendar = async (resources) => {
  if (resources?.length > 0) {
    let calendarEl = document.getElementById("calendarTimeGrid");
    let calendar = new Calendar(calendarEl, {
      plugins: [resourceTimeGridPlugin],
      initialView: "resourceTimeGridDay",
      resources: resources,
      locales: allLocales,
      locale: store.getters["config/getLanguage"],
      headerToolbar: {
        left: "title",
        right: "today prev,next",
      },
      events: async function (data, callback) {
        if (data) {
          let events = await getEventByDate(dateToString(data.start));
          callback(events);
        }
        callback([]);
        handleAfterRenderCalendar(calendarEl);
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
        title: `${item.phoneNumber}-${item.nameDetail}`,
        start: item.start,
        end: item.end,
      });
    }
    return lstResult;
  }
};
</script>

<template>
  <section class="pbhn-screen" style="height: 100%">
    <div class="container" style="height: 100%">
      <div class="body">
        <div id="calendarTimeGrid"></div>
      </div>
    </div>
  </section>
</template>

<style scoped>
#calendarTimeGrid {
  height: calc(100vh - 120px);
}
</style>
