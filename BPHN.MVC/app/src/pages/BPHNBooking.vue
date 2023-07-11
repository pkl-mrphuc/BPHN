<template>
  <section>
    <h2 class="fs-2">{{ t("Booking") }}</h2>
    <section>
      <el-alert
        type="warning"
        :closable="false"
        :description="t('BookingStep')"
        class="mb-2"
      />
      <el-steps :active="step" finish-status="success" simple>
        <el-step :title="t('Step1')" />
        <el-step :title="t('Step2')" />
        <el-step :title="t('Step3')" />
      </el-steps>
      <div class="p-2">
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

const objStadium = ref(null);
const objBooking = ref(null);
const { t } = useI18n();
const step = ref(0);

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