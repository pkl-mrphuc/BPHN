<script setup>
import { useI18n } from "vue-i18n";
import { Refresh, Search } from "@element-plus/icons-vue";
import { ref, onMounted, inject } from "vue";
import { useStore } from "vuex";
import useToggleModal from "@/register-components/actionDialog";
import { ElLoading } from "element-plus";

const loadingOptions = inject("loadingOptions");
const { openModal, hasRole } = useToggleModal();
const { t } = useI18n();
const store = useStore();
const accountData = ref([]);
const pageIndex = ref(1);
const pageSize = ref(50);
const totalRecord = ref(0);
const txtSearch = ref("");
const running = ref(0);
const tenantDataForm = ref(null);
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
        accountData.value = res.data.data;
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
};

const edit = (id) => {
  openForm(id);
  mode.value = "edit";
};

const openForm = (id) => {
  const loading = ElLoading.service(loadingOptions);
  store.dispatch("account/getInstance", id).then((res) => {
    if (res?.data?.data) {
      openModal("TenantDialog");
      tenantDataForm.value = res.data.data;
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
  <section class="pbhn-screen" style="height: 100%">
    <div class="container" style="height: 100%">
      <div class="head">
        <h3 class="head_title">{{ t("Accounts") }}</h3>
        <div class="head_toolbar">
          <el-input
            style="margin-right: 12px; width: 300px"
            v-model="txtSearch"
            :placeholder="t('Search')"
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
      <div class="body" style="margin-top: 20px">
        <el-table
          :data="accountData"
          style="height: calc(100vh - 220px)"
          :empty-text="t('NoData')"
        >
          <el-table-column :label="t('Status')">
            <template #default="scope">
              <el-tag type="success" v-if="scope.row.status == 'ACTIVE'">{{
                t("ACTIVE")
              }}</el-tag>
              <el-tag type="danger" v-else>{{ t("INACTIVE") }}</el-tag>
            </template>
          </el-table-column>
          <el-table-column :label="t('Username')">
            <template #default="scope">
              {{ scope.row.userName }}
            </template>
          </el-table-column>
          <el-table-column :label="t('FullName')">
            <template #default="scope">
              {{ scope.row.fullName }}
            </template>
          </el-table-column>
          <el-table-column :label="t('Gender')">
            <template #default="scope">
              <el-tag type="success" v-if="scope.row.gender == 'MALE'">{{
                t("Male")
              }}</el-tag>
              <el-tag type="danger" v-else-if="scope.row.gender == 'FEMALE'">{{
                t("Female")
              }}</el-tag>
              <el-tag type="info" v-else>{{ t("Other") }}</el-tag>
            </template>
          </el-table-column>
          <el-table-column :label="t('PhoneNumber')">
            <template #default="scope">
              {{ scope.row.phoneNumber }}
            </template>
          </el-table-column>
          <el-table-column label="">
            <template #default="scope">
              <el-button
                @click="edit(scope.row.id)"
                type="primary"
                >{{ t("Edit") }}</el-button
              >
            </template>
          </el-table-column>
        </el-table>
      </div>
      <div class="footer">
        <el-pagination
          background
          v-model:current-page="pageIndex"
          v-model:page-size="pageSize"
          layout="sizes, prev, pager, next"
          :total="totalRecord"
          v-if="accountData.length > 0"
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
    :data="tenantDataForm"
    :mode="mode"
    @callback="loadData"
  >
  </TenantDialog>
</template>

<style scoped>
.footer {
  display: flex;
  align-items: center;
  justify-content: flex-end;
  padding-top: 20px;
}
</style>