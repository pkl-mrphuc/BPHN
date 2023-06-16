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
const pitchDataForm = ref(null);
const listPitch = ref([]);
const mode = ref("add");

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
      pitchDataForm.value = res.data.data;
    } else {
      let msg = res?.data?.message;
      alert(msg ?? t("ErrorMesg"));
    }
    loading.close();
  });
};

const loadData = () => {
  const loading = ElLoading.service(loadingOptions);
  store
    .dispatch("pitch/getPaging", {
      accountId: store.getters["account/getAccountId"],
      hasDetail: false,
      hasInactive: true,
    })
    .then((res) => {
      loading.close();
      listPitch.value = res?.data?.data ?? [];
    });
};
</script>

<template>
  <section class="pbhn-screen" style="height: 100%">
    <div class="container" style="height: 100%">
      <div class="head">
        <h3 class="head_title">{{ t("MyFootballFields") }}</h3>
        <div class="head_toolbar">
          <el-button @click="loadData">
            <el-icon><Refresh /></el-icon>
          </el-button>
          <el-button type="primary" @click="addNew">{{
            t("AddNew")
          }}</el-button>
        </div>
      </div>
      <div class="body" style="height: calc(100vh - 160px); overflow: scroll; margin-top: 20px">
        <el-row>
          <el-col
            v-for="item in listPitch"
            :key="item.id"
            :span="7"
            class="football-field-card"
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
      </div>
    </div>
  </section>
  <el-empty :description="t('NoData')" v-if="listPitch.length == 0" />
  <FootballFieldDialog
    v-if="hasRole('FootballFieldDialog')"
    :data="pitchDataForm"
    :mode="mode"
    @callback="loadData"
  >
  </FootballFieldDialog>
</template>

<style scoped>
.football-field-card {
  margin-bottom: 40px;
  margin-right: 40px;
}
</style>
