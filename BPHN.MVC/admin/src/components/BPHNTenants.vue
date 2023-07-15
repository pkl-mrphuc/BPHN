<script setup>
import { useI18n } from "vue-i18n";
import { Refresh, Search } from "@element-plus/icons-vue";
import { ref, onMounted, inject } from "vue";
import { useStore } from "vuex";
import useToggleModal from "@/register-components/actionDialog";
import { ElLoading } from "element-plus";
import { StatusEnum, GenderEnum } from "@/const";
import useCommonFn from "@/commonFn";

const loadingOptions = inject("loadingOptions");
const { openModal, hasRole } = useToggleModal();
const { t } = useI18n();
const store = useStore();
const { equals } = useCommonFn();
const lstAccount = ref([]);
const pageIndex = ref(1);
const pageSize = ref(50);
const totalRecord = ref(0);
const txtSearch = ref("");
const running = ref(0);
const objTenant = ref(null);
const mode = ref("add");

const loadData = () => {
  if (running.value > 0) {
    return;
  }
  ++running.value;
  store
    .dispatch("account/getPaging", {
      pageIndex: pageIndex.value,
      pageSize: pageSize.value,
      txtSearch: txtSearch.value,
    })
    .then((res) => {
      if (res?.data?.data) {
        lstAccount.value = res.data.data;
      }
      setTimeout(() => {
        running.value = 0;
      }, 1000);
    });

  store
    .dispatch("account/getCountPaging", {
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

const addNew = () => {
  openForm("");
  mode.value = "add";
};

const edit = (id) => {
  openForm(id);
  mode.value = "edit";
};

const permission = (id) => {
  alert(id);
};

const openForm = (id) => {
  const loading = ElLoading.service(loadingOptions);
  store.dispatch("account/getInstance", id).then((res) => {
    if (res?.data?.data) {
      openModal("TenantDialog");
      objTenant.value = res.data.data;
    } else {
      let msg = res?.data?.message;
      alert(msg ?? t("ErrorMesg"));
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
        <h3 class="fs-3 col-12 col-sm-12 col-md-12 col-lg-8">{{ t("Accounts") }}</h3>
        <div class="col-12 col-sm-12 col-md-12 col-lg-4 d-flex flex-row">
          <el-input
            class="ml-2"
            v-model="txtSearch"
            :placeholder="t('Search')"
            :suffix-icon="Search"
            @keyup.enter="loadData"
          />
          <el-button @click="loadData" class="ml-2">
            <el-icon><Refresh /></el-icon>
          </el-button>
          <el-button type="primary" @click="addNew" class="ml-2">{{
            t("AddNew")
          }}</el-button>
        </div>
      </div>
      <div>
        <el-table
          :data="lstAccount"
          style="height: calc(100vh - 300px)"
          :empty-text="t('NoData')"
        >
          <el-table-column :label="t('Status')" width="150">
            <template #default="scope">
              <el-tag
                type="success"
                size="small"
                v-if="equals(scope.row.status, StatusEnum.ACTIVE)"
                >{{ t("Active") }}</el-tag
              >
              <el-tag type="danger" size="small" v-else>{{
                t("Inactive")
              }}</el-tag>
            </template>
          </el-table-column>
          <el-table-column :label="t('Username')" min-width="150">
            <template #default="scope">
              <span class="text-truncate" :title="scope.row.userName">{{
                scope.row.userName
              }}</span>
            </template>
          </el-table-column>
          <el-table-column :label="t('FullName')" min-width="150">
            <template #default="scope">
              <span class="text-truncate" :title="scope.row.fullName">{{
                scope.row.fullName
              }}</span>
            </template>
          </el-table-column>
          <el-table-column :label="t('Gender')">
            <template #default="scope">
              <el-tag
                type="success"
                size="small"
                v-if="equals(scope.row.gender, GenderEnum.MALE)"
                >{{ t("Male") }}</el-tag
              >
              <el-tag
                type="danger"
                size="small"
                v-else-if="equals(scope.row.gender, GenderEnum.FEMALE)"
                >{{ t("Female") }}</el-tag
              >
              <el-tag type="info" size="small" v-else>{{ t("Other") }}</el-tag>
            </template>
          </el-table-column>
          <el-table-column :label="t('PhoneNumber')" min-width="150">
            <template #default="scope">
              <span class="text-truncate" :title="scope.row.phoneNumber">{{
                scope.row.phoneNumber
              }}</span>
            </template>
          </el-table-column>
          <el-table-column label="" min-width="200" fixed="right">
            <template #default="scope">
              <el-button size="small" @click="edit(scope.row.id)" type="primary">{{
                t("Edit")
              }}</el-button>
              <el-button size="small" @click="permission(scope.row.id)" type="info">{{
                t("Permission")
              }}</el-button>
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
          v-if="lstAccount.length > 0"
          @prev-click="prevClick"
          @next-click="nextClick"
          @size-change="sizePageChange"
          @current-change="currentChange"
        />
      </div>
    </div>
  </section>
  <TenantDialog
    v-if="hasRole('TenantDialog')"
    :data="objTenant"
    :mode="mode"
    @callback="loadData"
  >
  </TenantDialog>
</template>