<script setup>
import {
  computed,
  onMounted,
  nextTick,
  defineProps,
  ref,
  inject,
  defineEmits,
} from "vue";
import useToggleModal from "@/register-components/actionDialog";
import { useI18n } from "vue-i18n";
import { v4 as uuidv4 } from "uuid";
import { useStore } from "vuex";
import { ElLoading, ElNotification } from "element-plus";
import useCommonFn from "@/commonFn";
import { StatusEnum } from "@/const";
import { LocationInformation, Plus, Delete } from "@element-plus/icons-vue";
import MaskNumberInput from "../MaskNumberInput.vue";

const props = defineProps({
  data: Object,
  mode: String,
});
const { newDate, ticks } = useCommonFn();
const emit = defineEmits(["callback"]);
const { t } = useI18n();
const { toggleModel } = useToggleModal();

const loadingOptions = inject("loadingOptions");
const store = useStore();
const quantity = ref(props.data?.quantity ?? 1);
const minutesPerMatch = ref(props.data?.minutesPerMatch ?? 90);
const timeSlotPerDay = ref(props.data?.timeSlotPerDay ?? 1);
const name = ref(props.data?.name ?? "");
const address = ref(props.data?.address ?? "");
const status = ref(props.data?.status ?? StatusEnum.ACTIVE);
const lstTimeFrame = ref(props.data?.timeFrameInfos ?? []);
const lstDetail = ref(props.data?.listNameDetails ?? []);
const inpName = ref(null);
const running = ref(0);
const maxTimeSlot = computed(() => {
  return 1440 / minutesPerMatch.value;
});

const nameDetail = (sortOrder) => {
  return `${t("Detail")} ${sortOrder}`;
};

const nameTimeFrame = (sortOrder) => {
  return `${t("Frame")} ${sortOrder}`;
};

const save = () => {
  if (running.value > 0) return;
  ++running.value;
  setTimeout(() => {
    running.value = 0;
  }, 1000);

  if (!name.value) {
    ElNotification({ title: t("Notification"), message: t("NameFootballFieldEmptyMesg"), type: "warning", });
    return;
  }
  if (!address.value) {
    ElNotification({ title: t("Notification"), message: t("AddressFootballFieldEmptyMesg"), type: "warning", });
    return;
  }
  if (timeSlotPerDay.value != lstTimeFrame.value.length || timeSlotPerDay.value > maxTimeSlot.value) {
    ElNotification({ title: t("Notification"), message: t("TimeSlotInvalidMesg"), type: "warning", });
    return;
  }
  if (!isValidNameDetail()) {
    ElNotification({ title: t("Notification"), message: t("NameDetailsEmptyMesg"), type: "warning", });
    return;
  }
  if (hasConflictTimeFrame()) {
    ElNotification({ title: t("Notification"), message: t("ConfictTimeFrame"), type: "warning", });
    return;
  }

  const loading = ElLoading.service(loadingOptions);
  store .dispatch(props.mode == "edit" ? "pitch/update" : "pitch/insert", 
  {
    id: props.data?.id,
    name: name.value,
    address: address.value,
    minutesPerMatch: minutesPerMatch.value,
    quantity: quantity.value,
    timeSlotPerDay: timeSlotPerDay.value,
    status: status.value,
    timeFrameInfos: lstTimeFrame.value,
    listNameDetails: lstDetail.value,
  })
  .then((res) => {
    loading.close();
    if (res.data?.success) {
      emit("callback", res);
      toggleModel();
      ElNotification({ title: t("Notification"), message: t("SaveSuccess"), type: "success", });
    } else {
      ElNotification({ title: t("Notification"), message: res?.data?.message ?? t("ErrorMesg"), type: "error", });
    }
  });
};

onMounted(() => {
  nextTick(() => {
    inpName.value.focus();
    setReadonlyInputField();
    addEvent("inpTimeSlot", decreaseTimeFrameInfosFn, increaseTimeFrameInfosFn);
    addEvent("inpQuantity", decreaseListNameDetailsFn, increaseListNameDetailsFn);
  });
});

const setReadonlyInputField = () => {
  document.getElementById("inpTimeSlot").setAttribute("readonly", true);
  document.getElementById("inpQuantity").setAttribute("readonly", true);
};

const addEvent = (name, callbackDecreaseFn, callbackIncreaseFn) => {
  let lstInp = document.getElementsByClassName(name);
  if (lstInp && lstInp.length > 0) {
    let inpElement = lstInp[0];
    inpElement
      .getElementsByClassName("el-input-number__decrease")[0]
      .addEventListener("click", callbackDecreaseFn);
    inpElement
      .getElementsByClassName("el-input-number__increase")[0]
      .addEventListener("click", callbackIncreaseFn);
  }
};

