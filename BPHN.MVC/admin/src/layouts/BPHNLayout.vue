<script setup>
import { ref, watch } from 'vue'

const store = useStore()
let darkMode = ref(store.getters['config/getDarkMode'])

const getDarkMode = computed(() => {
  return store.getters['config/getDarkMode']
})

watch(getDarkMode, (newValue) => {
  darkMode.value = newValue
})

const darkClass = computed(() => {
  return darkMode.value ? 'dark' : ''
})

</script>


<template>
  <div class="common-layout" :class="darkClass">
    <el-container>
      <el-header>
        <bphn-header></bphn-header>
      </el-header>
      <el-container>
        <el-aside width="200px" class="menu-layout">
          <bphn-menu></bphn-menu>
        </el-aside>
        <el-main class="main-layout">
          <router-view />
        </el-main>
      </el-container>
    </el-container>
  </div>
</template>

<script>
import BphnHeader from '@/layouts/BPHNHeader.vue'
import BphnMenu from '@/layouts/BPHNMenu.vue'
import AccountAPI from '@/apis/AccountAPI'
import { useStore } from 'vuex'
import { useI18n } from 'vue-i18n'
import { computed } from 'vue'
export default {
  name: "BPHNLayout",
  components: { BphnHeader, BphnMenu },
  async created() {
    const store = useStore()
    const t = useI18n()
    let validateResult = await AccountAPI.validateToken(store.getters['account/getToken'])
    if (validateResult?.data?.success) {
      let configs = localStorage.getItem('config-key')
      if(!configs || (!window['loadedConfig'] && configs)) {
        store.dispatch('config/loadConfig')
        window['loadedConfig'] = true
      }
      else {
        t.locale.value = store.getters['config/getLanguage']
      }
    }
    else {
      window.location = '/login'
    }
  }
}
</script>