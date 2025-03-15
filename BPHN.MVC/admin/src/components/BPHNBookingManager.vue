<script setup>
import { useI18n } from "vue-i18n";
import {
  Filter,
  Search,
  User,
  Delete,
  Checked,
  Money
} from "@element-plus/icons-vue";
import useToggleModal from "@/register-components/actionDialog";
import { useStore } from "vuex";
import { ElLoading, ElNotification } from "element-plus";
import { inject, ref, onMounted, computed } from "vue";
import useCommonFn from "@/commonFn";
import { BookingStatusEnum, CustomerTypeEnum } from "@/const";

const { t } = useI18n();
const { openModal, hasRole } = useToggleModal();
const store = useStore();
const loadingOptions = inject("loadingOptions");
const objBooking = ref(null);
const pageIndex = ref(1);
const pageSize = ref(50);
const totalRecord = ref(0);
const txtSearch = ref("");
const { dateToString, getWeekdays, equals, fakeNumber } = useCommonFn();
const lstBooking = ref([]);
const running = ref(0);
const mode = ref("");
const visible = ref(false);
const objInvoice = ref(null);

const formatDate = computed(() => {
  return store.getters["config/getFormatDate"];
});

const addNew = () => {
  if (running.value > 0) return;
  ++running.value;
  setTimeout(() => {
    running.value = 0;
  }, 1000);

  const loading = ElLoading.service(loadingOptions);
  store.dispatch("booking/getInstance", "")
  .then((res) => {
    if (res?.data?.data) {
      mode.value = "add";
      openModal("BookingDialog");
      objBooking.value = res.data.data;
    } else {
      ElNotification({ title: t("Notification"), message: res?.data?.message ?? t("ErrorMesg"), type: "error", });
    }
    loading.close();
  });
};

const approval = (id) => {
  if (running.value > 0) return;
  ++running.value;
  setTimeout(() => {
    running.value = 0;
  }, 1000);

  const loading = ElLoading.service(loadingOptions);
  store.dispatch("booking/getInstance", id)
  .then((res) => {
    if (res?.data?.data) {
      mode.value = "approval";
      openModal("BookingDialog");
      objBooking.value = res.data.data;
    } else {
      ElNotification({ title: t("Notification"), message: res?.data?.message ?? t("ErrorMesg"), type: "error", });
    }
    loading.close();
  });
};

const pay = (data) => {
  openModal("InvoiceDialog");
  console.log(data);
  objInvoice.value = 
  {
    customerType: CustomerTypeEnum.BOOKING,
    customerPhone: data.phoneNumber,
    customerName: data.email,
    deposit: data.deposit
  };
};

