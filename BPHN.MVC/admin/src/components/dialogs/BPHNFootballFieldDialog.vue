<script setup>
import { computed, onMounted, nextTick, defineProps, ref, inject, defineEmits } from "vue";
// import { Location } from "@element-plus/icons-vue"
import useToggleModal from "@/register-components/actionDialog";
import { useI18n } from "vue-i18n";
import { v4 as uuidv4 } from "uuid";
import { useStore } from "vuex";
import { ElLoading } from "element-plus";

const props = defineProps({
  data: Object,
  mode: String
});
const emit = defineEmits(["callback"]);

const loadingOptions = inject("loadingOptions");
const { t } = useI18n();
const store = useStore();
const { toggleModel } = useToggleModal();
const configTimeFrameData = [
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
];

const configFootballFieldInfoData = [
  {
    name: t("NameFootballField"),
    key: "Name",
  },
];

const quantity = ref(props.data?.quantity ?? 1);

const minutesPerMatch = ref(props.data?.minutesPerMatch ?? 90);

const timeSlotPerDay = ref(props.data?.timeSlotPerDay ?? 1);

const name = ref(props.data?.name ?? "");

const address = ref(props.data?.address ?? "");

const status = ref(props.data?.status ?? "ACTIVE");

const timeFrameInfos = ref(props.data?.timeFrameInfos);

const listNameDetails = ref(props.data?.listNameDetails);

const maxTimeSlot = computed(() => {
  return 1440 / minutesPerMatch.value;
});

const save = () => {
  if (!name.value) {
    alert(t("NameFootballFieldEmptyMesg"));
    return;
  }
  if (!address.value) {
    alert(t("AddressFootballFieldEmptyMesg"));
    return;
  }
  if (
    timeSlotPerDay.value != timeFrameInfos.value.length ||
    timeSlotPerDay.value > maxTimeSlot.value
  ) {
    alert(t("TimeSlotInvalidMesg"));
    return;
  }
  if(!isValidNameDetail()) {
    alert(t('NameDetailsEmptyMesg'));
    return;
  }
  if (hasConflictTimeFrame()) {
    alert(t("ConfictTimeFrame"));
    return;
  }

  const loading = ElLoading.service(loadingOptions);

  let actionPath = "pitch/insert"
  if(props.mode == "edit") actionPath = "pitch/update"

  store
    .dispatch(actionPath, {
      id: props.data?.id,
      name: name.value,
      address: address.value,
      minutesPerMatch: minutesPerMatch.value,
      quantity: quantity.value,
      timeSlotPerDay: timeSlotPerDay.value,
      status: status.value,
      timeFrameInfos: timeFrameInfos.value,
      listNameDetails: listNameDetails.value
    })
    .then((res) => {
      emit("callback", res);
      loading.close();
      toggleModel();
    });
};

