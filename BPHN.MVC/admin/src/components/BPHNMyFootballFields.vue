<script setup>
import { useI18n } from "vue-i18n";
import FootballFieldCard from "@/components/FootballFieldCard.vue";
import useToggleModal from "@/register-components/actionDialog";
import { ElLoading } from "element-plus";
import { onMounted, inject, ref } from "vue";
import { useStore } from "vuex";
import { Refresh } from "@element-plus/icons-vue";

const { t } = useI18n();
const store = useStore();
const { openModal, hasRole } = useToggleModal();
const loadingOptions = inject("loadingOptions");
const objStadium = ref(null);
const lstStadium = ref([]);
const mode = ref("add");
const running = ref(0);

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
  const loading = ElLoading.service(loadingOptions);
  store.dispatch("pitch/getInstance", id).then((res) => {
    if (res?.data?.data) {
      openModal("FootballFieldDialog");
      objStadium.value = res.data.data;
    } else {
      let msg = res?.data?.message;
      alert(msg ?? t("ErrorMesg"));
    }
    loading.close();
  });
};

const loadData = () => {
  if(running.value > 0) return;
  ++running.value;
  const loading = ElLoading.service(loadingOptions);
  store
    .dispatch("pitch/getPaging", {
      accountId: store.getters["account/getAccountId"],
      hasDetail: false,
      hasInactive: true,
    })
    .then((res) => {
      loading.close();
      setTimeout(() => {
        running.value = 0;
      }, 1000);
      lstStadium.value = res?.data?.data ?? [];
    });
};
</script>

<template>
  <section>
    <div class="container">
      <div class="d-flex flex-row align-items-center justify-content-between">
        <h3 class="fs-3">{{ t("MyFootballFields") }}</h3>
        <div class="d-flex flex-row">
          <el-button @click="loadData">
            <el-icon><Refresh /></el-icon>
          </el-button>
          <el-button type="primary" @click="addNew" class="ml-2">{{
            t("AddNew")
          }}</el-button>
        </div>
      </div>
      <div style="height: calc(100vh - 190px); overflow: scroll;">
        <el-row>
          <el-col
            v-for="item in lstStadium"
            :key="item.id"
            :span="7"
            class="mb-3 mr-3"
          >
            <football-field-card
              :name="item.name"
              :status="item.status"
              :id="item.id"
              :avatarUrl="item.avatarUrl"
              @edit="edit"
            ></football-field-card>
          </el-col>
        </el-row>
        <el-empty :description="t('NoData')" v-if="lstStadium.length == 0" />
      </div>
    </div>
  </section>
  
  <FootballFieldDialog
    v-if="hasRole('FootballFieldDialog')"
    :data="objStadium"
    :mode="mode"
    @callback="loadData"
  >
  </FootballFieldDialog>
</template>
