<template>
  <div class="container">
    <div class="row">
      <div
        class="mb-3 col-12 col-sm-12 col-md-6 col-lg-5"
        
      >
        <div class="mx-4" style="border: var(--el-border)">
          <h1
            class="fs-3 m-0 m-4 d-flex flex-row align-items-center justify-content-center"
          >
            {{ t("Booking") }}
          </h1>
          <div class="row mb-3">
            <div class="col-6 fw-bold d-flex flex-row-reverse">
              <div class="mx-3">
                {{ t("StadiumName") }}
              </div>
            </div>
            <div class="col-6">
              <div class="mx-3">{{ stadiumName }} - {{ nameDetail }}</div>
            </div>
          </div>
          <div class="row mb-3">
            <div class="col-6 fw-bold d-flex flex-row-reverse">
              <div class="mx-3">
                {{ t("BookingDate") }}
              </div>
            </div>
            <div class="col-6">
              <div class="mx-3">
                {{ bookingDate }}
              </div>
            </div>
          </div>
          <div class="row mb-3">
            <div class="col-6 fw-bold d-flex flex-row-reverse">
              <div class="mx-3">
                {{ t("MatchDate") }}
              </div>
            </div>
            <div class="col-6">
              <div class="mx-3">
                {{ matchDate }}
              </div>
            </div>
          </div>
          <div class="row mb-3">
            <div class="col-6 fw-bold d-flex flex-row-reverse">
              <div class="mx-3">
                {{ t("TimeFrame") }}
              </div>
            </div>
            <div class="col-6">
              <div class="mx-3">{{ timeFrameInfo }}</div>
            </div>
          </div>
          <div class="row mb-3">
            <div class="col-6 fw-bold d-flex flex-row-reverse">
              <div class="mx-3">
                {{ t("Price") }}
              </div>
            </div>
            <div class="col-6">
              <div class="mx-3">{{ price }}</div>
            </div>
          </div>
        </div>
      </div>
      <div class="col-12 col-sm-12 col-md-6 col-lg-7">
        <div>
          <div class="mb-2 fw-bold">
            <div class="mx-4">
              {{ t("PhoneNumber") }} <span class="text-danger">(*)</span>
            </div>
          </div>
          <div class="mb-3">
            <div class="mx-4">
              <el-input v-model="phoneNumber" maxlength="255" />
            </div>
          </div>
        </div>
        <div>
          <div class="mb-2 fw-bold">
            <div class="mx-4">
              {{ t("Email") }}<span class="text-danger">(*)</span>
            </div>
          </div>
          <div class="mb-3">
            <div class="mx-4">
              <el-input v-model="email" maxlength="255" />
            </div>
          </div>
        </div>
        <div>
          <div class="mb-2 fw-bold">
            <div class="mx-4">
              {{ t("FootballTeam") }}
            </div>
          </div>
          <div class="mb-3">
            <div class="mx-4">
              <el-input v-model="teamA" maxlength="255" />
            </div>
          </div>
        </div>
        <div>
          <div class="mb-2 fw-bold">
            <div class="mx-4">
              {{ t("Note") }}
            </div>
          </div>
          <div class="mb-3">
            <div class="mx-4">
              <el-input
                v-model="note"
                maxlength="500"
                :rows="3"
                type="textarea"
              />
            </div>
          </div>
        </div>
        <div class="d-flex flex-row mx-4">
          <div class="ml-auto"></div>
          <el-button @click="prevStep">{{ t("Back") }}</el-button>
          <el-button type="primary" @click="complete">{{
            t("Complete")
          }}</el-button>
        </div>
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
      startDate: dateToString(matchDateReal.value, "yyyy-MM-dd"),
      endDate: dateToString(matchDateReal.value, "yyyy-MM-dd"),
      timeFrameInfoId: timeFrameInfoId.value,
      pitchId: pitchId.value,
      nameDetail: nameDetail.value,
      teamA: teamA.value,
      note: note.value,
      accountId: accountId.value,
      bookingDate: dateToString(bookingDateReal.value, "yyyy-MM-dd"),
    })
    .then((res) => {
      if (res?.data?.success) {
        alert(t("BookingSuccessMesg"));
        emit("complete");
      } else {
        let msg = res?.data?.message;
        alert(msg ?? t("ErrorMesg"));
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

.text-danger {
  color: #f56c6c;
}
</style>>
