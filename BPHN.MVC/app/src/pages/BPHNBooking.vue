<template>
  <section>
    <h1 class="fs-36">{{ t("Booking") }}</h1>
    <section>
      <el-alert
        type="warning"
        :closable="false"
        description="Bạn có thể đặt sân theo sau khi hoàn thiện các bước sau hoặc liên hệ chủ sân bóng"
        class="mb-8"
      />
      <el-alert
        type="warning"
        :closable="false"
        description="Lưu ý: Bạn có thể đặt online nếu chủ sân là đối tác của BPHN. Xem chi tiết danh sách ở đây"
        class="mb-8"
      />
      <el-steps :active="active" finish-status="success" simple>
        <el-step title="Tìm sân" />
        <el-step title="Xem lịch" />
        <el-step title="Đặt sân" />
      </el-steps>
      <div class="p-12">
        <div class="content">
          <div v-show="active == 0">
            <el-autocomplete
              class="wp-100 mb-8"
              v-model="state"
              :fetch-suggestions="querySearch"
              popper-class="my-autocomplete"
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

            <el-table :data="lstStadium" height="250" style="width: 100%">
              <el-table-column prop="name" label="Tên" width="180" />
              <el-table-column prop="address" label="Địa chỉ" />
              <el-table-column fixed="right" label="" width="200">
                <template #default="scope">
                  <el-button
                    type="info"
                    size="small"
                    @click="view(scope.row)"
                    >Xem chi tiết</el-button
                  >
                  <el-button
                    type="primary"
                    size="small"
                    @click="choose(scope.row)"
                    >Chọn</el-button
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
          <div v-show="active == 1">
            <div class="d-flex justify-content-between align-items-center">
              <span class="fs-36">{{ stadiumName }}</span>
              <div class="ml-auto"></div>
              <el-button type="primary" @click="today" :disabled="isToday"
                >Hôm nay</el-button
              >
              <el-button-group class="ml-8">
                <el-button type="primary" @click="prev">
                  <el-icon><ArrowLeft /></el-icon>
                </el-button>
                <el-button type="primary" @click="next">
                  <el-icon><ArrowRight /></el-icon>
                </el-button>
              </el-button-group>
            </div>
            <div id="calendar" class="wp-100"></div>
          </div>
          <div v-show="active == 2"></div>
        </div>
        <div class="d-flex justify-content-between align-items-center my-8">
          <div class="ml-auto"></div>
          <el-button @click="prevStep" type="primary" v-if="active != 0"
            >Prev</el-button
          >
          <el-button @click="nextStep" type="primary" v-if="active == 2"
            >Complete</el-button
          >
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
import allLocales from "@fullcalendar/core/locales-all";
import { useI18n } from "vue-i18n";

const state = ref("");
const store = useStore();
const language = ref(store.getters["config/getLanguage"]);
const objCalendar = ref(null);
const isToday = ref(true);
const { t } = useI18n();
const active = ref(0);
const lstStadium = ref([]);
const pageIndex = ref(1);
const pageSize = ref(100);
const totalRecord = ref(0);
const stadiumName = ref("");

const getLanguage = computed(() => {
  return store.getters["config/getLanguage"];
});

watch(getLanguage, (newValue) => {
  language.value = newValue;
  renderCalendar();
});

const querySearch = (queryString, cb) => {
  store
    .dispatch("stadium/getPaging", {
      pageIndex: pageIndex.value,
      txtSearch: queryString,
    })
    .then((res) => {
      let data = res.data?.data ?? [];
      lstStadium.value = data;
      if (typeof cb === "function") cb(lstStadium.value);
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
  state.value = item.name;
  lstStadium.value = [item];
};

const today = () => {
  if (objCalendar.value) {
    objCalendar.value.today();
    isToday.value = true;
  }
};

const prev = () => {
  if (objCalendar.value) {
    objCalendar.value.prev();
    isToday.value = false;
  }
};

const next = () => {
  if (objCalendar.value) {
    objCalendar.value.next();
    isToday.value = false;
  }
};

const renderCalendar = () => {
  let calendarElement = document.getElementById("calendar");
  if (calendarElement) {
    const calendar = new Calendar(calendarElement, {
      plugins: [timeGridPlugin],
      initialView: "timeGridWeek",
      locales: allLocales,
      locale: language.value,
      headerToolbar: {
        left: "",
        right: "",
      },
      height: "1785px",
    });
    objCalendar.value = calendar;
    calendar.render();
  }
};

const nextStep = () => {
  active.value++;
};

const prevStep = () => {
  active.value--;
  if (active.value < 0) active.value = 0;
};

const nextPage = () => {
  querySearch(state.value);
};

const prevPage = () => {
  querySearch(state.value);
};

const currentPage = () => {
  querySearch(state.value);
};

const view = (stadium) => {
  alert(stadium);
};

const choose = (stadium) => {
  stadiumName.value = stadium.name;
  nextStep();
  renderCalendar();
};
</script>
