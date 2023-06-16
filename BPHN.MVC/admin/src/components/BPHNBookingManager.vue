<script setup>
import { useI18n } from "vue-i18n";
import { Refresh, Search, User } from "@element-plus/icons-vue";
import useToggleModal from "@/register-components/actionDialog";
import { useStore } from "vuex";
import { ElLoading } from "element-plus";
import { inject, ref, onMounted } from "vue";
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

const bmData = ref([]);

const loadData = () => {
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

onMounted(() => {
  loadData();
});
</script>

<template>
  <section class="pbhn-screen pbhn-bm h-full-screen">
    <div class="container h-full-screen">
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
      <div
        class="body"
        style="height: calc(100% - 32px - 20px - 32px); overflow: scroll"
      >
        <el-table :data="bmData" style="width: 100%">
          <el-table-column type="expand">
            <template #default="props">
              <div m="4" style="margin-left: 60px">
                <el-table :data="props.row.bookingDetails" :border="false">
                  <el-table-column
                    width="100"
                    :label="t('Status')"
                    prop="status"
                  >
                    <template #default="scope">
                      {{ scope.row.status }}
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
                      {{ dateToString(scope.row.matchDate) }}
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
          <el-table-column :label="t('Status')" width="100">
            <template #default="scope">
              {{ scope.row.status }}
            </template>
          </el-table-column>
          <el-table-column :label="t('BookingDate')">
            <template #default="scope">
              {{ dateToString(scope.row.bookingDate) }}
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
          layout="total, sizes, prev, pager, next, jumper"
          :page-sizes="[10, 20, 30, 50, 100]"
          :total="totalRecord"
          v-if="bmData.length > 0"
        />
      </div>
    </div>
  </section>
  <el-empty :description="t('NoData')" v-if="bmData.length == 0" />
  <BookingDialog
    v-if="hasRole('BookingDialog')"
    :data="bookingForm"
    @callback="loadData"
  ></BookingDialog>
</template>

<style scoped>
.h-full-screen {
  height: 100%;
}

.footer {
  display: flex;
  align-items: center;
  justify-content: flex-end;
}

.head_toolbar {
  display: flex;
  align-items: center;
}
</style>