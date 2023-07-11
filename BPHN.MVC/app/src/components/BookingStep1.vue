<template>
  <div>
    <el-alert
      type="warning"
      :closable="false"
      :description="t('OnlyPartner')"
      class="mb-2"
    />
    <el-autocomplete
      class="w-100 mb-2"
      v-model="key"
      :fetch-suggestions="searchStadium"
      popper-class="my-autocomplete"
      :placeholder="t('FindStadium')"
      @select="selectStadium"
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

    <el-table
      :data="lstStadium"
      height="250"
      class="w-100"
      :span-method="objSpanMethod"
    >
      <el-table-column prop="name" :label="t('Name')" />
      <el-table-column prop="address" :label="t('Address')" />
      <el-table-column :label="t('NameDetails')">
        <template #default="scope">
          {{ nameDetails(scope.row.nameDetails) }}
        </template>
      </el-table-column>
      <el-table-column prop="timeFrameName" :label="t('TimeFrameName')" />
      <el-table-column :label="t('TimeFrameStart')">
        <template #default="scope">
          {{
            dateToString(scope.row.timeFrameStart, "dd/MM/yyyy", true, false)
          }}
        </template>
      </el-table-column>
      <el-table-column :label="t('TimeFrameEnd')">
        <template #default="scope">
          {{ dateToString(scope.row.timeFrameEnd, "dd/MM/yyyy", true, false) }}
        </template>
      </el-table-column>
      <el-table-column prop="timeFramePrice" :label="t('TimeFramePrice')" />
      <el-table-column fixed="right" label="">
        <template #default="scope">
          <el-button type="primary" size="small" @click="choose(scope.row)">{{
            t("Choose")
          }}</el-button>
        </template>
      </el-table-column>
    </el-table>
    <el-pagination
      class="m-3 d-flex flex-row align-items-center justify-content-end"
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
</template>

<script setup>
import { useI18n } from "vue-i18n";
import { useStore } from "vuex";
import { ref, defineEmits, onMounted } from "vue";
import useCommonFn from "@/commonFn";

const { dateToString } = useCommonFn();
const store = useStore();
const { t } = useI18n();
const emit = defineEmits(["choose"]);
const key = ref("");
const lstStadium = ref([]);
const pageIndex = ref(1);
const pageSize = ref(100);
const totalRecord = ref(0);

onMounted(() => {
  searchStadiumFromDataCache();
});

const objSpanMethod = ({ row, column, rowIndex, columnIndex }) => {
  console.log(column ? "" : "");
  let lstRow = lstStadium.value.filter((item) => item.id == row.id);
  if (lstRow.length > 1) {
    let curRow = lstRow.filter((item) => item.timeFrameId == row.timeFrameId);
    let curObj = Array.isArray(curRow) && curRow.length > 0 ? curRow[0] : null;
    if (curObj) {
      if (curObj && curObj["firstIndex"] == undefined) {
        for (let i = 0; i < lstRow.length; i++) {
          const item = lstRow[i];
          item["firstIndex"] = rowIndex;
        }
      }
      if (rowIndex < lstRow.length + curObj["firstIndex"]) {
        if (
          columnIndex == 0 ||
          columnIndex == 1 ||
          columnIndex == 2 ||
          columnIndex == 7
        ) {
          if (rowIndex == curObj["firstIndex"]) {
            return {
              rowspan: lstRow.length,
              colspan: 1,
            };
          } else {
            return {
              rowspan: 0,
              colspan: 0,
            };
          }
        }
      }
    }
  }
};

const nameDetails = (nameDetails) => {
  return !nameDetails ? "" : nameDetails.split(";").join("/");
};

const searchStadium = (queryString, cb) => {
  store
    .dispatch("stadium/getPaging", {
      pageIndex: pageIndex.value,
      txtSearch: queryString,
    })
    .then((res) => {
      lstStadium.value = [];
      let data = res.data?.data ?? [];
      if (typeof cb === "function") cb(data);
      for (let i = 0; i < data.length; i++) {
        const item = data[i];
        if (Array.isArray(item?.timeFrameInfos)) {
          for (let j = 0; j < item.timeFrameInfos.length; j++) {
            const propItem = item.timeFrameInfos[j];
            lstStadium.value.push({
              id: item.id,
              name: item.name,
              address: item.address,
              managerId: item.managerId,
              nameDetails: nameDetails(item.nameDetails),
              timeFrameName: propItem.name,
              timeFrameStart: propItem.timeBegin,
              timeFrameEnd: propItem.timeEnd,
              timeFramePrice: propItem.price,
              timeFrameId: propItem.id,
              timeFrameInfos: item.timeFrameInfos,
            });
          }
        }
      }
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

const selectStadium = (item) => {
  key.value = item.name;
  searchStadium(key.value);
};

const nextPage = () => {
  searchStadium(key.value);
};

const prevPage = () => {
  searchStadium(key.value);
};

const currentPage = () => {
  searchStadium(key.value);
};

const choose = (stadium) => {
  emit("choose", stadium);
};

const searchStadiumFromDataCache = () => {
  let stadiumJSON = localStorage.getItem("stadium-data");
  if (stadiumJSON) {
    let stadiumData = JSON.parse(stadiumJSON)
    key.value = stadiumData.name;
    searchStadium(key.value);
  }
};
</script>
