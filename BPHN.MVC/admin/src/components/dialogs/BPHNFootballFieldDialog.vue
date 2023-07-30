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
import { LocationInformation } from "@element-plus/icons-vue";
import MaskNumberInput from "../MaskNumberInput.vue";

const props = defineProps({
  data: Object,
  mode: String,
});
const { newDate, ticks, equals } = useCommonFn();
const emit = defineEmits(["callback"]);
const loadingOptions = inject("loadingOptions");
const { t } = useI18n();
const store = useStore();
const { toggleModel } = useToggleModal();
const lstConfigTimeFrame = ref([
  {
    name: t("Price"),
    key: "Price",
  },
  {
    name: t("TimeBegin"),
    key: "TimeBegin",
  },
  {
    name: t("TimeEnd"),
    key: "TimeEnd",
  },
]);
const lstConfigInfo = ref([
  {
    name: t("NameFootballField"),
    key: "Name",
  },
]);
const quantity = ref(props.data?.quantity ?? 1);
const minutesPerMatch = ref(props.data?.minutesPerMatch ?? 90);
const timeSlotPerDay = ref(props.data?.timeSlotPerDay ?? 1);
const name = ref(props.data?.name ?? "");
const address = ref(props.data?.address ?? "");
const status = ref(props.data?.status ?? StatusEnum.ACTIVE);
const lstTimeFrame = ref(props.data?.timeFrameInfos);
const lstDetail = ref(props.data?.listNameDetails);
const inpName = ref(null);
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
  if (!name.value) {
    ElNotification({
      title: t("Notification"),
      message: t("NameFootballFieldEmptyMesg"),
      type: "warning",
    });
    return;
  }
  if (!address.value) {
    ElNotification({
      title: t("Notification"),
      message: t("AddressFootballFieldEmptyMesg"),
      type: "warning",
    });
    return;
  }
  if (
    timeSlotPerDay.value != lstTimeFrame.value.length ||
    timeSlotPerDay.value > maxTimeSlot.value
  ) {
    ElNotification({
      title: t("Notification"),
      message: t("TimeSlotInvalidMesg"),
      type: "warning",
    });
    return;
  }
  if (!isValidNameDetail()) {
    ElNotification({
      title: t("Notification"),
      message: t("NameDetailsEmptyMesg"),
      type: "warning",
    });
    return;
  }
  if (hasConflictTimeFrame()) {
    ElNotification({
      title: t("Notification"),
      message: t("ConfictTimeFrame"),
      type: "warning",
    });
    return;
  }

  const loading = ElLoading.service(loadingOptions);

  let actionPath = "pitch/insert";
  if (props.mode == "edit") actionPath = "pitch/update";
  store
    .dispatch(actionPath, {
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
        ElNotification({
          title: t("Notification"),
          message: t("SaveSuccess"),
          type: "success",
        });
      } else {
        ElNotification({
          title: t("Notification"),
          message: res?.data?.message ?? t("ErrorMesg"),
          type: "error",
        });
      }
    });
};

