<script setup>
import { useI18n } from "vue-i18n";
const { t } = useI18n();
</script>

<template>
  <Dialog :title="t('BookingForm')" :width="1200">
    <template #body>
      <el-form>
        
      </el-form>
    </template>
    <template #foot>
      <span class="dialog-footer">
        <el-button @click="toggleModel">{{ t("Close") }}</el-button>
        <el-button type="primary" @click="save">{{ t("Save") }}</el-button>
      </span>
    </template>
  </Dialog>
</template>