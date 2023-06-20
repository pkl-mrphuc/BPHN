<script setup>
import { useI18n } from "vue-i18n";
import { Refresh, Search, User } from "@element-plus/icons-vue";
import useToggleModal from "@/register-components/actionDialog";
import { useStore } from "vuex";
import { ElLoading } from "element-plus";
import { inject, ref, onMounted, computed } from "vue";
import useCommonFn from "@/commonFn";

const { t } = useI18n();
const { openModal, hasRole } = useToggleModal();
const store = useStore();
const loadingOptions = inject("loadingOptions");
const bookingForm = ref(null);
const pageIndex = ref(1);
const pageSize = ref(50);
const totalRecord = ref(0);
const txtSearch = ref("");
const { dateToString, getWeekdays } = useCommonFn();
const bmData = ref([]);
const running = ref(0);

const formatDate = computed(() => {
  return store.getters["config/getFormatDate"];
});

const addNew = () => {
  const loading = ElLoading.service(loadingOptions);
  store.dispatch("booking/getInstance", "").then((res) => {
    if (res?.data?.data) {
      openModal("BookingDialog");
      bookingForm.value = res.data.data;
    } else {
      let msg = res?.data?.message;
      alert(msg ?? t("ErrorMesg"));
    }
    loading.close();
  });
};

const loadData = () => {
  if (running.value > 0) {
    return;
  }
  ++running.value;
  store
    .dispatch("booking/getPaging", {
      pageIndex: pageIndex.value,
      pageSize: pageSize.value,
      hasBookingDetail: true,
      txtSearch: txtSearch.value,
      hasInactive: true,
    })
    .then((res) => {
      if (res?.data?.data) {
        bmData.value = res.data.data;
      }
      setTimeout(() => {
        running.value = 0;
      }, 1000);
    });

  store
    .dispatch("booking/getCountPaging", {
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

const cancel = (id) => {
  store.dispatch("bookingDetail/cancel", id).then((res) => {
    if (res?.data?.success) {
      loadData();
    } else {
      alert(t("ErrorMesg"));
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
        <h3 class="head_title">{{ t("BookingManager") }}</h3>
        <div class="head_toolbar">
          <el-input
            style="margin-right: 12px; width: 300px"
            v-model="txtSearch"
            :placeholder="t('SearchBy')"
            :suffix-icon="Search"
            @keyup.enter="loadData"
          />
          <el-button @click="loadData">
            <el-icon><Refresh /></el-icon>
          </el-button>
          <el-button type="primary" @click="addNew">{{
            t("AddNew")
          }}</el-button>
        </div>
      </div>
      <div class="body" style="margin-top: 20px">
        <el-table
          :data="bmData"
          style="height: calc(100vh - 220px)"
          :empty-text="t('NoData')"
        >
          <el-table-column type="expand">
            <template #default="props">
              <div m="4" style="margin-left: 60px">
                <el-table :data="props.row.bookingDetails" :border="false">
                  <el-table-column
                    width="150"
                    :label="t('Status')"
                    prop="status"
                  >
                    <template #default="scope">
                      <el-tag type="success" v-if="scope.row.status == 'SUCCESS'">{{ scope.row.status }}</el-tag>
                      <el-tag type="danger" v-else>{{ scope.row.status }}</el-tag>
                    </template>
                  </el-table-column>
                  <el-table-column
                    width="100"
                    :label="t('Weekdays')"
                    prop="weekendays"
                  >
                    <template #default="scope">
                      {{ t(getWeekdays(scope.row.weekendays)) }}
                    </template>
                  </el-table-column>
                  <el-table-column
                    width="200"
                    :label="t('MatchDate')"
                    prop="matchDate"
                  >
                    <template #default="scope">
                      {{ dateToString(scope.row.matchDate, formatDate) }}
                    </template>
                  </el-table-column>
                  <el-table-column :label="t('Deposite')">
                    <template #default="scope">
                      <span v-if="scope.row.deposit > 0">{{
                        scope.row.deposit
                      }}</span>
                      <span v-else></span>
                    </template>
                  </el-table-column>
                  <el-table-column label="" width="100">
                    <template #default="scope">
                      <el-button
                        :class="scope.row.id"
                        @click="cancel(scope.row.id)"
                        type="danger"
                        v-if="scope.row.status != 'CANCEL'"
                        >{{ t("Cancel") }}</el-button
                      >
                    </template>
                  </el-table-column>
                </el-table>
              </div>
            </template>
          </el-table-column>
          <el-table-column :label="t('Status')" width="150">
            <template #default="scope">
              <el-tag type="success">{{ scope.row.status }}</el-tag>
            </template>
          </el-table-column>
          <el-table-column :label="t('BookingDate')">
            <template #default="scope">
              {{ dateToString(scope.row.bookingDate, formatDate) }}
            </template>
          </el-table-column>
          <el-table-column :label="t('BookingUser')">
            <template #default="scope">
              <span
                ><el-icon :title="scope.row.email"><User /></el-icon>
                {{ scope.row.phoneNumber }}</span
              >
            </template>
          </el-table-column>
          <el-table-column :label="t('Infrastructure')">
            <template #default="scope">
              {{ scope.row.pitchName }}
            </template>
          </el-table-column>
          <el-table-column :label="t('TimeFrame')">
            <template #default="scope">
              {{ scope.row.timeFrameInfoName }}
            </template>
          </el-table-column>
          <el-table-column :label="t('NameDetail')">
            <template #default="scope">
              {{ scope.row.nameDetail }}
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
          v-if="bmData.length > 0"
          @prev-click="prevClick"
          @next-click="nextClick"
          @size-change="sizePageChange"
          @current-change="currentChange"
        />
      </div>
    </div>
  </section>
  <BookingDialog
    v-if="hasRole('BookingDialog')"
    :data="bookingForm"
    @callback="loadData"
  ></BookingDialog>
</template>

<style scoped>
.footer {
  display: flex;
  align-items: center;
  justify-content: flex-end;
  padding-top: 20px;
}

.head_toolbar {
  display: flex;
  align-items: center;
}
</style>