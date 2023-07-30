<script setup>
import { useI18n } from "vue-i18n";
import {
  ref,
  defineProps,
  onMounted,
  defineEmits,
  inject,
  nextTick,
} from "vue";
import useToggleModal from "@/register-components/actionDialog";
import useCommonFn from "@/commonFn";
import { useStore } from "vuex";
import { ElLoading, ElNotification } from "element-plus";

const { toggleModel, openModal, hasRole } = useToggleModal();
const { sameDate, yearEndDay, time, dateToString, equals } = useCommonFn();
const { t } = useI18n();
const store = useStore();
const emits = defineEmits(["callback"]);
const loadingOptions = inject("loadingOptionsDark");
const props = defineProps({
  data: Object,
  mode: String,
});

const running = ref(0);
const isRecurring = ref(props.data?.isRecurring ?? false);
const weekdays = ref(props.data?.weekendays + "");
const fromDate = ref(props.data?.startDate ?? new Date());
const toDate = ref(props.data?.endDate ?? new Date());
const lstStadium = ref([]);
const lstTimeFrame = ref([]);
const lstDetail = ref([]);
const pitchId = ref(props.data?.pitchId ?? null);
const nameDetail = ref(props.data?.nameDetail ?? null);
const timeFrameInfoId = ref(props.data?.timeFrameInfoId ?? null);
const phoneNumber = ref(props.data?.phoneNumber ?? null);
const email = ref(props.data?.email ?? null);
const inpPhoneNumber = ref(null);

const showMakeRecurring = () => {
  if (isRecurring.value) {
    fromDate.value = new Date();
    toDate.value = yearEndDay(fromDate.value);
  } else {
    toDate.value = fromDate.value;
  }
  weekdays.value = `${fromDate.value.getDay()}`;
};

const changeDate = () => {
  if (sameDate(fromDate.value, toDate.value)) {
    toDate.value = fromDate.value;
    isRecurring.value = false;
  }
  weekdays.value = `${fromDate.value.getDay()}`;
};

const changePitchId = () => {
  let pitchSelected = lstStadium.value.filter(
    (item) => item.id == pitchId.value
  );
  if (pitchSelected && pitchSelected.length > 0) {
    let pitch = pitchSelected[0];
    lstDetail.value = pitch.nameDetails.split(";");

    if (!lstDetail.value.includes(nameDetail.value)) {
      nameDetail.value = null;
    }

    let isInclude = false;
    for (let i = 0; i < pitch.timeFrameInfos.length; i++) {
      const item = pitch.timeFrameInfos[i];
      if (item.id === timeFrameInfoId.value) {
        isInclude = true;
      }
      item["newName"] = `Khung ${time(item.timeBegin)} - ${time(item.timeEnd)}`;
    }
    lstTimeFrame.value = pitch.timeFrameInfos;

    if (!isInclude) {
      timeFrameInfoId.value = null;
    }
  }
};

const checkFreeTimeFrame = async () => {
  let result = await store.dispatch("booking/checkTimeFrame", {
    id: props.data?.id,
    phoneNumber: phoneNumber.value,
    email: email.value,
    isRecurring: isRecurring.value,
    startDate: dateToString(fromDate.value, "yyyy-MM-dd"),
    endDate: dateToString(toDate.value, "yyyy-MM-dd"),
    weekendays: weekdays.value,
    timeFrameInfoId: timeFrameInfoId.value,
    pitchId: pitchId.value,
    nameDetail: nameDetail.value,
  });

  return result;
};

const quickCheck = async () => {
  if (!pitchId.value || !nameDetail.value || !timeFrameInfoId.value) {
    ElNotification({
      title: t("Notification"),
      message: t("InputTimeFrameEmptyMesg"),
      type: "warning",
    });
    return;
  }
  let result = await checkFreeTimeFrame();
  if (result?.data?.success) {
    ElNotification({
      title: t("Notification"),
      message: t("Free"),
      type: "info",
    });
  } else {
    ElNotification({
      title: t("Notification"),
      message: result?.data?.message ?? t("Reserved"),
      type: "info",
    });
  }
};

