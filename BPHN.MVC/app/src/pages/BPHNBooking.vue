<template>
  <section class="w-100">
    <h2 class="fs-2">{{ t("Booking") }}</h2>
    <section>
      <el-alert
        type="warning"
        :closable="true"
        :description="t('BookingStep')"
        class="mb-3"
        v-if="!getLocalStorage('note_1')"
        @close="saveLocalStorage('note_1', '1')"
      />
      <el-steps class="step mb-3" :active="step" finish-status="success" simple>
        <el-step :title="t('Step1')" />
        <el-step :title="t('Step2')" />
        <el-step :title="t('Step3')" />
      </el-steps>
      <el-steps
        class="step-sm mb-3"
        :active="step"
      >
        <el-step :title="t('Step1')" />
        <el-step :title="t('Step2')" />
        <el-step :title="t('Step3')" />
      </el-steps>
      <div class="mb-3">
        <div>
          <booking-step-1 v-if="step == 0" @choose="choose"></booking-step-1>
          <booking-step-2
            v-if="step == 1"
            :data="objStadium"
            @back="prevStep"
            @agree="getBooking"
          ></booking-step-2>
          <booking-step-3
            v-if="step == 2"
            :data="objBooking"
            @back="prevStep"
            @complete="complete"
          ></booking-step-3>
        </div>
      </div>
    </section>
  </section>
</template>

<script setup>
import { ref } from "vue";
import { useI18n } from "vue-i18n";
import BookingStep1 from "@/components/BookingStep1.vue";
import BookingStep2 from "@/components/BookingStep2.vue";
import BookingStep3 from "@/components/BookingStep3.vue";
import useCommonFn from "@/commonFn";

const objStadium = ref(null);
const objBooking = ref(null);
const { t } = useI18n();
const step = ref(0);
const { getLocalStorage, saveLocalStorage } = useCommonFn();

const nextStep = () => {
  step.value++;
};

const prevStep = () => {
  step.value--;
  if (step.value < 0) step.value = 0;
};

const choose = (stadium) => {
  if (stadium) {
    let stadiumData = localStorage.getItem("stadium-data");
    if (stadiumData) {
      localStorage.removeItem("stadium-data");
    }
    localStorage.setItem("stadium-data", JSON.stringify(stadium));
    objStadium.value = stadium;
    nextStep();
  }
};

const getBooking = (data) => {
  if (data) {
    objBooking.value = data;
    nextStep();
  }
};

const complete = () => {
  localStorage.removeItem("stadium-data");
  step.value = 0;
};
</script>

<style scoped>
.step {
  display: none;
}

.step-sm {
  display: flex;
}
@media (min-width: 576px) {
}
@media (min-width: 768px) {
}
@media (min-width: 992px) {
}
@media (min-width: 1200px) {
  .step {
    display: flex;
  }

  .step-sm {
    display: none;
  }
}
@media (min-width: 1400px) {
}
</style>