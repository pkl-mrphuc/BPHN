<script setup>
import { useI18n } from "vue-i18n";
import { ref, defineProps, onMounted, defineEmits, inject } from "vue";
import useToggleModal from "@/register-components/actionDialog";
import useCommonFn from "@/commonFn";
import { useStore } from "vuex";
import { ElLoading } from "element-plus";
import { NotificationTypeEnum } from "@/const";
import connection from "@/ws";

const { toggleModel, openModal, hasRole } = useToggleModal();
const { sameDate, yearEndDay, time, dateToString } = useCommonFn();
const { t } = useI18n();
const store = useStore();
const emits = defineEmits(["callback"]);
const loadingOptions = inject("loadingOptions");
const props = defineProps({
  data: Object,
});

const running = ref(0);
const isRecurring = ref(props.data?.isRecurring ?? false);
const weekdays = ref(props.data?.weekendays + "");
const fromDate = ref(props.data?.fromDate ?? new Date());
const toDate = ref(props.data?.toDate ?? new Date());
const lstStadium = ref([]);
const lstTimeFrame = ref([]);
const lstDetail = ref([]);
const pitchId = ref(props.data?.pitchId ?? null);
const nameDetail = ref(props.data?.nameDetail ?? null);
const timeFrameInfoId = ref(props.data?.timeFrameInfoId ?? null);
const phoneNumber = ref(props.data?.phoneNumber ?? null);
const email = ref(props.data?.email ?? null);

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
    for (let i = 0; i < pitch.timeFrameInfos.length; i++) {
      const item = pitch.timeFrameInfos[i];
      item["newName"] = `Khung ${time(item.timeBegin)} - ${time(item.timeEnd)}`;
    }
    lstTimeFrame.value = pitch.timeFrameInfos;
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
    alert(t("InputTimeFrameEmptyMesg"));
    return;
  }
  let result = await checkFreeTimeFrame();
  if (result?.data?.success) {
    alert(t("Free"));
  } else {
    let msg = result?.data?.message;
    alert(msg ?? t("Reserved"));
  }
};

const finder = () => {
  openModal("FindBlankDialog");
};

const save = () => {
  if (running.value > 0) return;
  ++running.value;

  if (!phoneNumber.value) {
    alert(t("PhoneNumberEmptyMesg"));
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
        if (connection && connection.state === "Connected") {
          connection.invoke(
            "PushNotification",
            store.getters["account/getRelationIds"],
            store.getters["account/getAccountId"],
            NotificationTypeEnum.ADD_BOOKING
          );
        }
      } else {
        let msg = res?.data?.message;
        alert(msg ?? t("ErrorMesg"));
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

onMounted(() => {
  store
    .dispatch("pitch/getPaging", {
      accountId: store.getters["account/getAccountId"],
      hasDetail: true,
      hasInactive: false,
    })
    .then((res) => {
      if (res?.data?.data) {
        lstStadium.value = res.data.data;
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
        class="row d-flex flex-row justify-content-between align-items-center"
      >
        <div>
          <el-button class="mb-2" @click="quickCheck">{{
            t("QuickCheck")
          }}</el-button>
          <el-button class="mb-2" @click="finder">{{ t("Finder") }}</el-button>
        </div>

        <div>
          <el-button class="mb-2" @click="toggleModel">{{
            t("Close")
          }}</el-button>
          <el-button class="mb-2" type="primary" @click="save">{{
            t("Save")
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