<script setup>
import { ref, onMounted } from "vue";
import { Calendar } from "@fullcalendar/core";
import resourceTimeGridPlugin from "@fullcalendar/resource-timegrid";
import allLocales from "@fullcalendar/core/locales-all";

const date = ref(new Date());

onMounted(() => {
  let calendarEl = document.getElementById("calendarTimeGrid");
  let calendar = new Calendar(calendarEl, {
    plugins: [resourceTimeGridPlugin],
    initialView: "resourceTimeGridDay",
    resources: [{ title: "Resource A" }, { title: "Resource B" }],
    locales: allLocales,
    locale: "en",
  });
  calendar.render();
  handleAfterRenderCalendar(calendarEl);
});

const handleAfterRenderCalendar = (calendar) => {
  let headerElement = calendar.getElementsByClassName("fc-header-toolbar")[0];
  if(headerElement) {
    headerElement.style.display = "none";
  }
};
</script>

<template>
  <section
    class="pbhn-screen pbhn-calendar h-full-screen d-flex justify-content-between"
  >
    <div class="w30">
      <el-calendar v-model="date" />
    </div>
    <div class="w70" id="calendarTimeGrid"></div>
  </section>
</template>

<style scoped>
.h-full-screen {
  height: 100%;
}

.d-flex {
  display: flex;
}

.justify-content-between {
  justify-content: space-between;
}

.w30 {
  width: 30%;
}

.w70 {
  width: 70%;
}
</style>
