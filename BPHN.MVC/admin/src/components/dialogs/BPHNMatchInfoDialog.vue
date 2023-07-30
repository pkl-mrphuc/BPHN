<script setup>
import useToggleModal from "@/register-components/actionDialog";
import { useI18n } from "vue-i18n";
import { defineProps, ref, defineEmits } from "vue";
import { useStore } from "vuex";
import { BookingStatusEnum } from "@/const";
import useCommonFn from "@/commonFn";
import MaskNumberInput from "@/components/MaskNumberInput.vue";
import { ElNotification } from "element-plus";

const { toggleModel } = useToggleModal();
const { t } = useI18n();
const store = useStore();
const { equals } = useCommonFn();
const props = defineProps({
  data: Object,
});
const emit = defineEmits(["callback"]);
const teamA = ref(props.data?.teamA ?? "");
const teamB = ref(props.data?.teamB ?? "");
const note = ref(props.data?.note ?? "");
const id = ref(props.data?.bookingDetailId ?? "");
const status = ref(props.data?.status ?? "");
const deposite = ref(props.data?.deposite ?? "");
const bookingId = ref(props.data?.bookingId ?? "");

const save = () => {
  if (!teamA.value) {
    ElNotification({
      title: t("Notification"),
      message: t("TeamAEmptyMesg"),
      type: "warning",
    });
    return;
  }

  let data = {
    id: id.value,
    teamA: teamA.value,
    teamB: teamB.value,
    note: note.value,
    deposite: deposite.value,
  };
  store.dispatch("bookingDetail/updateMatch", data);
  emit("callback", data);
  toggleModel();
  ElNotification({
    title: t("Notification"),
    message: t("SaveSuccess"),
    type: "success",
  });
};

const decline = () => {
  store.dispatch("booking/decline", bookingId.value).then((res) => {
    if (res?.data?.success) {
      ElNotification({
        title: t("Notification"),
        message: t("SaveSuccess"),
        type: "success",
      });
      emit("callback", {
        teamA: teamA.value,
        teamB: teamB.value,
        note: note.value,
        deposite: deposite.value,
        status: BookingStatusEnum.CANCEL,
      });
      toggleModel();
    } else {
      ElNotification({
        title: t("Notification"),
        message: res?.data?.message ?? t("ErrorMesg"),
        type: "error",
      });
    }
  });
};

const approval = () => {
  store.dispatch("booking/approval", bookingId.value).then((res) => {
    if (res?.data?.success) {
      ElNotification({
        title: t("Notification"),
        message: t("SaveSuccess"),
        type: "success",
      });
      emit("callback", {
        teamA: teamA.value,
        teamB: teamB.value,
        note: note.value,
        deposite: deposite.value,
        status: BookingStatusEnum.SUCCESS,
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
</script>

<template>
  <Dialog :title="t('MatchInfoForm')">
    <template #body>
      <el-form-item>
        <el-col :span="7" class="fw-bold">
          {{ t("TeamA") }}<span class="text-danger">(*)</span>
        </el-col>
        <el-col :span="17">
          <el-input
            v-model="teamA"
            maxlength="255"
            :placeholder="t('ShouldContainPhoneNumber')"
          />
        </el-col>
      </el-form-item>
      <el-form-item>
        <el-col :span="7" class="fw-bold">
          {{ t("TeamB") }}
        </el-col>
        <el-col :span="17">
          <el-input v-model="teamB" maxlength="255" />
        </el-col>
      </el-form-item>
      <el-form-item>
        <el-col :span="7" class="fw-bold">
          {{ t("Deposite") }}
        </el-col>
        <el-col :span="17">
          <mask-number-input
            :value="deposite"
            @value="
              (value) => {
                deposite = value;
              }
            "
          ></mask-number-input>
        </el-col>
      </el-form-item>
      <el-form-item>
        <el-col :span="7" class="fw-bold">
          {{ t("Note") }}
        </el-col>
        <el-col :span="17">
          <el-input v-model="note" maxlength="500" :rows="3" type="textarea" />
        </el-col>
      </el-form-item>
    </template>
    <template #foot>
      <div class="d-flex flex-row justify-content-between">
        <div>
          <el-button
            v-if="equals(status, BookingStatusEnum.PENDING)"
            type="danger"
            @click="decline"
            >{{ t("Decline") }}</el-button
          >
          <el-button
            v-if="equals(status, BookingStatusEnum.PENDING)"
            type="primary"
            @click="approval"
            >{{ t("Approval") }}</el-button
          >
        </div>
        <div>
          <el-button @click="toggleModel">{{ t("Close") }}</el-button>
          <el-button type="primary" @click="save">{{ t("Save") }}</el-button>
        </div>
      </div>
    </template>
  </Dialog>
</template>