<script setup>
import { computed, ref } from "vue"
import { Location } from "@element-plus/icons-vue"
import useToggleModal from '@/register-components/actionDialog'

const { toggleModel } = useToggleModal()

const configData = [
  {
    name: "Giá",
  },
  {
    name: "Bắt đầu lúc",
  },
  {
    name: "Kết thúc lúc",
  }
]

const status = ref('shanghai')
const name = ref('')
const address = ref('')
const quantity = ref('')
const minutesPerMatch = ref('')
const timeSlotPerDay = ref('')
const isShowConfigTimeSlotTable = computed(() => {
  if(timeSlotPerDay.value && !isNaN(Number(timeSlotPerDay.value))) return true
  return false 
})
</script>


<template>
  <Dialog :title="'Thêm mới sân bóng'" :width="900">
    <template #body>
      <el-form>
        <el-form-item>
          <el-select v-model="status" placeholder="Trạng thái hoạt động">
            <el-option label="Zone one" value="shanghai" />
            <el-option label="Zone two" value="beijing" />
          </el-select>
        </el-form-item>
        <el-form-item>
          <el-input v-model="name" placeholder="Tên sân" />
        </el-form-item>
        <el-form-item>
          <el-col :span="22">
            <el-input v-model="address" placeholder="Địa chỉ" />
          </el-col>
          <el-col :span="1">
            <el-icon><Location /></el-icon>
          </el-col>
        </el-form-item>
        <el-form-item>
          <el-col :span="7">
            <el-input v-model="quantity" placeholder="Số lượng sân" />
          </el-col>
          <el-col :span="7">
            <el-input v-model="minutesPerMatch" placeholder="Số phút / trận" />
          </el-col>
          <el-col :span="7">
            <el-input v-model="timeSlotPerDay" placeholder="Khung giờ / ngày" />
          </el-col>
        </el-form-item>
        <el-form-item v-if="isShowConfigTimeSlotTable">
          <el-table :data="configData" style="width: 100%">
            <el-table-column label="">
              <template #default="scope">
                <span>{{ scope.row.name }}</span>
              </template>
            </el-table-column>
            <el-table-column label="Khung 1"></el-table-column>
            <el-table-column label="Khung 2"> </el-table-column>
          </el-table>
        </el-form-item>
      </el-form>
    </template>
    <template #foot>
      <span class="dialog-footer">
        <el-button @click="toggleModel">Đóng</el-button>
        <el-button type="primary">Lưu</el-button>
      </span>
    </template>
  </Dialog>
</template>

