<script setup>
import { useI18n } from "vue-i18n";
import { Refresh, Search } from "@element-plus/icons-vue";
import { ref, onMounted, computed } from "vue";
import { useStore } from "vuex";
import useCommonFn from "@/commonFn";
import { ElNotification } from "element-plus";

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
      txtSearch: txtSearch.value,
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

const goToViewDetail = (id) => {
  console.log(id);
  ElNotification({
    title: t("Notification"),
    message: t("FeatureIsDeveloping"),
    type: "info",
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
      <div
        class="row mb-3 d-flex flex-row align-items-center justify-content-between"
      >
        <h3 class="fs-3 col-12 col-sm-12 col-md-12 col-lg-8">
          {{ t("HistoryLog") }}
        </h3>
        <div class="col-12 col-sm-12 col-md-12 col-lg-4 d-flex flex-row">
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
          style="height: calc(100vh - 300px)"
          :empty-text="t('NoData')"
        >
          <el-table-column :label="t('CreatedDate')" width="200">
            <template #default="scope">
              {{ dateToString(scope.row.createdDate, formatDate, true) }}
            </template>
          </el-table-column>
          <el-table-column :label="t('IPAddress')" width="150">
            <template #default="scope">
              {{ scope.row.ipAddress }}
            </template>
          </el-table-column>
          <el-table-column :label="t('Actor')" min-width="150">
            <template #default="scope">
              <span class="text-truncate">{{ scope.row.actor }}</span>
            </template>
          </el-table-column>
          <el-table-column :label="t('ActionName')" min-width="150">
            <template #default="scope">
              <span class="text-truncate">{{ t(scope.row.actionName) }}</span>
            </template>
          </el-table-column>
          <el-table-column :label="t('Entity')" min-width="150">
            <template #default="scope">
              <span class="text-truncate">{{ t(scope.row.entity) }}</span>
            </template>
          </el-table-column>
          <el-table-column :label="t('Description')" min-width="150">
            <template v-slot="scope">
              <el-button
                v-if="scope.row.id == scope.row.description"
                type="danger"
                @click="goToViewDetail(scope.row.id)"
                link
                >{{ t("ViewDetail") }}</el-button
              >
              <span v-else v-html="scope.row.description"></span>
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