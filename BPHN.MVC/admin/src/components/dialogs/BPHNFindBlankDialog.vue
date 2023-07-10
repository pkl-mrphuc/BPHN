<script setup>
import { useI18n } from "vue-i18n";
import useToggleModal from "@/register-components/actionDialog";
import { defineProps, onMounted, ref, defineEmits, computed } from "vue";
import { useStore } from "vuex";
import useCommonFn from "@/commonFn";

const store = useStore();
const { t } = useI18n();
const { toggleModel } = useToggleModal();
const lstBlank = ref([]);
const { dateToString, getWeekdays } = useCommonFn();
const emits = defineEmits(["callback"]);
const props = defineProps({
  isRecurring: Boolean,
  startDate: String,
  endDate: String,
});
const pitchName = ref(null);
const timeFrameInfoName = ref(null);
const detailName = ref(null);
const weekdays = ref(null);
const listWeekday = ref([]);
const listPitch = ref([]);
const listTimeFrame = ref([]);
const listDetail = ref([]);

const formatDate = computed(() => {
  return store.getters["config/getFormatDate"];
});

const choose = (id) => {
  emits("callback", lstBlank.value.filter((item) => item.id == id)[0]);
  toggleModel();
};

const filter = () => {
  if (!localStorage.getItem("LSTBLANK")) {
    localStorage.setItem("LSTBLANK", JSON.stringify(lstBlank.value));
  }
  lstBlank.value = JSON.parse(localStorage.getItem("LSTBLANK"));

  if (pitchName.value) {
    lstBlank.value = lstBlank.value.filter(
      (item) => item.pitchName == pitchName.value
    );
  }
  if (timeFrameInfoName.value) {
    lstBlank.value = lstBlank.value.filter(
      (item) => item.timeFrameInfoName == timeFrameInfoName.value
    );
  }
  if (detailName.value) {
    lstBlank.value = lstBlank.value.filter(
      (item) => item.nameDetail == detailName.value
    );
  }
  if (weekdays.value) {
    lstBlank.value = lstBlank.value.filter(
      (item) => t(getWeekdays(item.weekendays)) == weekdays.value
    );
  }
};

onMounted(() => {
  localStorage.removeItem("LSTBLANK");
  store
    .dispatch("booking/findBlank", {
      isRecurring: props.isRecurring,
      startDate: props.startDate,
      endDate: props.endDate,
    })
    .then((res) => {
      if (res?.data?.data) {
        lstBlank.value = res.data.data;
        listPitch.value = Array.from(
          new Set(lstBlank.value.map((item) => item.pitchName))
        );
        listTimeFrame.value = Array.from(
          new Set(lstBlank.value.map((item) => item.timeFrameInfoName))
        );
        listDetail.value = Array.from(
          new Set(lstBlank.value.map((item) => item.nameDetail))
        );
        listWeekday.value = Array.from(
          new Set(lstBlank.value.map((item) => t(getWeekdays(item.weekendays))))
        );
      }
    });
});
</script>

<template>
  <Dialog :title="t('FindBlankForm')" :width="900">
    <template #body>
      <el-form>
        <el-form-item>
          <el-col :span="5">
            <el-select
              class="w-100"
              :placeholder="t('Infrastructure')"
              v-model="pitchName"
              @change="filter"
            >
              <el-option
                v-for="item in listPitch"
                :key="item"
                :label="item"
                :value="item"
              />
            </el-select>
          </el-col>
          <el-col :span="5">
            <el-select
              class="w-100"
              :placeholder="t('TimeFrame')"
              v-model="timeFrameInfoName"
              @change="filter"
            >
              <el-option
                v-for="item in listTimeFrame"
                :key="item"
                :label="item"
                :value="item"
              />
            </el-select>
          </el-col>
          <el-col :span="5">
            <el-select
              class="w-100"
              :placeholder="t('NameDetail')"
              v-model="detailName"
              @change="filter"
            >
              <el-option
                v-for="item in listDetail"
                :key="item"
                :label="item"
                :value="item"
              />
            </el-select>
          </el-col>
          <el-col :span="5">
            <el-select
              class="w-100"
              :placeholder="t('Weekdays')"
              v-model="weekdays"
              @change="filter"
            >
              <el-option
                v-for="item in listWeekday"
                :key="item"
                :label="item"
                :value="item"
              />
            </el-select>
          </el-col>
        </el-form-item>
      </el-form>
      <el-table :data="lstBlank" class="w-100" height="350">
        <el-table-column :label="t('FromDate')" width="100">
          <template #default="scope">
            <span>{{ dateToString(scope.row.startDate, formatDate) }}</span>
          </template>
        </el-table-column>
        <el-table-column :label="t('ToDate')" width="100">
          <template #default="scope">
            <span>{{ dateToString(scope.row.endDate, formatDate) }}</span>
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
        <el-table-column label="">
          <template #default="scope">
            <el-button type="primary" @click="choose(scope.row.id)">{{
              t("Choose")
            }}</el-button>
          </template>
        </el-table-column>
      </el-table>
    </template>
    <template #foot>
      <span class="dialog-footer">
        <el-button @click="toggleModel">{{ t("Close") }}</el-button>
      </span>
    </template>
  </Dialog>
</template>