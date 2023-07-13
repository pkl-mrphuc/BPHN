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
const lstWeekday = ref([]);
const lstStadium = ref([]);
const lstTimeFrame = ref([]);
const lstDetail = ref([]);

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
        lstStadium.value = Array.from(
          new Set(lstBlank.value.map((item) => item.pitchName))
        );
        lstTimeFrame.value = Array.from(
          new Set(lstBlank.value.map((item) => item.timeFrameInfoName))
        );
        lstDetail.value = Array.from(
          new Set(lstBlank.value.map((item) => item.nameDetail))
        );
        lstWeekday.value = Array.from(
          new Set(lstBlank.value.map((item) => t(getWeekdays(item.weekendays))))
        );
      }
    });
});
</script>

<template>
  <Dialog :title="t('FindBlankForm')">
    <template #body>
      <el-form>
        <div class="row">
          <div class="col-12 col-sm-12 col-md-6 col-lg-3">
            <div class="m-1">
              <el-select
                class="w-100"
                :placeholder="t('Infrastructure')"
                v-model="pitchName"
                @change="filter"
              >
                <el-option
                  v-for="item in lstStadium"
                  :key="item"
                  :label="item"
                  :value="item"
                />
              </el-select>
            </div>
          </div>
          <div class="col-12 col-sm-12 col-md-6 col-lg-3">
            <div class="m-1">
              <el-select
                class="w-100"
                :placeholder="t('TimeFrame')"
                v-model="timeFrameInfoName"
                @change="filter"
              >
                <el-option
                  v-for="item in lstTimeFrame"
                  :key="item"
                  :label="item"
                  :value="item"
                />
              </el-select>
            </div>
          </div>
          <div class="col-12 col-sm-12 col-md-6 col-lg-3">
            <div class="m-1">
              <el-select
                class="w-100"
                :placeholder="t('NameDetail')"
                v-model="detailName"
                @change="filter"
              >
                <el-option
                  v-for="item in lstDetail"
                  :key="item"
                  :label="item"
                  :value="item"
                />
              </el-select>
            </div>
          </div>
          <div class="col-12 col-sm-12 col-md-6 col-lg-3">
            <div class="m-1">
              <el-select
                class="w-100"
                :placeholder="t('Weekdays')"
                v-model="weekdays"
                @change="filter"
              >
                <el-option
                  v-for="item in lstWeekday"
                  :key="item"
                  :label="item"
                  :value="item"
                />
              </el-select>
            </div>
          </div>
        </div>
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
        <el-table-column :label="t('Weekdays')" width="80">
          <template #default="scope">
            <span class="text-truncate">{{
              t(getWeekdays(scope.row.weekendays))
            }}</span>
          </template>
        </el-table-column>
        <el-table-column :label="t('Infrastructure')" min-width="200">
          <template #default="scope">
            <span class="text-truncate">{{ scope.row.pitchName }}</span>
          </template>
        </el-table-column>
        <el-table-column :label="t('TimeFrame')" min-width="200">
          <template #default="scope">
            <span class="text-truncate">{{ scope.row.timeFrameInfoName }}</span>
          </template>
        </el-table-column>
        <el-table-column :label="t('NameDetail')" min-width="200">
          <template #default="scope">
            <span class="text-truncate">{{ scope.row.nameDetail }}</span>
          </template>
        </el-table-column>
        <el-table-column label="" width="100">
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