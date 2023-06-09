<script setup>
import { useI18n } from "vue-i18n";
import { Refresh, Search, User } from "@element-plus/icons-vue";
import useToggleModal from "@/register-components/actionDialog";
import { useStore } from "vuex";
import { ElLoading } from "element-plus";
import { inject, ref, onMounted, computed } from "vue";
import useCommonFn from "@/commonFn";
import { BookingStatusEnum } from "@/const";

const { t } = useI18n();
const { openModal, hasRole } = useToggleModal();
const store = useStore();
const loadingOptions = inject("loadingOptions");
const objBooking = ref(null);
const pageIndex = ref(1);
const pageSize = ref(50);
const totalRecord = ref(0);
const txtSearch = ref("");
const { dateToString, getWeekdays, equals } = useCommonFn();
const lstBooking = ref([]);
const running = ref(0);

const formatDate = computed(() => {
  return store.getters["config/getFormatDate"];
});

const addNew = () => {
  const loading = ElLoading.service(loadingOptions);
  store.dispatch("booking/getInstance", "").then((res) => {
    if (res?.data?.data) {
      openModal("BookingDialog");
      objBooking.value = res.data.data;
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
        lstBooking.value = res.data.data;
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
  <section>
    <div class="container">
      <div
        class="row mb-3 d-flex flex-row align-items-center justify-content-between"
      >
        <h3 class="fs-3 col-12 col-sm-12 col-md-12 col-lg-8">
          {{ t("BookingManager") }}
        </h3>
        <div class="col-12 col-sm-12 col-md-12 col-lg-4 d-flex flex-row">
          <el-input
            v-model="txtSearch"
            :placeholder="t('SearchBy')"
            :suffix-icon="Search"
            @keyup.enter="loadData"
            class="w-100"
          />
          <el-button @click="loadData" class="ml-2">
            <el-icon><Refresh /></el-icon>
          </el-button>
          <el-button type="primary" @click="addNew" class="ml-2">{{
            t("AddNew")
          }}</el-button>
        </div>
      </div>
      <div class="body">
        <el-table
          :data="lstBooking"
          :empty-text="t('NoData')"
          style="height: calc(100vh - 300px)"
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
                      <el-tag
                        type="success"
                        size="small"
                        v-if="
                          equals(scope.row.status, BookingStatusEnum.SUCCESS)
                        "
                        >{{ scope.row.status }}</el-tag
                      >
                      <el-tag
                        type="info"
                        size="small"
                        v-else-if="
                          equals(scope.row.status, BookingStatusEnum.PENDING)
                        "
                        >{{ scope.row.status }}</el-tag
                      >
                      <el-tag type="danger" v-else size="small">{{
                        scope.row.status
                      }}</el-tag>
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
                        v-if="
                          !equals(scope.row.status, BookingStatusEnum.CANCEL)
                        "
                        >{{ t("Cancel") }}</el-button
                      >
                    </template>
                  </el-table-column>
                </el-table>
              </div>
            </template>
          </el-table-column>
          <el-table-column :label="t('Status')" width="100">
            <template #default="scope">
              <el-tag
                type="success"
                size="small"
                v-if="equals(scope.row.status, BookingStatusEnum.SUCCESS)"
                >{{ scope.row.status }}</el-tag
              >
              <el-tag
                type="info"
                size="small"
                v-else-if="equals(scope.row.status, BookingStatusEnum.PENDING)"
                >{{ scope.row.status }}</el-tag
              >
              <el-tag type="danger" size="small" v-else>{{
                scope.row.status
              }}</el-tag>
            </template>
          </el-table-column>
          <el-table-column :label="t('BookingDate')" width="100">
            <template #default="scope">
              {{ dateToString(scope.row.bookingDate, formatDate) }}
            </template>
          </el-table-column>
          <el-table-column :label="t('BookingUser')" min-width="200">
            <template #default="scope">
              <span
                ><el-icon :title="scope.row.email"><User /></el-icon>
                {{ scope.row.phoneNumber }}</span
              >
            </template>
          </el-table-column>
          <el-table-column :label="t('Infrastructure')" min-width="200">
            <template #default="scope">
              <span class="text-truncate">{{ scope.row.pitchName }}</span>
            </template>
          </el-table-column>
          <el-table-column :label="t('TimeFrame')" min-width="200">
            <template #default="scope">
              <span class="text-truncate">{{
                scope.row.timeFrameInfoName
              }}</span>
            </template>
          </el-table-column>
          <el-table-column :label="t('NameDetail')">
            <template #default="scope">
              <span class="text-truncate">{{ scope.row.nameDetail }}</span>
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
          v-if="lstBooking.length > 0"
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
    :data="objBooking"
    @callback="loadData"
  ></BookingDialog>
</template>