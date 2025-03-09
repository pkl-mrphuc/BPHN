<script setup>
import { useI18n } from "vue-i18n";
import { Refresh, Edit } from "@element-plus/icons-vue";
import { ref, onMounted } from "vue";
import useToggleModal from "@/register-components/actionDialog";

const { t } = useI18n();
const { openModal, hasRole } = useToggleModal();

const lstService = ref([]);
const mode = ref("add");
const objService = ref(null);

const loadData = () => {
  for (let index = 0; index < 100; index++) {
    lstService.value.push({
      id: index,
      code: "HH"+ index,
      name: "Hàng hóa "+index,
      quantity: 1,
      salePrice: 1000,
      status: "ACTIVE"
    });
  }
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
  openModal("ServiceDialog");
  objService.value = {id: id};
};

onMounted(() => {
  loadData();
});
</script>


<template>
  <section>
    <div class="container">
      <div class="row mb-3 d-flex flex-row align-items-center justify-content-between">
        <h3 class="fs-3 col-4 col-sm-4 col-md-4 col-lg-8">{{ t("Services") }}</h3>
        <div class="col-8 col-sm-8 col-md-8 col-lg-4 d-flex flex-row-reverse">
          <el-button type="primary" @click="addNew" class="ml-2">{{ t("AddNew") }}</el-button>
          <el-button @click="loadData" class="ml-2">
            <el-icon><Refresh /></el-icon>
          </el-button>
        </div>
      </div>
      <div>
        <el-table :data="lstService" style="height: calc(100vh - 300px)" :empty-text="t('NoData')">
          <el-table-column :label="t('Status')" width="100">
            <template #default="scope">
              <el-tag type="success" size="small">{{ t(scope.row.status) }}</el-tag>
            </template>
          </el-table-column>
          <el-table-column :label="t('Code')" width="150">
            <template #default="scope">{{ scope.row.code}}</template>
          </el-table-column>
          <el-table-column :label="t('Name')" min-width="150">
            <template #default="scope">{{ scope.row.name}}</template>
          </el-table-column>
          <el-table-column :label="t('Quantity')" min-width="50">
            <template #default="scope">{{ scope.row.quantity}}</template>
          </el-table-column>
          <el-table-column :label="t('SalePrice')" min-width="50">
            <template #default="scope">{{ scope.row.salePrice}}</template>
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
  <ServiceDialog
    v-if="hasRole('ServiceDialog')"
    :data="objService"
    :mode="mode"
    @callback="loadData"
  >
  </ServiceDialog>
</template>