const finder = () => {
  openModal("FindBlankDialog");
};

const save = () => {
  if (running.value > 0) return;
  ++running.value;

  if (!phoneNumber.value) {
    ElNotification({
      title: t("Notification"),
      message: t("PhoneNumberEmptyMesg"),
      type: "warning",
    });
    return;
  }
  const loading = ElLoading.service(loadingOptions);
  store
    .dispatch("booking/insert", {
      id: props.data?.id,
      phoneNumber: phoneNumber.value,
      email: email.value,
      isRecurring: isRecurring.value,
      startDate: dateToString(fromDate.value, "yyyy-MM-dd"),
      endDate: dateToString(toDate.value, "yyyy-MM-dd"),
      weekendays: weekdays.value,
      timeFrameInfoId: timeFrameInfoId.value,
      pitchId: pitchId.value,
      nameDetail: nameDetail.value,
    })
    .then((res) => {
      loading.close();
      if (res?.data?.success) {
        emits("callback");
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

      setTimeout(() => {
        running.value = 0;
      }, 1000);
    });
};

const bindBlankData = (data) => {
  if (data) {
    isRecurring.value = data.isRecurring;
    pitchId.value = data.pitchId;
    timeFrameInfoId.value = data.timeFrameInfoId;
    nameDetail.value = data.nameDetail;
    fromDate.value = data.startDate;
    toDate.value = data.endDate;
    weekdays.value = data.weekendays + "";
    changePitchId();
  }
};

const disabledDate = (time) => {
  return time.getTime() < Date.now() - 24 * 60 * 60 * 1000;
};

const decline = () => {
  if (running.value > 0) return;
  ++running.value;
  const loading = ElLoading.service(loadingOptions);
  store.dispatch("booking/decline", props?.data?.id).then((res) => {
    loading.close();
    if (res?.data?.success) {
      emits("callback");
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

    setTimeout(() => {
      running.value = 0;
    }, 1000);
  });
};

const approval = () => {
  if (running.value > 0) return;
  ++running.value;
  const loading = ElLoading.service(loadingOptions);
  store.dispatch("booking/approval", props?.data?.id).then((res) => {
    loading.close();
    if (res?.data?.success) {
      emits("callback");
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

    setTimeout(() => {
      running.value = 0;
    }, 1000);
  });
};

onMounted(() => {
  const loading = ElLoading.service(loadingOptions);
  store
    .dispatch("pitch/getPaging", {
      accountId: store.getters["account/getAccountId"],
      hasDetail: true,
      hasInactive: false,
      pageIndex: 1,
      pageSize: 1000,
    })
    .then((res) => {
      if (res?.data?.data) {
        lstStadium.value = res.data.data;
        changePitchId();
        loading.close();
      }
    });

  nextTick(() => {
    if (inpPhoneNumber.value) {
      inpPhoneNumber.value.focus();
    }
  });
});
</script>

<template>
  <Dialog :title="t('BookingForm')">
    <template #body>
      <el-form>
        <div class="row">
          <div class="mb-2 col-12">
            <div class="mx-2">
              <el-input
                :placeholder="t('PhoneNumber')"
                v-model="phoneNumber"
                maxlength="225"
                ref="inpPhoneNumber"
              />
            </div>
          </div>
          <div class="mb-2 col-12">
            <div class="mx-2">
              <el-input
                :placeholder="t('Email')"
                v-model="email"
                maxlength="225"
              />
            </div>
          </div>
        </div>
        <div class="row">
          <div class="mb-2 col-12 col-sm-4">
            <div class="mx-2">
              <el-select
                class="w-100"
                :placeholder="t('Infrastructure')"
                v-model="pitchId"
                @change="changePitchId"
              >
                <el-option
                  v-for="item in lstStadium"
                  :key="item"
                  :label="item.name"
                  :value="item.id"
                />
              </el-select>
            </div>
          </div>
          <div class="mb-2 col-12 col-sm-4">
            <div class="mx-2">
              <el-select
                class="w-100"
                :placeholder="t('TimeFrame')"
                v-model="timeFrameInfoId"
              >
                <el-option
                  v-for="item in lstTimeFrame"
                  :key="item"
                  :label="item.newName"
                  :value="item.id"
                />
              </el-select>
            </div>
          </div>
          <div class="mb-2 col-12 col-sm-4">
            <div class="mx-2">
              <el-select
                class="w-100"
                :placeholder="t('NameDetail')"
                v-model="nameDetail"
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
        </div>
        <div class="mx-2 mb-2">
          <el-checkbox
            :label="t('MakeRecurring')"
            v-model="isRecurring"
            @change="showMakeRecurring"
          />
        </div>
        <div class="mx-2 mb-2">
          <el-radio-group v-model="weekdays" :disabled="!isRecurring">
            <el-radio label="1">{{ t("Monday") }}</el-radio>
            <el-radio label="2">{{ t("Tuesday") }}</el-radio>
            <el-radio label="3">{{ t("Wednesday") }}</el-radio>
            <el-radio label="4">{{ t("Thursday") }}</el-radio>
            <el-radio label="5">{{ t("Friday") }}</el-radio>
            <el-radio label="6">{{ t("Saturday") }}</el-radio>
            <el-radio label="0">{{ t("Sunday") }}</el-radio>
          </el-radio-group>
        </div>
        <div class="mb-2" v-if="!isRecurring">
          <div class="mx-2">
            <el-date-picker
              type="date"
              placeholder="Ngày"
              class="w-100"
              v-model="fromDate"
              @change="changeDate"
              :disabled-date="disabledDate"
            />
          </div>
        </div>
        <div class="mb-2 row" v-if="isRecurring">
          <div class="mb-2 col-12 col-sm-6">
            <div class="mx-2">
              <el-date-picker
                type="date"
                :placeholder="t('FromDate')"
                class="w-100"
                v-model="fromDate"
                @change="changeDate"
                :disabled-date="disabledDate"
              />
            </div>
          </div>
          <div class="mb-2 col-12 col-sm-6">
            <div class="mx-2">
              <el-date-picker
                type="date"
                :placeholder="t('ToDate')"
                class="w-100"
                v-model="toDate"
                @change="changeDate"
                disabled
              />
            </div>
          </div>
        </div>
      </el-form>
    </template>
    <template #foot>
      <div
        class="row d-flex flex-row-reverse justify-content-between align-items-center"
      >
        <div class="d-flex flex-row-reverse">
          <el-button
            class="mb-2"
            v-if="!equals(props.mode, 'approval')"
            type="primary"
            @click="save"
            >{{ t("Save") }}</el-button
          >
          <el-button
            class="mb-2"
            v-if="equals(props.mode, 'approval')"
            type="primary"
            @click="approval"
            >{{ t("Approval") }}</el-button
          >
          <el-button
            class="m-0 mb-2 mr-2"
            v-if="equals(props.mode, 'approval')"
            type="danger"
            @click="decline"
            >{{ t("Decline") }}</el-button
          >
          <el-button class="mb-2 mr-2" @click="toggleModel">{{
            t("Close")
          }}</el-button>
        </div>

        <div class="d-flex flex-row-reverse">
          <el-button
            v-if="!equals(props.mode, 'approval')"
            class="mb-2"
            @click="finder"
            >{{ t("Finder") }}</el-button
          >
          <el-button class="mb-2 mr-2" @click="quickCheck">{{
            t("QuickCheck")
          }}</el-button>
        </div>
      </div>
    </template>
  </Dialog>
  <FindBlankDialog
    v-if="hasRole('FindBlankDialog')"
    :isRecurring="isRecurring"
    :startDate="dateToString(fromDate, 'yyyy-MM-dd')"
    :endDate="dateToString(toDate, 'yyyy-MM-dd')"
    @callback="bindBlankData"
  ></FindBlankDialog>
</template>