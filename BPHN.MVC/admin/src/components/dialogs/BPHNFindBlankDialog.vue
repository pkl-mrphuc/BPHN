<script setup>
import { useI18n } from "vue-i18n";
import useToggleModal from "@/register-components/actionDialog";
import { defineProps, onMounted, ref } from "vue";
import { useStore } from "vuex";
import useCommonFn from "@/commonFn";

const store = useStore();
const { t } = useI18n();
const { toggleModel } = useToggleModal();
const lstBlank = ref([]);
const { dateToString, getWeekdays } = useCommonFn();
const props = defineProps({
  isRecurring: Boolean,
  startDate: String,
  endDate: String,
});

const choose = () => {
  alert("choose");
};

onMounted(() => {
  store
    .dispatch("booking/findBlank", {
      isRecurring: props.isRecurring,
      startDate: props.startDate,
      endDate: props.endDate,
    })
    .then((res) => {
      if (res?.data?.data) {
        lstBlank.value = res.data.data;
      }
    });
});
</script>

<template>
  <Dialog :title="t('FindBlankForm')" :width="900">
    <template #body>
      <el-table :data="lstBlank" class="w100" height="350">
        <el-table-column label="" width="50">
          <template #default="scope">
            <el-checkbox :checkbox_value="scope.row.id" />
          </template>
        </el-table-column>
        <el-table-column :label="t('FromDate')" width="100">
          <template #default="scope">
            <span>{{ dateToString(scope.row.startDate) }}</span>
          </template>
        </el-table-column>
        <el-table-column :label="t('ToDate')" width="100">
          <template #default="scope">
            <span>{{ dateToString(scope.row.endDate) }}</span>
          </template>
        </el-table-column>
        <el-table-column :label="t('Weekdays')">
          <template #default="scope">
            <span>{{ t(getWeekdays(scope.row.weekendays)) }}</span>
          </template>
        </el-table-column>
        <el-table-column :label="t('Infrastructure')">
          <template #default="scope">
            <span>{{ scope.row.pitchName }}</span>
          </template>
        </el-table-column>
        <el-table-column :label="t('TimeFrame')">
          <template #default="scope">
            <span>{{ scope.row.timeFrameInfoName }}</span>
          </template>
        </el-table-column>
        <el-table-column :label="t('NameDetail')">
          <template #default="scope">
            <span>{{ scope.row.nameDetail }}</span>
          </template>
        </el-table-column>
      </el-table>
    </template>
    <template #foot>
      <span class="dialog-footer">
        <el-button @click="toggleModel">{{ t("Close") }}</el-button>
        <el-button type="primary" @click="choose">{{ t("Choose") }}</el-button>
      </span>
    </template>
  </Dialog>
</template>

<style scoped>
</style>