const decreaseTimeFrameInfosFn = () => {
  lstTimeFrame.value.pop();
  if (lstTimeFrame.value.length == 0) {
    lstTimeFrame.value.push(defaultTimeFrame());
  }
};

const increaseTimeFrameInfosFn = () => {
  if (lstTimeFrame.value && Array.isArray(lstTimeFrame.value)) {
    let lastItem = lstTimeFrame.value[lstTimeFrame.value.length - 1];
    let sortOrder = lastItem.sortOrder + 1;

    let timeBegin = new Date(1999, 11, 10, 0, 0, 0);
    let timeEnd = new Date(timeBegin.getTime() + minutesPerMatch.value * 60 * 1000);
    lstTimeFrame.value.push({
      id: uuidv4(),
      sortOrder: sortOrder,
      name: nameTimeFrame(sortOrder),
      price: 0,
      timeBegin: timeBegin,
      timeEnd: timeEnd,
      timeBeginTick: ticks(timeBegin),
      timeEndTick: ticks(timeEnd)
    });
  }
};

const decreaseListNameDetailsFn = () => {
  lstDetail.value.pop();
  if (lstDetail.value.length == 0) {
    lstDetail.value.push(nameDetail(1));
  }
};

const increaseListNameDetailsFn = () => {
  lstDetail.value.push(nameDetail(lstDetail.value.length + 1));
};

const changeTimeBegin = (item) => {
  item.timeEnd = newDate(item.timeBegin, minutesPerMatch.value * 60 * 1000);
  item.timeEndTick = ticks(item.timeEnd);
  item.timeBeginTick = ticks(item.timeBegin);
};

const changeTimeEnd = (item) => {
  item.timeBegin = newDate(item.timeEnd, -1 * minutesPerMatch.value * 60 * 1000);
  item.timeBeginTick = ticks(item.timeBegin);
  item.timeEndTick = ticks(item.timeEnd);
};

const hasConflictTimeFrame = () => {
  let timeLine = [];
  for (let i = 0; i < lstTimeFrame.value.length; i++) {
    const item = lstTimeFrame.value[i];
    timeLine.push({
      data: new Date(item.timeBegin),
      key: i,
    });
    timeLine.push({
      data: new Date(item.timeEnd),
      key: i,
    });
  }

  timeLine.sort((a, b) => {
    return a.data - b.data;
  });

  let counter = 0;
  for (let i = 1; i < timeLine.length; i++) {
    const current = timeLine[i];
    const prev = timeLine[i - 1];
    if (current.key != prev.key) counter++;
  }
  return counter > timeLine.length / 2 - 1;
};

const isValidNameDetail = () => {
  for (let i = 0; i < lstDetail.value.length; i++) {
    const item = lstDetail.value[i];
    if (!item) return false;
  }
  return true;
};

const addDetail = (i) => {
  lstDetail.value.splice(i + 1, 0, nameDetail(lstDetail.value.length + 1));
  quantity.value ++;
};

const removeDetail = (i) => {
  lstDetail.value.splice(i, 1);
  quantity.value --;
  if (lstDetail.value.length == 0) {
    lstDetail.value.push(nameDetail(1));
    quantity.value ++;
  }
};

const addTimeFrame = (i) => {
  lstTimeFrame.value.splice(i + 1, 0, defaultTimeFrame(lstTimeFrame.value.length + 1));
  timeSlotPerDay.value ++;
};

const removeTimeFrame = (i) => {
  lstTimeFrame.value.splice(i, 1);
  timeSlotPerDay.value --;
  if (lstTimeFrame.value.length == 0) {
    lstTimeFrame.value.push(defaultTimeFrame(1));
    timeSlotPerDay.value ++;
  }
};

const defaultTimeFrame = (sortOrder) => {
  let timeBegin = new Date(1999, 11, 10, 0, 0, 0);
  let timeEnd = new Date(timeBegin.getTime() + minutesPerMatch.value * 60 * 1000);
  return {
    id: uuidv4,
    sortOrder: sortOrder,
    name: nameTimeFrame(sortOrder),
    price: 0,
    timeBegin: timeBegin,
    timeEnd: timeEnd,
    timeBeginTick: ticks(timeBegin),
    timeEndTick: ticks(timeEnd),
  };
};
</script>


