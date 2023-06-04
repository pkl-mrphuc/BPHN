<script setup>
import { useI18n } from "vue-i18n"
import FootballFieldCard from '@/components/FootballFieldCard.vue'
import useToggleModal from '@/register-components/actionDialog'
import { ElLoading } from 'element-plus'
import { onMounted, inject, ref } from 'vue'
import { useStore } from 'vuex'

const { t } = useI18n()
const store = useStore()
const { openModal, hasRole } = useToggleModal()
const loadingOptions = inject('loadingOptions')
const pitchDataForm = ref(null)
const listPitch = ref([])

onMounted(() => {
  loadData()
})

const addNew = (() => {
  const loading = ElLoading.service(loadingOptions)
  store.dispatch('pitch/getInstance', '').then((res) => {
    if(res?.data?.data) {
      openModal('FootballFieldDialog')
      pitchDataForm.value = res.data.data
    }
    loading.close()
  })  
})

const loadData = (() => {
  const loading = ElLoading.service(loadingOptions)
  store.dispatch('pitch/getPaging', store.getters['account/getAccountId']).then((res) => {
    loading.close()
    listPitch.value = res?.data?.data??[]
  })
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
            v-for="item in listPitch"
            :key="item"
            :span="7"
            class="football-field-card"
            >
            <football-field-card
                :name="item.name"
                :status="item.status"
            ></football-field-card>
          </el-col>
        </el-row>
      </div>
    </div>
  </section>
  <FootballFieldDialog 
  v-if="hasRole('FootballFieldDialog')"
  :data="pitchDataForm"
  >
  </FootballFieldDialog>
</template>

<style scoped>
@import "@/assets/css/BPHNMyFootballFields.css";
</style>
