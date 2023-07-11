<script setup>
import { useI18n } from "vue-i18n";
import { Refresh, Search } from "@element-plus/icons-vue";
import { ref, onMounted, computed } from "vue";
import { useStore } from "vuex";
import useCommonFn from "@/commonFn";

const { t } = useI18n();
const store = useStore();
const lstHistoryLog = ref([]);
const pageIndex = ref(1);
const pageSize = ref(50);
const totalRecord = ref(0);
const txtSearch = ref("");
const running = ref(0);
const { dateToString } = useCommonFn();

const formatDate = computed(() => {
  return store.getters["config/getFormatDate"];
});

const loadData = () => {
  if (running.value > 0) {
    return;
  }
  ++running.value;
  store
    .dispatch("historyLog/getPaging", {
      pageIndex: pageIndex.value,
      pageSize: pageSize.value,
      txtSearch: txtSearch.value
    })
    .then((res) => {
      if (res?.data?.data) {
        lstHistoryLog.value = res.data.data;
      }
      setTimeout(() => {
        running.value = 0;
      }, 1000);
    });

  store
    .dispatch("historyLog/getCountPaging", {
      pageIndex: pageIndex.value,
      pageSize: pageSize.value,
      txtSearch: txtSearch.value,
    })
    .then((res) => {
      if (res?.data?.data) {
        let result = res.data.data;
        totalRecord.value = result.totalAllRecords;
      }
    });
};

const prevClick = () => {
  loadData();
};

const nextClick = () => {
  loadData();
};

const sizePageChange = () => {
  loadData();
};

const currentChange = () => {
  loadData();
};

onMounted(() => {
  loadData();
});
</script>


<template>
  <section>
    <div class="container">
      <div class="d-flex flex-row align-items-center justify-content-between">
        <h3 class="fs-3">{{ t("HistoryLog") }}</h3>
        <div class="d-flex flex-row">
          <el-input
            v-model="txtSearch"
            :placeholder="t('Search')"
            :suffix-icon="Search"
            @keyup.enter="loadData"
          />
          <el-button @click="loadData" class="ml-2">
            <el-icon><Refresh /></el-icon>
          </el-button>
        </div>
      </div>
      <div>
        <el-table
          :data="lstHistoryLog"
          style="height: calc(100vh - 252px)"
          :empty-text="t('NoData')"
        >
          <el-table-column :label="t('CreatedDate')">
            <template #default="scope">
              {{ dateToString(scope.row.createdDate, formatDate, true) }}
            </template>
          </el-table-column>
          <el-table-column :label="t('IPAddress')">
            <template #default="scope">
              {{ scope.row.ipAddress }}
            </template>
          </el-table-column>
          <el-table-column :label="t('Actor')">
            <template #default="scope">
              {{ scope.row.actor }}
            </template>
          </el-table-column>
          <el-table-column :label="t('ActionName')">
            <template #default="scope">
              {{ scope.row.actionName }}
            </template>
          </el-table-column>
          <el-table-column :label="t('Entity')">
            <template #default="scope">
              {{ scope.row.entity }}
            </template>
          </el-table-column>
          <el-table-column :label="t('Description')">
            <template v-slot="scope">
              <span v-html="scope.row.description"></span>
            </template>
          </el-table-column>
        </el-table>
      </div>
      <div class="p-3 d-flex flex-row align-items-center justify-content-end">
        <el-pagination
          background
          v-model:current-page="pageIndex"
          v-model:page-size="pageSize"
          layout="sizes, prev, pager, next"
          :total="totalRecord"
          v-if="lstHistoryLog.length > 0"
          @prev-click="prevClick"
          @next-click="nextClick"
          @size-change="sizePageChange"
          @current-change="currentChange"
        />
      </div>
    </div>
  </section>
</template>