<script setup>
import { computed, ref, watch } from "vue";
import { useI18n } from "vue-i18n";
import { useStore } from "vuex";

const store = useStore();
const { t } = useI18n();
const darkMode = ref(store.getters["config/getDarkMode"]);
const language = ref(store.getters["config/getLanguage"]);
const formatDate = ref(store.getters["config/getFormatDate"]);
const running = ref(0);
const isLoading = ref(false);
const configData = [
  {
    name: t("DarkMode"),
    key: "DarkMode",
  },
  {
    name: t("Language"),
    key: "Language",
  },
  {
    name: t("FormatDate"),
    key: "FormatDate"
  }
];

const getDarkMode = computed(() => {
  return store.getters["config/getDarkMode"];
});

const getLanguage = computed(() => {
  return store.getters["config/getLanguage"];
});

const getFormatDate = computed(() => {
  return store.getters["config/getFormatDate"];
});

watch(getDarkMode, (newValue) => {
  darkMode.value = newValue;
});

watch(getLanguage, (newValue) => {
  language.value = newValue;
});

watch(getFormatDate, (newValue) => {
  formatDate.value = newValue;
});

const save = () => {
  if(running.value > 0) return;
  ++running.value;
  isLoading.value = true;
  let configs = [
    {
      Key: "DarkMode",
      Value: `${darkMode.value}`,
    },
    {
      Key: "Language",
      Value: language.value,
    },
    {
      Key: "FormatDate",
      Value: formatDate.value
    }
  ];
  store.dispatch("config/save", configs).then(() => {
    setTimeout(() => {
      running.value = 0;
      isLoading.value = false;
    }, 1000);
  });
};
</script>

<template>
  <section class="pbhn-screen" style="height: 100%">
    <div class="container" style="height: 100%">
      <div class="head">
        <h3 class="head_title">{{ t("Configurations") }}</h3>
        <div class="head_toolbar">
          <el-button type="primary" @click="save" :loading="isLoading">{{ t("Save") }}</el-button>
        </div>
      </div>
      <div class="body" style="margin-top: 20px">
        <el-table :data="configData" style="height: calc(100vh - 220px)">
          <el-table-column :label="t('Title')" width="200">
            <template #default="scope">
              <span>{{ scope.row.name }}</span>
            </template>
          </el-table-column>
          <el-table-column label="">
            <template #default="scope">
              <el-switch
                v-if="scope.row.key == 'DarkMode'"
                v-model="darkMode"
              />
              <el-select v-if="scope.row.key == 'Language'" v-model="language">
                <el-option value="vi" label="Vietnamese" />
                <el-option value="en" label="English" />
              </el-select>
              <el-select v-if="scope.row.key == 'FormatDate'" v-model="formatDate">
                <el-option value="yyyy-MM-dd" label="yyyy-MM-dd" />
                <el-option value="dd/MM/yyyy" label="dd/MM/yyyy" />
                <el-option value="dd-MM-yyyy" label="dd-MM-yyyy" />
              </el-select>
            </template>
          </el-table-column>
        </el-table>
      </div>
    </div>
  </section>
</template>

<style scoped>
</style>
