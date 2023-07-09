<template>
  <div>
    <div class="d-flex justify-content-between">
      <div class="booking-info">
        <h1 class="fs-20 d-flex justify-content-center align-items-center">
          {{ t("Booking") }}
        </h1>
        <el-form-item>
          <el-col :span="7">
            <b>{{ t("StadiumName") }}</b>
          </el-col>
          <el-col :span="17"> {{ stadiumName }} - {{ nameDetail }} </el-col>
        </el-form-item>
        <el-form-item>
          <el-col :span="7">
            <b>{{ t("BookingDate") }}</b>
          </el-col>
          <el-col :span="17">
            {{ bookingDate }}
          </el-col>
        </el-form-item>
        <el-form-item>
          <el-col :span="7">
            <b>{{ t("MatchDate") }}</b>
          </el-col>
          <el-col :span="17">
            {{ matchDate }}
          </el-col>
        </el-form-item>
        <el-form-item>
          <el-col :span="7">
            <b>{{ t("TimeFrame") }}</b>
          </el-col>
          <el-col :span="17"> {{ timeFrameInfo }} </el-col>
        </el-form-item>
        <el-form-item>
          <el-col :span="7">
            <b>{{ t("Price") }}</b>
          </el-col>
          <el-col :span="17"> {{ price }} </el-col>
        </el-form-item>
      </div>
      <div class="user-info">
        <el-form-item>
          <el-col>
            <b>{{ t("PhoneNumber") }} <span class="red">(*)</span></b>
          </el-col>
          <el-col>
            <el-input v-model="phoneNumber" maxlength="255" />
          </el-col>
        </el-form-item>
        <el-form-item>
          <el-col>
            <b>{{ t("Email") }}<span class="red">(*)</span></b>
          </el-col>
          <el-col>
            <el-input v-model="email" maxlength="255" />
          </el-col>
        </el-form-item>
        <el-form-item>
          <el-col>
            <b>{{ t("FootballTeam") }}</b>
          </el-col>
          <el-col>
            <el-input v-model="teamA" maxlength="255" />
          </el-col>
        </el-form-item>
        <el-form-item>
          <el-col>
            <b>{{ t("Note") }}</b>
          </el-col>
          <el-col>
            <el-input
              v-model="note"
              maxlength="500"
              :rows="3"
              type="textarea"
            />
          </el-col>
        </el-form-item>
        <el-form-item>
          <div class="ml-auto"></div>
          <el-button @click="prevStep">{{ t("Back") }}</el-button>
          <el-button type="primary" @click="complete">{{
            t("Complete")
          }}</el-button>
        </el-form-item>
      </div>
    </div>
  </div>
</template>

<script setup>
import { useI18n } from "vue-i18n";
import { ref, defineEmits, defineProps } from "vue";
import { useStore } from "vuex";
import useCommonFn from "@/commonFn";

const { dateToString } = useCommonFn();
const props = defineProps({
  data: Object,
});
const store = useStore();
const { t } = useI18n();
const emit = defineEmits(["back"]);
const bookingId = ref(props.data?.bookingId);
const timeFrameInfoId = ref(props.data?.timeFrameInfoId);
const stadiumName = ref(props.data?.stadiumName);
const bookingDate = ref(props.data?.bookingDate);
const bookingDateReal = ref(props.data?.bookingDateReal);
const matchDate = ref(props.data?.matchDate);
const matchDateReal = ref(props.data?.matchDateReal);
const price = ref(props.data?.price);
const timeFrameInfo = ref(props.data?.timeFrameInfo);
const pitchId = ref(props.data?.pitchId);
const weekdays = ref(props.data?.weekdays);
const accountId = ref(props.data?.accountId);
const nameDetail = ref(props.data?.nameDetail);
const phoneNumber = ref(null);
const email = ref(null);
const note = ref(null);
const teamA = ref(null);

const prevStep = () => {
  emit("back");
};

const complete = () => {
  if (!phoneNumber.value) {
    alert(t("NotEmptyMesg", { name: t("PhoneNumber") }));
    return;
  }
  if (!email.value) {
    alert(t("NotEmptyMesg", { name: t("Email") }));
    return;
  }

  store
    .dispatch("booking/insertBookingRequest", {
      id: bookingId.value,
      phoneNumber: phoneNumber.value,
      email: email.value,
      isRecurring: false,
      startDate: dateToString(matchDateReal.value, "yyyy-MM-dd"),
      endDate: dateToString(matchDateReal.value, "yyyy-MM-dd"),
      weekendays: weekdays.value,
      timeFrameInfoId: timeFrameInfoId.value,
      pitchId: pitchId.value,
      nameDetail: nameDetail.value,
      teamA: teamA.value,
      note: note.value,
      accountId: accountId.value,
      bookingDate: dateToString(bookingDateReal.value, "yyyy-MM-dd"),
    })
    .then((res) => {
      if(res.data?.success) {
        alert(t("BookingSuccessMesg"));
        emit("complete");
      }
      else {
        alert(t("ErrorMesg"));
      }
    });
};
</script>

<style scoped>
.booking-info {
  width: 40%;
  border: var(--el-border);
  padding: 30px;
}

.user-info {
  width: 60%;
  padding: 30px;
}

.red {
  color: #f56c6c;
}
</style>>