const loadData = () => {
  if (running.value > 0) return;
  ++running.value;
  setTimeout(() => {
    running.value = 0;
  }, 1000);

  store.dispatch("booking/getPaging", 
  {
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
  });

  store.dispatch("booking/getCountPaging", 
  {
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
  store.dispatch("bookingDetail/cancel", id)
  .then((res) => {
    if (res?.data?.success) {
      loadData();
      ElNotification({ title: t("Notification"), message: t("SaveSuccess"), type: "success", });
    } else {
      ElNotification({ title: t("Notification"), message: res?.data?.message ?? t("ErrorMesg"), type: "error", });
    }
  });
};

const filter = () => {
  visible.value = true;
  loadData();
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
      <div class="row mb-3 d-flex flex-row align-items-center justify-content-between">
        <h3 class="fs-3 col-12 col-sm-12 col-md-12 col-lg-8">{{ t("BookingManager") }}</h3>
        <div class="col-12 col-sm-12 col-md-12 col-lg-4 d-flex flex-row">
          <el-input v-model="txtSearch" :placeholder="t('SearchBy')" :suffix-icon="Search" @keyup.enter="loadData" class="w-100"/>
          <el-popover :visible="visible" placement="bottom" :width="400">
            <div></div>
            <div class="d-flex flex-row align-items-center justify-content-end">
              <el-button size="small" text @click="visible = false">{{ t('Cancel') }}</el-button>
              <el-button size="small" type="primary" @click="visible = false">{{ t('Filter') }}</el-button>
            </div>
            <template #reference>
              <el-button @click="filter" class="ml-2">
                <el-icon><Filter /></el-icon>
              </el-button>
            </template>
          </el-popover>
          <el-button type="primary" @click="addNew" class="ml-2">{{ t("AddNew") }}</el-button>
        </div>
      </div>
      <div class="body">
        <el-table :data="lstBooking" :empty-text="t('NoData')" style="height: calc(100vh - 300px)">
          <el-table-column :label="t('Status')" width="100">
            <template #default="scope">
              <el-tag type="success" size="small" v-if="equals(scope.row.bookingStatus, BookingStatusEnum.SUCCESS)">{{ t(scope.row.bookingStatus) }}</el-tag>
              <el-tag type="info" size="small" v-else-if="equals(scope.row.bookingStatus, BookingStatusEnum.PENDING)">{{ t(scope.row.bookingStatus) }}</el-tag>
              <el-tag type="danger" size="small" v-else>{{ t(scope.row.bookingStatus) }}</el-tag>
            </template>
          </el-table-column>
          <el-table-column :label="t('BookingDate')" width="120">
            <template #default="scope">
              {{ dateToString(scope.row.bookingDate, formatDate) }}
            </template>
          </el-table-column>
          <el-table-column :label="t('BookingUser')" min-width="120">
            <template #default="scope">
              <span>
                <el-icon :title="scope.row.email"><User /></el-icon>{{ scope.row.phoneNumber }}
              </span>
            </template>
          </el-table-column>
          <el-table-column :label="t('MatchInfo')">
            <el-table-column :label="t('Status')" prop="status" width="100">
              <template #default="scope">
                <el-tag type="success" size="small" v-if="equals(scope.row.bookingDetailStatus, BookingStatusEnum.SUCCESS)">{{ t(scope.row.bookingDetailStatus) }}</el-tag>
                <el-tag type="info" size="small" v-else-if="equals(scope.row.bookingDetailStatus, BookingStatusEnum.PENDING)">{{ t(scope.row.bookingDetailStatus) }}</el-tag>
                <el-tag type="danger" v-else size="small">{{ t(scope.row.bookingDetailStatus) }}</el-tag>
              </template>
            </el-table-column>
            <el-table-column :label="t('Infrastructure')" min-width="200">
              <template #default="scope">
                <span class="text-truncate">{{ scope.row.pitchName }}</span>
              </template>
            </el-table-column>
            <el-table-column :label="t('TimeFrame')" min-width="200">
              <template #default="scope">
                <span class="text-truncate">{{ scope.row.timeFrameInfoName }}</span>
              </template>
            </el-table-column>
            <el-table-column :label="t('NameDetail')" min-width="120">
              <template #default="scope">
                <span class="text-truncate">{{ scope.row.nameDetail }}</span>
              </template>
            </el-table-column>
            <el-table-column :label="t('Weekdays')" prop="weekendays" min-width="120">
              <template #default="scope">
                {{ t(getWeekdays(scope.row.weekendays)) }}
              </template>
            </el-table-column>
            <el-table-column :label="t('MatchDate')" prop="matchDate" width="120">
              <template #default="scope">
                {{ dateToString(scope.row.matchDate, formatDate) }}
              </template>
            </el-table-column>
            <el-table-column :label="t('deposit')">
              <template #default="scope">
                <span v-if="scope.row.deposit > 0">{{ fakeNumber(scope.row.deposit) }}</span>
                <span v-else></span>
              </template>
            </el-table-column>
          </el-table-column>
          <el-table-column fixed="right" width="70">
            <template #default="scope">
              <div class="d-flex flex-row-reverse">
                <el-button class="ml-1" @click="cancel(scope.row.bookingDetailId)" type="danger" circle :icon="Delete" size="small" v-if=" !equals( scope.row.bookingDetailStatus, BookingStatusEnum.CANCEL) && !equals(scope.row.bookingDetailStatus, BookingStatusEnum.PENDING)"></el-button>
                <el-button class="ml-1" @click="approval(scope.row.bookingId)" type="warning" v-if="equals(scope.row.bookingStatus, BookingStatusEnum.PENDING)" circle :icon="Checked" size="small" ></el-button>
                <el-button class="ml-1" @click="pay(scope.row)" type="success" circle :icon="Money" size="small" v-if=" equals( scope.row.bookingDetailStatus, BookingStatusEnum.SUCCESS)"></el-button>
              </div>
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
  <BookingDialog v-if="hasRole('BookingDialog')" :data="objBooking" :mode="mode" @callback="loadData"></BookingDialog>
  <InvoiceDialog v-if="hasRole('InvoiceDialog')" :data="objInvoice" :mode="'add'"></InvoiceDialog>
</template>