<script setup>
import useToggleModal from "@/register-components/actionDialog";
import { defineProps, ref, onMounted, defineEmits, inject } from "vue";
import { useI18n } from "vue-i18n";
import { Delete, Plus } from "@element-plus/icons-vue";
import { useStore } from "vuex";
import { CustomerTypeEnum, PaymentTypeEnum, InvoiceStatusEnum } from "@/const";
import useCommonFn from "@/commonFn";
import { ElLoading, ElNotification } from "element-plus";

const loadingOptions = inject("loadingOptions");
const { t } = useI18n();
const { toggleModel } = useToggleModal();
const store = useStore();
const emit = defineEmits(["callback"]);
const { fakeNumber } = useCommonFn();
const props = defineProps({
  data: Array,
  mode: String
});

const running = ref(0);
const lstItem = ref([]);
const status = ref(props.data?.status ?? InvoiceStatusEnum.DRAFT);
const customerType = ref(props.data?.customerType ?? CustomerTypeEnum.RETAIL);
const paymentType = ref(props.data?.paymentType ?? PaymentTypeEnum.BANK);
const customerName = ref(props.data?.customerName ?? "");
const customerPhone = ref(props.data?.customerPhone ?? "");
const total = ref(props.data?.total ?? 0);
const currentRow = ref(null);
const lstRow = ref((props.data?.items ?? []).length != 0 ? props.data.items : [{
  id: 1,
  itemName: "",
  unit: "",
  quantity: 0,
  salePrice: 0,
  total: 0
}]);

const querySearch = (queryString, cb) => {
  const results = queryString ? lstItem.value.filter(x => x.value.toLowerCase().indexOf(queryString.toLowerCase()) === 0) : lstItem.value;
  cb(results);
};

const loadAll = () => {
  if (running.value > 0) {
    return;
  }
  ++running.value;
  setTimeout(() => {
    running.value = 0;
  }, 1000);

  store.dispatch("item/getAll", null).then((res) => {
    if (res?.data?.data) {
      for (let i = 0; i < res.data.data.length; i++) {
        const element = res.data.data[i];
        lstItem.value.push({
          value: element.name,
          id: element.id,
          unit: element.unit,
          salePrice: element.salePrice
        });
      }
    }
  });
};

const handleSelect = (item) => {
  currentRow.value.itemId = item.id;
  currentRow.value.itemName = item.value;
  currentRow.value.unit = item.unit;
  currentRow.value.salePrice = item.salePrice;
  currentRow.value.total = item.salePrice * currentRow.value.quantity;
  total.value += currentRow.value.total;
};

const setItem = (row) => {
  currentRow.value = row;
  lstRow[row.id] = currentRow.value;
  total.value = sum();
};

const save = () => {
  if (running.value > 0) return;
    ++running.value;
    setTimeout(() => {
        running.value = 0;
    }, 1000);

    const loading = ElLoading.service(loadingOptions);
    store.dispatch(props.mode == "edit" ? "invoice/update" : "invoice/insert", 
    {
      id: props.data?.id,
      status: status.value,
      customerType: customerType.value,
      customerName: customerName.value,
      customerPhone: customerPhone.value,
      total: total.value,
      items: lstRow.value
    })
    .then((res) => {
        loading.close();
        if (res?.data?.success) {
            emit("callback", res);
            toggleModel();
            ElNotification({ title: t("Notification"), message: t("SaveSuccess"), type: "success" });
        } else {
            ElNotification({ title: t("Notification"), message: t("ErrorMesg"), type: "error" });
        }
    });
};

const handleChange = (row) => {
  row.total = row.salePrice * row.quantity;
  total.value = sum();
};

const add = (i) => {
  lstRow.value.splice(i + 1, 0, defaultRow());
  total.value = sum();
};

const remove = (i) => {
  lstRow.value.splice(i, 1);
  if (lstRow.value.length == 0) {
    lstRow.value.push(defaultRow());
  }
  total.value = sum();
};

const sum = () => {
  let sum = 0;
  for (let i = 0; i < lstRow.value.length; i++) {
    const element = lstRow.value[i];
    sum += element.total;
  }
  return sum;
};

const defaultRow = () => {
  return {
    id: Math.max(...lstRow.value.map(x => x.id)) + 1,
    itemName: "",
    unit: "",
    quantity: 0,
    salePrice: 0,
    total: 0
  };
};

