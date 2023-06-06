<script setup>
import { useI18n } from "vue-i18n";
import { ref } from "vue";
import useToggleModal from "@/register-components/actionDialog";

const { toggleModel } = useToggleModal();
const { t } = useI18n();
const isRecurring = ref(false);
const weekdays = ref("2");
const date = ref(new Date());
const fromDate = ref(new Date());
const toDate = ref(new Date(fromDate.value.getTime() + 100*24*60*60*1000));
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
          <el-col :span="11">
            <el-select style="width: 100%" placeholder="Sân">
              <el-option label="Đầm hồng" value="ACTIVE" />
              <el-option label="10 Đức thắng" value="INACTIVE" />
            </el-select>
          </el-col>
          <el-col :span="11">
            <el-select style="width: 100%" placeholder="Khung giờ">
              <el-option label="Đầm hồng" value="ACTIVE" />
              <el-option label="10 Đức thắng" value="INACTIVE" />
            </el-select>
          </el-col>
        </el-form-item>
        <el-form-item>
          <el-checkbox
            label="Đặt lịch cố định theo tuần"
            v-model="isRecurring"
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
          <el-date-picker type="date" placeholder="Ngày" style="width: 100%" v-model="date" />
        </el-form-item>
        <el-form-item v-if="isRecurring">
          <el-col :span="11">
            <el-date-picker
              type="date"
              placeholder="Từ ngày"
              style="width: 100%"
              v-model="fromDate"
            />
          </el-col>
          <el-col :span="11">
            <el-date-picker
              type="date"
              placeholder="Đến ngày"
              style="width: 100%"
              v-model="toDate"
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
.action-footer{
  display: flex;
  align-items: center;
  justify-content: space-between;
}
</style>