onMounted(() => {
  nextTick(() => {
    inpName.value.focus();
    setReadonlyInputField();
    addEvent("inpTimeSlot", decreaseTimeFrameInfosFn, increaseTimeFrameInfosFn);
    addEvent(
      "inpQuantity",
      decreaseListNameDetailsFn,
      increaseListNameDetailsFn
    );
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
  if (
    lstTimeFrame.value &&
    Array.isArray(lstTimeFrame.value) &&
    lstTimeFrame.value.length > 0
  ) {
    lstTimeFrame.value.pop();
  }
};

const increaseTimeFrameInfosFn = () => {
  if (lstTimeFrame.value && Array.isArray(lstTimeFrame.value)) {
    let lastItem = lstTimeFrame.value[lstTimeFrame.value.length - 1];

    let id = uuidv4();
    let sortOrder = lastItem.sortOrder + 1;
    let name = nameTimeFrame(sortOrder);

    let now = new Date();
    let timeBegin = new Date(now.setSeconds(0));
    let timeEnd = new Date(
      timeBegin.getTime() + minutesPerMatch.value * 60 * 1000
    );
    lstTimeFrame.value.push({
      id: id,
      sortOrder: sortOrder,
      name: name,
      price: 0,
      timeBegin: timeBegin,
      timeEnd: timeEnd,
      timeBeginTick: ticks(timeBegin),
      timeEndTick: ticks(timeEnd),
    });
  }
};

const decreaseListNameDetailsFn = () => {
  if (
    lstDetail.value &&
    Array.isArray(lstDetail.value) &&
    lstDetail.value.length > 0
  ) {
    lstDetail.value.pop();
  }
};

const increaseListNameDetailsFn = () => {
  if (lstDetail.value && Array.isArray(lstDetail.value)) {
    lstDetail.value.push("");
  }
};

const changeTimeBegin = (item) => {
  item.timeEnd = newDate(item.timeBegin, minutesPerMatch.value * 60 * 1000);
  item.timeEndTick = ticks(item.timeEnd);
  item.timeBeginTick = ticks(item.timeBegin);
};

const changeTimeEnd = (item) => {
  item.timeBegin = newDate(
    item.timeEnd,
    -1 * minutesPerMatch.value * 60 * 1000
  );
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
</script>


<template>
  <Dialog :title="t('FootballFieldForm')">
    <template #body>
      <el-form>
        <div class="row">
          <div class="mb-2 col-12 col-sm-12 col-md-8">
            <div class="mx-2">
              <el-input
                v-model="name"
                :placeholder="t('NameFootballField')"
                maxlength="500"
                ref="inpName"
              />
            </div>
          </div>
          <div class="mb-2 col-12 col-sm-12 col-md-4">
            <div class="mx-2">
              <el-select
                v-model="status"
                :placeholder="t('StatusFootballField')"
                class="w-100"
              >
                <el-option :label="t('Active')" :value="StatusEnum.ACTIVE" />
                <el-option
                  :label="t('Inactive')"
                  :value="StatusEnum.INACTIVE"
                />
              </el-select>
            </div>
          </div>
        </div>
        <div class="row mb-2">
          <div class="col-11">
            <div class="mx-2">
              <el-input
                v-model="address"
                :placeholder="t('Address')"
                maxlength="500"
              />
            </div>
          </div>
          <div class="col-1 d-flex flex-row align-items-center">
            <el-icon size="24" class="pointer"><LocationInformation /></el-icon>
          </div>
        </div>
        <div class="row">
          <div
            class="col-12 col-sm-12 col-md-4 mb-2 d-flex flex-column align-items-center justify-content-center"
          >
            <div>{{ t("QuantityFootballField") }}</div>
            <el-input-number
              id="inpQuantity"
              class="inpQuantity"
              v-model="quantity"
              :min="1"
              :max="100"
            />
          </div>
          <div
            class="col-12 col-sm-12 col-md-4 mb-2 d-flex flex-column align-items-center justify-content-center"
          >
            <div>{{ t("MinutesPerMatch") }}</div>
            <el-input-number
              v-model="minutesPerMatch"
              :min="30"
              :max="1440"
              :step="30"
            />
          </div>
          <div
            class="col-12 col-sm-12 col-md-4 mb-2 d-flex flex-column align-items-center justify-content-center"
          >
            <div>{{ t("TimeSlotPerDay") }}</div>
            <el-input-number
              id="inpTimeSlot"
              class="inpTimeSlot"
              v-model="timeSlotPerDay"
              :min="1"
              :max="maxTimeSlot"
            />
          </div>
        </div>
        <el-tabs type="border-card">
          <el-tab-pane :label="t('FootballFieldInfo')">
            <el-table :data="lstConfigInfo" class="w-100">
              <el-table-column label="" width="200">
                <template #default="scope">
                  <span>{{ scope.row.name }}</span>
                </template>
              </el-table-column>

              <el-table-column
                v-for="(item, index) in lstDetail"
                :key="index"
                :label="nameDetail(index + 1)"
                :min-width="160"
              >
                <template #default="scope">
                  <el-input
                    v-if="equals(scope.row.key, 'Name')"
                    v-model="lstDetail[index]"
                  />
                </template>
              </el-table-column>
            </el-table>
          </el-tab-pane>
          <el-tab-pane :label="t('TimeFrameInfo')">
            <el-table :data="lstConfigTimeFrame" class="w-100">
              <el-table-column label="" width="200">
                <template #default="scope">
                  <span>{{ scope.row.name }}</span>
                </template>
              </el-table-column>

              <el-table-column
                v-for="item in lstTimeFrame"
                :key="item"
                :label="item.name"
                :min-width="160"
              >
                <template #default="scope">
                  <mask-number-input
                    v-if="equals(scope.row.key, 'Price')"
                    :value="item.price"
                    @value="
                      (value) => {
                        item.price = value;
                      }
                    "
                    class="w-100"
                  ></mask-number-input>

                  <el-time-picker
                    v-if="equals(scope.row.key, 'TimeBegin')"
                    v-model="item.timeBegin"
                    class="w-100"
                    @change="changeTimeBegin(item)"
                  />

                  <el-time-picker
                    v-if="equals(scope.row.key, 'TimeEnd')"
                    v-model="item.timeEnd"
                    class="w-100"
                    @change="changeTimeEnd(item)"
                  />
                </template>
              </el-table-column>
            </el-table>
          </el-tab-pane>
        </el-tabs>
      </el-form>
    </template>
    <template #foot>
      <span class="d-flex flex-row-reverse">
        <el-button type="primary" @click="save" class="ml-2">{{
          t("Save")
        }}</el-button>
        <el-button @click="toggleModel">{{ t("Close") }}</el-button>
      </span>
    </template>
  </Dialog>
</template>