onMounted(() => {
  loadAll();
});

</script>

<template>
  <Dialog :title="t('Invoices')">
    <template #body>
      <div class="container mb-3">
        <div class="row">
          <div class="col-12 col-sm-12 col-md-12">
            <div class="row mb-3">
              <div class="col-4 fw-bold d-flex flex-row align-items-center justify-content-start">{{ t("Status") }}</div>
              <div class="col-8">
                <el-select v-model="status" class="w-100">
                  <el-option :value="InvoiceStatusEnum.DRAFT" :label="t('DraftInvoice')" />
                </el-select>
              </div>
            </div>
            <div class="row mb-3">
              <div class="col-4 fw-bold d-flex flex-row align-items-center justify-content-start">{{ t("CustomerType") }}</div>
              <div class="col-8">
                <el-select v-model="customerType" class="w-100">
                  <el-option :value="CustomerTypeEnum.RETAIL" :label="t('RetailCustomer')" />
                  <el-option :value="CustomerTypeEnum.BOOKING" :label="t('BookingCustomer')" />
                </el-select>
              </div>
            </div>
            <div class="row mb-3">
              <div class="col-4 fw-bold d-flex flex-row align-items-center justify-content-start">{{ t("CustomerName") }}</div>
              <div class="col-8">
                <el-input v-model="customerName"></el-input>
              </div>
            </div>
            <div class="row mb-3">
              <div class="col-4 fw-bold d-flex flex-row align-items-center justify-content-start">{{ t("CustomerPhone") }}</div>
              <div class="col-8">
                <el-input v-model="customerPhone"></el-input>
              </div>
            </div>
            <div class="row mb-3">
              <div class="col-4 fw-bold d-flex flex-row align-items-center justify-content-start">{{ t("PaymentType") }}</div>
              <div class="col-8">
                <el-select v-model="paymentType" class="w-100">
                  <el-option :value="PaymentTypeEnum.BANK" :label="t('BankPayment')" />
                  <el-option :value="PaymentTypeEnum.CASH" :label="t('CashPayment')" />
                </el-select>
              </div>
            </div>
          </div>
        </div>
      </div>
      <el-table class="mb-3" :data="lstRow" style="height: 100%" :empty-text="t('NoData')">
        <el-table-column label="" width="50">
          <template #default="scope">
            <el-button circle :icon="Plus" size="small" @click="add(scope.$index)" type="secondary"></el-button>
          </template>
        </el-table-column>
        <el-table-column :label="t('ItemName')" min-width="150">
          <template #default="scope">
            <el-autocomplete v-model="scope.row.itemName" @focus="setItem(scope.row)" @select="handleSelect"
              :fetch-suggestions="querySearch" clearable class="inline-input w-100" />
          </template>
        </el-table-column>
        <el-table-column :label="t('Quantity')" width="200">
          <template #default="scope">
            <el-input-number @change="handleChange(scope.row)" v-model="scope.row.quantity" :min="0" :max="1000" />
          </template>
        </el-table-column>
        <el-table-column :label="t('Unit')" width="100">
          <template #default="scope">{{ scope.row.unit }}</template>
        </el-table-column>
        <el-table-column :label="t('SalePrice')" width="150">
          <template #default="scope">{{ fakeNumber(scope.row.salePrice) }}</template>
        </el-table-column>
        <el-table-column :label="t('Total')" width="150">
          <template #default="scope">{{ fakeNumber(scope.row.total) }}</template>
        </el-table-column>
        <el-table-column label="" width="70" fixed="right">
          <template #default="scope">
            <div class="d-flex flex-row-reverse">
              <el-button circle :icon="Delete" size="small" class="mr-2" @click="remove(scope.$index)" type="danger"></el-button>
            </div>
          </template>
        </el-table-column>
      </el-table>
      <div class="mb-3">
        <div class="row">
          <div class="col-12 d-flex flex-row align-items-center justify-content-end">
            <b>
              <i>{{ t("Total") }}: {{ fakeNumber(total) }}(đồng)</i>
            </b>
          </div>
        </div>
      </div>
    </template>
    <template #foot>
      <div class="d-flex flex-row-reverse">
        <el-button type="primary" @click="save" class="ml-2">{{ t("Save") }}</el-button>
        <el-button @click="toggleModel">{{ t("Close") }}</el-button>
      </div>
    </template>
  </Dialog>
</template>