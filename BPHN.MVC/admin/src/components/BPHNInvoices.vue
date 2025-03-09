<script setup>
import { useI18n } from "vue-i18n";
import { Refresh } from "@element-plus/icons-vue";

const { t } = useI18n();

const loadData = () => {
  console.log("load data");
};

const addNew = () => {
  console.log("add new");
};
</script>


<template>
  <section>
    <div class="container">
      <div class="row mb-3 d-flex flex-row align-items-center justify-content-between">
        <h3 class="fs-3 col-4 col-sm-4 col-md-4 col-lg-8">{{ t("Invoices") }}</h3>
        <div class="col-8 col-sm-8 col-md-8 col-lg-4 d-flex flex-row-reverse">
          <el-button type="primary" @click="addNew" class="ml-2">{{ t("AddNew") }}</el-button>
          <el-button @click="loadData" class="ml-2">
            <el-icon><Refresh /></el-icon>
          </el-button>
        </div>
      </div>
    </div>
  </section>
</template>