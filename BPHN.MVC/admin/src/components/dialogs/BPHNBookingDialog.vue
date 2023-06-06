<script setup>
import { useI18n } from "vue-i18n";
import { ref, defineProps, onMounted } from "vue";
import useToggleModal from "@/register-components/actionDialog";
import useCommonFn from "@/commonFn";
import { useStore } from "vuex";

const { toggleModel } = useToggleModal();
const { dateNow, sameDate, date, yearEndDay } = useCommonFn();
const { t } = useI18n();
const store = useStore();
const props = defineProps({
  data: Object,
});

const isRecurring = ref(props.data?.isRecurring ?? false);
const weekdays = ref(null);
const fromDate = ref(new Date(props.data?.fromDate ?? dateNow()));
const toDate = ref(new Date(props.data?.toDate ?? dateNow()));
const listPitch = ref([]);
const listTimeFrame = ref([]);
const listDetail = ref([]);
const pitchId = ref(props.data?.pitchId ?? null);
const nameDetail = ref(props.data?.nameDetail ?? null);

const showMakeRecurring = () => {
  if (isRecurring.value) {
    weekdays.value = "2";
    fromDate.value = dateNow();
    toDate.value = yearEndDay(fromDate.value);
  } else {
    weekdays.value = null;
    toDate.value = fromDate.value;
  }
};

const changeDate = () => {
  fromDate.value = date(fromDate.value);
  toDate.value = date(toDate.value);
  if (sameDate(fromDate.value, toDate.value)) {
    weekdays.value = null;
    toDate.value = fromDate.value;
    isRecurring.value = false;
  }
};

const changePitchId = () => {
  let pitchSelected = listPitch.value.filter(
    (item) => item.id == pitchId.value
  );
  if (pitchSelected && pitchSelected.length > 0) {
    let pitch = pitchSelected[0];
    listDetail.value = pitch.nameDetails.split(";");
    listTimeFrame.value = pitch.timeFrameInfos;
  }
};

onMounted(() => {
  store
    .dispatch("pitch/getPaging", {
      accountId: store.getters["account/getAccountId"],
      hasDetail: true
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
            <el-input placeholder="SDT" />
          </el-col>
          <el-col :span="11">
            <el-input placeholder="Email" />
          </el-col>
        </el-form-item>
        <el-form-item>
          <el-col :span="7">
            <el-select
              style="width: 100%"
              placeholder="Cơ sở"
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
            <el-select style="width: 100%" placeholder="Khung giờ">
              <el-option
                v-for="item in listTimeFrame"
                :key="item"
                :label="item.name"
                :value="item.id"
              />
            </el-select>
          </el-col>
          <el-col :span="7">
            <el-select
              style="width: 100%"
              placeholder="Sân"
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
            label="Đặt lịch cố định theo tuần"
            v-model="isRecurring"
            @change="showMakeRecurring"
          />
        </el-form-item>
        <el-form-item v-if="isRecurring">
          <el-radio-group v-model="weekdays">
            <el-radio label="2">Thứ 2</el-radio>
            <el-radio label="3">Thứ 3</el-radio>
            <el-radio label="4">Thứ 4</el-radio>
            <el-radio label="5">Thứ 5</el-radio>
            <el-radio label="6">Thứ 6</el-radio>
            <el-radio label="7">Thứ 7</el-radio>
            <el-radio label="8">Chủ nhật</el-radio>
          </el-radio-group>
        </el-form-item>
        <el-form-item v-if="!isRecurring">
          <el-date-picker
            type="date"
            placeholder="Ngày"
            style="width: 100%"
            v-model="fromDate"
            @change="changeDate"
          />
        </el-form-item>
        <el-form-item v-if="isRecurring">
          <el-col :span="11">
            <el-date-picker
              type="date"
              placeholder="Từ ngày"
              style="width: 100%"
              v-model="fromDate"
              @change="changeDate"
            />
          </el-col>
          <el-col :span="11">
            <el-date-picker
              type="date"
              placeholder="Đến ngày"
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
          <el-button>Kiểm tra nhanh</el-button>
          <el-button>Tìm khung giờ trống</el-button>
        </span>
        <span class="dialog-footer">
          <el-button @click="toggleModel">{{ t("Close") }}</el-button>
          <el-button type="primary" @click="save">{{ t("Save") }}</el-button>
        </span>
      </div>
    </template>
  </Dialog>
</template>

<style scoped>
.action-footer {
  display: flex;
  align-items: center;
  justify-content: space-between;
}
</style>