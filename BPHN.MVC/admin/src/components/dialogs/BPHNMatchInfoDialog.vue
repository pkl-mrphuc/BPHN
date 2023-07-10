<script setup>
import useToggleModal from "@/register-components/actionDialog";
import { useI18n } from "vue-i18n";
import { defineProps, ref, defineEmits } from "vue";
import { useStore } from "vuex";

const { toggleModel } = useToggleModal();
const { t } = useI18n();
const store = useStore();
const props = defineProps({
  data: Object,
});
const emit = defineEmits(["callback"]);
const teamA = ref(props.data?.teamA ?? "");
const teamB = ref(props.data?.teamB ?? "");
const note = ref(props.data?.note ?? "");
const id = ref(props.data?.bookingDetailId ?? "");

const save = () => {
  if (!teamA.value) {
    alert(t("TeamAEmptyMesg"));
    return;
  }

  let data = {
    id: id.value,
    teamA: teamA.value,
    teamB: teamB.value,
    note: note.value,
  };
  store.dispatch("bookingDetail/updateMatch", data);
  emit("callback", data);
  toggleModel();
};
</script>

<template>
  <Dialog :title="t('MatchInfoForm')" :width="500">
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
          {{ t("Note") }}
        </el-col>
        <el-col :span="17">
          <el-input v-model="note" maxlength="500" :rows="3" type="textarea" />
        </el-col>
      </el-form-item>
    </template>
    <template #foot>
      <span class="dialog-footer">
        <el-button @click="toggleModel">{{ t("Close") }}</el-button>
        <el-button type="primary" @click="save">{{ t("Save") }}</el-button>
      </span>
    </template>
  </Dialog>
</template>