<template>
  <Dialog :title="t('FootballFieldForm')">
    <template #body>
      <el-form>
        <div class="row mb-2 ">
          <div class="col-12 col-sm-12 col-md-9">
            <div class="mx-2">
              <el-input v-model="name" :placeholder="t('NameFootballField')" maxlength="500" ref="inpName" />
            </div>
          </div>
          <div class="col-12 col-sm-12 col-md-3">
            <div class="mx-2">
              <el-select v-model="status" :placeholder="t('StatusFootballField')" class="w-100">
                <el-option :label="t('Active')" :value="StatusEnum.ACTIVE" />
                <el-option :label="t('Inactive')" :value="StatusEnum.INACTIVE" />
              </el-select>
            </div>
          </div>
        </div>
        <div class="row mb-2">
          <div class="col-11">
            <div class="mx-2">
              <el-input v-model="address" :placeholder="t('Address')" maxlength="500"/>
            </div>
          </div>
          <div class="col-1 d-flex flex-row-reverse align-items-center">
            <div class="mx-2">
              <el-icon size="24" class="pointer"><LocationInformation /></el-icon>
            </div>
          </div>
        </div>
        <div class="row">
          <div class="col-12 col-sm-12 col-md-4 mb-2">
            <div class="mx-2 d-flex flex-column align-items-start justify-content-center">
              <div class="mb-1"><b>{{ t("QuantityFootballField") }}</b></div>
              <el-input-number id="inpQuantity" class="inpQuantity w-100" v-model="quantity" :min="1" :max="100"/>
            </div>
          </div>
          <div class="col-12 col-sm-12 col-md-4 mb-2">
            <div class="mx-2 d-flex flex-column align-items-start justify-content-center">
              <div class="mb-1"><b>{{ t("MinutesPerMatch") }}</b></div>
              <el-input-number v-model="minutesPerMatch" :min="30" :max="1440" :step="30" class="w-100" />
            </div>
          </div>
          <div class="col-12 col-sm-12 col-md-4 mb-2">
            <div class="mx-2 d-flex flex-column align-items-start justify-content-center">
              <div class="mb-1"><b>{{ t("TimeSlotPerDay") }}</b></div>
              <el-input-number id="inpTimeSlot" class="inpTimeSlot w-100" v-model="timeSlotPerDay" :min="1" :max="maxTimeSlot"/>
            </div>
          </div>
        </div>
        <el-tabs type="border-card">
          <el-tab-pane :label="t('FootballFieldInfo')">
            <el-table :data="lstDetail" class="w-100">
              <el-table-column label="" width="36">
                <template #default="scope">
                  <el-button circle :icon="Plus" size="small" @click="addDetail(scope.$index)" type="secondary"></el-button>
                </template>
              </el-table-column>
              <el-table-column :label="t('NameFootballField')" :min-width="160">
                <template #default="scope">
                  <el-input v-model="lstDetail[scope.$index]" />
                </template>
              </el-table-column>
              <el-table-column label="" width="36">
                <template #default="scope">
                  <el-button circle :icon="Delete" size="small" @click="removeDetail(scope.$index)" type="danger"></el-button>
                </template>
              </el-table-column>
            </el-table>
          </el-tab-pane>
          <el-tab-pane :label="t('TimeFrameInfo')">
            <el-table :data="lstTimeFrame" class="w-100">
              <el-table-column label="" width="36">
                <template #default="scope">
                  <el-button circle :icon="Plus" size="small" @click="addTimeFrame(scope.$index)" type="secondary"></el-button>
                </template>
              </el-table-column>
              <el-table-column :label="t('TimeBegin')" width="130">
                <template #default="scope">
                  <el-time-picker v-model="scope.row.timeBegin" class="w-100" @change="changeTimeBegin(scope.row)" />
                </template>
              </el-table-column>
              <el-table-column :label="t('TimeEnd')" width="130">
                <template #default="scope">
                  <el-time-picker v-model="scope.row.timeEnd" class="w-100" @change="changeTimeEnd(scope.row)" />
                </template>
              </el-table-column>
              <el-table-column :label="t('Price')">
                <template #default="scope">
                  <mask-number-input :numberDecimal="0" :value="scope.row.price" @value="(value) => { scope.row.price = value; }" class="w-100"></mask-number-input>
                </template>
              </el-table-column>
              <el-table-column label="" width="36">
                <template #default="scope">
                  <el-button circle :icon="Delete" size="small" @click="removeTimeFrame(scope.$index)" type="danger"></el-button>
                </template>
              </el-table-column>
            </el-table>
          </el-tab-pane>
        </el-tabs>
      </el-form>
    </template>
    <template #foot>
      <span class="d-flex flex-row-reverse">
        <el-button type="primary" @click="save" class="ml-2">{{ t("Save") }}</el-button>
        <el-button @click="toggleModel">{{ t("Close") }}</el-button>
      </span>
    </template>
  </Dialog>
</template>