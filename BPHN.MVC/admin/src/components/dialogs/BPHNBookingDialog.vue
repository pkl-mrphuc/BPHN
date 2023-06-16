<script setup>
import { useI18n } from "vue-i18n";
import { ref, defineProps, onMounted, defineEmits, inject } from "vue";
import useToggleModal from "@/register-components/actionDialog";
import useCommonFn from "@/commonFn";
import { useStore } from "vuex";
import { ElLoading } from "element-plus";

const { toggleModel, openModal, hasRole } = useToggleModal();
const { sameDate, yearEndDay, time, dateToString, getDay } = useCommonFn();
const { t } = useI18n();
const store = useStore();
const emits = defineEmits(["callback"]);
const loadingOptions = inject("loadingOptions");
const props = defineProps({
  data: Object,
});

const isRecurring = ref(props.data?.isRecurring ?? false);
const weekdays = ref(props.data?.weekendays + "");
const fromDate = ref(props.data?.fromDate ?? new Date());
const toDate = ref(props.data?.toDate ?? new Date());
const listPitch = ref([]);
const listTimeFrame = ref([]);
const listDetail = ref([]);
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
  weekdays.value = getDay(fromDate.value);
};

const changeDate = () => {
  if (sameDate(fromDate.value, toDate.value)) {
    toDate.value = fromDate.value;
    isRecurring.value = false;
  }
  weekdays.value = getDay(fromDate.value);
};

const changePitchId = () => {
  let pitchSelected = listPitch.value.filter(
    (item) => item.id == pitchId.value
  );
  if (pitchSelected && pitchSelected.length > 0) {
    let pitch = pitchSelected[0];
    listDetail.value = pitch.nameDetails.split(";");
    for (let i = 0; i < pitch.timeFrameInfos.length; i++) {
      const item = pitch.timeFrameInfos[i];
      item["newName"] = `Khung ${time(item.timeBegin)} - ${time(item.timeEnd)}`;
    }
    listTimeFrame.value = pitch.timeFrameInfos;
  }
};

const checkFreeTimeFrame = async () => {
  let result = await store.dispatch("booking/checkTimeFrame", {
    id: props.data?.id,
    phoneNumber: phoneNumber.value,
    email: email.value,
    isRecurring: isRecurring.value,
    startDate: dateToString(fromDate.value),
    endDate: dateToString(toDate.value),
    weekendays: weekdays.value,
    timeFrameInfoId: timeFrameInfoId.value,
    pitchId: pitchId.value,
    nameDetail: nameDetail.value,
  });

  return result?.data?.success ?? false;
};

const quickCheck = async () => {
  if (!pitchId.value || !nameDetail.value || !timeFrameInfoId.value) {
    alert(t("InputTimeFrameEmptyMesg"));
    return;
  }
  let result = await checkFreeTimeFrame();
  if (!result) {
    alert(t("Reserved"));
  } else {
    alert(t("Free"));
  }
};

const finder = () => {
  openModal("FindBlankDialog");
};

const save = () => {
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
      startDate: dateToString(fromDate.value),
      endDate: dateToString(toDate.value),
      weekendays: weekdays.value,
      timeFrameInfoId: timeFrameInfoId.value,
      pitchId: pitchId.value,
      nameDetail: nameDetail.value,
    })
    .then((res) => {
      if (res?.data?.success) {
        emits("callback");
        toggleModel();
      } else {
        let msg = res?.data?.message;
        alert(msg ?? t("ErrorMesg"));
      }
      loading.close();
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
        listPitch.value = res.data.data;
      }
    });
});
</script>

<template>
  <Dialog :title="t('BookingForm')" :width="750">
    <template #body>
      <el-form>
        <el-form-item>
          <el-col :span="11">
            <el-input :placeholder="t('PhoneNumber')" v-model="phoneNumber" />
          </el-col>
          <el-col :span="11">
            <el-input :placeholder="t('Email')" v-model="email" />
          </el-col>
        </el-form-item>
        <el-form-item>
          <el-col :span="7">
            <el-select
              style="width: 100%"
              :placeholder="t('Infrastructure')"
              v-model="pitchId"
              @change="changePitchId"
            >
              <el-option
                v-for="item in listPitch"
                :key="item"
                :label="item.name"
                :value="item.id"
              />
            </el-select>
          </el-col>
          <el-col :span="7">
            <el-select
              style="width: 100%"
              :placeholder="t('TimeFrame')"
              v-model="timeFrameInfoId"
            >
              <el-option
                v-for="item in listTimeFrame"
                :key="item"
                :label="item.newName"
                :value="item.id"
              />
            </el-select>
          </el-col>
          <el-col :span="7">
            <el-select
              style="width: 100%"
              :placeholder="t('NameDetail')"
              v-model="nameDetail"
            >
              <el-option
                v-for="item in listDetail"
                :key="item"
                :label="item"
                :value="item"
              />
            </el-select>
          </el-col>
        </el-form-item>
        <el-form-item>
          <el-checkbox
            :label="t('MakeRecurring')"
            v-model="isRecurring"
            @change="showMakeRecurring"
          />
        </el-form-item>
        <el-form-item>
          <el-radio-group v-model="weekdays" :disabled="!isRecurring">
            <el-radio label="1">{{ t("Monday") }}</el-radio>
            <el-radio label="2">{{ t("Tuesday") }}</el-radio>
            <el-radio label="3">{{ t("Wednesday") }}</el-radio>
            <el-radio label="4">{{ t("Thursday") }}</el-radio>
            <el-radio label="5">{{ t("Friday") }}</el-radio>
            <el-radio label="6">{{ t("Saturday") }}</el-radio>
            <el-radio label="0">{{ t("Sunday") }}</el-radio>
          </el-radio-group>
        </el-form-item>
        <el-form-item v-if="!isRecurring">
          <el-date-picker
            type="date"
            placeholder="Ngày"
            style="width: 100%"
            v-model="fromDate"
            @change="changeDate"
            :disabled-date="disabledDate"
          />
        </el-form-item>
        <el-form-item v-if="isRecurring">
          <el-col :span="11">
            <el-date-picker
              type="date"
              :placeholder="t('FromDate')"
              style="width: 100%"
              v-model="fromDate"
              @change="changeDate"
              :disabled-date="disabledDate"
            />
          </el-col>
          <el-col :span="11">
            <el-date-picker
              type="date"
              :placeholder="t('ToDate')"
              style="width: 100%"
              v-model="toDate"
              @change="changeDate"
              disabled
            />
          </el-col>
        </el-form-item>
      </el-form>
    </template>
    <template #foot>
      <div class="action-footer">
        <span class="other-footer">
          <el-button @click="quickCheck">{{ t("QuickCheck") }}</el-button>
          <el-button @click="finder">{{ t("Finder") }}</el-button>
        </span>
        <span class="dialog-footer">
          <el-button @click="toggleModel">{{ t("Close") }}</el-button>
          <el-button type="primary" @click="save">{{ t("Save") }}</el-button>
        </span>
      </div>
    </template>
  </Dialog>
  <FindBlankDialog
    v-if="hasRole('FindBlankDialog')"
    :isRecurring="isRecurring"
    :startDate="dateToString(fromDate)"
    :endDate="dateToString(toDate)"
    @callback="bindBlankData"
  ></FindBlankDialog>
</template>

<style scoped>
.action-footer {
  display: flex;
  align-items: center;
  justify-content: space-between;
}
</style>