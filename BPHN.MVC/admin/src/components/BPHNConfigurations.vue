<script setup>
import { computed, ref, watch } from 'vue'
import { useI18n } from 'vue-i18n'
import { useStore } from 'vuex'

const store = useStore()
const { t } = useI18n()

let darkMode = ref(store.getters['config/getDarkMode'])

let language = ref(store.getters['config/getLanguage'])

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
    type: 'switch',
  },
  {
    name: t('Language'),
    type: 'select',
  }
]



function save() {
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
}
</script>

<template>
  <section class="pbhn-config">
    <div class="container">
      <div class="head">
        <h3 class="head_title">{{ t('Configurations') }}</h3>
        <div class="head_toolbar">
            <el-button type="primary" @click="save">{{ t('Save') }}</el-button>
        </div>
      </div>
      <div class="body">
        <el-table :data="configData" style="width: 100%;" class="body_table">
          <el-table-column :label="t('Title')" width="200">
            <template #default="scope">
              <span>{{ scope.row.name }}</span>
            </template>
          </el-table-column>
          <el-table-column label="">
            <template #default="scope">
              <el-switch v-if="scope.row.type == 'switch'" v-model="darkMode" />
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
