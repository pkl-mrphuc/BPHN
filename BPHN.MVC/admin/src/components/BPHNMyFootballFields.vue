<script setup>
import { useI18n } from "vue-i18n"
import FootballFieldCard from '@/components/FootballFieldCard.vue'
import useToggleModal from '@/register-components/actionDialog'
import { ElLoading } from 'element-plus'
import { onMounted, inject } from 'vue'

const { t } = useI18n()
const { openModal, hasRole } = useToggleModal()
const loadingOptions = inject('loadingOptions')

onMounted(() => {
  loadData()
})

const addNew = (() => {
    openModal('FootballFieldDialog')
})

const loadData = (() => {
  const loading = ElLoading.service(loadingOptions)
  
  setTimeout(() => {
    loading.close()
  }, 2000)
})
</script>

<template>
  <section class="pbhn-screen pbhn-football-fields">
    <div class="container">
      <div class="head">
        <h3 class="head_title">{{ t("MyFootballFields") }}</h3>
        <div class="head_toolbar">
          <el-button type="primary" @click="addNew">{{ t("AddNew") }}</el-button>
        </div>
      </div>
      <div class="body">
        <el-row>
          <el-col
            v-for="o in 10"
            :key="o"
            :span="7"
            class="football-field-card"
            >
            <football-field-card
                :name="'Đầm hồng'"
                :status="'INACTIVE'"
            ></football-field-card>
          </el-col>
        </el-row>
      </div>
    </div>
  </section>
  <FootballFieldDialog v-if="hasRole('FootballFieldDialog')"></FootballFieldDialog>
</template>

<style scoped>
@import "@/assets/css/BPHNMyFootballFields.css";
</style>
