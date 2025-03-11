<script setup>
import { useI18n } from "vue-i18n";
import { ref, onMounted } from "vue";
import { Refresh, Edit } from "@element-plus/icons-vue";
import useToggleModal from "@/register-components/actionDialog";

const { t } = useI18n();
const { openModal, hasRole } = useToggleModal();

const lstInvoice = ref([]);
const mode = ref("add");
const objInvoice = ref(null);

const loadData = () => {
  lstInvoice.value = [];
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
  openModal("InvoiceDialog");
  console.log(id)
};

onMounted(() => {
  loadData();
});
</script>


<template>
  <section>
    <div class="container">
      <div class="row mb-3 d-flex flex-row align-items-center justify-content-between">
        <h3 class="fs-3 col-4 col-sm-4 col-md-4 col-lg-8">{{ t("Invoices") }}</h3>
        <div class="col-8 col-sm-8 col-md-8 col-lg-4 d-flex flex-row-reverse">
          <el-button type="primary" @click="addNew" class="ml-2">{{ t("AddNew") }}</el-button>
          <el-button @click="loadData" class="ml-2">
            <el-icon><Refresh /></el-icon>
          </el-button>
        </div>
      </div>
      <div>
        <el-table :data="lstInvoice" style="height: calc(100vh - 230px)" :empty-text="t('NoData')">
          <el-table-column :label="t('Status')" width="100">
            <template #default="scope">{{ scope.row.status}}</template>
          </el-table-column>
          <el-table-column :label="t('Date')" width="120">
            <template #default="scope">{{ t(scope.row.date) }}</template>
          </el-table-column>
          <el-table-column :label="t('CustomerPhone')" width="150">
            <template #default="scope">{{ t(scope.row.customerPhone) }}</template>
          </el-table-column>
          <el-table-column :label="t('CustomerName')">
            <template #default="scope">{{ t(scope.row.customerName) }}</template>
          </el-table-column>
          <el-table-column :label="t('Total')" width="150">
            <template #default="scope">{{ scope.row.total}}</template>
          </el-table-column>
          <el-table-column label="" width="70" fixed="right">
            <template #default="scope">
              <div class="d-flex flex-row-reverse">
                <el-button
                  circle
                  :icon="Edit"
                  size="small"
                  class="mr-2"
                  @click="edit(scope.row.id)"
                  type="primary"
                ></el-button>
              </div>
            </template>
          </el-table-column>
        </el-table>
      </div>
    </div>
  </section>
  <InvoiceDialog
    v-if="hasRole('InvoiceDialog')"
    :data="objInvoice"
    :mode="mode"
    @callback="loadData"
  >
  </InvoiceDialog>
</template>