<script setup>
import { computed, ref, onMounted, nextTick } from "vue"
import { Location } from "@element-plus/icons-vue"
import useToggleModal from '@/register-components/actionDialog'

const { toggleModel } = useToggleModal()
const configData = [
  {
    name: "Giá (Nghìn đồng)",
    type: 'input'
  },
  {
    name: "Giờ vào",
    type: 'time-picker'
  },
  {
    name: "Giờ ra",
    type: 'time-picker'
  }
]

const status = ref('shanghai')
const name = ref('')
const address = ref('')

const quantity = ref(1)
const minutesPerMatch = ref(90)
const timeSlotPerDay = ref(1)

const lstTimeSlot = computed(() => {
  let result = []
  for (let i = 0; i < timeSlotPerDay.value; i++) {
    result.push({
      name: `Khung ${i+1}`
    })
  }
  return result
})

const maxTimeSlot = computed(() => {
  return 1440 / minutesPerMatch.value
})

onMounted(() => {
  nextTick(() => {
    document.getElementById('inpTimeSlot').setAttribute('readonly', true)
  })
})

</script>


<template>
  <Dialog :title="'Thêm mới sân bóng'" :width="1200">
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
          <el-col :span="8" class="center">
            <div>Số lượng sân</div>
            <el-input-number v-model="quantity" :min="1" :max="100" />
          </el-col>
          <el-col :span="8" class="center">
            <div>Số phút / trận</div>
            <el-input-number v-model="minutesPerMatch" :min="30" :max="1440" :step="30" />
          </el-col>
          <el-col :span="8" class="center">
            <div>Khung giờ / ngày</div>
            <el-input-number id="inpTimeSlot" v-model="timeSlotPerDay" :min="1" :max="maxTimeSlot" />
          </el-col>
        </el-form-item>
        <el-form-item>
          <el-table :data="configData" class="w100">
            <el-table-column label="" width="200">
              <template #default="scope">
                <span>{{ scope.row.name }}</span>
              </template>
            </el-table-column>

            <el-table-column v-for="item in lstTimeSlot" 
                              :key="item" 
                              :label="item.name"
                              :min-width="160">
              <template #default="scope">
                <el-input-number v-if="scope.row.type == 'input'" class="w100" :min="0" :step="100" />
                <el-time-picker v-if="scope.row.type == 'time-picker'" class="w100" />
              </template>
            </el-table-column>
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

<style scoped>
.center {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
}
.w100 {
  width: 100%;
}
</style>