<script setup>
import { useI18n } from "vue-i18n";
import { ref, onMounted, computed, inject } from "vue";
import { useStore } from "vuex";
import { Filter, Edit, Search } from "@element-plus/icons-vue";
import useToggleModal from "@/register-components/actionDialog";
import useCommonFn from "@/commonFn";
import { ElLoading, ElNotification } from "element-plus";
import { CustomerTypeEnum, InvoiceStatusEnum, PaymentTypeEnum } from "@/const";

const { t } = useI18n();
const { openModal, hasRole } = useToggleModal();
const { dateToString, fakeNumber, equals } = useCommonFn();

const loadingOptions = inject("loadingOptions");
const store = useStore();
const lstInvoice = ref([]);
const mode = ref("add");
const objInvoice = ref(null);
const running = ref(0);
const visible = ref(false);

const formatDate = computed(() => {
  return store.getters["config/getFormatDate"];
});

const loadData = () => {
  if (running.value > 0) return;
  ++running.value;
  setTimeout(() => {
    running.value = 0;
  }, 1000);
  
  store.dispatch("invoice/getAll", null).then((res) => {
    if (res?.data?.data) {
      lstInvoice.value = res.data.data;
    }
  });
};

const addNew = () => {
  openForm("");
  mode.value = "add";
};

const edit = (id) => {
  openForm(id);
  mode.value = "edit";
};

const openForm = (id) => {
  const loading = ElLoading.service(loadingOptions);
  store.dispatch("invoice/getInstance", id).then((res) => {
    if (res?.data?.data) {
      openModal("InvoiceDialog");
      objInvoice.value = res.data.data;
    } else {
      ElNotification({ title: t("Notification"), message: res?.data?.message ?? t("ErrorMesg"), type: "error" });
    }
    loading.close();
  });
};

const filter = () => {
  visible.value = true;
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
        <h3 class="fs-3 col-12 col-sm-12 col-md-12 col-lg-8">{{ t("Invoices") }}</h3>
        <div class="col-12 col-sm-12 col-md-12 col-lg-4 d-flex flex-row-reverse">
          <el-button type="primary" @click="addNew" class="ml-2">{{ t("AddNew") }}</el-button>
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
          <el-input v-model="txtSearch" :placeholder="t('Search')" :suffix-icon="Search" @keyup.enter="loadData"/>
        </div>
      </div>
      <div>
        <el-table :data="lstInvoice" style="height: calc(100vh - 230px)" :empty-text="t('NoData')">
          <el-table-column :label="t('Status')" width="120">
            <template #default="scope">
              <el-tag v-if="equals(scope.row.status, InvoiceStatusEnum.DRAFT)" type="info" size="small">{{ t(scope.row.status) }}</el-tag>
              <el-tag v-else type="success" size="small">{{ t(scope.row.status) }}</el-tag>
            </template>
          </el-table-column>
          <el-table-column :label="t('CustomerType')" width="200">
            <template #default="scope">
              {{ equals(scope.row.customerType, CustomerTypeEnum.RETAIL) ? t('RetailCustomer') : t('BookingCustomer') }}
            </template>
          </el-table-column>
          <el-table-column :label="t('Date')" width="200">
            <template #default="scope">{{ dateToString(scope.row.date, formatDate) }}</template>
          </el-table-column>
          <el-table-column :label="t('CustomerPhone')" width="150">
            <template #default="scope">{{ scope.row.customerPhone }}</template>
          </el-table-column>
          <el-table-column :label="t('CustomerName')">
            <template #default="scope">{{ scope.row.customerName }}</template>
          </el-table-column>
          <el-table-column :label="t('PaymentType')">
            <template #default="scope">
              {{ equals(scope.row.paymentType, PaymentTypeEnum.BANK) ? t('BankPayment') : t('CashPayment') }}
            </template>
          </el-table-column>
          <el-table-column :label="t('Total')">
            <template #default="scope">{{ fakeNumber(scope.row.total) }}</template>
          </el-table-column>
          <el-table-column label="" width="70" fixed="right">
            <template #default="scope">
              <div class="d-flex flex-row-reverse">
                <el-button circle :icon="Edit" size="small" class="mr-2" @click="edit(scope.row.id)" type="primary"></el-button>
              </div>
            </template>
          </el-table-column>
        </el-table>
      </div>
    </div>
  </section>
  <InvoiceDialog v-if="hasRole('InvoiceDialog')" :data="objInvoice" :mode="mode" @callback="loadData"></InvoiceDialog>
</template>