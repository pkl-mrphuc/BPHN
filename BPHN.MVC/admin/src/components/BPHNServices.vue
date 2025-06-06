<script setup>
import { useI18n } from "vue-i18n";
import { ref, onMounted, inject, watchEffect } from "vue";
import { useStore } from "vuex";
import { Edit, Filter, Search } from "@element-plus/icons-vue";
import useToggleModal from "@/register-components/actionDialog";
import { ElLoading, ElNotification } from "element-plus";
import useCommonFn from "@/commonFn";
import { QuantityStatusEnum, StatusEnum } from "@/const";
import router from "@/routers";

const { t } = useI18n();
const { openModal, hasRole } = useToggleModal();
const { fakeNumber, equals } = useCommonFn();
const store = useStore();
const loadingOptions = inject("loadingOptions");
const isMobile = ref(store.getters["config/isMobile"]);
const visible = ref(false);

const running = ref(0);
const mode = ref("add");
const lstItem = ref([]);
const objItem = ref(null);

const txtSearch = ref("");
const status = ref(StatusEnum.ACTIVE);
const quantityStatus = ref(QuantityStatusEnum.AVAILABLE);
const code = ref("");
const unit = ref("");

const checked1 = ref(false);
const checked2 = ref(false);
const checked3 = ref(false);
const checked4 = ref(false);

watchEffect(() => { checked1.value = store.getters["cache/getServiceVariableCache"]?.checked1 ?? false; })
watchEffect(() => { checked2.value = store.getters["cache/getServiceVariableCache"]?.checked2 ?? false; })
watchEffect(() => { checked3.value = store.getters["cache/getServiceVariableCache"]?.checked3 ?? false; })
watchEffect(() => { checked4.value = store.getters["cache/getServiceVariableCache"]?.checked4 ?? false; })
watchEffect(() => { status.value = store.getters["cache/getServiceVariableCache"]?.status ?? StatusEnum.ACTIVE; })
watchEffect(() => { quantityStatus.value = store.getters["cache/getServiceVariableCache"]?.quantityStatus ?? QuantityStatusEnum.AVAILABLE; })
watchEffect(() => { code.value = store.getters["cache/getServiceVariableCache"]?.code ?? ""; })
watchEffect(() => { unit.value = store.getters["cache/getServiceVariableCache"]?.unit ?? ""; })

