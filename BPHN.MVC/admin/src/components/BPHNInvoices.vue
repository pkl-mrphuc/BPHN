<script setup>
import { useI18n } from "vue-i18n";
import { ref, onMounted, inject, watchEffect } from "vue";
import { useStore } from "vuex";
import { Filter, Edit, Search } from "@element-plus/icons-vue";
import useToggleModal from "@/register-components/actionDialog";
import useCommonFn from "@/commonFn";
import { ElLoading, ElNotification } from "element-plus";
import { CustomerTypeEnum, InvoiceStatusEnum, PaymentTypeEnum } from "@/const";
import router from "@/routers";

const { t } = useI18n();
const { openModal, hasRole } = useToggleModal();
const { dateToString, fakeNumber, equals } = useCommonFn();
const loadingOptions = inject("loadingOptions");
const store = useStore();
const formatDate = ref(store.getters["config/getFormatDate"]);
const isMobile = ref(store.getters["config/isMobile"]);
const visible = ref(false);
const running = ref(0);
const mode = ref("add");

const lstInvoice = ref([]);
const objInvoice = ref(null);

const txtSearch = ref("");
const status = ref(InvoiceStatusEnum.DRAFT);
const customerType = ref(CustomerTypeEnum.RETAIL);
const date = ref(new Date());
const paymentType = ref(PaymentTypeEnum.BANK);
const checked1 = ref(false);
const checked2 = ref(false);
const checked3 = ref(true);
const checked4 = ref(false);

watchEffect(() => { checked1.value = store.getters["cache/getInvoiceVariableCache"]?.checked1 ?? false; })
watchEffect(() => { checked2.value = store.getters["cache/getInvoiceVariableCache"]?.checked2 ?? false; })
watchEffect(() => { checked3.value = store.getters["cache/getInvoiceVariableCache"]?.checked3 ?? true; })
watchEffect(() => { checked4.value = store.getters["cache/getInvoiceVariableCache"]?.checked4 ?? false; })
watchEffect(() => { status.value = store.getters["cache/getInvoiceVariableCache"]?.status ?? InvoiceStatusEnum.DRAFT; })
watchEffect(() => { customerType.value = store.getters["cache/getInvoiceVariableCache"]?.customerType ?? CustomerTypeEnum.RETAIL; })
watchEffect(() => { date.value = store.getters["cache/getInvoiceVariableCache"]?.date ?? new Date(); })
watchEffect(() => { paymentType.value = store.getters["cache/getInvoiceVariableCache"]?.paymentType ?? PaymentTypeEnum.BANK; })

const onBack = () => {
  router.push("/");
};

