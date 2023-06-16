<script setup>
import { onMounted } from "vue";
import { Calendar } from "@fullcalendar/core";
import resourceTimeGridPlugin from "@fullcalendar/resource-timegrid";
import allLocales from "@fullcalendar/core/locales-all";
import { useStore } from "vuex";

const store = useStore();

onMounted(() => {
  let calendarEl = document.getElementById("calendarTimeGrid");
  let calendar = new Calendar(calendarEl, {
    plugins: [resourceTimeGridPlugin],
    initialView: "resourceTimeGridDay",
    resources: [{ title: "Resource A" }, { title: "Resource B" }],
    locales: allLocales,
    locale: store.getters["config/getLanguage"],
  });
  calendar.render();
  handleAfterRenderCalendar(calendarEl);
});

const handleAfterRenderCalendar = (calendar) => {
  let licenseElement = calendar.getElementsByClassName("fc-license-message")[0];
  if (licenseElement) {
    licenseElement.style.display = "none";
  }
};
</script>

<template>
  <section class="pbhn-screen">
    <div id="calendarTimeGrid"></div>
  </section>
</template>

<style scoped>
#calendarTimeGrid {
  height: calc(100vh - 120px);
}
</style>
