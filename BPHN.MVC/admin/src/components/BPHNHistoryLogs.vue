<script setup>
import { useI18n } from "vue-i18n";
import { Refresh, Search } from "@element-plus/icons-vue";

const { t } = useI18n();

const loadData = () => {
    console.log("abc");
};
</script>


<template>
  <section class="pbhn-screen" style="height: 100%">
    <div class="container" style="height: 100%">
      <div class="head">
        <h3 class="head_title">{{ t("HistoryLog") }}</h3>
        <div class="head_toolbar">
          <el-input
            style="margin-right: 12px; width: 300px"
            v-model="txtSearch"
            :placeholder="t('SearchBy')"
            :suffix-icon="Search"
            @keyup.enter="loadData"
          />
          <el-button @click="loadData">
            <el-icon><Refresh /></el-icon>
          </el-button>
        </div>
      </div>
      <div class="body" style="margin-top: 20px">
        
      </div>
    </div>
  </section>
</template>