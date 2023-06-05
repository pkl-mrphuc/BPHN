<script setup>
import { useI18n } from "vue-i18n";
import { Refresh } from "@element-plus/icons-vue";

const { t } = useI18n();

const addNew = () => {
  alert("thêm mới");
};
</script>

<template>
  <section class="pbhn-screen pbhn-bm">
    <div class="container">
      <div class="head">
        <h3 class="head_title">{{ t("BookingManager") }}</h3>
        <div class="head_toolbar">
          <el-button>
            <el-icon><Refresh /></el-icon>
          </el-button>
          <el-button type="primary" @click="addNew">{{
            t("AddNew")
          }}</el-button>
        </div>
      </div>
      <div class="body"></div>
    </div>
  </section>
  <el-empty :description="t('NoData')" v-if="true" />
</template>