const loadData = () => {
  if (running.value > 0) return;
  ++running.value;
  setTimeout(() => {
    running.value = 0;
  }, 1000);
  
  store.dispatch("invoice/getAll", 
  {
    txtSearch: txtSearch.value,
    status: checked1.value ? status.value : "",
    customerType: checked2.value ? customerType.value : "",
    date: checked3.value ? dateToString(date.value, "yyyy-MM-dd") : "",
    paymentType: checked4.value ? paymentType.value : ""
  })
  .then((res) => {
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
  visible.value = false;
  loadData();
  store.commit("cache/setInvoiceVariableCache", {
    checked1: checked1.value,
    checked2: checked2.value,
    checked3: checked3.value,
    checked4: checked4.value,
    status: status.value,
    paymentType: paymentType.value,
    customerType: customerType.value,
    date: date.value
  });
};

onMounted(() => {
  loadData();
}); 
</script>


<template>
  <section>
    <div class="container">
      <el-page-header icon="" v-if="isMobile" class="mb-3" @back="onBack">
        <template #content>
          <span class="text-large font-600 mr-3">{{ t("Invoices") }}</span>
        </template>
        <template #extra>
          <div class="d-flex flex-row">
            <el-popover :visible="visible" placement="bottom" :width="300">
              <div class="d-flex flex-column mb-3">
                <el-checkbox v-model="checked1" :label="t('Status')" size="large" />
                <div v-if="checked1 == true">
                  <el-select v-model="status" class="w-100">
                    <el-option :value="InvoiceStatusEnum.DRAFT" :label="t('DRAFT')" />
                  </el-select>
                </div>
                <el-checkbox v-model="checked2" :label="t('CustomerType')" size="large" />
                <div v-if="checked2 == true">
                  <el-select v-model="customerType" class="w-100">
                    <el-option :value="CustomerTypeEnum.RETAIL" :label="t('RetailCustomer')" />
                    <el-option :value="CustomerTypeEnum.BOOKING" :label="t('BookingCustomer')" />
                  </el-select>
                </div>
                <el-checkbox v-model="checked3" :label="t('Date')" size="large" />
                <div v-if="checked3 == true">
                  <el-date-picker type="date" class="w-100" v-model="date" />
                </div>
                <el-checkbox v-model="checked4" :label="t('PaymentType')" size="large" />
                <div v-if="checked4 == true">
                  <el-select v-model="paymentType" class="w-100">
                    <el-option :value="PaymentTypeEnum.BANK" :label="t('BankPayment')" />
                    <el-option :value="PaymentTypeEnum.CASH" :label="t('CashPayment')" />
                  </el-select>
                </div>
              </div>
              <div class="d-flex flex-row align-items-center justify-content-end">
                <el-button size="small" text @click="visible = false">{{ t('Cancel') }}</el-button>
                <el-button size="small" type="primary" @click="filter">{{ t('Filter') }}</el-button>
              </div>
              <template #reference>
                <el-button @click="visible = true" :icon="Filter" class="ml-2">
                </el-button>
              </template>
            </el-popover>
            <el-button type="primary" @click="addNew" class="ml-2">{{ t("AddNew") }}</el-button>
          </div>
        </template>
      </el-page-header>
      <div v-else class="row mb-3 d-flex flex-row align-items-center justify-content-between">
        <h3 class="col-12 col-sm-12 col-md-12 col-lg-8  fs-3 mt-1 mb-1">{{ t("Invoices") }}</h3>
        <div class="col-12 col-sm-12 col-md-12 col-lg-4 d-flex flex-row-reverse">
          <el-button type="primary" @click="addNew" class="ml-2">{{ t("AddNew") }}</el-button>
          <el-popover :visible="visible" placement="bottom" :width="300">
            <div class="d-flex flex-column mb-3">
              <el-checkbox v-model="checked1" :label="t('Status')" size="large" />
              <div v-if="checked1 == true">
                <el-select v-model="status" class="w-100">
                  <el-option :value="InvoiceStatusEnum.DRAFT" :label="t('DRAFT')" />
                </el-select>
              </div>
              <el-checkbox v-model="checked2" :label="t('CustomerType')" size="large" />
              <div v-if="checked2 == true">
                <el-select v-model="customerType" class="w-100">
                  <el-option :value="CustomerTypeEnum.RETAIL" :label="t('RetailCustomer')" />
                  <el-option :value="CustomerTypeEnum.BOOKING" :label="t('BookingCustomer')" />
                </el-select>
              </div>
              <el-checkbox v-model="checked3" :label="t('Date')" size="large" />
              <div v-if="checked3 == true">
                <el-date-picker type="date" class="w-100" v-model="date" />
              </div>
              <el-checkbox v-model="checked4" :label="t('PaymentType')" size="large" />
              <div v-if="checked4 == true">
                <el-select v-model="paymentType" class="w-100">
                  <el-option :value="PaymentTypeEnum.BANK" :label="t('BankPayment')" />
                  <el-option :value="PaymentTypeEnum.CASH" :label="t('CashPayment')" />
                </el-select>
              </div>
            </div>
            <div class="d-flex flex-row align-items-center justify-content-end">
              <el-button size="small" text @click="visible = false">{{ t('Cancel') }}</el-button>
              <el-button size="small" type="primary" @click="filter">{{ t('Filter') }}</el-button>
            </div>
            <template #reference>
              <el-button @click="visible = true" :icon="Filter" class="ml-2">
              </el-button>
            </template>
          </el-popover>
          <el-input v-model="txtSearch" :placeholder="t('Search')" :suffix-icon="Search" @keyup.enter="loadData" />
        </div>
      </div>
      <div>
        <el-table :data="lstInvoice" border style="height: calc(100vh - 156px)" :empty-text="t('NoData')">
          <el-table-column :label="t('Status')" width="120">
            <template #default="scope">
              <el-tag v-if="equals(scope.row.status, InvoiceStatusEnum.DRAFT)" type="info" size="small">{{
                t(scope.row.status) }}</el-tag>
              <el-tag v-else type="success" size="small">{{ t(scope.row.status) }}</el-tag>
            </template>
          </el-table-column>
          <el-table-column :label="t('CustomerType')" width="150">
            <template #default="scope">
              {{ equals(scope.row.customerType, CustomerTypeEnum.RETAIL) ? t('RetailCustomer') : t('BookingCustomer') }}
            </template>
          </el-table-column>
          <el-table-column :label="t('Date')" width="100">
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
                <el-button circle :icon="Edit" size="small" class="mr-2" @click="edit(scope.row.id)"
                  type="primary"></el-button>
              </div>
            </template>
          </el-table-column>
        </el-table>
      </div>
    </div>
  </section>
  <InvoiceDialog v-if="hasRole('InvoiceDialog')" :data="objInvoice" :mode="mode" @callback="loadData"></InvoiceDialog>
</template>