const loadData = () => {
  if (running.value > 0) return;
  ++running.value;
  setTimeout(() => {
    running.value = 0;
  }, 1000);
  
  store.dispatch("item/getAll", 
  {
    txtSearch: txtSearch.value,
    code: checked2.value ? code.value : "",
    unit: checked3.value ? unit.value : "",
    status: checked1.value ? status.value : "",
    quantity: checked4.value ? quantityStatus.value : ""
  })
  .then((res) => {
    if (res?.data?.data) {
      lstItem.value = res.data.data;
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
  if (running.value > 0) return;
  ++running.value;
  setTimeout(() => {
    running.value = 0;
  }, 1000);

  const loading = ElLoading.service(loadingOptions);
  store.dispatch("item/getInstance", id).then((res) => {
    if (res?.data?.data) {
      openModal("ServiceDialog");
      objItem.value = res.data.data;
    } else {
      ElNotification({ title: t("Notification"), message: res?.data?.message ?? t("ErrorMesg"), type: "error" });
    }
    loading.close();
  });
};

const filter = () => {
  visible.value = false;
  loadData();
  store.commit("cache/setServiceVariableCache", {
    status: status.value,
    quantityStatus: quantityStatus.value,
    code: code.value,
    unit: unit.value,
    checked1: checked1.value,
    checked2: checked2.value,
    checked3: checked3.value,
    checked4: checked4.value,
  });
};

const onBack = () => {
  router.push("/");
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
          <span class="text-large font-600 mr-3">{{ t("Services") }}</span>
        </template>
        <template #extra>
          <div class="d-flex flex-row">
            <el-popover :visible="visible" placement="bottom" :width="300">
              <div class="d-flex flex-column mb-3">
                <el-checkbox v-model="checked1" :label="t('Status')" size="large" />
                <div v-if="checked1 == true">
                  <el-select v-model="status" class="w-100">
                    <el-option :label="t('ACTIVE')" :value="StatusEnum.ACTIVE" />
                    <el-option :label="t('INACTIVE')" :value="StatusEnum.INACTIVE" />
                  </el-select>
                </div>
                <el-checkbox v-model="checked2" :label="t('Code')" size="large" />
                <div v-if="checked2 == true">
                  <el-input v-model="code" maxlength="36" />
                </div>
                <el-checkbox v-model="checked3" :label="t('Unit')" size="large" />
                <div v-if="checked3 == true">
                  <el-input v-model="unit" maxlength="255" />
                </div>
                <el-checkbox v-model="checked4" :label="t('Quantity')" size="large" />
                <div v-if="checked4 == true">
                  <el-select v-model="quantityStatus" class="w-100">
                    <el-option :label="t('AVAILABLE')" :value="QuantityStatusEnum.AVAILABLE" />
                    <el-option :label="t('UNAVAILABLE')" :value="QuantityStatusEnum.UNAVAILABLE" />
                  </el-select>
                </div>
              </div>
              <div class="d-flex flex-row align-items-center justify-content-end">
                <el-button size="small" text @click="visible = false">{{ t('Cancel') }}</el-button>
                <el-button size="small" type="primary" @click="filter">{{ t('Filter') }}</el-button>
              </div>
              <template #reference>
                <el-button @click="visible = true" class="ml-2" :icon="Filter"></el-button>
              </template>
            </el-popover>
            <el-button type="primary" @click="addNew" class="ml-2">{{ t("AddNew") }}</el-button>
          </div>
        </template>
      </el-page-header>
      <div v-else class="row mb-3 d-flex flex-row align-items-center justify-content-between">
        <h3 class="col-12 col-sm-12 col-md-12 col-lg-8 fs-3 mt-1 mb-1">{{ t("Services") }}</h3>
        <div class="col-12 col-sm-12 col-md-12 col-lg-4 d-flex flex-row-reverse">
          <el-button type="primary" @click="addNew" class="ml-2">{{ t("AddNew") }}</el-button>
          <el-popover :visible="visible" placement="bottom" :width="300">
            <div class="d-flex flex-column mb-3">
              <el-checkbox v-model="checked1" :label="t('Status')" size="large" />
              <div v-if="checked1 == true">
                <el-select v-model="status" class="w-100">
                  <el-option :label="t('ACTIVE')" :value="StatusEnum.ACTIVE" />
                  <el-option :label="t('INACTIVE')" :value="StatusEnum.INACTIVE" />
                </el-select>
              </div>
              <el-checkbox v-model="checked2" :label="t('Code')" size="large" />
              <div v-if="checked2 == true">
                <el-input v-model="code" maxlength="36" />
              </div>
              <el-checkbox v-model="checked3" :label="t('Unit')" size="large" />
              <div v-if="checked3 == true">
                <el-input v-model="unit" maxlength="255" />
              </div>
              <el-checkbox v-model="checked4" :label="t('Quantity')" size="large" />
              <div v-if="checked4 == true">
                <el-select v-model="quantityStatus" class="w-100">
                  <el-option :label="t('AVAILABLE')" :value="QuantityStatusEnum.AVAILABLE" />
                  <el-option :label="t('UNAVAILABLE')" :value="QuantityStatusEnum.UNAVAILABLE" />
                </el-select>
              </div>
            </div>
            <div class="d-flex flex-row align-items-center justify-content-end">
              <el-button size="small" text @click="visible = false">{{ t('Cancel') }}</el-button>
              <el-button size="small" type="primary" @click="filter">{{ t('Filter') }}</el-button>
            </div>
            <template #reference>
              <el-button @click="visible = true" class="ml-2" :icon="Filter"></el-button>
            </template>
          </el-popover>
          <el-input v-model="txtSearch" :placeholder="t('Search')" :suffix-icon="Search" @keyup.enter="loadData" />
        </div>
      </div>
      <div>
        <el-table :data="lstItem" border style="height: calc(100vh - 156px)" :empty-text="t('NoData')">
          <el-table-column :label="t('Status')" width="150">
            <template #default="scope">
              <el-tag v-if="equals(scope.row.status, StatusEnum.ACTIVE)" type="success" size="small">{{
                t(scope.row.status) }}</el-tag>
              <el-tag v-else type="danger" size="small">{{ t(scope.row.status) }}</el-tag>
            </template>
          </el-table-column>
          <el-table-column :label="t('Code')" width="120">
            <template #default="scope">{{ scope.row.code }}</template>
          </el-table-column>
          <el-table-column :label="t('Name')">
            <template #default="scope">{{ scope.row.name }}</template>
          </el-table-column>
          <el-table-column :label="t('Unit')">
            <template #default="scope">{{ scope.row.unit }}</template>
          </el-table-column>
          <el-table-column :label="t('Quantity')">
            <template #default="scope">{{ scope.row.quantity }}</template>
          </el-table-column>
          <el-table-column :label="t('SalePrice')">
            <template #default="scope">{{ fakeNumber(scope.row.salePrice) }}</template>
          </el-table-column>
          <el-table-column label="" fixed="right" width="70">
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
  <ServiceDialog v-if="hasRole('ServiceDialog')" :data="objItem" :mode="mode" @callback="loadData"></ServiceDialog>
</template>