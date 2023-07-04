<template>
  <el-autocomplete
    v-model="state"
    :fetch-suggestions="querySearch"
    popper-class="my-autocomplete"
    placeholder="Tìm sân bóng"
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
  <div id="calendar"></div>
</template>

<script setup>
import { ref } from "vue";
import { useStore } from "vuex";
import { Calendar } from "@fullcalendar/core";
import timeGridPlugin from "@fullcalendar/timegrid";
import allLocales from "@fullcalendar/core/locales-all";

const state = ref("");
const store = useStore();
const querySearch = (queryString, cb) => {
  if (!queryString) return;
  store.dispatch("stadium/fetchStadium", queryString).then((res) => {
    let data = res.data?.data;
    cb(data ?? []);
  });
};

const handleSelect = (item) => {
  state.value = item.name;
  let calendarElement = document.getElementById("calendar");
  if (calendarElement) {
    const calendar = new Calendar(calendarElement, {
      plugins: [timeGridPlugin],
      initialView: "timeGridWeek",
      locales: allLocales,
      locale: store.getters["config/getLanguage"],
    });

    calendar.render();
  }
};
</script>