onMounted(() => {
  nextTick(() => {
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
  if (
    timeFrameInfos.value &&
    Array.isArray(timeFrameInfos.value) &&
    timeFrameInfos.value.length > 0
  ) {
    timeFrameInfos.value.pop();
  }
};

const increaseTimeFrameInfosFn = () => {
  if (timeFrameInfos.value && Array.isArray(timeFrameInfos.value)) {
    let lastItem = timeFrameInfos.value[timeFrameInfos.value.length - 1];

    let id = uuidv4();
    let sortOrder = lastItem.sortOrder + 1;
    let name = `Khung ${sortOrder}`;

    let now = new Date();
    let timeBegin = new Date(now.setSeconds(0));
    let timeEnd = new Date(
      timeBegin.getTime() + minutesPerMatch.value * 60 * 1000
    );
    timeFrameInfos.value.push({
      id: id,
      sortOrder: sortOrder,
      name: name,
      price: 0,
      timeBegin: timeBegin,
      timeEnd: timeEnd,
    });
  }
};

const decreaseListNameDetailsFn = () => {
  if (
    listNameDetails.value &&
    Array.isArray(listNameDetails.value) &&
    listNameDetails.value.length > 0
  ) {
    listNameDetails.value.pop();
  }
};

const increaseListNameDetailsFn = () => {
  if (listNameDetails.value && Array.isArray(listNameDetails.value)) {
    listNameDetails.value.push('');
  }
};

const changeTimeBegin = (item) => {
  item.timeEnd = new Date(
    item.timeBegin.getTime() + minutesPerMatch.value * 60 * 1000
  );
};

const changeTimeEnd = (item) => {
  item.timeBegin = new Date(
    item.timeEnd.getTime() - minutesPerMatch.value * 60 * 1000
  );
};

const hasConflictTimeFrame = () => {
  let timeLine = [];
  let index = 0;
  for (let i = 0; i < timeFrameInfos.value.length; i++) {
    const item = timeFrameInfos.value[i];
    timeLine.push({
      sortOrder: index,
      data: new Date(item.timeBegin),
    });
    index++;
    timeLine.push({
      sortOrder: index,
      data: new Date(item.timeEnd),
    });
    index++;
  }

  timeLine.sort((a, b) => {
    return a.data - b.data;
  });

  for (let i = 1; i < timeLine.length; i++) {
    const current = timeLine[i];
    const prev = timeLine[i - 1];
    if (current.sortOrder - prev.sortOrder != 1) return true;
  }
  return false;
};

const isValidNameDetail = () => {
  for (let i = 0; i < listNameDetails.value.length; i++) {
    const item = listNameDetails.value[i];
    if(!item) return false;
  }
  return true;
};
</script>


<template>
  <Dialog :title="t('FootballFieldForm')" :width="1200">
    <template #body>
      <el-form>
        <el-form-item>
          <el-col :span="19">
            <el-input v-model="name" :placeholder="t('NameFootballField')" />
          </el-col>
          <el-col :span="4">
            <el-select v-model="status" :placeholder="t('StatusFootballField')">
              <el-option :label="t('Active')" value="ACTIVE" />
              <el-option :label="t('Inactive')" value="INACTIVE" />
            </el-select>
          </el-col>
        </el-form-item>
        <el-form-item>
          <el-col :span="24">
            <el-input v-model="address" :placeholder="t('Address')" />
          </el-col>
          <!-- <el-col :span="1">
            <el-icon><Location /></el-icon>
          </el-col> -->
        </el-form-item>
        <el-form-item>
          <el-col :span="8" class="center">
            <div>{{ t("QuantityFootballField") }}</div>
            <el-input-number
              id="inpQuantity"
              class="inpQuantity"
              v-model="quantity"
              :min="1"
              :max="100"
            />
          </el-col>
          <el-col :span="8" class="center">
            <div>{{ t("MinutesPerMatch") }}</div>
            <el-input-number
              v-model="minutesPerMatch"
              :min="30"
              :max="1440"
              :step="30"
            />
          </el-col>
          <el-col :span="8" class="center">
            <div>{{ t("TimeSlotPerDay") }}</div>
            <el-input-number
              id="inpTimeSlot"
              class="inpTimeSlot"
              v-model="timeSlotPerDay"
              :min="1"
              :max="maxTimeSlot"
            />
          </el-col>
        </el-form-item>
        <el-tabs type="border-card">
          <el-tab-pane :label="t('FootballFieldInfo')">
            <el-table :data="configFootballFieldInfoData" class="w100">
              <el-table-column label="" width="200">
                <template #default="scope">
                  <span>{{ scope.row.name }}</span>
                </template>
              </el-table-column>

              <el-table-column
                v-for="(item, index) in listNameDetails"
                :key="index"
                :label="`${index + 1}`"
                :min-width="160"
              >
                <template #default="scope">
                  <el-input
                    v-if="scope.row.key == 'Name'"
                    v-model="listNameDetails[index]"
                  />
                </template>
              </el-table-column>
            </el-table>
          </el-tab-pane>
          <el-tab-pane :label="t('TimeFrameInfo')">
            <el-table :data="configTimeFrameData" class="w100">
              <el-table-column label="" width="200">
                <template #default="scope">
                  <span>{{ scope.row.name }}</span>
                </template>
              </el-table-column>

              <el-table-column
                v-for="item in timeFrameInfos"
                :key="item"
                :label="item.name"
                :min-width="160"
              >
                <template #default="scope">
                  <el-input-number
                    v-if="scope.row.key == 'Price'"
                    v-model="item.price"
                    style="width: 100%"
                    :min="0"
                    :step="100"
                  />

                  <el-time-picker
                    v-if="scope.row.key == 'TimeBegin'"
                    v-model="item.timeBegin"
                    style="width: 100%"
                    @change="changeTimeBegin(item)"
                  />

                  <el-time-picker
                    v-if="scope.row.key == 'TimeEnd'"
                    v-model="item.timeEnd"
                    style="width: 100%"
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
      <span class="dialog-footer">
        <el-button @click="toggleModel">{{ t("Close") }}</el-button>
        <el-button type="primary" @click="save">{{ t("Save") }}</el-button>
      </span>
    </template>
  </Dialog>
</template>

<style scoped>
.center {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
}

.m0 {
  margin: 0;
}
</style>