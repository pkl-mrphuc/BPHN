<script setup>
import { useI18n } from "vue-i18n";
import useToggleModal from "@/register-components/actionDialog";

const { t } = useI18n();
const { toggleModel } = useToggleModal();
</script>


<template>
  <Dialog :title="t('AccountInfoForm')">
    <template #body> </template>
    <template #foot>
      <span class="dialog-footer">
        <el-button @click="toggleModel">{{ t("Close") }}</el-button>
      </span>
    </template>
  </Dialog>
</template>