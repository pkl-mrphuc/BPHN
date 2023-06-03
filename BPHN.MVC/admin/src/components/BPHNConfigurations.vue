<script setup>
import { computed, ref, watch } from 'vue'
import { useI18n } from 'vue-i18n'
import { useStore } from 'vuex'

const store = useStore()
const { t } = useI18n()

const darkMode = ref(store.getters['config/getDarkMode'])

const language = ref(store.getters['config/getLanguage'])

const getDarkMode = computed(() => {
  return store.getters['config/getDarkMode']
})

const getLanguage = computed(() => {
  return store.getters['config/getLanguage']
})

watch(getDarkMode, (newValue) => {
  darkMode.value = newValue
})

watch(getLanguage, (newValue) => {
  language.value = newValue
})

const configData = [
  {
    name: t('DarkMode'),
    key: 'DarkMode',
  },
  {
    name: t('Language'),
    key: 'Language',
  }
]



const save = (() => {
  let configs = [
    {
      Key: 'DarkMode',
      Value: `${darkMode.value}`
    },
    {
      Key: 'Language',
      Value: language.value
    }
  ]
  store.dispatch('config/save', configs)
})
</script>

<template>
  <section class="pbhn-screen pbhn-config">
    <div class="container">
      <div class="head">
        <h3 class="head_title">{{ t('Configurations') }}</h3>
        <div class="head_toolbar">
            <el-button type="primary" @click="save">{{ t('Save') }}</el-button>
        </div>
      </div>
      <div class="body">
        <el-table :data="configData" style="width: 100%;">
          <el-table-column :label="t('Title')" width="200">
            <template #default="scope">
              <span>{{ scope.row.name }}</span>
            </template>
          </el-table-column>
          <el-table-column label="">
            <template #default="scope">
              <el-switch v-if="scope.row.key == 'DarkMode'" v-model="darkMode" />
              <el-select v-if="scope.row.key == 'Language'" v-model="language">
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
