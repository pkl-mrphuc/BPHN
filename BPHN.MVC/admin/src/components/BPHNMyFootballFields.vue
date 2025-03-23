<script setup>
import { useI18n } from "vue-i18n";
import FootballFieldCard from "@/components/FootballFieldCard.vue";
import useToggleModal from "@/register-components/actionDialog";
import { ElLoading, ElNotification } from "element-plus";
import { onMounted, inject, ref, computed } from "vue";
import { useStore } from "vuex";
import { Refresh } from "@element-plus/icons-vue";
import router from "@/routers";

const { t } = useI18n();
const store = useStore();
const { openModal, hasRole } = useToggleModal();
const loadingOptions = inject("loadingOptions");
const objStadium = ref(null);
const lstStadium = ref([]);
const mode = ref("add");
const running = ref(0);

const isMobile = computed(() => {
  return store.getters["config/isMobile"];
});

const onBack = () => {
  router.push("overview");
};

onMounted(() => {
  loadData();
});

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
  store.dispatch("pitch/getInstance", id).then((res) => {
    if (res?.data?.data) {
      openModal("FootballFieldDialog");
      objStadium.value = res.data.data;
    } else {
      ElNotification({ title: t("Notification"), message: res?.data?.message ?? t("ErrorMesg"), type: "error" })
    }
    loading.close();
  });
};

const loadData = () => {
  if (running.value > 0) return;
  ++running.value;
  setTimeout(() => {
    running.value = 0;
  }, 1000);

  const loading = ElLoading.service(loadingOptions);
  store.dispatch("pitch/getPaging", 
  {
    accountId: store.getters["account/getAccountId"],
    hasDetail: false,
    hasInactive: true,
    pageIndex: 1,
    pageSize: 1000
  })
  .then((res) => {
    loading.close();
    lstStadium.value = res?.data?.data ?? [];
  });
};
</script>

<template>
  <section>
    <div class="container">
      <el-page-header icon="" v-if="isMobile" class="mb-3" @back="onBack">
        <template #content>
          <span class="text-large font-600 mr-3">{{ t("MyFootballFields") }}</span>
        </template>
        <template #extra>
          <div class="d-flex flex-row">
            <el-button @click="loadData">
              <el-icon>
                <Refresh />
              </el-icon>
            </el-button>
            <el-button type="primary" @click="addNew" class="ml-2">{{ t("AddNew") }}</el-button>
          </div>
        </template>
      </el-page-header>
      <div v-else class="row mb-3 d-flex flex-row align-items-center justify-content-between">
        <h3 class="col-12 col-sm-12 col-md-12 col-lg-8 fs-3 mt-1 mb-1">{{ t("MyFootballFields") }}</h3>
        <div class="col-12 col-sm-12 col-md-12 col-lg-4 d-flex flex-row-reverse">
          <el-button type="primary" @click="addNew" class="ml-2">{{ t("AddNew") }}</el-button>
          <el-button @click="loadData">
            <el-icon>
              <Refresh />
            </el-icon>
          </el-button>
        </div>
      </div>
      <div style="height: calc(100vh - 190px); overflow: scroll">
        <div class="row">
          <div v-for="item in lstStadium" :key="item.id"
            class="mb-3 mr-3 col-12 col-sm-12 col-md-6 col-lg-4 col-xl-4 col-xxl-3">
            <football-field-card :name="item.name" :status="item.status" :id="item.id" :avatarUrl="item.avatarUrl"
              @edit="edit"></football-field-card>
          </div>
        </div>
        <el-empty :description="t('NoData')" v-if="lstStadium.length == 0" />
      </div>
    </div>
  </section>

  <FootballFieldDialog v-if="hasRole('FootballFieldDialog')" :data="objStadium" :mode="mode" @callback="loadData">
  </FootballFieldDialog>
</template>
