<script setup>
import { useI18n } from "vue-i18n";
import { Refresh, Search } from "@element-plus/icons-vue";
import { ref, onMounted } from "vue";
import { useStore } from "vuex";
import useCommonFn from "@/commonFn";

const { t } = useI18n();
const store = useStore();
const historyLogData = ref([]);
const pageIndex = ref(1);
const pageSize = ref(50);
const totalRecord = ref(0);
const txtSearch = ref("");
const running = ref(0);
const { dateToString } = useCommonFn();

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
        historyLogData.value = res.data.data;
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
  <section class="pbhn-screen" style="height: 100%">
    <div class="container" style="height: 100%">
      <div class="head">
        <h3 class="head_title">{{ t("HistoryLog") }}</h3>
        <div class="head_toolbar">
          <el-input
            style="margin-right: 12px; width: 300px"
            v-model="txtSearch"
            :placeholder="t('Search')"
            :suffix-icon="Search"
            @keyup.enter="loadData"
          />
          <el-button @click="loadData">
            <el-icon><Refresh /></el-icon>
          </el-button>
        </div>
      </div>
      <div class="body" style="margin-top: 20px">
        <el-table
          :data="historyLogData"
          style="height: calc(100vh - 220px)"
          :empty-text="t('NoData')"
        >
          <el-table-column :label="t('CreatedDate')">
            <template #default="scope">
              {{ dateToString(scope.row.createdDate, true) }}
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
          <el-table-column :label="t('Description')">
            <template #default="scope">
              {{ scope.row.description }}
            </template>
          </el-table-column>
        </el-table>
      </div>
      <div class="footer">
        <el-pagination
          background
          v-model:current-page="pageIndex"
          v-model:page-size="pageSize"
          layout="sizes, prev, pager, next"
          :total="totalRecord"
          v-if="historyLogData.length > 0"
          @prev-click="prevClick"
          @next-click="nextClick"
          @size-change="sizePageChange"
          @current-change="currentChange"
        />
      </div>
    </div>
  </section>
</template>

<style scoped>
.footer {
  display: flex;
  align-items: center;
  justify-content: flex-end;
  padding-top: 20px;
}
</style>