<script setup>
import { useI18n } from "vue-i18n";
import { ref, onMounted, inject } from "vue";
import { useStore } from "vuex";
import { Refresh, Edit } from "@element-plus/icons-vue";
import useToggleModal from "@/register-components/actionDialog";
import { ElLoading, ElNotification } from "element-plus";

const { t } = useI18n();
const { openModal, hasRole } = useToggleModal();
const store = useStore();
const loadingOptions = inject("loadingOptions");

const lstItem = ref([]);
const mode = ref("add");
const objItem = ref(null);
const running = ref(0);

const loadData = () => {
  if (running.value > 0) {
    return;
  }
  ++running.value;
  store.dispatch("item/getAll", null).then((res) => {
    if (res?.data?.data) {
      lstItem.value = res.data.data;
    }
    setTimeout(() => {
      running.value = 0;
    }, 1000);
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
  store.dispatch("item/getInstance", id).then((res) => {
    if (res?.data?.data) {
      openModal("ServiceDialog");
      objItem.value = res.data.data;
    } else {
      ElNotification({
        title: t("Notification"),
        message: res?.data?.message ?? t("ErrorMesg"),
        type: "error"
      })
    }
    loading.close();
  });
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
        <el-table :data="lstItem" style="height: calc(100vh - 230px)" :empty-text="t('NoData')">
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
          <el-table-column :label="t('Unit')" width="150">
            <template #default="scope">{{ scope.row.unit}}</template>
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
    :data="objItem"
    :mode="mode"
    @callback="loadData"
  >
  </ServiceDialog>
</template>