<script setup>
import { ref } from 'vue'

const mode = ref(window['DarkMode'])
const language = ref(window['Language'])

const configData = [
  {
    name: 'Nền tối',
    type: 'switch',
  },
  {
    name: 'Ngôn ngữ',
    type: 'select',
  }
]

function save() {
    window['LoadedConfig'] = false
    window.location.reload()
}
</script>

<template>
  <section class="pbhn-config">
    <div class="container">
      <div class="head">
        <h3 class="head_title">Cấu hình</h3>
        <div class="head_toolbar">
            <el-button type="primary" @click="save">Lưu</el-button>
        </div>
      </div>
      <div class="body">
        <el-table :data="configData" style="width: 100%">
          <el-table-column label="Tiêu đề" width="200">
            <template #default="scope">
              <span>{{ scope.row.name }}</span>
            </template>
          </el-table-column>
          <el-table-column label="">
            <template #default="scope">
              <el-switch v-if="scope.row.type == 'switch'" v-model="mode" />
              <el-select v-if="scope.row.type == 'select'" v-model="language">
                <el-option value="vn" label="Vietnamese" />
                <el-option value="en" label="English" />
              </el-select>
            </template>
          </el-table-column>
        </el-table>
      </div>
    </div>
  </section>
</template>

<style scoped>
@import "@/assets/css/BPHNConfig.css";
</style>
